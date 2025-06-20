namespace MobileBanking.Data.Models.DTOs;
public class ReverseTansactionDTO
{
    public string? BVRCNO { get; init; } = "";
    public int JournalNo { get; init; } = 0;
    public required string enteredBY { get; init; }
    public string? Description { get; init; } = "";
    public string? Newbvrcno { get; init; } = "";
}
