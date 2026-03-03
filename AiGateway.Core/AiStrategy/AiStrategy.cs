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
}
