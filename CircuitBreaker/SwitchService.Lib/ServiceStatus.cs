namespace SwitchService.Lib
{
    public sealed class ServiceStatus
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public static ServiceStatus Default
        {
            get
            {
                return new ServiceStatus
                {
                    Id = 0,
                    Name = "Working"
                };
            }
        }
    }
}
