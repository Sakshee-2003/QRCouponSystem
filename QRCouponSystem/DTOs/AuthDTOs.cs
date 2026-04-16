namespace QRCouponSystem.DTOs
{
    public class AuthRequestDto
    {
        public string? UserName { get; set; }
        public string? UserPassword { get; set; }
    }

    public class AuthResponseDto
    {
        public string? AccessToken { get; set; }
    }
}
