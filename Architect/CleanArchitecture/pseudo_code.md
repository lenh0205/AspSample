> https://www.plainionist.net/Implementing-Clean-Architecture-UseCases/

# About project
* agile backlog visualizer
* -> mission is to bring maximum transparency into the backlog of our agile teams
* -> it fetches all work items from our Team Foundation Server - "TFS" and visualizes those nicely on an intra-net page to give us the views and reports we need

## 3 core "Use Cases":
* -> show the backlog with projected team capacity (work balance we consider to be a different view on the backlog itself)
* -> calculate a burn down chart
* -> ensure backlog conventions via governance rules

* => so we will create 3 projects/DLLs accordingly: 