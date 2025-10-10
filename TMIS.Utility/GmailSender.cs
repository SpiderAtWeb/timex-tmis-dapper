using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using TMIS.Models.HRRS;

namespace TMIS.Utility
{
    public class GmailSender(IConfiguration configuration, ILogger<GmailSender> logger) : IGmailSender
    {
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
                    From = new MailAddress(smtpSettings["senderEmail"]!, "TMIS Messenger"),
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
                SmtpClient smtpClient = new(smtpSettings["Host"], int.Parse(smtpSettings["Port"]!))
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
            string sApprove = "mailto:sujitha.costha@timexsl.com?subject=Approved [" + qrCode + "] TMIS(" + entryNo + ")";
            string sReject = "mailto:sujitha.costha@timexsl.com?subject=Rejected [" + qrCode + "] TMIS(" + entryNo + ")";

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

            string recipientEmailTo = "sujitha.costha@timexsl.com";
            string recipientEmailCc = "";
            string recipientEmailBcc = "";

            SendMail(recipientEmailTo, recipientEmailCc, recipientEmailBcc, sBody, subject);
        }

        //Emp Permit To Approve
        public void EPRequestToApprove(string mailTo, string[] myArray)
        {
            if (myArray.Length < 6)
                throw new ArgumentException("Expected at least 6 header fields");
            string baseUrl = _configuration["BaseUrl"] ?? "https://localhost:44383";

            string empGpNo = myArray[0];
            string subjectText = $"Exit Permit Request For Approval [{empGpNo}] (TMIS)";
            var encryptedGpCode = SecurityBox.EncryptString(empGpNo);

            var mainComps = new Dictionary<string, string>
            {
                { "MailHeading", "Exit Permit Request For Approval" },
                { "ReferenceNumber", empGpNo },
                { "TableHeading", "Exit Permit Details" },
                { "DetailsHeadA", "Employee Name" },
                { "DetailsHeadB", "Employee Number" },
                { "MailDate", DateTime.Now.ToString("yyyy-MM-dd") },
                { "MailTime", DateTime.Now.ToString("HH:mm:ss") },
                { "approveUrl", $"{baseUrl}/api/endorse/emp-approve?gatepassNumber={encryptedGpCode}&action=approve" },
                { "rejectUrl",  $"{baseUrl}/api/endorse/emp-approve?gatepassNumber={encryptedGpCode}&action=reject" }
            };

            var headerRowsH = new List<(string, string)>
            {
                ("Gate Name", myArray[1]),
                ("Location", myArray[2]),
                ("Reason", myArray[3]),
                ("Out Time", myArray[4]),
                ("Return", myArray[5]),
                ("Generated User", myArray[6]),
            };

            var headerD = new List<(string, string)>();

            for (int i = 7; i < myArray.Length; i++)
            {
                var parts = myArray[i].Split('|');
                if (parts.Length == 2)
                {
                    headerD.Add((parts[0], parts[1]));
                }
            }

            string emailBody = EMailFormatRead.GetApprovalTwoColumnsEmailBody(mainComps, headerRowsH, headerD);

            string recipientEmailTo = mailTo;
            string recipientEmailCc = "";
            string recipientEmailBcc = "sujitha.costha@timexsl.com";

            SendMail(recipientEmailTo, recipientEmailCc, recipientEmailBcc, emailBody, subjectText);
        }

