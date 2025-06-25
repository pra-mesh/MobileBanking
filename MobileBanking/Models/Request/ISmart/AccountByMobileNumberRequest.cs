using System.ComponentModel.DataAnnotations;

namespace MobileBanking.Models.Request.ISmart;

public class AccountByMobileNumberRequest
{
    [Required]
    [StringLength(10, MinimumLength = 10, ErrorMessage = "Invalid Mobile Numbe provided")]
    public required string mobileNumber { get; set; }
}
