# Hashing
* -> to map an **`arbitrary sized data value`** to a **unique fixed size** value
* -> a **one-way** functions - a value is easy to hash, but near **`impossible to go from the hash to the original value`**
* -> a hash function allows someone to easily **`verify that two values are the same`**, **without actually knowing the value itself**
* -> is used for ensuring the **integrity** of the data
* => the **`hash can be used as a unique identifier`** of **`its associated data`**

* _VD: MD5, SHA2, SWIFFT_
* _the hash can be appended to the message or transmitted seperately in a different protected channel_
* _VD: sometimes be used with hashes of very big files (like ISO-images); where the hash itself is delivered over HTTPS, while the big file can be transmitted over an insecure channel_

## Usage
* -> **`the sender`** would send the **`message`**, alongside the **`hash of the message`**
* -> **`the receiver`** would then use the **`same hashing function`** (as the sender) on the message
* -> if these two hashes are the same, then the message has **`not been tampered with`**

## Common use case: Storing passwords
* -> when user enter password, use the **`same algorithm`** (_that create the already stored hashed version of the password_) to create the hash of the entered password; 
* -> then compare them

* _hashing algorithms are also known to hackers_ because **`hash function is deterministic`**
* -> hacker can use **`brute force dictionary attack`** - by hashing the passwords for every matched hash, they'll know the actual password
* -> to mitigate the risk, adding a **salt** - **`a fixed length random value`** to the **`input of hash functions`** to create unique hashes for every input
* -> as hackers can't know the **`salt`** (_random value_), the hash passwords are **`more secure`**


