using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.ITIS.IRepository;
using TMIS.DataAccess.TAPS.IRepository;
using TMIS.Models.Auth;
using TMIS.Models.ITIS;
using TMIS.Models.TAPS;
using TMIS.Models.TAPS.VM;

namespace TMIS.DataAccess.TAPS.Repository
{
    public class AdminRepository(IDatabaseConnectionAdm dbConnection, IDatabaseConnectionSys dbConnectionSys, ITAPSLogdbRepository iTAPSLogdb) : IAdminRepository
    {
        private readonly IDatabaseConnectionAdm _dbConnection = dbConnection;
        private readonly IDatabaseConnectionSys _dbConnectionSys = dbConnectionSys;
        private readonly ITAPSLogdbRepository _iTAPSLogdb = iTAPSLogdb;

        public async Task<IEnumerable<SelectListItem>> LoadUsers()
        {
            string query = @"select ID as Value, UserEmail AS Text from _MasterUsers
                            ORDER BY Text";
            var results = await _dbConnection.GetConnection().QueryAsync<SelectListItem>(query);
            return results;
        }
        public async Task<IEnumerable<SelectListItem>> LoadUserRoles()
        {
            string query = @"select ID as Value, UserRole AS Text from _MasterUserRoles 
                            ORDER BY Text";
            var results = await _dbConnection.GetConnection().QueryAsync<SelectListItem>(query);
            return results;
        }
        public async Task<IEnumerable<UserRole>> LoadUserRole(int UserId)
        {
            string query = @"select t.UserId, t.UserRoleId, u.UserEmail, r.UserRole as UserRoleDesc from _TrPermissionRoles as t left join _MasterUsers as u on u.Id=t.UserId
                            left join _MasterUserRoles as r on r.Id=t.UserRoleId where t.UserId=@UserId";

            var result = await _dbConnection.GetConnection().QueryAsync<UserRole>(query, new { UserId });

            return result;
        }

        public async Task<bool> CheckRoleExistToUser(int userID, int roleID)
        {
            string query = @"SELECT COUNT(*) FROM _TrPermissionRoles WHERE UserId = @UserId AND UserRoleId = @UserRoleId;";

            int count = await _dbConnection.GetConnection().ExecuteScalarAsync<int>(query, new
            {
                UserId = userID,
                UserRoleId = roleID
            });
            return count > 0;
        }
        public async Task<bool> AssignUserRole(int userID, int roleID)
        {
            string query = @"INSERT INTO _TrPermissionRoles (UserId, UserRoleId)
                            VALUES (@UserId,@UserRoleId);";

            int rowAfected = await _dbConnection.GetConnection().ExecuteAsync(query, new
            {
                UserId = userID,
                UserRoleId = roleID
            });

            return rowAfected > 0;
        }

        public void DeleteUserRole(int userID, int roleID)
        {
            string query = @"DELETE FROM _TrPermissionRoles WHERE UserId = @UserId AND UserRoleId = @UserRoleId;";
            int row = _dbConnection.GetConnection().Execute(query, new
            {
                UserId = userID,
                UserRoleId = roleID
            });

            if (row > 0) 
            {
                TAPSLogdb logdb = new()
                {
                    TrObjectId = userID,
                    TrLog = $"Unassigned role from user. User ID: {userID}, Role ID: {roleID}, Status: DELETED"

                };

                _iTAPSLogdb.InsertLog(logdb);
            }

        }

        public async Task<IEnumerable<SelectListItem>> LoadEmployeeList()
        {
            string query = @"select EmpEmail as Value, EmpEmail AS Text from ITIS_MasterADEMPLOYEES where IsDelete=0";
            //replace with real datasource
            var results = await _dbConnectionSys.GetConnection().QueryAsync<SelectListItem>(query);
            return results;
        }

        public async Task<bool> CheckUserEmailExist(string userEmail)
        {
            string query = @"SELECT COUNT(*) FROM _MasterUsers WHERE UserEmail = @UserEmail and IsActive=1;";
            int count = await _dbConnection.GetConnection().ExecuteScalarAsync<int>(query, new { UserEmail = userEmail });
            return count > 0;
        }

        public async Task<bool> InsertNewUser(NewUserVM newUserVM)
        {
            string query = @"INSERT INTO _MasterUsers (UserEmail, UserPassword, UserShortName, IsActive, DefLocId)
                            VALUES (@UserEmail, @UserPassword, @UserShortName, 1, @DefLocId);
                            SELECT CAST(SCOPE_IDENTITY() AS INT) AS InsertedId;";
            //_TrPermissionLocation insert to this table
            var insertedId = await _dbConnection.GetConnection().QuerySingleOrDefaultAsync<int?>(query, new
            {
                UserEmail = newUserVM.UserEmail,
                UserPassword = newUserVM.UserPassword,
                UserShortName = newUserVM.UserShortName,
                DefLocId = newUserVM.Location
            });

            if (insertedId.HasValue)
            {
                TAPSLogdb logdb = new()
                {
                    TrObjectId = insertedId.Value,
                    TrLog = "NEW USER CREATED"

                };

                _iTAPSLogdb.InsertLog(logdb);
            }
            return insertedId > 0;
        }
        public async Task<IEnumerable<SelectListItem>> LoadLocationList()
        {
            string query = @"select Id as Value, PropName AS Text from COMN_VwTwoCompLocs";
            //replace with real datasource
            var results = await _dbConnectionSys.GetConnection().QueryAsync<SelectListItem>(query);
            return results;
        }
    }
}
