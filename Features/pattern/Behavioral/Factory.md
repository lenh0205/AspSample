
===============================================================
# Arise
* -> in **`Dependency Injection`**, we typically **put our dependencies in the constructor**; when class is created, the dependencies get created and injected
* -> however, there're times we want to **create our dependencies more often or in a different way**
* -> one solution is **Factory pattern**

===============================================================
# Factory Design Pattern
* -> the pattern **`encapsulates object generation`** - separates clients from the creation process, allowing dynamic object instantiation based on runtime requirements
* -> enables us to **generate objects without defining the specific type of object** that will be created 
* => by using a **factory method** or **factory class**, it allows us to **`abstract away the instantiation process`** - resulting in a more modular and scalable solution

## Factory Method Pattern
* -> a _design pattern_ that **`provides an interface for creating objects`** while **`allowing subclasses to determine the type of objects to be made`**
* -> **bypassing the instantiation process to subclasses** encourages **`loose coupling`**

```r - Ex:
// consider "a logistics company" that uses a variety of "transportation methods" - such as trucks, ships, and planes
// the company may utilize a factory method to "generate the proper transportation object" depending on the "individual requirements"
// => allowing for "simple extension or modification" of transportation kinds without affecting the core logistics code
```

===============================================================
# Problems of not using the 'Factory Design Pattern'

## Tight coupling: 
* -> this occurs when our code is tightly bound to specific classes, making it **`impossible to change or extend the types of objects created without modifying`** the current code
* -> this can result in decreased flexibility and more maintenance effort

## Code Duplication:
* -> without a factory, **`object creation code is frequently duplicated across the program`**
* -> this redundancy can cause inconsistencies and problems in the code, making it more challenging to manage

## Difficulty in Testing: 
* -> Testing components based on specific implementations might be tricky
* -> using a factory allows us to **`simply change out implementations for testing`**, making our code more testable and reusable

## Scalability Issues:
* -> as our application expands, managing object creation directly in client code can become inefficient
* -> The Factory Design Pattern **`centralizes object generation`**, making it more scalable and maintainable

```cs
using System;

public abstract class Document
{
    public abstract void Open();
}

public class WordDocument : Document
{
    public override void Open()
    {
        Console.WriteLine("Opening a Word document.");
    }
}

public class PDFDocument : Document
{
    public override void Open()
    {
        Console.WriteLine("Opening a PDF document.");
    }
}

public class DocumentManager
{
    public void OpenDocument(string type)
    {
        Document doc;

        // the DocumentManager class uses conditional logic to create proper Document
        // -> this ties DocumentManager to specific document types 
        // -> and necessitates modification of the OpenDocument function whenever new document types are added
        if (type == "Word")
        {
            doc = new WordDocument();
        }
        else if (type == "PDF")
        {
            doc = new PDFDocument();
        }
        else
        {
            throw new ArgumentException("Invalid document type");
        }

        doc.Open();
    }
}

class Program
{
    static void Main()
    {
        DocumentManager manager = new DocumentManager();

        manager.OpenDocument("Word"); // Output: Opening a Word document.
        manager.OpenDocument("PDF");  // Output: Opening a PDF document.
    }
}
```

# Implementing the Factory Design Pattern

```cs - Example 1:
// Abstract Product
public abstract class Document
{
    public abstract void Open();
}

// Concrete Product 1
public class WordDocument : Document
{
    public override void Open()
    {
        Console.WriteLine("Opening a Word document.");
    }
}

// Concrete Product 2
public class PDFDocument : Document
{
    public override void Open()
    {
        Console.WriteLine("Opening a PDF document.");
    }
}

public class DocumentFactory
{
    public abstract Document CreateDocument(string type);
}

public class ConcreteDocumentFactory : DocumentFactory
{
    // centralizes the generation of Document objects
    public override Document CreateDocument(string type)
    {
        switch (type)
        {
            case "Word":
                return new WordDocument();
            case "PDF":
                return new PDFDocument();
            default:
                throw new ArgumentException("Invalid document type");
        }
    }
}

// Client Code
class Program
{
    static void Main()
    {
        try
        {
            // using Factory allowing the client code to request documents without knowing which classes are involved
            // simplifies object generation
            // increases flexibility by eliminating reliance on specific implementations
            // makes document types easier to manage and extend

            DocumentFactory documentFactory = new ConcreteDocumentFactory();

            // Use the factory to create a WordDocument
            Document wordDoc = documentFactory.CreateDocument("Word");
            wordDoc.Open(); // Output: Opening a Word document.

            // Use the factory to create a PDFDocument
            Document pdfDoc = documentFactory.CreateDocument("PDF");
            pdfDoc.Open(); // Output: Opening a PDF document.

            // Attempting to create an invalid document type
            Document invalidDoc = documentFactory.CreateDocument("Excel");
            invalidDoc.Open(); // This line will throw an exception
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine(ex.Message); // Output: Invalid document type
        }
    }
}
```
