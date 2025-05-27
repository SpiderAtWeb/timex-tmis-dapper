using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TMIS.Models.ITIS;
using TMIS.Models.ITIS.VM;

namespace TMIS.DataAccess.ITIS.IRepository
{
    public interface IDeviceUserRepository
    {
        Task<IEnumerable<DeviceUserVM>> GetAllAsync();
        Task<CreateDeviceUserVM> LoadDropDowns();
        Task<DeviceDetailVM> LoadDeviceDetail(int deviceID);
        Task<bool> AddAsync(CreateDeviceUserVM obj);
        Task<ReturnDeviceVM> LoadInUseDevices();
        Task<DeviceUserDetailVM> LoadUserDetail(int deviceID);
        Task<bool> ReturnDevice(ReturnDeviceVM obj, IFormFile? image);
    }
}
