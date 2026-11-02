using AEIG.Domain.Entities;
using FluentValidation;

namespace AEIG.Domain.AppService.Validators;

public class AssociadoValidator : AbstractValidator<Associado>
{
    public AssociadoValidator()
    {
        RuleFor(a => a.Name).NotEmpty().MaximumLength(200);
        RuleFor(a => a.Cpf).NotEmpty().Matches(@"^\d{11}$").WithMessage("CPF deve ter 11 dígitos.");
        RuleFor(a => a.MonthlyFee).GreaterThan(0).WithMessage("Informe o valor da mensalidade.");
    }
}
