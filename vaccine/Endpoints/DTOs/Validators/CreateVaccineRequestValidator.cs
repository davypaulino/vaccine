using FluentValidation;
using vaccine.Endpoints.DTOs.Requests;

namespace vaccine.Endpoints.DTOs.Validators;

public class CreateVaccineRequestValidator 
    : AbstractValidator<CreateVaccineRequest>
{
    public CreateVaccineRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("O nome da vacina é obrigatório.")
            .MaximumLength(100)
            .WithMessage("O nome da vacina deve ter no máximo 100 caracteres.");

        RuleFor(x => x.AvailableEDoses)
            .NotNull()
            .WithMessage("O tipo de dose é obrigatório.")
            .Must(doses => doses.Length > 0)
            .WithMessage("Informe ao menos um tipo de dose.");

        RuleForEach(x => x.AvailableEDoses)
            .IsInEnum()
            .WithMessage("Tipo de dose inválido.");
    }
}