namespace AppointmentService.Application.Interfaces
{
    public interface ILogService
    {
        Task LogAsync(LogEntry entry);
    }

    public class LogEntry
    {
        public Guid Id { get; set; }
        public string Source { get; set; } = null!;
        public string Message { get; set; } = null!;
        public DateTime Timestamp { get; set; }
    }
}
