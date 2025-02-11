
# Log4Net
* -> lâu đời
* -> its configuration is primarily **XML-based** - flexibility (easy to manage and update logging settings without changing code) but verbosity 
* -> support various appenders: console, file, database, ...

* => robustness and stability in production enviroments - suitable for enterprise applications

# NLog
* -> offers a more modern approach with a simpler configuration system
* -> often using an easier to understand configuration file (XML or JSON) or code-based settings
* -> supports various targets (even email)
* -> **`high performance`** - NLog performance is generally considered excellent

* => NLog is a good option for a mature well-documented and very flexible option

# Serilog
* -> offers a fluent API for **configuration within our code** - more flexibility but requiring more coding effort
* -> and strong focus on **`structured logging`** 

* => means we can **log data in a structured format like JSON** - make it easy to search to search, analyze and correlate log events
* => its **extensibility** is remarkable, allowing integration with various sinks and enrichers to customize our logging experience