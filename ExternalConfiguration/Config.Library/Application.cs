namespace Config.Library
{
    public class Application
    {
        public int Id { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is Application other)
            {
                return Id == other.Id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
