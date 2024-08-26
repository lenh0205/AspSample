==============================================================================

# Authentication and Authorization

## Authentication
* -> determines **`whether users are who they claim to be`**
* -> challenges the user to **validate credentials** (_for example: through passwords, answers to security questions, or facial recognition_)
* -> usually done **`before authorization`**
* -> generally, transmits info through an **ID Token**
* -> generally governed by the **OIDC - OpenID Connect protocol**

```r - Example:
Employees in a company are required to authenticate through the network before accessing their company email
```

## Authorization
* -> determines what users **`can and cannot access`**
* -> verifies whether **`access is allowed`** through **policies and rules**
* -> usually done **`after successful authentication`**
* -> generally, transmits info through an **Access Token**
* -> generally governed by the **OAuth 2.0 framework**

```r - Example
After an employee successfully authenticates, the system determines what information the employees are allowed to access
```
