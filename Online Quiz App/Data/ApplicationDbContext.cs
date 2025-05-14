using Microsoft.EntityFrameworkCore;
using Online_Quiz_App.Models;

namespace Online_Quiz_App.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Add your DbSets (tables)
        public DbSet<User> Users { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }

        public DbSet<Option> Options { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Quiz>()
                .HasMany(q => q.Questions)
                .WithOne(q => q.Quiz)
                .HasForeignKey(q => q.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Question>()
                .HasMany(q => q.Options)
                .WithOne(o => o.Question)
                .HasForeignKey(o => o.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);


            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Quiz>().HasData(
                new Quiz { QuizId = 1, Title = "General Knowledge Quiz" }
            );

            modelBuilder.Entity<Question>().HasData(
                new Question { QuestionId = 1, Text = "What is the capital of France?", CorrectAnswer = "Paris", QuizId = 1 },
                new Question { QuestionId = 2, Text = "What is 2 + 2?", CorrectAnswer = "4", QuizId = 1 }
            );

            modelBuilder.Entity<Option>().HasData(
                new Option { OptionId = 1, Text = "Paris", QuestionId = 1 },
                new Option { OptionId = 2, Text = "Madrid", QuestionId = 1 },
                new Option { OptionId = 3, Text = "Berlin", QuestionId = 1 },
                new Option { OptionId = 4, Text = "Rome", QuestionId = 1 },
                new Option { OptionId = 5, Text = "4", QuestionId = 2 },
                new Option { OptionId = 6, Text = "3", QuestionId = 2 },
                new Option { OptionId = 7, Text = "5", QuestionId = 2 },
                new Option { OptionId = 8, Text = "6", QuestionId = 2 }
            );
        }




    }
}
