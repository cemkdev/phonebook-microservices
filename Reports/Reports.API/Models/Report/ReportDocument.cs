using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Shared.Contracts.Enums;

namespace Reports.API.Models.Report
{
    [BsonIgnoreExtraElements]
    public class ReportDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        public DateTime RequestedAt { get; set; }
        public ReportStatus Status { get; set; }
        public List<ReportItem> Items { get; set; }
        public string? Error { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class ReportItem
    {
        public string Location { get; set; }
        public int ContactCount { get; set; }
        public int PhoneCount { get; set; }
    }
}
