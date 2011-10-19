using System.Net;
using System.Net.Mail;
using System.Text;
using CapRaffle.Domain.Abstract;
using CapRaffle.Domain.Model;
using System;

namespace CapRaffle.Domain.Implementation
{
    using System.IO;

    // Could'nt all of these be exported to Web.config?
    public class EmailSettings
    {
        public string MailFromAddress = "capraffle@capgemini.com";
        public bool UseSsl = false;
        public string Username = "";
        public string Password = "";
        public string ServerName = "ismtp.corp.capgemini.com";
        public int ServerPort = 25;
        public bool WriteAsFile = false; //Remeber this one
        public string FileLocation = @"c:\temp\";
    }

    public class EmailSender : IEmailSender
    {
        readonly EmailSettings emailSettings;
        SmtpClient smtpClient;

        public EmailSender()
        {
            emailSettings = new EmailSettings();
            SetUpSmtpClient();
        }

        public bool ForgotPassword(string email, string newPassword)
        {
            if (smtpClient != null) {
                var body = string.Format("Your new CapRaffle password is: <strong>{0}</strong>", newPassword);

                var mailMessage = new MailMessage
                    {
                        From = new MailAddress(emailSettings.MailFromAddress),
                        Subject = "[CapRaffle] Password reset",
                        Body = body,
                        IsBodyHtml = true
                    };
                mailMessage.To.Add(new MailAddress(email));

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
            var body = "<h1>Congratulations!</h1><br />";
            body += string.Format("You've won <strong>{0}</strong> {2} in the raffle for event: <strong>{1}</strong> <br />", 
                winner.NumberOfSpotsWon, winner.Event.Name, winner.NumberOfSpotsWon == 1 ? "ticket" : "tickets");
            body += string.Format("Please look at the event details or contact <a href=\"{0}\">{0}</a> to get {1}.", 
                winner.Event.Creator, winner.NumberOfSpotsWon == 1 ? "it" : "them");

            var mailMessage = new MailMessage()
                {
                    From = new MailAddress(emailSettings.MailFromAddress),
                    Subject = string.Format("[CapRaffle] {0} winner!", winner.Event.Name),
                    Body = body,
                    IsBodyHtml = true
                };
            mailMessage.To.Add(new MailAddress(winner.UserEmail));

            using (var stream = new FileStream("event.vcs", FileMode.OpenOrCreate))
            using (var writer = new StreamWriter(stream))
            {
                writer.WriteLine("BEGIN:VCALENDAR");
                writer.WriteLine("VERSION:2.0");
                writer.WriteLine("PRODID:-//hacksw/handcal//NONSGML v1.0//EN");
                writer.WriteLine("BEGIN:VEVENT");
                writer.WriteLine("DTSTAMP:{0}", ToCalendarDateString(winner.Event.Created));
                writer.WriteLine("ORGANIZER:mailto:{0}", winner.Event.Creator);
                writer.WriteLine("DTSTART:{0}", ToCalendarDateString(winner.Event.StartTime));
                // End time
                // writer.WriteLine("DTEND:{0}", ToCalendarDateString(winner.Event.DeadLine));
                writer.WriteLine("SUMMARY:{0}", winner.Event.Name);
                writer.WriteLine("END:VEVENT");
                writer.WriteLine("END:VCALENDAR");
            }

            mailMessage.Attachments.Add(new Attachment("event.vcs"));
           
            if (emailSettings.WriteAsFile)
            {
                mailMessage.BodyEncoding = Encoding.ASCII;
            }
            return SendEmail(mailMessage);
        }

        private static string ToCalendarDateString(DateTime dateTime)
        {
            return dateTime.ToString("yyyyMMddTHHmmssZ");
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
                Console.WriteLine("Exception caught in SendEmail(): {0}", ex);
                return false;
            }
            return true;
        }

        private  void SetUpSmtpClient()
        {
            smtpClient = new SmtpClient
                {
                    EnableSsl = this.emailSettings.UseSsl,
                    Host = this.emailSettings.ServerName,
                    Port = this.emailSettings.ServerPort,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(this.emailSettings.Username, this.emailSettings.Password)
                };

            if (!this.emailSettings.WriteAsFile)
                return;

            smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
            smtpClient.PickupDirectoryLocation = this.emailSettings.FileLocation;
            smtpClient.EnableSsl = false;
        }
    }
}
