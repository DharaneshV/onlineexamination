using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineExamQuestionApi.ConsoleUI;
using OnlineExamQuestionApi.Data;
using OnlineExamQuestionApi.Repositories;
using OnlineExamQuestionApi.Services;

// 1. Build configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

// 2. Configure Dependency Injection
var services = new ServiceCollection();

services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

services.AddScoped<IQuestionRepository, QuestionRepository>();
services.AddScoped<IQuestionService, QuestionService>();

// Register the new Console UI
services.AddTransient<AppMenu>();

var serviceProvider = services.BuildServiceProvider();

// 3. Resolve the AppMenu and run the interactive loop
var appMenu = serviceProvider.GetRequiredService<AppMenu>();
await appMenu.RunAsync();