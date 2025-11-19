using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RealEstateAgency.Core.Models;
using RealEstateAgency.Infrastructure.Configurations;

namespace RealEstateAgency.Infrastructure.Context
{
    public class RealEstateContext(DbContextOptions<RealEstateContext> options) : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
    {
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Statement> Statements { get; set; }
        public DbSet<StatementType> StatementTypes { get; set; }
        public DbSet<PropertyType> PropertyTypes { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<AuAction> AuActions { get; set; }
        public DbSet<AuHistory> AuHistories { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Verification> Verifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new AnnouncementConfiguration());
            modelBuilder.ApplyConfiguration(new StatementConfiguration());
            modelBuilder.ApplyConfiguration(new StatementTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PropertyConfiguration());
            modelBuilder.ApplyConfiguration(new PropertyTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CommentConfiguration());
            modelBuilder.ApplyConfiguration(new AnswerConfiguration());
            modelBuilder.ApplyConfiguration(new QuestionConfiguration());
            modelBuilder.ApplyConfiguration(new AuActionConfiguration());
            modelBuilder.ApplyConfiguration(new AuHistoryConfiguration());
            modelBuilder.ApplyConfiguration(new ImageConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentConfiguration());
            modelBuilder.ApplyConfiguration(new VerificationConfiguration());

            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new RoleClaimConfiguration());
            modelBuilder.ApplyConfiguration(new UserClaimConfiguration());
            modelBuilder.ApplyConfiguration(new UserLoginConfiguration());
            modelBuilder.ApplyConfiguration(new UserTokenConfiguration());

            modelBuilder.Entity<User>().ToTable("t_user");
            modelBuilder.Entity<IdentityRole<Guid>>().ToTable("t_role");
            modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("t_user_role");
            modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("t_user_claim");
            modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("t_role_claim");
            modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("t_user_login");
            modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("t_user_token");
        }
    }
}
