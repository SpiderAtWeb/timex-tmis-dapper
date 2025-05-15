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
        Task<DeviceType> GetByIdAsync(int id);
        Task<bool> AddAsync(DeviceType deviceType, IFormFile? image);
        Task UpdateAsync(DeviceType deviceType);
        Task DeleteAsync(int id);
        Task<bool> CheckDeviceTypeExist(string deviceType);
    }
}
