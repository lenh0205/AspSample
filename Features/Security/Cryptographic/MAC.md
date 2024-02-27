# MAC - Message Authentication Code
*  -> is a generic term for **`any function`** that produces **a signature** based on **a message** and **a secret key**
* -> the signature can be used to verify that the message has **`not been tampered with`** or **`forged by an attacker`** 
* -> MAC functions can be classified into two types: **`symmetric`** (_VD: HMAC_) and **`asymmetric`** (_VD: Digital Signature_)

* _MACs can be created from unkeyed hashes (e.g. with the HMAC construction), or created directly as MAC algorithms_
* _some example of HMAC: HMAC, CBC-MAC, CMAC, GMAC_

=========================================================
# HMAC - Hashed Message Authentication Code
* -> is a **`symmetric MAC`** that uses **a cryptographic key** alongside **a hash function** 
* -> to determine both the **integrity** and **authenticity** of the message

* _one of HMAC application is **`SSL protocol`**_
* _we can use HMAC with a **`JWT implementation`**_
* _easy to implement and has low computational overhead, suitable for resource-constrained devices and applications_

## Limit
* -> HMAC requires both parties to **share a secret key** in advance, which **`can be challenging`** in some scenarios, such as **`public key infrastructures or distributed systems`**
* -> HMAC is that it does not provide **non-repudiation**, which means that the sender cannot prove to a third party that they sent a message with a valid signature

## Usage
* -> **`the sender`** and **`the receiver`** would both share a **`secret key`** for a **symmetric algorithm**

* _the sender_
* -> **`hash the message`** with a strong cryptographic **hash function** (_VD: SHA-256_) to create a fixed-length **a digest**
* -> from there, performs an **HMAC computation** using **`the digest`** and the **`secret key`** to generate **a unique signature** for the message
* -> then **`sends both the original message and the generated HMAC signature`** to the receiver

* _the receiver_ 
* -> applies the **same cryptographic hash function** to the **`received message`** to get a **`digest`**
* -> then perform the **same HMAC computation** using the **`digest`** and the **`shared secret key`** to generate a computed HMAC
* -> the compare the **`computed HMAC`** with the **`received HMAC`**

* => _assurance that the message originated from a party with knowledge of the secret key and that the message hasn't been tampered with_
* _note that HMAC does not involve encryption_

```r
// we have 2 user Alice and Bob that send message to each other, and they want to make sure that message came from the right person
// both of the users have same "secret key" to use with same "symmetric algorithm" when they exchange messages
// => so they use "HMAC" for "authenticating" each other

// Alice want to send message to Bob
// -> she use "secret key" to "encrypt" the message
// -> then she uses a "Hashing Algorithm" to generate a "hash file" from the "encrypted message" and the "secret key" 
// -> Alice send both the "hash file" and the "encrypted message" to Bob

// when Bob receive the message, he want make sure the message come from Alice first before "decrypt" the message
// -> by using the same "hashing algorithm", he generate a "hash file" from the same "encrypted file" and "secret key"
// -> then he compare that "hash file" with the one that Alice sent to him
// -> if these 2 hash file are exactly the same, then Bob knows that the message came from Alice (because Alice is the one who has that "key")
// -> now Bob can use the same "secret key" to "decrypt" the message and see the content of that message
```