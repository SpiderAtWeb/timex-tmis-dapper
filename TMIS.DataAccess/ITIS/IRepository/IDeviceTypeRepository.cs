using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TMIS.Models.ITIS;

namespace TMIS.DataAccess.ITIS.IRepository
{
    public interface IDeviceTypeRepository
    {
        Task<IEnumerable<DeviceType>> GetAllAsync();
        Task<bool> AddAsync(DeviceType deviceType, IFormFile? image);
        Task<bool> UpdateDeviceType(DeviceType deviceType, IFormFile? image);
        Task<bool> CheckDeviceTypeExist(string deviceType);
        Task<bool> CheckDeviceTypeExist(DeviceType obj);
        Task<DeviceType?> LoadDeviceType(int id);
    }
}
