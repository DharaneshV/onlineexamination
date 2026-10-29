using OnlineExamQuestionApi.Models;

namespace OnlineExamQuestionApi.Repositories;

public interface IQuestionRepository
{
	Task<List<Question>> GetAllAsync(int? topicId, DifficultyLevel? difficulty, string? search, int page, int pageSize);
	Task<Question?> GetByIdAsync(int id);
	Task<List<Question>> GetRandomAsync(int count, int? topicId);
	Task<bool> TopicExistsAsync(int topicId);
	Task<bool> DuplicateExistsAsync(int topicId, string text);
	Task AddAsync(Question question);
	Task UpdateAsync(Question question);
}