
# Cost of Microservices system Deployment
* -> IIS Cost - using multiple IIS will cause significant cost, but if all services deployed in same Windows machine all app will fail if IIS has an issue
* -> Database Cost
* => solution: **Container** - the most popular is **`Docker`**

* -> when using Docker, there comes the question of how to scale out or scale down hence we need an orchestration engine 
* => **`Kubenetes`** is the most popular and open source container orchestration engine
* _when we start we dont actually need orchestration engine, only when we go into managing 50 or 100 microservices; we can go with static load, static number of containers like when using ECS service of AWS_