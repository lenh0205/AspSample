
# where to place 'Dockerfile'
* -> recommendation is to go with option 2. It’s the most intuitive for people not familiar with your project structure

## Option 1: Place it next to '.csproj' of the project that is going to be containerized
* -> Pros: intuitive and easy to find

* -> Cons: if the project references any other projects needed for compilation (like ../CommonProject/common.csproj), the docker context will not have this dependency and our compilation will fail. We could however write the docker file with the assumption that the context will be something OTHER then where dockerfile lives. If we do this however, an extra argument needs to be passed to the docker build command to set the context, and whoever is building the image needs to know what the correct context is. If you do this, at least put a comment at top of your Dockerfile something like #compile with 'docker build -t mytag ../' to inform others on how to properly use the dockerfile

## Option 2: Place it at the solution level
* -> Pros: docker context is usually correct and will have all the necessary dependencies the projects in solution necessary to compile app

* -> Cons: if more then one project in solution need to be turned into docker image, the default Dockerfile filename is no longer an option, and you’ll have to pass it explicitly to the docker build command via -f <DOCKERFILE> argument

# What to put into Dockerfile to build .NET Framework webapp

## Abstract step
* -> start with the **necessary base image** with **`all tooling needed to compile .NET Framework app`**
* -> copy in our app bits into this compilation container
* -> invoke the necessary compilation commands
* -> create a new image that only has .NET Framework runtime & IIS
* -> move compilation bits into the IIS root folder