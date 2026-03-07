namespace AiGateway.Core.Contracts;

public class AnalyzeTestMessage
{
    public Guid TaskId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string TestName { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public string StackTrace { get; set; } = string.Empty;
    public string DomSnapshot { get; set; } = string.Empty;
    
    public string ModelType { get; set; } = "Openai";
}