
# Dùng code để tạo 1 File tạm
* -> khác với web khi deploy ra file .msi, app sẽ được cài đặt vào thư **`Program Files`** của ổ C:
* -> để truy cập vào các file thuộc thư mục có thể sẽ requires **`administrator privileges`** to modify or create files
* -> vậy nên ta nên sửa đường dẫn bằng đường dẫn đến thư mục được hệ thống hỗ trợ để lưu file
```cs
// "Documents" folder - common for saving user-generated files
string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
string filePath = Path.Combine(directoryPath, "myscan.pdf");

// "Application Data" folder - common for application-specific files
// -> Local AppData (specific to the user but not synced across devices):
string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
string filePath = Path.Combine(localAppDataPath, "MyApp", "myscan.pdf");
// -> Roaming AppData (synced across devices if the user is on a domain): 
string roamingPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
string filePath = Path.Combine(roamingPath, "MyApp", "myscan.pdf");

// system's "Temporary" folder - used for temporary files
string tempPath = Path.GetTempPath();
string filePath = Path.Combine(tempPath, "myscan.pdf");

// to allow user to specify where to save the file
using (SaveFileDialog saveFileDialog = new SaveFileDialog())
{
    saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
    if (saveFileDialog.ShowDialog() == DialogResult.OK)
    {
        string filePath = saveFileDialog.FileName;
        // Save the file
    }
}
```