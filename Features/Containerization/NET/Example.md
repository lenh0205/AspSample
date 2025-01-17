> we will use "enviroment variable" while containerizing
 
# Docker with ASP.NET Core application and SQL Server
* -> create 2 containers 
* -> adding the **container orchestrator** support - use **docker compose** to deploy the application with all the docker images needed

## Step
* -> right-click vào .NET project -> Add -> chọn **Container Ochestrator Support** -> ta chọn **`Docker Compose`** và click "OK" -> Target OS là **Linux** và click "OK"
* => nó tạo sẽ tạo **`Dockerfile`** trong project của ta, đồng thời tạo thêm 1 **`docker-compose`** project trong solution của ta
* => đồng thời **pull all needed images** for **`creating the Debug Docker Container`** 
* _Visual Studio sẽ cho phép ta chạy application ở **Debug mode sử dụng Docker Compose**_

```Dockerfile - inside our project will look like this:
# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["LenhASP/LenhASP.csproj", "LenhASP/"]
RUN dotnet restore "./LenhASP/LenhASP.csproj"
COPY . .
WORKDIR "/src/LenhASP"
RUN dotnet build "./LenhASP.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LenhASP.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LenhASP.dll"]
```

* -> giờ ta sẽ mở file **`docker-compose.yml`** trong **docker-compose** project
```yml
services:
  # hiện tại solution của ta chỉ có 1 app project là "LenhASP"
  # Docker Compose cũng sẽ chỉ create only a single container  
  lenhasp:
    image: ${DOCKER_REGISTRY-}lenhasp
    build:
      context: .
      dockerfile: LenhASP/Dockerfile
```

* -> ta sẽ cần chỉnh sửa file **docker-compose.yml** để tạo cả **`SQL Server Container`**
```yml
networks:
  # tên network là tuỳ ý 
  # ta không cung cấp network drive info nên Docker sẽ dùng "bridge' driver
  demoblazorapp:


services:
  lenhasp:
    container_name: demo-blazor-app
    image: ${DOCKER_REGISTRY-}lenhasp
    build:
      context: .
      dockerfile: LenhASP/Dockerfile
    # provide port mapping to map port 8001 of the host machine with port 80 of the container
    # trong 'Dockerfile' của 'LenhASP' ta thấy nó đang được expose ở port 80
    # so that we can access the application using 8001 from the host machine
    ports:
      - 8001:80
    # assign some enviroment variables for SQL Server Docker Container
    depends_on: 
      # so that Docker will start the database container before this application container
      - demoappdb
    # provide enviroment variables for our application container
    enviroment:
      # to generate Connection String based on the enviroment variables 
      - DB_HOST=demoappdb
      - DB_NAME=DemoBlazorApp
      - DB_SA_PASSWORD=password@12345#
    networks:
      - demoblazorapp
  
  # for SQL Server
  demoappdb:
    container_name: app-db
    # the official image for MS SQL Server on Linux for Docker Engine
    image: mcr.microsoft.com/mssql/server:2019-latest
    # the port mapping optional if we need to access the database from the host machine
    # "1443" is default port number for SQL Server
    port:
      - 8002:1433
    enviroment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=password@12345#
    networks:
      - demoblazorapp

# Network in Docker
# we can place multiple containers in the same Network if they need to communicate each other
```

* -> now inside our application, we need to read the values from the enviroment variables and use them in Connection String
```cs - Program.cs
var dbHost = Enviroment.GetEnviromentVariable("DB_HOST");
var dbName = Enviroment.GetEnviromentVariable("DB_NAME");
var dbPassword = Enviroment.GetEnviromentVariable("DB_SA_PASSWORD");
var connectionString = $"Data Source={dbHost};Initial Catalog={dbName};User ID=sa;Password={dbPassword}";
builder.Services.AddDbContext<DatabaseContext>(opt => opt.UseSqlServer(connectionString));
```

