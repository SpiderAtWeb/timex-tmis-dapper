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
        Task<CreateAttributeVM> LoadDropDowns(int? id);
        Task<IEnumerable<AttributeVM>> GetAllAsync();
        Task<bool> AddAsync(AttributeModel attribute, List<AttributeListOption>? attributeListOption);       
        Task<bool> CheckAttributeExist(string deviceType, string deviceTypeID);
        Task<bool> CheckAttributeExist(CreateAttributeVM obj);
        Task<bool> UpdateAttribute(AttributeModel attribute, List<AttributeListOption>? attributeListOption);
        Task<bool> DeleteAttribute(AttributeModel attribute);
    }
}
