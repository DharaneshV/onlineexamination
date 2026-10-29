using Microsoft.EntityFrameworkCore;
using OnlineExamQuestionApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// ADD THESE LINES RIGHT HERE:
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// ... existing code ...
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();