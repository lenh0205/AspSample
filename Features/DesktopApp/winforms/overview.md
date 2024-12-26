> **original project type in .NET** designed to be a rapid application development enviroment for **`desktop applications`**
> sau này có thêm WPF, UWP, MAUI

=====================================================================
# Setup Project
* -> create an Installer For a WinForm Application, publish WinForms into a '.msi' file

# Tạo 'Setup Project'
* đầu tiên cần install the **`Visual Studio Installer Projects Extension`**
* -> Visual Studio -> **Extension** menu -> Manage Extensions -> tìm **Visual Studio Installer Projects**
* -> sau đó nó sẽ chạy **VSIX Installer** -> click **Modify** để kích hoạt
* -> giờ restart lại Visual Studio 

* -> nếu có sẵn **setup_project** trong solution thì ta chỉ cần **Add existing project** và chọn file project **`.vdproj`** là được

* -> còn nếu cần tạo mới thì: File -> Add -> New Project -> search for **`Setup Project`** (or **`Setup Wizard`** template)
* -> trong cửa sổ **File System** của setup project -> right-click on **`Application Folder`** -> chọn "Add" -> chọn "Project Output..." -> chọn project ta muốn và chọn **`Primary Output`** -> OK
* -> nó sẽ add 1 primary output of our setup với tên **Primary output from ....**
* -> ta sẽ right-click lên nó -> chọn **Create Shortcut to ...** -> đổi tên shortcut thành "My App" chẳng hạn (đây là tên App của ta sau khi publish ra thành production)
* -> ta có thể thay đổi icon cho product app bằng cách: right-click vào shortcut -> chọn **Properties Window** -> thay đổi **Icon** property
* -> giờ ta sẽ kéo shortcut này vào **`User's Desktop`** (để sau khi user cài xong app, nó sẽ hiện thị app trong desktop của user)
* -> (ta cũng có thể tạo thêm 1 shorcut tương tự và kéo vào **`User's Programs Menu`** nếu muốn)
* -> giờ ta right-click vào Setup project chọn **Build**
* -> sau khi build thì **installers (Setup.exe and ProjectName.msi)** are ready in the **Debug** folder of the Setup Project
* -> ta có thể chạy 1 trong 2 on target machine

# Đảm bảo 'Setup project' chạy success
* -> trước tiên kiểm tra xem **`Primary Out`** có đúng là project ta muốn public ra file .msi chưa; 
* -> right-click vào project để check xem đang **Active** Debug hay Release để biết nó build msi vào thư mục nào
* -> cũng như kiểm tra trong **`Prerequisite`** xem nó có đang trỏ đến cùng **.NET target Framework** mà "primary output" project đang trỏ đến không
* -> right-click để **`Rebuild`** nó 
* -> nếu không thể build ra file .msi mới cần đảm bảo clear tất cả error lẫn warning (nhất là warning về version, thiếu file)
* -> lưu ý chỉ để những Reference và kiểm tra version của project ta chỉ định làm Primary Output
* -> xem các **detected Reference** của Setup_project có thừa thiếu gì với "primary out" project không
* -> nếu build success nhưng vẫn bị warning trong quá trình build thì ta cũng nên kiểm tra lại

# Debug Desktop App installed by .msi
* -> khi Rebuild cần đảm bảo **setup_project** đang active **`Debug`** mode
* -> chạy file **.msi** để install application và chạy application
* -> Visual Studio -> Debug -> Attach to Process -> tìm process **`application_name.exe`**
* -> đặt breakpoint vào project xem có được không

# 'Setup Project' vs 'Setup Wizard' template
* -> **`Setup Project`** template sẽ tạo trực tiếp setup project
* -> còn **`Setup Wizard`** sẽ cho ta 1 wizard gồm 5 steps để tạo 1 trong các project này: **Setup Project**, **Web Setup Project**, **Merge Module Project**, **CAB Project** 

=====================================================================
## Use case
* -> nó cho phép trả xây dựng app rất nhanh nhưng sẽ kèm với 1 số bất lợi lâu dài; vậy nên sẽ phù hợp đối khi cần triền khai thử 1 ý tưởng 

## Setup
* -> mở Visual Studio -> create new project -> chọn Template **`Windows Forms App`**
* -> khi lần đầu ta mở lên ta sẽ có ngay **Form1.cs [Design]** - a design surface cho ta làm việc, có nút phóng to, thu nhỏ, đóng
* -> giờ ta có thể chạy project để visualize nó 

## Develop
* -> khi ta right-click vào "Form1.cs" sẽ có **`View Designer`** để xem designer của ta và **`View Code`** để xem code
* -> về cơ bản thì 1 **Form** (Ex: Form1) sẽ cấu thành cơ bản từ **`Designer`** (Form1.cs [Designer]), **`Event`** (Form1.cs), **`Code associate between Designer and Event`** (Form1.Designer.cs) và Form1.resx (không biết làm gì)

