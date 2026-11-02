using AEIG.Domain.AppService.Dtos.Responses.DashboardResponses;
using AEIG.Domain.AppService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AEIG.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;

    public DashboardController(IDashboardService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<DashboardResponse>> Get()
    {
        return Ok(await _service.GetAsync());
    }
}
