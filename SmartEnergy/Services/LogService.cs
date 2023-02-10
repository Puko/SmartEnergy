using Serilog;
using ILogService = SmartEnergy.Interfaces.ILogService;

namespace SmartEnergy.Services
{
    public class LogService : ILogService
    {
        public LogService()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(Path.Combine(FileSystem.Current.AppDataDirectory, "log.txt"))
                .CreateLogger();
        }

        public void Info(string message) 
        {
            Log.Information(message);
        }
        
        public void Warning(string message) 
        {
            Log.Information(message);
        }

        public void Exception(Exception e, string message)
        {
            Log.Error(e, message);
        }

        public IEnumerable<string> GetLogs()
        {
            var path = Path.Combine(FileSystem.Current.AppDataDirectory, "log.txt");
            if (!File.Exists(path))
                return new List<string>();  

            return File.ReadAllLines(path);
        }
    }
}
