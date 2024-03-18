
# Client-side/Stateless Sessions
* -> **`stateless sessions`** is **`client-side data`**
* -> Client-side data is **subject to tampering**; as such it must be handled with great care by the backend
* =>  the key aspect of this application lies in the use of **`signing`** and possibly **`encryption`** to **authenticate and protect the contents** of the **`session`**
* => JWTs, by virtue of JWS and JWE, can provide various types of signatures and encryption

## Overview
* -> most of the time **sessions need only be signed** - means, there is **no security** or **privacy concern** when data stored in them is **`read by third parties`**

* -> a common example of a claim that can usually be **`safely read by third parties`** is the **sub** claim ("subject")
* -> a claim that may **`not be appropriately left in the open`** could be an **items** claim representing a **user’s shopping cart**

```r - "subject" claim
// The "subject" claim usually identifies one of the parties to the other (think of user IDs or emails)
// It is not a requirement that this claim be unique. In other words, additional claims may be required to uniquely identify a user. This is left to the users to decide
```

```r - "item" claim
// cart might be filled with items that the user is about to purchase and thus are "associated to his or her session"
// A third party (a client-side script) might be able to harvest these items if they are stored in an unencrypted JWT, which could raise privacy concerns
```

## Security Considerations
* _Method_: a common method for attacking a signed JWT is to simply **remove the signature** (and then change the header to claim the JWT is **unsigned**)

* _Consequence_: careless use of certain JWT validation libraries can result in **`unsigned tokens being taken as valid tokens`**, which may allow an attacker to modify the payload at his or her discretion (_VD: "role": admin_)

* _Solution_: this is easily solved by making sure that the application that **`performs the validation does not consider unsigned JWTs valid`**

### CSRF - Cross-Site Request Forgery
* -> the attacks attempt to **`perform requests against sites`** where the user is **`logged in`** by tricking the user’s browser into sending a request from a different site 

* _Method_:
* -> to accomplish this, a specially crafted site (or item) must **`contain the URL to the target `** 
* -> **`a common example`** is an <img> tag **embedded in a malicious page** with the **src pointing to the attack’s target**

```html - For instance:
<!-- This is embedded in another domain's site -->
<img src="http://target.site.com/add-user?user=name&grant=admin">
```

* -> the above <img> tag will **`send a request`** to "target.site.com" **`every time the page that contains it is loaded`**
* -> if the user had **previously logged in** to "target.site.com" and **the site used a cookie** to
keep the **`session active`**, this cookie will be sent as well
* -> if the **`target site`** does not implement any **CSRF mitigation techniques**, the request will be handled as a valid request on behalf of the user.
* -> _JWTs, like any other client-side data, can be stored as **`cookies`**_

* _Solution_: 
* -> **Short-lived JWTs** can help in this case
* -> **`Common CSRF mitigation techniques`** include **special headers** that are added to requests only when they are performed **from the right origin, per session cookies, and per request tokens** 
* -> if JWTs (and session data) are not stored as cookies, **`CSRF attacks are not possible`**. **`Cross-site scripting attacks are still possible`**, though

### XSS - Cross-Site Scripting
* -> the attacks attempt to **`inject JavaScript in trusted sites`**; injected JavaScript can then **steal tokens from cookies and local storage**
* -> if an access token is leaked before it expires, a malicious user could use it to **`access protected resources`**

* _Reason_: Common XSS attacks are usually caused by **improper validation of data passed to the backend** (_in similar fashion to **`SQL injection attacks`**)

* _Example_: an example of a XSS attack could be related to the **`comments section`** of a public site
* -> every time a user adds a comment, it is **`stored by the backend`** and displayed to users who load the comments section
* -> if the backend does **`not sanitize the comments`**, a malicious user could **write a comment in such a way that it could be interpreted by the browser as a <script> tag**
* -> So, a malicious user could insert arbitrary JavaScript code and **`execute it in every user’s browser`**; thus, **`stealing credentials`** **stored as cookies and in local storage**

* _Solution:_
* -> mitigation techniques rely on **`proper validation`** of **all data passed to the backend** (_in particular, any data received from clients must always be sanitized_)
* -> if **`cookies are used`**, it is possible to **protect them from being accessed by JavaScript** by setting the **`HttpOnly flag`** 
* _the HttpOnly flag, while useful, will not protect the cookie from CSRF attacks_

##  Client-Side Sessions pros and cons
* -> some applications may require big sessions. Sending this state back and forth for every request (or groupof requests) can easily overcome the benefits of the reduced chattiness in the backend. 
* -> a certain balance between client-side data and database lookups in the backend is necessary; 
* -> this depends on the data model of our application (_some applications do not map well to client-side sessions; others may depend entirely on client-side data_)
* -> Run benchmarks, study the benefits of keeping certain state client-side. 

## To decide a right approach
* _Follow these question_
* -> Are the JWTs too big? 
* -> Does this have an impact on bandwidth? 
* -> Does this added bandwidth overthrow the reduced latency in the backend? 
* -> Can small requests be aggregated into a single bigger request? 
* -> Do these requests still require big database lookups?

