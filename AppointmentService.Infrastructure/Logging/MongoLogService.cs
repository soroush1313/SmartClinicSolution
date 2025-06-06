using AppointmentService.Application.Interfaces;
using MongoDB.Driver;

namespace AppointmentService.Infrastructure.Logging
{
    public class MongoLogService : ILogService
    {
        private readonly IMongoCollection<LogEntry> _logCollection;

        public MongoLogService(string connectionString, string databaseName = "SmartClinicLogs", string collectionName = "Logs")
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _logCollection = database.GetCollection<LogEntry>(collectionName);
        }

        public async Task LogAsync(LogEntry logEntry)
        {
            await _logCollection.InsertOneAsync(logEntry);
        }
    }
}
