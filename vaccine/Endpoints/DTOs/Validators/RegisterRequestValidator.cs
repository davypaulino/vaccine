using FluentValidation;
using vaccine.Domain.Enums;
using vaccine.Endpoints.DTOs.Requests;

namespace vaccine.Endpoints.DTOs.Validators;

public class RegisterRequestValidator
    : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório.")
            .EmailAddress().WithMessage("Email inválido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Senha é obrigatória.")
            .MinimumLength(8).WithMessage("A senha deve ter no mínimo 8 caracteres.")
            .Matches("[A-Z]").WithMessage("A senha deve conter ao menos uma letra maiúscula.")
            .Matches("[a-z]").WithMessage("A senha deve conter ao menos uma letra minúscula.")
            .Matches("[0-9]").WithMessage("A senha deve conter ao menos um número.")
            .Matches("[^a-zA-Z0-9]").WithMessage("A senha deve conter ao menos um caractere especial.");
        
        RuleFor(x => x.Document)
            .NotEmpty().WithMessage("CPF é obrigatório.")
            .Must(BeValidCpf)
            .WithMessage("CPF inválido.");
    }
    
    private static bool BeValidCpf(string document)
        => new Cpf(document).IsValid();
}