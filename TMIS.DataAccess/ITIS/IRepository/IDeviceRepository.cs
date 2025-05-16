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
        Task<CreateDeviceVM> LoadDropDowns();
        Task<IEnumerable<Device>> GetAllAsync();
        Task<CreateDeviceVM> GetAllAttributes(int deviceTypeID);     
        Task<bool> AddAsync(CreateDeviceVM objCreateDevice, IFormFile? image1, IFormFile? image2, IFormFile? image3, IFormFile? image4);
    }
}
