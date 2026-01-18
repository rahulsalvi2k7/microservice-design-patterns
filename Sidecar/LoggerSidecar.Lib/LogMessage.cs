namespace LoggerSidecar.Lib
{
    public sealed record LogMessage 
    {
        public DateTime DateTime { get; set; }

        public LogLevel LogLevel { get; set; }

        public string? ServiceName { get; set; }

        public string? Message { get; set; }
    }
}
