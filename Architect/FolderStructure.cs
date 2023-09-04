# FolderStructure
// Controller
// |-Base
// |--Generic Controller
// |-MyController

// Domain
// |-Entities: 
// |--BaseEntity.cs
// |--MyDomainModel.cs
// |-SeedWork
// |--ISeedService.cs
// |--IUnitOfWork.cs
// |-Services
// |--Base
// |---GenericService.cs
// |--MyService.cs

// Infrastructure
// |-EntityRepositories
// |--Base
// |---GenericRepository.cs
// |---MyRepository.cs
// |-IEntityRepositories
// |--Base
// |---GenericRepository.cs
// |---MyRepository.cs
// |-Mapper
// |--MappingProfile.cs
// |-ModelConfiguration
// |--MyConfiguration.cs

// ViewModel
// |-BaseEntity.cs
// |-ResultApi.cs
// |-MyEntity
// |--Requests
// |---MyEntityRequest.cs
// |--Responses
// |---MyEntityRequest.cs

// Middleware
// |-ApiKeyMiddleware.cs

// Enums
// |-MyEnums.cs

// Extensions
// |-MyExtension.cs

// Connected Services
// |-MyService
// |--ConnectedService.json
// |--Reference.cs