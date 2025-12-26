using FluentValidation;
using vaccine.Endpoints.DTOs.Requests;

namespace vaccine.Endpoints.DTOs.Validators;

public class CreatePersonRequestValidator : AbstractValidator<CreatePersonRequest>
{
    public CreatePersonRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MaximumLength(100).WithMessage("O nome não pode ter mais de 100 caracteres.");
        
        RuleFor(x => x.Document)
            .NotEmpty().WithMessage("CPF é obrigatório.")
            .Must(BeValidCpf)
            .WithMessage("CPF inválido.");
        
        RuleFor(x => x.BirthDate)
            .NotEmpty().WithMessage("A data de nascimento é obrigatória.")
            .LessThan(DateTime.UtcNow).WithMessage("A data de nascimento deve ser no passado.");
    }
    
    private static bool BeValidCpf(string document)
        => new Cpf(document).IsValid();

    
}
