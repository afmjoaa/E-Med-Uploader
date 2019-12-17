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
            string codeToSend = "123456";
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            SmtpServer.EnableSsl = true;
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("emed.uploader@gmail.com");
                mail.To.Add(email);
                mail.Subject = "E-Med patient account registration";
                mail.Body = "Thank you for registering with E-Med services .." +
                            "\nYou can find more of our services from the E-Med Patient Mobile app.. " +
                    "\n Download it from Play store or Apple app store..." +
                    "\n And register with your associated mobile number and your are good to go ..";

               
                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("emed.uploader@gmail.com", "emed123456");
               
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