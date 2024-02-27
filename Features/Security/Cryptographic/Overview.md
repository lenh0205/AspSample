# Security Goal
* **Confidentiality**: **`hiding or scrambling data`** so that only the **`intended recipient`** has access 
* _make sure that nobody in between the sender and receiver is able to read what data or information is sent; typically accomplished by some means of **encryption**_

* **Integrity**: can the recipient be confident that the message has not been accidentally **`modified`**
* _make sure that nobody in between the sender and receiver changed some parts of the shared information_
* _the information received is exact and accurate_
* _if the content of the message is changed after the sender sends it but before reaching the intended receiver, then it is said that the integrity of the message is lost_ 
* _this is typically accomplished with the use of a **hashing algorithm**_


* **Authentication**: can the recipient be confident that the message **`originates from the sender`**
* _in other words, it means that the initial message sender is who he/she claims to be_
* _Authenticity often implies **integrity**, but it’s not the same_
* Authenticity is about proving who you are, while integrity is about protecting the data from unauthorized changes5.

* **Non-repudiation**: if the recipient passes the message and the proof to a third party, can the **`third party`** be confident that the message **`originated from the sender`**? 
* _tức là người nhận có thể giả làm người gửi vì sử dụng chung 1 private key_

===========================================================
# Cryptography
* -> **`transform`** information (_a message_) into **`secure form`** (_not readable, understandable_) so **`unauthorized`** persons can't access (_read the content_) it 

* -> send **`a plain text`** of readable message to a process (**`Algorithm`**), the message get convert to an readable message - **cipher text**  
* -> in the process, we're in **`ciphering the message`**; when the recipient receive the message they **`can't read`** it
* -> recipient has to **`send that message to the same process`** and get back the plain text

## Key
* **Problem**: if an **`authorized person`** gets access to the **`cipher text`** and know the used **`algorithm`** 
* -> they can send own cipher text through the same algorithm to get the **`plain text`**
* => to address this issue, we'll use **key**

* -> send the **`plain text`** and the **`key`** to the **`algorithm`** (_Encryption Algorithms_) to get the **`cipher text`**
* -> to convert **`cipher text`** back to **`plain text`**; we send it and the **`key`** to the same **`algorithm`** (_Decryption Algorithms_)

## Encryption Algorithms
* protect the **confidentiality** and **integrity** of the message

* _basically, have 2 types:_ 
* -> **Symmetric Algorithm**: use **`only one key`** to transform between the _plain text_ and _cipher text_ (_VD: DES, Triple DES, AES, ..._)
* -> **Asymmetric Algorithm**: use **`a key pair`**; if we one key for _encryption_, we must use the other key for _decryption_ (_VD: RSA, ..._)

* _the larger the **key size** the stronger the encryption can be_
* _asymmetric algorithm has key size much bigger than symmetric algorithm; but it **`cost much more resource`** (especially the CPU) that make the communication much more slower_ 

```r
// DES - Data Encryption Standard has a key of "56 bit"; this mean when we want to use this algorithm the key that we are going to use must be 56 bit of key
// AES - Advanced Encryption Standard can work with "multiple sizes of keys"; we can have a key of size 128 bit or 256 bit

// RSA - Rivest-Shamir-Adleman has key size 1024, 2048, 3072, 4096,...
```

## Hashing Algorithms
* -> is the **one way** algorithm - when we encrypt a message, we can't decrypt it back; so technically with **`Hashing Algorithm`** we're not really encrypt the message
* -> the **`result`** of hashing algorithm is called a **digest** / **digital fingerprint** / **hash file**
* -> hashing algorithm is just for the **integrity** of the message (_don't care if someone sees the content of message, just care about the exact message that recipient receive_)

* _whether we send a letter, a page, 10 volume book,... as an input to the hashing algorithm, **the end result will be the same size**_

### Process
* in Hashing algorithm, when we send a message to someone
* -> we **`caculate a hash`** of that message 
* -> then **`send the message with the hash file`** to recipient
* -> we just need to tell the recipient **`the Hashing Algorithm we use`**
* -> then they can **`use the same algorithm`** to generate a hash of that message
* -> if the produced hash is exactly the same as the one we send them, then they know the message is **`intact and the content hasn't been change`**
* => the **integrity** of the message that we send

### Types
* Hashing have multiple algorithm: **`MD5`** (_earliest_), **`SHA`**, **`SHA-2`**, **`RIPEMD`**

```r
// the length of the hash file that is generate by "MD5" is 128 bit
// "SHA-2" generates different hash length 224, 256, 384, 512 bit
// "RIPEMD" also generates hash file in different sizes 128, 160, 256, 320 bit
```

=========================================================
# Note:
* the **`authentication`** without **confidence in the keys** used is **`useless`**
* -> for **`digital signatures`**, a recipient must be confident that the **verification key actually belongs to the sender** 
* -> for **`MACs`**, a recipient must be confident that the **shared symmetric key has only been shared with the sender**

# Issue
* **a digital signature** has a **`large key size`**, calculating a digital signature is incredibly slow
* **HMAC** requires **`n numbers of keys`** because each sender/receiver pair must have their own private key (_while PKI has 2 keys only_)
* all data protection algorithms will fail by **`Replay issue`** 

* _**Relay** - a malicious adversary sending the same message, found by passive listening on original messages_
* **`Solution`**: _uses **`timestamps`** and **nonces** (number used once) to ensure the message is the request that the user intended to sent; a protocol should take measures against this_
* _so if the message is attempted to be sent again, the receiver will reject the message for being used previously_

========================================================
# Password-based schemes
* -> type of **`authentication method`** that use **`a secret password or passphrase`** to **`generate and verify signatures`** 
* -> can be implemented using various techniques, such as **salted hashes**, **key derivation functions**, or **password-authenticated key exchange protocols**
* -> often used for **user authentication**, such as logging into a website or a service

* _the **salt** is to prevent attackers from using pre-computed hashes and from identifying duplicate passwords in your database_

## Limit
* -> **password security**: passwords can be guessed, cracked, or stolen by attackers
* => this can compromise the **`integrity and confidentiality`** of messages, as well as the **`identity and privacy`** of users 

* **password usability**, as passwords can be forgotten, lost, or mistyped by users. 
* => this can affect the **`availability and convenience`** of message exchange, as well as **`the user experience and satisfaction`**

