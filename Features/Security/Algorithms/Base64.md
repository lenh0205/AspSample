# Base64
* -> encoding scheme that **`represents binary data`** in a **`printable ASCII format`**
* -> is the most popular **`binary-to-text`** algorithm used to **convert data as plain text** 

* -> to **`prevent data corruption during transmission`** between different storage mediums
* -> in addition, it is often used to **`embed binary data into text documents`** such as _HTML, CSS, JavaScript, or XML_

## btoa
* -> in this approach, we're using btoa to encode **`a UTF-8 string representation of a JSON object`**
* _first, the `JSON data is converted to a UTF-8 string` using **unescape** and **encodeURIComponent**_
* _then **btoa** encodes `this UTF-8 string to Base64`_

```js
const jData = { name: 'GFG', age: 30 };
const utf8Str = unescape(encodeURIComponent(JSON.stringify(jData)));
const res = btoa(utf8Str);
console.log(res); // eyJuYW1lIjoiR0ZHIiwiYWdlIjozMH0=
```

## manually
* -> in this approach, we're **`manually converting a JSON string to Base64`**
* _the function encodes the `UTF-16 characters of the JSON string into Base64`_
* _using a custom algorithm that follows the `Base64 encoding table` (including handling padding characters ('='))_

```js
const jData = {
    name: 'GFG',
    age: 30
};
const jStr = JSON.stringify(jData);
const baseTable = 
    'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/';

function conversionFn(str) {
    let res = '';
    let i = 0;
    while (i < str.length) {
        const c1 = str.charCodeAt(i++);
        const c2 = str.charCodeAt(i++);
        const c3 = str.charCodeAt(i++);
        const triplet = (c1 << 16) | (c2 << 8) | c3;
        const b64c1 = baseTable.
            charAt((triplet >>> 18) & 0x3F);
        const b64c2 = baseTable.
            charAt((triplet >>> 12) & 0x3F);
        const b64c3 = baseTable.
            charAt((triplet >>> 6) & 0x3F);
        const b64c4 = baseTable.
            charAt(triplet & 0x3F);
        res += b64c1 + b64c2 + b64c3 + b64c4;
    }
    const padding = str.length % 3;
    if (padding === 1) {
        res = res.slice(0, -2) + '==';
    } else if (padding === 2) {
        res = res.slice(0, -1) + '=';
    }
    return res;
}
const res = conversionFn(jStr);
console.log(res); // eyJuYW1lIjoiR0ZHIiwiYWdlIjozMH0=
```
