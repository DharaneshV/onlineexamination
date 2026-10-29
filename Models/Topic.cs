namespace OnlineExamQuestionApi.Models;

public class Topic
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SubjectId { get; set; }
    public Subject? Subject { get; set; }
    public List<Question> Questions { get; set; } = new();
}