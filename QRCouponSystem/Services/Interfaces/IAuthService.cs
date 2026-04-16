using QRCouponSystem.DTOs;

namespace QRCouponSystem.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(AuthRequestDto dto);
        Task<AuthResponseDto> LoginAsync(AuthRequestDto dto);
    }
}
