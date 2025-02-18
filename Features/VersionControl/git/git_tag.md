# git tag
* -> used to **`mark specific points in our Git history`**
* -> typically to label important milestones like **`releases`** (e.g., v1.0, v2.0)
* =>  like **bookmarks** (_mark releases, identify stable points in the project_) for commits, making it easy to **reference them later**

```bash - create tag
git tag v1.0
git tag -a v1.0 -m "Version 1.0 release" # add metadata: -a (author), -m (message), -d (date)
```

```bash - reference specific versions
git tag -a v1.0 -m "Stable release v1.0"
git checkout v1.0
```

```bash - information
git tag # list all tag
git show v1.0 # show tag detail 
```

```bash - other
git push origin v1.0 # Push a tag to a remote repository
git tag -d v1.0 # Delete a local tag
git push --delete origin v1.0 # Delete a remote tag
```
