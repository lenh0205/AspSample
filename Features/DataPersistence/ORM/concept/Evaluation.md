> Prior to version 3.0, Entity Framework Core supported client evaluation anywhere in the query

================================================================
# Client Evaluation vs Server Evaluation

## General "rule" in Entity Framework
* -> As a general rule, Entity Framework Core attempts to evaluate a query on the server as much as possible. EF Core converts parts of the query into parameters, which it can evaluate on the client side. The rest of the query (along with the generated parameters) is given to the database provider to determine the equivalent database query to evaluate on the server

## Partial client evaluation
* -> EF Core supports partial client evaluation in the top-level projection (essentially, the last call to Select()). If the top-level projection in the query can't be translated to the server, EF Core will fetch any required data from the server and evaluate remaining parts of the query on the client. If EF Core detects an expression, in any place other than the top-level projection, which can't be translated to the server, then it throws a runtime exception

## what can't be translated to server
https://learn.microsoft.com/en-us/ef/core/querying/how-query-works 

================================================================
# Client evaluation in the top-level projection


================================================================
# Unsupported client evaluation

================================================================
# Explicit client evaluation

================================================================
# Potential memory leak in client evaluation

================================================================
# Previous versions