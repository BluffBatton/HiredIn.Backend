namespace HiredIn.Backend.Contracts.DTOs.AuthDTOs
{
    public class AuthResponseDto
    {
        public required string AccessToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public required UserInfoDto User { get; set; }
    }
}
