===========================================================================
# Code Review
* -> code reviews is a part of **`software development cycle`** (saved more money than resolving defects after they were found by customers)
* -> also referred to as **`peer code review`** - a process where 1 or (should) 2 developers analyze a teammate's code, identifying bugs, logic errors, and overlooked edge cases
* _always provide constructive feedback, making mistakes is not a crime and that's the reason we have peer review_

* => improve **code quality** and help **developers grow** professionally

```bash - Example: from 'feature' branch to 'develop' branch in project using GitHub with GitFlow
# quá trình develop:
git checkout develop
git pull origin develop  # Ensure you're up-to-date
git checkout -b feature/my-feature
git add .
git commit -m "Implemented feature XYZ"
git push origin feature/my-feature

# create 'Pull Request' on Github
# -> Go to your GitHub repository.
# -> Click on Pull Requests > New pull request.
# -> Select 'base' as "develop"
# -> Select 'compare' as "feature/my-feature"
# -> Write a clear title and description (what changes were made, why, any dependencies).
# -> Assign reviewers (our teammates, rất có thể gồm 2 người là: bên khách hàng, bên team mình)
# -> Click "Create pull request"
```

===========================================================================
# Best pratices

## Create a "Code Review Checklist"
* -> **a code review checklist** is a predetermined **`set of questions and rules our team will follow during the code review process`**
* -> identify important issues (secure, high-performing, easy to maintain)
* => giving team member the benefit of a consistent, clear and structured guidance on the factors to consider during the process

## Internal Code review metrics
* _some commonly used review metrics include:_

* _i **`Inspection rate`** - thời gian mà team dành ra để review một lượng code nhất định_
* -> dividing lines of code (LoC) by number of inspection hours
* -> Inspection rate should under 500 LOC per hour; do not review for more than 60 minutes at a time (must taking breaks)
* => if it takes a long time to review the code, there may be **`readability issues`** that need to be addressed

* _i **`Defect rate`** - số lượng defect tìm thấy trong khoảng thời gian kiểm tra_
* -> dividing the defect count by hours spent on inspection
* => determine the **`effectiveness of our code review procedures`** (_Ex: if our developers are slow to find defects, we may need better testing tools_)

* _i **`Defect density`** - số lượng defect tìm thấy trong một lượng code nhất định_
* -> dividing the defect count by thousands of lines of code (kLOC)
* => identify **`which components are more prone`** to defects than others and allocating more resources toward the vulnerable components
* _Ex: if one of our web applications has significantly more defects than others in the same project, we may need to assign more experienced developers to work on it_

## Ensure our feedback justifies our stance 
* -> khi yêu cầu chỉnh sửa thì ta cho **lý do tại sao cần sửa?**

* _vì nhiều khi là do sự khác biệt về solution cho 1 vấn đề giữa ta và code author_
* _điều này giúp đỡ tốn thời gian phải hỏi lại, cũng như giúp code author giải quyết các vấn đề tương tự trong tương lai_

## Don't review more than 200-400 lines of code at a time
* -> nghiên cứu cho thấy rằng khi 1 developer review trên 200 lines, their ability to identify defects waned 
* _hay nói cách khác hầu hết lỗi nằm ở 200 dòng đầu tiên_

## Supplement your best practices with automation
* -> if we use **Bitbucket** as our git solution, we can enhance our **source code management (SCM)** workflow with an app like **`Workzone`** - plan how and when to push changes and how to add reviewers and groups to new pull requests
* -> another **Bitbucket** app that can help us to automate our code reviews is **`Code Owners`** for Bitbucket - decide which users in Bitbucket should review pull requests using a concept known as code owners

## Other
* -> when encountering a large pull request as a reviewer, it’s better to suggest to the author to split the pull request to serve one specific thing (not all the changes)

===========================================================================
# Code Review Checklist

## Example
* * _our checklist may include:_ 
* -> **`Functionality and Test coverage`**: Does the code implement the intended requirement? Is there a need to test more cases (edge cases, potential error)? Does the code include appropriate unit tests or integration tests?
* -> **`Readability and Maintainability`**: Are there any redundant comments in the code? Does code follow code convention?
* -> **`Security`**: Does the code expose the system to a cyber attack? Is user input validated and sanitized properly?
* -> **`Architecture, Code Structure and Design`**: Does the code follow established design patterns and architectural guidelines? Is the code modular and maintainable? Does the code use encapsulation and modularization to achieve separation of concerns?
* -> **`Reusability and Dependencies`**: Does the code use existing reusable components, libraries, functions, and services? Are any unnecessary dependencies or duplicate code segments removed?
* -> **`Performance and Efficiency`**: Are there any potential performance bottlenecks or memory leaks? Are algorithms and data structures appropriate and efficient? Are there any opportunities for caching or parallelization?
* -> **`Error Handling and Logging`**: Are exceptions used appropriately and caught at the correct level? Is logging implemented for debugging and troubleshooting purposes?

## Tips
* -> start with reviewing the **most critical sections** (new features, complex logic, or parts that handle sensitive data) of the code (_avoid big problem become larger_)
* -> use automated tools (_like **`linters`** and **`static analyzers`** (Spotless, husky, etc) can automatically check for simple errors and style issues_) to save time and have more time on things that require human judgment (_like understanding the logic and design of the code_)

===========================================================================
# Other
* https://google.github.io/eng-practices/review/reviewer/
