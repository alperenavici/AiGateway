using System.Text;
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

namespace AiGateway.Service.Features.Ai.Consumers;

public class AnalyzeTestConsumer:IConsumer<AnalyzeTestMessage>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AnalyzeTestConsumer> _logger;
    private readonly IHubContext<AiHub> _hubContext;
    private readonly IServiceProvider _serviceProvider;

    public AnalyzeTestConsumer(ApplicationDbContext context, IServiceProvider serviceProvider,
        ILogger<AnalyzeTestConsumer> logger, IHubContext<AiHub> hubContext)
    {
        _context = context;
        _logger = logger;
        _hubContext = hubContext;
        _serviceProvider = serviceProvider;
    }

    public async Task Consume(ConsumeContext<AnalyzeTestMessage> context)
    {
        var msg=context.Message;
        _logger.LogInformation("Yeni test analizi yakalandı! TaskId: {TaskId}, Test: {TestName}", msg.TaskId, msg.TestName);
        var taskRecord = await _context.TestAnalysisTasks
            .FirstOrDefaultAsync(x => x.TestId == msg.TaskId, context.CancellationToken);
        if (taskRecord == null)
        {
            _logger.LogCritical("Kritik Hata: {TaskId} numaralı test kaydı veritabanında yok!", msg.TaskId);
            return; 
        }
        var strategy = _serviceProvider.GetKeyedService<ILanguageModelStrategy>(msg.ModelType);
        if (strategy == null) throw new ArgumentException($"Sistemde {msg.ModelType} AI modeli bulunamadı!");
        try
        {
            taskRecord.Status = AiRequestStatus.Processing;
            await _context.SaveChangesAsync(context.CancellationToken);

            string engineeredPrompt = $@"
Sen kıdemli bir Frontend Developer ve QA (Kalite Güvence) mühendisisin.
Aşağıda başarısız olan bir '{msg.TestName}' testinin detayları var.
Görevin bu hatanın kök nedenini bulmak ve çözüm kodunu vermektir.

KURALLAR:
- Asla 'Merhaba', 'Tabii ki yardımcı olayım' gibi giriş cümleleri kullanma.
- Hatayı tek bir cümleyle özetle.
- Sadece değiştirilmesi gereken spesifik kodu Markdown formatında ver.

--- HATA LOGU (STACK TRACE) ---
{msg.StackTrace}

--- DOM SNAPSHOT (HATA ANINDAKİ EKRAN) ---
{msg.DomSnapshot}
";

            var fullResponseBuilder = new StringBuilder();
            
            await foreach (var chunk in strategy.StreamResponseAsync(engineeredPrompt, context.CancellationToken))
            {
                fullResponseBuilder.Append(chunk); // DB için hafızada biriktir
                
                await _hubContext.Clients.Group(msg.TaskId.ToString())
                    .SendAsync("ReceiveAiResponseChunk", msg.TaskId, chunk, context.CancellationToken);
            }

            taskRecord.AiAnalysisResult = fullResponseBuilder.ToString();
            taskRecord.Status = AiRequestStatus.Completed;
            
            await _hubContext.Clients.Group(msg.TaskId.ToString())
                .SendAsync("ReceiveAiResponseCompleted", msg.TaskId, context.CancellationToken);
        }
        catch (Exception ex)
        {
            taskRecord.Status = AiRequestStatus.Failed;
            taskRecord.AiAnalysisResult = $"Sistem Hatası: {ex.Message}";
            _logger.LogError(ex, "Yapay zeka analizi sırasında patladık!");

            await _hubContext.Clients.Group(msg.TaskId.ToString())
                .SendAsync("ReceiveAiResponseChunk", msg.TaskId, "\n\n[SİSTEM HATASI: Yapay zeka motoruna ulaşılamadı veya analiz başarısız oldu.]", context.CancellationToken);
            
            await _hubContext.Clients.Group(msg.TaskId.ToString())
                .SendAsync("ReceiveAiResponseCompleted", msg.TaskId, context.CancellationToken);
        }

        await _context.SaveChangesAsync(context.CancellationToken);
        _logger.LogInformation("Test analizi tamamlandı ve kaydedildi. TaskId: {TaskId}", msg.TaskId);
    }
    
}