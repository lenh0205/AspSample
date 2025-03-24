
# Dead Letter Exchange (DLX)
Messages that fail to process (due to errors or TTL expiry) can be moved to a Dead Letter Exchange (DLX) for debugging or retries.


## Time-To-Live (TTL) for Messages
Messages expire after a specific time and are either discarded or moved to a Dead Letter Queue (DLQ)

var args = new Dictionary<string, object>
{
    { "x-message-ttl", 30000 } // 30 seconds
};
channel.QueueDeclare("task_queue", durable: true, exclusive: false, autoDelete: false, arguments: args);

## Dead Letter Exchange (DLX)
If a message expires or is rejected, it can be redirected to another queue.

Helps in monitoring and retrying failed messages.

var args = new Dictionary<string, object>
{
    { "x-dead-letter-exchange", "dlx_exchange" },
    { "x-dead-letter-routing-key", "dlx_key" }
};
channel.QueueDeclare("task_queue", durable: true, exclusive: false, autoDelete: false, arguments: args);
