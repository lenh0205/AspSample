
# Ignore the tracked file
* -> **.gitignore** ignores **`just files that weren't tracked before`**
* -> _tức là sau khi ta **git commit** hoặc **git add** file đó rồi thì dù ta thêm file đó vào **.gitignore** thì **`git vẫn sẽ track change của file đó`**_

* -> run **`git reset name_of_file`** (nếu file đang ở **staged** status) to unstage the file and keep it
* -> in case we want to also remove the given file from the repository (after pushing), use git **`rm --cached name_of_file`** (repository sẽ đánh dấu là ta đã xoá file đó, file đó trên máy ta vẫn còn nhưng máy khác pull về sẽ không có)

# stage 1 file đang bị .gitignore
```bash
git add --force ./index.html
```
