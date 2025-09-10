using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Reports.Worker.Projection
{
    [BsonIgnoreExtraElements]
    public class ContactInfoProjection
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [BsonElement(Order = 0)]
        public Guid Id { get; set; }

        [BsonRepresentation(BsonType.String)]
        [BsonElement(Order = 1)]
        public Guid ContactId { get; set; }

        [BsonElement(Order = 2)]
        public int InfoType { get; set; }

        [BsonElement(Order = 3)]
        public string Content { get; set; }
    }
}
