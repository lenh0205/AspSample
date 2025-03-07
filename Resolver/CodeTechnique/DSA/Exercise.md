
# Bài toán: check sự xuất hiện của phần tử trong 1 chuỗi
* -> ta sẽ lặp qua chuỗi đó và đếm số lần xuất hiện của từng phần tử lưu vào 1 Dictionary<char,int>
* -> ta cần nhớ là không thể lặp qua Dic để check điều kiện và tìm kết quả cuối cùng vì Dic không đảm bảo thứ tự khi insert
* -> ta nên lặp qua chuỗi đó lần nữa và dựa vào Dic để kiểm tra điều kiện để đưa ra kết quả cuối cùng

# Bài toán: kiểm tra 1 chuỗi có phải là "palindrome" - check ký tự có phải alphanumeric không + đảo ngược chuỗi + so sánh  
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

* -> example:
```cs
// A phrase is a palindrome if, after converting all uppercase letters into lowercase letters and removing all non-alphanumeric characters, it reads the same forward and backward. 
// Alphanumeric characters include letters and numbers
// Given a string s, return true if it is a palindrome, or false otherwise

string cleaned = new string(s.Where(char.IsLetterOrDigit).Select(char.ToLower).ToArray());
return cleaned == new string(cleaned.Reverse().ToArray());
```

# Bài toán: trong 1 chuỗi tìm chuỗi con là "palindrome" với độ dài dài nhất
* -> ta sẽ cần sử dụng kỹ thuật **`Expand Around Center (EAC)`** (hoặc **`Dynamic Programming`**), 
* -> nếu không như bình thường ta lặp 2 lần brute-force and check whether for every start and end position a substring is a palindrome thì sẽ khó pass Time Limit để execute
* 
```cs
public class Solution {
    public string LongestPalindrome(string s) {
        if (string.IsNullOrEmpty(s) || s.Length < 1) return "";

        int start = 0, end = 0;
        
        for (int i = 0; i < s.Length; i++)
        {
            int len1 = ExpandFromCenter(s, i, i); // Odd length palindrome
            int len2 = ExpandFromCenter(s, i, i + 1); // Even length palindrome
            int len = Math.Max(len1, len2);

            if (len > end - start)
            {
                start = i - (len - 1) / 2;
                end = i + len / 2;
            }
        }

        return s.Substring(start, end - start + 1);
    }

    private static int ExpandFromCenter(string s, int left, int right)
    {
        while (left >= 0 && right < s.Length && s[left] == s[right])
        {
            left--;
            right++;
        }
        return right - left - 1;
    }
}
```
