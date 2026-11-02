using AEIG.Domain.AppService.Dtos.Requests.CobrancaRequests;
using FluentValidation;

namespace AEIG.Domain.AppService.Validators;

public class AvisoPagamentoValidator : AbstractValidator<AvisoPagamentoRequest>
{
    public AvisoPagamentoValidator()
    {
        RuleFor(a => a.IdEvento)
            .NotEmpty().WithMessage("Informe o identificador do evento.")
            .MaximumLength(100);
        RuleFor(a => a.NossoNumero)
            .NotEmpty().WithMessage("Informe o nosso número.")
            .MaximumLength(20);
        RuleFor(a => a.DataPagamento)
            .Must(data => data != default)
            .WithMessage("Informe a data de pagamento.");
        RuleFor(a => a.DataPagamento)
            .Must(data => data == default || data <= DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("A data de pagamento não pode ser futura.");
    }
}
