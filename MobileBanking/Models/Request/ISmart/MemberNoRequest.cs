using System.ComponentModel.DataAnnotations;

namespace MobileBanking.Models.Request.ISmart;

public class MemberNoRequest
{
    [Required]
    public required string memberID { get; set; }
}