        //Gate Pass To Approve
        public void GPRequestToApprove(string mailTo, string[] myArray)
        {
            if (myArray.Length < 6)
                throw new ArgumentException("Expected at least 6 header fields");
            string baseUrl = _configuration["BaseUrl"] ?? "https://localhost:44383"; // Configure this in appsettings.json

            string gPCode = myArray[0];

            string subjectText = $"Goods Gatepass Request For Approval [{gPCode}] (TMIS)";
            var encryptedGpCode = SecurityBox.EncryptString(gPCode);

            var mainComps = new Dictionary<string, string>
            {
                { "MailHeading", "Goods Gatepass Request For Approval" },
                { "ReferenceNumber", gPCode },
                { "TableHeading", "Goods Gatepass Details" },
                { "DetailsHeadA", "Item Name" },
                { "DetailsHeadB", "Item Description" },
                { "DetailsHeadC", "Quantity" },
                { "DetailsHeadD", "Units" },
                { "MailDate", DateTime.Now.ToString("yyyy-MM-dd") },
                { "MailTime", DateTime.Now.ToString("HH:mm:ss") },
                { "approveUrl", $"{baseUrl}/api/endorse/gp-approve?gatepassNumber={encryptedGpCode}&action=approve" },
                { "rejectUrl",  $"{baseUrl}/api/endorse/gp-approve?gatepassNumber={encryptedGpCode}&action=reject" }
            };

            var headerRowsH = new List<(string, string)>
            {
                ("Subject", myArray[1]),
                ("Locations", myArray[6]),
                ("Generated Date Time", myArray[2]),
                ("Attention", myArray[3]),
                ("Generated By", myArray[5]),
                ("Remarks", myArray[4]),
            };

            var headerD = new List<(string, string, string, string)>();

            for (int i = 7; i < myArray.Length; i++)
            {
                var parts = myArray[i].Split('|');
                if (parts.Length == 4)
                {
                    headerD.Add((parts[0], parts[1], parts[2], parts[3]));
                }
            }

            string emailBody = EMailFormatRead.GetApprovalThreeColumnsEmailBody(mainComps, headerRowsH, headerD);

            string recipientEmailTo = mailTo;
            string recipientEmailCc = "";
            string recipientEmailBcc = "sujitha.costha@timexsl.com";

            SendMail(recipientEmailTo, recipientEmailCc, recipientEmailBcc, emailBody, subjectText);
        }

        #region - ITIS EMAIL AREA

        //Request To Approve
        public void RequestToApprove(string mailTo, string[] myArray)
        {
            if (myArray.Length < 8)
                throw new ArgumentException("Expected at least 6 header fields");
            string baseUrl = _configuration["BaseUrl"] ?? "https://localhost:44383"; // Configure this in appsettings.json

            string refID = myArray[0];

            string subjectText = $"Device Assignment Request For Approval [{refID}] (TMIS)";
            var encryptedCode = SecurityBox.EncryptString(refID);

            var mainComps = new Dictionary<string, string>
            {
                { "MailHeading", "Device Assignment Request For Approval" },
                { "ReferenceNumber", refID },
                { "TableHeading", "Device Assignment Details" },
                { "DetailsHeadA", "Emp Name" },
                { "DetailsHeadB", "Emp #" },
                { "DetailsHeadC", "Email" },
                { "DetailsHeadD", "Designation" },
                { "MailDate", DateTime.Now.ToString("yyyy-MM-dd") },
                { "MailTime", DateTime.Now.ToString("HH:mm:ss") },
                { "approveUrl", $"{baseUrl}/api/endorse/device-approve?assignmentID={encryptedCode}&action=approve" },
                { "rejectUrl",  $"{baseUrl}/api/endorse/device-approve?assignmentID={encryptedCode}&action=reject" }
            };

            var headerRowsH = new List<(string, string)>
            {
                ("Device", myArray[1]),
                ("Serial", myArray[2]),
                ("Assigned Date Time", myArray[3]),
                ("Assigned By", myArray[4]),
                ("Assigned Location", myArray[6]),
                ("Assigned Department", myArray[7]),
                ("Remarks", myArray[5]),
            };

            var headerD = new List<(string, string, string, string)>();

            headerD.Add((myArray[8], myArray[9], myArray[10], myArray[11]));


            string emailBody = EMailFormatRead.GetApprovalThreeColumnsITISEmailBody(mainComps, headerRowsH, headerD);

            string recipientEmailTo = mailTo;
            string recipientEmailCc = "";
            string recipientEmailBcc = "sujitha.costha@timexsl.com";

            SendMailITIS(recipientEmailTo, recipientEmailCc, recipientEmailBcc, emailBody, subjectText);
        }

        private void SendMailITIS(string mailTo, string mailCc, string mailBcc, string mailBody, string mailSubject)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("SmtpSettings");

