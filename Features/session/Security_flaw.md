# Broken authentication
* -> arises when **`authentication mechanisms`** for users are **`not implemented correctly or are flawed`**
* -> allowing unauthorized individuals to access an application or system 
* -> those individuals can potentially steal passwords, prevent proper logging out or termination procedures, and even gain access to the system

===============================================================
# Threats and Cyber Attacks Related to Session Management
## Session Hijacking
* -> involve an attacker attempting to **`get the session ID`** of the victim **`after the user logs in`**
* -> afterwards, the **`attacker can then impersonate the user`** and perform actions on their behalf using the obtained session ID

## Session Fixation
* -> essentially **`exploits the flaws`** of **`authentication`** and **`session management`** of web app; than **`fixes`** an **`established session on the user’s browser`**
* -> to do this, the hacker tricks the victim into using an identifier that the hacker already knows instead of stealing new identifiers
* -> Afterwards, the hacker can then impersonate the user

## cross-site scripting (XSS) 
* attackers insert malicious scripts into web pages, enabling them to steal session cookies

## brute-force attacks
* a malicious actor repeatedly tries to guess passwords on authorized user accounts

