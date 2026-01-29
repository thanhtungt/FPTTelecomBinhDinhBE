namespace FPTTelecomBE.Models
{
    public class Package
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty; // "Internet Giga", "Combo Sky + FPT Play", "SpeedX2 Pro WiFi 7"...

        public int SpeedDown { get; set; } // Mbps

        public int SpeedUp { get; set; }

        public decimal PriceMonthly { get; set; } // VND

        public string PromotionText { get; set; } = string.Empty; // "Tặng Modem WiFi 6 + Giảm 50k online + Tặng 1 tháng nếu trả trước 12 tháng"

        public string DeviceBonus { get; set; } = string.Empty; // "Modem WiFi 6 + 1 Mesh + FPT Play Box"

        public bool Active { get; set; } = true;
    }
}
