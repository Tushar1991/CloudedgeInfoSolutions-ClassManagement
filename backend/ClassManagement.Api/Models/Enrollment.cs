using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ClassManagement.Api.Models;

public class Enrollment
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("classId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string ClassId { get; set; } = string.Empty;

    [BsonElement("studentId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string StudentId { get; set; } = string.Empty;

    [BsonElement("enrolledAt")]
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
}
