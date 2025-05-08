using Microsoft.AspNetCore.Mvc.Rendering;

namespace TMIS.DataAccess.COMON.IRpository
{
    public interface IUserControls
    {
        Task<IEnumerable<SelectListItem>> LoadDropDownsAsync(string tableName);
    }
}
