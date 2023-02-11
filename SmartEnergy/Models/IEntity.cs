namespace SmartEnergy.Database.Models
{
    public interface IEntity<TKey>
    {
        public TKey Id { get; set; }
    }
}
