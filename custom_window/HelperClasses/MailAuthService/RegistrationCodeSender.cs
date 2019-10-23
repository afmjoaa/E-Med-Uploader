using System;
using System.Diagnostics;
using System.Net.Mail;

namespace custom_window.HelperClasses.MailAuthService
{
    public class RegistrationCodeSender
    {
        #region Init

        private static RegistrationCodeSender _instance = null;

        private RegistrationCodeSender()
        {
        }

        public static RegistrationCodeSender GetInstance()
        {
            return _instance ?? (_instance = new RegistrationCodeSender());
        }

        #endregion

        public string SendCodeToEmail(string email)
        {
            string codeToSend = GeneratCode();

            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("xumeinc@gmail.com");
                mail.To.Add(email);
                mail.Subject = "Test Mail";
                mail.Body = "This is for testing SMTP mail from GMAIL";

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("xumeinc@gmail.com", "xumedume101430");
                SmtpServer.EnableSsl = true;
                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw ex;
            }

            return codeToSend;
        }

        public string GeneratCode()
        {
            var ret = "";
            ret += (new Random().Next(1000000).ToString());
            while (ret.Length < 5)
            {
                ret += (new Random()).Next(9).ToString();
            }

            return ret;
        }
    }
}