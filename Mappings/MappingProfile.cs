using SampleDotNet6App.Models;

namespace SampleDotNet6App.Mappings
{
    /// <summary>
    /// Manual object mapper replacing AutoMapper for .NET 10 compatibility.
    /// </summary>
    public static class MappingProfile
    {
        public static ProductDto ToDto(this Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Category = product.Category,
                StockQuantity = product.StockQuantity,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt
            };
        }

        public static IEnumerable<ProductDto> ToDtoList(this IEnumerable<Product> products)
        {
            return products.Select(p => p.ToDto());
        }

        public static UserDto ToDto(this User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }

        public static IEnumerable<UserDto> ToDtoList(this IEnumerable<User> users)
        {
            return users.Select(u => u.ToDto());
        }
    }
}
