using MailKit.Net.Smtp;
using MimeKit;

public class EmailSender
{
    private readonly string _smtpServer = "smtp.gmail.com";
    private readonly int _smtpPort = 587;
    private readonly string _smtpUser = "soroush.jalali13@gmail.com";
    private readonly string _smtpPass = "vmhtxcyrcpjwewrn"; // از App Password گوگل استفاده کن

    public void Send(string to, string subject, string body)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("SmartClinic", _smtpUser));
        message.To.Add(new MailboxAddress("", to));
        message.Subject = subject;

        message.Body = new TextPart("plain") { Text = body };

        using var client = new SmtpClient();
        client.Connect(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
        client.Authenticate(_smtpUser, _smtpPass);
        client.Send(message);
        client.Disconnect(true);
    }
}
