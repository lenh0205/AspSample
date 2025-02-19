=========================================================================
# Branching strategy
* a set of rules on how multiple developers can simultaneously work and interact with a shared codebase with the help of a version control system like Git

* -> parallel development
* -> enhanced productivity due to efficient collaboration
* -> organized and structured feature releases
* -> clear path for software development process
* -> bug-free environment without disrupting development workflow

## Branch
* -> is like a separate workspace version where we can make changes to our code without affecting the main project version
* -> once our changes are ready, we merge the branch back into the main branch

=========================================================================
> Common Git branching strategies

# GitFlow
![gitflow](https://nvie.com/img/git-model@2x.png)

* -> **`Master`** - main branch used for production, không bao giờ sửa trực tiếp chỉ hợp nhất từ nhánh `release` và `hotfix`
* -> **`Develop`** - tách ra từ `master`, used for ongoing development
* -> **`Feature branches`** - tách từ `develop`, to develop new features
* -> **`Release`** - tách từ `develop`, sau khi 1 quá trình development thì ta sẽ có 1 nhánh để assist in preparing a new production release and bug fixing, sau khi xong sẽ merge ngược về master và develop 
* -> **`Hotfix`** - tách ra từ `master`, sau khi fix xong sẽ được merge ngược về master và develop specifically for critical bug resolution in the production release

## Command:
```bash
# -----> Tạo nhánh develop
git branch develop
git push -u origin develop

# -----> Bắt đầu phát triển feature: tạo nhánh riêng để phát triển feature
git checkout -b feature/1-feature-01 develop
git status 
git add some-file
git commit

# -----> Hoàn thành feature: merge feature vào dev, đẩy dev lên, rồi xóa feature đi
git pull origin develop
git checkout develop
git merge --no-ff feature/1-feature-01
git push origin develop
git branch -d feature/1-feature-01 # xóa branch local
git push origin --delete feature/1-feature-01 # xóa branch remote


# -----> Bắt đầu phát hành bản release
git checkout -b release-0.1.0 develop

# -----> Hoàn thành bản release: merge release vô dev và master; xóa branch release; thêm tag cho master cho bản Release theo PATCH
git checkout main
git merge --no-ff release-0.1.0
git push

git checkout develop
git merge --no-ff release-0.1.0
git push

git branch -d release-0.1.0 # xóa local
git-follow-action git:(main) git push origin --delete release-0.1.0 # xóa remote

git tag -a v0.1.0 master
git push --tags

# -----> Nếu có lỗi thì tạo nhánh hotfix
git checkout -b hotfix-0.1.1 main

# -----> Hoàn thành hotfix: merge hotfix vô dev và master; xóa hotfix; thêm tage cho master cho bản Release theo PATCH
git checkout main
git merge --no-ff hotfix-0.1.1
git push

git checkout develop
git merge --no-ff hotfix-0.1.1
git push

git branch -d hotfix-0.1.1

git tag -a v0.1.1 master
git push --tags
```

## Cons
* -> complexity - khó quản lý khi có nhiều branch được add thêm
* -> merging branch từ development branches to the main branch requires multiple steps, dẫn đến các nguy cơ lỗi và merge conflict
* -> vì commmit history rất nhiều nên rất khó để debug, slow down the development process and release cycle

# GitHub Flow
* -> a simpler alternative to GitFlow, idea for smaller teams
* -> only has **`feature branches`** (to implement new features or address bugs) that stem directly from the **`master branch`** 
* -> and are merged back to master after completing changes
* -> if a merge conflict arises, developers are required to resolve it prior to finalizing the merge.

* => The fundamental concept of this model revolves around maintaining the master code in a consistently deployable condition, thereby enabling the seamless implementation of faster release cycles, continuous integration and continuous delivery workflows

## Cons
* -> rất dễ làm production unstable nếu code changes không được test trước khi merge
* -> merge conflict sẽ diễn ra thường xuyên hơn do everyone merging changes to the same branch

# GitLab Flow
* -> an alternative to GitFlow, designed to be more robust and scalable than GitHub Flow
* -> this approach streamlines development by concentrating on a solitary, protected branch, usually the master branch. 
* -> continuous integration and automated testing form the core elements of GitLab Flow, guaranteeing the stability of the master branch.

* -> **`Master`**: main production branch housing stable release ready code.
* -> **`Develop`**: contains new features and bug fixes.
* -> **`Feature`**: developers initiate feature branches from the develop branch to implement new features or address bugs. Upon completion, they integrate the changes from the feature branch into the develop branch.
* -> **`Release`**: prior to a new release, a release branch is branched off from the develop branch. This release branch serves as a staging area for integrating new features and bug fixes intended for the upcoming release. Upon completion, developers merge the changes from the release branch into both the develop and main branches.

# Trunk-based development
* -> developers work on **`a single "trunk" branch`**, mostly the **master branch** and use **feature flags** to isolate features until they are ready for release
* -> this main branch should be ready for release any time
* => enables continuous integration and delivery, making it an attractive choice for teams aiming to **release updates swiftly and frequently**

## Cons
* -> thường phù hợp cho senior developer vì nó requires a significant amount of autonomy việc này gây khó khăn cho less experienced developers
* -> demands a considerable level of discipline and effective communication among developers to prevent conflicts and ensure proper isolation of new features

# Picking The Right Branching Strategy
```cs
+-----------------------------------------------------+----------+--------------------------+
|              Product Type                           | Team Size| Applicable Strategy      |
|-----------------------------------------------------|----------|--------------------------|
|  Continuous Deployment and Release                  | Small    | GitHub Flow and TBD      |
|-----------------------------------------------------|----------|--------------------------|
|  Scheduled and Periodic Version Release             | Medium   | GitFlow and GitLab Flow  |
|-----------------------------------------------------|----------|--------------------------|
|  Continuous deployment for quality-focused products | Medium   | GitLab Flow              |
|-----------------------------------------------------|----------|--------------------------|
|  Products with long maintenance cycles              | Large    | GitFlow                  |
+-----------------------------------------------------+----------+--------------------------+
```

* => therefore, teams seeking an **`Agile DevOps workflow`** with strong support for continuous integration and delivery may find **GitHub Flow** or **Trunk-based development** more suitable
* => **GitFlow** is beneficial for projects requiring strict access control, particularly in **`open-source environments`**

=========================================================================
# Branching strategies for agile teams
* https://www.atlassian.com/agile/software-development/branching
