namespace MinimalApiLivros.Infrastructure.Data.DataBaseConfigurationMongo
{
    public class DatabaseMongoConfig : IDatabaseMongoConfig
    {
        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
    }
}
