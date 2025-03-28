=======================================================================
# Single Sign On
* -> SSO is an **`authentication scheme`** - enables a user to **securely access multiple applications and services** using **`a single ID`**

### Example
* -> when **SSO is integrated into apps** like Gmail, Workday, or Slack; it **provides a pop-up widget or login page for the same set of credentials**
* -> with SSO, **users can access many apps without having to log in each time**

=======================================================================
# SSO and Federated Identity
* -> SSO is built on a concept called **`federated identity`** - enables **sharing of identity information across trusted but independent systems**
* -> _there are two **common protocols** for this authentication process:_

## SAML (Security Assertion Markup Language)
* -> an **`XML-based open standard`** for exchanging identity information between services
* -> it is commonly found in the **`work environment`**

## OpenID Connect
* -> **`when we use our personal account (e.g., Google) to sign into applications`** like YouTube
* -> it uses **`JSON Web Token`** to share **identity information** between services

=======================================================================
# SSO Login Flow 

## Using SAML

### Step 2: Gmail Requests Authentication
* -> an office worker visits an application like Gmail; in SAML terms, Gmail in this example is a **Service Provider**
* -> the Gmail server detects that the user is from a work domain (e.g., @company.com)
* -> then returns a **`SAML Authentication Request`** (302 HTTP Redirect) back to the browser

### Step 3: Redirect to Identity Provider (IdP)
* -> the browser **`redirects the user to the Identity Provider`** (Okta, Auth0,...) for the company **specified in the SAML authentication request**
* -> the **`Identity Provider shows the login page`**, where the user enters the login credentials

### Step 4: User Authentication at the IdP
* -> once the user is authenticated, the **Identity Provider** generates a SAML response called a **`SAML assertion`** and returns that to the browser
* -> the "SAML assertion" is a **cryptographically-signed** XML document that contains **`information about the user`**, and **`what the user can access with the service provider`**

### Step 5: Returning the SAML Assertion to the Service Provider
* -> the **browser forwards the signed SAML assertion to the Service Provider**
* -> the **Service Provider validates that the assertion** was **`signed by the Identity Provider`** by (usually) using **`public key cryptography`**

### Step 6: Access Granted
* -> the Service Provider returns the **`protected resource`** to the browser based on what the user is allowed to access as specified in the SAML assertion
* -> this completes the walkthrough of a basic SSO login flow

=======================================================================
# SSO for Multiple Applications
* -> Let’s see what happens when the user navigates to another SSO-integrated application, say, Workday.

* -> The Workday server, as in the previous example with Gmail, detects the work domain and sends a SAML authentication request back to the browser.

* -> The browser again redirects the user to the Identity Provider.

* -> Since the user has already logged in with the Identity Provider, it skips the login process and instead, generates a SAML assertion for Workday, detailing what the user can access there.

* -> The SAML assertion is returned to the browser and forwarded to Workday.

* -> Workday validates the signed assertion and grants the user access accordingly.

# SSO with OpenID Connect
Earlier, we mentioned that OpenID Connect is another common protocol.

The OpenID Connect flow is similar to SAML, but instead of passing signed XML documents, OpenID Connect passes around JWT.

JWT is a signed JSON document.

The implementation details are a little bit different, but the overall concept is similar.

Which SSO Method Should We Use?
Both implementations are secure.

For an enterprise environment where it is common to outsource identity management to a commercial identity platform, the good news is that many of these platforms provide strong support for both.

The decision then depends on the application being integrated and which protocol is easier to integrate with.

✅ If we are writing a new web application, integrating with some of the more popular OpenID Connect platforms like Google, Facebook, and GitHub is probably a safe bet.

Conclusion
If you like to learn more about system design, check out our books and free weekly newsletter.

Please subscribe if you learned something new!

Thank you so much, and we’ll see you next time! 🎉

Changes Made:
Only formatted the content (headings, bullet points, line breaks).

Kept all words and sentences exactly as in your original text.
