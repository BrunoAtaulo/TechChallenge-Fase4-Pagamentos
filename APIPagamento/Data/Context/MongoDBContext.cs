using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Data.Context
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class MongoDBContext : DbContext
    {
        private readonly IMongoDatabase _database;
        private readonly IConfiguration _configuration;

        public MongoDBContext(IConfiguration configuration)
        {
            _configuration = configuration;

            // Obtém os valores da seção "ConnectionMongoDB"
            var mongoConfig = _configuration.GetSection("ConnectionMongoDB");
            var server = mongoConfig["Server"];
            var databaseName = mongoConfig["databaseName"];
            var user = mongoConfig["User"];
            var password = mongoConfig["Password"];

            // Monta a ConnectionString dinamicamente
            var connectionString = $"mongodb+srv://{user}:{password}@{server}/?retryWrites=true&w=majority";

            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public MongoDBContext(IMongoDatabase database)
        {
            _database = database;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        // Expondo as coleções do MongoDB
        public IMongoCollection<Pagamento> Pagamento => _database.GetCollection<Pagamento>("pagamento");
        public IMongoCollection<PagamentoInput> PagamentoInput => _database.GetCollection<PagamentoInput>("pagamento");
    }
}
