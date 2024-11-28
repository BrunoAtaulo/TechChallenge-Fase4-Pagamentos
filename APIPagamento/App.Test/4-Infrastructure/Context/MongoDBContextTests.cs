using Data.Context;
using Mongo2Go;
using MongoDB.Driver;

namespace App.Test._4_Infrastructure.Context
{
    public class MongoDBContextTests(IMongoDatabase database) : MongoDBContext(database)
    {
        private readonly MongoDbRunner _runner = MongoDbRunner.Start();

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public override void Dispose()
        {
            base.Dispose();
            _runner.Dispose();
        }
    }
}
