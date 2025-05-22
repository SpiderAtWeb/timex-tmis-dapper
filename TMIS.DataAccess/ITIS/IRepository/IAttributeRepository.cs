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
    public interface IAttributeRepository
    {
        Task<CreateAttributeVM> LoadDropDowns();
        Task<IEnumerable<AttributeVM>> GetAllAsync();
        Task<bool> AddAsync(AttributeModel attribute, List<AttributeListOption>? attributeListOption);
        //Task UpdateAsync(DeviceType deviceType);
        //Task DeleteAsync(int id);
        Task<bool> CheckAttributeExist(string deviceType, string deviceTypeID);
    }
}
