using Dapper;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.TGPS.IRpository;
using TMIS.Models.TGPS;

namespace TMIS.DataAccess.TGPS.Rpository
{
    public class EmployeePass(IDatabaseConnectionSys dbConnection, ISessionHelper sessionHelper) : IEmployeePass
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly ISessionHelper _iSessionHelper = sessionHelper;

        public async Task<IEnumerable<EmpPassVM>> GetList()
        {
            string sql = @"SELECT  Id, [EmpGpNo], [GateName], [ExpLoc], [ExpReason], [ExpOutTime], [IsReturn], [IsResponsed]
            FROM            TGPS_VwEGPHeaders
            WHERE        (GenUserId = @GenUser)";

            return await _dbConnection.GetConnection().QueryAsync<EmpPassVM>(sql, new { GenUser = _iSessionHelper.GetUserId() });
        }

        public async Task<EmployeePassVM> GetAllAsync()
        {
            using var dbConnection = _dbConnection.GetConnection();

            var goodsFromSql = @"SELECT A.Id, B.PropName AS Text
            FROM  ADMIN.dbo.GTPS_RelGRoomsLoc AS A INNER JOIN
            ADMIN.dbo.GTPS_MasterGRooms AS B ON A.GuardRoomId = B.Id INNER JOIN
            ADMIN.dbo._MasterUsers AS U ON A.Id = U.DefGRoomId
            WHERE        (U.Id = @UserId)";

            var approvalListSql = @"SELECT ADMIN.dbo.GTPS_RelApproveUser.ApproveUserId AS Id,
            ADMIN.dbo._MasterUsers.UserShortName AS Text
            FROM ADMIN.dbo.GTPS_RelApproveUser INNER JOIN
            ADMIN.dbo._MasterUsers ON ADMIN.dbo.GTPS_RelApproveUser.ApproveUserId = ADMIN.dbo._MasterUsers.Id
            WHERE (ADMIN.dbo.GTPS_RelApproveUser.UserId = @UserId)";


            var guardRoomsList = await GetDataFromTable(goodsFromSql, dbConnection);
            var approvalList = await GetDataFromTable(approvalListSql, dbConnection);

            return new EmployeePassVM
            {
                GuardRooms = guardRoomsList,
                ApprovEmps = approvalList

            };
        }

