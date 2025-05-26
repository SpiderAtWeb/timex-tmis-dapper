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
            var sqlUserDetails = @"
                SELECT Id, NameWi, UserRole 
                FROM VwUsers 
                WHERE UserName = @Username AND Password = @Password";

            var sqlAccessPlants = @"
                SELECT dbo.MasterUnits.TmisId
                FROM dbo.TrUsersUnits
                INNER JOIN dbo.MasterUnits ON dbo.TrUsersUnits.UnitId = dbo.MasterUnits.Id
                WHERE dbo.TrUsersUnits.UserId = (
                    SELECT Id FROM VwUsers WHERE UserName = @Username AND Password = @Password
                )";


            using var connection = _dbConnection.GetConnection();
            var user = await connection.QueryFirstOrDefaultAsync<User>(sqlUserDetails, new { Username = username, Password = password });

            if (user == null)
                return new User();

            var accessPlants = await connection.QueryAsync<int>(sqlAccessPlants, new { Username = username, Password = password });
            user.AccessPlants = accessPlants.ToArray();

            _iSessionHelper.SetUserSession(user.Id.ToString(), user.NameWi, user.UserRole, user.AccessPlants);

            return user;
        }

        public async Task<bool> UpdateUserPassword(string oldPassword, string newPassword)
        {
            using var connection = _dbConnection.GetConnection();
            var queryCheckOldPassword = "SELECT [Password] FROM [ADMIN].[dbo].[MasterUsers] WHERE [Id] = @UserId";

            var storedPassword = await connection.QueryFirstOrDefaultAsync<string>(queryCheckOldPassword, new { UserId = _iSessionHelper.GetUserId() });

            if (storedPassword == null)
            {
                return false;
            }

            if (storedPassword != oldPassword)
            {
                return false;
            }

            var queryUpdatePassword = "UPDATE [ADMIN].[dbo].[MasterUsers] SET [Password] = @NewPassword WHERE [Id] = @UserId";
            var result = await connection.ExecuteAsync(queryUpdatePassword, new { UserId = _iSessionHelper.GetUserId(), NewPassword = newPassword });

            return result > 0;
        }
    }
}
