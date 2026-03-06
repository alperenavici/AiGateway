using AiGateway.Core.Contracts;
using AiGateway.Core.Enums;
using AiGateway.Core.Interfaces;
using AiGateway.Data.Context;
using AiGateway.Service.Hubs;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AiGateway.Service.Consumers;

public class AiProcessingConsumer:IConsumer<AiProcessingMessage>
{
    private readonly ApplicationDbContext _context;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AiProcessingConsumer> _logger;
    private readonly IHubContext<AiHub> _hubContext;
    public AiProcessingConsumer(ApplicationDbContext context, IServiceProvider serviceProvider,
        ILogger<AiProcessingConsumer> logger,IHubContext<AiHub> hubContext)
    {
        _hubContext=hubContext;
        _context = context;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<AiProcessingMessage> context)
    {
        var message = context.Message;
        _logger.LogInformation("Yeni mesaj yakalandı! LogId: {LogId}, Model: {Model}", message.LogId, message.ModelType);
        var log=await _context.AiRequestLogs
            .FirstOrDefaultAsync(x=>x.RequestId==message.LogId,context.CancellationToken);
        if (log == null)
        {
            throw new Exception($"Kritik Hata: {message.LogId} numaralı log veritabanında bulunamadı!");
        }
        
        var strategy=_serviceProvider.GetKeyedService<ILanguageModelStrategy>(message.ModelType);
        if (strategy == null)
        {
            throw new ArgumentException($"Sistemde {message.ModelType} adında bir AI modeli kayıtlı değil!");        
        }
        try
        {
            _logger.LogInformation("Yapay zekaya soru soruluyor...");
            var fullResponseBuilder = new System.Text.StringBuilder();
            await foreach (var chunk in strategy.StreamResponseAsync(message.Prompt, context.CancellationToken))
            {
                fullResponseBuilder.Append(chunk); 
                await _hubContext.Clients.Group(message.LogId.ToString())
                    .SendAsync("ReceiveAiResponseChunk", message.LogId, chunk, context.CancellationToken);
            }
            log.Response = fullResponseBuilder.ToString();
            log.Status = AiRequestStatus.Completed; 
            _logger.LogInformation("Streaming bitti, veritabanı güncelleniyor.");
            await _hubContext.Clients.Group(message.LogId.ToString())
                .SendAsync("ReceiveAiResponseCompleted", message.LogId, context.CancellationToken);
        }
        catch (Exception ex)
        {
            log.Status = AiRequestStatus.Failed;
            log.Response = $"Hata oluştu: {ex.Message}";
            _logger.LogError(ex, "Yapay zeka çağrısında hata!");
        
            await _hubContext.Clients.Group(message.LogId.ToString())
                .SendAsync("ReceiveAiResponseChunk", message.LogId, "\n[SİSTEM HATASI: Yapay zekaya ulaşılamadı.]", context.CancellationToken);
            
            await _hubContext.Clients.Group(message.LogId.ToString())
                .SendAsync("ReceiveAiResponseCompleted", message.LogId, context.CancellationToken);
        
        }

        await _context.SaveChangesAsync(context.CancellationToken);
    
        _logger.LogInformation("İşlem başarıyla tamamlandı ve veritabanına işlendi! LogId: {LogId}", message.LogId);
        _logger.LogInformation("SignalR üzerinden müşteriye anons geçiliyor...");
        


    }
}