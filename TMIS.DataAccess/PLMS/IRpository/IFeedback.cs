using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMIS.Models.PLMS;

namespace TMIS.DataAccess.PLMS.IRpository
{
    public interface IFeedback
    {
        Task<FeedbackVM> GetInquiryAsync(string id);

        Task<string> SaveFeedbackAsync(int id, string buyerComment, int actionType);

        Task<int> CheckPendingActivities(int id);
    }
}
