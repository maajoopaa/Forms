using Forms.DataAccess.Configurations;
using Forms.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Forms.DataAccess
{
    public class FormsDbContext : DbContext
    {
        public DbSet<UserEntity> Users { get; set; }

        public DbSet<UserSettingEntity> UserSettings { get; set; }

        public DbSet<ThemeEntity> Themes { get; set; }

        public DbSet<TemplateEntity> Templates { get; set; }

        public DbSet<QuestionEntity> Questions { get; set; }

        public DbSet<LikeEntity> Likes { get; set; }

        public DbSet<FormEntity> Forms { get; set; }

        public DbSet<CommentEntity> Comments { get; set; }

        public DbSet<AnswerEntity> Answers { get; set; }

        public DbSet<OptionEntity> Options { get; set; }

        public FormsDbContext(DbContextOptions<FormsDbContext> options)
            : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AnswerConfiguration());
            modelBuilder.ApplyConfiguration(new CommentConfiguration());
            modelBuilder.ApplyConfiguration(new FormConfiguration());
            modelBuilder.ApplyConfiguration(new LikeConfiguration());
            modelBuilder.ApplyConfiguration(new QuestionConfiguration());
            modelBuilder.ApplyConfiguration(new TemplateConfiguration());
            modelBuilder.ApplyConfiguration(new ThemeConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new UserSettingConfiguration());
            modelBuilder.ApplyConfiguration(new OptionConfiguration());

            base.OnModelCreating(modelBuilder);
        }

    }
}
