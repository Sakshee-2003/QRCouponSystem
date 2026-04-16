using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCouponSystem.DbModels;
using QRCouponSystem.DTOs;
using QRCouponSystem.Services.Interfaces;

namespace QRCouponSystem.Controllers
{
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpPost]
        [Route("campaigns")]
        public async Task<IActionResult> CreateCampaign([FromBody] CampaignDto dto)
        {
            var result = await _adminService.CreateCampaignAsync(dto);
            return Ok(result);
        }

        [HttpPost]
        [Route("campaigns/{campaignId}/generate-coupons")]
        public async Task<IActionResult> GenerateCoupons(int campaignId, [FromBody] GenerateCouponDto dto)
        {
            var coupons = await _adminService.GenerateCoupons(campaignId, dto);

            return Ok(new
            {
                Message = "Coupons generated successfully",
                CampaignId = campaignId,
                Coupons = coupons.Select(c => new
                {
                    c.Code,
                    c.Value,
                    c.ExpiryDate
                })
            });
        }

        [HttpPost]
        [Route("reconcile")]
        public async Task<IActionResult> Reconcile()
        {
            var result = await _adminService.ReconcileAsync();
            return Ok(result);
        }
    }
}
