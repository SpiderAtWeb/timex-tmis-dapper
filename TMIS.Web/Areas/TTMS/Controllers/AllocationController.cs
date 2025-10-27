using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using TMIS.DataAccess.TTMS.IRepository;
using TMIS.Models.TTMS;
using TMIS.Models.TTMS.VM;

namespace TMIS.Areas.TTMS.Controllers
{
  public class AllocationController(IDailyToursRepository repository) : Controller
  {
    private readonly IDailyToursRepository _repository = repository;

    [HttpGet]
    public async Task<IActionResult> VehicleEmployeeAllocation()
    {
      var viewModel = new VehicleAllocationViewModel
      {
        Vehicles = (await _repository.GetTransportVehiclesAsync()).ToList(),
        Destinations = (await _repository.GetEmployeeDestinationsAsync()).ToList()
      };
      return PartialView("_VehicleEmployeeAllocation", viewModel);
    }   
  }
}
