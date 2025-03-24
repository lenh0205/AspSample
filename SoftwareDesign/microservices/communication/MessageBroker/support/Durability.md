
# Persistent vs. Non-Persistent Messages
* **Persistent Messages** → By default, messages are transient (lost if RabbitMQ restarts). You can make them persistent by setting the delivery_mode = 2.

* **Durable Queues** → Ensure queues survive server restarts.

* **Confirmations** → Use Publisher Confirms to ensure messages are successfully published.