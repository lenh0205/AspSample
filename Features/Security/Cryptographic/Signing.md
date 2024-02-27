# Signing / Digital Signature
* -> are a form of **`asymmetric MAC`**
* -> is a **`mathematical scheme`** for presenting the **authenticity** of **a digital messages or documents**
* -> use **asymmetric cryptographic** schemes; the signature is **`created with a private key`**, and **`verified with the public key`**
* -> to determine the **integrity**, **authenticity**, **Non-repudiation** of the message

* _most signature schemes actually are implemented with the help of a **`hash function`**_

## Digital Signature vs HMAC
* -> Digital Signature is more scalable because it just need to **`distribute public key`**, while in HMAC share a secret key can be challenging
* -> about **`performance`**: slower than HMACs, and as such used normally only when there is not yet a shared secret, or the non-repudiation property is important
* -> require a reliable and secure way to generate, store, and distribute public and private keys. this can involve **`additional protocols`**, such as certificates, trust models, and revocation mechanisms_

* => to protect data from compromise and authenticate the center at the same time, **encryption** and **digital signing** are used together
* -> they are both used in tandem to **`fulfill compliance standards like FIPS and GDPR`**
* -> encryption and digital signing aslo **`users can be secure`** in the knowledge that data sent to and from will not be compromised 

## Usage
* -> within the realm of digital signatures, **`a PKI`** (Public Key Infrastructure) is needed in order to **`ensure that the public key belongs to the correct entity`**
```r
// Amazon has a public key that is used to encrypt data being sent to them 
// However, what stops a malicious actor from giving a victim user a public key, claiming that it is Amazons? If this happened, then the malicious actor can read the message sent to Amazon. 
// => This is the reason for the PKI system
```

* -> in terms of the example, the sender needs to have a asymmetric encryption scheme that allows for digital signatures, such as RSA
* -> once the keys are created, then the fun begins! To start with, the **`sender`** **hashes the message** (_using Hashing Algorithm_)
* -> this is where the HMAC and digital signatures differ: the **`sender`** uses their **private key** (that only they know) to **sign the message**, instead of **`a symmetric key`**
* -> now, the **`receiver`** of the message will use the **public key to decrypt** the hash, V1 
* -> the **`receiver`** then **hashes the message** themselves, to get V2
* -> if V1 and V2 are the same, that means the message has **`not been tampered with`** and the **`sender is truthful`**
* => this demonstrates **integrity**, **authenticity** and **non-repudiation** for the message  

```r - Bob wants to send a digitally signed document to Alice
// Bob has 2 keys in form of random characters and numbers: "private key" and "public key"
// -> the "private key" is always kept private
// -> in order to digitally sign a document for Alice, Bob need to share "public key" to Alice

// when the document is sent
// -> its content is run through a "Hash Algorithm" - creates a unique sequence of letters and numbers - called a "digest"
// -> the "digest" is then "encrypted" by with Bob "private key", which finally output the "digital signature" of the document

// For Alice to verify the "authenticity" of the document
// -> any variation in the "content" or in "private key" will create a different signature
// -> so Alice can use the "document" and her "digital signature" to "reverse the process" and verify its legitimacy

// -> Alice can process the document to the same "hasing algorithm" that Bob used which will output a "digest 1"
// -> if the "document" is untampered, the "digest" should be exactly the same
// -> as Bob process the digest with his "private key algorithm" to create digital signature, Alice can "decrypt" the digital signature with Bob "public key algorithm" to also get a "digest 2"
// -> if the "signature" is untampered, the "digest" should be exactly the same

// finally, Alice will have 2 "digest": one based on "digital signature", the other one based on the "content of the document"
// => if both "digest" match, then Alice can be sure that the message hasnt change in transit and verify that Bob is actually the author
```

```r - understand asymmetric key in digital signature
"Bob send message to Alice"
// Alice has a pair of "public and private key"
// Alice share "public key" to Bob

// Bob has a plain text and he use Alice "public key" to "encrypt" the message; then forwards it to Alice
// -> now this message can only "decrypted" with Alice "private key" (if we encrypt the message with 1 key, we can decrypt it with the other key only) 
// -> even if the whole crowd has Alice "public key", they can not decrypt the message that was ecrypted with Alice "public key"

// => Alice is the only one has "private key" so she can "decrypt" the message using her private key and see the content of the message


"Alice send message to Bob"
// if Alice "encrypt" the message using her "private key" and forward it to Bob
// Bob and the rest of public that has Alice "public key" can "decrypt" the message
// => why encrypting the message if every one can decrypt that message and see the content ?

// if someone "encrypt" a message using their "private key", the concern is not all about the confidentiality ("sự bảo mật") of the message (no one can see the message) 
// => the concern is to make sure the message came from the right person

// if Bob ask Alice to "encrypt" the message using Alice "private key" that means Bob doesnt really care that everybody sees the "content" of the message 
// -> what Bob cares is "the message came from Alice"
// -> when Alice encrypts a message using her "private key", the only key that can decrypt that message is Alice "public key"
// => if Bob can decrypt the message then Bob know that "message came from Alice"

// if Tom, a malicious user, decides to "decrypt" the message using Alice "public key" and change the message 
// -> then when he "encrypts" the message using any key (even Alice public key) and forwards it to Bob
// -> Bob can not open the message using Alice "public key", because that message is not encrypted by Alice "private key"
// => in this case, Bob knows that this message has been "tampered" with or this message didnt come from Alice
```

## The Digital Signature Algorithm

## Public key Certificate
* -> in essence, **a certificate** consists of **`owner's public key`**, **`owner's information`** (userId, name,...), **`Certificate Authority information`**
* -> then sign it using **`Certificate Authority's private key`**
* -> the certificate can also include: the period of validity of the certificate (_how long the certificate is valid for the public key_)

### Create
* -> if the owner wants the Certificate Authority CA to create a certificate   

### Verify

### Distribute public key