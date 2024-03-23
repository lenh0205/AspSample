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
* -> _a permission_ is a **declaration of an action** that can be **executed on a resource**
* -> a permission is something **`related to an object (a resource)`**, **not to the user of that resource**

# Privileges
* _privileges_ are **assigned permissions to the user** 
* -> hiểu đơn giản thì, **`resources expose permissions`** and **`users have privileges`**
* -> when we assign a permission to a user, we are **`granting them a privilege`**

# Authorization Scenarios
* _there're 2 common authorization scenario:_
* -> first scenario is the **`typical and most known authorization scenario`** - corresponds to the situation where a **user attempts to access a protected resource** for **which they have privileges**
* -> second scenario is known as a **`delegated authorization scenario`** - represents the case in which a **third-party application** wants to **`access a protected resource`** on **behalf of a user**

* the **delegated authorization** scenario 
* -> is the **`typical scenario`** introduced by the **OAuth 2 Authorization Framework**
* -> this framework allows **`a third-party application`** to **`operate on a resource`** on **`behalf of a user`**
* -> in other words, the **application exercises the user's privilege** to **`use a resource`**

* => for **`an application`** to be **`delegated to access the user’s resource`**, we need **Scopes**

```r - fictions examples of 2 scenarios
// The concierge of a hotel is the entity that supervises the hotels rooms (the resources) from unauthorized access
// Each room has the ability to accommodate one guest (permission)
// A customer is assigned one room, so they have the privilege of being accommodated in that room

// ---> senario 1:
// When the customer goes for a walk and then comes back to the hotel
// the concierge checks if actually the customer has been assigned the permission to be accommodated in that room by consulting the booking list. 
// If everything fits, the customer can come in, and we are all happy

// ---> senario 2:
// one day, while the customer is out, a person shows up at the concierge desk. That person says they were requested by the customer to come into the room to get their briefcase
// The concierge doesn nott allow them to come in, of course. They need to do their own verifications to confirms what that person is asserting, such as calling the customer on the phone, for example
// however, even after everything is clear, the concierge needs to make sure that person will only do what they came to the hotel to do: take the customers briefcase. Nothing else. 
// The concierge will likely walk the person to the customer room and make sure nothing else is done other than what was agreed upon

// => the first scenario looks quite straightforward: it reduces the access control to comparing the customer booking data with the hotel booking list
// => the second scenario is a bit more complex: it requires an additional check on what the person declares and control on what they are allowed to do
```

# Scopes
* _Scopes_ enable a mechanism to **define what an application can do on behalf of the user**
* -> typically, scopes are **`permissions of a resource`** that the **`application wants to exercise on behalf of the user`**
* -> the **`application`** requests these scopes to the **`authorization server`** (_the server responsible for authorizing the third-party application in the **delegated scenario**_)
* -> Scopes were introduced in and specific to **OAuth 2.0 framework**

* _in practical terms, **`scopes`** are **strings that represent what the application wants to do** on **`behalf of the user`**_
```r - Example: "scope" shown in the following authorization request
https://YOUR_DOMAIN/authorize?
  response_type=code&
  client_id=YOUR_CLIENT_ID&
  redirect_uri=https://YOUR_APP/callback& 
  scope=read:email&   #👈 here is the requested scope
  audience=YOUR_API_AUDIENCE&
  state=YOUR_STATE_VALUE    
```

* _actually, it is **not the authorization server** that **`allows the application to exercise the user's privileges`**_
* -> the final word on **`granting delegated access to the application`** **is the user's** own
* -> the user can approve or reject **`delegated access to their resource`** with **`specific scopes`** by using a **consent screen** 
* -> nói dễ hiểu tức là authorization server không thể tự động cấp privilege của user, mà nó sẽ cần hiển thị 1 cái màn hình để user có thể chọn đồng ý hoặc không

* the granted scopes allow the application to access **`only that user's resources`**, **`cannot access other users' resources`**
* => _usually, the **`scopes granted to a third-party application`** are a **subset of the permissions granted to the user**_

* _however, **`an application can request scopes`** corresponding to **privileges that the user doesn't have**; and the **`user can grant them`**_
* -> this covers situations where the **`user doesn't have a given privilege`** in the moment they **grant the scopes to the application**
* -> but they **will have that privilege** when the application **`exercises that scope`**
* _nói dễ hiểu tức là lúc hoàn thành đăng nhập user chỉ cấp quyền "read" cho app, nhưng sau này khi app cần "write", nó có thể hiện màn hình "consent" để hỏi user cấp thêm quyền "write"_

* -> the vice versa can happen too: **`a user has a given privilege`** and **`grants a delegated access for the corresponding scope`**, but the user **loses the privilege when the application tries to exercise its scope**
* => what is relevant when the application exercises its scopes is the possession of the corresponding privileges by the user at that time
* => this means that the **application can't do more than the user can do**

* once the **`user approves the application's request`**
* -> the **authorization server communicates the granted scopes to the application** 
* -> so that it can **access the resource with the limited granted privileges**

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