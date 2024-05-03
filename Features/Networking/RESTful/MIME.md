
# Get "MIME" type
* -> GetMimeMapping() method takes a file name or a file path as an argument
* -> depend on the file extension passed returns the corresponding MIME type

```c#
string fileName = "example.pdf";
string mimeType = MimeMapping.GetMimeMapping(fileName);

Console.WriteLine($"The MIME type for file '{fileName}' is '{mimeType}'.");
// The MIME type for file 'example.pdf' is 'application/pdf'.
```

=========================================================================
# Media Type / MIME type - Multipurpose Internet Mail Extensions
* -> indicates the **`nature and format`** of **`a document, file, or assortment of bytes`**
* -> **Browsers use the MIME type**, **`not the file extension`**, to determine how to process a URL 
* => _so it's important that web servers send the correct MIME type in the response's **`Content-Type`** header_

## Structure
* -> **type/subtype;parameter=value** (_`parameter` is optional_)
* -> the **type** represents the general category into which the data type falls
* -> the **subtype** identifies the exact kind of data of the specified type the MIME type represents

```r
text/plain;charset=UTF-8
// -> if no charset is specified, the default is ASCII;  unless overridden by the user agents settings
```
* -> **`type`** có 2 loại là **discrete** và **multipart**
* -> for **`text documents without a specific subtype`**, **text/plain** should be used; similarly, for **`binary documents without a specific or known subtype`**, **application/octet-stream** should be used

================================================================

## "discrete" type
* -> types which represent **a single file or medium**, such as **`a single text`** or **`music file`**, or **`a single video`**

### application
* -> **any kind of binary data** that **doesn't fall explicitly** into **`one of the other types`**
* -> either data that will be **executed or interpreted in some way** 
* -> or **`binary data`** that **requires a specific application or category of application** to use

* -> **`Generic binary data`** - **`binary data`** whose true type is **unknown**, is **application/octet-stream**
* -> other common examples include **application/pdf**, **application/pkcs8**, and **application/zip**

### audio
* -> **audio or music data**; examples include **audio/mpeg**, **audio/vorbis**

### example
* -> reserved for use as **`a placeholder in examples showing how to use MIME types`**
* -> **`should never be used outside of sample code`** listings and documentation

* -> **example** can also be used as **`a subtype`**
* -> Ex: in an example related to working with audio on the web, the MIME type **`audio/example`** can be used 
* -> to indicate that the type is **`a placeholder and should be replaced with an appropriate one`** when using the code in the real world

### font
* -> **Font/typeface data**; common examples include **font/woff**, **font/ttf**, and **font/otf**

### image
* -> **image or graphical** data 
* -> including both **`bitmap and vector still images`** ; as well as **`animated versions of still image formats`** such as animated **GIF** or **APNG**
* -> common examples are **image/jpeg**, **image/png**, and **image/svg+xml**

### model
* -> **`model data for a 3D object or scene`**; examples include **model/3mf** and **model/vrml**

### text
* -> **text-only data** including any **`human-readable content`**, **`source code`**, or **`textual data`** such as comma-separated value (CSV) formatted data
* -> examples include: **text/plain**, **text/csv**, and **text/html**

### video
* -> **`Video data or files`**, such as MP4 movies **video/mp4**


## "multipart" type
* -> represents **a document that's comprised of multiple component parts**, each of which may have its **`own individual MIME type`**
* -> or, a multipart type may **encapsulate multiple files being sent** together **`in one transaction`**

### message
* -> **a message that encapsulates other messages**
* -> can be used to represent an email that **`includes a forwarded message as part of its data`**, or to allow **`sending very large messages in chunks as if it were multiple messages`**

* -> _l **message/rfc822** - for **`forwarded or replied-to message quoting`**_
* -> _l **message/partial** to allow **`breaking a large message`** into smaller ones automatically to be reassembled by the recipient_

### multipart
* -> data that consists of **`multiple components`** which may individually have **`different MIME types`**

* -> _l **multipart/form-data** - for data produced using the **`FormData API`**_
* -> _l **multipart/byteranges** - used with HTTP's **`206 "Partial Content" response`** returned when the fetched data is only part of the content, such as is delivered using the **`Range`** header_

==============================================================
# Important MIME types for Web developers

## application/octet-stream

## text/plain

## text/css

## text/html

## text/javascript

## Image types

## Audio and video types

## multipart/form-data

## multipart/byteranges