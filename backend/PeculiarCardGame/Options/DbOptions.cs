namespace PeculiarCardGame.Options
{
    public class DbOptions
    {
        public const string ConfigurationKey = "SqlServer";

        public string ConnectionString { get; set; } = string.Empty;
    }
}
