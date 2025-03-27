> ta cần suy nghĩ liệu có thể gom Authentication và Authorization thành duy nhất chỉ 1 quá trình là Authorization được không ? tức là chỉ cần đơn giản bảo vệ quyền truy cập đến resource
> nếu vậy mỗi lần truy cập protected resource ta sẽ cần nhập password, đó là lý do cần có thuật ngữ Authen để tách riêng về phần user experience
> vì đơn giản quá trình Authen chỉ cần thực hiện duy nhất 1 lần, còn quá trình Author thì phải thực hiện nhiều lần tương ứng với mỗi lần truy cập protected resource
> vậy liệu có khả năng thực hiện việc Author duy nhất 1 lần như Authen không ?
> ta cần hiểu đúng: thực chất Authen cũng phải được thực hiện nhiều lần như Author, ta chỉ đơn giản là không nhập password nhiều lần thôi; ta sẽ sử dụng thứ khác (Token/SessionID) thay cho password
> mỗi lần truy cập protected resource, ta vẫn phải Authen trước Author sau dựa trên Token/SessionID

> https://auth0.com/docs/get-started/identity-fundamentals/identity-and-access-management 
> Auth0 chính là 1 IAM platform

> liệu OAuth là Authorization hay delegation Authorization ? '
> theo t thấy thì cũng không quan trọng Resource Server có thuộc cùng 1 organization với Client application hay không, cứ "Authorize" là sẽ cần OAuth
> cái khác là nếu chúng chung 1 hệ thống thì ta có thể trong Access Token ta chỉ cần gửi UserID , backend sẽ tìm kiếm trong database User này có permission phù hợp để truy cập API không
> nếu khác hệ thống, thì Client Application cần được User xác nhận uỷ quyền những permission nhất định; hay nói cách khác Scope của Client Application hệ thống này chỉ là tập con của Scope User hệ thống khác
> vậy trong trường hợp này Client không thể chỉ gửi mỗi UserID để truy cập đến Resource của hệ thống khác
> cách đầu tiên có thể nghỉ tới là khi issue 1 Access Token, Authorization Server sẽ cho nó 1 cái ID và lưu nó vào database tương ứng với Scope của nó; backend khi cần kiểm tra chỉ cần tìm đến data này 
> câu hỏi là làm như thế này có hơi khó không ? việc tích hợp với authentication flow có dễ không ? dữ liệu trong database có thể bị người khác sửa đổi không ?
> cách 2 là gửi thêm Scope trong Access Token, kiểm tra xem data trong Access Token có thật được tạo bởi Authorization Server bằng secret key; nếu đúng thì ta không cần nghi ngờ Scope của Client Application
> vì lúc issue Access Token, Authorization Server đã kiểm tra Privilege của User với Scope được request bởi Client rồi; cách này có vẻ dễ hơn vì chỉ cần kiểm tra tính valid của token rồi lấy data từ nó để sử dụng là được        
> mặc dù Access Token không nhất thiết phải là JWT, nhưng ta nên nhớ JWT là self-contained - tức là nó đã chứa đầy đủ thông tin cần thiết để có thể thực hiện nhiệm vụ 

> Vậy thì bước consent có cần thực hiện trên Client Application của cùng 1 organization không ? vì mặc định khi User đăng nhập trên đúng Client của mình thì Server phải cấp đầy đủ Privilege của User rồi
> ta biết rằng màn hình consent sẽ hiện những Scope mà Client yêu cần
> nếu consent luôn cần thiết thì đổi với những Client cùng organize với Resource Server ta chỉ cần hiện đầy đủ Privilege của User để User xác nhận là được 

> Tại sao nên có Authorization Server riêng với Resource Server ?

===========================================================
# IAM - Identity and Access Management 
* -> hiểu đơn giản nó là 1 cách gọi chung để chỉ các **systems handle user validation and resource access**
* => ensures that the **`right people`** access the **`right digital resources`** at the **`right time`** and for the **`right reasons`**

===========================================================
# What does "IAM" do?
* _Identity and access management gives us control over `user validation` and `resource access`:_ 
* -> **How users become a part of your system ?**
* -> **What user information to store ?**
* -> **How users can prove their identity ?**
* -> **When and how often users must prove their identity ?**
* -> **The experience of proving identity ?**
* -> **Who can and cannot access different resources ?**

* => _IAM systems typically provide the following core functionality:_

## Identity management
* -> the process of **`creating, storing, and managing`** **identity information**
* -> **`Identity providers (IdP)`** are software solutions that are used to **track and manage user identities**, as well as the **permissions and access levels** associated with those identities

## Identity federation 
* -> allow **`users who already have passwords elsewhere`** (_for example, in our enterprise network or with an internet or social identity provider_) to **get access to our system**

## Provisioning and deprovisioning of users
* -> the process of creating and managing user accounts, which includes **`specifying which users have access to which resources, and assigning permissions and access levels`**

## Authentication of users 
* -> authenticate a user, machine, or software component by **`confirming that they're who or what they say they are`**
* -> we can add **`multifactor authentication (MFA)`** for individual users for **extra security** 
* -> or **`single sign-on (SSO)`** to **allow users to authenticate their identity with one portal instead of many different resources**

## Authorization of users 
* -> Authorization **`ensures a user is granted the exact level and type of access`** to a tool that they're entitled to
* -> Users can also be **`portioned into groups or roles`** so large cohorts of users can be **`granted the same privileges`**

## Access control 
* -> the process of **`determining who or what has access to which resources`**
* -> this includes **defining user roles and permissions**, as well as **setting up authentication and authorization mechanisms**
* => Access controls regulate access to systems and data.

## Reports and monitoring 
* -> **`generate reports after actions taken on the platform`** (like sign-in time, systems accessed, and type of authentication) to ensure compliance and assess security risks
* -> gain insights into the security and usage patterns of our environment

===========================================================
## Common practices of how does IAM work? 
* _IAM is not one clearly defined system, it is a discipline and a type of framework; so there’s no limit to the different approaches for implementing an IAM system_
* _there're elements and practices in common implementations:_
* -> **the user (resource owner)** initiates an **authentication request** with the **identity provider/authorization server** from the **client application**
* -> if the **credentials** are valid, the identity provider/authorization server first sends an **ID token** containing information about the user back to the client application.
* -> the identity provider/authorization server also obtains **end-user consent** and **grants the client application authorization** to access the **protected resource**
* -> **Authorization** is provided in an **access token**, which is also sent back to the **client application**
* -> the **access token** is attached to **subsequent requests** made to the **protected resource server** from the **client application**
* -> the **identity provider/authorization server** validates the **access token**; if successful the request for protected resources is granted, and **a response is sent back to the client application**




