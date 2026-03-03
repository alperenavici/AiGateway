using AiGateway.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AiGateway.Service;

public class AskAiCommandHandler:IRequestHandler<AskAiCommand,string>
{
    private readonly IServiceProvider _serviceProvider;

    public AskAiCommandHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }


    public async Task<string> Handle(AskAiCommand request, CancellationToken cancellationToken)
    {
        var strategy = _serviceProvider.GetKeyedService<ILanguageModelStrategy>(request.ModelType);
        if (strategy == null)
        {
            throw new ArgumentException($"{request.ModelType} adında bir AI modeli bulunamadı!");        }
        return await strategy.GenerateResponseAsync(request.Prompt, cancellationToken);

    }
}