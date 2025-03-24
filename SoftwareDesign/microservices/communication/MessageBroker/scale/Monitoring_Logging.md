
# Enable RabbitMQ Management Plugin
Access RabbitMQ Dashboard for monitoring queues, exchanges, and consumers.

```bash
$ docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:management
```
Access http://localhost:15672 (guest/guest)

# Enable Logging
Logs provide insights into message flow and errors.

```
rabbitmqctl set_log_level debug
```
