using Microsoft.AspNetCore.Http;
using TMIS.Models.SMIS;
using TMIS.Models.SMIS.VM;

namespace TMIS.DataAccess.SMIM.IRpository
{
    public interface IInventory
    {
        Task<MachinesVM> GetList();

        Task<McCreateVM> LoadInventoryDropDowns(int? id);

        Task<McCreatedRnVM> LoadRentInventoryDropDowns(int? id);

        Task LoadOwnedMachineListsAsync(McCreateVM mcCreateVM);

        Task LoadRentedMachineListsAsync(McCreatedRnVM mcCreatedRnVM);

        Task<bool> CheckQrAlreadyAvailable(string qrCode);

        Task<bool> CheckSnAlreadyAvailable(string serialNo);

        Task<bool> InsertMachineAsync(McInventory mcInventory, IFormFile? imageFront, IFormFile? imageBack);

        Task<int> UpdateOwnedMachineAsync(McInventory mcInventory, IFormFile? imageFront, IFormFile? imageBack);

        Task<McInventory?> GetMcInventoryByIdAsync(int? id);

        Task<bool> InsertRentMachineAsync(McInventory mcInventory, IFormFile? imageFront, IFormFile? imageBack, IFormFile? dispatchNote, IFormFile? returnGatePass);

        Task<int> UpdateRentMachineAsync(McInventory mcInventory, IFormFile? imageFront, IFormFile? imageBack, IFormFile? dispatchNote, IFormFile? returnGatePass);

        Task<McInventory?> GetRentMcInventoryByIdAsync(int? id);

        Task<MachineOwnedVM?> GetOwnedMcById(int id);

        Task<MachineRentedVM?> GetRentedMcById(int id);

        Task<byte[]?> GetPdfByIdAsync(int id, int docType);



    }
}
