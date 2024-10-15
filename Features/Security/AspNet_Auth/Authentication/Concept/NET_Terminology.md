> with the rise of `ASP.NET Core over ASP.NET 4.x`, the built in authentication has undergone a shift from **role-based access control (RBAC)** to **claim-based access control (CBAC)**
> the most notable change is the **`User` property on `HttpContext`** is now of type **ClaimsPrincipal** instead of **IPrincipal**

> ClaimIdentity và ClaimPrincipal mặc định được sử dụng trong cơ chế Auth của .NET? đều được sử dụng trong cả Cookie-based và Token-based? còn ở trong ASP.NET Core Identity thì sao ?

https://learn.microsoft.com/en-us/dotnet/api/system.security.claims.claimsprincipal?view=net-8.0
https://learn.microsoft.com/en-us/dotnet/api/system.security.claims.claimsprincipal.claims?view=net-8.0
https://stackoverflow.com/questions/32584074/whats-the-role-of-the-claimsprincipal-why-does-it-have-multiple-identities

=======================================================================
# Claim
* -> are **name-key values** and are represented via the **System.Security.Claim** class
* -> _a claim_ is simply **`a piece of information about a subject`** (_include user, application, service, device_); a claim **does not dictate what a subject can, or cannot do**
* => claims are the foundation behind **`claims-based authentication`** 

```cs
public class Claim {
  public string Type { get; } // used to identify the claim
  public string Value { get; } // holds the data of the claim

  // if the data type of Value is not a string then the ValueType property can be set 
  // so the claim consumer knows how to interpret the Value
  public string ValueType { get; } 

  // .... some properties have been omitted
}
```

```cs - Ex: 
// represent a claim for a user's Id 
Claim idClaim = new Claim("Id", "1", "Integer");

// use for a user’s birthday
Claim dobClaim = new Claim("dob", "04/20/1969", "Date");
```

=======================================================================
# Identities - ClaimsIdentity
* -> **`claims representing the same subject`** can be **`grouped together`** and placed in **a ClaimsIdentity**

```cs
public class ClaimsIdentity {
  public string Name { get; }
  public IEnumerable<Claim> Claims { get; }

  // holds the authentication method used (such as "Bearer" or "Basic")
  public string AuthenticationType { get; }

  // returns true as long as "AuthenticationType" is not null
  public bool IsAuthenticated { get; }

  // .... some properties have been omitted
}
```

```cs - Ex: Usage
// For example, we we’re working on an API where "users" are identified via their "Id" and "Name"
// -> after validating a bearer token (JWT, etc…) recieved from the user,
// -> we could create a ClaimsIdentity to represent them:

ClaimsIdentity userIdentity = new ClaimsIdentity(
  new Claim[] {
    new Claim("Id", "1"),
    new Claim("Username", "Bert")
  },
  "Bearer" // => userIdentity.IsAuthenticated == true since we passed "Bearer" as AuthenticationType
);
```

=======================================================================
# Principals
* -> **a ClaimsIdentity** is convenient for **representing a subject via a collection of claims** but what if we want to **`assign more than one identity to a subject`**
* -> there's case that we might want **seperate into different identities** - this allows us to **`indicate that each can exist without the other`**
* -> then we can use **`ClaimPrincipal`** to **express the relation between these indentities**
* => **a principal object** represents the **`security context of the user`** on whose behalf the code is running (_including that `user’s identity`_)

* -> _ClaimsPrincipal_ provides a handful of **helper methods / properties to check things** (_such as if a claim exists using **HasClaim()**_) in **`any of the associated identities`**
* -> when working **within an API controller** in ASP.NET we can **`access the current principal`** via the **`User` property**

```r - Example:
// returning back to our previous example of the API, 
// what would happen if we wanted to also "identify the device being used by the user" to ensure it is whitelisted
// -> sure we could add a new Claim with device IP address, or agent string to the user identity, 

// but what if the user accesses the API from more than one device?
// -> to handle this use case without "duplicating user information" we’d be best off to "create a new identity to represent the device" that holds the IP address, and agent string.
```

```cs - seperating the "user claims" from the "device claims" into two seperate identities
public class ClaimsPrincipal 
{
  public IEnumerable<Claim> Claims { get; }
  public IEnumerable<ClaimsIdentity> { get; }
  public ClaimsIdentity Identity { get; }

  // .... some properties have been omitted
}
```

```cs - using a ClaimsPrincipal
// -> group the "user identity" and "device identity" into one context 
// -> without having to duplicate any info
var principal = new ClaimsPrincipal(new IIdentity[] { userIdentity, deviceIdentity });
```

```cs - access current "ClaimPricipal" in Controller
if (HttpContext.Current.User is ClaimsPrincipal principal)
{
   foreach (Claim claim in principal.Claims)
   {
      Response.Write("CLAIM TYPE: " + claim.Type + "; CLAIM VALUE: " + claim.Value + "</br>");
   }
}
```

```py
import math
import numpy as np
from sklearn.linear_model import LogisticRegression

class HLRItem:
    def __init__(self, content):
        self.content = content
        self.last_reviewed = 0
        self.total_reviews = 0
        self.correct_reviews = 0

    def review(self, correct, current_time):
        delta = current_time - self.last_reviewed
        self.last_reviewed = current_time
        self.total_reviews += 1
        if correct:
            self.correct_reviews += 1
        return delta, correct

class HLR:
    def __init__(self):
        self.model = LogisticRegression()

    def train(self, data):
        X = np.array([[math.log(delta), p] for delta, p in data])
        y = np.array([correct for _, correct in data])
        self.model.fit(X, y)

    def predict_recall_probability(self, delta, p):
        return self.model.predict_proba([[math.log(delta), p]])[0][1]

    def calculate_half_life(self, p):
        return math.exp((-math.log(0.5) - self.model.intercept_[0] - self.model.coef_[0][1] * p) / self.model.coef_[0][0])

def main():
    hlr = HLR()
    item = HLRItem("What is the capital of France?")

    # Simulate some review data
    data = [
        item.review(True, 1),  # Correct after 1 day
        item.review(True, 3),  # Correct after 2 more days
        item.review(False, 7),  # Incorrect after 4 more days
        item.review(True, 9),  # Correct after 2 more days
    ]

    hlr.train(data)

    p = item.correct_reviews / item.total_reviews
    half_life = hlr.calculate_half_life(p)
    print(f"Estimated half-life for this item: {half_life:.2f} days")

    next_review_time = 14  # Predict recall probability after 14 days
    prob = hlr.predict_recall_probability(next_review_time - item.last_reviewed, p)
    print(f"Probability of correct recall after {next_review_time} days: {prob:.2f}")

if __name__ == "__main__":
    main()
```