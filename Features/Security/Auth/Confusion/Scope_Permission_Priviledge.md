> What is the difference between **permissions**, **privileges**, and **scopes** in the **`authorization context`**?
> behaft of user ?

========================================================
# Actors of Access Control
* **access control** is the **`selective restriction`** of **`access to a resource`**; determines who can do what on a resource

* -> the **user** - the **`entity`** that wants to **`perform an action`** on an object
* -> the **resource** - the **`object`** that a **`user wants to use`**
* -> the **application** - the software that **`mediates`** the interaction **`between the user and the resource`**
* => the `relationship between` the three actors described above contributes to the _complexity of access control_

# Permissions
* -> is a **declaration of an `action` that can be executed on a `resource`**
* _a permission is related to resource not to the user_

# Privileges
* -> are **`assigned permissions`** to a **`user` or `application`**

# Scopes
* -> enable a mechanism in OAuth2.0 to **define what an `application` can do `on behalf of the user`**
* -> by requests these scopes to the authorization server
* => the scopes granted to a third-party application are a subset of the permissions granted to the user

# Delegated Authorization
* -> the case in which **`a third-party application`** wants to access a protected resource on **`behalf of a user`**
* -> this is the typical scenario introduced by the **`OAuth 2 Authorization Framework`**
* _the **application** exercises the **`user's privilege`** to access a **resource** that require **`permissions`**

# IMPORTANT
* -> however, an application can request **`scopes corresponding to privileges that the user doesn't have`** and the **`user can grant them`**
* -> this covers situations where the **`user doesn't have a given privilege`** in the moment they **grant the scopes to the application** but **`the user will have that privilege when the application exercises that scope to resources`**
* -> the vice versa can happen too, **`a user has a given privilege`** and **grants a delegated access for the corresponding scope**, but **`the user loses the privilege when the application tries to exercise its scope`**

* => this means that, **`on the resource side users' privileges must be checked even in the presence of granted scopes`** (_scopes should not be considered application's privileges_)

```r - Example: "scope" shown in the following authorization request
https://YOUR_DOMAIN/authorize?
  response_type=code&
  client_id=YOUR_CLIENT_ID&
  redirect_uri=https://YOUR_APP/callback& 
  scope=read:email&   #👈 here is the requested scope
  audience=YOUR_API_AUDIENCE&
  state=YOUR_STATE_VALUE    
```

========================================================
# A Complicated Relationship - most common misunderstandings

## Scopes and privileges

* developers often think that **`scopes are application's privileges`** because after all, the **`user granted their consent to use them`**
* -> however it's missing a little detail, **`applications are authorized`** to exercise those privileges **on behalf of the user**
* => if the **`user doesn't have a privilege`**, the **`application cannot exercise it`**

* in a **`delegated authorization scenario`**, the **`application`** may **act on behalf of the user even when the user is not logged in**
* => if the **`user no longer has those privileges`** between their **`consent and the exercise by the application`**, the **`application must be prevented from exercising its delegated access`**
* => on the resource side, **users'privileges must be checked even in the presence of granted scopes**

* => **scopes should not be considered application's privileges**

## Scopes and permissions
* we might have an **`assumption`** that there is a natural **mapping between permissions and scopes**
* -> _tức là nếu 1 resource có permission A và B, thì sẽ tồn tại scope X và Y để application exercise những permissions đó on resource and vice versa_
* -> while in **`most cases permissions are exposed as scopes`**, this mapping is not strictly correct for a few reasons
* -> in a **`delegated scenario`**, **not all the permissions** on a resource should necessarily be made available to **`be requested as scopes`**
* -> in some case, we **should reserve some permissions for the user and decide that they are not delegable**

```r - For example
// we could decide that the "permission to delete" a document is "never delegated" to a third-party application
// tức là nếu muốn xoá ta có thể mở 1 "consent screen" để user trực tiếp quyết định có nên xoá không
```

* the assumption that there's a **mapping between scopes and privileges** is **`not accurate`** either
* -> _tức là ngoại trừ những permission not assigned to the user, nếu tồn tại privilege A thì sẽ luôn tồn tại scope X tương ứng_
* -> there are **scopes that don't have a match among the resource's permissions or the user's privileges**
* -> consider the **openid scope** defined by **`OpenID Connect specifications`** - it is a request for the **`authorization server`** to **`return an ID token`** as the result of the user's authentication
* -> this scope **`does not correspond to any permission on a specific user's resource`**
* -> the same applies to the other **`scopes defined by OpenID Connect`**: **profile**, **email**, **address**, and **phone**