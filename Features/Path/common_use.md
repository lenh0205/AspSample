
```c# - 
// check đường dẫn đến thư mục chứa file xem có chưa để tạo trước khi bỏ file vô
if (!Directory.Exists(pathToSaveSatus)) Directory.CreateDirectory(pathToSaveSatus);

// kiểm trả đường dẫn tới file có tồn tại không
File.Exists(path)
```