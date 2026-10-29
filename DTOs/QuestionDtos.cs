using System.ComponentModel.DataAnnotations;
using OnlineExamQuestionApi.Models;

namespace OnlineExamQuestionApi.DTOs;

public class OptionCreateDto
{
    [Required, MaxLength(500)]
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}

public class QuestionCreateDto
{
    [Required, MaxLength(1000)]
    public string Text { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Explanation { get; set; }

    public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Medium;

    [Required]
    public int TopicId { get; set; }

    [Required, MinLength(2), MaxLength(6)]
    public List<OptionCreateDto> Options { get; set; } = new();
}

public class OptionResponseDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}

public class QuestionResponseDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? Explanation { get; set; }
    public string Difficulty { get; set; } = string.Empty;
    public int TopicId { get; set; }
    public string? TopicName { get; set; }
    public List<OptionResponseDto> Options { get; set; } = new();
}