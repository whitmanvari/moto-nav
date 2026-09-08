namespace MotoNav.Application.DTOs.Hazards;

public class VerifyHazardDto
{
    public Guid UserId { get; set; }

    /// true: Tehlike hala var (UpVote), false: Yol temiz/yok (DownVote)

    public bool StillPresent { get; set; }

    public string? Comment { get; set; }
}