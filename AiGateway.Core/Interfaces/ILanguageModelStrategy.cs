namespace AiGateway.Core.Interfaces;

public interface ILanguageModelStrategy
{
    Task<string> GenerateResponseAsync(string prompt, CancellationToken cancellationToken);
    IAsyncEnumerable<string> StreamResponseAsync(string prompt, CancellationToken cancellationToken = default);
    
}