> thường thì máy in sẽ hỗ trợ 2 chế độ scan: **`Flatbed Scanner`** và **`ADF - Automatic Document Feeder`** 
> **Flatbed Scanner** - place the document on the glass scanner; great for scanning photos, books, and other items that might not feed well through **ADF**
> **ADF** - place the document in the slot; convenient for scanning multiple pages quickly

> 2 most popular multipage image formats are TIFF and PDF (tức là đối với scan nhiều image thì bỏ thành file TIFF và file PDF là hợp lý nhất)

==================================================================================
# Document Scanner
* -> several **scanning drivers** in the market: **`TWAIN Scanner`**, **`WIA Scanner`**, **`ISIS Scanner`**, **`SANE Scanner`** 
* -> that support **`acquiring physical images from scanners`** and **`storing the digital images on a computer`** to achieve the same task basically

## Usage
* -> both **TWAIN and WIA can work with scanners and cameras** as long as **`the driver is installed`**

* -> **`TWAIN` is the most commonly used protocol and the standard in document scanners** (_especially, members of the TWAIN working group: Fujitsu Computer Products of America, Kodak Alaris, Epson America, HP, and Visioneer_)
* ->  if our application is going to **interact with scanners most of the time**, especially if **old scanners** need to be supported, **`TWAIN`** is recommended
* -> for **cameras**, **`WIA`** offers better support
* -> but, sometimes TWAIN based applications can communicate with WIA devices, such as scanners or cameras, via the **`TWAIN compatibility layer`**

## TWAIN
* -> **the `Source Manager Interface` provided by `TWAIN`** allows our **`application to control data sources`** (_such as scanners and digital cameras, and acquire images_)

* -> although **`nearly all scanners contain a TWAIN driver`** that **complies with the TWAIN standard**, 
* -> **the implementation of each TWAIN scanner driver may vary slightly** in terms of **`scanner setting dialog, custom capabilities, and other features`**
* => it's fine if we use features specific to a particular scanner model, but if we want our **application’s scanning behavior to be consistent on different scanners**, we need to be wary of customized code

https://github.com/DanKyungu/NTwainScanProject/tree/master/ScanProject
https://stackoverflow.com/questions/73251879/how-to-scan-using-ntwain
https://github.com/soukoku/ntwain

==================================================================================
> source: https://github.com/mgriit/ScanAppForWeb/tree/master

# Communication Web and Scanner (desktop App)

## Setup
* -> máy scan của công ty: vào phần **Network** trong "File Explorer" để xem tên máy scan là gì
* -> vào **Settings** của Window -> vào phần **Bluetooth & devices** -> vào phần **Printers & scanners** -> Add device
* -> giờ chạy Desktop Scan App của ta
* -> giờ ta sẽ chạy Web lên -> bấm nút "Scan" để kết nối và mở UI của Scan App để ta tương tác -> select source và reload để xem nó có tìm thấy máy scan nào đang hoạt động -> chọn máy Scan ta muốn 
* -> để tờ giấy vô máy Scan -> bấm nút "start scan" trên Scan App -> xem nó có upload đúng hình lên web không

## View
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
==================================================================================
> https://ourcodeworld.com/articles/read/382/creating-a-scanning-application-in-winforms-with-csharp#google_vignette

# WIA - Windows Image Acquisition (or Windows Imaging Architecture)
* -> is **`a Microsoft driver model and application programming interface (API)`** to create **a scanning application with C# in WinForms**
* -> used for **Microsoft Windows 2000 and later operating systems** that enables **`graphics software`** to communicate with **`imaging hardware`** (_such as scanners, digital cameras and Digital Video-equipment_)

## Setup
* -> create a new WinForms project -> reference the **`Microsoft Windows Image Acquisition Library`** COM component directly from visual studio -> set the the **Embed Interop Types** property of the "WIA" component to False

## Handle WIA Exception
* -> list of error codes: https://learn.microsoft.com/en-us/windows/win32/wia/-wia-error-codes?redirectedfrom=MSDN
* -> to handle errors of WIA, we'll catch the **`COMException`** object

## Showing scanning progress
* -> to show the progress of the scanner, we can use the **`ShowTransfer`** method of the **`CommonDialogClass`**
* -> the **CommonDialog** control is an **`invisible-at-run-time control`** that we can create using **WIA.CommonDialog** as **`the ProgID in a call to 'CreateObject'`** or by **`dropping a 'CommonDialog' object on a form`**

```cs
CommonDialogClass dlg = new CommonDialogClass(); 

object scanResult = dlg.ShowTransfer(scannerItem, WIA.FormatID.wiaFormatPNG, true);

if (scanResult != null) {
    ImageFile image = (ImageFile)scanResult;
}
```

