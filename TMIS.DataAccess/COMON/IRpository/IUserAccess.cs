using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMIS.Models.Auth;

namespace TMIS.DataAccess.COMON.IRpository
{
    public interface IUserAccess
    {
        public Task<User> GetUserByUsernameAsync(string username, string password);

        public Task<bool> UpdateUserPassword(string oldPassword, string newPassword);
    }
}
