namespace AiGateway.Core.Dto;

public record AiRequestLogDto(string Prompt, string Response, DateTime CreatedAt);