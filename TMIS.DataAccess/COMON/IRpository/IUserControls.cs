using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace TMIS.DataAccess.COMON.IRpository
{
    public interface IUserControls
    {
        Task<IEnumerable<SelectListItem>> LoadDropDownsAsync(string tableName);

        public Task<string> GenerateRefAsync(IDbConnection connection, IDbTransaction transaction, string tableName, string gpType);


        public Task<string> GenerateSeqRefAsync(IDbConnection connection, IDbTransaction transaction, string tableName, string codeType);
    }
}
