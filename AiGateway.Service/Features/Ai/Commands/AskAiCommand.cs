using AiGateway.Core.Dto;
using AiGateway.Core.Entities;
using MediatR;

namespace AiGateway.Service;

public record AskAiCommand(string Prompt,string ModelType):IRequest<Guid>;
