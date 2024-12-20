
# Decompile code from an .msi (by Winforms) file

## Step
* -> đầu tiên là chạy (extract) file .msi + để ý đường dẫn nó dùng để cài đặt là gì
* -> mở DotPeek lên (JetBrain tool để decompile code C# .NET) -> chọn File -> Open -> chọn đường dẫn ta cài đặt app
* -> chọn file .exe (tên file là tên Winform project) -> OK
* -> giờ mở ra xem

## Note
* -> dotPeek can generate **.pdb** (Program Database) files for compiled assemblies, **`allowing us to debug the assembly`** with meaningful stack traces in Visual Studio
* -> **`assess how effective our obfuscation or code-protection techniques`** are by testing them with dotPeek
* -> there is also **Legal Concerns**, avoid decompiling code that violates licensing agreements or intellectual property rights.