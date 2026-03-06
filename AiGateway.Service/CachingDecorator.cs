using System.Runtime.CompilerServices;
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

    public async IAsyncEnumerable<string> StreamResponseAsync(string prompt, [EnumeratorCancellation] CancellationToken cancellationToken = default)    {
        await Task.Delay(1000,cancellationToken);
        string[] words = { "Merhaba", "Alperen,", "bu", "kelimeler", "sana", "arka", "plandaki", "bir", "işçiden", "kelime", "kelime", "akarak", "geliyor!" };

        foreach (var word in words)
        {
            await Task.Delay(250,cancellationToken);
            yield return word + " ";
        }
    }
}