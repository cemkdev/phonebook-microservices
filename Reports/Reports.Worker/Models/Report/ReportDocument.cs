using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Shared.Contracts.Enums;

namespace Reports.Worker.Models.Report
{
    [BsonIgnoreExtraElements] // sayesinde RequestedAt'e gerek yok.
    public class ReportDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

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
