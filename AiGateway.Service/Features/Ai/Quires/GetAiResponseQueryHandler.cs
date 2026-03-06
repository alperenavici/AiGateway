using AiGateway.Core.Dto;
using Dapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace AiGateway.Service;

public class GetAiResponseQueryHandler:IRequestHandler<GetAiResponseQuery, AiResponseDto>
{
    private readonly string _connectionString;
    public GetAiResponseQueryHandler(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
                            ?? throw new ArgumentNullException("Connection string bulunamadı!");
    }

    public async Task<AiResponseDto> Handle(GetAiResponseQuery request, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        const string sql = """
                               SELECT Prompt, Response, ModelType, CreatedAt
                               FROM AiRequestLogs
                               WHERE Id = @RequestId
                           """;

        var result = await connection.QueryFirstOrDefaultAsync<AiResponseDto>(
            new CommandDefinition(sql, new { Id = request.RequestId }, cancellationToken: cancellationToken)
        );

        if (result == null)
        {
            throw new KeyNotFoundException("Bu ID'ye ait bir AI yanıtı bulunamadı.");
        }

        return result;
    }
    
    
}