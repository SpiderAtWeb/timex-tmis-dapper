using Microsoft.AspNetCore.Mvc;
using TMIS.DataAccess.TTMS.IRepository;
using TMIS.Models.TTMS;
using TMIS.Models.TTMS.VM;


namespace TMIS.Areas.TTMS.Controllers
{
  [Area("TTMS")]
  public class DailyToursController(IDailyToursRepository repository) : Controller
  {
    private readonly IDailyToursRepository _repository = repository;

    public async Task<IActionResult> Index()
    {
      var viewModel = new DailyTourViewModel
      {
        Vehicles = (await _repository.GetTransportVehiclesAsync()).ToList()
      };
      return View(viewModel);
    }

    [HttpGet]
    public async Task<JsonResult> GetEmployeeAllocationData()
    {
      var destinations = await _repository.GetEmployeeDestinationsAsync();
      var vehicles = await _repository.GetTransportVehiclesAsync();

      return Json(new
      {
        destinations = destinations,
        vehicles = vehicles
      });
    }

    [HttpGet]
    public async Task<JsonResult> GetPaymentAllocationData()
    {
      var vehicles = await _repository.GetTransportVehiclesAsync();
      var paymentMethods = await _repository.GetPaymentMethodsAsync();

      return Json(new
      {
        vehicles = vehicles,
        paymentMethods = paymentMethods
      });
    }

    [HttpGet]
    public async Task<JsonResult> GetEmployeesByDestination(int destinationId)
    {
      var employees = await _repository.GetEmployeesByDestinationAsync(destinationId);
      return Json(employees);
    }
    [HttpPost]
    public async Task<JsonResult> SaveVehicleEmployeeAllocation([FromBody] VehicleAllocationViewModel model)
    {
      try
      {
        if (model.SelectedEmployeeIds != null && model.SelectedEmployeeIds.Any())
        {
          foreach (var employeeId in model.SelectedEmployeeIds)
          {
            var allocation = new VehicleEmployeeAllocation
            {
              VehicleId = model.VehicleId,
              EmployeeId = employeeId,
              AllocationDate = DateTime.Today,
              IsActive = true,
              CreatedBy = User.Identity.Name ?? "System"
            };
            await _repository.SaveVehicleEmployeeAllocationAsync(allocation);
          }

          return Json(new { success = true, message = "Employee allocation saved successfully!" });
        }

        return Json(new { success = false, message = "Please select at least one employee." });
      }
      catch (Exception ex)
      {
        return Json(new { success = false, message = "Error saving allocation: " + ex.Message });
      }
    }

    [HttpPost]
    public async Task<JsonResult> SaveVehiclePaymentAllocation([FromBody] VehiclePaymentAllocationViewModel model)
    {
      try
      {
        var paymentMethod = new VehiclePaymentMethod
        {
          VehicleId = model.VehicleId,
          PaymentMethodId = model.PaymentMethodId,
          Cost = model.Cost,
          EffectiveDate = model.EffectiveDate,
          IsActive = true
        };

        await _repository.SaveVehiclePaymentMethodAsync(paymentMethod);
        return Json(new { success = true, message = "Payment method allocated successfully!" });
      }
      catch (Exception ex)
      {
        return Json(new { success = false, message = "Error saving payment allocation: " + ex.Message });
      }
    }

    [HttpGet]
    public async Task<JsonResult> GetEmployeesByVehicle(int vehicleId, int month, int year)
    {
      var employees = await _repository.GetAllocatedEmployeesByVehicleAsync(vehicleId);
      var existingTours = await _repository.GetDailyToursAsync(vehicleId, month, year);
      var monthDays = GetMonthDays(year, month);

      var employeeTours = employees.Select(e => new EmployeeTour
      {
        EmployeeId = e.EmployeeId,
        EmployeeCode = e.EmployeeCode,
        EmployeeName = e.EmployeeName,
        Attendance = monthDays.ToDictionary(
              day => day,
              day => existingTours.Any(t =>
                  t.EmployeeId == e.EmployeeId &&
                  t.TourDate.Date == day.Date &&
                  t.IsPresent)
          )
      }).ToList();

      return Json(new { employees = employeeTours, monthDays });
    }

    [HttpPost]
    public async Task<JsonResult> SaveDailyTours(DailyTourViewModel model)
    {
      try
      {
        var dailyTours = new List<DailyTour>();
        var monthDays = GetMonthDays(model.Year, model.Month);

        foreach (var employee in model.Employees)
        {
          if (employee.Attendance == null)
            continue;

          foreach (var kvp in employee.Attendance)
          {
            // Only save checked (true) days
            if (!kvp.Value)
            {
              dailyTours.Add(new DailyTour
              {
                VehicleId = model.VehicleId,
                EmployeeId = employee.EmployeeId,
                TourDate = kvp.Key, // Key is date string
                IsPresent = true,
                CreatedBy = User.Identity?.Name ?? "System"
              });
            }
          }
        }

        await _repository.SaveDailyToursAsync(dailyTours);
        return Json(new { success = true, message = "Daily tours saved successfully!" });
      }
      catch (Exception ex)
      {
        return Json(new { success = false, message = "Error saving daily tours: " + ex.Message });
      }
    }

    private List<DateTime> GetMonthDays(int year, int month)
    {
      var daysInMonth = DateTime.DaysInMonth(year, month);
      return Enumerable.Range(1, daysInMonth)
          .Select(day => new DateTime(year, month, day))
          .ToList();
    }

    [HttpGet]
    public async Task<JsonResult> GetEmployeesWithAllocationStatus(int destinationId, int? vehicleId)
    {
      try
      {
        var employees = await _repository.GetEmployeesByDestinationAsync(destinationId);

        if (vehicleId.HasValue)
        {
          var allocatedEmployees = await _repository.GetAllocatedEmployeesByVehicleAsync(vehicleId.Value);
          var allocatedEmployeeIds = allocatedEmployees.Select(e => e.EmployeeId).ToHashSet();

          var result = employees.Select(e => new
          {
            e.EmployeeId,
            e.EmployeeCode,
            e.EmployeeName,
            isAllocated = allocatedEmployeeIds.Contains(e.EmployeeId)
          });

          return Json(new { employees = result });
        }
        else
        {
          var result = employees.Select(e => new
          {
            e.EmployeeId,
            e.EmployeeCode,
            e.EmployeeName,
            isAllocated = false
          });

          return Json(new { employees = result });
        }
      }
      catch (Exception ex)
      {
        return Json(new { error = ex.Message });
      }
    }
  }
}
