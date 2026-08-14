using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ClassManagement.Api.Models;

public class Attendance
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

    [BsonElement("date")]
    public DateTime Date { get; set; }

    [BsonElement("isPresent")]
    public bool IsPresent { get; set; }

    [BsonElement("markedBy")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string MarkedBy { get; set; } = string.Empty;

    [BsonElement("notes")]
    public string? Notes { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
