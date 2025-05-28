using Dapper;
using Microsoft.AspNetCore.Http;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.Models.Auth;

namespace TMIS.DataAccess.COMON.Rpository
{
    public class UserAccess(IDatabaseConnectionAdm dbConnection, ISessionHelper sessionHelper) : IUserAccess
    {
        private readonly IDatabaseConnectionAdm _dbConnection = dbConnection;
        private readonly ISessionHelper _iSessionHelper = sessionHelper;

        public async Task<User> GetUserByUsernameAsync(string username, string password)
        {
            using var connection = _dbConnection.GetConnection();

            // Check User Existence and Get User Details
            var _sqlUserDetails = @"SELECT Id, UserShortName FROM _MasterUsers
               WHERE (UserEmail = @Username) AND (UserPassword = @Password) AND (IsActive = 1)";

            var user = await connection.QueryFirstOrDefaultAsync<User>(_sqlUserDetails, new { Username = username, Password = password });

            if (user == null)
                return new User();

            // Get User Access Locations
            var sqlUserLocList = @"SELECT LocationId FROM _TrPermissionLocation WHERE (UserId = @UserId)";

            var _userLocationList = await connection.QueryAsync<int>(sqlUserLocList, new { UserId = user.Id });
            user.UserLocationList = _userLocationList.ToArray();

            // Get User Roles
            var sqlUserRolesList = @"SELECT  _MasterUserRoles.UserRole
            FROM            _TrPermissionRoles INNER JOIN
            _MasterUserRoles ON _TrPermissionRoles.UserRoleId = _MasterUserRoles.Id
            WHERE        (_TrPermissionRoles.UserId =  @UserId)";

            var _userRolesList = await connection.QueryAsync<string>(sqlUserRolesList, new { UserId = user.Id });
            user.UserRolesList = _userRolesList.ToArray();         

            _iSessionHelper.SetUserSession(user.Id.ToString(), user.UserShortName, user.UserRolesList, user.UserLocationList);
            return user;
        }

        public async Task<bool> UpdateUserPassword(string oldPassword, string newPassword)
        {
            using var connection = _dbConnection.GetConnection();
            var queryCheckOldPassword = "SELECT [UserPassword] FROM [_MasterUsers] WHERE [Id] = @UserId";

            var storedPassword = await connection.QueryFirstOrDefaultAsync<string>(queryCheckOldPassword, new { UserId = _iSessionHelper.GetUserId() });

            if (storedPassword == null)
            {
                return false;
            }

            if (storedPassword != oldPassword)
            {
                return false;
            }

            var queryUpdatePassword = "UPDATE [_MasterUsers] SET [UserPassword] = @NewPassword WHERE [Id] = @UserId";
            var result = await connection.ExecuteAsync(queryUpdatePassword, new { UserId = _iSessionHelper.GetUserId(), NewPassword = newPassword });

            return result > 0;
        }
    }
}
