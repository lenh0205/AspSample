==================================================================
# Media Type / Content-Type / MIME type - Multipurpose Internet Mail Extensions
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

===================================================================

# Get "MIME" type
* -> GetMimeMapping() method takes a file name or a file path as an argument
* -> depend on the file extension passed returns the corresponding MIME type

```c#
string fileName = "example.pdf";
string mimeType = MimeMapping.GetMimeMapping(fileName);

Console.WriteLine($"The MIME type for file '{fileName}' is '{mimeType}'.");
// The MIME type for file 'example.pdf' is 'application/pdf'.
```

==================================================================
# Common use case

## RAR-compressed files
* -> in this case, the ideal would be the **true type of the original files**; 
* -> but this is **`often impossible`** as .RAR files can **`hold several resources of different types`**
* -> in this case, configure the server to send **application/x-rar-compressed**

## Audio and video
* -> only resources with the **correct MIME Type will be played** in **`<video> or <audio> elements`**; so be sure to specify the correct media type for audio and video.

## Proprietary file types
* -> **avoid using application/octet-stream** as most browsers **`do not allow defining a default behavior`** (_like "Open in Word"_) for this generic MIME type
* -> **`a specific type`** (_like **`application/vnd.mspowerpoint`**_) lets users open such files automatically in the presentation software of their choice.

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

==========================================================================

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
* -> the **`default for binary files`** - means **unknown binary file**; browsers usually don't execute it, or even ask if it should be executed
* -> commonly used for **unrecognized resources** (_l **`arbitrary binary data`** (RFC 2046) that **`doesn't have a specific file format`**_)
* ->  for security reasons, most browsers **do not allow setting a custom default action** for such resources, **forcing the user to save it to disk** to use it

* => _nói chung nó chỉ phù hợp khi ta có mục đích / dự tính **`lưu những entities vào disk`**_ 
* => _tức là, the only thing one can **`safely do with application/octet-stream`** is to **`save it to file`** and **`hope someone else knows what it's for`**_

```c#
httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
```
## text/plain
* -> this is the **`default for textual files`**; even if it really means **unknown textual file**, browsers assume they can display it

## text/css
* -> **`CSS files used to style a Web page`** **must be sent with text/css**
* -> if a server doesn't recognize the .css suffix for CSS files, it may send them with **`text/plain`** or **`application/octet-stream`** MIME types
* => if so, they **won't be recognized as CSS by most browsers and will be ignored**

## text/html
* -> **all HTML content** should be served with this type
* -> alternative MIME types for XHTML (_like **`application/xhtml+xml`**_) are **`mostly useless`** nowadays

## text/javascript
* -> **JavaScript content** should always be served using the MIME type text/javascript. 
* -> no other MIME types are considered valid for JavaScript, and **`using any MIME type other than text/javascript`** may result in **scripts that do not load or run**

* -> **Note**:  **`charset`** parameter isn't valid for JavaScript content, and in most cases will result in **a script failing to load**

## Image types
* -> contain **image data**; the **`subtype`** specifies which **`specific image file format`** the data represents
* -> **image/apng**, **image/avif**, **image/gif**, **image/jpeg**, **image/png**, **image/svg+xml**, **image/webp**

## Audio and video types
* -> 

## multipart/form-data
* -> can be used when **sending the values of a completed HTML Form from browser to server**
* -> as a multipart document format, it consists of different parts, delimited by **a boundary** - a string starting with a double dash **--**
* -> each part is **its own entity** with its **`own HTTP headers, Content-Disposition, and Content-Type`** for file uploading fields

```r - Sample
Content-Type: multipart/form-data; boundary=aBoundaryString
(other headers associated with the multipart document as a whole)

--aBoundaryString
Content-Disposition: form-data; name="myFile"; filename="img.jpg"
Content-Type: image/jpeg

(data)
--aBoundaryString
Content-Disposition: form-data; name="myField"

(data)
--aBoundaryString
(more subparts)
--aBoundaryString--
```

```js - Example
<form
  action="http://localhost:8000/"
  method="post"
  enctype="multipart/form-data">
  <label>Name: <input name="myTextField" value="Test" /></label>
  <label><input type="checkbox" name="myCheckBox" /> Check</label>
  <label>
    Upload file: <input type="file" name="myFile" value="test.txt" />
  </label>
  <button>Send the file</button>
</form>

POST / HTTP/1.1
Host: localhost:8000
User-Agent: Mozilla/5.0 (Macintosh; Intel Mac OS X 10.9; rv:50.0) Gecko/20100101 Firefox/50.0
Accept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8
Accept-Language: en-US,en;q=0.5
Accept-Encoding: gzip, deflate
Connection: keep-alive
Upgrade-Insecure-Requests: 1
Content-Type: multipart/form-data; boundary=---------------------------8721656041911415653955004498
Content-Length: 465

-----------------------------8721656041911415653955004498
Content-Disposition: form-data; name="myTextField"

Test
-----------------------------8721656041911415653955004498
Content-Disposition: form-data; name="myCheckBox"

on
-----------------------------8721656041911415653955004498
Content-Disposition: form-data; name="myFile"; filename="test.txt"
Content-Type: text/plain

Simple file.
-----------------------------8721656041911415653955004498--
```

## multipart/byteranges
* -> used to **send partial responses to the browser**
* -> when the **`206 Partial Content`** status code is sent, this MIME type indicates that the **`document is composed of several parts`**, **one for each of the requested ranges**
* -> like other multipart types, the Content-Type uses **a boundary** to separate the pieces; each piece has a **`Content-Type`** header with its actual type and a **`Content-Range`** of the range it represents

```r
HTTP/1.1 206 Partial Content
Accept-Ranges: bytes
Content-Type: multipart/byteranges; boundary=3d6b6a416f9b5
Content-Length: 385

--3d6b6a416f9b5
Content-Type: text/html
Content-Range: bytes 100-200/1270

eta http-equiv="Content-type" content="text/html; charset=utf-8" />
    <meta name="viewport" content
--3d6b6a416f9b5
Content-Type: text/html
Content-Range: bytes 300-400/1270

-color: #f0f0f2;
        margin: 0;
        padding: 0;
        font-family: "Open Sans", "Helvetica
--3d6b6a416f9b5--
```
