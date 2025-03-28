
# Fast-Forward Merge
* -> đây là default của git khi ta merge 2 branch lại
* -> Git can **`move the branch pointer forward without creating a new commit`** nếu the branch being merged has no other commits diverging from the main branch

```bash - trường hợp Git sẽ mặc định sử dụng Fast-forward
A -- B (main)
       \
        D -- E (feature-branch)

# thì sau khi merge 'feature-branch' vô 'main' sẽ tạo ra a 'linear history':
# the history looks like the feature was developed directly on main, making it harder to track
A -- B -- C -- D -- E (main) 
```

```bash - trường hợp không thể Fast-forward
A -- B -- C (main)
       \
        D -- E (feature-branch)

# thì sau khi merge 'feature-branch' vô 'main':
A -- B -- C -- M (main)
       \       /
        D -- E (feature-branch)
```

## Chỉ định No Fast-Forward Merge với '--no-ff'
* -> _ghi đè hành vi mặc định_
* -> **`a new merge commit is always created`** (even if a fast-forward is possible); keeps the feature branch history separate

```bash
$ git checkout main
$ git merge --no-ff feature-branch
```

## Visualize the graph
```bash
$ git log --graph --oneline --decorate
$ git log --graph --oneline --decorate --all

$ git log -- path/to/file # check the history of a specific file
```
