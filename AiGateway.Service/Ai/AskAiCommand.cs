using MediatR;

namespace AiGateway.Service;

public record AskAiCommand(string Prompt,string ModelType):IRequest<string>;