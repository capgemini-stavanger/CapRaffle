using System.Net;
using System.Net.Mail;
using System.Text;
using CapRaffle.Domain.Abstract;

namespace CapRaffle.Domain.Implementation
{
   public class EmailSettings
    {
        public string MailFromAddress = "DoNotReplyCapRaffle@capgemini.com";
        public bool UseSsl = false;
        public string Username = "SMTPusername";
        public string Password = "SMTPpassword";
        public string ServerName = "smtp.server.com";
        public int ServerPort = 25;
        public bool WriteAsFile = true; //Remeber this one
        public string FileLocation = @"c:\temp\";
    }

    public class EmailSender : IEmailSender
    {
        EmailSettings emailSettings;

        public EmailSender()
        {
            emailSettings = new EmailSettings();
        }

        public void ForgotPassword(string email, string newPassword)
        {
            using (var smtpClient = new SmtpClient())
            {

                smtpClient.EnableSsl = emailSettings.UseSsl;
                smtpClient.Host = emailSettings.ServerName;
                smtpClient.Port = emailSettings.ServerPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials
                = new NetworkCredential(emailSettings.Username, emailSettings.Password);

                if (emailSettings.WriteAsFile)
                {
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.PickupDirectoryLocation = emailSettings.FileLocation;
                    smtpClient.EnableSsl = false;
                }

                string body = string.Format("Your new CapRaffle password is: {0}", newPassword);

                MailMessage mailMessage = new MailMessage(
                    emailSettings.MailFromAddress, // From
                    email, // To
                    "CapRaffle password", // Subject
                    body.ToString()
                    ); // Body

                if (emailSettings.WriteAsFile)
                {
                    mailMessage.BodyEncoding = Encoding.ASCII;
                }
                smtpClient.Send(mailMessage);
            }
        }
    }
}
