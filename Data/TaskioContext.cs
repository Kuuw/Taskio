using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Data;

public partial class TaskioContext : DbContext
{
    public TaskioContext()
    {
    }

    public TaskioContext(DbContextOptions<TaskioContext> options)
   : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectUser> ProjectUsers { get; set; }

    public virtual DbSet<Entities.Models.Task> Tasks { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
      => optionsBuilder
       .UseLazyLoadingProxies()
    .UseSqlServer("Server=localhost\\SQLEXPRESS;Database=Taskio;Trusted_Connection=True;integrated security=true;Encrypt=false");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__19093A0B35CB8F78");

            entity.ToTable("Category", t => t.UseSqlOutputClause(false));

          entity.HasIndex(e => e.ProjectId, "IX_Category_ProjectId");

        entity.Property(e => e.CategoryId).HasDefaultValueSql("(newid())");
    entity.Property(e => e.CategoryName).HasMaxLength(255);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysdatetime())");

    entity.HasOne(d => d.Project).WithMany(p => p.Categories)
         .HasForeignKey(d => d.ProjectId)
        .HasConstraintName("FK_Category_Project");
        });

    modelBuilder.Entity<Project>(entity =>
   {
          entity.HasKey(e => e.ProjectId).HasName("PK__Project__761ABEF0DB3AF8B5");

            entity.ToTable("Project", t => t.UseSqlOutputClause(false));

    entity.Property(e => e.ProjectId).HasDefaultValueSql("(newid())");
entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
       entity.Property(e => e.ProjectName).HasMaxLength(255);
 entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysdatetime())");
 });

        modelBuilder.Entity<ProjectUser>(entity =>
        {
   entity.HasKey(e => new { e.ProjectId, e.UserId }).HasName("PK__ProjectU__A76232348D4FEC20");

        entity.HasIndex(e => e.UserId, "IX_ProjectUsers_UserId");

      entity.HasOne(d => d.Project).WithMany(p => p.ProjectUsers)
          .HasForeignKey(d => d.ProjectId)
  .HasConstraintName("FK_ProjectUsers_Project");

       entity.HasOne(d => d.User).WithMany(p => p.ProjectUsers)
    .HasForeignKey(d => d.UserId)
        .HasConstraintName("FK_ProjectUsers_User");
    });

      modelBuilder.Entity<Entities.Models.Task>(entity =>
    {
        entity.HasKey(e => e.TaskId).HasName("PK__Task__7C6949B18AD5BC57");

            entity.ToTable("Task", t => t.UseSqlOutputClause(false));

          entity.HasIndex(e => e.CategoryId, "IX_Task_CategoryId");

            entity.HasIndex(e => e.ProjectId, "IX_Task_ProjectId");

            entity.Property(e => e.TaskId).HasDefaultValueSql("(newid())");
      entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
          entity.Property(e => e.TaskName).HasMaxLength(255);
     entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysdatetime())");

    entity.HasOne(d => d.Category).WithMany(p => p.Tasks)
         .HasForeignKey(d => d.CategoryId)
         .OnDelete(DeleteBehavior.NoAction)
         .HasConstraintName("FK_Task_Category");

    entity.HasOne(d => d.Project).WithMany(p => p.Tasks)
          .HasForeignKey(d => d.ProjectId)
          .HasConstraintName("FK_Task_Project");

        entity.HasMany(d => d.Users).WithMany(p => p.Tasks)
  .UsingEntity<Dictionary<string, object>>(
          "TaskUser",
       r => r.HasOne<User>().WithMany()
     .HasForeignKey("UserId")
.HasConstraintName("FK_TaskUser_User"),
        l => l.HasOne<Entities.Models.Task>().WithMany()
         .HasForeignKey("TaskId")
 .HasConstraintName("FK_TaskUser_Task"),
   j =>
      {
                  j.HasKey("TaskId", "UserId").HasName("PK__TaskUser__AD11C575C8257086");
              j.ToTable("TaskUser");
    j.HasIndex(new[] { "UserId" }, "IX_TaskUser_UserId");
        });
        });

        modelBuilder.Entity<User>(entity =>
        {
 entity.HasKey(e => e.UserId).HasName("PK__User__1788CC4C1C474319");

     entity.ToTable("User", t => t.UseSqlOutputClause(false));

            entity.HasIndex(e => e.Email, "UQ__User__A9D1053437BBD717").IsUnique();

    entity.Property(e => e.UserId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Email).HasMaxLength(255);
       entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
    entity.Property(e => e.Password).HasMaxLength(255);
     entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysdatetime())");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
