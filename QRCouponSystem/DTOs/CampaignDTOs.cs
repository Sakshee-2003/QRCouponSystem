namespace QRCouponSystem.DTOs
{
    public class CampaignDto
    {
        public string? Name { get; set; }
        public DateTime ExpiryDate { get; set; }
    }

    public class CampaignResponseDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
