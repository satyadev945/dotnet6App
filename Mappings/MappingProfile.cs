using AutoMapper;
using SampleDotNet6App.DTOs;
using SampleDotNet6App.Models;

namespace SampleDotNet6App.Mappings;

/// <summary>
/// AutoMapper profile for entity to DTO mappings
/// </summary>
public class MappingProfile : Profile
{
    /// <summary>
    /// Constructor - configure mappings
    /// </summary>
    public MappingProfile()
    {
        // Product mappings
        CreateMap<Product, ProductDto>().ReverseMap();

        // User mappings
        CreateMap<User, UserDto>();
        CreateMap<RegisterRequest, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Role, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.LastLoginDate, opt => opt.Ignore());
    }
}
