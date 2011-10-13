using System.Net;
using System.Net.Mail;
using System.Text;
using CapRaffle.Domain.Abstract;
using CapRaffle.Domain.Model;
using System;

namespace CapRaffle.Domain.Implementation
{
   public class EmailSettings
    {
        public string MailFromAddress = "CapRaffle@capgemini.com";
        public bool UseSsl = false;
        public string Username = "";
        public string Password = "";
        public string ServerName = "smtp.some.place.com";
        public int ServerPort = 25;
        public bool WriteAsFile = false; //Remeber this one
        public string FileLocation = @"c:\temp\";
    }

    public class EmailSender : IEmailSender
    {
        EmailSettings emailSettings;
        SmtpClient smtpClient;
        public EmailSender()
        {
            emailSettings = new EmailSettings();
            SetUpSmtpClient();
        }

        public bool ForgotPassword(string email, string newPassword)
        {
            if (smtpClient != null) {
                string body = string.Format("Your new CapRaffle password is: {0}", newPassword);

                MailMessage mailMessage = new MailMessage(
                    emailSettings.MailFromAddress, // From
                    email, // To
                    "[CapRaffle] password", // Subject
                    body.ToString()
                    ); // Body

                if (emailSettings.WriteAsFile)
                {
                    mailMessage.BodyEncoding = Encoding.ASCII;
                }
                return SendEmail(mailMessage);
            }
            return false;
        }

        public bool NotifyWinner(Winner winner)
        {
            string body = string.Format("Your won the raffle for event: {0} <br />", winner.Event.Name);
            body += string.Format("You won {0} tickets <br />", winner.NumberOfSpotsWon);
            body += string.Format("Please contact {0} to get your ticket(s)", winner.Event.Creator);
            MailMessage mailMessage = new MailMessage(
                emailSettings.MailFromAddress, // From
                winner.UserEmail, // To
                string.Format("[CapRaffle] {0} winner!", winner.Event.Name), // Subject
                body
                ); // Body
            mailMessage.IsBodyHtml = true;
           
            if (emailSettings.WriteAsFile)
            {
                mailMessage.BodyEncoding = Encoding.ASCII;
            }
            return SendEmail(mailMessage);
        }

        private bool SendEmail(MailMessage mailMessage)
        {
            if (smtpClient == null) return false;
            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in CreateTestMessage1(): {0}",
                ex.ToString());
                return false;
            }
            return true;
        }

        private  void SetUpSmtpClient()
        {
            smtpClient = new SmtpClient();
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
        }
    }
}
