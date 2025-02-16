https://git-scm.com/docs/gitignore
https://git-scm.com/docs/git-status
https://git-scm.com/book/en/v2/Git-Basics-Recording-Changes-to-the-Repository
https://docs.github.com/en/get-started/getting-started-with-git/ignoring-files
https://sigalambigha.home.blog/2020/03/11/how-to-refresh-gitignore/
https://www.cloudbees.com/blog/git-status-in-depth
https://www.atlassian.com/git/tutorials/inspecting-a-repository
https://stackoverflow.com/questions/5533050/gitignore-exclude-folder-but-include-specific-subfolder
https://www.freecodecamp.org/news/gitignore-file-how-to-ignore-files-and-folders-in-git/
https://stackoverflow.com/questions/25436312/gitignore-not-working
https://stackoverflow.com/questions/38983153/git-is-not-respecting-gitignore-instruction-to-ignore-env-files
https://www.copia.io/blog/gitignore
https://www.w3schools.com/git/git_ignore.asp?remote=github
https://www.pluralsight.com/resources/blog/guides/how-to-use-gitignore-file
https://www.designveloper.com/blog/how-gitignore-works/
https://www.bmc.com/blogs/gitignore/
https://wenijinew.medium.com/git-how-exactly-ignore-works-47c85f0dd190
https://www.atlassian.com/git/tutorials/saving-changes/gitignore
https://stackoverflow.com/questions/19730565/how-to-remove-files-from-git-staging-area


https://stackoverflow.com/questions/9594229/accessing-session-using-asp-net-web-api/14668693#14668693

=================================================================
# Recording Changes to the Repository
* lifecycle of the status of files: **Untracked**; (_Tracked_) **Unmodified**, **Modified**; **Staged**

* -> **Untracked files** basically means that **`Git sees a file you didn’t have in the previous snapshot`** (commit), and which **`hasn’t yet been 'staged'`**

* -> **Tracked files** are **`files that were in the last snapshot`**, as well as any **`newly staged files`**; they can be _unmodified, modified, or staged_

* -> _`when we first clone a repository`_, all of our files will be **tracked** and **Unmodified** because Git just checked them out and we haven’t edited anything

* -> _as we edit files_, Git sees them as **modified**, because we’ve **`changed them since our last commit`**

* -> as you work, you selectively **stage** (_l **`Staged`** or **`Changed to be committed`** or **`Staging Modified Files`**_) these **`modified files`** 

* -> finally, **commit** all those staged changes - the version of files at the time we ran "git add" is what will **`be in the subsequent historical snapshot`**
* -> and the **cycle repeats**

=================================================================
# Tracking New Files, Staging Modified Files
* -> **git add** is a _multipurpose command_ - we use it to **`begin tracking new files`**, to **`stage files`**, and to do other things like **`marking merge-conflicted files as resolved`**
* -> _hiểu đơn giản thì nó là **add precisely this content to the next commit**_

* -> _git add_ command takes **a path name** for either **`a file`** or **`a directory`**
* -> if it’s a **directory**, the command **`adds all the files in that directory recursively`**

=================================================================
# Checking the Status of Files
* -> **git status** - checking **`which files are in which state`**

