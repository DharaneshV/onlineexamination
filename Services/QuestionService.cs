using OnlineExamQuestionApi.DTOs;
using OnlineExamQuestionApi.Helper;
using OnlineExamQuestionApi.Models;
using OnlineExamQuestionApi.Repositories;

namespace OnlineExamQuestionApi.Services;

public class QuestionService : IQuestionService
{
    private readonly IQuestionRepository _repo;
    public QuestionService(IQuestionRepository repo) => _repo = repo;

    public async Task<List<QuestionResponseDto>> GetAllAsync(
        int? topicId, DifficultyLevel? difficulty, string? search, int page, int pageSize)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);
        var items = await _repo.GetAllAsync(topicId, difficulty, search, page, pageSize);
        return items.Select(q => q.ToDto()).ToList();
    }

    public async Task<QuestionResponseDto?> GetByIdAsync(int id) =>
        (await _repo.GetByIdAsync(id))?.ToDto();

    public async Task<List<QuestionResponseDto>> GetRandomAsync(int count, int? topicId)
    {
        count = Math.Clamp(count, 1, 50);
        var items = await _repo.GetRandomAsync(count, topicId);
        return items.Select(q => q.ToDto()).ToList();
    }

    public async Task<(QuestionResponseDto? Result, string? Error)> CreateAsync(QuestionCreateDto dto)
    {
        var error = Validate(dto);
        if (error != null) return (null, error);

        if (!await _repo.TopicExistsAsync(dto.TopicId))
            return (null, "Topic not found.");

        if (await _repo.DuplicateExistsAsync(dto.TopicId, dto.Text.Trim()))
            return (null, "DUPLICATE");

        var entity = dto.ToEntity();
        await _repo.AddAsync(entity);
        return (entity.ToDto(), null);
    }

    public async Task<(bool Success, string? Error)> UpdateAsync(int id, QuestionCreateDto dto)
    {
        var error = Validate(dto);
        if (error != null) return (false, error);

        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return (false, "NOTFOUND");

        existing.Text = dto.Text.Trim();
        existing.Explanation = dto.Explanation;
        existing.Difficulty = dto.Difficulty;
        existing.TopicId = dto.TopicId;

        existing.Options.Clear();
        foreach (var o in dto.Options)
            existing.Options.Add(new Option { Text = o.Text.Trim(), IsCorrect = o.IsCorrect });

        await _repo.UpdateAsync(existing);
        return (true, null);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return false;
        existing.IsActive = false; // soft delete
        await _repo.UpdateAsync(existing);
        return true;
    }

    private static string? Validate(QuestionCreateDto dto)
    {
        if (dto.Options.Count < 2) return "A question needs at least 2 options.";
        if (dto.Options.Count(o => o.IsCorrect) != 1) return "Exactly one option must be correct.";
        return null;
    }
}