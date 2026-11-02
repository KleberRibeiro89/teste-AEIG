using AEIG.Domain.AppService.Dtos.QueryRequests.CobrancaQueryRequests;
using AEIG.Domain.AppService.Dtos.Requests.CobrancaRequests;
using AEIG.Domain.AppService.Dtos.Responses.CobrancaResponses;
using AEIG.Domain.AppService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AEIG.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CobrancaController : ControllerBase
{
    private readonly ICobrancaService _cobrancaService;

    public CobrancaController(ICobrancaService cobrancaService)
    {
        _cobrancaService = cobrancaService;
    }

    [HttpGet("paged")]
    public async Task<ActionResult<AEIG.Domain.DynamicQueryResult<CobrancaResponse>>> GetPaged([FromQuery] CobrancaQueryRequest query)
    {
        var result = await _cobrancaService.GetPagedAsync(query);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CobrancaResponse>> GetById(Guid id)
    {
        var cobranca = await _cobrancaService.GetByIdAsync(id);
        return cobranca is null ? NotFound() : Ok(cobranca);
    }

    [HttpPost]
    public async Task<ActionResult<CobrancaResponse>> Create(CobrancaRequest request)
    {
        var created = await _cobrancaService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.NavigationId }, created);
    }

    [HttpPost("{id:guid}/pagar")]
    public async Task<ActionResult<PagamentoResponse>> Pagar(Guid id, [FromBody] PagamentoRequest? request)
    {
        var pagamento = await _cobrancaService.PagarAsync(id, request ?? new PagamentoRequest());
        return pagamento is null ? NotFound() : Ok(pagamento);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CobrancaResponse>> Update(Guid id, CobrancaRequest request)
    {
        var updated = await _cobrancaService.UpdateAsync(id, request);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _cobrancaService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