```r
## ------> directly after a clone

$ git status
On branch master ## branch we're on
Your branch is up-to-date with 'origin/master'. ## it has not diverged from the same branch on the server
nothing to commit, working tree clean ## none of our tracked files are modified. Git also doesn’t see any untracked files

## ------> add a new file to project name 'README' file

$ echo 'My Project' > README
$ git status
On branch master
Your branch is up-to-date with 'origin/master'.
Untracked files: ## the file is "Untracked"
  (use "git add <file>..." to include in what will be committed)

    README

nothing added to commit but untracked files present (use "git add" to track)

## -----> after running "git add README"

$ git status
On branch master
Your branch is up-to-date with 'origin/master'.
Changes to be committed: ## 'README' file is now "tracked" and "staged" to be committed
  (use "git restore --staged <file>..." to unstage)

    new file:   README

## -----> we change an already tracked file called 'CONTRIBUTING.md'

$ git status
On branch master
Your branch is up-to-date with 'origin/master'.
Changes to be committed:
  (use "git reset HEAD <file>..." to unstage)

    new file:   README

Changes not staged for commit: ## a file that is "tracked" has been "modified" in the working directory, but "not yet staged"
  (use "git add <file>..." to update what will be committed)
  (use "git checkout -- <file>..." to discard changes in working directory)

    modified:   CONTRIBUTING.md

## -----> after running "git add CONTRIBUTING.md"

$ git status
On branch master
Your branch is up-to-date with 'origin/master'.
Changes to be committed: ## both files are "staged" and will go into our "next commit"
  (use "git reset HEAD <file>..." to unstage)

    new file:   README
    modified:   CONTRIBUTING.md

## -----> open 'CONTRIBUTING.md' again and make a little change

$ git status
On branch master
Your branch is up-to-date with 'origin/master'.
Changes to be committed:
  (use "git reset HEAD <file>..." to unstage)

    new file:   README
    modified:   CONTRIBUTING.md ## Git stages a file exactly as it is when you run the "git add" command

Changes not staged for commit:
  (use "git add <file>..." to update what will be committed)
  (use "git checkout -- <file>..." to discard changes in working directory)

    modified:   CONTRIBUTING.md

## -----> git add CONTRIBUTING.md
$ git status
On branch master
Your branch is up-to-date with 'origin/master'.
Changes to be committed:
  (use "git reset HEAD <file>..." to unstage)

    new file:   README
    modified:   CONTRIBUTING.md
```


* -> **git status -s** (_or **`git status --short`**_) to see our changes in a more compact way

```r
$ git status -s
 M README ## modified but not yet staged
M  lib/simplegit.rb ##  modified and staged
MM Rakefile ## modified, staged and then modified again - both staged and unstaged
?? LICENSE.txt ## new files that aren’t tracked
A  lib/git.rb ## new files that have been added to the staging area

## nhìn kỹ 1 cái status sẽ có 2 chữ, " M" khác với "M "
```

========================================================================
# git diff - viewing Staged and Unstaged Changes
* -> in case we want to **know exactly what we changed**, **`not just which files were changed`** — we can use the _git diff_ command
* -> _git diff_ shows us the **`exact lines added and removed`** - **the patch**, as it were
* -> _two questions: What have we `changed but not yet staged`? And what have we `staged about to commit`?_

* -> **git diff** - **`changed but not yet staged`**
* -> **git diff --staged** - **`compares staged changes to the last commit`**
* -> **git diff --cached** - to see **`what we've staged`**

```r - "edit and stage" the 'README' file again and then "edit without staging" the 'CONTRIBUTING.md' file
## -----------> git diff
diff --git a/CONTRIBUTING.md b/CONTRIBUTING.md
index 8ebb991..643e24f 100644
--- a/CONTRIBUTING.md
+++ b/CONTRIBUTING.md
@@ -65,7 +65,8 @@ branch directly, things can get messy.
 Please include a nice description of your changes when you submit your PR;
 if we have to read the whole diff to figure out why you're contributing
 in the first place, you're less likely to get feedback and have your change
-merged in.
+merged in. Also, split your changes into comprehensive chunks if your patch is
+longer than a dozen lines.

 If you are starting to work on a particular area, feel free to submit a PR
 that highlights your work in progress (and note in the PR title that its)

## -----------> git diff --staged
diff --git a/README b/README
new file mode 100644
index 0000000..03902a1
--- /dev/null
+++ b/README
@@ -0,0 +1 @@
+My Project

## ----------> git status
On branch master
Your branch is up-to-date with 'origin/master'.
Changes to be committed:
  (use "git reset HEAD <file>..." to unstage)

    modified:   README

Changes not staged for commit:
  (use "git add <file>..." to update what will be committed)
  (use "git checkout -- <file>..." to discard changes in working directory)

    modified:   CONTRIBUTING.md
```

