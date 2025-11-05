using TMIS.DataAccess.COMON.IRpository;
using TMIS.Models.TTMS;
using Dapper;
using TMIS.DataAccess.TTMS.IRepository;

namespace TMIS.DataAccess.TTMS.Repository
{
    public class DailyToursRepository(IDatabaseConnectionSys dbConnection) : IDailyToursRepository
    {
        private readonly IDatabaseConnectionSys _connectionFactory = dbConnection;

        public async Task<IEnumerable<EmployeeDestination>> GetEmployeeDestinationsAsync()
        {
            return await _connectionFactory.GetConnection().QueryAsync<EmployeeDestination>(
                "SELECT * FROM TTMS_EmployeeDestinations WHERE IsActive = 1 ORDER BY DestinationName");
        }

        public async Task<IEnumerable<Employee>> GetEmployeesAsync()
        {
            var query = @"SELECT e.*, ed.DestinationName 
                     FROM TTMS_Employees e 
                     LEFT JOIN TTMS_EmployeeDestinations ed ON e.DestinationId = ed.DestinationId 
                     WHERE e.IsActive = 1 
                     ORDER BY e.EmployeeName";
            return await _connectionFactory.GetConnection().QueryAsync<Employee>(query);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDestinationAsync(int destinationId)
        {
            var query = @"SELECT e.*, ed.DestinationName 
                     FROM TTMS_Employees e 
                     LEFT JOIN TTMS_EmployeeDestinations ed ON e.DestinationId = ed.DestinationId 
                     WHERE e.IsActive = 1 AND e.DestinationId = @DestinationId 
                     ORDER BY e.EmployeeName";
            return await _connectionFactory.GetConnection().QueryAsync<Employee>(query, new { DestinationId = destinationId });
        }

        public async Task<IEnumerable<PaymentMethod>> GetPaymentMethodsAsync()
        {
            return await _connectionFactory.GetConnection().QueryAsync<PaymentMethod>(
                "SELECT * FROM TTMS_PaymentMethods WHERE IsActive = 1 ORDER BY PaymentMethodName");
        }

        public async Task<IEnumerable<TransportVehicle>> GetTransportVehiclesAsync()
        {
            var query = @"SELECT v.*, ed.DestinationName 
                     FROM TTMS_TransportVehicles v 
                     LEFT JOIN TTMS_EmployeeDestinations ed ON v.DestinationId = ed.DestinationId 
                     WHERE v.IsActive = 1 
                     ORDER BY v.VehicleNumber";
            return await _connectionFactory.GetConnection().QueryAsync<TransportVehicle>(query);
        }

        public async Task<IEnumerable<TransportVehicle>> GetVehiclesByDestinationAsync(int destinationId)
        {
            var query = @"SELECT v.*, ed.DestinationName 
                     FROM TTMS_TransportVehicles v 
                     LEFT JOIN TTMS_EmployeeDestinations ed ON v.DestinationId = ed.DestinationId 
                     WHERE v.IsActive = 1 AND v.DestinationId = @DestinationId 
                     ORDER BY v.VehicleNumber";
            return await _connectionFactory.GetConnection().QueryAsync<TransportVehicle>(query, new { DestinationId = destinationId });
        }

        public async Task<int> SaveVehicleEmployeeAllocationAsync(VehicleEmployeeAllocation allocation)
        {
            var query = @"INSERT INTO TTMS_VehicleEmployeeAllocations 
                     (VehicleId, EmployeeId, AllocationDate, IsActive, CreatedBy) 
                     VALUES (@VehicleId, @EmployeeId, @AllocationDate, @IsActive, @CreatedBy);
                     SELECT CAST(SCOPE_IDENTITY() as int)";
            return await _connectionFactory.GetConnection().ExecuteScalarAsync<int>(query, allocation);
        }

        public async Task<int> SaveVehiclePaymentMethodAsync(VehiclePaymentMethod paymentMethod)
        {
            var query = @"INSERT INTO TTMS_VehiclePaymentMethods 
                     (VehicleId, PaymentMethodId, Cost, EffectiveDate, IsActive) 
                     VALUES (@VehicleId, @PaymentMethodId, @Cost, @EffectiveDate, @IsActive);
                     SELECT CAST(SCOPE_IDENTITY() as int)";
            return await _connectionFactory.GetConnection().ExecuteScalarAsync<int>(query, paymentMethod);
        }

        public async Task<IEnumerable<Employee>> GetAllocatedEmployeesByVehicleAsync(int vehicleId)
        {
            var query = @"SELECT e.*, ed.DestinationName 
                     FROM TTMS_Employees e 
                     INNER JOIN TTMS_VehicleEmployeeAllocations vea ON e.EmployeeId = vea.EmployeeId 
                     LEFT JOIN TTMS_EmployeeDestinations ed ON e.DestinationId = ed.DestinationId 
                     WHERE vea.VehicleId = @VehicleId AND vea.IsActive = 1 AND e.IsActive = 1 
                     ORDER BY e.EmployeeName";
            return await _connectionFactory.GetConnection().QueryAsync<Employee>(query, new { VehicleId = vehicleId });
        }

        public async Task<int> SaveDailyToursAsync(List<DailyTour> dailyTours)
        {
            using var connection = _connectionFactory.GetConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                // First, delete existing records for the same vehicle and dates
                var deleteQuery = @"DELETE FROM TTMS_DailyTours 
                              WHERE VehicleId = @VehicleId AND EmployeeId = @EmployeeId 
                              AND TourDate IN @TourDates";

                var insertQuery = @"INSERT INTO TTMS_DailyTours 
                              (VehicleId, EmployeeId, TourDate, IsPresent, CreatedBy) 
                              VALUES (@VehicleId, @EmployeeId, @TourDate, @IsPresent, @CreatedBy)";

                foreach (var tour in dailyTours)
                {
                    var tourDates = dailyTours.Select(d => d.TourDate.Date).Distinct().ToList();
                    await connection.ExecuteAsync(deleteQuery,
                        new { tour.VehicleId, tour.EmployeeId, TourDates = tourDates },
                        transaction);
                }

                var result = await connection.ExecuteAsync(insertQuery, dailyTours, transaction);
                transaction.Commit();
                return result;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<IEnumerable<DailyTour>> GetDailyToursAsync(int vehicleId, int month, int year)
        {
            var query = @"SELECT * FROM TTMS_DailyTours 
                     WHERE VehicleId = @VehicleId 
                     AND MONTH(TourDate) = @Month 
                     AND YEAR(TourDate) = @Year";
            return await _connectionFactory.GetConnection().QueryAsync<DailyTour>(query,
                new { VehicleId = vehicleId, Month = month, Year = year });
        }
    }
}
