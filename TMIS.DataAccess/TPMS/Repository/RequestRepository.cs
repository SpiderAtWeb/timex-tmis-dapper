using Dapper;
using Microsoft.Extensions.Configuration;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.ITIS.IRepository;
using TMIS.DataAccess.TPMS.IRepository;
using TMIS.Models.HRRS;
using TMIS.Models.HRRS.VM;
using TMIS.Models.TPMS;
using TMIS.Models.TPMS.VM;
using TMIS.Utility;

namespace TMIS.DataAccess.TPMS.Repository
{
    public class RequestRepository(IDatabaseConnectionSys dbConnection, ITPMSLogdb iTPMSLogdb,
         ISessionHelper sessionHelper, IGmailSender gmailSender,
         ICommonList iCommonList, IDatabaseConnectionAdm admConnection, IConfiguration configuration) : IRequestRepository
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly IDatabaseConnectionAdm _admConnection = admConnection;
        private readonly ITPMSLogdb _iTPMSLogdb = iTPMSLogdb;
        private readonly ICommonList _icommonList = iCommonList;
        private readonly ISessionHelper _iSessionHelper = sessionHelper;
        private readonly IGmailSender _gmailSender = gmailSender;
        private readonly IConfiguration _configuration = configuration;

        public async Task<bool> AddAsync(TPMS_PurchaseRequests obj)
        {
            const string query = @"
            INSERT INTO TPMS_PurchaseRequests
            (
                Description, SerialNumber, UserName, Requirement, Department, Designation, Unit, QTY, RequestBy,
                RequestStatus
            )
            VALUES
            (
                @Description, @SerialNumber, @UserName, @Requirement, @Department, @Designation, @Unit, @QTY,
                @RequestBy, @RequestStatus
            );
            SELECT CAST(SCOPE_IDENTITY() AS INT);";



            try
            {
                var insertedId = await _dbConnection.GetConnection().QuerySingleOrDefaultAsync<int?>(query, new
                {                    
                    RequestBy = _iSessionHelper.GetShortName().ToUpper(),
                    RequestStatus = 1 // Default to 'Pending' or change based on logic
                });

                if (insertedId.HasValue)
                {
                    TPMS_TrLogger logdb = new()
                    {
                        RefID = insertedId.Value,
                        TrLog = "PURCHASE REQUEST CREATED"

                    };

                    _iTPMSLogdb.InsertLog(_dbConnection, logdb);
                }

                return insertedId > 0;
            }
            catch
            {

                return false;
            }
        }
        public async Task<IEnumerable<PurchaseVM>> GetAllAsync()
        {
            string sql = @"select r.RequestID, r.Description, r.SerialNumber, r.UserName, r.Requirement,
                            r.Department, r.Designation, r.Unit, r.QTY, r.RequestDate, r.RequestBy,
                            r.PurchaseDate, s.PropName as RequestStatus from TPMS_PurchaseRequests as r 
                            inner join TPMS_PurchaseRequestStatus as s on s.Id=r.RequestStatus
                            where r.IsDelete=0";

            return await _dbConnection.GetConnection().QueryAsync<PurchaseVM>(sql);
        }
        public async Task<CreateRequestVM?> LoadListItems()
        {
            CreateRequestVM createRequestVM = new()
            {
                DepartmentList = await _icommonList.LoadDepartmentsFromAD(),
                DesignationList = await _icommonList.LoadDesignationsFromAD(),
                UnitList = await _icommonList.LoadLocationsFromAD(),  
                SerialNumberList = await _icommonList.LoadExistsSerialList(),
                UserList = await _icommonList.LoadEmployeeListHRRS()

            };
            return createRequestVM;
        }
    }
}
