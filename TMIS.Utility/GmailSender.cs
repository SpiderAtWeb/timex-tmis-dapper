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
                MailMessage mail = new()
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

        public void EPRequestToApprove(params string[] myArray)
        {
            if (myArray.Length < 6)
                throw new ArgumentException("Expected at least 6 header fields");
            string baseUrl = _configuration["BaseUrl"] ?? "https://localhost:44383"; // Configure this in appsettings.json


            string empGpNo = myArray[0];
            string gateName = myArray[1];
            string expLoc = myArray[2];
            string expReason = myArray[3];
            string expOutTime = myArray[4];
            string isReturn = myArray[5];
            string genUser = myArray[6];

            string approveUrl = $"{baseUrl}/api/gatepass/emp-approve?gatepassNumber={empGpNo}&action=approve";
            string rejectUrl = $"{baseUrl}/api/gatepass/emp-approve?gatepassNumber={empGpNo}&action=reject";

            // Build HTML table rows for details starting from index 6
            string itemTable = @"<table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"" style=""background-color: #ffffff; border-radius: 10px; margin-bottom: 15px; border: 1px solid #e2e8f0; box-shadow: 0 2px 4px rgba(0,0,0,0.1);"">
                                    <tr>
                                    <td style=""padding: 15px;"">
                                    <!-- Items Table -->
                                    <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"" style=""border-radius: 8px; overflow: hidden; border: 1px solid #e2e8f0;"">
                                    <!-- Table Header -->
                                    <tr style=""background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); background-color: #667eea;"">
                                    <td style=""padding: 10px; text-align: center; font-weight: 600; color: #ffffff; border-right: 1px solid rgba(255,255,255,0.3);"">Employee Name</td>
                                    <td style=""padding: 10px; text-align: center; font-weight: 600; color: #ffffff;"">Employee Number</td>
                                </tr>";

            for (int i = 7; i < myArray.Length; i++)
            {
                var parts = myArray[i].Split('|');
                if (parts.Length == 2)
                {
                    string empName = parts[0];
                    string empNo = parts[1];

                    itemTable += $@"<tr style=""background-color: #f8fafc;"">
                        <td style = ""padding: 10px; color: #475569; border-right: 1px solid #e2e8f0;"">{empName}</td>
                        <td style = ""padding: 10px; color: #475569; text-align: center;"">{empNo}</td>
                    </tr> ";
                }
            }

            itemTable += "</table></td></tr></table>";

            string subject = $"Exit Permit To Approve [{empGpNo}] (TMIS)";


            string sBody = $@"<!DOCTYPE html>
                            <html>
                            <head>
                                <meta http-equiv='Content-Type' content='text/html; charset=utf-8'>
                                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                            </head>
                            <body style=""margin: 0; padding: 0; font-family: Arial, Helvetica, sans-serif; font-size: 14px; background-color: #f4f4f4;"">
                                <!-- Main Container -->
                                <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"" style=""background-color: #f4f4f4;"">
                                    <tr>
                                        <td align=""center"" style=""padding: 20px 0;"">
                                            <!-- Email Container -->
                                            <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""600"" style=""max-width: 600px; background-color: #ffffff; border-radius: 10px; box-shadow: 0 4px 8px rgba(0,0,0,0.1); overflow: hidden;"">
                    
                                                <!-- Header -->
                                                <tr>
                                                    <td style=""background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); background-color: #667eea; color: #ffffff; padding: 15px; text-align: center;"">
                                                        <h1 style=""margin: 0 0 10px 0; font-size: 18px; font-weight: 600; color: #ffffff;"">Exit Permit For Approval</h1>
                                                        <div style=""background-color: rgba(255,255,255,0.2); padding: 8px 16px; border-radius: 10px; display: inline-block; font-weight: 500; color: #ffffff;"">
                                                            {empGpNo}
                                                        </div>
                                                    </td>
                                                </tr>
                    
                                                <!-- Content -->
                                                <tr>
                                                    <td style=""padding: 15px;"">
                                                        <!-- Details Card -->
                                                        <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"" style=""background-color: #f8fafc; border-radius: 10px; margin-bottom: 15px; border: 1px solid #e2e8f0;"">
                                                            <tr>
                                                                <td style=""padding: 15px;"">
                                                                    <!-- Details Header -->
                                                                    <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"" style=""margin-bottom: 15px;"">
                                                                        <tr>
                                                                            <td style=""background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); background-color: #667eea; color: #ffffff; padding: 10px; border-radius: 8px; text-align: center; font-weight: 600; font-size: 16px;"">
                                                                                Exit Permit Details
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                        
                                                                    <!-- Details Table -->
                                                                    <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"">                                                                   
                                                                        <tr style=""border-bottom: 1px solid #e2e8f0;"">
                                                                            <td style=""background-color: #f1f5f9; padding: 8px 12px; font-weight: 600; color: #475569; border-radius: 6px 0 0 6px;"">Gate Name</td>
                                                                            <td style=""padding: 8px 12px; color: #334155; border-radius: 0 6px 6px 0;"">{gateName}</td>
                                                                        </tr>
                                                                        <tr style=""border-bottom: 1px solid #e2e8f0;"">
                                                                            <td style=""background-color: #f1f5f9; padding: 8px 12px; font-weight: 600; color: #475569; border-radius: 6px 0 0 6px;"">Location</td>
                                                                            <td style=""padding: 8px 12px; color: #334155; border-radius: 0 6px 6px 0;"">{expLoc}</td>
                                                                        </tr>
                                                                        <tr style=""border-bottom: 1px solid #e2e8f0;"">
                                                                            <td style=""background-color: #f1f5f9; padding: 8px 12px; font-weight: 600; color: #475569; border-radius: 6px 0 0 6px;"">Reason</td>
                                                                            <td style=""padding: 8px 12px; color: #334155; border-radius: 0 6px 6px 0;"">{expReason}</td>
                                                                        </tr>
                                                                        <tr style=""border-bottom: 1px solid #e2e8f0;"">
                                                                            <td style=""background-color: #f1f5f9; padding: 8px 12px; font-weight: 600; color: #475569; border-radius: 6px 0 0 6px;"">Expected Out</td>
                                                                            <td style=""padding: 8px 12px; color: #334155; border-radius: 0 6px 6px 0;"">{expOutTime}</td>
                                                                        </tr>
                                                                        <tr style=""border-bottom: 1px solid #e2e8f0;"">
                                                                            <td style=""background-color: #f1f5f9; padding: 8px 12px; font-weight: 600; color: #475569; border-radius: 6px 0 0 6px;"">Generated User</td>
                                                                            <td style=""padding: 8px 12px; color: #334155; border-radius: 0 6px 6px 0;"">{genUser}</td>
                                                                        </tr>                                                                       
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                        </table>
                            
                                                        <!-- Items Card -->
                                                         {itemTable}       
                            
                                                        <!-- System Link and Action Buttons -->
                                                        <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"">
                                                            <tr>
                                                                <td align=""center"" style=""padding: 20px 0;"">
                                                                    <!-- System Link -->
                                                                    <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" style=""margin-bottom: 15px;"">
                                                                        <tr>
                                                                            <td style=""background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); background-color: #667eea; border-radius: 20px;"">
                                                                                <a href=""https://tmis.timexsl.com"" style=""display: inline-block; padding: 10px 20px; color: #ffffff; text-decoration: none; font-weight: 500; border-radius: 20px;"">
                                                                                    Approve From System
                                                                                </a>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                        
                                                                    <!-- Action Buttons -->
                                                                    <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"">
                                                                        <tr>
                                                                            <!-- Approve Button -->
                                                                            <td style=""padding-right: 5px;"">
                                                                                <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"">
                                                                                    <tr>
                                                                                        <td style=""background: linear-gradient(135deg, #48bb78 0%, #38a169 100%); background-color: #48bb78; border-radius: 12px; box-shadow: 0 4px 8px rgba(0,0,0,0.1);"">
                                                                                            <a href='{approveUrl}' style=""display: inline-block; width: 150px; padding: 12px 20px; color: #ffffff; text-decoration: none; font-weight: 600; text-align: center; border-radius: 12px;"">
                                                                                                ✓ Approve
                                                                                            </a>
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                            </td>
                                                
                                                                            <!-- Reject Button -->
                                                                            <td style=""padding-left: 5px;"">
                                                                                <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"">
                                                                                    <tr>
                                                                                        <td style=""background: linear-gradient(135deg, #f56565 0%, #e53e3e 100%); background-color: #f56565; border-radius: 12px; box-shadow: 0 4px 8px rgba(0,0,0,0.1);"">
                                                                                            <a href='{rejectUrl}' style=""display: inline-block; width: 150px; padding: 12px 20px; color: #ffffff; text-decoration: none; font-weight: 600; text-align: center; border-radius: 12px;"">
                                                                                                ✗ Reject
                                                                                            </a>
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                    
                                                <!-- Footer -->
                                                <tr>
                                                    <td style=""background-color: #f7fafc; padding: 15px; text-align: center; color: #718096; font-size: 11px; font-style: italic; border-top: 1px solid #e2e8f0;"">
                                                        This is an Auto Generated Mail From Timex Mail System Developed By Timex IT Department<br>
                                                        [Generated Date {DateTime.Now:yyyy-MM-dd} Time {DateTime.Now:HH:mm:ss} - From Timex TMIS]
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </body>
                            </html>";

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
            string baseUrl = _configuration["BaseUrl"] ?? "https://localhost:44383"; // Configure this in appsettings.json

            string gPCode = myArray[0];
            string Locations = myArray[6];
            string gpSubject = myArray[1];
            string generatedDateTime = myArray[2];
            string attention = myArray[3];
            string gGPRemarks = myArray[4];
            string generatedBy = myArray[5];

            string approveUrl = $"{baseUrl}/api/gatepass/gp-approve?gatepassNumber={gPCode}&action=approve";
            string rejectUrl = $"{baseUrl}/api/gatepass/gp-approve?gatepassNumber={gPCode}&action=reject";

            // Build HTML table rows for details starting from index 6
            string itemTable = @"<table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"" style=""background-color: #ffffff; border-radius: 10px; margin-bottom: 15px; border: 1px solid #e2e8f0; box-shadow: 0 2px 4px rgba(0,0,0,0.1);"">
                                    <tr>
                                    <td style=""padding: 15px;"">
                                    <!-- Items Table -->
                                    <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"" style=""border-radius: 8px; overflow: hidden; border: 1px solid #e2e8f0;"">
                                    <!-- Table Header -->
                                    <tr style=""background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); background-color: #667eea;"">
                                    <td style=""padding: 10px; text-align: center; font-weight: 600; color: #ffffff; border-right: 1px solid rgba(255,255,255,0.3);"">Item Name</td>
                                    <td style=""padding: 10px; text-align: center; font-weight: 600; color: #ffffff; border-right: 1px solid rgba(255,255,255,0.3);"">Item Description</td>
                                    <td style=""padding: 10px; text-align: center; font-weight: 600; color: #ffffff; border-right: 1px solid rgba(255,255,255,0.3);"">Quantity</td>
                                    <td style=""padding: 10px; text-align: center; font-weight: 600; color: #ffffff;"">Units</td>
                                </tr>";

            for (int i = 7; i < myArray.Length; i++)
            {
                var parts = myArray[i].Split('|');
                if (parts.Length == 4)
                {
                    string itemName = parts[0];
                    string itemDesc = parts[1];
                    string qty = parts[2];
                    string units = parts[3];

                    itemTable += $@"<tr style=""background-color: #f8fafc;"">
                        <td style=""padding: 10px; color: #475569; border-right: 1px solid #e2e8f0;"">{itemName}</td>
                        <td style=""padding: 10px; color: #475569; border-right: 1px solid #e2e8f0;"">{itemDesc}</td>
                        <td style=""padding: 10px; color: #475569; text-align: center; border-right: 1px solid #e2e8f0;"">{qty}</td>
                        <td style=""padding: 10px; color: #475569; text-align: center;"">{units}</td>
                    </tr>";
                }
            }

            itemTable += "</table></td></tr></table>";

            string subject = $"Gatepass Request To Approve [{gPCode}] (TMIS)";

            string sBody = $@"<!DOCTYPE html>
                            <html>
                            <head>
                                <meta http-equiv='Content-Type' content='text/html; charset=utf-8'>
                                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                            </head>
                            <body style=""margin: 0; padding: 0; font-family: Arial, Helvetica, sans-serif; font-size: 14px; background-color: #f4f4f4;"">
                                <!-- Main Container -->
                                <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"" style=""background-color: #f4f4f4;"">
                                    <tr>
                                        <td align=""center"" style=""padding: 20px 0;"">
                                            <!-- Email Container -->
                                            <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""600"" style=""max-width: 600px; background-color: #ffffff; border-radius: 10px; box-shadow: 0 4px 8px rgba(0,0,0,0.1); overflow: hidden;"">
                    
                                                <!-- Header -->
                                                <tr>
                                                    <td style=""background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); background-color: #667eea; color: #ffffff; padding: 15px; text-align: center;"">
                                                        <h1 style=""margin: 0 0 10px 0; font-size: 18px; font-weight: 600; color: #ffffff;"">Gatepass Request For Approval</h1>
                                                        <div style=""background-color: rgba(255,255,255,0.2); padding: 8px 16px; border-radius: 10px; display: inline-block; font-weight: 500; color: #ffffff;"">
                                                            {gPCode}
                                                        </div>
                                                    </td>
                                                </tr>
                    
                                                <!-- Content -->
                                                <tr>
                                                    <td style=""padding: 15px;"">
                                                        <!-- Details Card -->
                                                        <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"" style=""background-color: #f8fafc; border-radius: 10px; margin-bottom: 15px; border: 1px solid #e2e8f0;"">
                                                            <tr>
                                                                <td style=""padding: 15px;"">
                                                                    <!-- Details Header -->
                                                                    <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"" style=""margin-bottom: 15px;"">
                                                                        <tr>
                                                                            <td style=""background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); background-color: #667eea; color: #ffffff; padding: 10px; border-radius: 8px; text-align: center; font-weight: 600; font-size: 16px;"">
                                                                                Gatepass Details
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                        
                                                                    <!-- Details Table -->
                                                                    <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"">                                                                   
                                                                        <tr style=""border-bottom: 1px solid #e2e8f0;"">
                                                                            <td style=""background-color: #f1f5f9; padding: 8px 12px; font-weight: 600; color: #475569; border-radius: 6px 0 0 6px;"">Subject</td>
                                                                            <td style=""padding: 8px 12px; color: #334155; border-radius: 0 6px 6px 0;"">{gpSubject}</td>
                                                                        </tr>
                                                                        <tr style=""border-bottom: 1px solid #e2e8f0;"">
                                                                            <td style=""background-color: #f1f5f9; padding: 8px 12px; font-weight: 600; color: #475569; border-radius: 6px 0 0 6px;"">Locations</td>
                                                                            <td style=""padding: 8px 12px; color: #334155; border-radius: 0 6px 6px 0;"">{Locations}</td>
                                                                        </tr>
                                                                        <tr style=""border-bottom: 1px solid #e2e8f0;"">
                                                                            <td style=""background-color: #f1f5f9; padding: 8px 12px; font-weight: 600; color: #475569; border-radius: 6px 0 0 6px;"">Generated Date Time</td>
                                                                            <td style=""padding: 8px 12px; color: #334155; border-radius: 0 6px 6px 0;"">{generatedDateTime}</td>
                                                                        </tr>
                                                                        <tr style=""border-bottom: 1px solid #e2e8f0;"">
                                                                            <td style=""background-color: #f1f5f9; padding: 8px 12px; font-weight: 600; color: #475569; border-radius: 6px 0 0 6px;"">Attention</td>
                                                                            <td style=""padding: 8px 12px; color: #334155; border-radius: 0 6px 6px 0;"">{attention}</td>
                                                                        </tr>
                                                                        <tr style=""border-bottom: 1px solid #e2e8f0;"">
                                                                            <td style=""background-color: #f1f5f9; padding: 8px 12px; font-weight: 600; color: #475569; border-radius: 6px 0 0 6px;"">Generated By</td>
                                                                            <td style=""padding: 8px 12px; color: #334155; border-radius: 0 6px 6px 0;"">{generatedBy}</td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td style=""background-color: #f1f5f9; padding: 8px 12px; font-weight: 600; color: #475569; border-radius: 6px 0 0 6px;"">Remarks</td>
                                                                            <td style=""padding: 8px 12px; color: #334155; border-radius: 0 6px 6px 0;"">{gGPRemarks}</td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                        </table>
                            
                                                        <!-- Items Card -->
                                                         {itemTable}       
                            
                                                        <!-- System Link and Action Buttons -->
                                                        <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"">
                                                            <tr>
                                                                <td align=""center"" style=""padding: 20px 0;"">
                                                                    <!-- System Link -->
                                                                    <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" style=""margin-bottom: 15px;"">
                                                                        <tr>
                                                                            <td style=""background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); background-color: #667eea; border-radius: 20px;"">
                                                                                <a href=""https://tmis.timexsl.com"" style=""display: inline-block; padding: 10px 20px; color: #ffffff; text-decoration: none; font-weight: 500; border-radius: 20px;"">
                                                                                    Approve From System
                                                                                </a>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                        
                                                                    <!-- Action Buttons -->
                                                                    <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"">
                                                                        <tr>
                                                                            <!-- Approve Button -->
                                                                            <td style=""padding-right: 5px;"">
                                                                                <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"">
                                                                                    <tr>
                                                                                        <td style=""background: linear-gradient(135deg, #48bb78 0%, #38a169 100%); background-color: #48bb78; border-radius: 12px; box-shadow: 0 4px 8px rgba(0,0,0,0.1);"">
                                                                                            <a href='{approveUrl}' style=""display: inline-block; width: 150px; padding: 12px 20px; color: #ffffff; text-decoration: none; font-weight: 600; text-align: center; border-radius: 12px;"">
                                                                                                ✓ Approve
                                                                                            </a>
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                            </td>
                                                
                                                                            <!-- Reject Button -->
                                                                            <td style=""padding-left: 5px;"">
                                                                                <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"">
                                                                                    <tr>
                                                                                        <td style=""background: linear-gradient(135deg, #f56565 0%, #e53e3e 100%); background-color: #f56565; border-radius: 12px; box-shadow: 0 4px 8px rgba(0,0,0,0.1);"">
                                                                                            <a href='{rejectUrl}' style=""display: inline-block; width: 150px; padding: 12px 20px; color: #ffffff; text-decoration: none; font-weight: 600; text-align: center; border-radius: 12px;"">
                                                                                                ✗ Reject
                                                                                            </a>
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                    
                                                <!-- Footer -->
                                                <tr>
                                                    <td style=""background-color: #f7fafc; padding: 15px; text-align: center; color: #718096; font-size: 11px; font-style: italic; border-top: 1px solid #e2e8f0;"">
                                                        This is an Auto Generated Mail From Timex Mail System Developed By Timex IT Department<br>
                                                        [Generated Date {DateTime.Now:yyyy-MM-dd} Time {DateTime.Now:HH:mm:ss} - From Timex TMIS]
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </body>
                            </html>";

            string recipientEmailTo = "rasika.dalpathadu@timexsl.com";
            string recipientEmailCc = "";
            string recipientEmailBcc = "rasika.dalpathadu@timexsl.com";

            SendMail(recipientEmailTo, recipientEmailCc, recipientEmailBcc, sBody, subject);
        }
    }
}
