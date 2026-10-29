using OnlineExamQuestionApi.DTOs;
using OnlineExamQuestionApi.Models;

namespace OnlineExamQuestionApi.Services;

public interface IQuestionService
{
    Task<List<QuestionResponseDto>> GetAllAsync(int? topicId, DifficultyLevel? difficulty, string? search, int page, int pageSize);
    Task<QuestionResponseDto?> GetByIdAsync(int id);
    Task<List<QuestionResponseDto>> GetRandomAsync(int count, int? topicId);
    Task<(QuestionResponseDto? Result, string? Error)> CreateAsync(QuestionCreateDto dto);
    Task<(bool Success, string? Error)> UpdateAsync(int id, QuestionCreateDto dto);
    Task<bool> DeleteAsync(int id);
}