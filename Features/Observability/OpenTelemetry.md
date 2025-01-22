> when it comes to managing complex multicloud environments and the services that run on them, it’s important to **standardize**
> there are OpenTelemetry implementations for most languages and platforms (including .NET)

===============================================================================
# OpenTelemetry (OTel)
* -> is an **`open source Observability framework`** built on open standards made up of **a collection of tools, APIs, and SDKs** to **generate, collect, manage and export telemetry data (such as traces, metrics, and logs)**
* -> was born from the combination of **`OpenCensus`** and **`OpenTracing`** and has emerged as **the open source telemetry standard**

## Purpose
* -> a major goal of OpenTelemetry is that we can **`easily instrument our applications or systems, no matter their language, infrastructure, or runtime environment`**
* -> having **`a standard format for how observability data is collected and sent`** 
* -> provide **unified sets of vendor-agnostic libraries and APIs** (_mainly for collecting data and transferring it somewhere_)
* -> supports logs, metrics, and traces in a single framework, with hundreds of popular integrations already available

* => OTel enables IT teams to **instrument, generate, collect, and export telemetry data** for **`analysis and to understand software performance and behavior`** 
* -> **Operators** monitor systems with OpenTelemetry to identify and troubleshoot issues quickly
* -> **Developers** create custom telemetry data using OpenTelemetry's APIs
* -> **Security teams** use OpenTelemetry to understand the security posture of their systems

## Note:
* -> **`the storage and visualization of telemetry`** is intentionally **left to other tools**

## Usage
* -> OTel is a **specialized protocol** for **`collecting telemetry data`** and **`exporting it to a target system`**
* -> **OpenTelemetry's API** is a standard that developers use to **`instrument applications and infrastructure across many different technologies`**
* -> the **OTel Collector** collects telemetry data and exports it to **`monitoring systems, logging platforms, and other backends`**

## Telemetry data
* -> **`logs, metrics, and traces`** make up most of all telemetry data

* -> capturing data is critical to understanding how our applications and infrastructure perform at any given time
* -> this information is gathered from remote, **`often inaccessible points`** within our ecosystem and processed by some tool or equipment
* => **monitoring** begins here
* -> the data is incredibly plentiful and difficult to store over long periods because of capacity limitations
* -> as a result, private and public cloud storage services have been a boon to DevOps teams

## Architect
* https://assets.dynatrace.com/en/docs/ebook/16927-ebk-open-telemetry-opportunity-for-intelligent-observability.pdf?_gl=1*19rhqha*_ga*MTYyMDA2NDIzLjE3Mzc1MTM4Nzc.*_ga_1MEMV02JXV*MTczNzUxMzg3Ni4xLjEuMTczNzUxNDAzMS4wLjAuMA..*_gcl_aw*R0NMLjE3Mzc1MTM4NzcuQ2owS0NRaUFxTDI4QmhDckFSSXNBQ1lKdmtkWGJjX2JsU1ZaenY5UnhidHhOTnVFT1hFaGNKX0FZbThvMHdia0c2eV9mNk5YRE1ReGxnd2FBbkw5RUFMd193Y0I.*_gcl_dc*R0NMLjE3Mzc1MTM4NzcuQ2owS0NRaUFxTDI4QmhDckFSSXNBQ1lKdmtkWGJjX2JsU1ZaenY5UnhidHhOTnVFT1hFaGNKX0FZbThvMHdia0c2eV9mNk5YRE1ReGxnd2FBbkw5RUFMd193Y0I.*_gcl_au*MjEzOTc1MTMzMi4xNzM3NTEzODg0

## Support
* -> **`APIs for libraries`** to use to **record telemetry data as code is running**

* -> **`APIs that app developers use to configure`** what **portion of the recorded data will be sent across the network, where it will be sent to, and how it may be filtered, buffered, enriched, and transformed**

* -> **`Semantic conventions`** provide **guidance on naming and content of telemetry data**
* -> it is important for **the apps that produce telemetry data** and **the tools that receive the data** to agree on what different kinds of data means and what sorts of data are useful so that the tools can provide effective analysis

* -> an **`interface for exporters`**; "Exporters" are plugins that **allow telemetry data to be transmitted in specific formats to different telemetry backends**

* -> **`OTLP wire protocol`** is **a vendor neutral network protocol option for transmitting telemetry data** - some tools and vendors support this protocol in addition to pre-existing proprietary protocols they may have

* => OTel is **`vendor- and tool-agnostic`** - enables the use of a wide variety of **`APM systems`** (**`Observability backends`**) 
* _including open-source systems such as **Prometheus** and **Grafana**, **Azure Monitor** (Microsoft's APM product in Azure), ..._

===============================================================================
# OpenTelemetry vs Prometheus
OpenTelemetry and Prometheus are both open-source tools used for collecting, storing, and analyzing telemetry data, but they have some key differences in their capabilities and focus.

OpenTelemetry is a vendor-neutral observability framework that provides a unified set of APIs, libraries, and agents for collecting telemetry data from various sources, including applications, infrastructure, and security tools. It can be used to instrument applications and services in a language-agnostic way, and it supports a wide range of telemetry data types, including traces, metrics, and logs. OpenTelemetry is designed to be extensible and customizable, allowing users to integrate it with various backends, including logging systems, tracing platforms, and monitoring tools.

On the other hand, Prometheus is a monitoring and alerting system primarily focused on time-series metrics data. It is designed to collect metrics data from various sources, including applications, services, and infrastructure components, and to store them in a time-series database. Prometheus provides a powerful query language for analyzing metrics data, and it supports a wide range of alerting options.

While both OpenTelemetry and Prometheus can be used for monitoring and analytics, they have different strengths and use cases. OpenTelemetry is more focused on observability and telemetry data collection, while Prometheus is more focused on monitoring and alerting based on metrics data. OpenTelemetry is generally better suited for distributed systems that generate various telemetry data types, while Prometheus is better suited for monitoring infrastructure components and services that generate metrics data.


===============================================================================
https://help.sumologic.com/docs/apm/traces/get-started-transaction-tracing/opentelemetry-instrumentation/
