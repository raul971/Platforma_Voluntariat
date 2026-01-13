using Microsoft.EntityFrameworkCore;
using VolunteerFlow.Api.Models;

namespace VolunteerFlow.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<TaskModel> Tasks { get; set; } = null!;
    public DbSet<TaskAssignment> TaskAssignments { get; set; } = null!;
    public DbSet<Event> Events { get; set; } = null!;
    public DbSet<EventParticipation> EventParticipations { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configurations
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(255);
        });

        // Task configurations
        modelBuilder.Entity<TaskModel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.EstimatedHours).HasColumnType("decimal(10,2)");

            entity.HasOne(e => e.CreatedByAdmin)
                .WithMany(u => u.CreatedTasks)
                .HasForeignKey(e => e.CreatedByAdminId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // TaskAssignment configurations
        modelBuilder.Entity<TaskAssignment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.WorkedHours).HasColumnType("decimal(10,2)");

            entity.HasOne(e => e.Task)
                .WithMany(t => t.Assignments)
                .HasForeignKey(e => e.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Volunteer)
                .WithMany(u => u.TaskAssignments)
                .HasForeignKey(e => e.VolunteerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Event configurations
        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Description).IsRequired();

            entity.HasOne(e => e.CreatedByAdmin)
                .WithMany(u => u.CreatedEvents)
                .HasForeignKey(e => e.CreatedByAdminId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // EventParticipation configurations
        modelBuilder.Entity<EventParticipation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Response).IsRequired().HasMaxLength(50);
            entity.Property(e => e.OccurrenceReport).IsRequired().HasMaxLength(50);

            entity.HasOne(e => e.Event)
                .WithMany(ev => ev.Participations)
                .HasForeignKey(e => e.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Volunteer)
                .WithMany(u => u.EventParticipations)
                .HasForeignKey(e => e.VolunteerId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
