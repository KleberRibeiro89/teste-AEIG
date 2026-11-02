using AEIG.Domain.AppService.Dtos.QueryRequests.AssociadoQueryRequests;
using AEIG.Domain.AppService.Dtos.Requests.AssociadoRequests;
using AEIG.Domain.AppService.Dtos.Responses.AssociadoResponses;
using AEIG.Domain.AppService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AEIG.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public class AssociadoController : ControllerBase
{
    private readonly IAssociadoService _associadoService;

    public AssociadoController(IAssociadoService associadoService)
    {
        _associadoService = associadoService;
    }

    [HttpGet("paged")]
    public async Task<ActionResult<AEIG.Domain.DynamicQueryResult<AssociadoResponse>>> GetPaged([FromQuery] AssociadoQueryRequest query)
    {
        var result = await _associadoService.GetPagedAsync(query);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AssociadoResponse>> GetById(Guid id)
    {
        var associado = await _associadoService.GetByIdAsync(id);
        return associado is null ? NotFound() : Ok(associado);
    }

    [HttpPost]
    public async Task<ActionResult<AssociadoResponse>> Create(AssociadoRequest request)
    {
        var created = await _associadoService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.NavigationId }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AssociadoResponse>> Update(Guid id, AssociadoRequest request)
    {
        var updated = await _associadoService.UpdateAsync(id, request);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _associadoService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
