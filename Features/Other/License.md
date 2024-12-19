===============================================================
# License
* -> the license is **`a plain text XML file`** that contains details such as **the product name, number of developers it is licensed to, subscription expiry date and so on**
* -> the file is **`digitally signed`** - so **don't modify the file**; even inadvertent addition of an extra line break into the file will **invalidate it**

===============================================================
> https://docs.aspose.com/cells/net/licensing/#:~:text=The%20easiest%20way%20to%20set,file%20through%20its%20path%20Aspose

# Apply a License in 'Aspose' component
* -> we need to **set a license before utilizing Aspose.Cells** if we want to avoid its **`evaluation limitation`**
* -> it is only required to **set a license once per application (or process)**
* -> the license can be loaded from a **file**, **stream** or an **embedded resource**

===============================================================
# Use a ".linq" file as a 'licensing setup script' for a product

## Example: License for GdPicture.NET SDK
* -> the script generates and **stores a computer-specific license key** in the **`Windows Registry`**, **`associating` it with the GdPicture.NET library**
* -> this ensures that the SDK **`can be used only on the intended machine`**, acting as **a form of activation** or **licensing control**

```cs
SetDeveloperKey("211883860501001421116010749430779"); 

void SetDeveloperKey(string licenseKey)
{
    // Generates a unique key based on the MAC address of the machine
    var computerSpecificKey = "GdPicture.NET14" + GetMacAddress();

    // Encrypts the provided license key (using a custom XOR-based cipher and the generated key)
    var base64 = XorCrypt(computerSpecificKey, Encoding.ASCII.GetBytes(licenseKey));

    // stores the encrypted key and edition information in the Windows Registry 
    // under the paths relevant to GdPicture.NET
    var registryKey = GetRegistryKey();
    if (registryKey == null) registryKey = Registry.CurrentUser.CreateSubKey("Software\\Orpalis\\GdPicture.NET14", true);
    registryKey.SetValue("CoreKey", Convert.ToBase64String(base64));
    registryKey.SetValue("Edition", "GdPicture.Net Document Imaging SDK Ultimate V14");
}

// MAC address - a unique hardware identifier for the machine
// ensure the license is bound to a particular machine
string GetMacAddress()
{ 
    var text = string.Empty;
    using (var managementClass = new ManagementClass("Win32_NetworkAdapterConfiguration"))
    {
        foreach (var managementObject in managementClass.GetInstances())
        {
            if (managementObject["MacAddress"] == null)
                continue;

            if (managementObject["IpEnabled"] == null || !(bool)managementObject["IpEnabled"])
                continue;

            var text2 = managementObject["MacAddress"].ToString().Trim().ToUpper().Replace(":", "");
            if (string.IsNullOrEmpty(text))
                text = text2;
            if (text.Replace("00", "").Length <= text2.Replace("00", "").Length)
                text = text2;
        }
    }
    return text;
}

// locates the registry key for storing license informatio
RegistryKey GetRegistryKey()
{
    var registryKey = Registry.CurrentUser.OpenSubKey("Software\\Orpalis\\GdPicture.NET14", true) ??
                      Registry.CurrentUser.OpenSubKey("Software\\Wow6432Node\\Orpalis\\GdPicture.NET14", true) ??
                      Registry.LocalMachine.OpenSubKey("Software\\Orpalis\\GdPicture.NET14", true) ??
                      Registry.LocalMachine.OpenSubKey("Software\\Wow6432Node\\Orpalis\\GdPicture.NET14", true);
    return registryKey;
}

byte[] XorCrypt(string passPhrase, byte[] inputData)
{
    var array = new byte[256];
    var swapIndex1 = 0;
    if (string.IsNullOrEmpty(passPhrase))
    {
        throw new ArgumentNullException(nameof(passPhrase));
    }
    if (passPhrase.Length > 256)
        passPhrase = passPhrase.Substring(0, 256);
    var passPhraseArray = Encoding.GetEncoding(1252).GetBytes(passPhrase);
    // Initialize array with values 0 to 255
    for (var i = 0; i <= 255; i++)
        array[i] = (byte)i;
    // Swap the values around based on the licensekey
    for (var i = 0;i <= 255; i++)
    {
        swapIndex1 = (swapIndex1 + array[i] + passPhraseArray[i%passPhrase.Length])%256;
        // Swap
        var b = array[i];
        array[i] = array[swapIndex1];
        array[swapIndex1] = b;
    }
    swapIndex1 = 0;
    var swapIndex2 = 0;
    var output = new byte[inputData.Length - 1 + 1];
    for (var i = 0; i <= inputData.Length - 1; i++)
    {
        swapIndex1 = (swapIndex1 + 1)%256;
        swapIndex2 = (swapIndex2 + array[swapIndex1])%256;
        // Swap
        var b = array[swapIndex1];
        array[swapIndex1] = array[swapIndex2];
        array[swapIndex2] = b;
        output[i] = (byte) (inputData[i] ^ array[(array[swapIndex1] + array[swapIndex2])%256]);
    }
    return output;
}
```

```cs - Apply lincense to code
GdPicture14.LicenseManager oLicenseManager = new GdPicture14.LicenseManager();
oLicenseManager.RegisterKEY("211883860501001421116010749430779");
```