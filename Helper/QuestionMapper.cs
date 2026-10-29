using OnlineExamQuestionApi.DTOs;
using OnlineExamQuestionApi.Models;

namespace OnlineExamQuestionApi.Helper;

public static class QuestionMapper
{
    public static QuestionResponseDto ToDto(this Question q) => new()
    {
        Id = q.Id,
        Text = q.Text,
        Explanation = q.Explanation,
        Difficulty = q.Difficulty.ToString(),
        TopicId = q.TopicId,
        TopicName = q.Topic?.Name,
        Options = q.Options.Select(o => new OptionResponseDto
        {
            Id = o.Id,
            Text = o.Text,
            IsCorrect = o.IsCorrect
        }).ToList()
    };

    public static Question ToEntity(this QuestionCreateDto dto) => new()
    {
        Text = dto.Text.Trim(),
        Explanation = dto.Explanation,
        Difficulty = dto.Difficulty,
        TopicId = dto.TopicId,
        Options = dto.Options.Select(o => new Option
        {
            Text = o.Text.Trim(),
            IsCorrect = o.IsCorrect
        }).ToList()
    };
}