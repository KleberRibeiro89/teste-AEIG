using AEIG.Domain.AppService.Dtos.Requests.ParametroPagamentoRequests;
using FluentValidation;

namespace AEIG.Domain.AppService.Validators;

public class ParametroPagamentoValidator : AbstractValidator<ParametroPagamentoRequest>
{
    public ParametroPagamentoValidator()
    {
        RuleFor(p => p.MultaPercentual)
            .GreaterThanOrEqualTo(0)
            .WithMessage("A multa não pode ser negativa.");
        RuleFor(p => p.JurosDiarioPercentual)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Os juros diários não podem ser negativos.");
    }
}
