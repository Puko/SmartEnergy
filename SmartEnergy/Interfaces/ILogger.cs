namespace SmartEnergy.Interfaces
{
    public interface ILogService 
    {
        void Info(string message);
        void Warning(string message);
        void Exception(Exception e, string message);
        IEnumerable<string> GetLogs();
    }
}
