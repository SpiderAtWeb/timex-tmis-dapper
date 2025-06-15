using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.TGPS.IRpository;
using TMIS.Models.TGPS;
using TMIS.Utility;

namespace TMIS.DataAccess.TGPS.Rpository
{
    public class EmployeePass(IDatabaseConnectionSys dbConnection, ISessionHelper sessionHelper, IUserControls userControls, IGmailSender gmailSender) : IEmployeePass
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly ISessionHelper _iSessionHelper = sessionHelper;
        private readonly IUserControls _userControls = userControls;
        private readonly IGmailSender _gmailSender = gmailSender;

        public async Task<IEnumerable<EmpPassVM>> GetList()
        {
            string sql = @"SELECT  Id, [EmpGpNo], [GateName], [ExpLoc], [ExpReason], [ExpOutTime], [IsReturn], [IsApproved]
            FROM            TGPS_VwEGPHeaders
            WHERE        (GenUserId = @GenUser) ORDER BY EmpGpNo DESC";

            return await _dbConnection.GetConnection().QueryAsync<EmpPassVM>(sql, new { GenUser = _iSessionHelper.GetUserId() });
        }

        public async Task<EmployeePassVM> GetAllAsync()
        {
            using var dbConnection = _dbConnection.GetConnection();

            var goodsFromSql = @"SELECT GrLocRelId AS Id, GrName AS Text FROM TGPS_VwGRUsers WHERE (Id = @UserId)";

            var approvalListSql = @"SELECT AppUserId AS Id, UserShortName AS Text
            FROM  ADMIN.dbo.TGPS_VwUserApprovePersons WHERE (UserId = @UserId) AND (SystemType = N'TEP')";

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

                string genRef = await _userControls.GenerateGpRefAsync(dbConnection, transaction, "[TGPS_XysGenerateNumber]", "TEP");

                // Insert Header
                string insertHeaderSql = @"
                INSERT INTO [dbo].[TGPS_TrGpEmpHeader]
                    ([EmpGpNo], [EGpLocId], [ExpLoc], [ExpReason], [ExpOutTime], ExpDate, [GenUserId], [IsNoReturn], IsApproved, ApprovedById, IsOutUpdated, IsInUpdate)
                VALUES
                    (@EmpGpNo, @GuardRoomId, @Location, @Reason, @OutTime, GETDATE(), @GenUserId, @IsNoReturn, 0, @ApprovedById, 0, @IsInUpdate);
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
                        model.EmployeePass.ApprovedById,
                        IsInUpdate = model.EmployeePass.IsNoReturn ? 1 : 0 // If IsNoReturn is true, IsInUpdate is set to 0, otherwise 1
                    },
                    transaction
                );

                // Insert Details
                string insertDetailSql = @"
                INSERT INTO [dbo].[TGPS_TrGpEmpDetails]
                    ([EGpPassId], [EmpName], [EmpEPF])
                VALUES
                (@EGpPassId, @EmpName, @EmpEPF);";

                foreach (var emp in model.EmployeePass.EmpPassEmpList)
                {
                    await dbConnection.ExecuteAsync(
                        insertDetailSql,
                        new
                        {
                            EGpPassId = headerId,
                            emp.EmpName,
                            emp.EmpEPF
                        },
                        transaction
                    );
                }

                transaction.Commit();

                PrepairEmail(headerId);
                return genRef;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private void PrepairEmail(int genId)
        {
            using var connection = _dbConnection.GetConnection();

            string headerQuery = @"SELECT  EmpGpNo, GateName, ExpLoc, ExpReason, ExpOutTime, IsReturn, GenUser, ApprovedById
            FROM TGPS_VwEGPHeaders WHERE (Id = @GenId)";

            var header = connection.Query(headerQuery, new { GenId = genId }).FirstOrDefault();

            string detailsQuery = @"SELECT EmpName, EmpEPF
            FROM            TGPS_VwEGPDetails WHERE        (EGpPassId = @GenId)";

            var details = connection.Query(detailsQuery, new { GenId = genId }).ToList();

            // Prepare header part of array
            var myList = new List<string>
            {
                $"{header!.EmpGpNo}",
                $"{header.GateName}",
                $"{header.ExpLoc}",
                $"{header.ExpReason}",
                $"{header.ExpOutTime}",
                $"{header.IsReturn}",
                $"{header.GenUser}"
            };

            // Append each detail row as a string item in the array
            foreach (var d in details)
            {
                string detailString = $"{d.EmpName}|{d.EmpEPF}";
                myList.Add(detailString);
            }

            // Convert to array
            string[] myArray = [.. myList];

            // Prepare email recipient
            string mailTo = connection.QuerySingleOrDefault<string>(
                "SELECT UserEmail FROM ADMIN.dbo._MasterUsers WHERE Id = @Id",
                new { Id = header.ApprovedById }
            ) ?? throw new InvalidOperationException("No email found for the approved user.");

            // Send email
            Task.Run(() => _gmailSender.EPRequestToApprove(mailTo, myArray));
        }

        public async Task<EmpPassVM> GetEmpPassesAsync(int id)
        {
            using var dbConnection = _dbConnection.GetConnection();

            string headerSql = @"SELECT [EmpGpNo], [GateName], [ExpLoc], [ExpReason], [ExpOutTime], [IsReturn], [IsApproved], [ApprovedBy], [ResponsedDateTime]
                FROM [TMIS].[dbo].[TGPS_VwEGPHeaders] WHERE Id = @Id;";


            // Fetch header details using Id
            var header = await dbConnection.QuerySingleOrDefaultAsync<EmpPassVM>(headerSql, new { Id = id }) ?? throw new InvalidOperationException($"No header found for Id {id}");

            // Fetch details using EGpPassId (matches EmpGpNo from header)
            var detailsSql = @"SELECT  [EGpPassId], [EmpName], [EmpEPF], [ActualOutTime], [ActualInTime]
                FROM [TMIS].[dbo].[TGPS_VwEGPDetails] WHERE EGpPassId = @Id;";

            var details = await dbConnection.QueryAsync<EmpPassEmployees>(detailsSql, new { Id = id });

            header.ShowGPItemVMList = details.ToList();

            return header;
        }
    }
}
