namespace HiredIn.Backend.Contracts.DTOs.CandidateProfileDTOs
{
    public class CandidateProfileReadDTO
    {
        public Guid Id { get; set; }
        public string ?City { get; set; }
        public string ?About { get; set; }
        public bool OpenToWork { get; set; }
    }
}