* -> ta mở bảng **`Toolbox`** lên (View -> Toolbox) nó sẽ chứa các control để ta thêm vào form của ta
* -> ta sẽ thêm nhưng control thường dùng như: button, textbox, label, checkbox, progress bar,....
* -> khi ta kéo thêm 1 control vào form của ta thì đồng nghĩa với việc ta thêm 1 variable vào form class của ta 

## Control properties
* -> ta có thể right-click vào control để mở bảng **`properties`** của chúng (đây thực sự là những properties trong class của control)
* -> xem các **Event** mà 1 control có bằng nút có hình sấm sét
* -> ta có thể xem **(Name)** property của control để sử dụng nó như 1 biến trong code của ta
* -> ví dụ đối với progress bar thì ta có thể update **Value** để hiện tiến độ ta muốn

## Event
* -> khi ta double-click lên 1 control trong designer nó sẽ tạo **`default event`** 
* -> VD: của button là "click"; với form là "load" (cho phép ta làm gì đó khi form load)

* -> Ví dụ giờ ta muốn thêm event khi click button
* -> ta sẽ double-click vào button, nó sẽ tự động mở file **Form1.cs** với **`class Form1 : Form`** chứa "button1_Click" method rỗng
```cs
// VD: ta đang có 5 textbox, 1 checkbox, 1 progress bar, 1 button
public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
        label2.Text = "Last Name"; // đổi chữ "label2" thành "Last Name" ở runtime
        label3.Text = "Full Name"; // đổi chữ "label3" thành "Full Name" ở runtime
    }

    private void button1_Click(object sender, EventArgs e)
    {   
        MessageBox.Show($"Hello { textBox1.Text } { textBox2.Text }");

        // Ví dụ: ta thêm logic để mở 1 Form2 nào đó
        Form2 frm = new();
        frm.Show();
    }
}
```

* -> giờ ta sẽ chạy dự án và nhập vào 2 textbox đầu tiên là "Tim" và "Cook" rồi click vào button nó sẽ hiện thị lên cho ta 1 bảng thông báo với dòng chữ là `Hello Tim Cook` đồng trời text thứ 3 sẽ hiện chữ "Tim Cook"

## Most common problem: Designer break
* -> ví dụ giờ ta thử double-click vào Form -> nó sẽ tạo method "Form1_load" trong class "Form" -> nhưng ta không muốn nó nên xoá nó đi -> nếu giờ ta save lại và thử "View Designer" lại thì designer sẽ báo lỗi: `the name "Form1_load" does not exist in the current context`
* -> lỗi này là do ta chỉ modify 1 chỗ mà không modify những chỗ liên quan

* -> đúng hơn lỗi này là do `the code within the method 'InitializeComponent' is generated by the designer and should not be manually modified`
* -> giờ ta sẽ click vào "Go to Code" ở phần báo lỗi, nó sẽ đưa ta đến file **Form1.Designer.cs**

* -> vì cơ bản, **a window form** has split class behind it - gồm các **partial class** khác nhau cùng tên cho phép đặt code ở 2 nơi khác nhau nhưng form cùng 1 class
* -> **Form1.Designer.cs** đại diện cho UI look like; giờ ta chỉ cần modify nó lại file và "View Designer" lại là được

# Program.cs
* -> lý do tại sao khi launch app thì Form1 lại hiện lên; và khi ta đóng Form1 thì nó cũng đóng toàn bộ Application (dù các form khác vẫn mở)
```cs
[STAThread]
static void Main()
{
    ApplicationConfiguration.Initialize();
    Application.Run(new Form1()); // Form1 consider the main form 
}
```

=====================================================================
# Window Forms lifecycle

## Concept
* -> in Windows Forms, **every control** (e.g., a Form, Button, etc.) has an **`HWND - underlying Windows handle`**, 
* -> which is a unique identifier for the control in the Windows operating system; this handle is required for Windows to interact with the control

## Key Lifecycle Events
* -> Constructor: Initializes the control. No handle yet.
* -> OnHandleCreated: Handle is created. Control can now interact with Windows.
* -> OnLoad: Form and its controls are ready to be displayed.
* -> Shown: The form becomes visible.

## 'OnHandleCreated'
* -> **a protected virtual method** of **`Control`** class that get called when **`the control's underlying window handle is created`**
* -> by **override this method** we can **`perform custom actions at the point where the control's handle becomes available`**
* -> this is often necessary for **`interacting with unmanaged resources or APIs`** that **depend on the control's handle**
