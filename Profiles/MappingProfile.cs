using AutoMapper;
using SampleDotNet6App.Models;
using SampleDotNet6App.Models.DTOs;

namespace SampleDotNet6App.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDto>();
            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>();
            CreateMap<User, UserDto>();
            CreateMap<RegisterUserDto, User>();
        }
    }
}
