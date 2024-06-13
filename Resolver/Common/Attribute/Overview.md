# Attribute (or "Annotation" in Java) 
* -> **`associating metadata, or declarative information`** with code (assemblies, types, methods, properties, and so forth)
* -> after an attribute is associated with a program entity, the **`attribute can be queried at runtime`** by using a technique called **Reflection**

## Technical
* -> the **System.Attribute** class is an **`abstract class`** defining the required services of **any attribute**
* -> the **`C# compiler`** itself has been programmed to **discover the presence of numerous attributes** during the **`compilation process`**
```cs   
// if the csc.exe compiler discovers an item being annotated with the [obsolete] attribute, it will display a compiler warning in the IDE error list
```

==========================================================================
# Predefined Attributes
* -> defined by Microsoft as a part of .NET FCL
* -> many of them receive **`special support from the C# compiler`**
* -> which implies that for those specific attributes, the compiler could **customize the compilation process in a specific way**
* -> some common predefined attributes: **`[Serialization]`** and **`[NonSerialization]`**, **`[WebMethod]`**, **`[DLLImport]`**, **`[CLSCompliant]`**

## 'AttributeUsageAttribute' attribute

## [Serialization] and [NonSerialization] attribute
* -> a class that can be persisted in a binary format using the [Serialization] attribute
* -> once the **`class has been compiled`**, we can **view the extra metadata** using the **ildasm.exe utility** (_a tool to display metadata level of elements_)
* -> these attributes are **recorded using the serializable token** and the field have [NonSerialization] attribute **is tokenized**

# Custom Attributes

## Step of Creating
* -> the **`custom attribute class`** should be **derived from System.Attribute** (_the Attribute name should suffixed by "Attribute"_)
* -> set the probable targets with the **AttributeUsage** attribute
* -> implement the **`class constructor`** and **`write-accessible properties`**
