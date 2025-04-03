# MongoDB

## MQL - MongoDB Query Language
* https://www.knowi.com/blog/the-best-introduction-to-mongodb-query-language-mql/#:~:text=MQL%2C%20short%20for%20MongoDB%20Query,documents%2C%20and%20perform%20document%20deletions.
* -> MongoDB **stores data in BSON documents inside collections** (similar to tables but more flexible)
* -> **MongoDB does not use SQL** because it is a **NoSQL database**,  its own query language
* -> since **MongoDB Shell** (or Node.js driver) is **`JavaScript-based`**, the **commands look similar to JavaScript**

```mql
db.users.find({ age: { $gt: 30 } })
```
```js
const users = await db.collection("users").find({ age: { $gt: 30 } }).toArray();
```

## BJSON

## Schema
* -> MongoDB is **`schema-less`** - documents in the same collection can have different structures

## Transactions
* -> MongoDB supports **multi-document transactions** (_but they are less commonly used compared to relational databases_)

## Join
* -> MongoDB doesn't have traditional joins but supports **embedding documents** or using **$lookup** for similar functionality
