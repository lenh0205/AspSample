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
* -> **`verify the interactions between different units`** (_for example: databases or external web services_)
* -> are typically used and triggered by a **Continuous Integration (CI) system** to ensure that the code being integrated does not conflict with other parts of the code
* -> focus on integration with **out-of-process dependencies**, provide confidence that the **`API is ready for deployment`**
* -> however, it's time consuming and hard to diagnose failure (_because when errors happen we don't know exactly what unit cause these errors_)

* _not just about combining multiple classes together but also to involve components like database, network call, external system call, file system call_

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

