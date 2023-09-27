namespace PeculiarCardGame.Options
{
    public class SqlServerOptions
    {
        public const string ConfigurationKey = "SqlServer";

        public string ConnectionString { get; set; } = string.Empty;
    }
}
