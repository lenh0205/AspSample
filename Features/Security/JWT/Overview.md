> **simplicity**, **compactness** and **usability** are **`key features`** of JWT architecture
> a standard for safely **passing claims** in space constrained environments; được sử dụng trong **`all major web frameworks`**

# Claims
* -> are _definitions_ or _assertions_ made about a certain party or object
* -> JWT standardize certain claims that are useful in the context of some common operations, defined as part of the **`JWT spec`**;  others are **`user defined`** 

```r
// For example, one of these common operations is establishing the "identity of certain party"
// so one of the standard claims found in JWTs is the "sub" (from “subject”) claim
```

# the Pratical Application
* Authentication
* Authorization
* Federated identity
* Client-side sessions (stateless sessions)
* Client-side secrets

# The idea
* the **JOSE** (**`JSON Object Signing and Encryption group`**) group's objective was to **standardize** 
* -> the mechanism for **integrity protection** (_signature and MAC_) and **encryption**
* -> as well as the **format for keys** and **algorithm identifiers** to support interoperability of security services for protocols that use JSON

* => later become the **JWT, JWS, JWE, JWK and JWA** RFCs

# JWE, JWS
* _JWTs, by virtue of JWS and JWE, can provide various types of signatures and encryption_
* -> **`Signatures`** are useful to validate the data against **tampering** 
* -> **`Encryption`** is useful to protect the data from being **read by third parties**

