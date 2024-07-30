using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Entities;

namespace TaskManagementAPI.Context
{
    public partial class TasksDbContext : DbContext
    {
        public TasksDbContext(DbContextOptions<TasksDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Tasks> Tasks { get; set; }
        public DbSet<TaskNote> TaskNotes { get; set; }
        public DbSet<TaskDocument> TaskDocuments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
            .HasMany(e => e.Tasks)
            .WithOne(t => t.Employee)
            .HasForeignKey(t => t.EmployeeId);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Manager)
                .WithMany()
                .HasForeignKey(e => e.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);   

            modelBuilder.Entity<Tasks>()
                .HasMany(t => t.Notes)
                .WithOne(n => n.Tasks)
                .HasForeignKey(n => n.TaskId);

            modelBuilder.Entity<Tasks>()
                .HasMany(t => t.Documents)
                .WithOne(d => d.Tasks)
                .HasForeignKey(d => d.TaskId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
