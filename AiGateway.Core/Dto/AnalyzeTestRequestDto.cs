namespace AiGateway.Core.Dto;

public record AnalyzeTestRequestDto(string ProjectName,
    string TestName,
    string ErrorMessage,
    string StackTrace,
    string DomSnapshot,
    string ModelType = "Openai" 
);