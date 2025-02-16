=========================================================================
# Branching strategy
* -> a set of rules that aid the developers on how to go about writing, merging and deploying code and interact with a shared codebase with the help of a version control system like Git

* -> parallel development
* -> enhanced productivity due to efficient collaboration
* -> organized and structured feature releases
* -> clear path for software development process
* -> bug-free environment without disrupting development workflow

* => keeping project repositories organized, error free and avoid the dreaded merge conflicts when multiple developers simultaneously push and pull code from the same repository

## Branch
* -> Branches are independent lines of work, stemming from the original codebase
* -> developers create separate branches for independently working on features so that changes from other developers don't interfere with an individual's line of work. 
* -> developers can easily pull changes from different branches and also merge their code with the main branch
* => this allows easier collaboration for developers working on one codebase.

=========================================================================
> Common Git branching strategies

# GitFlow
* -> **`Master`** - main branch used for product release
* -> **`Develop`** - tách ra từ master, used for ongoing development
* -> **`Feature branches`** - tách từ develop, to develop new features
* -> **`Release`** - tách từ develop, sau khi 1 quá trình development thì ta sẽ có 1 nhánh để assist in preparing a new production release and bug fixing, sau khi xong sẽ merge ngược về master và develop 
* -> **`Hotfix`** - tách ra từ master, sau khi fix xong sẽ được merge ngược về master và develop specifically for critical bug resolution in the production release

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
