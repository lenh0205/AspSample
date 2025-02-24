
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
var reverseStr = new string(Array.Reverse(str.ToCharArray()));
// or:
var reverseStr = new string(cleaned.Reverse().ToArray());
```

```cs
// A phrase is a palindrome if, after converting all uppercase letters into lowercase letters and removing all non-alphanumeric characters, it reads the same forward and backward. Alphanumeric characters include letters and numbers
// Given a string s, return true if it is a palindrome, or false otherwise.
string cleaned = new string(s.Where(char.IsLetterOrDigit).Select(char.ToLower).ToArray());
return cleaned == new string(cleaned.Reverse().ToArray());
```
