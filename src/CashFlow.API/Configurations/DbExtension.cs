using System.Diagnostics.CodeAnalysis;
using MongoDB.Driver;

namespace CashFlow.API.Configurations;

[ExcludeFromCodeCoverage]
public static class DbExtension
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("MongoDB");
        string databaseName = configuration.GetValue<string>("MongoDB:DatabaseName");

        // Criando a conexão com o MongoDB
        MongoClient mongoClient = new MongoClient(connectionString);
        services.AddSingleton<IMongoDatabase>(mongoClient.GetDatabase(databaseName));
        
        return services;
    }
}