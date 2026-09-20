namespace MinimalApiLivros.Infrastructure.Data.DataBaseConfigurationMongo
{
    public interface IDatabaseMongoConfig
    {
        string DatabaseName { get; set; }
        string ConnectionString { get; set; }
    }
}
