namespace DomainEvents.Lib.BusinessObjects
{
    public abstract class BusinessObject
    {
        public int Id { get; set; }

        public string? Status { get; set; }
    }
}
