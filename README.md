# Sample .NET 10 Application

This is a comprehensive sample .NET 10 Web API application designed for testing version upgrade tools. It demonstrates common patterns and features used in real-world applications.

## Features

- **.NET 10 Web API** with minimal hosting model
- **Entity Framework Core 10** with in-memory database
- **JWT Authentication** and role-based authorization
- **AutoMapper** for object mapping
- **Built-in OpenAPI** documentation (ASP.NET Core 10 native, replaces Swashbuckle)
- **CORS** support
- **Structured logging**
- **Comprehensive API endpoints** for products and users
- **Health check endpoints**

## Architecture

### Models
- `Product` - Product entity with properties like Name, Price, Category, etc.
- `User` - User entity with authentication and role management

### Controllers
- `ProductsController` - CRUD operations for products with role-based access
- `UsersController` - User registration, authentication, and profile management
- `HealthController` - Health check endpoints

### Services
- `IProductService` / `ProductService` - Business logic for product operations
- `IUserService` / `UserService` - Business logic for user operations and authentication

### Features Tested by Upgrade Tools
- **Target Framework**: `net10.0`
- **Package References**: EF Core 10.x, Authentication packages, etc.
- **Minimal Hosting Model**: Uses `WebApplication.CreateBuilder()`
- **Global Using Directives**: Uses `ImplicitUsings` and `Nullable` features
- **Configuration Pattern**: Uses the new configuration system
- **Dependency Injection**: Uses built-in DI container
- **Built-in OpenAPI**: Uses `AddOpenApi()` / `MapOpenApi()` (no Swashbuckle needed)

## Getting Started

### Prerequisites
- .NET 10 SDK
- Visual Studio 2022 (17.12+) or VS Code

### Running the Application

1. **Restore packages**:
   ```bash
   dotnet restore
   ```

2. **Run the application**:
   ```bash
   dotnet run
   ```

3. **Open OpenAPI UI**:
   Navigate to `https://localhost:7001/openapi/v1.json` or `http://localhost:5001/openapi/v1.json`

### Default Users

The application seeds with these default users:
- **Admin**: Username: `admin`, Password: `Admin123!`
- **Manager**: Username: `jane.smith`, Password: `Manager123!`
- **User**: Username: `john.doe`, Password: `User123!`

## API Endpoints

### Authentication
- `POST /api/users/login` - User login
- `POST /api/users/register` - User registration
- `GET /api/users/profile` - Get current user profile

### Products (Requires Authentication)
- `GET /api/products` - Get all products
- `GET /api/products/{id}` - Get product by ID
- `GET /api/products/search?q={term}` - Search products
- `POST /api/products` - Create product (Admin/Manager only)
- `PUT /api/products/{id}` - Update product (Admin/Manager only)
- `DELETE /api/products/{id}` - Delete product (Admin only)

### Users (Admin Only)
- `GET /api/users` - Get all users
- `GET /api/users/{id}` - Get user by ID

### Health Checks
- `GET /api/health` - Basic health check
- `GET /api/health/detailed` - Detailed health information

## Configuration

The application uses standard .NET configuration with these key sections:
- `ConnectionStrings` - Database connections
- `Jwt` - JWT token configuration
- `Logging` - Logging configuration
- `ApiSettings` - Custom API settings

## .NET 10 Upgrade Notes

### Key Changes from .NET 6
1. **TargetFramework**: Updated from `net6.0` → `net10.0`
2. **EF Core**: Updated from `6.0.0` → `10.0.0`
3. **JWT Bearer**: Updated from `6.0.0` → `10.0.0`
4. **OpenAPI**: Replaced `Swashbuckle.AspNetCore` with ASP.NET Core 10 built-in OpenAPI (`AddOpenApi()` / `MapOpenApi()`)
5. **AutoMapper**: Updated to `14.0.0`
6. **Logging**: Updated `Microsoft.Extensions.Logging.Console` to `10.0.0`

## Testing Version Upgrades

This application includes various .NET 10 specific features and patterns:

1. **Framework Version**: `<TargetFramework>net10.0</TargetFramework>`
2. **Package Versions**: EF Core 10.x, Authentication packages at 10.x
3. **Hosting Model**: Minimal hosting model works with .NET 10
4. **Authentication**: JWT authentication continues to work
5. **Entity Framework**: EF Core 10 with in-memory and SQL Server providers
6. **OpenAPI**: Built-in ASP.NET Core 10 OpenAPI (no Swashbuckle dependency)
7. **Logging**: Structured logging configuration
8. **Configuration**: Configuration binding works correctly

## Notes

- The application uses **in-memory database** for simplicity in testing
- **JWT secrets** are configured for development (should be externalized in production)
- **CORS** is configured to allow all origins for testing purposes
- **OpenAPI** endpoint is enabled in development environment via `MapOpenApi()`

This sample provides a realistic scenario for testing automated upgrade tools while being simple enough to understand and debug any issues that arise during the upgrade process.
