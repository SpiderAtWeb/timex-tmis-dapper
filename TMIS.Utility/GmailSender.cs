using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Mail;

namespace TMIS.Utility
{
    public class GmailSender(IConfiguration configuration, ILogger<GmailSender> logger) : IGmailSender
    {
        // Gmail SMTP server settings
        //string smtpServer = "smtp.gmail.com";
        //int smtpPort = 587;
        //string senderEmail = "mism@timexsl.com";
        //string senderPassword = "zdmq btej luaw qjqx"; // Use an app password if 2FA is enabled.

        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<GmailSender> _logger = logger;

        private void SendMail(string mailTo, string mailCc, string mailBcc, string mailBody, string mailSubject)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("SmtpSettings");

                // Create the MailMessage object
                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(smtpSettings["senderEmail"], "TMIS Messenger"),
                    Subject = mailSubject,
                    Body = mailBody,
                    IsBodyHtml = true // Enable HTML formatting
                };

                if (mailTo.Trim().Length > 0)
                {
                    foreach (string sToBreak in mailTo.Split(",".ToCharArray()))
                        mail.To.Add(sToBreak);
                }

                if (mailCc.Trim().Length > 0)
                {
                    foreach (string sCcBreak in mailCc.Split(",".ToCharArray()))
                        mail.CC.Add(sCcBreak);
                }

                if (mailBcc.Trim().Length > 0)
                {
                    foreach (string sBccBreak in mailBcc.Split(",".ToCharArray()))
                        mail.Bcc.Add(sBccBreak);
                }

                // Configure the SMTP client
                SmtpClient smtpClient = new(smtpSettings["Host"], int.Parse(smtpSettings["Port"]))
                {
                    Credentials = (ICredentialsByHost)new NetworkCredential(smtpSettings["senderEmail"], smtpSettings["senderPassword"]),
                    EnableSsl = true, // Secure connection
                    UseDefaultCredentials = false,
                    Timeout = 20000
                };

