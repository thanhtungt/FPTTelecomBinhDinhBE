namespace FPTTelecomBE.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty; // "Internet Gia Đình", "Combo Đa Dịch Vụ"

        public string Slug { get; set; } = string.Empty; // "internet-gia-dinh", "combo-da-dich-vu

        public int DisplayOrder { get; set; } = 0; // Thứ tự hiển thị

        public bool Active { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public ICollection<Package> Packages { get; set; } = new List<Package>();
    }
}