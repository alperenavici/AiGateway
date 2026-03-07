using System.ComponentModel.DataAnnotations;
using AiGateway.Core.Enums;

namespace AiGateway.Core.Entities;

public class TestAnalysisTask
{
    [Key]
    public Guid TestId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string TestName { get; set; } = string.Empty;

    public string ErrorMessage { get; set; } = string.Empty; // Timeout 3000
    public string StackTrace { get; set; } = string.Empty; // Konsoldaki o uzun kırmızı log
    public string DomSnapshot { get; set; } = string.Empty; // O anki HTML sayfasının yapısı

    public string? AiAnalysisResult { get; set; }
    public AiRequestStatus Status { get; set; } 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
}