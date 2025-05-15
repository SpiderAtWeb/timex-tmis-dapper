using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TMIS.Models.ITIS;

namespace TMIS.DataAccess.ITIS.IRepository
{
    public interface IDeviceRepository
    {
        Task<IEnumerable<Device>> GetAllAsync();
       // Task<bool> AddAsync(Device device, IFormFile? image1, IFormFile? image2, IFormFile? image3, IFormFile? image4);
    }
}
