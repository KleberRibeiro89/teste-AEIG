using AEIG.Domain.AppService.Dtos.Requests.UserRequests;
using AEIG.Domain.Entities;
using AEIG.Domain.Security;
using AutoMapper;

namespace AEIG.Domain.AppService.Mappings;

public class RequestMappingProfile : Profile
{
    public RequestMappingProfile()
    {
        CreateMap<UserRequest, User>()
            .ForMember(dest => dest.PasswordHash, opt =>
            {
                opt.PreCondition(src => !string.IsNullOrWhiteSpace(src.Password));
                opt.MapFrom(src => PasswordHasher.Hash(src.Password));
            });
    }
}
