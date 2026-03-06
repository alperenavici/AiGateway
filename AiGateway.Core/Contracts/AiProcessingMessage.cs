namespace AiGateway.Core.Contracts;

public record AiProcessingMessage(Guid LogId,string Prompt,string ModelType);