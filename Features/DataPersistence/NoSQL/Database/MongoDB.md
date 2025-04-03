# MongoDB

## MQL - MongoDB Query Language
* -> MongoDB **stores data in BSON documents inside collections** (similar to tables but more flexible)
* -> **MongoDB does not use SQL** because it is a **NoSQL database**,  its own query language

```mql
db.Users.find({ "Age": { $gt: 25 } })
```

## BJSON

## Schema
* -> MongoDB is **`schema-less`** - documents in the same collection can have different structures

## Transactions
* -> MongoDB supports **multi-document transactions** (_but they are less commonly used compared to relational databases_)

## Join
* -> MongoDB doesn't have traditional joins but supports **embedding documents** or using **$lookup** for similar functionality
