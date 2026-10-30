namespace OnlineExamQuestionApi.Models;

public class AuditLog
{
    public int Id { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // Added, Modified, Deleted
    public string? PrimaryKey { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}