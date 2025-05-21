using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using TMIS.Models.ITIS.VM;

namespace TMIS.DataAccess.ITIS.IRepository
{
    public interface ICommonList
    {
        Task<IEnumerable<SelectListItem>> LoadAttributeTypes();
        Task<IEnumerable<SelectListItem>> LoadDeviceTypes();
        Task<IEnumerable<SelectListItem>> LoadLocations();    
        Task<IEnumerable<SelectListItem>> LoadDeviceStatus();
        Task<IEnumerable<SelectListItem>> LoadVendors();
        Task<IEnumerable<SelectListItem>> LoadInstoreSerialList();
        Task<IEnumerable<SelectListItem>> LoadEmployeeList();
        Task<DeviceDetailVM> LoadDeviceDetail(int deviceID);
        Task<IEnumerable<SelectListItem>> LoadInUseSerialList();
        Task<DeviceUserDetailVM> LoadUserDetail(int deviceID);      
    }
}
