
# Hiểu đúng về cách mà JWT Attack xảy ra: many of these attacks are related to the implementation, rather than the design, of JSON Web Tokens
* => nhưng ta cần nhớ the JWT specification and format is set in stone, vậy nên sẽ không đi thay đổi design mà sẽ thay đổi trong implementation

#  the most common representation for JWTs is: the JWS Compact Serialization format
* Unserialized JWTs have two main JSON objects in them: the header and the payload
* Signed JWTs in compact format are simply the header and payload objects encoded using Base64-URL encoding and separated by a dot (.), last part is the signature

# 1 Attack dễ xảy ra nếu ta không để ý: header chứa “alg: none” 
* lỗi này thường xảy ra là do sự ambiguity in the API of certain JWT libraries
*  Signed JWTs sign both the header and the payload; while encrypted JWTs only encrypt the payload (the header must always be readable)
* in case of signed token, Attacker sẽ rewrite the JWT without using the signature, change data contain in it (trường "alg" và payload)
* many libraries relied on "alg" claim to pick the verification algorithm; **`alg: none`** means that there’s no verification algorithm, and the verification step always succeeds
* to mitigations for this type of attack, the most important one being to always check the algorithm specified in the header before attempting to verify a token; hoặc không rely on the alg claim mà passing an explicit algorithm to the verification function

# Attack: RS256 Public-Key as HS256 Secret 
* Attack xảy ra do nhiều JWT libraries sử dụng duy nhất 1 verification function signature với 1 single argument can represent both a public key (for RS, ES, or PS algorithms) and a shared secret (for HS algorithms)
* -> Attacker sẽ tạo ra 1 token mới bằng cách sửa signing algorithm trong header từ "RS256" thành "HS256", đồng thời s/d public key của "RS256" như shared secret của "HS256"; và thay đổi payload (ví dụ role: admin)
* -> và khi user pass "public key" cho verification algorithm; rather than being used as a public
key for the RS256 algorithm, it will be used as a shared secret for the HS256 algorithm
* -> the token will be considered valid
* => Mitigations against this attack include passing an explicit algorithm to the jwtDecode function,
checking the alg claim, or using APIs that separate public-key algorithms from shared secret
algorithms

# Attack: Weak HMAC Keys
* Attack này xảy ra do brute force attacks for HS256 sẽ rất dễ dàng if the shared secret is too short

* _shared secrets are quite similar to passwords, they should be kept secret_
* Nhưng với password, độ dài của nó thường ngắn nếu so với other types of secrets; vì hashing algorithms that are used to store passwords (along with a salt) có thể prevent brute force attacks in reasonable timeframes
* Mặt khác, HMAC shared secrets are optimized for speed; điều này giúp many sign/verify operations to be performed efficiently nhưng lại làm brute force attacks easier
* => vậy nên JSON Web Algorithms quy định rằng "A key of the same size as the hash output (for instance, 256 bits for HS256) or larger MUST be used with this algorithm"
* => Another good option is to switch to RS256 or other public-key algorithms, which are much
more robust and flexible

# Kiểu dữ liệu của 1 key (private/public key or shared secret): usually strings or byte arrays

# 1 Assumption sai lầm chí mạng:  encryption also provides protection against tampering in all cases
* The rationale for this assumption is usually something like : "if the data can’t be read,
how would an attacker be able to modify it for their benefit?"
* ta cần hiểu rằng even if the encrypted data was modified, something will come out of the
decryption process; thường thì việc modify 1 cách mù quáng sẽ tạo ra rác nhưng đối với a malicious
attacker this may be enough to get access to a system
* => do đó,  JSON Web Algorithms only defines encryption algorithms that also include **`data integrity verification`**
* => còn nếu ta s/d a non-standard algorithm, ta cần chắc rằng nó cung cấp data integrity; hoặc ta sẽ cần lồng các JWT, using a signed JWT as the innermost JWT to ensure data integrity

# Attack: Invalid Elliptic-Curve
* Attack này xảy ra do some implementation fail to validate input cho các arithmetic operations mà elliptic-curves rely on
* -> In elliptic-curve cryptography, the public key is a point on the elliptic curve, while the private key is simply a number that sits within a special, but very big, range
* -> việc s/d những input không đc validate này sẽ dẫn đến những những result not valid
* -> these result can be used to recover the private key

* _Elliptic-curve cryptography is one of the public-key algorithm families supported by JSON Web Algorithms_
* _nó giúp prevents the recovery of the private key from a public key, an encrypted message, and its plaintext_
* _compared to RSA (a public-key algorithm which is also supported by JSON Web Algorithms), elliptic-curves provide a similar level of strength while requiring smaller keys_

# Attack: Substitution
## Different recipient attacks
* tức là trong trường hợp used the signature as the only check for validity, rất có thể 
