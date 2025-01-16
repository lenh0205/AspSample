https://www.youtube.com/watch?v=bg0QVTS4Q0c

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

## Solution
* -> one way is to package an app is with a **Virtual Machine**
* -> another is **Container**

* -> both **VM files** and **Container files** are **`portable`** - we can move them easily to different machines
* -> but **container are a lot smaller so it's much more portable**

### Virtual Machine
* -> where the **`hardware is simulated`**
* -> then we just need to install with the **required OS** and **dependencies**
* -> this allows us to run multiple apps on the same infrastructure
* -> however because each VM is running its own operating system, they tend to be bulky and slow

* -> về kiến trúc, ta sẽ start off with **`hardware`** (such as server)
* -> then on top of the **hardware** there is software called **`Hypervisor`** - this is what allows one machine to run multiple virtual machines **by allocating and controling the sharing of a machine's hardware** 
* -> and on top of the **Hypervisor** are the **`Virtual Machines with each own Operating System`** 
* -> and on top of the **Operating System** is the running application 

### Container
* -> **`Docker container`** 
* -> is conceptually very similar to a **Virtual Machine** with one key different - instead of virtualizing hardware, containers only **`virtualize the OS`**
* -> emulate a minimal file system while piggybaking resources by sharing the host's kernel
* => in others words, all apps or containers are run by a single kernel 
* => makes almost everything faster and more efficent

* -> về kiến trúc, ta sẽ start off with **`hardware`** (such as server)
* -> and then on top of the **hardware** is the **`Operation System`**
* -> then on top of the **Operating System** is the **`Container Engine`** - what unpacks the containers files and hands them off to the operating system kernel
* -> all the **`Containers`** is control by **Container Engine** and share the **underlying Operating System** on the Server

## IMPORTANT NOTE
* -> **`Containers (or image) must be packaged to work with the same Operating System of server that it's is running on`**
* _tức là nếu OS của Server mà ta sẽ chạy là Linux, vậy thì container file must be Linux-based_
* _nó khác với Virtual Machine, vì mỗi VM có thể chạy bất cứ OS nào nó muốn_

* -> since all the containers share the underlying OS between them that means **if the OS on Server crashes then all the containers will go down** 

============================================================================
# Relative concepts

## Kernel
* -> is the core of any Operating System - the brigde between **what the softeware asks for** and **what the hardware actually does**
* -> responsible for all sorts of critical low-level tasks like CPU and memory management, device I.O, file systems and process management 

## Virtualization
* -> we start off with a **host machine** (_could be a Local PC, a server up in Cloud, server in a data center_) - it's **`a piece of hardware`** 
* -> in this piece of hardware, we have different **things that control how this hardware work** - CPU, memory, IO
* -> what happen in virtualization is we **`take little pieces of these pieces of hardware`** and **`separate them out`** into a separate machine - a **`Virtual Machine`**

* -> in the virtual machine, we actually run a full entire **Operating System**
* -> the **`Hypervisor`** - a special program run and manage the life cycle of these machines (_start, stops, creates, deletes them, provisions resources for them_)
* -> the common Hypervisor we usually use is **VMware**, **Virtual Box**

## Containerization
* -> is ability to create a lightweight enviroment where processes can run on a host OS, they share all the same things in that OS but can't touch anything outside their bound
* -> Docker is a program that **manages the life cycles of containers**

============================================================================
# Container

# Dockerfile 
* -> just command tells Docker how to build an image - a napshot of our software along with all of its dependencies down to the OS level
* -> Docker will execute command in sequence and add each generated change to the final image as a new **file system layer** or a **metadata layer** 

```bash - Example: Dockerfile
# use "FROM" to start from an existing template 
FROM ubuntu:20.04

# use "RUN" to tells Docker that we want to run something on the image we just add above
# -> this run a terminal command that installs dependencies into our image
RUN apt-get install sl

# do all kind of stuff, like set enviroment variables 
ENV PORT=8080

# finally, set a default command that's executed when we start up a container
CMD ["echo", "Docker is easy"]
```

```bash - create image file
# running docker build command
# -> go to each step of Dockerfile to build the image layer by layer
# -> "-t" is the name tag: image name + ":" + tên tag tuỳ ý (by default is "latest")
# -> "./" is for providing "path to Dockerfile" (để trống cũng được)
docker build -t myapp ./

# check what images we have currently built on our system 
docker images

# bring the image to life as a container, các step nó sẽ làm bao gồm:
# -> find image "myapp:latest" ("myapp" image with "latest" tag) locally
# -> không tìm thấy thì Docker client sẽ contact với Docker daemon
# -> Docker daemon pulled the "myapp" image from the Docker Hub
# -> Docker daemon created a new container from that image 
docker run myapp 
```

## Container Image
* -> is immutable (muốn update thì cần update lại Dockerfile sau đó build lại image)
* -> can be used to spin up multiple containers which is our actual software running in the real world
* -> all containers run from **`a base FileSystem`** and **`some metadata`**, presented to us as **a container image**
* -> the way container images work is formed with **overlapping layers**

* -> in the context of a FileSystem instead of changing data and its source, file changes are tracked by their differences to the previous layer and then composed together to achieve the final system state
* _it is similar to how Source Control track changes in our code_

* _there's loads of pre-made and officially supported base images out there that we can match to our project's core requirements and then add our own packages, code, configuration to_
* _build image lần đầu có thể lâu, nhưng lần sau sẽ rất nhanh vì những package ta đã kéo về lần sau sẽ không cần kéo về nữa (và các images này vẫn tồn tại song song trên máy của ta)_
* _các images build từ cùng 1 Dockerfile này phân biệt nhau bởi **`-t`** flag - a snapshot of our Dockerfile that was built into an image_

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