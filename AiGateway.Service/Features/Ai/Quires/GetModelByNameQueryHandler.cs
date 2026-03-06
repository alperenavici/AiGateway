using AiGateway.Core.Dto;
using AiGateway.Core.Entities;
using AiGateway.Data.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiGateway.Service;

public class AiModelsHandler:IRequestHandler<GetModelByNameQuery,IEnumerable<AiRequestLogDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly IServiceProvider _serviceProvider;

    public AiModelsHandler(ApplicationDbContext context, IServiceProvider serviceProvider)
    {
        _context = context;
        _serviceProvider = serviceProvider;
    }

    public async Task<IEnumerable<AiRequestLogDto>> Handle(GetModelByNameQuery query,CancellationToken cancellationToken)
    {
        return await _context.AiRequestLogs
            .AsNoTracking()
            .Where(x=>x.Model==query.ModelType)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x=>new AiRequestLogDto(x.Prompt,x.Response,x.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    
    
}