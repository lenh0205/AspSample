
# Publisher Confirms & Transactions (Ensuring Message Delivery)

## Publisher Confirms (Asynchronous ACKs)
Ensures a message is successfully received by the broker before proceeding.

More efficient than transactions.
```
channel.ConfirmSelect();
channel.BasicPublish("", "task_queue", null, body);

if (channel.WaitForConfirms())
    Console.WriteLine("Message confirmed");
else
    Console.WriteLine("Message lost!");
```

## Transactions (Stronger Guarantee but Slower)
Ensures messages are not lost but at the cost of performance.

```cs
channel.TxSelect();
try
{
    channel.BasicPublish("", "task_queue", null, body);
    channel.TxCommit(); // Commit if successful
}
catch
{
    channel.TxRollback(); // Rollback on failure
}
```