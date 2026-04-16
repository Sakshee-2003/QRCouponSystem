using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRCouponSystem.DTOs;
using QRCouponSystem.Services.Interfaces;
using System.Security.Claims;

namespace QRCouponSystem.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/wallet")]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _service;

        public WalletController(IWalletService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("balance")]
        public async Task<IActionResult> GetBalance()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _service.GetBalanceAsync(userId);

            return Ok(result);
        }
    }
}
