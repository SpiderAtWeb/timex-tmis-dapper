using log4net;
using Microsoft.AspNetCore.Mvc;
using TMIS.Areas.ITIS.Controllers;
using TMIS.Controllers;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.TTMS.IRepository;
using TMIS.Models.ITIS;
using TMIS.Models.TTMS;
using TMIS.Models.TTMS.VM;


namespace TMIS.Areas.TTMS.Controllers
{
  [Area("TTMS")]
  public class MasterDataController(IMasterDataRepository masterDataRepository, ISessionHelper sessionHelper) : BaseController
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(DeviceTypeController));
    private readonly IMasterDataRepository  _masterDataRepository = masterDataRepository;
    private readonly ISessionHelper _iSessionHelper = sessionHelper;
    public async Task<IActionResult> Employees()
    {
      _logger.Info("[" + _iSessionHelper.GetShortName() + "] - PAGE VISIT Employees");
      var employeeList = await _masterDataRepository.GetAllEmployee();
      return View(employeeList);
    }
    public IActionResult Vehicles()
    {
      return View();
    }
    public async Task<IActionResult> Drivers()
    {
      _logger.Info("[" + _iSessionHelper.GetShortName() + "] - PAGE VISIT Drivers");
      var driverList = await _masterDataRepository.GetAllDrivers();
      return View(driverList);
    }
    public IActionResult Owners()
    {
      return View();
    }
    public IActionResult PaymentTerms()
    {
      return View();
    }

    #region New Views
    public async Task<IActionResult> NewEmployee()
    {
      _logger.Info("[" + _iSessionHelper.GetShortName() + "] - PAGE VISIT NewEmployee");      
      var vm = new NewEmployeeViewModel();

      vm.DestinationList = await _masterDataRepository.LoadDestinations();
      vm.LocationList = await _masterDataRepository.LoadLoactions();

      return View(vm);
    }    
    public IActionResult NewVehicle()
    {
      return View();
    }
    public IActionResult NewDriver()
    {
      _logger.Info("[" + _iSessionHelper.GetShortName() + "] - PAGE VISIT NewDriver");
      var vm = new NewDriverViewModel();
     
      return View(vm);
    }
    public IActionResult NewOwner()
    {
      return View();
    }
    public IActionResult NewPaymentTerm()
    {
      return View();
    }
    #endregion

    #region Edit Views
    public async Task<IActionResult> EditEmployee(int employeeID)
    {
      var vm = new NewEmployeeViewModel();

      var Employee = await _masterDataRepository.LoadEmployee(employeeID);

      if (Employee == null)
      {
        return NotFound();
      }

      vm.DestinationList = await _masterDataRepository.LoadDestinations();
      vm.LocationList = await _masterDataRepository.LoadLoactions();
      vm.Employee = Employee;

      _logger.Info("[" + _iSessionHelper.GetShortName() + "] - PAGE VISIT Edit Employee [" + vm.Employee.EmployeeId + "]");      
                 
      return View(vm);            
    }
    public IActionResult EditVehicle()
    {
      return View();
    }
    public async Task<IActionResult> EditDriver(int driverID)
    {
      var vm = new NewDriverViewModel();

      var driver = await _masterDataRepository.LoadDriver(driverID);

      if (driver == null)
      {
        return NotFound();
      }

      vm.Driver = driver;

      _logger.Info("[" + _iSessionHelper.GetShortName() + "] - PAGE VISIT Edit Driver [" + vm.Driver.DriverId + "]");

      return View(vm);
    }
    public IActionResult EditOwner()
    {
      return View();
    }
    public IActionResult EditPaymentTerm()
    {
      return View();
    }
    #endregion

    #region Create Methods
    [HttpPost]
    public async Task<IActionResult> NewEmployee(NewEmployeeViewModel model)
    {
      model.DestinationList = await _masterDataRepository.LoadDestinations();
      model.LocationList = await _masterDataRepository.LoadLoactions();

      if (await _masterDataRepository.CheckEmployeeExist(model.Employee.EmployeeCode))
      {
        ModelState.AddModelError("Employee.EmployeeCode", "Employee Already Exists !");
      }
      if (model.Employee.LocationId == 0)
      {
        ModelState.AddModelError("Employee.LocationId", "Location field is required.");
      }
      if (model.Employee.DestinationId == 0)
      {
        ModelState.AddModelError("Employee.DestinationId", "Route  field is required.");
      }

      // Check if the ModelState is valid
      if (!ModelState.IsValid)
      {
        return View(model);
      }

      // Insert type data if everything is valid
      await _masterDataRepository.AddEmployeeAsync(model.Employee);

      // Show success message and redirect
      TempData["success"] = "Record Created Successfully";

      _logger.Info("EMPLOYEE CREATED [" + model.Employee.EmployeeName + "] - [" + _iSessionHelper.GetShortName() + "]");

      return RedirectToAction("Employees");
    }
    [HttpPost]
    public async Task<IActionResult> NewVehicle(NewEmployeeViewModel model)
    {
      if (await _masterDataRepository.CheckEmployeeExist(model.Employee.EmployeeCode))
      {
        ModelState.AddModelError("EmployeeCode", "Employee Already Exists !");
      }

      // Check if the ModelState is valid
      if (!ModelState.IsValid)
      {
        return View(model);
      }

      // Insert type data if everything is valid
      await _masterDataRepository.AddEmployeeAsync(model.Employee);

      // Show success message and redirect
      TempData["success"] = "Record Created Successfully";

      _logger.Info("VEHICLE CREATED [" + model.Employee.EmployeeName + "] - [" + _iSessionHelper.GetShortName() + "]");

      return RedirectToAction("Vehicles");
    }
    [HttpPost]
    public async Task<IActionResult> NewDriver(NewDriverViewModel model)
    {
      // Check if driver already exists (by NIC)
      if (await _masterDataRepository.CheckDriverExist(model.Driver.NIC))
      {
        ModelState.AddModelError("Driver.NIC", "Driver with this NIC already exists!");
      }

      // Validate required fields
      if (string.IsNullOrWhiteSpace(model.Driver.NIC))
      {
        ModelState.AddModelError("Driver.NIC", "NIC field is required.");
      }
      if (string.IsNullOrWhiteSpace(model.Driver.DriverName))
      {
        ModelState.AddModelError("Driver.DriverName", "Driver Name is required.");
      }
      if (string.IsNullOrWhiteSpace(model.Driver.LicenseNo))
      {
        ModelState.AddModelError("Driver.LicenseNo", "License Number is required.");
      }

      // Check if ModelState is valid
      if (!ModelState.IsValid)
      {
        return View(model);
      }

      // Insert driver record
      await _masterDataRepository.AddDriverAsync(model.Driver);

      // Show success message and redirect
      TempData["success"] = "Record Created Successfully";

      _logger.Info("DRIVER CREATED [" + model.Driver.DriverName + "] - [" + _iSessionHelper.GetShortName() + "]");

      return RedirectToAction("Drivers");
    }
    [HttpPost]
    public async Task<IActionResult> NewOwner(NewEmployeeViewModel model)
    {
      if (await _masterDataRepository.CheckEmployeeExist(model.Employee.EmployeeCode))
      {
        ModelState.AddModelError("EmployeeCode", "Employee Already Exists !");
      }

      // Check if the ModelState is valid
      if (!ModelState.IsValid)
      {
        return View(model);
      }

      // Insert type data if everything is valid
      await _masterDataRepository.AddEmployeeAsync(model.Employee);

      // Show success message and redirect
      TempData["success"] = "Record Created Successfully";

      _logger.Info("VEHICLE CREATED [" + model.Employee.EmployeeName + "] - [" + _iSessionHelper.GetShortName() + "]");

      return RedirectToAction("Vehicles");
    }
    [HttpPost]
    public async Task<IActionResult> NewPaymentTerm(NewEmployeeViewModel model)
    {
      if (await _masterDataRepository.CheckEmployeeExist(model.Employee.EmployeeCode))
      {
        ModelState.AddModelError("EmployeeCode", "Employee Already Exists !");
      }

      // Check if the ModelState is valid
      if (!ModelState.IsValid)
      {
        return View(model);
      }

      // Insert type data if everything is valid
      await _masterDataRepository.AddEmployeeAsync(model.Employee);

      // Show success message and redirect
      TempData["success"] = "Record Created Successfully";

      _logger.Info("VEHICLE CREATED [" + model.Employee.EmployeeName + "] - [" + _iSessionHelper.GetShortName() + "]");

      return RedirectToAction("Vehicles");
    }
    #endregion

    #region Edit Methods
    [HttpPost]
    public async Task<IActionResult> EditEmployee(NewEmployeeViewModel model)
    {
      model.DestinationList = await _masterDataRepository.LoadDestinations();
      model.LocationList = await _masterDataRepository.LoadLoactions();
      
      if (model.Employee.LocationId == 0)
      {
        ModelState.AddModelError("Employee.LocationId", "Location field is required.");
      }
      if (model.Employee.DestinationId == 0)
      {
        ModelState.AddModelError("Employee.DestinationId", "Route  field is required.");
      }

      // Check if the ModelState is valid
      if (!ModelState.IsValid)
      {
        return View(model);
      }

      // Insert type data if everything is valid
      await _masterDataRepository.UpdateEmployee(model.Employee);

      // Show success message and redirect
      TempData["success"] = "Record Updated Successfully";

      _logger.Info("EMPLOYEE EDIT [" + model.Employee.EmployeeId + "] - [" + _iSessionHelper.GetShortName() + "]");

      return RedirectToAction("Employees");
    }
    [HttpPost]
    public async Task<IActionResult> EditVehicle(NewEmployeeViewModel model)
    {
      if (await _masterDataRepository.CheckEmployeeExist(model.Employee.EmployeeCode))
      {
        ModelState.AddModelError("EmployeeCode", "Employee Already Exists !");
      }

      // Check if the ModelState is valid
      if (!ModelState.IsValid)
      {
        return View(model);
      }

      // Insert type data if everything is valid
      await _masterDataRepository.AddEmployeeAsync(model.Employee);

      // Show success message and redirect
      TempData["success"] = "Record Created Successfully";

      _logger.Info("VEHICLE CREATED [" + model.Employee.EmployeeName + "] - [" + _iSessionHelper.GetShortName() + "]");

      return RedirectToAction("Vehicles");
    }
    [HttpPost]
    public async Task<IActionResult> EditDriver(NewDriverViewModel model)
    {
      if (string.IsNullOrWhiteSpace(model.Driver.DriverName))
      {
        ModelState.AddModelError("Driver.DriverName", "Driver Name field is required.");
      }
      if (string.IsNullOrWhiteSpace(model.Driver.NIC))
      {
        ModelState.AddModelError("Driver.NIC", "NIC field is required.");
      }
      if (string.IsNullOrWhiteSpace(model.Driver.PhoneMobile))
      {
        ModelState.AddModelError("Driver.ContactNumber", "Contact Number field is required.");
      }

      // Check if the ModelState is valid
      if (!ModelState.IsValid)
      {
        return View(model);
      }

      // Update driver record if everything is valid
      await _masterDataRepository.UpdateDriver(model.Driver);

      // Show success message and redirect
      TempData["success"] = "Record Updated Successfully";

      _logger.Info("DRIVER EDIT [" + model.Driver.DriverId + "] - [" + _iSessionHelper.GetShortName() + "]");

      return RedirectToAction("Drivers");
    }
    [HttpPost]
    public async Task<IActionResult> EditOwner(NewEmployeeViewModel model)
    {
      if (await _masterDataRepository.CheckEmployeeExist(model.Employee.EmployeeCode))
      {
        ModelState.AddModelError("EmployeeCode", "Employee Already Exists !");
      }

      // Check if the ModelState is valid
      if (!ModelState.IsValid)
      {
        return View(model);
      }

      // Insert type data if everything is valid
      await _masterDataRepository.AddEmployeeAsync(model.Employee);

      // Show success message and redirect
      TempData["success"] = "Record Created Successfully";

      _logger.Info("VEHICLE CREATED [" + model.Employee.EmployeeName + "] - [" + _iSessionHelper.GetShortName() + "]");

      return RedirectToAction("Vehicles");
    }
    [HttpPost]
    public async Task<IActionResult> EditPaymentTerm(NewEmployeeViewModel model)
    {
      if (await _masterDataRepository.CheckEmployeeExist(model.Employee.EmployeeCode))
      {
        ModelState.AddModelError("EmployeeCode", "Employee Already Exists !");
      }

      // Check if the ModelState is valid
      if (!ModelState.IsValid)
      {
        return View(model);
      }

      // Insert type data if everything is valid
      await _masterDataRepository.AddEmployeeAsync(model.Employee);

      // Show success message and redirect
      TempData["success"] = "Record Created Successfully";

      _logger.Info("VEHICLE CREATED [" + model.Employee.EmployeeName + "] - [" + _iSessionHelper.GetShortName() + "]");

      return RedirectToAction("Vehicles");
    }
    #endregion  
  }
}
