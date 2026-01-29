using FPTWifiBE.Models;

namespace FPTTelecomBE.Models
{
    public class Registration
    {
        public int Id { get; set; }

        public int? UserId { get; set; }
        public User? User { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty; // "123 Lê Lợi, Quy Nhơn, Bình Định"

        public int PackageId { get; set; }
        public Package? Package { get; set; }

        public string? Note { get; set; } // "Nhà 2 tầng cần Mesh, lắp gấp"

        public string Status { get; set; } = "pending"; // pending, contacting, need_survey, surveyed, contract_signed, installation_scheduled, installed, cancelled, done

        public int? AssignedStaffId { get; set; }
        public User? AssignedStaff { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
