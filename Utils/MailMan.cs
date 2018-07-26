using System.Net;
using System.Net.Mail;

namespace AparmentListingMonitor
{
    public class MailMan
    {
        private readonly string _username;
        private readonly string _password;

        public MailMan(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public void SendMail(string to, string msg, string subject)
        {
            var fromAddress = new MailAddress(_username, "Namu Naujienos i Namus");
            var toAddress = new MailAddress(to);

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, _password)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = msg
            })
            {
                smtp.Send(message);
            }
        }
    }
}