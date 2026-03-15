using HiredIn.Backend.Domain.Common;

namespace HiredIn.Backend.Domain.Entities
{
    public class ResumeFile : BaseEntity
    {
        public Guid ResumeId { get; set; }
        public string FileName { get; set; } = null!;
        public string FileUrl { get; set; } = null!;

        public Resume Resume { get; set; } = null!;
    }
}
