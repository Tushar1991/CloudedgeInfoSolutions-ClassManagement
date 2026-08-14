using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ClassManagement.Api.Models;

public class Mark
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

    [BsonElement("examName")]
    public string ExamName { get; set; } = string.Empty;

    [BsonElement("score")]
    public decimal Score { get; set; }

    [BsonElement("maxScore")]
    public decimal MaxScore { get; set; } = 100;

    [BsonElement("recordedBy")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string RecordedBy { get; set; } = string.Empty;

    [BsonElement("recordedAt")]
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("feedback")]
    public string? Feedback { get; set; }
}