* -> giờ ta sẽ sử dụng **Docker Compose** để chạy application; Visual Studio sẽ tạo các Containers theo như "docker-compose.yml" file của ta rồi chạy nó
* -> nó sẽ mở application của ta trên Browser tại "http://localhost:8001" (thay vì chạy "http://localhost:5211" như cũ)
* -> nếu ta vào **Docker Desktop**, ta cũng sẽ thấy **Containers** và **images** được tạo bởi Visual Studio for debugging

* -> giờ ta sẽ publish this application to **`DockerHub`**, so that we can use the Docker image directly from the DockerHub while  
* -> ta right-click vào project -> chọn **Publish** -> chọn **Docker Container Registry** -> next, **Docker Hub** -> next, cung cấp **credential** của ta để login Docker Hub -> Finish -> Publish
* -> nó sẽ build our application and push the image to Docker Hub repository

* -> để deploy entire application in a new machine, **`tất cả những gì ta cần là trên new machine có Docker, Docker Compose và file "docker-compose.yml" (mà ta sẽ tạo bên dưới đây)`**
* _ta sẽ không cần cài .NET runtime or SQL Server, other dependencies, libraries_
* (_nếu ta đã install Docker Desktop thì nó đã có sẵn Docker Compose, còn không thì ta có thể cài nó tại docs.docker.com/compose/install_)
* -> ta sẽ tạo 1 file **docker-compose.yml** như trên nhưng chỉnh sửa 1 chút rồi save nó vào 1 thư mục nào đó
```yml
networks:
  demoblazorapp:

services:
  lenhasp:
    container_name: demo-blazor-app

    # we will be replace with the name of the image in Docker Hub (we just publish)
    image: condingdroplets/demoblazorserverapp 

    # also skip the "build" section as Docker image in DockerHub is already compiled and ready to use image 
    build:
      context: .
      dockerfile: LenhASP/Dockerfile
    ports:
      - 8001:80
    depends_on: 
      - demoappdb
    enviroment:
      - DB_HOST=demoappdb
      - DB_NAME=DemoBlazorApp
      - DB_SA_PASSWORD=password@12345#
    networks:
      - demoblazorapp
  
  demoappdb:
    container_name: app-db
    image: mcr.microsoft.com/mssql/server:2019-latest
    port:
      - 8002:1433
    enviroment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=password@12345#
    networks:
      - demoblazorapp
```

* -> ta run the docker compose command 
```bash
docker ps -a 

docker images

# "up" - to pull the needed images and start the container
# -> ta sẽ thấy nó pull 2 images là application image và Sql Server image
# -> sau đó Docker Compose sẽ create the Network và Containers; rồi chạy Containers
docker-compose up -d

docker ps

docker images
```

* -> h ta thử chạy app lại trên Browser "http://localhost:8001"

* -> we can use the **Docker Compose** to stop, remove containers and related networks
```bash
# this will remove the containers and the network (but images will not got deleted)
docker-compose down
```

# Docker Networking
* -> while creating a docker network, we can specify the **Network Driver** we need to use 

## Available Network Drivers in Docker

### 'Bridge' network driver
* -> this is the **`default network driver`** (_if we don't specify the driver, this is the type of network you are creating_)
* -> **Bridge** network are usually used when our application run in standalone containers that need to communicate 

### 'Host' network driver
* -> if we use the host network driver for a container, that **containers network stack** is **`not isolated from the Docker host`** - the container shares the host's network namespace
* -> and the **`container does not get its own IP-address allocated`**

### 'Overlay' network driver
* -> this driver creates **`a distributed network among multiple Docker hosts`**

### 'IPVlan' network driver
* -> this driver gives users total control over both IPv4 and IPv6 addressing 

## 'Macvlan' network driver
* -> allow us to assign a MAC address to a container, making it appear as a physical device on our network
* -> the Docker will route traffic to containers by their MAC addresses

## None
* -> used if we want to completely disable the networking stack on a container

## Apply
* -> our solution will 1 Web Application and 1 Web API communicate with each other; and the Web API is the one access directly to the Database
* -> so for this solution, we need to create 2 networks: Web App and Web API, Web API and Database
* -> ensure that the "Web App" docker container can not even access the "Database" docker container because they don't have any network connectivity  
