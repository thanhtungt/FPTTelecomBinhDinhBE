using System.ComponentModel.DataAnnotations;

namespace FPTTelecomBE.DTOs.Registration;

public class AssignStaffDto
{
    [Required(ErrorMessage = "Staff ID là bắt buộc")]
    public int StaffId { get; set; }
}