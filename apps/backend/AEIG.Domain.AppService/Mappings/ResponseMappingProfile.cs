using AEIG.Domain.AppService.Dtos.Responses.AssociadoResponses;
using AEIG.Domain.AppService.Dtos.Responses.CobrancaResponses;
using AEIG.Domain.AppService.Dtos.Responses.UserResponses;
using AEIG.Domain.Entities;
using AutoMapper;

namespace AEIG.Domain.AppService.Mappings;

public class ResponseMappingProfile : Profile
{
    public ResponseMappingProfile()
    {
        CreateMap<User, UserResponse>()
            .ForMember(dest => dest.NavigationId, opt => opt.MapFrom(src => src.NavigationId));

        CreateMap<Associado, AssociadoResponse>();
        CreateMap<Cobranca, CobrancaResponse>()
            .ForMember(dest => dest.NomeStatus, opt => opt.MapFrom(src => src.Status != null ? src.Status.Nome : null));
    }
}
