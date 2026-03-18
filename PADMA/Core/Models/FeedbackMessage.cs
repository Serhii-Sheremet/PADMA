namespace PADMA.Core.Models;

public class FeedbackMessage
{
    public string Category { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Date { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string AppVersion { get; set; } = string.Empty;
}
