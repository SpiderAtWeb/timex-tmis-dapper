using System.Net;
using System.Net.Mail;

namespace TMIS.Utility
{
    public class GmailSender
    {
        // Gmail SMTP server settings
        string smtpServer = "smtp.gmail.com";
        int smtpPort = 587;
        string senderEmail = "mism@timexsl.com";
        string senderPassword = "twcd wdxx ybxp iyzd"; // Use an app password if 2FA is enabled.
       

        private void SendMail(string mailTo, string mailCc, string mailBcc, string mailBody, string mailSubject)
        {
            try
            {
                // Create the MailMessage object
                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(senderEmail, "TMIS Messenger"),
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
                SmtpClient smtpClient = new(smtpServer, smtpPort)
                {
                    Credentials = (ICredentialsByHost)new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true, // Secure connection
                    UseDefaultCredentials = false,
                    Timeout = 20000
                };

                // Send the email
                smtpClient.Send(mail);
                Console.WriteLine("Email sent successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending email: " + ex.Message);
            }
        }

        //JK Gate Pass To Approve
        public void RequestToApprove(params string[] myArray)
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
    }
}
