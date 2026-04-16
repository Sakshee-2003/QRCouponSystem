using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCouponSystem.DbModels;
using QRCouponSystem.DTOs;
using QRCouponSystem.Enums;
using QRCouponSystem.Services.Implementations;
using QRCouponSystem.Services.Interfaces;
using System;
using System.Security.Claims;

namespace QRCouponSystem.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/coupon")]
    public class CouponController : ControllerBase
    {
        private readonly ICouponService _service;

        public CouponController(ICouponService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("available")]
        public async Task<IActionResult> GetAvailableCoupons()
        {
            var result = await _service.GetAvailableCouponsAsync();
            return Ok(result);
        }

        [HttpPost]
        [Route("redeem")]
        public async Task<IActionResult> Redeem([FromBody] CouponRedeemRequestDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var result = await _service.RedeemCouponAsync(userId, dto);

            return result.RedemptionStatus switch
            {
                RedemptionStatus.Success => Ok(result),

                RedemptionStatus.InvalidCoupon => BadRequest(result),
                RedemptionStatus.Expired => BadRequest(result),

                RedemptionStatus.AlreadyRedeemed => Conflict(result),
                RedemptionStatus.ConcurrencyConflict => Conflict(result),
                RedemptionStatus.AlreadyProcessed => Ok(result),

                _ => StatusCode(500, result)
            };
        }
    }
}
