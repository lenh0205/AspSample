
=========================================
> _`In-memory vs On-disk Databases`_

# In-memory Database
* **stores the data in memory** (_faster than disk_)
* **uses disk for backup** (_for the durability of data_)

## Question: Doesn't "backing up data on the disk" defeat the purpose of "using the memory to have faster operations"?
* If we **synchronously backup** the data, then operations must **`wait for the disk`**
* If we **asynchronous backup**, we might have **`data loss`**, when our system crashes after writing to the memory and returning the operation, and before backing up on disk. 

* -> how an in-memory database enjoys the higher speed of the RAM while guaranteeing the durability of our data ?

## Essence
* Can't avoid **`writing data to non-volatile memory`** to guarantee the durability of data
* => even an `in-memory database` must make sure that data has been written to some sort of `non-volatile memory` such as disk _before returning the operation to the client_

* note that, the **`latency overhead of an on-disk database`** does not come only from writing to the disk, rather it comes from maintaining a data structure such as B-tree on disk
* -> If we simply append data to a file, we are still faster than an on-disk database
* -> We can batch together multiple writes and quickly write a whole disk block. 
* -> not all disk accesses are the same. There is a huge difference between quickly write blocks one after another to a file with seeking different locations and updating data in them

## Solution to guarantee durability
* -> in-memory databases first simply **`append the data to an append-only log (inside disk)`**
* -> then **`write it to the memory`** and return the operation

* _Now, when the system recovers from a crash, it fills the memory with data that it has on the disk_

## Question: Doesn't this log become very long overtime? If we simply append data, instead of updating the existing data, our data will continue growing. What is the end game?
* Having a very long log has two big issues:
* -> one is its **`wastes storage`**
* -> two is it makes **`recovery time very long`** (_as the system has to read the whole log to rebuild the database on the memory_)

## Solution
* in-memory databases **maintain a disk data structure** such as **`B-tree`** and **`write the log entries periodically`** to the B-tree
* After applying log entries to the B-tree, we purge them from the log. This is called **`checkpointing`**
* Now, when the system restarts, we first **`load the checkpoint`** and **`write more recent log entries`** that are not included in the checkpoint to the memory

## Question: In an on-disk database also we have a WAL and a B-tree, and we cache the data in the RAM. Can't we conclude an in-memory database is nothing but a normal on-disk database with a big cache
* In an on-disk database, we still have **`serialization and data layout overhead`** compared with an in-memory database
* In an in-memory database, we assume the whole data is always in the RAM. We **`do not have such an assumption in an on-disk database`**
* -> This will change how we design things like data structures and indexing for our database

# On-disk Database
* an on-disk database stores the data on disk and uses memory for caching
