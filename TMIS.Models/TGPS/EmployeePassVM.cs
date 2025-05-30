using Microsoft.AspNetCore.Mvc.Rendering;

namespace TMIS.Models.TGPS
{
    public class EmployeePassVM
    {
        public IEnumerable<SelectListItem>? GuardRooms { get; set; }
        public IEnumerable<SelectListItem>? ApprovEmps { get; set; }
        public EmployeePass EmployeePass { get; set; }

        public EmployeePassVM()
        {
            EmployeePass = new EmployeePass();
        }
    }
}