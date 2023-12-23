
# Overview
* Whenever a database uses data, that data must have existed in memory. 
* Accessing this data is relatively fast, but once the data becomes very large, it becomes impossible to fit all of it within memory. 
* -> Disks are used to cheaply store all of a database’s data, but they incur a large cost whenever data is accessed or new data is written

# Disk
* The basic API for disks includes READ and WRITE 
* READ - stands for transferring "pages" of data from disk to RAM 
* WRITE - transferring "pages" of data from RAM to disk respectively. 
* Note: both API calls are very slow due to the structure of magnetic disks

=================================================
> _`Về phương diện kỹ thuật`_
> Memory and Disk(storage) are physical components of a computer that store data

# Disk (Storage)
* Disks are not only known for holding permanent data (unless manually removed), but also for being able to store a lot of it

* **Disk storage is "serial"** — you _start at the beginning of the disk_ and _read the data in sequence_
* -> has to pay the cost to `start spinning` or `keep spinning` at all times
* -> `compared to RAM`, disk drives consume more energy

* **HDDs** and **SSDs** 
* -> HDDs are less expensive; but SSD is better in `latency, data-transfer times, and increased reliability`
* -> HDDs were the first to be widely used in computing, but SSD is gaining in popularity
* -> data must travel `via a bus`

## Hard Disk Drive - HHD
* HDDs **`store data by rotating the disk`** using magnetic material (vật liệu từ tính)
* ->  The **`platter`**, magnetized surface of the disk, contains tiny components that are either magnetized or not (1/0)
* -> A small magnet called the **`read-write head`** moves over the platter to store data

## Solid-state Drive - SSD
* SSDs are made of semiconductor chips and use it to store data without requiring a read-write head 

## "seeks" on hard disks are the most expensive operations
* There are two components to hard disk drive performance: **`access`** and **`data transfer times`**

## Access time (How long it takes to transfer data ?)
* Rotating disk drives are spinning with the **`head reading the data`**
* Access time relies on the time it takes to
* -> **`seek`**: travel to where the data needs to be read/written
* -> **`rotate`**: assembling the disk under the "head" appropriately
* -> **`process commands`**: organize the communication for reading/writing data
* -> **`settle`**: the "head" need to stabilize on the track so it doesn’t read/write incorrectly

## Data transfer rate
* The rate that data is transferred either to or from the disk (read or written)
* _This is the time it takes for data to physically go from point A to point B_

=====================================================
# Memory / RAM / in-memory / main memory
* faster than Disk because closer to the CPU and **`doesn’t need to be read serially`**
* but RAM is **`volatile`** (data is keeped for a certain amount of time or until power source is removed)

## RAM is "Random Access Memory"
* The data in RAM is accessed randomly, _means there is `no time difference to read data` in address A versus B_ 

## In-memory databases - IMDB
* primarily use the RAM instead of a disk drive (VD: **Redis**)

* Databases **`can scale vertically with RAM`**, 
* -> allowing for **`more computation to be done`**
* -> not necessarily faster computations because you are just adding more resources, not faster ones
* -> having more RAM in your laptop will make more applications able to run at once

## Dynamic RAM - DRAM 
* is structurally simple with **one transistor/capacitor per bit**
* -> DRAM **`needs to be refreshed constantly`**
* -> In a memory refresh, it **`rewrite data`** to the chip
* -> As time goes on, the chip starts to lose/leak the charge. each `rewrite` restores the capacitor’s charge

* _DRAM is more widely used and referred to as **`main memory`**_

## Static RAM - SRAM
* uses **more transistors per bit (4–6 typically)**
* -> SRAM is typically the **`cache in the CPU`**
* -> SRAM **`doesn’t need memory`** refreshes like DRAM
* -> SRAM is typically smaller, faster, but more expensive (_use more transitor_) than DRAM

## NVRAM - non-volatile RAM / flash memory
* is most common in cameras
* is expensive and not typically used in computers