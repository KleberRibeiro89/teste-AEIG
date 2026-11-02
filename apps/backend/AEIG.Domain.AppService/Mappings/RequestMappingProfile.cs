using AEIG.Domain.AppService.Dtos.Requests.AssociadoRequests;
using AEIG.Domain.AppService.Dtos.Requests.CobrancaRequests;
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
            .ForMember(dest => dest.HashSenha, opt =>
            {
                opt.PreCondition(src => !string.IsNullOrWhiteSpace(src.Senha));
                opt.MapFrom(src => PasswordHasher.Hash(src.Senha));
            });

        CreateMap<AssociadoRequest, Associado>()
            .ForMember(dest => dest.Cpf, opt => opt.MapFrom(src => new string(src.Cpf.Where(char.IsDigit).ToArray())));

        CreateMap<CobrancaRequest, Cobranca>()
            .ForMember(dest => dest.IdStatus, opt => opt.Condition(src => src.IdStatus > 0))
            .ForMember(dest => dest.Valor, opt => opt.Condition(src => src.Valor is > 0))
            .ForMember(dest => dest.ValorPago, opt => opt.Condition(src => src.ValorPago.HasValue))
            .ForMember(dest => dest.DataVencimento, opt => opt.Condition(src => src.DataVencimento.HasValue))
            .ForMember(dest => dest.DataPagamento, opt => opt.Condition(src => src.DataPagamento.HasValue));
    }
}
