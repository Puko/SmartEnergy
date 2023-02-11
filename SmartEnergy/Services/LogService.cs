using LiteDB;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using Serilog.Parsing;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ILogService = SmartEnergy.Interfaces.ILogService;

namespace SmartEnergy.Services
{
    public class LogService : ILogService
    {
        private string _logDatabasePath => Path.Combine(FileSystem.Current.AppDataDirectory, "logs.db");

        public LogService()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.LiteDB(_logDatabasePath)
                .CreateLogger();
        }

        public void Info(string message)
        {
            Log.Information("{Timestamp} {Level} {Message}", DateTimeOffset.Now.ToString("dd-MM-yyyy HH:mm:ss"), LogEventLevel.Information, message);
        }

        public void Warning(string message)
        {
            Log.Warning("{Timestamp} {Level} {Message}", DateTimeOffset.Now.ToString("dd-MM-yyyy HH:mm:ss"), LogEventLevel.Information, message);
        }

        public void Exception(Exception e, string message)
        {
            Log.Error("{Timestamp} {Level} {Message}\n{Exception}", DateTimeOffset.Now.ToString("dd-MM-yyyy HH:mm:ss"), 
                LogEventLevel.Information, message, 
                $"Exception: {e.Message}\n StackTrace: {e.StackTrace}");
        }

        public IEnumerable<string> GetLogs()
        {
            using LiteDatabase lite = new LiteDatabase(_logDatabasePath);
            var logCollection = lite.GetCollection("log");
            var col = logCollection.FindAll().ToList();
            List<string> logs = new List<string>();

            foreach (var document in col)
            {
                logs.Add($"[{document["Timestamp"]}] [{document["Level"]}]\n{document["Message"]}{(!string.IsNullOrEmpty(document["Exception"]) ? $"\n{document["Exception"]}" : string.Empty)}");
            }

            logs.Reverse();
            return logs;
        }
    }
}