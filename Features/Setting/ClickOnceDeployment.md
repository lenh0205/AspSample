# "ClickOnce" deployment
* a deployment technology - `deploy .NET applications` to a user's computer with a single click
* When building project in `"Release"` mode, the ClickOnce deployment feature is **automatically enabled**
* -> this feature creates the **"install" folder** and the two ".zip" files: `_Install.zip , _Source.zip`

* To disable `ClickOnce deployment` feature 
<PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ">
  <OutputPath>bin\Release\</OutputPath>
  <DefineConstants>TRACE;DEBUG</DefineConstants>
  <Optimize>true</Optimize>
  <WarningLevel>4</WarningLevel>
  <EnableClickOnce>false</EnableClickOnce> // thêm dòng này vào
</PropertyGroup>


 There are several ways to deploy .NET applications, each with its own advantages and disadvantages. The most common methods are:
* **ClickOnce deployment:** ClickOnce is a deployment technology that allows you to deploy your .NET applications to a user's computer with a single click. ClickOnce is easy to use and does not require any special configuration on the user's computer. However, ClickOnce is not as secure as other deployment methods and is not suitable for applications that require high security.
* **Web deployment:** Web deployment is a method of deploying your .NET applications to a web server. Web deployment is more secure than ClickOnce deployment, but it requires more configuration on the user's computer.
* **MSI deployment:** MSI deployment is a method of deploying your .NET applications using a Windows Installer package. MSI deployment is more secure than ClickOnce and web deployment, but it is also more complex to configure.
* **EXE deployment:** EXE deployment is a method of deploying your .NET applications using a self-extracting executable file. EXE deployment is easy to use and does not require any special configuration on the user's computer. However, EXE deployment is not as secure as other deployment methods.
The best deployment method for your .NET application will depend on the specific needs of your application. If you need a quick and easy way to deploy your application, then ClickOnce or EXE deployment may be a good option. If you need a more secure deployment method, then web deployment or MSI deployment may be a better choice.
Here is a table that summarizes the different deployment methods for .NET applications:
| Deployment Method | Advantages | Disadvantages |
|---|---|---|
| ClickOnce | Easy to use | Not as secure |
| Web deployment | More secure | Requires more configuration |
| MSI deployment | Most secure | More complex to configure |
| EXE deployment | Easy to use | Not as secure |
## Additional information about ClickOnce deployment
ClickOnce is a deployment technology that allows you to deploy your .NET applications to a user's computer with a single click. ClickOnce is easy to use and does not require any special configuration on the user's computer. However, ClickOnce is not as secure as other deployment methods and is not suitable for applications that require high security.
ClickOnce works by creating a self-contained application that includes all of the files that are needed to run your application. When a user clicks on the ClickOnce deployment file, the application is installed on
