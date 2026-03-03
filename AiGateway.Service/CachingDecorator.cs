using AiGateway.Core.Interfaces;

namespace AiGateway.Service;

public class CachingDecorator:ILanguageModelStrategy
{
    private readonly ILanguageModelStrategy _innerStrategy;
    private readonly Dictionary<string, string> _cache = new();

    public CachingDecorator(ILanguageModelStrategy innerStrategy)
    {
        _innerStrategy = innerStrategy;
    }

    public async Task<string> GenerateResponseAsync(string prompt, CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue(prompt, out string cachedResponse))
        {
            Console.WriteLine("[CACHE HIT] daha once soruldu");
            return cachedResponse; 
        }
        
        var response = await _innerStrategy.GenerateResponseAsync(prompt, cancellationToken);
        _cache[response] = response;
        return response;
    }
}