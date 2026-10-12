using AutoMapper;
using SampleDotNet6App.Models;

namespace SampleDotNet6App.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping between entities and DTOs.
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Product mappings
            CreateMap<Product, ProductDto>();
            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // User mappings
            CreateMap<User, UserDto>();
            CreateMap<RegisterUserDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
        }
    }
}
