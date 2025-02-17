====================================================================
# Test Pyramid
* -> visualizes different types of **`automated tests`** by categorizing them based on their **complexity** and **execution speed**
* _the higher a test is on the pyramid, the slower it will be in overall execution_

====================================================================
## Unit Tests
* -> **`verify a small piece of code`** (a unit) in quickly and isolated
* -> focus on the domain (_not involving database, external system, file system, network, ..._)

* _Ex: class, method, ..., something very small and isolated_

## Integration Tests
* -> ensure that an app's components function correctly at a level that includes the **app's supporting infrastructure** (_such as the **`database`**, **`file system`**, and **`network`**_)
* -> ASP.NET Core supports integration tests using a **`unit test framework`** with a **test web host** and an **in-memory test server**

* -> in contrast to **unit tests** - that use fabricated components (**`fakes or mock objects`**) in place of infrastructure components, **integration tests** use the **`actual components that the app uses in production`**

* => the integration test require more code, data processing and take longer to run so limit the use of integration tests to the most important infrastructure scenarios, 
* -> don't write integration tests for every permutation of data and file access with databases and file systems

## End To End Tests (E2E)
* -> **`validate the system from the user's perspective`** (_what happen if user use the system from the UI to backend then to database_)
* -> extremely time consuming; very fragile; hard to maintain, diagnose and debug; challenging to prepare test enviroment / data setup

## Contract Tests
* -> **`ensure the contracts between services are honored`** 
* -> allow making service changes without affecting other services
* -> early detection of integration issues 
* -> no need to stand up entire system for testing 
* -> promote communication and collaboration between teams
* -> contracts become the living documentation of services interactions

====================================================================

