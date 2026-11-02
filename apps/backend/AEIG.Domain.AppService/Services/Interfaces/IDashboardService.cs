using AEIG.Domain.AppService.Dtos.Responses.DashboardResponses;

namespace AEIG.Domain.AppService.Interfaces;

public interface IDashboardService
{
    Task<DashboardResponse> GetAsync();
}
