
## RabbitMQ Clustering
Connect multiple RabbitMQ nodes together to increase availability.

Messages are shared only in mirrored queues (not all queues by default).

## Mirrored Queues (HA Queues)
Queues are replicated across multiple nodes for high availability.

If a node fails, messages are still available.

```bash
rabbitmqctl set_policy ha-all "^ha\." '{"ha-mode":"all"}'
```