## Example: shopping application
```r
// The user’s shopping cart will be stored client-side
// there are multiple JWTs present:
// -> 1 JWT for the ID token, a token that carries the "user’s profile information" (useful for the
UI)
// -> 1 JWT for interacting with the API backend - the "access token"
// -> 1 JWT for our client-side state: "the shopping cart"

// The encoded and signed JWT:
"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.
eyJpdGVtcyI6WzAsMiw0XSwiaWF0IjoxNDkzMTM5NjU5LCJleHAiOjE0OTMxNDMyNTl9.
932ZxtZzy1qhLXs932hd04J58Ihbg5_g_rIrj-Z16Js"

// decoded shopping cart:
{
"items": [0, 2, 4], // each item is identified by a numeric ID
"iat": 1493139659,
"exp": 1493143259
}
```

* the **frontend** 
* -> only needs to retrieve it from its **`cookie`**; 
* -> it simply **`decodes the JWT`** to display its contents, **`no need to check the signature`**
```js
function populateCart() {
    const cartElem = $('#cart');
    cartElem.empty();

    const cartToken = Cookies.get('cart'); // retrieve from cookie
    if(!cartToken) {
        return;
    }

    const cart = jwt_decode(cartToken).items; // decode JWT

    cart.forEach(itemId => {
        const name = items.find(item => item.id == itemId).name;
        cartElem.append(`<li>${name}</li>`); // display JWT contents
    });
}
```

* the **backend** 
* -> performed the **`actual checks`** for the **`validity of the cart JWT implemented`**;
* -> **`constructs a new JWT and a new signature`** when content is changed
* -> when come to **`/protected/*`** endpoint, must **first pass the access token validation** step
before **`validating the cart`**
* => one token validates access (**Authorization**) to the API and the other token validates the **integrity** of the **`client side data`** (the cart)
* -> implementing **CSRF mitigation techniques**

```js
// validate as an Express middleware
function cartValidator(req, res, next) {
    if(!req.cookies.cart) {
        req.cart = { items: [] };
    } else {
        try {
            req.cart = { // add to request context to be used by next middleware
                items: jwt.verify( // verify "signature" and decode "payload"
                        req.cookies.cart, 
                        process.env.AUTH0_CART_SECRET,
                        cartVerifyJwtOptions
                    ).items
            };
        } catch(e) {
            req.cart = { items: [] };
        }
    }
    next();
}

// When items are added, the backend constructs a new JWT with the new item in it and a new signature:
app.get('/protected/add_item', idValidator, cartValidator, (req, res) => {
    req.cart.items.push(parseInt(req.query.id));

    const newCart = jwt.sign(req.cart, process.env.AUTH0_CART_SECRET, cartSignJwtOptions);

    res.cookie('cart', newCart, {
        maxAge: 1000 * 60 * 60
    });
    res.end();
    console.log(`Item ID ${req.query.id} added to cart.`);
});

// the locations prefixed by "/protected" are also protected by the API "access token" using "express-jwt" library:
// tức là khi route này nó sẽ kiểm tra "access token" trong "cookie" của "request"
app.use('/protected', expressJwt({
    secret: jwksClient.expressJwtSecret(jwksOpts),
    issuer: process.env.AUTH0_API_ISSUER,
    audience: process.env.AUTH0_API_AUDIENCE,
    requestProperty: 'accessToken',
    getToken: req => {
        return req.cookies['access_token'];
    }
}));
```

* the **authentication and authorization server** (_Auth0_) 
* -> the **access token** and the **ID token** are **`assigned by Auth0`** to our application (_this requires **`setting up a client and an API endpoint`** using the Auth0 dashboard_)
* -> the **`authentication and authorization server`** **displays a login screen** with our settings 
* -> and then **redirects back to our application (backend API)** at a **`specific path`** with **`the tokens we requested`**
```js - set up "client" and "API endpoint" using "Auth0" dashboard
// these are retrieved using the Auth0 JavaScript library, called by our frontend
const clientId = "t42WY87weXzepAdUlwMiHYRBQj9qWVAT"; //Auth0 Client ID
const domain = "speyrott.auth0.com"; //Auth0 Domain

const auth0 = new window.auth0.WebAuth({
    domain: domain,
    clientID: clientId,
    audience: '/protected', // must match the one we setup for your API endpoint using the Auth0
    scope: 'openid profile purchase',
    responseType: 'id_token token',
    redirectUri: 'http://localhost:3000/auth/',
    responseMode: 'form_post'
});

$('#login-button').on('click', function(event) {
    auth0.authorize();
});

// the backend handled the redirect request come from Authorization Server with the token requested; then simply sets them as cookies
app.post('/auth', (req, res) => {
    res.cookie('access_token', req.body.access_token, {
        httpOnly: true,
        maxAge: req.body.expires_in * 1000
    });
    res.cookie('id_token', req.body.id_token, {
        maxAge: req.body.expires_in * 1000
    });
    res.redirect('/');
});
```

================================================================
# Federated Identity



