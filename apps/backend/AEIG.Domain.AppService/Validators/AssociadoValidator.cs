using AEIG.Domain.Entities;
using AEIG.Domain.Validation;
using FluentValidation;

namespace AEIG.Domain.AppService.Validators;

public class AssociadoValidator : AbstractValidator<Associado>
{
    public AssociadoValidator()
    {
        RuleFor(a => a.Name).NotEmpty().MaximumLength(200);
        RuleFor(a => a.Cpf)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Matches(@"^\d{11}$").WithMessage("CPF deve ter 11 dígitos.")
            .Must(CpfValidator.IsValid).WithMessage("CPF inválido.");
        RuleFor(a => a.MonthlyFee).GreaterThan(0).WithMessage("Informe o valor da mensalidade.");
    }
}
