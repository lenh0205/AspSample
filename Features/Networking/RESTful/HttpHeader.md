# Response Header

## Content-Type: 

### application/octet-stream
* -> commonly used for **arbitrary binary data** (_RFC 2046_) that **doesn't have a specific file format**
* -> to **`indicating to the client`** that the **`data being transferred`** is in the **`form of a stream`**
* -> _nó chung là để chỉ dẫn cho Client biết cần interpret the data as a stream_
* => nó chỉ phù hợp khi ta có mục đích / dự tính **`lưu những entities vào disk`** 
* => tức là, the only thing one can **`safely do with application/octet-stream`** is to **save it to file** and **hope someone else knows what it's for**

```c#
httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
```

## Content-Disposition
* -> ta có thể kết hợp **`Content-Disposition`** header với **`Content-Type`** (khác với ) như là: _image/png, text/html_ 
* -> most browsers recognise **inline** to mean we want the entity **`displayed if possible`** but if it's **a type the browser knows how to display**
* _tức là muốn Browser cho phép người dùng preview 1 file .pdf trước khi download, thì **`Content-Type`** cần là "application/pdf" chứ không thể là "application/octet-stream" được_
* -> ta cũng có thể thêm **filename** 1 phần của header; browsers will use as the suggestion if the **`user tries to save`** (_but users can always override that_)

```r
// I dont know what the hell this is. Please save it as a file, preferably named picture.png:
Content-Type: application/octet-stream
Content-Disposition: attachment; filename="picture.png"

// This is a PNG image. Please save it as a file, preferably named picture.png:
Content-Type: image/png
Content-Disposition: attachment; filename="picture.png"

// This is a PNG image. Please display it unless you dont know how to display PNG images. 
// And if the user chooses to save it, we recommend the name picture.png for the file you save it as
Content-Type: image/png
Content-Disposition: inline; filename="picture.png"
```

```c#
httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
httpResponseMessage.Content.Headers.ContentDisposition.FileName = "myfile.pdf";
```

## Location
* -> the _Location response header_ indicates **the URL to redirect a page to**
* -> it only provides a meaning when served with **`a 3xx (redirection) or 201 (created) status response`**: **201, 301, 302, 303, 307, 308**

========================================================
# Request Header