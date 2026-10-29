using Microsoft.AspNetCore.Mvc;
using OnlineExamQuestionApi.DTOs;
using OnlineExamQuestionApi.Models;
using OnlineExamQuestionApi.Services;

namespace OnlineExamQuestionApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuestionsController : ControllerBase
{
    private readonly IQuestionService _service;
    public QuestionsController(IQuestionService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<QuestionResponseDto>>> GetAll(
        int? topicId, DifficultyLevel? difficulty, string? search,
        int page = 1, int pageSize = 10)
        => Ok(await _service.GetAllAsync(topicId, difficulty, search, page, pageSize));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<QuestionResponseDto>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet("random")]
    public async Task<ActionResult<List<QuestionResponseDto>>> GetRandom(int count = 10, int? topicId = null)
        => Ok(await _service.GetRandomAsync(count, topicId));

    [HttpPost]
    public async Task<ActionResult<QuestionResponseDto>> Create(QuestionCreateDto dto)
    {
        var (result, error) = await _service.CreateAsync(dto);
        if (error == "DUPLICATE") return Conflict("This question already exists in the topic.");
        if (error != null) return BadRequest(error);
        return CreatedAtAction(nameof(GetById), new { id = result!.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, QuestionCreateDto dto)
    {
        var (success, error) = await _service.UpdateAsync(id, dto);
        if (error == "NOTFOUND") return NotFound();
        if (!success) return BadRequest(error);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) =>
        await _service.DeleteAsync(id) ? NoContent() : NotFound();
}