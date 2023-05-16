namespace SmartEnergy.Interfaces
{
    public interface IMessageReceiver
    {
        void OnMessage(string message);
        Task ResubscribeAsync();
    }
}
