using MongoDB.Driver;
using Reports.API.Models.Report;

namespace Reports.API.Infrastructure
{
    public class MongoContext
    {
        private readonly IMongoDatabase mongoDatabase;

        public MongoContext(IConfiguration configuration)
        {
            MongoClient client = new(configuration["MongoDB:ConnectionString"] ?? throw new InvalidOperationException("MongoDB:ConnectionString not set."));
            var databaseName = configuration["MongoDB:Database"] ?? throw new InvalidOperationException("MongoDB:Database not set.");

            mongoDatabase = client.GetDatabase(databaseName);
        }

        public IMongoCollection<ReportDocument> Reports
            => mongoDatabase.GetCollection<ReportDocument>("reports");
    }
}
