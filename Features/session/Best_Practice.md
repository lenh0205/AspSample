# Implementing Session Management - Best Pratices
* -> refers to handling **`multiple requests and responses`** from **`a user or entity`** on a website or web application
* -> it's critical to **`properly secure and manage sessions`**, especially within **`authenticated sessions`**, in order to prevent **`broken authentication`**

===============================================================
## Properties of Session ID
* a **session ID** or **session token** is **`a unique identifier`** 
* there are a few properties of session ID that **`has to be properly configured`** in order to **`prevent attacks`** related to session management

* -> it is recommended by **`OWASP`** to use a good **CSPRNG**(_Cryptographically Secure Pseudorandom Number Generator_) to generate Session ID and implement **logging mechanisms** to track their usage
* -> the **`length of the session ID`** should be at least 128 bits (16 bytes)
* -> and _session ID has to be unpredictable_ to protect users from **`guessing attacks`**
* -> the session ID **`shouldn’t be descriptive`** or its content **`should be meaningless`** so that any **`relevant information will not be disclosed`** when an attack manages to **`decode the content of the ID`**
* -> don't pass the session IDs as **`a parameter in URLs`**
* -> **cookies** are usually used to contain the session ID since they **`have some attributes that can protect exchange of session ID`** (_if attacker get Session ID, they can impersonate the user_)
* -> avoid using obvious names for cookies

===============================================================
## Attributes of Cookies
* _cookies_ have some attributes that help **`protect session ID exchange`**
* -> **Secure** tells the browser **`only to send cookies over HTTPS connections`** (_which are encrypted as opposed to HTTP_)
* -> **HttpOnly** tells the browser **`not to allow JavaScript access to the cookie`**; this helps to prevent _malicious scripts_ from stealing cookies
* -> **SameSite** have different values (**`Strict, Lax, None`**) to specify whether **`cross-site requests should send cookies`**; **Strict cookies** are **`only sent with same-site requests`**; **lax cookies** are sent with **`both cross-site and same-site requests`**; the request will be sent with all requests if left at **`none`**
* -> **Expires / Max-Age** - when having this attribute, the cookie'll be considered "persistent" and **`will be stored by the browser until the expiration time`** specified by the attributes

===============================================================
## Provide appropriate access controls
* ensure that users have the **`correct level of access`** to the **`resources`** and **`functionalities`** in a web application
* -> Use **RBAC** - **`role-based access control`**, in which _roles assigned to users_ decide what actions and resources they can access 
* -> Define permissions at a highly detail level to prevent providing users with extraneous access. 
* -> Consider implementing dynamic access controls, which restrict access to specific resources based on factors such as user location, role, and even the time of day. 
* -> regularly review permissions and adjust them for users who have changed roles or no longer require access to certain areas or functionalities

===============================================================
## Generation of New Session IDs
* Session ID shouldn’t remain the same throughout the entire session, it should be **`renewed or regenerated whenever a user’s privilege level changes`**
* _for example, when a user visits a website, it will get a session ID. Once the user logs into the account, the privilege level will change and the session ID should be regenerated_

* -> other scenarios where session ID should be changed include **password changes** and **permission changes**
* -> have mechanisms in place to **`detect permissions changes`**
* -> **`invalidate the existing session`** and **`force the user to reauthenticate for access`**
* -> **`generate a new, unique session ID`** for the user that is associated with their updated permissions.
* -> **`copy`** over any _relevant session data, user settings, and authentication_ **`information to the new session`**
* -> **`send the new session ID to the user's browser as a cookie`** and **`update the session state on the server`**
* -> notify the user about their need to reauthenticate due to the permissions change

===============================================================
## Session "encrypting" and "recording"
* **Encrypting** web sessions helps **`maintain data`** confidentiality, while **recording** supports accountability and auditing when **`troubleshooting or investigating`** incidents during sessions  

* _For encrypting_
* -> it’s important to encrypt data while being transmitted between the user’s browser and the web server as well as to prepare it for storage
* -> we can obtain a **`Transport Layer Security`** (**TLS**) / **`Secure Sockets Layer`** (**SSL**) certificate from a trusted authority to enable encrypted communication

* _Recording_
* -> should **`log session activities`** such as login attempts and other actions
* -> for potentially **`suspicious actions`** like changing settings or other information, make sure **`recording the user’s IP address and a timestamp`**
* -> ensure that **`session logs`** are **`encrypted and stored securely`**
* -> this will provide an audit trail for additional monitoring or an investigation.
* -> make sure you abide by any regulatory requirements (such as **GDPR** or **HIPAA**) that mandate specific requirements for storing session data

===============================================================
## Network segmentation 
* _network segmentation_ can support **`improved security`** by **`restricting communication between segments`**
* this can be beneficial _if one segment is attacked or compromised_; the power to **`define different rules and policies for each segment`** also allows for more **`control over access`**

* -> **`classify data and assets`** according to their sensitivity, importance, and regulatory requirements, then ensure the network architecture reflects those factors
* -> Isolate administrative functions and other sensitive systems and data in separate segments that have stricter access controls and strong authentication.
* -> Use the **principle of least privilege** to ensure users can access only the segments and functionalities they are authorized for
* => This limits the entire network from being exposed to potential attacks or breaches.
* -> implement strong mechanisms for monitoring and logging traffic for suspicious behaviors during sessions

===============================================================
## Session termination on logoff 
* session should be properly terminated when the **`user logs off`** or the **`server ends it due to expiration`**
* => this reduces the **`risk of unauthorized access, breaches, and data exposure`**

* -> the server **`removes the session data`** **`associated with the user's session`** or **`marks it as invalid`**
* -> the **`cookie should be set to expire immediately`** after termination or shortly afterward (_this prevents the browser from sending the `now-invalid session ID` in subsequent requests_)
* -> the server should **`record details about the logoff`**, such as the time and date.
* -> the application’s **`user interface should update`** to show that the user is logged out, such as by redirecting to a landing page, showing a message indicating they have logged off, or changing the "log out" button to "log in"

===============================================================
## Audit logs
* **`Audit logs`** are **`essential for detecting and investigating unauthorized access`** and improving session management security
* -> clearly define what events and activities during sessions need to be logged
* -> include critical relevant details such as the user's identity, timestamp, session ID, IP address, the action that took place, and what resource they accessed
* -> when possible, prevent audit logs from being modified or deleted. If not, protect them from tampering by implementing measures such as digital signatures, checksums, or hashing

===============================================================
## User training
* users are valuable resources to help ensure strong session management
* -> provide clear and accessible documentation that explains how to log on and log off correctly, how to keep their authorization credentials safe, and how to recognize attempts at infiltrating their sessions.
* -> Remind user to uncheck the “remember me” option and completely log out of web applications when they are using a device that is not their own, such as at a library.
* -> Encourage users to report suspicious activity or attempts to gain access to credentials or other information.

===============================================================
## Other
* ngoài ra, để tăng security ta có thể
* -> **`invalidate all open sessions when a password is changed`**
* -> **`Limit the number of simultaneous sessions per account`** (_only be able to access the account from one device at a time_)