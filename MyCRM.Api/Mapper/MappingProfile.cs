using AutoMapper;
using MyCRM.Domain.Constants;
using MyCRM.Domain.Entities;
using MyCRM.Application.DTOs;

namespace MyCRM.Api.Mapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Client
        CreateMap<ClientCreateDto, Client>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ViewCount, opt => opt.Ignore())
            .ForMember(dest => dest.Contacts, opt => opt.Ignore())
            .ForMember(dest => dest.Deals, opt => opt.Ignore());

        CreateMap<ClientUpdateDto, Client>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ViewCount, opt => opt.Ignore())
            .ForMember(dest => dest.Contacts, opt => opt.Ignore())
            .ForMember(dest => dest.Deals, opt => opt.Ignore());

        CreateMap<Client, ClientDto>()
            .ForMember(dest => dest.ViewCount, opt => opt.MapFrom(src => src.ViewCount));

        // Deal
        CreateMap<DealCreateDto, Deal>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => System.Enum.Parse<DealStatus>(src.Status, true)))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Tasks, opt => opt.Ignore());

        CreateMap<DealUpdateDto, Deal>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => System.Enum.Parse<DealStatus>(src.Status, true)))
            .ForMember(dest => dest.Tasks, opt => opt.Ignore());

        CreateMap<Deal, DealDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client != null ? $"{src.Client.Name} {src.Client.Surname}" : "Unknown"));

        CreateMap<MyCRM.Application.DTOs.DealDto, MyCRM.Application.DTOs.DealDto>();
    }
}
