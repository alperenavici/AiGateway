using System.Runtime.CompilerServices;
using AiGateway.Core.Interfaces;

namespace AiGateway.Core.AiStrategy;

public class OpenAiStrategy : ILanguageModelStrategy
{
    public async Task<string> GenerateResponseAsync(string prompt, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            return "Prompt is required.";
        }
        
        await Task.Delay(1000, cancellationToken);
        return $"[OpemAI Response:{prompt}]";
    }
    public async IAsyncEnumerable<string> StreamResponseAsync(string prompt, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await Task.Delay(800, cancellationToken); 

        string[] words = { "Merhaba,", "OpenAI", "bağlantısı", "henüz", "kurulmadı", "ama", "sistemin", "harika", "bir", "şekilde", "kelime", "kelime", "akıyor!" };

        foreach (var word in words)
        {
            await Task.Delay(200, cancellationToken);
            yield return word + " "; 
        }
    }
  
}

public class LocalLlmStrategy : ILanguageModelStrategy
{
    public async Task<string> GenerateResponseAsync(string prompt, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            return "Prompt is required.";
        }
        await Task.Delay(500,cancellationToken);
        return $"[LlmAI Response:{prompt}";
    }
    public async IAsyncEnumerable<string> StreamResponseAsync(string prompt, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await Task.Delay(500, cancellationToken); 

        string[] words = { "Selam,", "ben", "senin", "makinende", "çalışan", "yerel", "modelim.", "Sistem", "başarılı." };

        foreach (var word in words)
        {
            await Task.Delay(150, cancellationToken); 
            yield return word + " ";
        }
    }

   
}
