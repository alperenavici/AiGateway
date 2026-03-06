using System.ClientModel;
using System.Runtime.CompilerServices;
using AiGateway.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;

namespace AiGateway.Core.AiStrategy;

public class OpenAiStrategy : ILanguageModelStrategy
{
    private readonly string _apiKey;
    private readonly string _model;

    public OpenAiStrategy(IConfiguration configuration)
    {
        _apiKey=configuration["ApiSettings:GroqApiKey"];
        _model=configuration["ApiSettings:GroqModel"];
    }

    private ChatClient GetClient()
    {
        var options = new OpenAIClientOptions { Endpoint = new Uri("https://api.groq.com/openai/v1/") };
        return new ChatClient(_model, new ApiKeyCredential(_apiKey), options);
    }

    public async Task<string> GenerateResponseAsync(string prompt, CancellationToken cancellationToken)
    {
        await Task.Delay(1000, cancellationToken);
        return $"OpenAi";
    }
    public async IAsyncEnumerable<string> StreamResponseAsync(string prompt, [EnumeratorCancellation] CancellationToken cancellationToken = default)    {
        var client = GetClient();
        var messages=new List<ChatMessage>(){new UserChatMessage(prompt)};
        var streamingResult=client.CompleteChatStreamingAsync(messages, cancellationToken:cancellationToken);
        await foreach (StreamingChatCompletionUpdate update in streamingResult)
        {
            if (update.ContentUpdate.Count > 0)
            {
                yield return update.ContentUpdate[0].Text;
            }
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
