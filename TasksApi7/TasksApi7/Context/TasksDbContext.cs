using Microsoft.EntityFrameworkCore;
using TasksApi7.Entities;

namespace TasksApi7.Context
{
    public partial class TasksDbContext : DbContext
    {
        public TasksDbContext()
        {
        }
        public TasksDbContext(DbContextOptions<TasksDbContext> options)
            : base(options)
        {

        }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public virtual DbSet<Tasks> Taskss { get; set; }
        public virtual DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.Property(e => e.ExpiryDate).HasColumnType("smalldatetime");
                entity.Property(e => e.TokenHash)
                    .IsRequired()
                    .HasMaxLength(1000);
                entity.Property(e => e.TokenSalt)
                    .IsRequired()
                    .HasMaxLength(1000);
                entity.Property(e => e.Ts)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("TS");
                entity.HasOne(d => d.Users)


                    .WithMany(p => p.RefreshTokens)


                    .HasForeignKey(d => d.UserId)


                    .OnDelete(DeleteBehavior.ClientSetNull)


                    .HasConstraintName("FK_RefreshToken_User");


                entity.ToTable("RefreshToken");


            });


            modelBuilder.Entity<Tasks>(entity =>


            {




                entity.Property(e => e.Name)


                    .IsRequired()


                    .HasMaxLength(100);


                entity.Property(e => e.Ts)


                    .HasColumnType("smalldatetime")


                    .HasColumnName("TS");


                entity.HasOne(d => d.Users)


                    .WithMany(p => p.Taskss)


                    .HasForeignKey(d => d.UserId)


                    .OnDelete(DeleteBehavior.ClientSetNull)


                    .HasConstraintName("FK_Tasks_User");


                entity.ToTable("Tasks");


            });


            modelBuilder.Entity<User>(entity =>


            {


                entity.Property(e => e.Email)


                    .IsRequired()


                    .HasMaxLength(50);


                entity.Property(e => e.FirstName)


                    .IsRequired()


                    .HasMaxLength(255);


                entity.Property(e => e.LastName)


                    .IsRequired()


                    .HasMaxLength(255);


                entity.Property(e => e.Password)


                    .IsRequired()


                    .HasMaxLength(255);


                entity.Property(e => e.PasswordSalt)


                    .IsRequired()


                    .HasMaxLength(255);


                entity.Property(e => e.Ts)


                    .HasColumnType("smalldatetime")


                    .HasColumnName("TS");


                entity.ToTable("User");


            });


            OnModelCreatingPartial(modelBuilder);


        }


        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);


    }
}
