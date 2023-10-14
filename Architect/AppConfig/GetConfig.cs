
## Access the "appSettings" section of the "Web.config" file in an ASP.NET app (.NET framework)
// Cách 1: more general and not tied to the "System.Web" namespace 
// -> make code more portable if decide to move parts of it outside of an ASP.NET context
// -> "System.Configuration.ConfigurationManager"
System.Configuration.ConfigurationManager.AppSettings["Environment"];

// Cách 2: suitable if working with Web-specific features like URL authorization and membership providers
// -> "System.Web.Configuration.WebConfigurationManager"
System.Web.Configuration.WebConfigurationManager.AppSettings["Environment"]";
