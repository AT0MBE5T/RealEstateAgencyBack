using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace RealEstateAgency.Core.Models
{
    public class User : IdentityUser<Guid>
    {
        [Required]
        [Column("name")]
        [StringLength(maximumLength: 50, MinimumLength = 2)]
        public required string Name { get; set; }

        [Required]
        [Column("surname")]
        [StringLength(maximumLength: 50, MinimumLength = 1)]
        public required string Surname { get; set; }

        [Required]
        [Column("age")]
        [Range(1, 130)]
        public required int Age { get; set; }

        [Required]
        [Column("created_at")]
        public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Statement> StatementsNavigation { get; set; } = [];
        public ICollection<Comment> CommentsNavigation { get; set; } = [];
        public ICollection<Question> QuestionsNavigation { get; set; } = [];
        public ICollection<Answer> AnswersNavigation { get; set; } = [];
        public ICollection<AuHistory> AuHistoriesNavigation { get; set; } = [];
        public ICollection<Payment> PaymentsNavigation { get; set; } = [];
        public ICollection<Verification> VerificationsNavigation { get; set; } = [];
        public ICollection<Announcement> AnnouncementsNavigation { get; set; } = [];
    }
}
