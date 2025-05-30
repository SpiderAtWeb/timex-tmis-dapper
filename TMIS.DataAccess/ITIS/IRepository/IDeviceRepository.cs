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
    public interface IDeviceRepository
    {
        Task<CreateDeviceVM> LoadDropDowns(int? deviceID);
        Task<IEnumerable<Device>> GetAllAsync();
        Task<CreateDeviceVM> GetAllAttributes(int deviceTypeID);     
        Task<bool> AddAsync(CreateDeviceVM objCreateDevice, IFormFile? image1, IFormFile? image2, IFormFile? image3, IFormFile? image4);
        Task<bool> UpdateDevice(CreateDeviceVM objCreateDevice, IFormFile? image1, IFormFile? image2, IFormFile? image3, IFormFile? image4);
        Task<bool> CheckSerialNumberExist(string serialNumber);
        Task<DeviceDetailVM> LoadDeviceDetail(int deviceID);
        Task<DeviceUserDetailVM> LoadUserDetail(int deviceID);
        Task<bool> CheckSerialEdit(string serialNumber, int deviceID);
    }
}
