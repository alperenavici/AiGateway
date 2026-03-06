using AiGateway.Core.Dto;
using MediatR;

namespace AiGateway.Service;

public record GetAiResponseQuery(Guid RequestId):IRequest<AiResponseDto>;
