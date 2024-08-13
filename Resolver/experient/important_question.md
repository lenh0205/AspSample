# 'in-memory cache' vs 'in-memory database'
* -> **`in-memory caches`** provide better performance because **writes are not persisted**, eliminating the extra time needed for persisting data.
* -> an **`in-memory database`** **persists writes, making data changes durable**

* => this durability comes at **`the expense of lower performance for writes`**
* => however, in-memory databases still **provide better performance than a disk-based database**
* => from a performance standpoint, they **sit between an in-memory cache and a disk-based database**

# what are the limitations of in-memory caches
* -> because all data is stored and managed exclusively in memory, in-memory caches risk **`losing data upon a process or server failure`**
* -> to improve durability, an in-memory cache **`may persist data periodically on disk databases`**
* -> there're some mechanisms to improve durability below.

## Snapshot files
* -> snapshot files record the database state at a given moment in time
* -> the in-memory cache generates snapshots periodically or during a controlled shutdown. While snapshotting improves durability to some extent, data loss may still occur between snapshots

## Transaction logging
* -> transaction logging records changes to the database in an external journal file
* -> logging is independent of data read/write and does not impact performance. The journal file facilitates the automatic recovery of an in-memory cache.

## Replication
* -> Some in-memory caches rely on redundancy to provide high availability. They maintain multiple copies of the same data in different memory modules. Module failure results in automatic failover to the duplicate backup copy. This mitigates the risk of data loss with a cache.
