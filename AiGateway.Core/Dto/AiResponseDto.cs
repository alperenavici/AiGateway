namespace AiGateway.Core.Dto;

public record AiResponseDto(string Prompt,string Response,string ModelType,DateTime CreatedAt);