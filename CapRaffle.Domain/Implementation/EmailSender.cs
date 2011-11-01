using System.Net;
using System.Net.Mail;
using System.Text;
using CapRaffle.Domain.Abstract;
using CapRaffle.Domain.Model;
using System;
using System.Configuration;

namespace CapRaffle.Domain.Implementation
{
    using System.IO;
    using System.Collections.Generic;

    // Could'nt all of these be exported to Web.config?
    public class EmailSettings
    {
        public string MailFromAddress = ConfigurationManager.AppSettings["SmtpMailFromAddress"];
        public bool UseSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["SmtpUseSsl"]);
        public string Username = ConfigurationManager.AppSettings["SmtpUsername"];
        public string Password = ConfigurationManager.AppSettings["SmtpPassword"];
        public string ServerName = ConfigurationManager.AppSettings["SmtpServer"];
        public int ServerPort = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpServerPort"]);
        public bool WriteAsFile = Convert.ToBoolean(ConfigurationManager.AppSettings["SmtpWriteAsFile"]);
        public string FileLocation = ConfigurationManager.AppSettings["SmtpFileLocation"];
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

        public bool NotifyLooser(UserEvent looser)
        {
            var body = "<h1>Sorry!</h1><br />";
            body += string.Format("You did not win any tickets in the raffle for event: <strong>{0}</strong><br />", looser.Event.Name);
            body += "Better luck next time";

            var mailMessage = new MailMessage()
                {
                    From = new MailAddress(emailSettings.MailFromAddress),
                    Subject = string.Format("[CapRaffle] {0} result", looser.Event.Name),
                    Body = body,
                    IsBodyHtml = true
                };
            mailMessage.To.Add(new MailAddress(looser.UserEmail));

            if (emailSettings.WriteAsFile)
            {
                mailMessage.BodyEncoding = Encoding.ASCII;
            }
            return SendEmail(mailMessage);
        }

        public bool NotifyCreator(Event selectedEvent)
        {
            var body = "<h1>CapRaffle results</h1><br />";
            body += "<h3>Winners</h3><br />";
            if (selectedEvent.Winners.Count > 0) body += getWinnerTable(selectedEvent.Winners);
            else body += "There was no winners for this event";
            body += "<br />";
            body += "An email has also been sent to all participants of this event";

            var mailMessage = new MailMessage()
            {
                From = new MailAddress(emailSettings.MailFromAddress),
                Subject = string.Format("[CapRaffle] {0} result", selectedEvent.Name),
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(new MailAddress(selectedEvent.Creator));

            if (emailSettings.WriteAsFile)
            {
                mailMessage.BodyEncoding = Encoding.ASCII;
            }
            return SendEmail(mailMessage);
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

            var calenderData = new StringWriter();
            calenderData.WriteLine("BEGIN:VCALENDAR");
            calenderData.WriteLine("VERSION:2.0");
            calenderData.WriteLine("PRODID:-//hacksw/handcal//NONSGML v1.0//EN");
            calenderData.WriteLine("BEGIN:VEVENT");
            calenderData.WriteLine("DTSTAMP:{0}", ToCalendarDateString(winner.Event.Created));
            calenderData.WriteLine("ORGANIZER:mailto:{0}", winner.Event.Creator);
            calenderData.WriteLine("DTSTART:{0}", ToCalendarDateString(winner.Event.StartTime));
            calenderData.WriteLine("SUMMARY:{0}", winner.Event.Name);
            calenderData.WriteLine("END:VEVENT");
            calenderData.WriteLine("END:VCALENDAR");

            using (MemoryStream memoryStream = new MemoryStream(UTF32Encoding.Default.GetBytes(calenderData.ToString())))
            {
                mailMessage.Attachments.Add(new Attachment(memoryStream, "event.vcs"));
            }
            if (emailSettings.WriteAsFile)
            {
                mailMessage.BodyEncoding = Encoding.ASCII;
            }
            return SendEmail(mailMessage);
        }


        private string getWinnerTable(IEnumerable<Winner> winners)
        {
            var table = "<table>";
            table += "<tr><th>Name</th><th>Number of spots won</th></tr>";
            foreach (var winner in winners)
            {
                table += string.Format("<tr><td>{0}</td><td>{1}</td></tr>", winner.User.Name, winner.NumberOfSpotsWon);
            }
            table += "</table>";
            return table;
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

        private void SetUpSmtpClient()
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
