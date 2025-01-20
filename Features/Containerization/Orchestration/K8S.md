> **`Docker compose`** is a very nice option in **development enviroment**, but **`Kubernetes`** is the option in **production**
> sẽ có rất nhiều layer liên quan khi làm việc với kubernetes
> việc ta tương tác với Docker như trên thì oke khi development, nhưng với microservices architecture khi ta cần những việc như make sure containers to run and restart when crashing, scale them out, ... thì cần K8S
> **`Declarative Model`** - define a desired **end state**, let Kubernetes figure out how to get there

# Kubernetes
* -> often referred to as **`K8S`** - is a **`Container Orchestrator`**

* -> a tool for **managing and automating containerized workloads in the cloud** 
* -> orchestrates the infrastructure to handle changing workload - it can scale containers across mutiple machines and if one fails it know how to replace it with a new one 

* -> a **`Cluster`** - **a system deployed on Kubernetes**
* -> the **`Control Plane`** - the brain of the operation which exposes **an API server** that can handle both internal and external requests to **manage the cluster**; 
* -> the "control plane" also contains its own **key-value database** called **`ETCD`** used to store important information about running the cluster

* => what "Control Plane" managing is one or more worker machines called **`node`** - as a machine **running Kubelet** 
* -> **`Kubelet`** - a tiny **application** that runs on the machine to **communicate back with the main Control Plane** mother ship
* -> inside of each **Node** we have multiple **`Pod`** - which is the **smallest deployable unit** in "Kubernetes" (_a pod of containers running together_)

* -> as workload increases Kubernetes can **automatically scale horizontally** by **`adding more 'node' to the 'cluster'`**
* -> in the process it takes care of complicated things like **networking**, **secret management**, **persistent storage**, ...
* -> it's designed for **`high availability`** - one way to achieves that is by **maintaining a replica set** which is just a set of running pods or containers ready to go at any given time

* -> as a developer, we define **`objects in yml`** that describe the **`desired state`** of our **Cluster**; 
* -> we can take this configuration and use it to **provision and scale containers automatically**; and ensure that they're always up, running and healthy

```yml - Example:
# Ex: we might have an nginx deployment that has a replica set with 3 pods
apiVersion: apps/v1
kind: Deployment
metadata: 
    name: nginx-deployment
spec:
    replicas: 3
    containers: 
    - name: nginx
      image: ngix:1.14.2
      ports: 
      - containerPort: 80
      volumes:
      - name: cool-volume
```