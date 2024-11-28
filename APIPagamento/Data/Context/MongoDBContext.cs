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
            var connectionString = _configuration.GetConnectionString("ConnectionMongoDB");
            var databaseName = _configuration.GetConnectionString("databaseName");
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

        public IMongoCollection<Pagamento> Pagamento => _database.GetCollection<Pagamento>("pagamento");
        public IMongoCollection<PagamentoInput> PagamentoInput => _database.GetCollection<PagamentoInput>("pagamento");
    }
}
