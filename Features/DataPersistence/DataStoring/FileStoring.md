
# Saving files in "File System" or "Database" ?
* **`should do both`**
* -> to avoid clogging up the DB with huge data transfers: file systems are used to that but the bigger the data, the more memory the DB server has to allocate, the more bandwidth it uses to transfer, and so forth.

* small ==> database

* large file ==> folder; with links to them going into the DB.
* -> don't use the original file name: keep that in the DB with the file link, and give the file a temporary name (_Ex: use Guids_) to avoid problems when you get two users with the same file name 

* Another option for large files in SQL Server is to store the files in a database **`using the Filestream`** data type. The files are then stored in the file system by SQL Server and retrieved using SQL queries.
* -> not giving access outside of the DMZ to a web application, except through a database connection
* -> efficiency goes up as file size goes up. Myself, shouldn't use it for files smaller than 100MB

# Solution base on "demand"
```cs 
// when the project was started, the need of having a separate file system and its management adds to extra costs. since the number of files will not be huge
// Although if usage of files from day 1 is going to high then it makes sense to use a file-system based approach over DB
// Personally I am inclined towards putting files on separate file-system as I prefer to avoid loading file (database blob) in application memory and is more cost effective solution over database
// Hence I will store a simple relative path of files on disk(SFTP server may be) in the database than storing in a BLOB
```

===============================================

# Saving Files in "Database" is Bad practice
## Purpose:
* If your user's file needs to be more tightly coupled, secured and confidential.  
* If your application will not demand a large number of files from a large number of users.

## Best practice
* Be cautious with your Select query, Avoid Unwanted Select * queries which may frequently retrieve the file data unnecessarily.
* Caching the file data can pave a way to reduce memory and database usage.
* If you are using SQL server 2008 or higher version, Make use of FILESTREAM
* -> Filestream enables storing BLOB data in NTFS while at the same time it ensures transactional consistency between the unstructured Blob data with a structured data in DB.

## Advantages
* provides **`ACID`** compliance for each row -> consistency, which includes a rollback of an update that is complicated when the files are stored outside the database.
* provides **`data integrity`** between the file and its metadata -> Files will be in sync with the database so cannot be orphaned from it which gives us an upper hand in tracking transactions.
* **`Database Security`** is available by default
* Backups automatically include files, no extra management of file system necessary
* Database indexes perform better than file system trees when more number of items are to be stored
* Images smaller than 64kb and the storage engine of your db supports inline Blobs (InnoDB puts only 767 bytes of a TEXT or BLOB inline, MyISAM puts inline), it improves performance further as no indirection is required (Locality of reference is achieved, more about this can be read here).
* File deletion/updation is always in sync with row operations, no extra maintenance needed.

## Downside
### Slows down overall database queries performance
* -> because there is **`more data transmitted`** between the application and the database
* -> **`files use up RAM`** which is used internally by the database to improve performance.

* In other words, when database is busy serving files, other resources can't be used to respond to other queries
* -> **`Frequently accessed data is stored in RAM`** because it provides much faster reads than disk storage
* -> Querying files likely means the database will end up **`storing those files into RAM`**
* _but Servers are typically constrained on the amount of RAM available_
* -> database ends up **`prioritising some data over the other`**
* -> Data that isn't stored in RAM has to be read from disk which is always much slower than RAM

### Database maintenance becomes more difficult
* _`The level of knowledge required to maintain a database goes up in proportion to the size of the database`_
* and a sure way to **rapidly increase the size of database** is to **`store large files in it`**

* **Back up a larger database** takes more time and results in `larger backup files` that are slower to move around

* **`Other maintenance tasks`** (restoring from backup, adding indexes, database defragmentation, ...) also get slower over time and are more likely to fail

* => The longer it takes for a database maintenance task to run, the larger the impact will be on your application and end-users
* => In a replica set, a larger database leads to slower replication times and bigger replication delays
-> may cause `race conditions` and `synchronisation problems` **`if our application code doesn't handle that well`**

### Storing and serving files is more complex
* To save a file in a database, it often needs to be **converted in a way so it can be correctly stored**

* **`If store the file as text`**
* -> we might decide to store it in base64 format for example
* -> we need to write some logic in your application to convert files to base64 before they're saved into the database. 
* -> also need to handle the reverse case — reading from the database and converting from base64 to binary before passing the file on to the client
* -> **`Files stored in base64 end up using 33% more space than their original size`**

* **`Storing files as binaries`** in the database has a similar downside
* -> escaping/encoding binary data in your application before sending it off to the database

* You may have to convert the files to blob in order to store it in DB

* => end up with **an additional layer in application** that needs to be maintained 
* -> _None of this is difficult or time-consuming, but it adds `complexity and becomes another point of failure`_

### Other downside
* **Increased costs**
* -> Hard disk storage is generally cheap but **`the equivalent in RAM is significantly more expensive`**
* -> A larger database will **`use up more RAM to store`** indexes, commonly queried data to improve performance (_not to mention a database is already often the largest consumer of RAM in the application stack_)

* **Database limits**. 
* **`MongoDB documents are limited to 16MB in size`**; we have to use GridFS to store larger files in MongoDB
* **`In PostgreSQL, columns are limited to 1GB per row`**. If the file we want to store is larger, we have to use a dedicated table designed for storing large objects.

## Does that mean you should never store files in the database?
* -> it depends - the scale you're optimising for might never arrive in the future
* -> on a **`small scale`**, the downsides of storing files in the database may not be that important
* -> _when you're working on a quick prototype to show to a client, or a side project for learning purposes, or or an application with a slow growth projection_
* -> it's more important to **`deliver quickly and focus on the must-have features`**. 

