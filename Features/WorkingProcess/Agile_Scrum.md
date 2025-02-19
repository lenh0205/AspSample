> https://www.youtube.com/watch?v=n2XLNlThjms
> https://www.youtube.com/watch?v=vuBFzAdaHDY
> PI - Program Increment
> liệu khách hàng có thêm yêu cầu thì có thêm tiền không ?

# Waterfall
* -> các step có thể bao gồm: Plan -> Build -> Test -> Review -> Deploy
* -> việc sai ở mỗi bước có thể sẽ khiển ta phải bắt đầu làm lại từ bước trước đó, có khi sẽ phải làm lại từ đầu
* -> lengthy planning process that required to completed before any work begins
* => rất có thể sản phẩm ra mắt cuối cùng lại sai so với market demand or technology vì chúng đã thay đổi so với original plan

# Agile
* _là phương thức phát triển phần mềm - chia nhỏ requirement thành `product backlog`, chia product backlog thành những `Sprint backlog` bao gồm những `User Stories` bên trong, và bỏ những "User Stories" vào `Sprint`_
* -> cá nhân và sự tương hỗ quan trọng hơn tool
* -> self-organizing - Ví dụ có thể không cần PM assign requirement, mà từng cá nhân có thể tự xử luôn
* -> cộng tác với khách hàng quan trọng hơn hợp đồng
* -> phản hồi với sự thay đổi quan trong hơn kế hoạch
* -> sản phẩm dùng được quan trọng hơn là document

* => delivery nhanh nhất có thể và flexible; cho phép khách hàng thấy sản phầm liên tục và liên tục đưa qua yêu cầu

## Scum có thể fail
* -> Scum master không thể estimate, phần công việc hiệu quả
* -> trong product backlog, trong epic sẽ có nhiều user stories sẽ có nhiều task

# Scrum - implementation of Agile
* -> process is broken up into smaller pieces

* _1 proccess nhỏ (**`a Sprint`**) kéo dài từ **`2-4 tuần`** sẽ bao gồm các bước:_
* -> đầu tiền lên plan vừa đủ để start 
* -> xây dựng 1 tập chức năng tối thiểu đã được lên plan
* -> test và review tập chức năng nhỏ đó
* -> get it ready to ship a product

* -> các process tương tự sẽ được lặp lại lần này đến lần khác tạo ra product increment đến khi các feature của product hoàn thiện
* => giảm thời gian từ planning đến development cho đến testing

## 3 Key Roles
* _required for framework to work well_

* **`Production Owner`**
* -> define what kind of software or product is to be built
* -> defining features that are needed in the product (base on customer feedback) (create, prioritiez, accept stories; release planning)
* _(có thể làm việc với khách hàng trực tiếp hoặc gián tiếp qua `Business Analyst` customer support, sales team)_
* _**Business Analyst** (nếu có) complement the PO in translating business needs into actionable technical requirements, ensures that requirements are well-understood and actionable_

* -> **`Scum Master`** - a servant leader to the team; reponsible for protecting (from outside influence) the team and the process, running the meetings and keeping things going
* _có thể là nhiều vị trí khác nhau trong team_

* -> **`Team`** - có thể bao gồm B.A, U.X, software engineer, QA, Tester, writers, ... để help in building product
* -> mỗi thành viên sẽ self-managing/không có manager và sẽ làm việc cùng nhau
* -> cross-functional - tức là một thành viên có thể đảm nhiệm nhiều vị trí khác nhau để hoàn thành công việc nhanh hớn (việc này sẽ khó thực hiện đối với tổ chức lớn)
* -> từng thành viên sẽ estimate độ khó/effort để hoàn thành task

## 3 Artifacts (or Document)
* **`Product Backlog`** 
* -> where **Product Owner** create a **prioritized list of features** của product known as **`user stories`**; 
* _cái list này sẽ evolve và changes with every Sprint_
* -> **Product owner** sẽ mang 1 số items cao nhất đưa cho **Team**

* **`User Stories`** - a way of describing **a feature set** that follows this construct **`As a ..., I need ..., So that ...`**
* => bằng cách diễn đặt 1 user story như này, nó cho phép **Product Owner** to specify the right amount of detail for the team **`to estimate the size of the task`**
* -> the highest priority user stories go into **Sprint Backlog**, nó sẽ được **estimated for size** và **committed to for the next Sprint**

* **`BurndownChart`** - show the progress during a Sprint on the completion of tasks in the Sprint backlog
* -> this chart should approach zero points as the work is being completed

## 3 Ceremonies
* _think of these as meetings or discussions_

* **`Sprint planning`** - where the **Product Owner**, **Scrum Master** and **Team** meet to discuss the **`user stories`** and **`estimate their relative sizes`**
* -> choose the top priority user stories (so called **`Sprint Backlog`**) from **Product Backlog**, do estimate để **Team** có thể deliver trong 1 single Sprint tiếp theo

* **`Daily Scrum`** (mỗi ngày) - a brief standup meeting where the team member answer 3 questions
* -> **what they have completed since the previous meeting ?**
* -> **what they're working on ?**
* -> **anything that might be block or need help ?**

* **`Sprint Review`** and **`Sprint Retrospective`** - occurs at the end of the Sprint 
* -> **Sprint Review** - **Team** showcase completed work and demo of new features add in product to the **Product Owner**
* -> **Sprint Retrospective** - **Team** suy ngẫm về cái gì làm tốt, chưa tốt, và có thể cải thiện

## Flow
* -> **`Product Backlog`**
* -> **`Sprint Planning`** (các thành viên sẽ có solid understanding của từng user stories trong **Sprint Backlog**)
* -> **`Sprint Backlog`** - a list of user stories that have been committed to for the next Sprint (output of Sprint plainning)

* -> **`Sprint`** - the work committed to in the Sprint backlog is worked on through to completion
* _during Sprint, the **Daily Scrum** occurs_

* -> **`Potential Shippable Product`** (outcome of a sprint) - the **Product Owner** can decide if it is ready to ship or if there are any additional features needed before it shipped

* -> **`Sprint Review`** and **`Sprint Retrospective`** 

## Software to Manage the work flow

