using AiGateway.Core.Contracts;
using AiGateway.Core.Dto;
using AiGateway.Core.Entities;
using AiGateway.Core.Enums;
using AiGateway.Data.Context;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AiGateway.WebApi.Controller;
[ApiController]
[Route("api/[controller]")]
public class TestAnalysisController: ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;
    public TestAnalysisController(ApplicationDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    [HttpPost("analyze")]
    public async Task<IActionResult> AnalyzeFailedTest([FromBody] AnalyzeTestRequestDto request, CancellationToken cancellationToken)
    {
        var analysisTask = new TestAnalysisTask
        {
            TestId  = Guid.NewGuid(), 
            ProjectName = request.ProjectName,
            TestName = request.TestName,
            ErrorMessage = request.ErrorMessage,
            StackTrace = request.StackTrace,
            DomSnapshot = request.DomSnapshot,
            Status = AiRequestStatus.Pending, 
            CreatedAt = DateTime.UtcNow
        };

        _context.TestAnalysisTasks.Add(analysisTask);
        await _context.SaveChangesAsync(cancellationToken);

        var message = new AnalyzeTestMessage
        {
            TaskId = analysisTask.TestId,
            ProjectName = analysisTask.ProjectName,
            TestName = analysisTask.TestName,
            ErrorMessage = analysisTask.ErrorMessage,
            StackTrace = analysisTask.StackTrace,
            DomSnapshot = analysisTask.DomSnapshot,
            ModelType = request.ModelType
        };

        await _publishEndpoint.Publish(message, cancellationToken);

        return Ok(analysisTask.TestId);
    }
    
    [HttpGet("history")]
    public async Task<IActionResult> GetHistory(CancellationToken cancellationToken)
    {
        var history = await _context.TestAnalysisTasks
            .OrderByDescending(x => x.CreatedAt)
            .Take(50) // Son 50 patlayan test
            .Select(x => new 
            {
                x.TestId,
                x.ProjectName,
                x.TestName,
                Status = x.Status.ToString(), // Enum'ı string'e çeviriyoruz
                x.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return Ok(history);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTaskDetails(Guid id, CancellationToken cancellationToken)
    {
        var task = await _context.TestAnalysisTasks
            .FirstOrDefaultAsync(x => x.TestId == id, cancellationToken);

        if (task == null) return NotFound("Görev bulunamadı.");

        return Ok(task);
    }
    
}