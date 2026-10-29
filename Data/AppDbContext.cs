using Microsoft.EntityFrameworkCore;
using OnlineExamQuestionApi.Models;

namespace OnlineExamQuestionApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<Topic> Topics => Set<Topic>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<Option> Options => Set<Option>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Subject>(e =>
        {
            e.Property(x => x.Name).IsRequired().HasMaxLength(100);
            e.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<Topic>(e =>
        {
            e.Property(x => x.Name).IsRequired().HasMaxLength(100);
            e.HasOne(x => x.Subject)
             .WithMany(s => s.Topics)
             .HasForeignKey(x => x.SubjectId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Question>(e =>
        {
            e.Property(x => x.Text).IsRequired().HasMaxLength(1000);
            e.Property(x => x.Explanation).HasMaxLength(2000);
            e.HasIndex(x => x.TopicId);
            e.HasOne(x => x.Topic)
             .WithMany(t => t.Questions)
             .HasForeignKey(x => x.TopicId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Option>(e =>
        {
            e.Property(x => x.Text).IsRequired().HasMaxLength(500);
            e.HasOne(x => x.Question)
             .WithMany(q => q.Options)
             .HasForeignKey(x => x.QuestionId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed data
        modelBuilder.Entity<Subject>().HasData(
            new Subject { Id = 1, Name = "Programming", Description = "Programming fundamentals" });

        modelBuilder.Entity<Topic>().HasData(
            new Topic { Id = 1, Name = "C# Basics", SubjectId = 1 },
            new Topic { Id = 2, Name = "Entity Framework Core", SubjectId = 1 });
    }
}