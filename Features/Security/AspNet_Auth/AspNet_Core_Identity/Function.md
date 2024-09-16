
# Register
* -> ta sẽ tạo 1 instance của **`IdentityUser`**
* -> gán giá trị cho property của nó bằng những giá trị Input; riêng thằng **Email** và **UserName** thì ta cần sử dụng **`IUserStore`** và **`IUserEmailStore`** để gán
* -> sau đó dùng **`UserManager`** để 
* -> sau đó nó sử dụng 1 **`IEmailSender`** để gửi 1 email xác nhận tài khoản với nội dung là ta hãy bấm vào link **`..../Account/ConfirmEmail?area=Identity&userId=1&code=1&returnUrl=%2F`**
* -> đường link này 
* -> đồng thời redirect ta tới trang **`/Account/RegisterConfirmation?email=abc&returnUrl=%2F`**, hiện tại thì nó  

