# InstallPackage:
/*
* Microsoft.EntityFrameworkCore
* Microsoft.EntityFrameworkCore.SqlServer
* Microsoft.EntityFrameworkCore.Design - for design-time cần thiết cho Migration, Reverse Engineering như "Add-Migration", "dotnet ef"
* Microsoft.EntityFrameworkCore.Tools - "Add-Migration" , "update-database"

* Newtonsoft.Json - "JsonProperty" cho "ResultApi", "JsonIgnore" cho Model; serialize, deserialize; parse, modify JSON,...

* Swashbuckle.AspNetCore.SwaggerGen
* Swashbuckle.AspNetCore.SwaggerUI
*/

# AddModel:
// Tạo Model -> Add vào DbContext -> tạo Migration
// Tạo Controller

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