================================================
# Store Files Option
* Aside from the database, you can **store files locally** or **in the cloud**. In both cases, it's common to **`store a path to the file's location in the database`**

================================================
# Saving Files in "File System"
* Storing files on the file system is a **`popular choice`**. 
* It's convenient to have files stored locally close to your application code
* -> makes it easy to **`write/read files without having to send them over the network`** to an external provider

## Purpose: 
* khi our application is liable to handle Large file size more than 5mb and a massive number (VD: thousands of file uploads) 
* our application will have a large number of users

## Best practice:
* **Platform-as-a-Service** providers such as _Heroku, AWS Elastic Beanstalk and DigitalOcean App Platform_
* -> have an **`ephemeral/short-lived file system`**. 
* -> Any files saved locally will disappear after deploying or restarting the application. 
* -> so we can't rely on the file system to store files that **`need to persist for a longer time`**
* -> Use a **VPS** if you want to store files locally, or store files in the **cloud** instead

* A good Internal Folder Structure and choosing a folder location which may be a little difficult to access by others

## Advantage:
* **Performance**: read/write to a file system is always faster than to a DB 

* Saving the files and downloading them in the file system is much simpler than database 
* -> since a simple Save as function will help you out
* -> Downloading can be done by addressing a URL with the location of the saved file

* Migrating the data is an easy process here. 
* -> You can just copy and paste the folder to your desired destination while ensuring that write permissions are provided to your destination

* Cost effective 
* -> expand your web server rather than paying for certain Databases.
* -> file servers are much cheaper compared to database

* allows usage of lightweight web-server/CDN/Amazon S3 for serving
* Easy to migrate it to Cloud storage like Amazon s3 or CDNs etc in the future

* avoids database overhead with its CRUD operations.

* easier in cases where files (video, audio etc) are to be shared with third party providers

## Disadvantages
* Loosely packed. 
* -> No ACID (Atomicity, Consistency, Isolation, Durability) operations relational mapping which mean there is no guarantee
* -> Consider a scenario if your files are deleted from the location manually or by some hacking dudes, you might not know whether the file exists or not

* Low Security
* -> Since your files can be saved in a folder where you should have provided write permissions, it is prone to safety issues and invites troubles like hacking.

=============================================
# Cloud storage
* Cloud storage is the **go-to solution for larger applications**
* -> takes away the burden of **`backups`**, **`redundancy`**, **`delivery`** and **`access control`**
* -> small websites can benefit too by **`using media-focused storage solutions`** 

* **`commonly used file/media storage solutions`**: **AWS S3**, **Cloudinary**, **DigitalOcean Spaces**, **Backblaze B2**

## AWS S3
* Amazon Web Services (AWS) has the _largest market share_ in the _cloud industry_
* **`their storage solution is called "S3"`** 
* With AWS S3, _files_ are called **`objects`** and they are **`stored in buckets`** which are _unique URL namespaces_
* S3 doesn't know the concept of folders because the **`file system is abstracted away`**
* Use the forward-slash (/) in your file names and you'll get **`folder-like navigation`** in the S3 dashboard

## Cloudinary 
* is different from the others in this list because it **operates at a higher level**
* Cloudinary is a media storage solution **built on top of AWS S3** and **`optimized for image/video storage`**. 
* they **`offer features`** such as _dynamic media transformations_, _automatic compression_ and _browser-specific media delivery_
* Cloudinary is great for when you're `building a responsive website` and want to **serve different image sizes for different devices**

## DigitalOcean Spaces
* DigitalOcean is known for its **vast collection of high-quality resources** 
* **`Anything related to DevOps, they've got you covered`**
* DigitalOcean Spaces is **`compatible with AWS S3`** therefore they have a similar concept of objects and buckets/spaces

## Backblaze B2 
* is similar to AWS S3 but **`several times cheaper`**
* they're known for **writing robust software centered on security and encryption**. 
* Backblaze can also continuously backup your laptop in the cloud 

==============================================
# Mongo GridFS(NoSql Database)
* GridFS of MongoDB is a effective storage of images
* storing files in mongo provides all the features of mongo like backup/replication/sharding/HA etc. But adds extra overhead of querying one or more collections to retrieve a single chunk

```bash - put a good quality large size Image - 2 MB at-least
// save image into:
$ mongofiles put some-large-image.jpg

$ db.fs.chunks.find().pretty()
{
    "_id" : ObjectId("5d0c95d94fcf6e4f5402702a"),
    "files_id" : ObjectId("5d0c95d94fcf6e4f54027028"),
    "n" : 1,
    "data" : BinData(0,"/9j/4AAQSkZJRgABAgAAZABkAAD/7AAR...")
}
{
    "_id" : ObjectId("5d0c95d94fcf6e4f54027029"),
    "files_id" : ObjectId("5d0c95d94fcf6e4f54027028"),
    "n" : 0,
    "data" : BinData(0,"/9j/4AAQSkZJRgABAgAAZABkAAD/7AAR...")
}
```
* we have two documents here, both pointing to the same **fs.files** through their **files_id**
* The value of **n** specifies the ordering of the chunks so the data doesn't get messed up

# GridFS Chunks
* `MongoDB’s document` size has an upper limit of 16MB, the idea of `chunking` is to allow **streaming** 
```cs
// allow users to download (or stream) a file without having the server to store the whole thing in RAM
```

* This **`mechanism`** is heavily used in streaming videos, imagine movie of 2 GB HD quality loaded in `server’s RAM`, that’d be really wastage of precious resources
* The **`default chunk size`** of GridFS is 256kB which is supposedly a good compromise of overhead (more queries to the database) and little memory use, but it can be configured

* `Streaming` hardly makes sense for small images; delivering those images will require at least two to **three round-trips** to the database instead of one
* -> One to find the fs.files document
* -> two to get the chunks
* -> three to start to deliver the file
