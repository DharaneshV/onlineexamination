using Microsoft.EntityFrameworkCore;
using OnlineExamQuestionApi.Data;
using OnlineExamQuestionApi.Models;

namespace OnlineExamQuestionApi.Repositories;

public class QuestionRepository : IQuestionRepository
{
    private readonly AppDbContext _db;
    public QuestionRepository(AppDbContext db) => _db = db;

    public async Task<List<Question>> GetAllAsync(
        int? topicId, DifficultyLevel? difficulty, string? search, int page, int pageSize)
    {
        var query = _db.Questions
            .AsNoTracking()
            .Include(q => q.Topic)
            .Include(q => q.Options)
            .Where(q => q.IsActive);

        if (topicId.HasValue) query = query.Where(q => q.TopicId == topicId);
        if (difficulty.HasValue) query = query.Where(q => q.Difficulty == difficulty);
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(q => q.Text.Contains(search));

        return await query
            .OrderBy(q => q.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public Task<Question?> GetByIdAsync(int id) =>
        _db.Questions
           .Include(q => q.Topic)
           .Include(q => q.Options)
           .FirstOrDefaultAsync(q => q.Id == id && q.IsActive);

    public async Task<List<Question>> GetRandomAsync(int count, int? topicId)
    {
        var query = _db.Questions
            .AsNoTracking()
            .Include(q => q.Options)
            .Where(q => q.IsActive);

        if (topicId.HasValue) query = query.Where(q => q.TopicId == topicId);

        return await query.OrderBy(q => Guid.NewGuid()).Take(count).ToListAsync();
    }

    public Task<bool> TopicExistsAsync(int topicId) =>
        _db.Topics.AnyAsync(t => t.Id == topicId);

    public Task<bool> DuplicateExistsAsync(int topicId, string text) =>
        _db.Questions.AnyAsync(q => q.TopicId == topicId && q.Text == text);

    public async Task AddAsync(Question question)
    {
        _db.Questions.Add(question);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Question question)
    {
        question.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }
}