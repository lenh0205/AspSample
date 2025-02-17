==========================================================================
# Ignore the tracked file
* -> **.gitignore** ignores **`just files that weren't tracked before`**
* -> _tức là sau khi ta **git commit** hoặc **git add** file đó rồi thì dù ta thêm file đó vào **.gitignore** thì **`git vẫn sẽ track change của file đó`**_

* -> run **`git reset name_of_file`** (nếu file đang ở **staged** status) to unstage the file and keep it
* -> in case we want to also remove the given file from the repository (after pushing), use git **`rm --cached name_of_file`** (repository sẽ đánh dấu là ta đã xoá file đó, file đó trên máy ta vẫn còn nhưng máy khác pull về sẽ không có)

# stage 1 file đang bị .gitignore
```bash
git add --force ./index.html
```

==========================================================================
# Tag
* -> Tags are just human readable shortcuts for hashes
```bash
$ git tag <tag-name>
```

==========================================================================
# Get Information

```bash
$ git help
$ git status # Shows list of modified files
$ git diff # Shows changes we have made
$ git log # Note the hash code for each commit
$ git show # Can use full or shortened hash
$ git reflog # see all changes that have occurred

$ git diff HEAD^^ # Show what has changed in last two commits
$ git diff HEAD~10..HEAD~2 # Show what changed between 10 commits ago and two commits ago
$ 
```

==========================================================================
# git cherry-pick
* -> để lấy 1 commit từ 1 nhánh khác về nhánh hiện tại
https://stackoverflow.com/questions/36975986/cherry-pick-shows-no-m-option-was-given-error

```r
git cherry-pick <SHA>

# trong trường hợp commit ta muốn lấy là 1 merge commit
# -> ta sẽ cần thêm "-m <parent-number>" option
git cherry-pick -m 1 <SHA>
```

==========================================================================
# git rebase

==========================================================================
# log commit between 2 dates
```js
git log --since="2022-04-22" --until="2022-04-24" // from '22 April 2022' to '24 April 2022'
git log --after="2014-7-1" --before="2014-7-4" 

git log --since='Apr 1 2021' --until='Apr 4 2021'
git log --since='2 weeks ago'
git log --until='yesterday'
```

==========================================================================
# delete branch
* _lưu ý không thể xoá branch nếu ta đang trên branch đó hiện tại_
* https://stackoverflow.com/questions/2003505/how-do-i-delete-a-git-branch-locally-and-remotely

```js
// delete remote (usually "origin")
git push -d [remote] [branch] 
git push [remote_name] --delete [branch] // for old Git v1.7.0

// delete locally
git branch -d [branch] 
git branch -D <branch_name>

// Error: unable to push to unqualified destination: remoteBranchName The destination refspec neither matches an existing ref on the remote nor begins with refs/, and we are unable to guess a prefix based on the source ref. error: failed to push some refs to 'git@repository_name'
// -> someone else has already deleted the branch
// -> we just need to synchronize our branch list:
git fetch -p
```

==========================================================================
# Create new branch and push to remote
* https://stackoverflow.com/questions/1519006/how-do-i-create-a-remote-git-branch

```bash
git checkout -b [branch]
git push [remote] [branch] 
```

==========================================================================
# Merge 2 Git repositories
* https://stackoverflow.com/questions/1425892/how-do-you-merge-two-git-repositories
* https://stackoverflow.com/questions/17371150/moving-git-repository-content-to-another-repository-preserving-history

```bash
cd path/to/project-a
git checkout some-branch

cd path/to/project-b
git remote add project-a /path/to/project-a
git fetch project-a --tags
git merge --allow-unrelated-histories project-a/some-branch
git remote remove project-a
```
