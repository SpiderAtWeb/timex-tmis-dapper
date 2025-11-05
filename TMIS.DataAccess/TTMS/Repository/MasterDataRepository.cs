using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.TTMS.IRepository;
using TMIS.Models.TTMS;
using TMIS.Models.TTMS.VM;

namespace TMIS.DataAccess.TTMS.Repository
{
    public class MasterDataRepository(IDatabaseConnectionSys dbConnection, ITTMSLogdbRepository iTTMSLogdb) : IMasterDataRepository
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly ITTMSLogdbRepository _iTTMSLogdbRepository = iTTMSLogdb;

        #region Employee Methods

        public async Task<bool> AddEmployeeAsync(Employee employee)
        {
            const string query = @"
        INSERT INTO TTMS_Employees
        (EmployeeCode, EmployeeName, DestinationId, LocationId)
        VALUES
        (@EmployeeCode, @EmployeeName, @DestinationId, @LocationId);
        SELECT CAST(SCOPE_IDENTITY() AS INT) AS InsertedId;";

            try
            {
                var insertedId = await _dbConnection.GetConnection().QuerySingleOrDefaultAsync<int?>(query, new
                {
                    employee.EmployeeCode,
                    employee.EmployeeName,
                    employee.DestinationId,                    
                    employee.LocationId
                });

                if (insertedId.HasValue)
                {
                    LogdbTTMS logdb = new()
                    {
                        TrObjectId = insertedId.Value,
                        TrLog = "EMPLOYEE CREATED"
                    };

                    _iTTMSLogdbRepository.InsertLog(_dbConnection, logdb);
                }

                return insertedId > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IEnumerable<EmployeeViewModel>> GetAllEmployee()
        {
            const string sql = @"
         SELECT 
            E.EmployeeId,
            E.EmployeeCode,
            E.EmployeeName,     
            D.DestinationName,
            E.CreatedDate,
            L.PropName as Location
        FROM TTMS_Employees E
        left JOIN TTMS_EmployeeDestinations D ON D.DestinationId = E.DestinationId 
		left JOIN COMN_MasterTwoLocations L ON L.Id = E.LocationId
        WHERE E.IsActive = 1";

            return await _dbConnection.GetConnection().QueryAsync<EmployeeViewModel>(sql);
        }

        public async Task<bool> UpdateEmployee(Employee employee)
        {
            const string query = @"
        UPDATE TTMS_Employees SET
            EmployeeCode = @EmployeeCode,
            EmployeeName = @EmployeeName,
            DestinationId = @DestinationId,
            IsActive = @IsActive,
            LocationId = @LocationId
        WHERE EmployeeId = @EmployeeId;";

            try
            {
                int rowsAffected = await _dbConnection.GetConnection().ExecuteAsync(query, new
                {
                    employee.EmployeeCode,
                    employee.EmployeeName,
                    employee.DestinationId,                   
                    employee.IsActive,
                    employee.LocationId,
                    employee.EmployeeId
                });

                if (rowsAffected > 0)
                {
                    LogdbTTMS logdb = new()
                    {
                        TrObjectId = employee.EmployeeId,
                        TrLog = "EMPLOYEE UPDATED"
                    };

                    _iTTMSLogdbRepository.InsertLog(_dbConnection, logdb);
                }

                return rowsAffected > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> CheckEmployeeExist(string employeeCode)
        {
            const string query = @"
        SELECT TOP 1 1 
        FROM TTMS_Employees
        WHERE EmployeeCode = @EmployeeCode AND IsActive = 1;";

            var result = await _dbConnection.GetConnection().QueryFirstOrDefaultAsync<int?>(query, new
            {
                EmployeeCode = employeeCode
            });

            return result.HasValue;
        }    

        public async Task<Employee?> LoadEmployee(int employeeId)
        {
            const string query = @"
        SELECT 
            E.EmployeeId,
            E.EmployeeCode,
            E.EmployeeName,
            E.DestinationId,            
            E.IsActive,
            E.CreatedDate,
            E.LocationId	     
        FROM TTMS_Employees E       
        WHERE E.EmployeeId = @EmployeeId;";

            var employee = await _dbConnection.GetConnection().QueryFirstOrDefaultAsync<Employee>(query, new
            {
                EmployeeId = employeeId
            });

            return employee;
        }
        #endregion

        #region Driver Methods

        public async Task<bool> AddDriverAsync(Driver driver)
        {
            const string query = @"
        INSERT INTO TTMS_Drivers
        (NIC, DriverName, LicenseNo, PhoneMobile)
        VALUES
        (@NIC, @DriverName, @LicenseNo, @PhoneMobile);
        SELECT CAST(SCOPE_IDENTITY() AS INT) AS InsertedId;";

            try
            {
                var insertedId = await _dbConnection.GetConnection().QuerySingleOrDefaultAsync<int?>(query, new
                {
                    driver.NIC,
                    driver.DriverName,
                    driver.LicenseNo,
                    driver.PhoneMobile
                });

                if (insertedId.HasValue)
                {
                    LogdbTTMS logdb = new()
                    {
                        TrObjectId = insertedId.Value,
                        TrLog = "DRIVER CREATED"
                    };

                    _iTTMSLogdbRepository.InsertLog(_dbConnection, logdb);
                }

                return insertedId > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IEnumerable<DriverViewModel>> GetAllDrivers()
        {
            const string sql = @"
        SELECT 
            D.DriverId,
            D.NIC,
            D.DriverName,
            D.LicenseNo,
            D.PhoneMobile,
            D.CreatedDate,
            D.IsActive
        FROM TTMS_Drivers D
        WHERE D.IsActive = 1";

            return await _dbConnection.GetConnection().QueryAsync<DriverViewModel>(sql);
        }

        public async Task<bool> UpdateDriver(Driver driver)
        {
            const string query = @"
        UPDATE TTMS_Drivers SET
            NIC = @NIC,
            DriverName = @DriverName,
            LicenseNo = @LicenseNo,
            PhoneMobile = @PhoneMobile,
            IsActive = @IsActive
        WHERE DriverId = @DriverId;";

            try
            {
                int rowsAffected = await _dbConnection.GetConnection().ExecuteAsync(query, new
                {
                    driver.NIC,
                    driver.DriverName,
                    driver.LicenseNo,
                    driver.PhoneMobile,
                    driver.IsActive,
                    driver.DriverId
                });

                if (rowsAffected > 0)
                {
                    LogdbTTMS logdb = new()
                    {
                        TrObjectId = driver.DriverId,
                        TrLog = "DRIVER UPDATED"
                    };

                    _iTTMSLogdbRepository.InsertLog(_dbConnection, logdb);
                }

                return rowsAffected > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> CheckDriverExist(string nic)
        {
            const string query = @"
        SELECT TOP 1 1 
        FROM TTMS_Drivers
        WHERE NIC = @NIC AND IsActive = 1;";

            var result = await _dbConnection.GetConnection().QueryFirstOrDefaultAsync<int?>(query, new
            {
                NIC = nic
            });

            return result.HasValue;
        }

        public async Task<Driver?> LoadDriver(int driverId)
        {
            const string query = @"
        SELECT 
            D.DriverId,
            D.NIC,
            D.DriverName,
            D.LicenseNo,
            D.PhoneMobile,
            D.IsActive,
            D.CreatedDate
        FROM TTMS_Drivers D
        WHERE D.DriverId = @DriverId;";

            var driver = await _dbConnection.GetConnection().QueryFirstOrDefaultAsync<Driver>(query, new
            {
                DriverId = driverId
            });

            return driver;
        }

        #endregion


        #region DropDown Methods
        public async Task<IEnumerable<SelectListItem>> LoadLoactions()
        {
            string query = @"SELECT Id AS Value, 
            PropName AS Text FROM COMN_MasterTwoLocations where IsDelete=0 ORDER BY Text";
            var results = await _dbConnection.GetConnection().QueryAsync<SelectListItem>(query);
            return results;
        }
        public async Task<IEnumerable<SelectListItem>> LoadDestinations()
        {
            string query = @"SELECT DestinationId AS Value, 
            DestinationCode + '-' + DestinationName AS Text FROM TTMS_EmployeeDestinations where IsActive=1 ORDER BY Text";
            var results = await _dbConnection.GetConnection().QueryAsync<SelectListItem>(query);
            return results;
        }
        public async Task<IEnumerable<SelectListItem>> LoadVehiclePaymentMethods()
        {
            string query = @"SELECT PaymentMethodId AS Value, 
            PaymentMethodName AS Text FROM TTMS_PaymentMethods where IsActive=1 ORDER BY Text";
            var results = await _dbConnection.GetConnection().QueryAsync<SelectListItem>(query);
            return results;
        }
        public async Task<IEnumerable<SelectListItem>> LoadDrivers()
        {
            string query = @"SELECT DriverId AS Value,
            DriverName + ' - ' + Nic AS Text FROM TTMS_Drivers WHERE IsActive = 1 ORDER BY DriverName, Nic;";
            var results = await _dbConnection.GetConnection().QueryAsync<SelectListItem>(query);
            return results;
        }
        #endregion
    }
}
