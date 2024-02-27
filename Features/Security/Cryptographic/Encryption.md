# Encryption
* -> making data **`unreadable`** and **`unusable`** to **`unauthorized viewers`**
* -> _to use and read encrypted data_, it must be **`decrypted`** which requires the use of **`security key`**
* -> Encryption may protect data at rest or in transit

=======================================================
# Types of Encryption

## Symmetric
* uses the **`same key to encrypt and decrypt`** the data 

## Asymmetric / Public Key Cryptography
* -> uses **`a public and private key pair`**
* -> is used for things like **TLS - Transport Layer Security** such as **`https protocol`** and **`data siging`**

=======================================================
# How to share the key
* the key cannot go alongside the message or get sent in a different letter, as it can still be **`intercepted and read`**
* => solution is **`Public Key Cryptography`**
* -> because the public key is known and available, then encrypt the data with the public key
* -> if the a malicious adversary tries to intercept the message it will not matter, as only the receiver will have the **`private key to decrypt the message`**

# When to use Symmetric or Asymmetric 
* -> most **`asymmetric encryption algorithms`** are **`extremely expensive to compute`**; so encrypting all data using _Public Key Cryptography_ is **`not a viable option`**
* -> because of this, **`symmetric key ciphers`** are still used to **`encrypt message information`** 
* -> practically speaking, **`asymmetric encryption`** is used in order to **`exchange keys`**, while **`symmetric algorithms`** use the transferred key in order to **`encrypt the data`**
* -> _now, that receiver knows the key, there is a very small chance for an adversary to be able to read the message_

=======================================================
# Public Key Infrastructure - PKI
* -> the **system** that provides the **Public Key Encryption keys** and **digital signatures** for an entity is the PKI
* -> the purpose of the PKI is to **`maintain and manage the keys/certificates`** that are **`in the system`**
* => by doing this, a **`trustworthy networking environment`** is created for using the cryptographic systems 

* => the PKI ensures that the entity saying they own the public key (_to encrypt the data to send to the other party_) is actually **`the entity they claim they are`** 
* => The most well known of this is implemented within the browser ecosystem, known as **TLS - Transport Layer Security**

* _nói chung là PKI take care việc exchanging public keys để encrypt data_

=======================================================
# Encryption at Rest
* -> **data at rest** is data that's stored on a **`physical device`** (_such as server_); it may be stored in **`a database or a storage account`**
* -> encryption of data at rest ensures that the **`data is unreadable without the keys and secrets`** needed to decrypted 

# Encryption in transit
* -> **data in transit** is the data **`moving from one location to another`** (_such as across the internet or through a private network_)
* -> secure transfer can be handled by **`several different layers`**; it could be be done by encrypting the data at the application layer before sending it over a network
* -> **https** is an example of encryption in transit
* -> Encryption in transit **`protects data from outside observers`** and provides a mechanism to transmit data while **`limiting the risk of exposure`**