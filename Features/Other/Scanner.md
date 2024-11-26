==================================================================================
> source: https://github.com/mgriit/ScanAppForWeb/tree/master

# View
* -> tạo 1 biến global **ws** là instance của **`window.WebSocket("ws://localhost:8181/")`**

* -> sự kiện **onmessage** chính là sự kiện lúc ta nhận data từ server
* -> ta sẽ có biến global "selDiv" để hiển thị những <img> được tạo từ name của Blob do server gửi tới
* -> và biến global "storedFiles" là 1 array để lưu những Blob đó
* -> event handler "editFiles" sẽ modify cái "storedFiles" array

* -> khi click nút "Scan" chạy **`ws.send("1100")`**

```js - event
$(document).ready(function () { // after the HTML document is fully loaded and ready
    selDiv = $("#selectedFiles");

    // attach a click event handler to all elements with the class "selFile" with event handler "editFiles"
    // ".selFile" là 1 <img> được tạo khi server gửi message cho client dưới dạng Blob thông qua websocket
    $("body").on("click", ".selFile", editFiles);
});
```

# Setup
* -> máy scan của công ty: vào phần **Network** trong "File Explorer" để xem tên máy scan là gì
* -> vào **Settings** của Window -> vào phần **Bluetooth & devices** -> vào phần **Printers & scanners** -> Add device
* -> giờ kiểm tra xem cái Desktop Scan App đã chạy chưa
* -> giờ ta sẽ chạy Web lên -> bấm nút "Scan" để mở Scan App lên -> select source và reload để xem có máy scan nào đang hoạt động -> chọn máy Scan ta muốn 
* -> để tờ giấy vô máy Scan -> bấm nút "start scan" trên Scan App -> xem nó có upload đúng hình lên web không

==================================================================================
# WIA - Windows Image Acquisition (or Windows Imaging Architecture)
* -> is **`a Microsoft driver model and application programming interface (API)`** to create **a scanning application with C# in WinForms**
* -> used for **Microsoft Windows 2000 and later operating systems** that enables **`graphics software`** to communicate with **`imaging hardware`** (_such as scanners, digital cameras and Digital Video-equipment_)

## 'OnHandleCreated'
* -> **a protected virtual method** of **`Control`** class that get called when **`the control's underlying window handle is created`**
* -> by **override this method** we can **`perform custom actions at the point where the control's handle becomes available`**
* -> this is often necessary for **`interacting with unmanaged resources or APIs`** that **depend on the control's handle**


