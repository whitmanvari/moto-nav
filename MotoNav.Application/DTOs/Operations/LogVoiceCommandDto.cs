namespace MotoNav.Application.DTOs.Operations;

public class LogVoiceCommandDto
{
    public Guid UserId { get; set; }
    public string RawTranscribedText { get; set; } = string.Empty;
    public string DetectedIntent { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public bool IsProcessedSuccessfully { get; set; }
}