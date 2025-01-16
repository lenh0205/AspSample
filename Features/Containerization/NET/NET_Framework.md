
# Containerizing .NET Framework apps
* -> we'll need **Docker Desktop** installed running in **`Windows container mode`**
* -> if we're running a Mac or Linux we're going to have to move over to use **Windows**
* => as it's a hard requirement to be able to make **`Windows-based container images`**, which is **what .NET Framework requires**

```cs
// assume your solution is structured with Dockerfiles sitting next to your .sln file and each project is its own subfolder
```

# What to put into Dockerfile to build .NET Framework webapp

## Overall steps
* -> start with the necessary base image with all tooling needed to compile .NET Framework app
* -> copy in our app bits into this compilation container
* -> invoke the necessary compilation commands
* -> create a new image that only has .NET Framework runtime & IIS
* -> move compilation bits into the IIS root folder

* -> this require us beyond having **`.NET Framework`** installed, we generally need **`Visual studio build tools`**, **`Nuget cli`**, **`.NET Framework targeting packs`**, **`ASP.NET Web targets`**
* => but today we're in luck because Microsoft provides **`a base .NET Framework SDK image`** that is packed with all the stuff we need to do all that
* => with that hassle out of the way, **a typical Dockerfile to containerize ASP.NET Framework app** may looks like this:
```Dockerfile
# -> we will place Myproject.dockerfile next to the solution

# start with a base image with all the necessary tooling to compile our app
# the 'mcr.microsoft.com/dotnet/framework/aspnet' image is already configured to startup IIS on port 80 when launched
FROM mcr.microsoft.com/dotnet/framework/sdk:4.8 AS build

# set the working directory inside compilation container to c:\app
WORKDIR /app
 
# copy everything from solution dir into the c:\app
COPY . .
# restore nuget packages
RUN nuget restore
# use msbuild to publish project in /FramworkApp folder to c:\publish, which includes only binaries and content files (no sources)
RUN msbuild "FrameworkApp\FrameworkApp.csproj" /p:DeployOnBuild=true /p:PublishUrl="c:\publish" /p:WebPublishMethod=FileSystem /p:DeployDefaultTarget=WebPublish
 
# start with new base image for running asp.net apps (which contains IIS)
FROM mcr.microsoft.com/dotnet/framework/aspnet:4.8 AS runtime
# set default work folder to c:\inetpub\wwwroot (IIS root)
WORKDIR /inetpub/wwwroot
# copy files from bin/publish in our sdk container into c:\inetpub\wwwroot
COPY --from=build /publish. ./
```

## Compiling image
* -> open up a command prompt and change the current folder to where your dockerfile is

* -> execute command: 
```bash
docker build -f Myproject.dockerfile -t myproject .
# "-f" flag - pointing to our dockerfile
# "-t" flag - is the tag (aka name) of our image (image names have naming rules: stick to lowercase and dashes only)
```

* -> after runing this command we should have a working image in our local repository. 

* -> run container from image:
```bash
docker run -it --rm -p 8080:80 myproject
# "-it" flag - starts the container in interactive mode, so all output will be sent to the current console. 
# -> without this argument, containers are started in the background and we need to use docker stop command to terminate it

# "--rm" flag - remove the container after execution. 
# -> Containers by default are left around after they exit (so anything that happened inside the container is preserved)
# -> for our scenario, we don't need that and that just eats up disk space. 
# -> Note this is different from image, which will still be preserved (think about that like class vs object instance)

# the "-p 8080:80" param – map port 8080 on the host to port 80 inside container
# -> we'll often have issues mapping port 80 on the host, so I recommend picking something else

# the final argument - is the name of the container image we want to start with.

# Assuming you see no errors in the console, you should be able to hit the app now by pointing your browser to http://localhost:8080
```

# Sharing your image with others
* -> Currently, your image sits in your local docker registry on your computer. If we want to share it with others we need to upload it to a container registry. The most common one used is Dockerhub. Head over to https://hub.docker.com and sign up for an account if you don’t already have one. After, come back to the command prompt and execute the following:

docker login -u <USERNAME>
Notice we didn’t specify the server. If no server is specified it just defaults to docker hub. Speaking of defaults, it’s worth discussing how docker image naming works. When we minted our image we gave it a name such as myproject. In the docker world, image names carry the URL of the container registry they belong to. So when we used an image like mcr.microsoft.com/dotnet/framework/sdk, it is not just the name of the image, it’s a full URL of where to find it. A server that hosts docker images is called a container registry (ex. mcr.microsoft.com), while the image that contains URL where to find it is synonymous with the word repository in the context of docker. There’s a special case though for Dockerhub. Any image that has a naming convention like <owner>/<image> is assumed to be living on Dockerhub, where the owner is the username. ANY other image registry will include a prefix (like mcr.microsoft.com). There’s one other thing you need to know about image naming: tags. All images have a “tag”, and if one is not specified it implicitly defaults to a tag called latest. Tags are specified via a colon after image name, so in fact, the image we created earlier is actually called myproject:latest. We can assign different tags to images, and they are most often used to differentiate between different versions or configurations of the same image.

Now that we got that out of the way, let’s get our image over to Dockerhub. First, we need to retag it to match our Dockerhub name. Execute the following:

docker tag myproject <MY-DOCKERHUB-USERNAME>/myproject
You’ve just created a new image. You now have two images in your local container registry:

myproject
<MY-DOCKERHUB-USERNAME>/myproject
As mentioned before, since images are just pointers to layers, they take up the same amount of disk space and are equivalent (until we change one or the other).

And now we can upload it to our remote registry by issuing a push command

docker push <MY-DOCKERHUB-USERNAME>/myproject
At this point, you should be able to find your image by heading over to https://hub.docker.com/r/<MY-DOCKERHUB-USERNAME>/myproject

Others can pull your image onto their computer just by calling

docker pull <MY-DOCKERHUB-USERNAME>/myproject
or pull and start with just one command like this

docker run -it --rm -p 8080:80 <MY-DOCKERHUB-USERNAME>/myproject




