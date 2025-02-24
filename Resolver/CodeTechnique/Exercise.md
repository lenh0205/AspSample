
# Bài toán: check sự xuất hiện của phần tử trong 1 chuỗi
* -> ta sẽ lặp qua chuỗi đó và đếm số lần xuất hiện của từng phần tử lưu vào 1 Dictionary<char,int>
* -> ta cần nhớ là không thể lặp qua Dic để check điều kiện và tìm kết quả cuối cùng vì Dic không đảm bảo thứ tự khi insert
* -> ta nên lặp qua chuỗi đó lần nữa và dựa vào Dic để kiểm tra điều kiện để đưa ra kết quả cuối cùng

# Bài toán: đảo ngược chuỗi và check ký tự có phải alphanumeric không 
* -> check ký tự
```cs
var valid = char.IsDigit(c); // check numeric
var valid = char.IsLetter(c); // check alphabetic 
var valid = char.IsLetterOrDigit(c); // check alphanumeric  
```
* -> reverse 1 chuỗi
```cs
char[] charArray = str.ToCharArray();
Array.Reverse(charArray);
var reverseStr = new string(charArray);
```
