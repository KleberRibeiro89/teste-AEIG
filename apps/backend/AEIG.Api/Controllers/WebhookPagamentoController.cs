using System.Text.Json.Serialization;
using AEIG.Domain.AppService.Dtos.Requests.CobrancaRequests;
using AEIG.Domain.AppService.Dtos.Responses.CobrancaResponses;
using AEIG.Domain.AppService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AEIG.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("webhooks")]
public class WebhookPagamentoController : ControllerBase
{
    private readonly ICobrancaService _cobrancaService;

    public WebhookPagamentoController(ICobrancaService cobrancaService)
    {
        _cobrancaService = cobrancaService;
    }

    [HttpPost("pagamento")]
    public async Task<IActionResult> Receber(AvisoPagamentoRequest request)
    {
        var result = await _cobrancaService.ReceberAvisoAsync(request);
        if (!result.CobrancaEncontrada)
            return NotFound(new WebhookMensagem("Cobrança não encontrada para o nosso número informado."));

        return Ok(result.Resposta);
    }

    private sealed record WebhookMensagem([property: JsonPropertyName("mensagem")] string Mensagem);
}
