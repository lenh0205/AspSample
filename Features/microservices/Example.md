
# Design
* microservice 1 của ta sẽ có 
* -> 1 **`REST API Controller`** (synchronous - in) cho external contract, nhận HTTP Request và trả về HTTP Response, kết nối với lớp **Repository**
* => đây sẽ là external interface duy nhất của ta còn lại bên dưới chỉ dùng cho microservices domain
* -> 1 **`HTTP Client`** (synchronous - out) để make HTTP request tới microservice khác và nhận về HTTP Response
* -> 1 **`Message Publisher`** (asynchronous - out) để publish event onto out **`Message Bus`**
* -> 1 **`gRPC Service`** (synchronous - in) để cho những client khác có thể sử dụng gRPC để request tới microservice của ta, kết nối với lớp **Repository**

* microservice 2 của ta sẽ có
* -> 1 **`REST API Controller`** (synchronous - in) (the only external API)
* -> 1 **`Message Subscriber`** (asynchronous - in) để receive messages from the **Message Bus**
* -> 1 **`gRPC Client`** (synchronous - out) để make requests to **gRPC Service** của microservice khác 

```cs - PlatformsController.cs
// as REST best practice, whenever we create a resource we should return a HTTP 201 along with the resource we created and also a URI to the resource location
// ta không nên trả thẳng model ta đã truyền vô để tạo resource

[HttpPost]
public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformCreateDto)
{
    var platformModel = _mapper.Map<Platform>(platformCreateDto);
    _repository.CreatePlatform(platformModel);
    _repository.SaveChanges();

    var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);

    return CreatedAtRoute(nameof(GetPlatformById), new { Id = platformReadDto.Id }, platformReadDto);
}
```

============================================================================
> the different between running **old server, PC, OS, application** vs **Virtual Machine** (VMware, VirtualBox) vs **Containers**

## Docker
* -> **`containerization`** -  package our application into **images** and run them as **containers** on any platform that can run Docker
* -> **`Dockerfile`** - a set of instructions that tells Docker how to take our application and turn it into an **image** by using **Docker Engine**
* _i **`image`** - the thing we can distribute somewhere into anything that's running Docker and allow to run it as a **container**_

* -> đầu tiên ta sẽ cần tạo 1 **Dockerfile** tại root of project 
* -> sau đó ta sẽ build và create an **image**
* -> rồi run our **image** as a **container**
* -> stop running container
* -> finally, push our **image** to **Docker Hub**

```Dockerfile
# search "Dockerize an ASP.NET Core application

# First, specify the 'images' we want to pull down from "Docker Hub" that we'll use to start our build
# pull down .NET SDK image to build the main part of our application 
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env

# create a working directory
WORKDIR /app

# copy ".csproj" file, so that we know what dependencies we actually need to work with
COPY *.csproj ./
# pull down packages contained in ".csproj"
RUN dotnet restore

# copy everything else and build it 
COPY . ./

# publish
RUN dotnet publish -c Release -o out

# build a runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0

# create working directory
WORKDIR /app

# copy the content of what we built over 
COPY --from=build-env /app/out .

# set the entry point for out 'image', so when we run our image that's what gets kicked off 
ENTRYPOINT [ "dotnet", "PlatformService.dll" ]
```