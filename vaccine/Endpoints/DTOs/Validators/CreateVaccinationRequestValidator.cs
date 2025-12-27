using FluentValidation;
using vaccine.Domain.Enums;
using vaccine.Endpoints.DTOs.Requests;

namespace vaccine.Endpoints.DTOs.Validators;

public class CreateVaccinationRequestValidator : AbstractValidator<CreateVaccinationRequest>
{
    
    public CreateVaccinationRequestValidator()
    {
        RuleFor(x => x.PersonId)
            .NotEmpty().WithMessage("O Id da pessoa é obrigatório.");

        RuleFor(x => x.VaccineId)
            .NotEmpty().WithMessage("O Id da vacina é obrigatório.");

        RuleFor(x => x.Doses)
            .NotNull().WithMessage("A lista de doses não pode ser nula.");
        
        RuleForEach(x => x.Doses).SetValidator(new DoseResponseValidator());
    }
}

public class DoseResponseValidator : AbstractValidator<DosesResponse>
{
    public DoseResponseValidator()
    {
        RuleFor(d => d.DoseType)
            .NotEqual(EDoseType.None)
            .WithMessage("O tipo da dose deve ser válido.");

        RuleFor(d => d.AppliedAt)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("A data de aplicação não pode ser no futuro.");
    }
}