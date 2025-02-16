# Ignore Files - using .gitignore
* -> we'll have a class of files that we **`don’t want Git`** to **automatically add** or even **show us as being untracked**
* -> we can use the **.gitignore** file, **` a file listing patterns`**

* -> these are generally **automatically generated files** such as **`log, tmp, or pid directory; log files; files produced by our build system`**
* -> **`setting up a .gitignore file for new repository before get going`** is generally a good idea so we **`don’t accidentally commit files`** that we really **don’t want in your Git repository**

=========================================================================
# using '.gitignore' in practical project
* -> **`Sample '.gitignore' file`**: https://github.com/github/gitignore

* -> in a repository, beside the **`.gitignore file in its root directory`** (_which **applies recursively to the entire repository**_)
* -> it's possible to have additional **`.gitignore files in subdirectories`** - the rules in these nested .gitignore files **apply only to the files under the directory** where they are located

=========================================================================
# Rules for the patterns
* -> **blank lines** or **lines starting with #** are **`ignored`**
* -> **Standard glob patterns** work, and will be **`applied recursively`** throughout the entire working tree
* -> **start patterns with a forward slash (/)** to **`avoid recursivity`**
* -> **end patterns with a forward slash (/)** to **`specify a directory`**
* -> **starting patterns with an exclamation point (!)** to **`negate a pattern`**

## Glob patterns
* _Glob patterns are like **`simplified`** **regular expressions that shells use**_

* -> **`an asterisk (*)`** matches **zero or more characters**
* -> **`[abc]`** matches **any character inside the brackets** (_in this case a, b, or c_)
* -> **`a question mark (?)`** matches **a single character**
* -> **`brackets enclosing characters separated by a hyphen ([0-9])`** matches **any character between them** (_in this case 0 through 9_)
* -> **``two asterisks`** to **match nested directories**
* -> **`a/**/z`** would match **a/z, a/b/z, a/b/c/z**
* -> so on

```r - Example
# ignore all .a files
*.a

#  ignore any files ending in ".o" or ".a" (usually the archive files the product of code)
*.[oa]

# ignore all files whose names end with a tilde (~)
*~

# but do track lib.a, even though you're ignoring .a files above
!lib.a

# only ignore the TODO file in the current directory, not subdir/TODO
/TODO

# ignore all files in any directory named build
build/

# ignore doc/notes.txt, but not doc/server/arch.txt
doc/*.txt

# ignore all .pdf files in the doc/ directory and any of its subdirectories
doc/**/*.pdf
```

=============================================================================
# Moving Files