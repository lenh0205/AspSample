# Response Header

## Content-Type: 
* _Vào `MIME.md` để tìm hiểu kĩ hơn_

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