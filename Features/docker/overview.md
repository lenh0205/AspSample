https://www.youtube.com/watch?v=Ud7Npgi6x8E
https://www.youtube.com/watch?v=bg0QVTS4Q0c
https://www.youtube.com/watch?v=bg0QVTS4Q0c&t=21s

https://learn.microsoft.com/en-us/dotnet/architecture/microservices/net-core-net-framework-containers/
https://stakhov.pro/containerize-dotnet-framework/
https://stackoverflow.com/questions/65972093/docker-net-framework-application
https://stackoverflow.com/questions/72177307/is-there-is-a-way-to-dockerize-a-net-framework-application-on-linux
https://github.com/dotnet-architecture/eShopModernizing/wiki/02.-How-to-containerize-the-.NET-Framework-web-apps-with-Windows-Containers-and-Docker

============================================================================
# Docker
* -> packages software into containers that run reliably in any enviroment
* -> 3 fundamental elements: Dockerfile, Image, Container

## Problem
* -> how do we replicate the enviroment our software needs on any machine

* -> one way is to package an app is with a **`Virtual Machine`** 
* -> where the **`hardware is simulated`** (_using a host's hypervisor layer_) then installed with the **required OS** and **dependencies**
* -> this allows us to run multiple apps on the same infrastructure
* -> however because each VM is running its own operating system, they tend to be bulky and slow

* -> **`Docker container`** 
* -> is conceptually very similar to a **Virtual Machine** with one key different - instead of virtualizing hardware, containers only **`virtualize the OS`**
* -> emulate a minimal file system while piggybaking resources by sharing the host's kernel
* => in others words, all apps or containers are run by a single kernel 
* => makes almost everything faster and more efficent

## Kernel
* -> is the core of any Operating System - the brigde between **what the softeware asks for** and **what the hardware actually does**
* -> responsible for all sorts of critical low-level tasks like CPU and memory management, device I.O, file systems and process management 

============================================================================
# Dockerfile 
* -> just command tells Docker how to build an image - a napshot of our software along with all of its dependencies down to the OS level
* -> Docker will execute command in sequence and add each generated change to the final image as a new **file system layer** or a **metadata layer** 

```bash - Example: Dockerfile
# use "FROM" to start from an existing template 
FROM ubuntu:20.04

# use "RUN" to run a terminal command that installs dependencies into our image
RUN apt-get install sl

# do all kind of stuff, like set enviroment variables 
ENV PORT=8080

# finally, set a default command that's executed when we start up a container
CMD ["echo", "Docker is easy"]
```

```bash - create image file
# running docker build command
# -> go to each step of Dockerfile to build the image layer by layer
# -> "-t" is the name tag and provide "path to Dockerfile"
docker build -t myapp ./

# bring the image to life as a container
docker run myapp 
```

## Container Image
* -> is immutable and can be used to spin up multiple containers which is our actual software running in the real world
* -> all containers run from **`a base FileSystem`** and **`some metadata`**, presented to us as **a container image**
* -> the way container images work is formed with **overlapping layers**

* -> in the context of a FileSystem instead of changing data and its source, file changes are tracked by their differences to the previous layer and then composed together to achieve the final system state
* _it is similar to how Source Control track changes in our code_

* => there's loads of pre-made and officially supported base images out there that we can match to our project's core requirements and then add our own packages, code, configuration to

## Container Runtime
* -> we can run as many containers as we like from a single image 
* -> this is because when a container is first created, the image's file system is **extended with a new file system layer** completely dedicated to that container
* -> this means that we can make any runtime changes we like and it won't affect other containers using the same image
* -> this new layer will persist until we delete the container, so we can stop and start them as we like **`without losing any data`**

## Accessing running container
* -> it's just like we do with a **VM**

```cs - for example: a Linux container
// we can start a shell prompt when executing it, give us access to the enviroment
```

## Communication between Containers
* -> is really simple because most runtimes virtualize a Network layer for us

## Publish to production
* -> tag it with something unique, like a version then publish it to a **Container Registry**

## Deployment
* -> many **`Cloud Platform`** have **built-in support** for deploying containers as standalone unit
* -> alternatively, we can **`install compatible container runtime (like Docker)`** on the machine we want to use and pull image from the Container Registry

## Kubernetes
* -> allow us to **`create our own container-based cloud`**
* -> we describe the **desired state** of our deployment declaratively, and let Kubernetes handle the details of how to get there