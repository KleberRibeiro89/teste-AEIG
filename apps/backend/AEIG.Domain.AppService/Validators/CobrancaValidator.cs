using System.Text.RegularExpressions;
using AEIG.Domain.Entities;
using FluentValidation;

namespace AEIG.Domain.AppService.Validators;

public partial class CobrancaValidator : AbstractValidator<Cobranca>
{
    public CobrancaValidator()
    {
        RuleFor(c => c.IdAssociado).GreaterThan(0).WithMessage("Informe o associado.");
        RuleFor(c => c.MesReferencia)
            .NotEmpty()
            .Matches(ReferenceMonthPattern()).WithMessage("O mês de referência deve estar no formato AAAA-MM.");
        RuleFor(c => c.NossoNumero).NotEmpty().MaximumLength(20);
        RuleFor(c => c.Valor).GreaterThan(0).WithMessage("Informe o valor da cobrança.");
        RuleFor(c => c.ValorPago).GreaterThanOrEqualTo(0).WithMessage("O valor pago não pode ser negativo.");
        RuleFor(c => c.DataVencimento).NotEmpty().WithMessage("Informe a data de vencimento.");
        RuleFor(c => c.IdStatus).GreaterThan(0).WithMessage("Informe o status.");
    }

    [GeneratedRegex(@"^\d{4}-(0[1-9]|1[0-2])$")]
    private static partial Regex ReferenceMonthPattern();
}
