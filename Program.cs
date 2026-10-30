using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineExamQuestionApi.ConsoleUI;
using OnlineExamQuestionApi.Data;
using OnlineExamQuestionApi.Repositories;
using OnlineExamQuestionApi.Services;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var services = new ServiceCollection();

services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

services.AddScoped<IQuestionRepository, QuestionRepository>();
services.AddScoped<IQuestionService, QuestionService>();
services.AddTransient<AppMenu>();

var serviceProvider = services.BuildServiceProvider();

var appMenu = serviceProvider.GetRequiredService<AppMenu>();
await appMenu.RunAsync();