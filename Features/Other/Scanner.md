
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
* -> máy scan của công ty: vào phần **Network** trong "File Explorer"