```r - Example: stage the 'CONTRIBUTING.md' file and then edit it
## ---------> "git diff" - what is still unstaged
diff --git a/CONTRIBUTING.md b/CONTRIBUTING.md
index 643e24f..87f08c8 100644
--- a/CONTRIBUTING.md
+++ b/CONTRIBUTING.md
@@ -119,3 +119,4 @@ at the
 ## Starter Projects

 See our [projects list](https://github.com/libgit2/libgit2/blob/development/PROJECTS.md).
+# test line

## --------> "git diff --cached" - what is staged
diff --git a/CONTRIBUTING.md b/CONTRIBUTING.md
index 8ebb991..643e24f 100644
--- a/CONTRIBUTING.md
+++ b/CONTRIBUTING.md
@@ -65,7 +65,8 @@ branch directly, things can get messy.
 Please include a nice description of your changes when you submit your PR;
 if we have to read the whole diff to figure out why you're contributing
 in the first place, you're less likely to get feedback and have your change
-merged in.
+merged in. Also, split your changes into comprehensive chunks if your patch is
+longer than a dozen lines.

 If you are starting to work on a particular area, feel free to submit a PR
 that highlights your work in progress (and note in the PR title that its
```

=============================================================================
# Committing Changes
* -> so now the **`staging area is set up`** the way we want it, we can **`commit our changes`**
* -> anything that is **`still unstaged`** (_any files we have **`created or modified`** that you haven’t run git add on since you edited them_) won’t go into this commit; they will **stay as modified files on your disk**

=============================================================================
# Removing files
* -> if we just **`simply remove the file from working directory`**, it shows up under the **Changes not staged for commit** - the **Unstaged** area of our **`git status`** output

## git rm
* -> the **git rm** command **`remove the file from our tracked files`** (more accurately, **remove it from staging area**) and then **commit**
* -> and also **`removes the file from your working directory`** so we **don't see it as an untracked file the next time around**
* -> **`the next time we commit`**, the file will be **gone and no longer tracked**

```r - if using "git rm"
## -----> git rm PROJECTS.md
rm 'PROJECTS.md'

## -----> git status
On branch master
Your branch is up-to-date with 'origin/master'.
Changes to be committed:
  (use "git reset HEAD <file>..." to unstage)

    deleted:    PROJECTS.md
```

```r - if normal remove
## -----> rm PROJECTS.md
## -----> git status
On branch master
Your branch is up-to-date with 'origin/master'.
Changes not staged for commit:
  (use "git add/rm <file>..." to update what will be committed)
  (use "git checkout -- <file>..." to discard changes in working directory)

        deleted:    PROJECTS.md

no changes added to commit (use "git add" and/or "git commit -a")
```

## git rm --f
* -> if we **`modified the file`** or had **`already added it to the staging area`**, we **must force the removal** with the **-f option**
* -> this is a safety feature to **prevent accidental removal of data that hasn't yet been recorded in a snapshot** and **that can’t be recovered from Git**

## git rm --cached
* -> another useful thing we may want to do is to **keep the file in our working tree but remove it from our staging area**
* -> in other words, we may want to **keep the file on our hard drive** but **not have Git track it anymore**
* -> this is particularly useful if we **forgot to add something to our .gitignore file and accidentally staged it**, like **`a large log file`** or **`a bunch of .a compiled files`**
* => to do this, use the **git rm --cached** + pass **`files, directories, and file-glob patterns`**

```r
$ git rm --cached README

## removes all files that have the .log extension in the log/ directory
$ git rm log/\*.log # the backslash (\) in front of the * is necessary because Git does its own filename expansion in addition to shell’s filename expansion

## removes all files whose names end with a ~
$ git rm \*~
```


