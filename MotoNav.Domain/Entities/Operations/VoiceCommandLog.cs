using MotoNav.Domain.Common;
using NetTopologySuite.Geometries;

namespace MotoNav.Domain.Entities.Operations;

public class VoiceCommandLog : BaseEntity
{
    public Guid UserId { get; set; }
    public string RawTranscribedText { get; set; } = string.Empty; // Söylenen ham metin
    public string DetectedIntent { get; set; } = string.Empty;     // Algılanan niyet (Örn: "ReportHazard")
    public Point Location { get; set; } = null!;                   // Komutun verildiği anlık GPS
    public bool IsProcessedSuccessfully { get; set; } = false;
}