using TMIS.Models.TTMS;

namespace TMIS.DataAccess.TTMS.IRepository
{
    public interface IDailyToursRepository
    {
        Task<IEnumerable<EmployeeDestination>> GetEmployeeDestinationsAsync();
        Task<IEnumerable<Employee>> GetEmployeesAsync();
        Task<IEnumerable<Employee>> GetEmployeesByDestinationAsync(int destinationId);
        Task<IEnumerable<PaymentMethod>> GetPaymentMethodsAsync();
        Task<IEnumerable<TransportVehicle>> GetTransportVehiclesAsync();
        Task<IEnumerable<TransportVehicle>> GetVehiclesByDestinationAsync(int destinationId);
        Task<int> SaveVehicleEmployeeAllocationAsync(VehicleEmployeeAllocation allocation);
        Task<int> SaveVehiclePaymentMethodAsync(VehiclePaymentMethod paymentMethod);
        Task<IEnumerable<Employee>> GetAllocatedEmployeesByVehicleAsync(int vehicleId);
        Task<int> SaveDailyToursAsync(List<DailyTour> dailyTours);
        Task<IEnumerable<DailyTour>> GetDailyToursAsync(int vehicleId, int month, int year);
    }
}
