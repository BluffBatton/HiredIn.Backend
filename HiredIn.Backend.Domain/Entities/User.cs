using HiredIn.Backend.Domain.Common;
using HiredIn.Backend.Domain.Enums;

namespace HiredIn.Backend.Domain.Entities
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string PasswordHash { get; set; } = null!;
        public string? AvatarUrl { get; set; }
        public UserRole Role { get; set; }
        public UserStatus Status { get; set; }

        public CandidateProfile? CandidateProfile { get; set; }
        public ICollection<CompanyMember> CompanyMembers { get; set; } = new List<CompanyMember>();
        public ICollection<Message> SentMessages { get; set; } = new List<Message>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public ICollection<FavouriteVacancy> FavouriteVacancies { get; set; } = new List<FavouriteVacancy>();
        public ICollection<ChatParticipant> ChatParticipants { get; set; } = new List<ChatParticipant>();
        public ICollection<CompanyRating> CompanyRatings { get; set; } = new List<CompanyRating>();

    }
}
