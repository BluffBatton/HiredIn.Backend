namespace HiredIn.Backend.Contracts.DTOs.AuthDTOs
{
    public class RegisterCandidateRequestDto
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }

        public string? City { get; set; }
        public string? About { get; set; }
    }
}
