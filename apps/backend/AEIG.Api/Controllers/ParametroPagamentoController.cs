using AEIG.Domain.AppService.Dtos.Requests.ParametroPagamentoRequests;
using AEIG.Domain.AppService.Dtos.Responses.ParametroPagamentoResponses;
using AEIG.Domain.AppService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AEIG.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public class ParametroPagamentoController : ControllerBase
{
    private readonly IParametroPagamentoService _service;

    public ParametroPagamentoController(IParametroPagamentoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ParametroPagamentoResponse>> Get()
    {
        var parametro = await _service.GetAtivoAsync();
        return parametro is null ? NotFound() : Ok(parametro);
    }

    [HttpPut]
    public async Task<ActionResult<ParametroPagamentoResponse>> Update(ParametroPagamentoRequest request)
    {
        var updated = await _service.AtualizarAsync(request);
        return updated is null ? NotFound() : Ok(updated);
    }
}