                // Create the MailMessage object
                MailMessage mail = new()
                {
                    From = new MailAddress(smtpSettings["senderEmail"]!, "TMIS Messenger"),
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
                SmtpClient smtpClient = new(smtpSettings["Host"], int.Parse(smtpSettings["Port"]!))
                {
                    Credentials = (ICredentialsByHost)new NetworkCredential(smtpSettings["senderEmail"], smtpSettings["senderPassword"]),
                    EnableSsl = true, // Secure connection
                    UseDefaultCredentials = false,
                    Timeout = 20000
                };

                // Send the email
                smtpClient.Send(mail);
                _logger.LogInformation($"Email sent for approve device Succefully {mailSubject}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send approve device email : {ex.Message}");
            }
        }

        #endregion
        #region - HRRS EMAIL AREA
        public void ITRequestToApprove(HRRS_ITRequest hRRS_ITRequest)
        {
            string baseUrl = _configuration["BaseUrl"] ?? "https://localhost:44383"; // Configure this in appsettings.json         
            var encryptedCode = SecurityBox.EncryptString(hRRS_ITRequest.RequestID.ToString());

            string subjectText = $"IT Request For Approval (TMIS) - Req# : " + hRRS_ITRequest.RequestID;

            var mainComps = new Dictionary<string, string?>
            {
                { "MailHeading", "IT Request For Approval" },
                { "TableHeading", "Request Details" },
                { "EmployeeNo", hRRS_ITRequest.EmployeeNo},
                { "FirstName", hRRS_ITRequest.FirstName},
                { "LastName", hRRS_ITRequest.LastName},
                { "Nic", hRRS_ITRequest.NIC},
                { "Location", hRRS_ITRequest.Location},
                { "Company", hRRS_ITRequest.Company},
                { "Designation" , hRRS_ITRequest.Designation},
                { "Department" , hRRS_ITRequest.Department},
                { "DueStartDate", hRRS_ITRequest.DueStartDate?.ToString("yyyy-MM-dd")},
                { "InterviewDate", hRRS_ITRequest.InterviewDate?.ToString("yyyy-MM-dd")},
                { "IsReplacement" , hRRS_ITRequest.IsReplacement? "Yes" : "No"},
                { "Replacement" , string.IsNullOrWhiteSpace(hRRS_ITRequest.Replacement) ? "-" : hRRS_ITRequest.Replacement},
                { "RequestRemark" , string.IsNullOrWhiteSpace(hRRS_ITRequest.RequestRemark) ? "-" : hRRS_ITRequest.RequestRemark},
                { "ComputerLogin" , hRRS_ITRequest.ComputerLogin? "Yes" : "No"},
                { "Email" , hRRS_ITRequest.Email? "Yes" : "No"},
                { "SAP" , hRRS_ITRequest.SAP? "Yes" : "No"},
                { "WFX" , hRRS_ITRequest.WFX? "Yes" : "No"},
                { "NewLandline" , hRRS_ITRequest.NewLandline? "Yes" : "No"},
                { "ExistingLandline" , hRRS_ITRequest.ExistingLandline? "Yes" : "No"},
                { "Extension" , hRRS_ITRequest.Extension? "Yes" : "No"},
                { "SIM" , hRRS_ITRequest.SIM? "Yes" : "No"},
                { "BasicPhone" , hRRS_ITRequest.BasicPhone? "Yes" : "No"},
                { "SmartPhone" , hRRS_ITRequest.SmartPhone? "Yes" : "No"},
                { "HomeAddress" , string.IsNullOrWhiteSpace(hRRS_ITRequest.HomeAddress) ? "-" : hRRS_ITRequest.HomeAddress},
                { "EmailGroup" , string.IsNullOrWhiteSpace(hRRS_ITRequest.EmailGroup) ? "-" : hRRS_ITRequest.EmailGroup},
                { "Computer" , hRRS_ITRequest.Computer},
                { "MailDate", DateTime.Now.ToString("yyyy-MM-dd") },
                { "MailTime", DateTime.Now.ToString("HH:mm:ss") },
                { "approveUrl", $"{baseUrl}/api/endorse/itrequest-approve?requestID={encryptedCode}&action=approve" },
                { "rejectUrl",  $"{baseUrl}/api/endorse/itrequest-approve?requestID={encryptedCode}&action=reject" }
            };


            string emailBody = EMailFormatRead.GetApprovalThreeColumnsHRRSEmailBody(mainComps!);
            string mailTo = _configuration["HRApprover"] ?? "admin@timexsl.com";
            string recipientEmailTo = mailTo;
            string recipientEmailCc = "";
            string recipientEmailBcc = "sujitha.costha@timexsl.com";

            SendEmailHRRS(recipientEmailTo, recipientEmailCc, recipientEmailBcc, emailBody, subjectText);
        }
        public void ITRequestToIT(HRRS_ITRequest hRRS_ITRequest)
        {
            string baseUrl = _configuration["BaseUrl"] ?? "https://localhost:44383"; // Configure this in appsettings.json         
            var encryptedCode = SecurityBox.EncryptString(hRRS_ITRequest.RequestID.ToString());

            string subjectText = $"IT Request (TMIS)_" + hRRS_ITRequest.FirstName + " " + hRRS_ITRequest.LastName + "_" + hRRS_ITRequest.EmployeeNo;

            var mainComps = new Dictionary<string, string?>
            {
                { "MailHeading", "NEW IT Request" },
                { "TableHeading", "Request Details" },
                { "EmployeeNo", hRRS_ITRequest.EmployeeNo},
                { "FirstName", hRRS_ITRequest.FirstName},
                { "LastName", hRRS_ITRequest.LastName},
                { "Nic", hRRS_ITRequest.NIC},
                { "Location", hRRS_ITRequest.Location},
                { "Company", hRRS_ITRequest.Company},
                { "Designation" , hRRS_ITRequest.Designation},
                { "Department" , hRRS_ITRequest.Department},
                { "DueStartDate", hRRS_ITRequest.DueStartDate?.ToString("yyyy-MM-dd")},
                { "InterviewDate", hRRS_ITRequest.InterviewDate?.ToString("yyyy-MM-dd")},
                { "IsReplacement" , hRRS_ITRequest.IsReplacement? "Yes" : "No"},
                { "Replacement" , string.IsNullOrWhiteSpace(hRRS_ITRequest.Replacement) ? "-" : hRRS_ITRequest.Replacement},
                { "RequestRemark" , string.IsNullOrWhiteSpace(hRRS_ITRequest.RequestRemark) ? "-" : hRRS_ITRequest.RequestRemark},
                { "ComputerLogin" , hRRS_ITRequest.ComputerLogin? "Yes" : "No"},
                { "Email" , hRRS_ITRequest.Email? "Yes" : "No"},
                { "SAP" , hRRS_ITRequest.SAP? "Yes" : "No"},
                { "WFX" , hRRS_ITRequest.WFX? "Yes" : "No"},
                { "NewLandline" , hRRS_ITRequest.NewLandline? "Yes" : "No"},
                { "ExistingLandline" , hRRS_ITRequest.ExistingLandline? "Yes" : "No"},
                { "Extension" , hRRS_ITRequest.Extension? "Yes" : "No"},
                { "SIM" , hRRS_ITRequest.SIM? "Yes" : "No"},
                { "BasicPhone" , hRRS_ITRequest.BasicPhone? "Yes" : "No"},
                { "SmartPhone" , hRRS_ITRequest.SmartPhone? "Yes" : "No"},
                { "HomeAddress" , string.IsNullOrWhiteSpace(hRRS_ITRequest.HomeAddress) ? "-" : hRRS_ITRequest.HomeAddress},
                { "EmailGroup" , string.IsNullOrWhiteSpace(hRRS_ITRequest.EmailGroup) ? "-" : hRRS_ITRequest.EmailGroup},
                { "Computer" , hRRS_ITRequest.Computer},
                { "MailDate", DateTime.Now.ToString("yyyy-MM-dd") },
                { "MailTime", DateTime.Now.ToString("HH:mm:ss") }
            };


            string emailBody = EMailFormatRead.GetITRequestHRRSEmailBody(mainComps!);
            string mailTo = _configuration["ITUser"] ?? "admin@timexsl.com";
            string recipientEmailTo = mailTo;
            string recipientEmailCc = "";
            string recipientEmailBcc = "sujitha.costha@timexsl.com";

            SendEmailHRRS(recipientEmailTo, recipientEmailCc, recipientEmailBcc, emailBody, subjectText);
        }


        private void SendEmailHRRS(string mailTo, string mailCc, string mailBcc, string mailBody, string mailSubject)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("SmtpSettings");

                // Create the MailMessage object
                MailMessage mail = new()
                {
                    From = new MailAddress(smtpSettings["senderEmail"]!, "TMIS Messenger"),
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
                SmtpClient smtpClient = new(smtpSettings["Host"], int.Parse(smtpSettings["Port"]!))
                {
                    Credentials = (ICredentialsByHost)new NetworkCredential(smtpSettings["senderEmail"], smtpSettings["senderPassword"]),
                    EnableSsl = true, // Secure connection
                    UseDefaultCredentials = false,
                    Timeout = 20000
                };

                // Send the email
                smtpClient.Send(mail);
                _logger.LogInformation($"Email sent for approve ITRequest Succefully {mailSubject}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send approve ITRequest email : {ex.Message}");
            }
        }
        #endregion

        //Gate Pass To Approve
        public void SMInvoiceApproval(string ApproveById, string mailTo, int levelIndex,string[] myArray)
        {
            if (myArray.Length < 6)
                throw new ArgumentException("Expected at least 6 header fields");
            string baseUrl = _configuration["BaseUrl"] ?? "https://localhost:44383"; // Configure this in appsettings.json

            string subjectText = $"Invoice Request For Approval [{myArray[1]}] (TMIS)";

            var encryptedGpCode = SecurityBox.EncryptString(myArray[0] + "|" + ApproveById + "|" + levelIndex);

            var endPlaceholders = new Dictionary<string, string>
            {
                {"MailDate", DateTime.Now.ToString("yyyy-MM-dd")},
                {"MailTime", DateTime.Now.ToString("HH:mm:ss")},
                {"StrAPprove", $"{baseUrl}/api/endorse/sm-approve?invoiceNumber={encryptedGpCode}&action=approve"},
                {"StrReject", $"{baseUrl}/api/endorse/sm-approve?invoiceNumber={encryptedGpCode}&action=reject"},
            };

            var placeholders = new Dictionary<string, string>
            {
                //X1 to X30
                {"InvoiceNo", myArray[1]},
                {"RaisedDate", myArray[2]},
                {"Supplier", myArray[3]},
                {"Unit", myArray[4]},
                {"InvoiceFromDate", myArray[5]},
                {"InvoiceToDate", myArray[6]},
                {"CertificateRemarks", myArray[8]},
                {"SystemCalculatedSum", myArray[9]},
                {"VendorContractSum", myArray[10]},
                {"VendorAdvancePayment", myArray[11]},
                {"VendorTotalAmount", myArray[12]},
                {"PreparedBy", myArray[13]},
                {"GeneratedOn", myArray[14]},
                {"ApproveLevel2By", myArray[15]},
                {"AppLevelStat2", myArray[16]},
                {"AppLevelStat2On", myArray[17]},
                {"ApproveLevel3By", myArray[18]},
                {"AppLevelStat3", myArray[19]},
                {"AppLevelStat3On", myArray[20]},
                {"ApproveLevel4By", myArray[21]},
                {"AppLevelStat4", myArray[22]},
                {"AppLevelStat4On", myArray[23]},
                {"ApproveLevel5By", myArray[24]},
                {"AppLevelStat5", myArray[25]},
                {"AppLevelStat5On", myArray[26]},
                {"ApproveLevel6By", myArray[27]},
                {"AppLevelStat6", myArray[28]},
                {"AppLevelStat6On", myArray[29]},
                {"ProcessStartDate", myArray[30]},
            };

            var machineDetails = new List<(string, string, string, string, string, string)>();

            for (int i = 31; i < myArray.Length; i++)
            {
                var parts = myArray[i].Split('|');
                if (parts.Length == 5)
                {
                    machineDetails.Add((parts[0], parts[1], parts[2], parts[3], parts[4] +" * "+ myArray[7], (decimal.Parse(parts[4]) * decimal.Parse(myArray[7])).ToString() ));
                }
            }

            string emailBody = EMailFormatRead.GetApprovalSMIMsEmailBody(placeholders, endPlaceholders, machineDetails);

            string recipientEmailTo = mailTo;
            string recipientEmailCc = "";
            string recipientEmailBcc = "sujitha.costha@timexsl.com";

            SendMail(recipientEmailTo, recipientEmailCc, recipientEmailBcc, emailBody, subjectText);
        }

    }
}
