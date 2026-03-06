using AiGateway.Core.Contracts;
using AiGateway.Core.Dto;
using AiGateway.Core.Entities;
using AiGateway.Core.Enums;
using AiGateway.Core.Interfaces;
using AiGateway.Data.Context;
using MassTransit;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AiGateway.Service;

public class AskAiCommandHandler:IRequestHandler<AskAiCommand,Guid>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ApplicationDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;
    public AskAiCommandHandler(IServiceProvider serviceProvider,ApplicationDbContext context,IPublishEndpoint publishEndpoint)
    {
        _serviceProvider = serviceProvider;
        _context = context;
        _publishEndpoint = publishEndpoint;
    }


    public async Task<Guid> Handle(AskAiCommand request, CancellationToken cancellationToken)
    {
        var strategy = _serviceProvider.GetKeyedService<ILanguageModelStrategy>(request.ModelType);
        if (strategy == null)
        {
            throw new ArgumentException($"{request.ModelType} adında bir AI modeli bulunamadı!");        
        }
        
        var logEntry = new AiRequestLog
        {
            RequestId = Guid.NewGuid(),
            Prompt = request.Prompt,
            Model = request.ModelType,
            Status = AiRequestStatus.Processing,
        };
        
        _context.AiRequestLogs.Add(logEntry);
        await _context.SaveChangesAsync(cancellationToken);
        var message=new AiProcessingMessage(logEntry.RequestId, logEntry.Prompt,request.ModelType);
        await _publishEndpoint.Publish(message, cancellationToken);
        return logEntry.RequestId;

    }
    
}