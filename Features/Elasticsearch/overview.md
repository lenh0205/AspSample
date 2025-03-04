==============================================================
# Elasticsearch
* -> is **`a distributed search and analytics engine`** built on Apache Lucene
* -> the most popular search engine and is commonly used for **log analytics**, **full-text search**, **security intelligence**, **business analytics**, and **operational intelligence** use cases

## License
* -> On January 21, 2021, Elastic NV announced that they would change their software licensing strategy and not release new versions of **Elasticsearch and Kibana** under the permissive **Apache License, Version 2.0 (ALv2) license**
* -> new versions of the software will be offered under the **Elastic license**, with source code available under the Elastic License or SSPL
* -> these licenses are **`not open source`** and do not offer users the same freedoms

* => to ensure that the open-source community and our customers continue to have a secure, high-quality, fully open-source search and analytics suite, AWS introduced the **`OpenSearch`** project
* -> a community-driven, ALv2 licensed fork of open-source Elasticsearch and Kibana

## How does Elasticsearch work?
* -> we can **`send data in the form of JSON documents`** to Elasticsearch using **the API or ingestion tools such as Logstash and Amazon Data Firehose**
* -> Elasticsearch **`automatically stores the original document`** and **`adds a searchable reference to the document in the cluster's index`**
* -> we can then search and retrieve the document using the **`Elasticsearch API`**
* -> we can also use **Kibana**, a visualization tool, with Elasticsearch to visualize your data and build interactive dashboards

## Elasticsearch benefits
Fast time-to-value
Elasticsearch offers simple REST-based APIs, a simple HTTP interface, and uses schema-free JSON documents, making it easy to get started and quickly build applications for various use cases.

High performance
The distributed nature of Elasticsearch enables it to process large volumes of data in parallel, quickly finding the best matches for your queries.

Complimentary tooling and plugins
Elasticsearch comes integrated with Kibana, a popular visualization and reporting tool. It also offers integration with Beats and Logstash, helping you easily transform source data and load it into your Elasticsearch cluster. You can also use various open-source Elasticsearch plugins such as language analyzers and suggesters to add rich functionality to your applications.

Near real-time operations
Elasticsearch operations such as reading or writing data usually take less than a second to complete. This lets you use Elasticsearch for near real-time use cases such as application monitoring and anomaly detection.

Easy application development
Elasticsearch provides support for various languages including Java, Python, PHP, JavaScript, Node.js, Ruby, and many more.

==============================================================
# Elasticsearch vs SQL Full-Text Search