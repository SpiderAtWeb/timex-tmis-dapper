using Dapper;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.COMON.Rpository;
using TMIS.DataAccess.SMIM.IRpository;
using TMIS.Models.SMIS;
using TMIS.Models.SMIS.VM;

namespace TMIS.DataAccess.SMIM.Repository
{
    public class Respond(IDatabaseConnectionSys dbConnection, ISMIMLogdb iSMIMLogdb, ISessionHelper sessionHelper) : IRespond
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly ISMIMLogdb _iSMIMLogdb = iSMIMLogdb;
        private readonly ISessionHelper _iSessionHelper = sessionHelper;


        public IEnumerable<RespondVM> GetRequestList()
        {
            string query = "SELECT [Id], [QrCode], [SerialNo], [MachineType], [ReqUnit], [ReqLocation], [TrUserId], [ReqRemark], [DateCreate] FROM [SMIM_VwMcRequest] WHERE isCompleted = 0";
            return _dbConnection.GetConnection().Query<RespondVM>(query);
        }

        public RespondDetailsVM GetReqDetailsList(int id)
        {

            var sqlRq = "SELECT [McId] FROM [SMIM_VwMcRequest] WHERE Id = @Id ;";
            var para = new { Id = id };
            var mcId = _dbConnection.GetConnection().QueryFirstOrDefault<int?>(sqlRq, para);


            var sql = @"
            SELECT [Id],[QrCode], [SerialNo], [MachineType], [ReqUnit], [ReqLocation], [TrUserId], [ReqRemark], [DateCreate] FROM [SMIM_VwMcRequest] WHERE Id = @Id;
            SELECT QrCode, SerialNo, ServiceSeq, MachineModel, MachineBrand, MachineType,
            CurrentUnit, Location, DatePurchased, DateBorrow, IsOwned, ImageFR, ImageBK FROM SMIM_VwMcInventory WHERE Id = @McId;";

            var oPara = new { Id = id, McId = mcId };

            using (var multi = _dbConnection.GetConnection().QueryMultiple(sql, oPara))
            {
                var _respondVM = multi.Read<RespondVM>().FirstOrDefault();
                var _machinesData = multi.Read<MachinesData>().FirstOrDefault();

                return new RespondDetailsVM
                {
                    RespondVM = _respondVM,
                    MachinesData = _machinesData
                };
            }
        }

        public string[] UpdateStatus(bool sts, int iD)
        {
            string[] result = new string[2];
            DateTime nowDT = DateTime.Now;

            try
            {
                var sqlRq = "SELECT [McId] FROM [SMIM_VwMcRequest] WHERE Id = @Id;";
                var mcId = _dbConnection.GetConnection().QueryFirstOrDefault<int?>(sqlRq, new { Id = iD });

                if (!mcId.HasValue)
                {
                    result[0] = "0";
                    result[1] = "No record found for the given Id.";
                    return result;
                }

                int statusId = sts ? 4 : 5;  // 4 for Approved, 5 for Rejected                       
                int mnStatus = sts ? 7 : 2;

                // Update machine inventory status
                string updateMnQuery = @"
                UPDATE [dbo].[SMIM_TrInventory]
                SET [CurrentStatusId] = @MnStatus, [LastUpdateTime] = @NowDT
                WHERE [Id] = @McId";
                int rowsAffected = _dbConnection.GetConnection().Execute(updateMnQuery, new { MnStatus = mnStatus, McId = mcId, NowDT = nowDT });

                // Update machine transfer status
                string updateTransferQuery = @"
                UPDATE [dbo].[SMIM_TrTransfers]
                SET [TrStatusId] = @StatusId, [isCompleted] = 1, [DateResponseDate] = @NowDT, [ResposeUserId] = @ResposeUserId
                WHERE [Id] = @iD";
                rowsAffected += _dbConnection.GetConnection().Execute(updateTransferQuery, new { StatusId = statusId, NowDT = nowDT, iD, ResposeUserId = _iSessionHelper.GetUserId() });

                string logMessage = sts ? "MACHINE REQUEST APPROVED - WEB" : "MACHINE REQUEST REJECTED - WEB";


                // Set result based on whether rows were affected
                if (rowsAffected > 0)
                {
                    result[0] = "1";
                    result[1] = "Update successful.";
                }
                else
                {
                    result[0] = "0";
                    result[1] = "No records were updated. The Id may not exist.";
                }
            }
            catch (Exception ex)
            {
                result[0] = "0";
                result[1] = ex.Message;
            }

            return result;
        }
    }
}
