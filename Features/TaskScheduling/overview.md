
# 'Task Scheduling' vs 'Background Jobs'

## Task Scheduling
* -> is about **`running tasks at specific intervals or times`**
* => typically used for **`recurring tasks`**

```cs
// -> Examples:
// sending daily reports
// database cleanup
// refreshing cache
```

## Technologies:
* -> **`IHostedService`** or **`BackgroundService`** for long-running tasks
* -> **`Quartz.NET`** for advanced scheduling
* -> **`Hangfire's recurring jobs`**

## Background Jobs
* -> **`run outside the main request-response cycle`**, meaning they do not block the user
* => can be one-time or recurring, but are generally used for **`processing tasks asynchronously`**

```cs
// -> Examples:
// Sending an email after a user submits a form
// Processing large file uploads
// Running machine learning inference
```

## Technologies:
* -> **`Hangfire`** (BackgroundJob.Enqueue, RecurringJob.AddOrUpdate)
* -> **`Azure Functions`** for cloud-based jobs
* -> **`Worker Services`** (hosted background services in .NET)
