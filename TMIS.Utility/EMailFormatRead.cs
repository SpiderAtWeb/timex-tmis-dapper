using System.Text;
using static iTextSharp.text.pdf.AcroFields;

namespace TMIS.Utility
{
    public static class EMailFormatRead
    {
        public static string GetApprovalTwoColumnsEmailBody(Dictionary<string, string> placeholders,
           List<(string ColA, string ColB)> headers,
           List<(string ColA, string ColB)> details)
        {
            string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmailTemplates", "Approval-Two-Columns.htm");

            string htmlBody = File.ReadAllText(templatePath);

            // Replace header
            var sbH = new StringBuilder();
            foreach (var (ColA, ColB) in headers)
            {
                sbH.AppendLine(@$"<tr style=""border-bottom: 1px solid #e2e8f0;"">
                                       <td style=""background-color: #f1f5f9; padding: 8px 12px; font-weight: 600; color: #475569; border-radius: 6px 0 0 6px;"">
                                           {ColA}
                                       </td>
                                       <td style=""padding: 8px 12px; color: #334155; border-radius: 0 6px 6px 0;""> 
                                          {ColB}
                                       </td>
                                   </tr>");
            }

            htmlBody = htmlBody.Replace("{HEADERS}", sbH.ToString());

            // Replace details
            var sbD = new StringBuilder();
            foreach (var (ColA, ColB) in details)
            {
                sbD.AppendLine(@$"<tr style=""background-color: #f8fafc;"">
                                       <td style=""padding: 10px; color: #475569; border-right: 1px solid #e2e8f0;"">
                                           {ColA}
                                       </td>
                                       <td style=""padding: 10px; color: #475569; text-align: center;""> 
                                          {ColB}
                                       </td>
                                   </tr>");
            }

            htmlBody = htmlBody.Replace("{DETAILS}", sbD.ToString());

            // Replace placeholders
            foreach (var item in placeholders)
            {
                htmlBody = htmlBody.Replace($"{{{item.Key}}}", item.Value);
            }

            return htmlBody;
        }

        public static string GetApprovalThreeColumnsEmailBody(Dictionary<string, string> placeholders,
          List<(string ColA, string ColB)> headers,
          List<(string ColA, string ColB, string ColC, string ColD)> details)
        {
            string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmailTemplates", "Approval-Three-Columns.htm");

            string htmlBody = File.ReadAllText(templatePath);

            // Replace header
            var sbH = new StringBuilder();
            foreach (var (ColA, ColB) in headers)
            {
                sbH.AppendLine(@$"<tr style=""border-bottom: 1px solid #e2e8f0;"">
                                       <td style=""background-color: #f1f5f9; padding: 8px 12px; font-weight: 600; color: #475569; border-radius: 6px 0 0 6px;"">
                                           {ColA}
                                       </td>
                                       <td style=""padding: 8px 12px; color: #334155; border-radius: 0 6px 6px 0;""> 
                                          {ColB}
                                       </td>
                                   </tr>");
            }

            htmlBody = htmlBody.Replace("{HEADERS}", sbH.ToString());

            // Replace details
            var sbD = new StringBuilder();
            foreach (var (ColA, ColB, ColC, ColD) in details)
            {
                sbD.AppendLine(@$"<tr style=""background-color: #f8fafc;"">
                                       <td style=""padding: 10px; color: #475569; border-right: 1px solid #e2e8f0;"">
                                           {ColA}
                                       </td>
                                       <td style=""padding: 10px; color: #475569; border-right: 1px solid #e2e8f0;""> 
                                          {ColB}
                                       </td>
                                        <td style=""padding: 10px; color: #475569; text-align: center; border-right: 1px solid #e2e8f0;"">
                                           {ColC}
                                       </td>
                                       <td style=""padding: 10px; color: #475569; text-align: center; border-right: 1px solid #e2e8f0;""> 
                                          {ColD}
                                       </td>
                                   </tr>");
            }

            htmlBody = htmlBody.Replace("{DETAILS}", sbD.ToString());

            // Replace placeholders
            foreach (var item in placeholders)
            {
                htmlBody = htmlBody.Replace($"{{{item.Key}}}", item.Value);
            }

            return htmlBody;
        }

        public static string GetApprovalThreeColumnsITISEmailBody(Dictionary<string, string> placeholders,
          List<(string ColA, string ColB)> headers,
          List<(string ColA, string ColB, string ColC, string ColD)> details)
        {
            string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmailTemplates", "Approval-Three-Columns-ITIS.htm");

            string htmlBody = File.ReadAllText(templatePath);

            // Replace header
            var sbH = new StringBuilder();
            foreach (var (ColA, ColB) in headers)
            {
                sbH.AppendLine(@$"<tr style=""border-bottom: 1px solid #e2e8f0;"">
                                       <td style=""background-color: #f1f5f9; padding: 8px 12px; font-weight: 600; color: #475569; border-radius: 6px 0 0 6px;"">
                                           {ColA}
                                       </td>
                                       <td style=""padding: 8px 12px; color: #334155; border-radius: 0 6px 6px 0;""> 
                                          {ColB}
                                       </td>
                                   </tr>");
            }

            htmlBody = htmlBody.Replace("{HEADERS}", sbH.ToString());

            // Replace details
            var sbD = new StringBuilder();
            foreach (var (ColA, ColB, ColC, ColD) in details)
            {
                sbD.AppendLine(@$"<tr style=""background-color: #f8fafc;"">
                                       <td style=""padding: 10px; color: #475569; border-right: 1px solid #e2e8f0;"">
                                           {ColA}
                                       </td>
                                       <td style=""padding: 10px; color: #475569; border-right: 1px solid #e2e8f0;""> 
                                          {ColB}
                                       </td>
                                        <td style=""padding: 10px; color: #475569; text-align: center; border-right: 1px solid #e2e8f0;"">
                                           {ColC}
                                       </td>
                                       <td style=""padding: 10px; color: #475569; text-align: center; border-right: 1px solid #e2e8f0;""> 
                                          {ColD}
                                       </td>
                                   </tr>");
            }

            htmlBody = htmlBody.Replace("{DETAILS}", sbD.ToString());

            // Replace placeholders
            foreach (var item in placeholders)
            {
                htmlBody = htmlBody.Replace($"{{{item.Key}}}", item.Value);
            }

            return htmlBody;
        }
        public static string GetApprovalThreeColumnsHRRSEmailBody(Dictionary<string, string> placeholders)
        {
            string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmailTemplates", "Approval-HRRS.htm");

            string htmlBody = File.ReadAllText(templatePath);

            // Replace placeholders
            foreach (var item in placeholders)
            {
                htmlBody = htmlBody.Replace($"{{{item.Key}}}", item.Value);
            }
            return htmlBody;
        }
       
        public static string GetITRequestHRRSEmailBody(Dictionary<string, string> placeholders)
        {
            string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmailTemplates", "ITRequest-HRRS.htm");

            string htmlBody = File.ReadAllText(templatePath);

            // Replace placeholders
            foreach (var item in placeholders)
            {
                htmlBody = htmlBody.Replace($"{{{item.Key}}}", item.Value);
            }
            return htmlBody;
        }

        public static string GetApprovalSMIMsEmailBody(Dictionary<string, string> placeholders, Dictionary<string, string> endPlaceholders, List<(string ColA, string ColB, string ColC, string ColD, string ColE, string ColF)> details)
        {
            string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmailTemplates", "SM-Invoice-Appoval.htm");

            string htmlBody = File.ReadAllText(templatePath);

            //Replace details
            var sbD = new StringBuilder();
            foreach (var (ColA, ColB, ColC, ColD, ColE, ColF) in details)
            {
                sbD.AppendLine(@$"<tr>
                    <td style=""border: 1px solid #dee2e6; padding: 8px; text-align: center; font-size: 11px;"">{ColA}</td>
                    <td style=""border: 1px solid #dee2e6; padding: 8px; text-align: center; font-size: 11px;"">{ColB}</td>
                    <td style=""border: 1px solid #dee2e6; padding: 8px; text-align: center; font-size: 11px;"">{ColC}</td>
                    <td style=""border: 1px solid #dee2e6; padding: 8px; text-align: center; font-size: 11px;"">{ColD}</td>
                    <td style=""border: 1px solid #dee2e6; padding: 8px; text-align: center; font-size: 11px;"">{ColE}</td>
                    <td style=""border: 1px solid #dee2e6; padding: 8px; text-align: right; font-size: 11px;"">{ColF}</td>
                </tr>");
            }

            htmlBody = htmlBody.Replace("{DETAILS}", sbD.ToString());

            // Replace placeholders
            foreach (var item in placeholders)
            {
                htmlBody = htmlBody.Replace($"{{{item.Key}}}", item.Value);
            }

            // Replace placeholders
            foreach (var item in endPlaceholders)
            {
                htmlBody = htmlBody.Replace($"{{{item.Key}}}", item.Value);
            }

            return htmlBody;
        }
    }
}
