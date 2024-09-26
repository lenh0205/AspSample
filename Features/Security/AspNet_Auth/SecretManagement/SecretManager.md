
# Safe storage of app secrets in development in ASP.NET Core
* -> the **Secret Manager tool** **`stores sensitive data`** during **`application development`**
* -> in this context, **a piece of sensitive data** is **`an app secret`**
* -> the **app secrets** are stored in **`a separate location from the project tree`**
* -> the **app secrets** are associated with **`a specific project or shared across several projects`**
* -> the **app secrets** **`aren't checked into source control`**

* -> the **Secret Manager tool** **`doesn't encrypt the stored secrets`** and **`shouldn't be treated as a trusted store`**
* => it's for development purposes only
* -> the _keys and values_ are **stored in a JSON configuration file** in **`the user profile directory`**