using OnlineExamQuestionApi.DTOs;
using OnlineExamQuestionApi.Models;
using OnlineExamQuestionApi.Services;

namespace OnlineExamQuestionApi.ConsoleUI;

public class AppMenu
{
    private readonly IQuestionService _service;

    public AppMenu(IQuestionService service)
    {
        _service = service;
    }

    public async Task RunAsync()
    {
        bool exit = false;
        while (!exit)
        {
            Console.Clear();
            Console.WriteLine("======================================");
            Console.WriteLine("      ONLINE EXAM QUESTION MANAGER    ");
            Console.WriteLine("======================================");
            Console.WriteLine("1. View all questions");
            Console.WriteLine("2. Add a new question");
            Console.WriteLine("3. Delete a question");
            Console.WriteLine("4. Exit");
            Console.WriteLine("======================================");
            Console.Write("Select an option (1-4): ");

            var choice = Console.ReadLine();

            Console.WriteLine();
            switch (choice)
            {
                case "1":
                    await ViewAllQuestionsAsync();
                    break;
                case "2":
                    await AddQuestionAsync();
                    break;
                case "3":
                    await DeleteQuestionAsync();
                    break;
                case "4":
                    exit = true;
                    Console.WriteLine("Goodbye!");
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    await PauseAsync();
                    break;
            }
        }
    }

    private async Task ViewAllQuestionsAsync()
    {
        Console.WriteLine("--- ALL QUESTIONS ---");
        var questions = await _service.GetAllAsync(null, null, null, 1, 50);

        if (!questions.Any())
        {
            Console.WriteLine("No questions found in the database.");
        }
        else
        {
            foreach (var q in questions)
            {
                Console.WriteLine($"\n[ID: {q.Id}] {q.Text} (Difficulty: {q.Difficulty})");
                foreach (var opt in q.Options)
                {
                    string mark = opt.IsCorrect ? "[x]" : "[ ]";
                    Console.WriteLine($"   {mark} {opt.Text}");
                }
            }
        }
        await PauseAsync();
    }

    private async Task AddQuestionAsync()
    {
        Console.WriteLine("--- ADD NEW QUESTION ---");

        Console.Write("Enter question text: ");
        string? text = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(text)) return;

        var dto = new QuestionCreateDto
        {
            Text = text,
            Explanation = "Added via Console App",
            Difficulty = DifficultyLevel.Medium,
            TopicId = 1 // Defaulting to C# Basics for the console demo
        };

        Console.WriteLine("\nYou must add exactly 4 options. (One must be correct)");
        for (int i = 1; i <= 4; i++)
        {
            Console.Write($"Option {i} text: ");
            string? optText = Console.ReadLine();

            Console.Write("Is this the correct answer? (y/n): ");
            bool isCorrect = Console.ReadLine()?.Trim().ToLower() == "y";

            dto.Options.Add(new OptionCreateDto { Text = optText ?? $"Option {i}", IsCorrect = isCorrect });
        }

        var (result, error) = await _service.CreateAsync(dto);

        if (error != null)
        {
            Console.WriteLine($"\nFailed to create question: {error}");
        }
        else
        {
            Console.WriteLine($"\nSuccess! Question created with ID: {result!.Id}");
        }

        await PauseAsync();
    }

    private async Task DeleteQuestionAsync()
    {
        Console.WriteLine("--- DELETE QUESTION ---");
        Console.Write("Enter the ID of the question to delete: ");

        if (int.TryParse(Console.ReadLine(), out int id))
        {
            bool success = await _service.DeleteAsync(id);
            if (success)
                Console.WriteLine($"Question {id} successfully deleted.");
            else
                Console.WriteLine($"Question {id} not found.");
        }
        else
        {
            Console.WriteLine("Invalid ID format.");
        }

        await PauseAsync();
    }

    private static async Task PauseAsync()
    {
        Console.WriteLine("\nPress any key to return to the menu...");
        Console.ReadKey();
    }
}