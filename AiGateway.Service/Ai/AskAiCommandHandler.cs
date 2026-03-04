using AiGateway.Core.Entities;
using AiGateway.Core.Interfaces;
using AiGateway.Data.Context;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AiGateway.Service;

public class AskAiCommandHandler:IRequestHandler<AskAiCommand,string>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ApplicationDbContext _context;
    public AskAiCommandHandler(IServiceProvider serviceProvider,ApplicationDbContext context)
    {
        _serviceProvider = serviceProvider;
        _context = context;
    }


    public async Task<string> Handle(AskAiCommand request, CancellationToken cancellationToken)
    {
        var strategy = _serviceProvider.GetKeyedService<ILanguageModelStrategy>(request.ModelType);
        if (strategy == null)
        {
            throw new ArgumentException($"{request.ModelType} adında bir AI modeli bulunamadı!");        
        }
        var response=await strategy
            .GenerateResponseAsync(request.Prompt,cancellationToken);
        var log = new AiRequestLog
        {
            Prompt = request.Prompt,
            Model = request.ModelType,
            Response = response,
            CreatedAt = DateTime.UtcNow
        };
        
        await _context.AiRequestLogs.AddAsync(log, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return response;

    }
}