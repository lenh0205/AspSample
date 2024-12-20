# WPF - Presentation Foundation
* -> WPF applications often use **`XAML (Extensible Application Markup Language)`** for designing the UI

## App.xml - Entry Point & startup
* -> WPF applications don't have a **Program.cs** file by default, it use **App.mxl**
* -> the "App.xaml" file typically links to a corresponding **`App.xaml.cs`** file where the startup logic resides

## ".xaml" file
* -> nó sẽ chứa cả **`Designer`** và **`code XAML markup`** 
* -> link tới file **`.xaml.cs`** - code-behind chứa các **event handler** và **logic related to the UI**
* -> **`InitializeComponent`** method sẽ generate trong file **.g.i.cs** - 1 file tạm được tạo trong quá trình compilation, thường locate trong thư mục "obj" và giấu khỏi project

```html - MainWindow.xaml
<Button Margin="3,3,0,3" Width="80" Name="buttonScanPages" Click="buttonScanPages_Click">_Scan Page(s)</Button>
```
```cs - MainWindow.xaml.cs
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    // event của control "buttonScanPages"
    private void buttonScanPages_Click(object sender, RoutedEventArgs e)
    {
    }
}
```