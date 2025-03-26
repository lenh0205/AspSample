==========================================================================================
> nếu là Window thì có thể sử dụng **`Windows Task Scheduler`** với **PowerShell script** để thay thế

# Cron job
* -> a cron job is an **`OS-level scheduled task`** that runs at **`specific intervals`** using **cron expressions** (_e.g., 0 0 * * * for midnight_)
* ->  **cron daemon (crond)** is a background process that runs on **Linux/macOS** and checks the **crontab*** (cron table) to schedule and execute **cron jobs**

## Example
* -> open and edit our **`crontab file`** (_each user in a Linux/macOS system has their own crontab (cron table) file where they can define scheduled tasks_)
```bash
$ crontab -e # opens the crontab file in default text editor (e.g., vim, nano).
```

* -> add **Cron Jobs**
``` bash
0 3 * * * /usr/bin/dotnet /path-to-app/MyApp.dll
# -> Runs at 3:00 AM daily
# -> Path to the .NET runtime
# -> Path to our .NET application

# a cron job to call an API every hour
$ 0 * * * * curl -X POST http://localhost:5149/api/cleanup

# a cron job to run a Shell Script Every Monday at 5 AM
$ 0 5 * * 1 /home/user/myscript.sh
```

* -> save and exit (_if using vim, press ESC, then type :wq and hit ENTER_) and confirm that cron job was added
```bash
$ crontab -l # lists all scheduled cron jobs for the current user
```

* -> might need to restart the cron service to apply changes
```bash
$ sudo systemctl restart cron
```

* -> check if cron job ran successfully by system logs:
```bash
$ grep CRON /var/log/syslog  # For Ubuntu/Debian
$ journalctl -u cron  # For systemd-based systems

# if our cron job is failing, add logging to capture errors:
$ 0 3 * * * /usr/bin/dotnet /path-to-app/MyApp.dll >> /home/user/cronlog.txt 2>&1
```

* -> remove all cron jobs
```bash
$ crontab -r
```
