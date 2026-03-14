CarService API
A RESTful backend service for managing car service operations, built with ASP.NET Core 8 following a clean N-Tier architecture.

Tech Stack
FrameworkASP.NET Core 8 (.NET 8)ORMEntity Framework Core 8 + SQL ServerAuthASP.NET Core Identity + JWT BearerImage UploadCloudinaryObject MappingAutoMapperAPI DocsSwagger / Swashbuckle

Project Structure
CarService-Microservice/
├── CarService.Entities/      # Domain models
├── CarService.Core/          # Interfaces & abstractions
├── CarService.DataAccess/    # EF Core DbContext & repositories
├── CarService.Business/      # Business logic & service implementations
└── CarServiceBG/             # API entry point (controllers, DI, middleware)
