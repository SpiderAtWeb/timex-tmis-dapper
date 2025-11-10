using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMIS.Models.TTMS;
using TMIS.Models.TTMS.VM;

namespace TMIS.DataAccess.TTMS.IRepository
{
    public interface IMasterDataRepository
    {
        // ------------------ EMPLOYEE METHODS ------------------
        Task<bool> AddEmployeeAsync(Employee employee);
        Task<IEnumerable<EmployeeViewModel>> GetAllEmployee();        
        Task<bool> UpdateEmployee(Employee employee);
        Task<bool> CheckEmployeeExist(string employeeCode);                
        Task<Employee?> LoadEmployee(int employeeid);

        // ------------------ DRIVER METHODS ------------------
        Task<bool> AddDriverAsync(Driver driver);
        Task<IEnumerable<DriverViewModel>> GetAllDrivers();
        Task<bool> UpdateDriver(Driver driver);
        Task<bool> CheckDriverExist(string nic);
        Task<Driver?> LoadDriver(int driverId);

        /// ------------------ PAYMENT TERM METHODS ------------------
        Task<bool> AddPaymentTermAsync(PaymentTerm paymentTerm);
        Task<IEnumerable<PaymentTermViewModel>> GetAllPaymentTerms();
        Task<bool> UpdatePaymentTerm(PaymentTerm paymentTerm);
        Task<bool> CheckPaymentTermExist(string paymentTermName);
        Task<PaymentTerm?> LoadPaymentTerm(int paymentTermId);

        /// ------------------ TRANSPORTER METHODS ------------------
        Task<bool> AddTransporterAsync(Transporter transporter);
        Task<IEnumerable<TransporterViewModel>> GetAllTransporters();
        Task<bool> UpdateTransporter(Transporter transporter);
        Task<bool> CheckTransporterExist(string nic);
        Task<Transporter?> LoadTransporter(int transporterId);

        // ------------------ DROPDOWN METHODS ------------------
        Task<IEnumerable<SelectListItem>> LoadDestinations();
        Task<IEnumerable<SelectListItem>> LoadLoactions();
    }
}
