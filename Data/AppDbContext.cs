using Microsoft.EntityFrameworkCore;
using OnlineExamQuestionApi.Models;

namespace OnlineExamQuestionApi.Data;

public class AppDbContext : DbContext
{
    // 1. Parameterless constructor required by EF Core design-time tools
    public AppDbContext() { }

    // 2. Standard constructor for application Dependency Injection
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<Topic> Topics => Set<Topic>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<Option> Options => Set<Option>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Fallback connection string strictly for 'dotnet ef' tooling
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=OnlineExamQuestionDb;Trusted_Connection=True;TrustServerCertificate=True");
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var auditEntries = new List<AuditLog>();
        // Keep track of added entities so we can update their IDs after the first save
        var addedEntries = new List<(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry Entry, AuditLog Audit)>();

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is AuditLog || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                continue;

            var primaryKey = entry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey());

            var auditLog = new AuditLog
            {
                EntityName = entry.Entity.GetType().Name,
                Action = entry.State.ToString(),
                Timestamp = DateTime.UtcNow,
                // If it's a new item, leave the ID null for now
                PrimaryKey = entry.State == EntityState.Added ? null : primaryKey?.CurrentValue?.ToString()
            };

            if (entry.State == EntityState.Added)
            {
                addedEntries.Add((entry, auditLog));
            }

            auditEntries.Add(auditLog);
        }

        // STEP 1: Save the actual entities to the database (This generates the real IDs)
        var result = await base.SaveChangesAsync(cancellationToken);

        // STEP 2: Update the missing IDs for newly added items and save the Audit Logs
        if (auditEntries.Any())
        {
            foreach (var item in addedEntries)
            {
                var primaryKey = item.Entry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey());
                item.Audit.PrimaryKey = primaryKey?.CurrentValue?.ToString();
            }

            AuditLogs.AddRange(auditEntries);
            await base.SaveChangesAsync(cancellationToken);
        }

        return result;
    }

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