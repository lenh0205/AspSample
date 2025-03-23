
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
// ....


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

### Stage 1: Build Environment

# First, specify the base 'images' we want to pull down from "Docker Hub" that we'll use to start our build
# pull down .NET 5 SDK image, which contains the necessary tools for building .NET applications 
# the "AS build-env" part names this stage as "build-env", allowing it to be referenced later
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env

# create a working directory inside the container
WORKDIR /app

# copy file from our local system into the image
# -> copies all .csproj files from the host machine to the "/app" directory in the container
# -> this step ensures that project dependencies can be restored without copying all source files, which speeds up builds when dependencies have not changed
COPY *.csproj ./

# restores the NuGet packages required for the project by using the .csproj file
RUN dotnet restore

# copies all the source files from the host machine to the /app directory in the container 
COPY . ./

# builds and publishes the application in Release configuration.
# outputs the compiled application to the "out" directory
RUN dotnet publish -c Release -o out

### End Stage 1


### Stage 2: Runtime Environment

# specifies the base image for the runtime environment
# this image contain only the ASP.NET runtime, optimized for running applications
FROM mcr.microsoft.com/dotnet/aspnet:5.0

# sets the working directory inside the container to /app
WORKDIR /app

# copies the output files from the out directory of the "build-env" stage to the "/app" directory in this stage 
COPY --from=build-env /app/out .

# set the entry point for out 'image', so when we run our image that's what gets kicked off 
# -> specifies the default command to run when the container starts (can be overwritten by CLI)
# -> run .NET application by executing the PlatformService.dll
ENTRYPOINT [ "dotnet", "PlatformService.dll" ]

### End Stage 2
```

# Build image
* -> ta nên dùng **Docker** của VSCode để theo dõi những image trên máy ta và những image đang run

```bash
# check if Docker running trên máy
docker --version

# build image 
# -> basically it will run through our "Dockerfile" and start executing all the scripts
# -> if the images that it need not already exist on our machine or out of date, it will pull down from DockerHub
docker build -t <our_dockerhub_id>/<name_of_our_service> .
```

# Run the image as a container
```bash
# "-p" flag - the "port mapping" is required to access the server running inside the container
# "-d" flag - detached mode to avoid the container output displays in this CLI và ta sẽ không thể gõ trên terminal này được nữa
docker run -p 8080:80 -d <our_dockerhub_id>/<name_of_our_service>

# show running containers bao gồm thông tin: container_id, image, command_names, ....
docker ps

# stop container
docker stop <container_id>

# mỗi lần ta dùng "docker run" nó sẽ run 1 container hoàn toàn mới, vậy nên để chạy lại container ta đã stop ta cần:
docker start <container_id_to_restart>
```

# using port mapping to get to running container
* giờ thì thay vì truy cập trực tiếp http://localhost:5000/api/platforms/ bằng cách dotnet run trực tiếp project trên mấy của ta
* ta có thể truy cập vào dockerized image thông qua http://localhost:8080/api/platforms/

# Push image to DockerHub
```bash
docker push <our_dockerhub_id>/<name_of_our_service>
```

============================================================================

# Kubernetes