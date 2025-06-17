using System.Net;
using System.Net.Mail;

namespace JobApplicationPortal.Service.Helper;

public class EmailHelper
{
     public static async Task SendEmailAsync(
        string receiverEmailAddress,
        string receiverDisplayName,
        string subject,
        string body)
    {
        var senderEmail = new MailAddress("test.dotnet@etatvasoft.com", "test.dotnet@etatvasoft.com");
        var receiverEmail = new MailAddress(receiverEmailAddress, receiverDisplayName);
        var password = "P}N^{z-]7Ilp";
        
        var smtp = new SmtpClient
        {
            Host = "mail.etatvasoft.com",
            Port = 587,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(senderEmail.Address, password)
        };

        using (var mess = new MailMessage(senderEmail, receiverEmail))
        {
            mess.Subject = subject;
            mess.Body = body;
            mess.IsBodyHtml = true;
            await smtp.SendMailAsync(mess);
        }
    }
}
