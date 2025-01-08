# what should learn about docker as an backend developer
https://www.youtube.com/watch?v=91Sp7dSlpzs
https://www.quora.com/As-a-web-developer-is-it-really-necessary-to-learn-Docker
https://dev.to/suzuki0430/docker-for-beginners-crafting-your-backend-development-environment-38oo
https://stackoverflow.com/beta/discussions/77475709/should-a-full-stack-developer-know-docker-as-mandatory
https://www.docker.com/blog/6-development-tool-features-backend-developers-need/

# Deployment

```bash - deploy an NodeJS app
// first, we must have a VM runing any Linux based OS
// install NodeJS
// put our file on the server and ask Nodejs to run our programs
// then NodeJS in conjunction with system files will create an enviroment and translate our program into machine code, hand it over to the Kernel
// Kernel can ask the bare mental to do its job
```

# Reason using Docker
* -> giải quyết vấn đề "it works on my machine"
* -> thuận tiện hơn cho việc deploy 1 full-fledged fullstack application (BE, FE, DB); ta chỉ cần package application along with the runtime into a single package
* -> Docker uses Linux kernel features to provide its functionality (khi ta cài Docker trên Window/MacOS thì thực chất nó sẽ cài 1 VM running Linux)

* -> by package all the system files required, our application and dependencies into a binary - that's **Docker image**, a distributable artifact which can be stored anywhere
* -> when we run this image on top of Docker it becomes a container

# 2 way of Docker installation
* -> install **`Docker CE`** (or **Docker CLI**) - chỉ run trên Linux
* -> cho **Window** and **MacOS**, ta sẽ cần install **`Docker Desktop`** (**VM** + **Docker CE** + **GUI**)

* => khi install Docker trên máy ta sẽ có:
* -> "Docker" is a **`Client server based application`** that reponsible for taking our command (_e.g, docker run, docker build, docker pull_)
* -> the client will ship those command to **`Docker Daemon`** - reponsible for managing resources on the machine it's installed (_downloading new images, spawn new containers, ..._)
* -> the images is download from **`Registry`** - **Docker Hub**, **Azure container registry**

# Docker
* -> **`Dockerfile`** is the blueprint for image
* _every image is starts with a base image, so base image is like the starting point for our image; we have to start from the **`Operation System`** or some intermediate image_

```bash - run "busybox" image (chứa các câu lệnh linux) on our machine
# -> vào Docker Hub -> tìm "busybox"

# open terminal, pull image to our local system by this command:
docker pull busybox

# list các images đang locate trong system
docker images

# run image
docker run busybox

# run image into an interactive mode (-it), then run a program of that image
docker run -it busybox /bin/sh
# this will drop us down to the Shell inside container, giờ thì ta có thể thực thi 1 số lệnh Linux như: ls, whoami

# -> ta có thể mở 1 terminal khác và list tất cả containers đang running
docker ps
``` 

```bash - create image that contain application 
# -> this app feature is to list down the content inside "data" directory that is mounted on that particular machine in the tabular format (-ltr)

# First, create Dockerfile
mkdir busybox-list
cd busybox-list
touch Dockerfile

# Editing Dockerfile
nvim Dockerfile

###
FROM busybox
# able to use the "ls" command that take from "busybox":
# the 'CMD' will execute the command when the image is run
CMD ["ls", "-ltr", "/data"]
```

=============================================================================
> https://stackoverflow.com/questions/16047306/how-is-docker-different-from-a-virtual-machine
> https://aws.amazon.com/compare/the-difference-between-docker-vm/
> https://www.reddit.com/r/docker/comments/q6ykxa/when_should_you_choose_vms_over_docker/?rdt=63673
> https://tel4vn.edu.vn/blog/so-sanh-su-khac-nhau-giua-may-ao-va-docker-container/
> https://www.freecodecamp.org/news/docker-vs-vm-key-differences-you-should-know/
> https://www.geeksforgeeks.org/difference-between-docker-and-virtualization/

# Docker vs Virtual Machine
* -> technologies used in **`application deployment`**