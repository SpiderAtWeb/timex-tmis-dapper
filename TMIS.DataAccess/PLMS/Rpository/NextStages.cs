using Dapper;
using Microsoft.AspNetCore.Http;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.PLMS.IRpository;
using TMIS.Models.PLMS;
using TMIS.Models.SMIS;

namespace TMIS.DataAccess.PLMS.Rpository
{
    public class NextStages(IDatabaseConnectionSys dbConnection, IUserControls userControls, IPLMSLogdb pLMSLogdb) : INextStages
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly IUserControls _userControls = userControls;
        private readonly IPLMSLogdb _pLMSLogdb = pLMSLogdb;

        public async Task<NextStageInquiryVM> LoadNextInquiryDropDowns(string id)
        {
            using (var connection = _dbConnection.GetConnection())
            {
                string query = @"SELECT [TrINQId], [CustomerId], [InquiryTypeId], ColorCode, SeasonName,
                            Customer, InquiryType, StyleNo, StyleDesc, ColorCode 
                         FROM  
                            [PLMS_VwInquiryListPending]
                         WHERE     
                            Id = @InquiryId";

                // Await the asynchronous database query to fetch the Inquiry
                var inquiry = await connection.QueryFirstOrDefaultAsync<Inquiry>(query, new { InquiryId = id });

                // Create and populate the NextStageInquiryVM
                var oNextStageInquiryVM = new NextStageInquiryVM
                {
                    InquiryTypesList = await _userControls.LoadDropDownsAsync("PLMS_MasterTwoInquiryTypes"),
                    SampleTypesList = await _userControls.LoadDropDownsAsync("PLMS_MasterTwoSampleTypes"),
                    RoutingPresetsList = await _userControls.LoadRouteDropAsync("PLMS_CPTemplateHeader"),

                    Inquiry = inquiry
                };

                return oNextStageInquiryVM;
            }
        }

        public async Task<string> SaveNextInquiryAsync(NextStageInquiryVM oNextInquiryVM, IFormFile? artwork)
        {
            var connection = _dbConnection.GetConnection();
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    if (oNextInquiryVM == null)
                    {
                        throw new ArgumentNullException(nameof(oNextInquiryVM), "InquiryVM cannot be null.");
                    }

                    var oNewInquiryVM = new NewInquiryVM
                    {
                        Id = oNextInquiryVM.Inquiry!.TrINQId,
                        CustomerId = oNextInquiryVM.Inquiry.CustomerId,
                        StyleNo = oNextInquiryVM.Inquiry.StyleNo,
                        StyleDesc = oNextInquiryVM.Inquiry.StyleDesc,
                        ColorCode = oNextInquiryVM.Inquiry.ColorCode,
                        Remarks = oNextInquiryVM.Inquiry.InquiryComment,
                        ReceivedDate = oNextInquiryVM.Inquiry.InquiryRecDate,
                        Season = oNextInquiryVM.Inquiry.SeasonName,
                        IsPriceStageAv = oNextInquiryVM.Inquiry.IsPriceStageAv,
                        IsSMVStageAv = oNextInquiryVM.Inquiry.IsSMVStageAv,
                        InquiryTypeId = oNextInquiryVM.Inquiry.InquiryTypeId,
                        SampleTypeId = oNextInquiryVM.Inquiry.SampleTypeId,
                        ActivityList = oNextInquiryVM.ActivityList
                    };

                    var inquiryDtId = await NewInquiry.InsertDetailsAsync(connection, transaction, oNewInquiryVM.Id, oNewInquiryVM, artwork);
                    await NewInquiry.InsertActivitiesAsync(connection, transaction, inquiryDtId, oNewInquiryVM.ActivityList);

                    Logdb logdb = new()
                    {
                        TrObjectId = inquiryDtId,
                        TrLog = "NEW INQUIRY STAGE CREATED"
                    };

                    await _pLMSLogdb.InsertLogTrans(connection, transaction, logdb);

                    // Commit the transaction if all operations are successful
                    transaction.Commit();

                    return "Success: Inquiry saved successfully.";
                }
                catch (Exception ex)
                {
                    // Rollback the transaction in case of an error
                    transaction.Rollback();
                    return $"Error: {ex.Message}";
                }
            }
        }
    }
}