                // Send the email
                smtpClient.Send(mail);
                _logger.LogInformation($"Email sent for gatepass Succefully {mailSubject}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send confirmation email for gatepass {ex.Message}");
            }
        }

        //TIMEX Gate Pass To Approve
        public void McRequestToApprove(params string[] myArray)
        {
            string entryNo = myArray[0];
            string qrCode = myArray[1].ToString();

            string subject = "Machine Request To Approve " + "[" + qrCode + "] (TMIS)";
           
            string serialNo = myArray[2].ToString();
            string mcModel = myArray[3].ToString();
            string mcBrand = myArray[4].ToString();
            string mcType = myArray[5].ToString();
            string crUnit = myArray[6].ToString();
            string crLocation = myArray[7].ToString();
            string reqUnit = myArray[8].ToString();
            string reqLocation = myArray[9].ToString();
            string reqComment = myArray[10].ToString();

            //Prepair Two Button
            string sApprove = "mailto:rasika.dalpathadu@timexsl.com?subject=Approved [" + qrCode + "] TMIS(" + entryNo + ")";
            string sReject = "mailto:rasika.dalpathadu@timexsl.com?subject=Rejected [" + qrCode + "] TMIS(" + entryNo + ")";

            string sBody = "<!DOCTYPE html> " +
            "<html> " +
            "<head> " +
            "<meta http-equiv='Content-Type' content='text/html; charset=iso-8859-1'></head> " +

            //Mail Header
            "<table style='border: 3px solid Blue; font-family: Cambria; height: 34px;'> " +
            "<tbody> " +
            "<tr><th style='width: 745px;'>You have a Machine Request To Approve " + "[" + qrCode + "]</th></tr> " +
            "</tbody> " +
            "</table> " +

            //Styles
            "<div><hr /></div> " +
            "<body>  " +
            "<table style='border-color: blue; border-collapse: collapse;font-family: Cambria; table-layout: fixed;' border='1'> " +
            "<tbody> " +

            "<tr> " +
            "<th style='background-color: #b6f96d; text-align: center;' colspan='2'><strong><span style='color: #black;'>Machine Details</span></strong></th> " +
            "</tr>  " +

            "<tr> " +
            "<th style='width: 375px;'>Headings</th> " +
            "<th style='width: 375px;'>Details</th> " +
            "</tr> " +

            //Table Details
            "<tr><td>QR Code</td> <td>" + qrCode + "</td></tr> " +
            "<tr><td>Serial No</td> <td>" + serialNo + "</td></tr> " +
            "<tr><td>Model</td> <td>" + mcModel + "</td></tr> " +
            "<tr><td>Brand</td> <td>" + mcBrand + "</td></tr> " +
            "<tr><td>Type</td><td>" + mcType + "</td></tr>" +
            "<tr><td>Current Unit</td><td>" + crUnit + "</td></tr>" +
            "<tr><td>Current Location</td><td>" + crLocation + "</td></tr> " +
            "<tr><td>Requested Unit</td><td>" + reqUnit + "</td></tr> " +
            "<tr><td>Requested Unit</td><td>" + reqLocation + "</td></tr> " +
            "<tr><td>Comments</td><td>" + reqComment + "</td></tr></tbody></table>" +

            "</hr> " +
            "<a href='https://tmis.timexsl.com' style='font-family:Cambria'>Approve From System</a> " +
            "<table style='height: 39px;' border='2'> " +
            "<tbody> " +
            "<tr> " +
            "<td bgcolor='#00FF00' style='width: 370px; text-align: center;'><a href='" + sApprove + "' style='font-family:Cambria'>Approve</a></td> " +
            "<td bgcolor='#FF0000' style='width: 370px; text-align: center;'><a href='" + sReject + "' style='font-family:Cambria'>Reject</a></td> " +
            "</tr> " +
            "</tbody> " +
            "</table> " +

            //Auto Generate Word
            "<hr><p><font size='1'><I>This is a Auto Generated Mail From Timex Mail System Developed By Timex IT Department [Generated Date " + DateTime.Now.ToString("yyyy-MM-dd") + " Time " + DateTime.Now.ToString("hh:mm:ss") + "- From Timex TMIS]</I></font></p></body>  " +
            "</hr></html></body></html>";

            string recipientEmailTo = "rasika.dalpathadu@timexsl.com";
            string recipientEmailCc = "";
            string recipientEmailBcc = "";

            SendMail(recipientEmailTo, recipientEmailCc, recipientEmailBcc, sBody, subject);
        }

        //TIMEX Gate Pass To Approve
        public void GPRequestToApprove(params string[] myArray)
        {
            if (myArray.Length < 6)
                throw new ArgumentException("Expected at least 6 header fields");
            string baseUrl = "https://localhost:44383" ?? "https://localhost:44383"; // Configure this in appsettings.json


            string gPCode = myArray[0];
            string Locations = myArray[6];
            string gpSubject = myArray[1];
            string generatedDateTime = myArray[2];
            string attention = myArray[3];
            string gGPRemarks = myArray[4];
            string generatedBy = myArray[5];

            string approveUrl = $"{baseUrl}/api/gatepass/approve-link?gpCode={gPCode}&action=approve";
            string rejectUrl = $"{baseUrl}/api/gatepass/approve-link?gpCode={gPCode}&action=reject";

            // Build HTML table rows for details starting from index 6
            string itemTable = "<table border='1' cellpadding='5' cellspacing='0' style='border-collapse: collapse; font-family: Cambria; width: 100%;'>" +
                               "<thead><tr style='background-color: #b6f96d; text-align: center;'>" +
                               "<th>Item Name</th>" +
                               "<th>Item Description</th>" +
                               "<th>Quantity</th>" +
                               "<th>Units</th>" +
                               "</tr></thead><tbody>";

            for (int i = 7; i < myArray.Length; i++)
            {
                var parts = myArray[i].Split('|');
                if (parts.Length == 4)
                {
                    string itemName = parts[0];
                    string itemDesc = parts[1];
                    string qty = parts[2];
                    string units = parts[3];

                    itemTable += "<tr>" +
                                 $"<td>{itemName}</td>" +
                                 $"<td>{itemDesc}</td>" +
                                 $"<td>{qty}</td>" +
                                 $"<td>{units}</td>" +
                                 "</tr>";
                }
            }

            itemTable += "</tbody></table>";

            string subject = $"Gatepass Request To Approve [{gPCode}] (TMIS)";

            string sBody = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta http-equiv='Content-Type' content='text/html; charset=iso-8859-1'>
            </head>
            <body>
                <table style='border: 3px solid Blue; font-family: Cambria;'>
                    <tr><th style='width: 745px;'>You have a Gatepass Request To Approve [{gPCode}]</th></tr>
                </table>
                <hr />
                <table border='1' style='border-color: blue; border-collapse: collapse; font-family: Cambria; table-layout: fixed; width: 100%;'>
                    <tr>
                        <th colspan='2' style='background-color: #b6f96d; text-align: center;'><strong>Gatepass Details</strong></th>
                    </tr>
                    <tr><th style='width: 375px;'>Headings</th><th style='width: 375px;'>Details</th></tr>
                    <tr><td>GP Code</td><td>{gPCode}</td></tr>
                    <tr><td>Subject</td><td>{gpSubject}</td></tr>
                    <tr><td>Locations</td><td>{Locations}</td></tr>
                    <tr><td>Gen. Date Time</td><td>{generatedDateTime}</td></tr>
                    <tr><td>Attention</td><td>{attention}</td></tr>
                    <tr><td>Gen. By</td><td>{generatedBy}</td></tr>
                    <tr><td>Remarks</td><td>{gGPRemarks}</td></tr>                  
                    <tr>
                        <td colspan='2'>
                            {itemTable}
                        </td>
                    </tr>
                </table>
                <br />
                <a href='https://tmis.timexsl.com' style='font-family:Cambria'>Approve From System</a>
                <table border='2' style='height: 39px; font-family:Cambria;'>
                    <tr>
                        <td bgcolor='#00FF00' style='width: 370px; text-align: center;'><a href='{approveUrl}'>Approve</a></td>
                        <td bgcolor='#FF0000' style='width: 370px; text-align: center;'><a href='{rejectUrl}'>Reject</a></td>
                    </tr>
                </table>
                <hr>
                <p><font size='1'><i>This is an Auto Generated Mail From Timex Mail System Developed By Timex IT Department 
                [Generated Date {DateTime.Now:yyyy-MM-dd} Time {DateTime.Now:HH:mm:ss} - From Timex TMIS]</i></font></p>
            </body>
            </html>";

            string recipientEmailTo = "rasika.dalpathadu@timexsl.com";
            string recipientEmailCc = "";
            string recipientEmailBcc = "";

            SendMail(recipientEmailTo, recipientEmailCc, recipientEmailBcc, sBody, subject);
        }

        public void EPRequestToApprove(params string[] myArray)
        {
            if (myArray.Length < 6)
                throw new ArgumentException("Expected at least 6 header fields");
            string baseUrl = "https://localhost:44383" ?? "https://localhost:44383"; // Configure this in appsettings.json


            string empGpNo = myArray[0];
            string gateName = myArray[1];
            string expLoc = myArray[2];
            string expReason = myArray[3];
            string expOutTime = myArray[4];
            string isReturn = myArray[5];
            string genUser = myArray[6];

            string approveUrl = $"{baseUrl}/api/emppass/approve-link?gpCode={empGpNo}&action=approve";
            string rejectUrl = $"{baseUrl}/api/emppass/approve-link?gpCode={empGpNo}&action=reject";

            // Build HTML table rows for details starting from index 6
            string itemTable = "<table border='1' cellpadding='5' cellspacing='0' style='border-collapse: collapse; font-family: Cambria; width: 100%;'>" +
                               "<thead><tr style='background-color: #b6f96d; text-align: center;'>" +
                               "<th>Employee Name</th>" +
                               "<th>Employee No</th>" +
                               "</tr></thead><tbody>";

            for (int i = 7; i < myArray.Length; i++)
            {
                var parts = myArray[i].Split('|');
                if (parts.Length == 2)
                {
                    string empName = parts[0];
                    string empNo = parts[1];

                    itemTable += "<tr>" +
                                 $"<td>{empName}</td>" +
                                 $"<td>{empNo}</td>" +
                                 "</tr>";
                }
            }

            itemTable += "</tbody></table>";

            string subject = $"Exit Permit To Approve [{empGpNo}] (TMIS)";


            string sBody = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta http-equiv='Content-Type' content='text/html; charset=iso-8859-1'>
            </head>
            <body>
                <table style='border: 3px solid Blue; font-family: Cambria;'>
                    <tr><th style='width: 745px;'>You have a Employee Exit Permit Request To Approve [{empGpNo}]</th></tr>
                </table>
                <hr />
                <table border='1' style='border-color: blue; border-collapse: collapse; font-family: Cambria; table-layout: fixed; width: 100%;'>
                    <tr>
                        <th colspan='2' style='background-color: #b6f96d; text-align: center;'><strong>Gatepass Details</strong></th>
                    </tr>
                    <tr><th style='width: 375px;'>Headings</th><th style='width: 375px;'>Details</th></tr>
                    <tr><td>Permit Code</td><td>{empGpNo}</td></tr>
                    <tr><td>Gate Name</td><td>{gateName}</td></tr>
                    <tr><td>Locations</td><td>{expLoc}</td></tr>
                    <tr><td>Reason</td><td>{expReason}</td></tr>
                    <tr><td>Exp. Out Time</td><td>{expOutTime}</td></tr>
                    <tr><td>Is Return</td><td>{isReturn}</td></tr>
                    <tr><td>Gen By</td><td>{genUser}</td></tr>                  
                    <tr>
                        <td colspan='2'>
                            {itemTable}
                        </td>
                    </tr>
                </table>
                <br />
                <a href='https://tmis.timexsl.com' style='font-family:Cambria'>Approve From System</a>
                <table border='2' style='height: 39px; font-family:Cambria;'>
                    <tr>
                        <td bgcolor='#00FF00' style='width: 370px; text-align: center;'><a href='{approveUrl}'>Approve</a></td>
                        <td bgcolor='#FF0000' style='width: 370px; text-align: center;'><a href='{rejectUrl}'>Reject</a></td>
                    </tr>
                </table>
                <hr>
                <p><font size='1'><i>This is an Auto Generated Mail From Timex Mail System Developed By Timex IT Department 
                [Generated Date {DateTime.Now:yyyy-MM-dd} Time {DateTime.Now:HH:mm:ss} - From Timex TMIS]</i></font></p>
            </body>
            </html>";

            string recipientEmailTo = "rasika.dalpathadu@timexsl.com";
            string recipientEmailCc = "";
            string recipientEmailBcc = "";

            SendMail(recipientEmailTo, recipientEmailCc, recipientEmailBcc, sBody, subject);
        }

    }
}
