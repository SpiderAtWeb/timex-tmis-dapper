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
using TMIS.Models.TAPS;

namespace TMIS.DataAccess.TAPS.Repository
{
    public class AdminRepository(IDatabaseConnectionAdm dbConnection, IITISLogdb iITISLogdb) : IAdminRepository
    {
        private readonly IDatabaseConnectionAdm _dbConnection = dbConnection;
        private readonly IITISLogdb _iITISLogdb = iITISLogdb;

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
    }
}
