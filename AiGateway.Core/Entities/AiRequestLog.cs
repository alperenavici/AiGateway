using System.ComponentModel.DataAnnotations;
using AiGateway.Core.Enums;

namespace AiGateway.Core.Entities;

public class AiRequestLog
{
    [Key]
    public Guid RequestId { get; set; }
    public string Prompt { get; set; }
    public string? Response{ get; set; }
    public string Model { get; set; }
    public AiRequestStatus Status { get; set; }=AiRequestStatus.Processing;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}