        private async Task<List<SelectListItem>> GetDataFromTable(string sql, IDbConnection dbConnection)
        {
            var results = await dbConnection.QueryAsync(sql, new { UserId = _iSessionHelper.GetUserId() });
            var items = results.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Text
            }).ToList();

            return items;
        }

        public async Task<string> InsertEmployeePassAsync(EmployeePassVM model)
        {
            using var dbConnection = _dbConnection.GetConnection();
            using var transaction = dbConnection.BeginTransaction();

            var userId = _iSessionHelper.GetUserId();
            try
            {

                string genRef = await GenerateGpRefAsync(dbConnection, transaction);

                // Insert Header
                string insertHeaderSql = @"
                INSERT INTO [dbo].[TGPS_TrGpEmpHeader]
                    ([EmpGpNo], [EGpLocId], [ExpLoc], [ExpReason], [ExpOutTime], ExpDate, [GenUserId], [IsNoReturn], IsResponsed, ApprovedById)
                VALUES
                    (@EmpGpNo, @GuardRoomId, @Location, @Reason, @OutTime, GETDATE(), @GenUserId, @IsNoReturn, 0, @ApprovedById);
                SELECT CAST(SCOPE_IDENTITY() as int);";

                var headerId = await dbConnection.ExecuteScalarAsync<int>(
                    insertHeaderSql,
                    new
                    {
                        EmpGpNo = genRef,
                        model.EmployeePass.GuardRoomId,
                        model.EmployeePass.Location,
                        model.EmployeePass.Reason,
                        OutTime = TimeSpan.Parse(model.EmployeePass.OutTime),
                        GenUserId = userId,
                        model.EmployeePass.IsNoReturn,
                        model.EmployeePass.ApprovedById
                    },
                    transaction
                );

                // Insert Details
                string insertDetailSql = @"
                INSERT INTO [dbo].[TGPS_TrGpEmpDetails]
                    ([EGpPassId], [EmpName], [EmpEPF], [ReturnTime], [ResponsedUserId])
                VALUES
                (@EGpPassId, @EmpName, @EmpEPF, @ReturnTime, @ResponsedUserId);";

                foreach (var emp in model.EmployeePass.EmpPassEmpList)
                {
                    await dbConnection.ExecuteAsync(
                        insertDetailSql,
                        new
                        {
                            EGpPassId = headerId,
                            emp.EmpName,
                            emp.EmpEPF,
                            ReturnTime = (DateTime?)null, // You can provide actual return time if available
                            ResponsedUserId = userId
                        },
                        transaction
                    );
                }

                transaction.Commit();
                return genRef;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<string> GenerateGpRefAsync(IDbConnection connection, IDbTransaction transaction)
        {
            int currentYear = DateTime.Now.Year;

            // 1. Try to get the generator for the current year
            var selectSql = @"SELECT TOP 1 [Id], [GenYear], [GenNo], [LastGeneratedDate]
            FROM [dbo].[TGPS_XysGenerateNumber] WHERE GenYear = @Year AND GpType='TGP'";

            var generator = await connection.QuerySingleOrDefaultAsync<dynamic>(
                selectSql, new { Year = currentYear }, transaction);

            int genNo;
            int id;

            if (generator == null)
            {
                // 2. No record for this year — insert new
                genNo = 1;

                var insertSql = @"INSERT INTO [dbo].[TGPS_XysGenerateNumber]
                    (GenYear, GenNo, LastGeneratedDate, GpType) VALUES (@GenYear, @GenNo, GETDATE(),'TGP' );
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                await connection.ExecuteScalarAsync<int>(
                    insertSql,
                    new { GenYear = currentYear, GenNo = genNo + 1 },
                    transaction
                );
            }
            else
            {
                // 3. Record exists — increment and update
                genNo = generator.GenNo;
                id = generator.Id;

                var updateSql = @"
                UPDATE [dbo].[TGPS_XysGenerateNumber]
                SET GenNo = @NewGenNo,
                    LastGeneratedDate = GETDATE()
                WHERE Id = @Id AND GpType='TGP';";

                await connection.ExecuteAsync(
                    updateSql,
                    new { NewGenNo = genNo + 1, Id = id },
                    transaction
                );
            }

            // 4. Format final reference number
            string reference = $"TEP/{currentYear}/{genNo.ToString("D5")}";
            return reference;
        }

        public async Task<EmpPassVM> GetEmpPassesAsync(int id)
        {
            using var dbConnection = _dbConnection.GetConnection();

            string headerSql = @"SELECT [EmpGpNo], [GateName], [ExpLoc], [ExpReason], [ExpOutTime], [IsReturn], [IsResponsed], [ResponsedBy], [ResponsedDateTime]
                FROM [TMIS].[dbo].[TGPS_VwEGPHeaders] WHERE Id = @Id;";


            // Fetch header details using Id
            var header = await dbConnection.QuerySingleOrDefaultAsync<EmpPassVM>(headerSql, new { Id = id });

            if (header == null)
            {
                throw new InvalidOperationException($"No header found for Id {id}");
            }

            // Fetch details using EGpPassId (matches EmpGpNo from header)
            var detailsSql = @"SELECT  [EGpPassId], [EmpName], [EmpEPF], [ReturnTime], [ResponsedUser]
                FROM [TMIS].[dbo].[TGPS_VwEGPDetails] WHERE EGpPassId = @Id;";

            var details = await dbConnection.QueryAsync<EmpPassEmployees>(detailsSql, new { Id = id });

            header.ShowGPItemVMList = details.ToList();

            return header;
        }
    }
}
