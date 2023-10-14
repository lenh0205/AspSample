# MSBuild - Microsoft Build Engine
* `a platform` for `building applications` 
* provides an `XML schema` for a project file
* controls how `the build platform` processes and builds software.

* **Visual Studio** uses MSBuild (_but MSBuild doesn't depend on Visual Studio_) (_64-bit version of MSBuild is used starting with Visual Studio 2022_)
* -> to load and build `managed projects` (_.csproj, .vbproj, .vcxproj, and others_) contain `MSBuild XML code` that executes when build a project
* -> By invoking **msbuild.exe** or **dotnet build** (command of .NET SDK) on `project or solution file`, we can orchestrate and build products in environments where Visual Studio isn't installed.

## Lưu ý:
* **Element and attribute** names are case-sensitive. However, `property, item, and metadata names` are not
```
// item type Compile, comPile, or any other case variation, and gives the item type the value "one.cs;two.cs"
<ItemGroup>
  <Compile Include="one.cs" />
  <Compile Include="two.cs" />
</ItemGroup>
```

# Run MSBuild
```
// build the file MyProj.proj with the Configuration property set to Debug: 
MSBuild.exe MyProj.proj -property:Configuration=Debug
```
* we can set **properties**, execute specific **targets**, and set other options that control the `build process`

# Build Logs
* we can log `build errors, warnings, and messages` to the _console or another output device_

# Project file
* MSBuild uses an XML-based project file format
* -> that's straightforward and extensible
* -> describe the `items` that are to be built
* -> how to be built for different operating systems and configurations
* -> author `reusable build rules` that can be factored into separate files (_performed consistently across different projects in the product_)

* Visual Studio build system stores project-specific logic in the project file itself
* uses `imported MSBuild XML files` with extensions like **.props** and **.targets** to define the standard build logic
* -> **.props** files define `MSBuild properties`
* -> **.targets** files define `MSBuild targets`

## Properties
* represent key/value pairs that can be used to **configure builds**
* declared by creating an `element` that has the `name of the property`
* as a _child of a **PropertyGroup** element_

```
// property named "BuildDir" that has a value of "Build":
<PropertyGroup>
    <BuildDir>Build</BuildDir>
</PropertyGroup>
```
### "Condition" attribute
* The `contents` of conditional elements are ignored unless the condition evaluates to `true`
* Properties can be **referenced** throughout the project file by using the syntax **`$(<PropertyName>)`** (_VD: $(Configuration)_)
```
// "Configuration" property is defined if it hasn't yet been defined
<Configuration  Condition=" '$(Configuration)' == '' ">DefaultValue</SomeProperty>
```
## Items
* **inputs** into the `build system` and typically represent **files**
* `Items` are grouped into **item types** based on user-defined item names

* **item types** can be used as `parameters` for tasks
* -> use the `individual items` to perform the `steps` of the build process.

* declared in the project file by creating an element that has the `name of the item type`
* as a child of an **ItemGroup** element

* `Item types` can be **referenced** throughout the project file by using the syntax **`@(<ItemType>)`** (_VD: @(Compile)_)

```
// creates an "item type" named "Compile", which includes two files:
<ItemGroup>
    <Compile Include = "file1.cs"/>
    <Compile Include = "file2.cs"/>
</ItemGroup>
```

## Tasks
* **units of executable code** that MSBuild projects use to perform `build operations`
* a task might _compile input files_ or _run an external tool_

* Tasks can be **`reused`**, and they can be shared by different developers in different projects

### Supported Task
* `MSBuild` includes **common tasks** that you can modify to suit our requirements
* -> **Copy** - copies files, 
* -> **MakeDir** - creates directories
* -> **Csc** - compiles Visual C# source code files. 

### Declare Task 
* A task is `executed` in an `MSBuild project file` by creating an element that has the `name of the task`
* as a child of a **Target** element

* Tasks typically accept **parameters**, which are passed as **attributes of the element**
* -> both `MSBuild properties and items` can be used as parameters
```
// following code calls the "MakeDir" task and passes it the value of the "BuildDir" property:
<Target Name="MakeBuildDirectory">
    <MakeDir  Directories="$(BuildDir)" />
</Target>
```

## Targets
* `Targets group tasks` together in a particular order and expose sections of the project file as **entry points** into the build process. 
* **Targets** are `often grouped` into logical sections -> increase readability and to allow for expansion

* Breaking the `build steps` into `targets` lets you call one piece of the build process from other targets without copying that section of code into every target

* `Targets` are declared in the project file by using the **Target** element
```
// creates a "target" named "Compile", which then calls the "Csc" task that has the "item" list that was declared in the earlier example:
<Target Name="Compile">
    <Csc Sources="@(Compile)" />
</Target>
```

### Other Ablility
* describe `relationships` among one another + perform `dependency analysis`
* => whole sections of the build process can be skipped if that target is up-to-date

### Mechanism
* The `execution logic of a task` is written in managed code and mapped to MSBuild by using the `UsingTask` element
* we can write your own task by authoring a managed type that implements the `ITask` interface

## Include Dependencies
* **`PackageReference`** - used to specify **NuGet package dependencies** directly within the project file (_instead of having a separate `packages.config` file_)
* **`Reference`** - used to include an assembly reference (_system assembly, assembly that is part of another project, ..._) in the project

## Exclude Resource
* use **None** element to specifies files and folders to exclude from the project 
* => `files and folders` specified in this element are `not copied to the output directory` and are `not included in the assembly`
```xml
<!-- excludes the "Components" folder from the project: -->
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="15.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFrameworkVersion>v4.5.2</TargetFrameworkVersion>
  </PropertyGroup>
  <ItemGroup>
    <None Include="Components\**\*">
      <CopyToOutputDirectory>Never</CopyToOutputDirectory>
    </None>
  </ItemGroup>
</Project>
```

* use the **Condition** attribute - allows to specify conditions under which to include or exclude resources
```xml
<!-- excludes the "Components" folder from the project only in the Release configuration -->
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="15.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFrameworkVersion>v4.5.2</TargetFrameworkVersion>
  </PropertyGroup>
  <ItemGroup>
    <Content Include="Components\**\*">
      <CopyToOutputDirectory>Always</CopyToOutputDirectory>
      <Condition>'$(Configuration)' == 'Debug'</Condition>
    </Content>
  </ItemGroup>
</Project>
```
