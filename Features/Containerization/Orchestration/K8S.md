> **`Docker compose`** is a very nice option in **development enviroment**, but **`Kubernetes`** is the option in **production**
> sẽ có rất nhiều layer liên quan khi làm việc với kubernetes
> việc ta tương tác với Docker như trên thì oke khi development, nhưng với microservices architecture khi ta cần những việc như make sure containers to run and restart when crashing, scale them out, ... thì cần K8S
> **`Declarative Model`** - define a desired **end state**, let Kubernetes figure out how to get there

=============================================================================
# Kubernetes - K8S
* -> is a **`Container Orchestrator`**
* -> a tool for **managing and automating containerized workloads in the cloud** (_orchestrate, automate the deployment, management, scaling, networking of these containers_)
* -> **`orchestrates the infrastructure to handle changing workload`** - it can scale containers across mutiple machines and if one fails it know how to replace it with a new one 

## Infrastructure
* -> a **`Cluster`** - **a system deployed on Kubernetes**
* -> the **`Control Plane`** - the brain of the operation which exposes **an API server** that can handle both internal and external requests to **manage the cluster**; 
* -> the "control plane" also contains its own **key-value database** called **`ETCD`** used to store important information about running the cluster

* => what "Control Plane" managing is one or more worker machines called **`node`** - as a machine **running Kubelet** 
* -> **`Kubelet`** - a tiny **application** that runs on the machine to **communicate back with the main Control Plane** mother ship
* -> inside of each **Node** we have multiple **`Pod`** - which is the **smallest deployable unit** in "Kubernetes" (_a pod of containers running together_)

## Mechanism
* -> as workload increases Kubernetes can **automatically scale horizontally** by **`adding more 'node' to the 'cluster'`**
* -> in the process it takes care of complicated things like **networking**, **secret management**, **persistent storage**, ...
* -> it's designed for **`high availability`** - one way to achieves that is by **maintaining a replica set** which is just a set of running pods or containers ready to go at any given time

* -> as a developer, we define **`objects in yml`** that describe the **`desired state`** of our **Cluster**; 
* -> we can take this configuration and use it to **provision and scale containers automatically**; and ensure that they're always up, running and healthy

```yml - Example: defines a deployment object with 3 replicas
# -> we might have an nginx deployment that has a replica set with 3 pods

apiVersion: apps/v1
kind: Deployment
metadata: 
    name: nginx-deployment
spec:
    # pod template specification
    # run 1 container "nginx" and there will be 3 pods
    replicas: 3
    containers: 
    - name: nginx
      image: ngix:1.14.2
      ports: 
      - containerPort: 80
      volumes:
      - name: cool-volume
```
* -> then run
```bash
# create deployment object base on this configuration declaration
kubectl apply -f deployment.yaml
```

=============================================================================
# Prerequisite

## Cloud
* -> companies often choose to deploy Kubernetes in the **Cloud** as **a managed solution**
* -> the provider AWS, Azure, ... manages the Kubernetes Cluster on its own virtual machines or servers; and then from there it can tie into all the other services that Cloud provider offers like Identity and Access, Logging, Networking,...
* _Ex: **Amazon Elastic Kubernetes Service** (EKS)_

## 'YAML' in declarative configuration
* -> is a data serialization language for writing configuration files; is a superset of JSON and in fact every JSON file is also a valid yaml file
* -> **`resources in Kubernetes are created in a declarative way`** - we declare how we want things to be and Kubernetes will make sure that it meets that declaration
* -> that declaration is provided in a **yaml configuration file** called a **`manifest file`** - describes the **desired state** of Kubernetes object
* => Kubernetes will make sure at all times that it meets that declaration, **if we change a value it will change its infrastructre accordingly** 

## Networking (specifically Linux networking)
* -> in Kubernetes, there's communication between the pods and the containers within them
* -> require us to have knownleadge like **OSI layers**, **protocol**, **IPS**, **DNS**, **gateway**
* _Ex: there're port to Service Networking and there's Networking out to external destinations and then the services have different service types like ClusterIP, NodePort_

## Terminal
* -> to interact with our **Kubernetes cluster**, we need to use the **`kubectl`** - this is the main way of managing our Kubernetes Cluster from Terminal
* -> also the **Linux bash**, **vi** editor

