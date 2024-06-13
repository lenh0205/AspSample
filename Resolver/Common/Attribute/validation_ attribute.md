
==========================================================================
# Creating custom validation "Attribute" 
* -> in case there are some requirements to validate the input for but **`built-in attributes did not work`**
* -> create metadata that we can **`use in the data model to validate data fields`**
* => logic will reside in one place, and attributes can be **reused**
* => define validation rules for the data models and **automatically trigger validation during the data binding process**

## Step
* -> create a class **`inherit`** from **ValidationAttribute** **`abstract class`** (_it base class for validation attributes like `RequiredAttribute`, `CompareAttribute`, `MaxLengthAttribute`_)
* -> **`overriding`** **IsValid** method of _ValidationAttribute_ to write custom validation logic
* -> the method accept an **object** type parameter to **`get the value passed to the field`** we are validating
* -> taking advantage of the **ErrorMessage** field in our _ValidationAttribute class_ so we can **`return a error message to our user`**
* -> hoặc ta cũng có thể **`override`** **FormatErrorMessage** (nhận tham số là **`Tên của phần tử`**); kết quả trả về sẽ dùng làm lỗi trả về khi **`isValid trả về false`**
* -> Add the Attribute to the field we want to validate


```cs - creating an "Attribute" that validates file extension
// -> ensure a model that contains an "IFormFile" field only accept certain file type

using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Collections.Generic;

namespace BookApp.CustomValidations
{
    // Define Custom Attribute
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class CustomFileExtensionValidation : ValidationAttribute
    {
        public string Extensions { get; set; }
        public CustomFileExtensionValidation()
        {
            //set a default error message if no error message is passed into the attribute
            ErrorMessage = "Invalid file extension(s)";
        }

        public override bool IsValid(object value) // get the uploaded file
        {
            if (value == null) return true;
            if (value is IFormFile file) return ValidateFile(file);
            if (value is List<IFormFile> files) return ValidateFiles(files);

            return false;
        }

        // validating a single file
        private bool ValidateFile(IFormFile file)
        {
            if (file == null ||string.IsNullOrEmpty(file.FileName))
            { // File is null or empty, return false indicating validation failure
                return false;
            }
            if (!string.IsNullOrEmpty(Extensions))
            { //check if the file type uploaded matches any of the extensions defined
                var allowedExtensions = Extensions
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(ext => ext.Trim())
                    .ToList();

                var fileName = file.FileName;

                var isValid = allowedExtensions.Any(
                    ext => fileName.EndsWith(ext, StringComparison.OrdinalIgnoreCase));

                return isValid;
            }
            // File is valid
            return true;
        }

        // validating multiple files
        private bool ValidateFiles(List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            { // Value is null or empty, return false indicating validation failure
                return false;
            }

            if (!string.IsNullOrEmpty(Extensions))
            {
                var allowedExtensions = Extensions
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(ext => ext.Trim().ToLower())
                    .ToList();

                bool isValid = files.All(file =>
                {
                    if (file == null || string.IsNullOrEmpty(file.FileName))
                    { // File is null or empty, return false indicating validation failure
                        return false;
                    }
                    var fileName = file.FileName.ToLower();
                    return allowedExtensions.Any(ext => fileName.EndsWith(ext));
                });
                return isValid;
            }
            // All files are valid
            return true;
        }
    }
}

// Usage:
namespace BookApp.Dtos
{
   public class SignUpDTO
   {
        [Required(ErrorMessage = "Email address is required.")]
        public string EmailAddress { get; set; }
        [Range(18,50, ErrorMessage ="The minimum age to be considered for this job is {1} and the maximum is {2}")]
        public int Age { get; set; }

        //Our custom validation attribute
        [CustomFileExtensionValidation(Extensions = "png,jpg,jpeg", ErrorMessage = "Only files with the extensions png, jpg, and jpeg are accepted.")]
        public IFormFile ProfilePicture { get; set; }
    }
}
```

## In Console Application
* -> in **`ASP.NET, MVC or ASP.NET Web API provides mechanisms to perform model validation`** using **attributes, annotations, or validation libraries**
* -> in Console App, we will need to do this manually using **ValidationContext** and **Validator**

```cs
public bool CreateEmployee()
{
    var emp = new Employee()
    {
        Email = "test@example.com"
    };
    var validationContext = new ValidationContext(emp);
    var validationResults = new List<ValidationResult>();

    bool isValid = Validator.TryValidateObject(emp, validationContext, validationResults, true);

    if (!isValid)
    {
        foreach (var result in validationResults)
        {
            Console.WriteLine(result.ErrorMessage);
        }
        return false;
    }
    // Create Employee in DB
    return true;
}
```