using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Json;

Console.WriteLine("📬 NotificationService is listening...");

var factory = new ConnectionFactory() { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "appointments",
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);

    var appointment = JsonSerializer.Deserialize<AppointmentNotification>(message);

    // ارسال ایمیل
    SendEmail(appointment.PatientEmail, "Appointment Confirmed", $"Your appointment with Dr. {appointment.DoctorName} has been scheduled for {appointment.StartTime}.");
    SendEmail(appointment.DoctorEmail, "New Appointment Scheduled", $"Patient {appointment.PatientName} has booked an appointment for {appointment.StartTime}.");
};

channel.BasicConsume(queue: "appointments", autoAck: true, consumer: consumer);
Console.ReadLine();

static void SendEmail(string to, string subject, string body)
{
    var smtpClient = new SmtpClient("smtp.gmail.com")
    {
        Port = 587,
        Credentials = new NetworkCredential("soroush.jalali13@gmail.com", "vmhtxcyrcpjwewrn"),
        EnableSsl = true,
    };

    var mailMessage = new MailMessage
    {
        From = new MailAddress("soroush.jalali13@gmail.com"),
        Subject = subject,
        Body = body,
        IsBodyHtml = false,
    };

    mailMessage.To.Add(to);

    try
    {
        smtpClient.Send(mailMessage);
        Console.WriteLine($"✅ Email sent to: {to}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error sending email to {to}: {ex.Message}");
    }
}

// ساختار مدل دریافتی
public class AppointmentNotification
{
    public string PatientEmail { get; set; } = null!;
    public string DoctorEmail { get; set; } = null!;
    public string PatientName { get; set; } = null!;
    public string DoctorName { get; set; } = null!;
    public DateTime StartTime { get; set; }
}
