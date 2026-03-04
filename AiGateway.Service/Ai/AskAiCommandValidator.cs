using FluentValidation;
using FluentValidation.Validators;

namespace AiGateway.Service;

public class AskAiCommandValidator : AbstractValidator<AskAiCommand>
{
    public AskAiCommandValidator()
    {
            RuleFor(x=>x.Prompt)
                .NotEmpty().WithMessage("Yapay zekaya sorulacak soru boş olamaz.")
                .MinimumLength(3).WithMessage("Soru en az 3 karakter olmalıdır");

            RuleFor(x => x.ModelType)
                .NotEmpty().WithMessage("Model tipi belirtilmelidir.")
                .Must(type => type == "Openai" || type == "LocalLlm")
                .WithMessage("Sadece 'Openai' veya 'LocalLlm' modelleri desteklenmektedir");

    }
    
}