## Demo
```cs
using WIA;
using System.IO;
using System.Runtime.InteropServices;

try
{
    // Create an empty variable to store the scanner instance
    DeviceInfo firstScannerAvailable = null;

    // Create a DeviceManager instance
    var deviceManager = new DeviceManager();

    // Loop through the list of devices to choose the first available
    for (int i = 1; i <= deviceManager.DeviceInfos.Count; i++)
    {
        // Skip the device if it's not a scanner
        if (deviceManager.DeviceInfos[i].Type != WiaDeviceType.ScannerDeviceType) continue;

        firstScannerAvailable = deviceManager.DeviceInfos[i];
        break;
    }

    // Connect to the first available scanner
    Device device = firstScannerAvailable.Connect();

    // Select the scanner 
    // 'Device' is a physical device; and "Item" is a specific functional unit of device
    // each 'Item' is indexed, starting from 1
    Item scannerItem = device.Items[1];

    // Set the scanner settings
    int resolution = 150;
    int width_pixel = 1250;
    int height_pixel = 1700;
    int color_mode = 1;
    AdjustScannerSettings(scannerItem, resolution, 0, 0, width_pixel, height_pixel, 0, 0, color_mode);

    // Retrieve a image in JPEG format and store it into a variable
    var imageFile = (ImageFile)scannerItem.Transfer(FormatID.wiaFormatJPEG);

    // Save the image in some path with filename
    var path = @"C:\Users\<username>\Desktop\scan.jpeg";
    if (File.Exists(path)) File.Delete(path);
    imageFile.SaveFile(path);
}
catch (COMException e) 
{
    // Convert the error code to UINT; to compare with the error codes of the table in MSDN
    uint errorCode = (uint)e.ErrorCode;

    // See the error codes
    if (errorCode ==  0x80210006)
    {
        Console.WriteLine("The scanner is busy or isn't ready");
    }
    else if(errorCode == 0x80210064)
    {
        Console.WriteLine("The scanning process has been cancelled.");
    }
    else if(errorCode == 0x8021000C)
    {
        Console.WriteLine("There is an incorrect setting on the WIA device.");
    }
    else if(errorCode == 0x80210005)
    {
        Console.WriteLine("The device is offline. Make sure the device is powered on and connected to the PC.");
    }
    else if(errorCode == 0x80210001)
    {
        Console.WriteLine("An unknown error has occurred with the WIA device.");
    }
}
```

```cs - WIA properties
// however to make image won't be incompleted on most of the scanners; we need to set some of the common properties of the scanner
// modifiable properties of WIA like the scanning width and height, the color mode, ....
// however WIA has a lot of properties constants that we can modify and that may be or not available on different scanning devices

/// <summary>
/// Adjusts the settings of the scanner with the providen parameters.
/// </summary>
/// <param name="scannnerItem">Scanner Item</param>
/// <param name="scanResolutionDPI">Provide the DPI resolution that should be used e.g 150</param>
/// <param name="scanStartLeftPixel"></param>
/// <param name="scanStartTopPixel"></param>
/// <param name="scanWidthPixels"></param>
/// <param name="scanHeightPixels"></param>
/// <param name="brightnessPercents"></param>
/// <param name="contrastPercents">Modify the contrast percent</param>
/// <param name="colorMode">Set the color mode</param>
private static void AdjustScannerSettings(IItem scannnerItem, int scanResolutionDPI, int scanStartLeftPixel, int scanStartTopPixel, int scanWidthPixels, int scanHeightPixels, int brightnessPercents, int contrastPercents, int colorMode)
{
    const string WIA_SCAN_COLOR_MODE = "6146";
    const string WIA_HORIZONTAL_SCAN_RESOLUTION_DPI = "6147";
    const string WIA_VERTICAL_SCAN_RESOLUTION_DPI = "6148";
    const string WIA_HORIZONTAL_SCAN_START_PIXEL = "6149";
    const string WIA_VERTICAL_SCAN_START_PIXEL = "6150";
    const string WIA_HORIZONTAL_SCAN_SIZE_PIXELS = "6151";
    const string WIA_VERTICAL_SCAN_SIZE_PIXELS = "6152";
    const string WIA_SCAN_BRIGHTNESS_PERCENTS = "6154";
    const string WIA_SCAN_CONTRAST_PERCENTS = "6155";
    SetWIAProperty(scannnerItem.Properties, WIA_HORIZONTAL_SCAN_RESOLUTION_DPI, scanResolutionDPI);
    SetWIAProperty(scannnerItem.Properties, WIA_VERTICAL_SCAN_RESOLUTION_DPI, scanResolutionDPI);
    SetWIAProperty(scannnerItem.Properties, WIA_HORIZONTAL_SCAN_START_PIXEL, scanStartLeftPixel);
    SetWIAProperty(scannnerItem.Properties, WIA_VERTICAL_SCAN_START_PIXEL, scanStartTopPixel);
    SetWIAProperty(scannnerItem.Properties, WIA_HORIZONTAL_SCAN_SIZE_PIXELS, scanWidthPixels);
    SetWIAProperty(scannnerItem.Properties, WIA_VERTICAL_SCAN_SIZE_PIXELS, scanHeightPixels);
    SetWIAProperty(scannnerItem.Properties, WIA_SCAN_BRIGHTNESS_PERCENTS, brightnessPercents);
    SetWIAProperty(scannnerItem.Properties, WIA_SCAN_CONTRAST_PERCENTS, contrastPercents);
    SetWIAProperty(scannnerItem.Properties, WIA_SCAN_COLOR_MODE, colorMode);
}

/// <summary>
/// Modify a WIA property
/// </summary>
/// <param name="properties"></param>
/// <param name="propName"></param>
/// <param name="propValue"></param>
private static void SetWIAProperty(IProperties properties, object propName, object propValue)
{
    Property prop = properties.get_Item(ref propName);
    prop.set_Value(ref propValue);
}
```

## Demo 2: List scan device for user + Convert image to different format + Save to custom path
* https://github.com/ourcodeworld/csharp-scanner-wia/blob/master/ScannerDemo/Form1.cs

==================================================================================
# TWAIN

## using 'NTWAIN' for .NET application

* -> Issue: in nhiều trang in ADF mode of HP - https://h30434.www3.hp.com/t5/Scanning-Faxing-Copying/Compatibility-Issues-between-HP-TWAIN-driver-and-NTWAIN-open/td-p/9066955