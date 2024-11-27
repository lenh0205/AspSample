# WebSocket
* -> the **`WebSocket protocol`** (_described in the specification RFC 6455_), provides a way to **`exchange data between browser and server`** via **a persistent connection** 
* -> the data can be passed in **`both directions`** as **packets**, without breaking the connection and the need of additional HTTP-requests

* => WebSocket is especially great for services that **`require continuous data exchange`**
* _e.g. online games, real-time trading systems and so on_

## Note
* WebSockets **don’t have cross-origin limitations**
* they are **`well-supported in browsers`**
* **`WebSocket by itself`** does not include **reconnection**, **authentication** and many other high-level mechanisms
* (_but it's possible to implement these capabilities manually or using some client/server libraries_)
* sometimes, to integrate WebSocket into existing projects, people **`run a WebSocket server in parallel with the main HTTP-server`**, and they share a single database
* (_requests to WebSocket use wss://ws.site.com, **`a subdomain`** that leads to the WebSocket server, while https://site.com goes to the **`main HTTP-server`**_)

## Open a websocket connection
* -> need to create **new WebSocket** using the special protocol **ws** or **wss** (_encrypted protocol_) in the url
* -> if we’d like to send something, then **socket.send(data)** will do that

* the **wss** protocol is **`not only encrypted, but also more reliable`**
* -> the ws:// data is not encrypted, **`visible for any intermediary`**
* -> **`old proxy servers`** do not know about WebSocket, they may see "strange" headers and **`abort the connection`**
* -> the wss:// is **WebSocket over TLS**, (same as HTTPS is HTTP over TLS), the transport security layer encrypts the data at the sender and decrypts it at the receiver
* => so data packets are **`passed encrypted through proxies`**; they can’t see what’s inside and let them through

## 4 Events of Socket
* _Once the socket is created, we can listen to events on it. There are totally 4 events:_
* -> **open** – connection established
* -> **message** – data received
* -> **error** – websocket error
* -> **close** – connection closed

```js - create web socket connection
let socket = new WebSocket("wss://javascript.info/article/websocket/demo/hello");

socket.onopen = function(e) {
  alert("[open] Connection established");
  alert("Sending to server");
  socket.send("My name is John");
};

socket.onmessage = function(event) {
  alert(`[message] Data received from server: ${event.data}`);
};

socket.onclose = function(event) {
  if (event.wasClean) {
    alert(`[close] Connection closed cleanly, code=${event.code} reason=${event.reason}`);
  } else {
    // e.g. server process killed or network down
    // event.code is usually 1006 in this case
    alert('[close] Connection died');
  }
};

socket.onerror = function(error) {
  alert(`[error]`);
};
```

```js - create a server for websocket
// server responds with “Hello from server, John”, then waits 5 seconds and closes the connection
// events open -> message -> close

const http = require('http');
const ws = require('ws');

const wss = new ws.Server({noServer: true});

function accept(req, res) {
  // all incoming requests must be websockets
  if (!req.headers.upgrade || req.headers.upgrade.toLowerCase() != 'websocket') {
    res.end();
    return;
  }

  // can be Connection: keep-alive, Upgrade
  if (!req.headers.connection.match(/\bupgrade\b/i)) {
    res.end();
    return;
  }

  wss.handleUpgrade(req, req.socket, Buffer.alloc(0), onConnect);
}

function onConnect(ws) {
  ws.on('message', function (message) {
    message = message.toString();
    let name = message.match(/([\p{Alpha}\p{M}\p{Nd}\p{Pc}\p{Join_C}]+)$/gu) || "Guest";
    ws.send(`Hello from server, ${name}!`);

    setTimeout(() => ws.close(1000, "Bye!"), 5000);
  });
}

if (!module.parent) {
  http.createServer(accept).listen(8080);
} else {
  exports.accept = accept;
}
```

======================================================
# Open a websocket
* -> _when new WebSocket(url) is created_, it starts **`connecting immediately`** (_tức là nó ngay khi tạo đối tượng WebSocket thì nó sẽ gửi request ngay lập tức_)
* -> during the connection, the browser (using headers) **`asks the server: Do you support Websocket?`** 
* -> _if the server replies "yes"_, then the **`talk continues in WebSocket protocol, which is not HTTP at all`**

## Browser Header
* **`Note`**: _can’t use "XMLHttpRequest" or "fetch" to make this kind of HTTP-request, because **JavaScript is not allowed to set these headers**_

* _browser headers for a request made by new WebSocket("wss://my-url"):_
* **Origin** 
* –> the origin of the client page, e.g. https://javascript.info
* -> WebSocket objects are **cross-origin by nature**
* -> _there are no special headers or other limitations_
* -> **`old servers are unable to handle WebSocket anyway`**, so there are no compatibility issues
* -> but the Origin header is important, as it **`allows the server to decide`** whether or not to talk WebSocket with this website

* **Connection: Upgrade** – signals that the client would like to **`change the protocol`**

* **Upgrade: websocket** – the requested protocol is **`websocket`**

* **Sec-WebSocket-Key** 
* –> **`a random browser-generated key`**, used to ensure that the **`server supports WebSocket protocol`**
* -> it’s random to prevent proxies from caching any following communication

* **Sec-WebSocket-Version** – WebSocket protocol version, 13 is the current one

```r - browser headers for a request 
// made by new WebSocket("wss://javascript.info/chat")

GET /chat
Host: javascript.info
Origin: https://javascript.info
Connection: Upgrade
Upgrade: websocket
Sec-WebSocket-Key: Iv8io/9s+lYFgZWcXczP8Q==
Sec-WebSocket-Version: 13
```

```r - server response
// If the server agrees to switch to WebSocket, it should send code 101 response:

101 Switching Protocols
Upgrade: websocket
Connection: Upgrade
Sec-WebSocket-Accept: hsBlbuDTkk24srzEOTBUlZAlC2g=

// => here "Sec-WebSocket-Accept" is actually "Sec-WebSocket-Key", recoded using a special algorithm
// => Upon seeing it, the browser understands that the server really does support the WebSocket protocol
// => afterwards, the data is transferred using the WebSocket protocol
```

===================================================
# Data Transfer
* _WebSocket communication consists of **`frames`** – data fragments, that can be sent from either side, and can be of several kinds:_
* -> **text frames** – contain **`text data`** that parties send to each other
* -> **binary data frames** – contain **`binary data`** that parties send to each other
* -> **ping/pong frames** are used to **`check the connection, sent from the server`**, the browser responds to these automatically
* -> there’s also **`connection close frame`** and a few other service frames

* _s **`in the browser`**, we directly work only with text or binary frames_

## "WebSocket.send()" method
* -> can send either **`text or binary data`**
* -> allows **`body`** in **string** or a binary format (_including **Blob**, **ArrayBuffer**, **TypedArray**_); (_no settings are required: just send it out in any format_)
* -> when we **`receive the data`**, text always comes as string. And for binary data, we can choose between Blob and ArrayBuffer formats
* (_that’s set by **socket.binaryType** property, it’s **`blob by default`**, so binary data comes as Blob objects_)

```js
// -----> Application A:
const socket = new WebSocket('ws://application-b');
const file = new File(["Hello, World!"], "example.txt", { type: "text/plain" });

// Send metadata
socket.onopen = () => {
    const metadata = {
        type: 'file-metadata',
        fileName: file.name,
    };
    socket.send(JSON.stringify(metadata));

    // Read and send the file as a binary blob
    const reader = new FileReader();
    reader.onload = () => socket.send(reader.result);
    reader.readAsArrayBuffer(file);
};

// -----> Application B:
const socket = new WebSocket('ws://application-b');
let fileBuffer = [];

socket.onmessage = (event) => {
    const data = event.data;
    if (typeof data === 'string') {
        const metadata = JSON.parse(data);
        console.log('Metadata received:', metadata);
    } else {
        fileBuffer.push(data);
    }
};

```

* _s **`Blob is a high-level binary object`**, it directly integrates with <a>, <img> and other tags, so that’s a sane default_
* _But for **`binary processing`**, to access individual data bytes, we can change it to **arraybuffer**_
```js
socket.binaryType = "arraybuffer";
socket.onmessage = (event) => {
  // event.data is either a string (if text) or arraybuffer (if binary)
};
```

=======================================================
# Rate limiting
* imagine, our app is generating a lot of data to send. But the user has a slow network connection (_maybe on a mobile internet, outside of a city_)
* we can **`call socket.send(data) again and again`**. But the **`data will be buffered (stored) in memory`** and sent out only as fast as network speed allows

* => The **socket.bufferedAmount** property stores how many **`bytes remain buffered at this moment`** (_that waiting to be sent over the network_)

```js
// every 100ms examine the socket and send more data
// only if all the existing data was sent out
setInterval(() => {
  if (socket.bufferedAmount == 0) {
    socket.send(moreData());
  }
}, 100);
```

=========================================================
# Connection close
* -> normally, when a party wants to **`close the connection`** (**both browser and server have equal rights**)
* -> they send a **connection close frame** with **`a numeric code`** (_a special WebSocket closing code_) and **`a textual reason`** (_describes the reason of closing_)

```js
// closing party:
socket.close([code], [reason]);

// Then the other party in the "close" event handler gets the code and the reason
socket.onclose = event => {
  // event.code === 1000
  // event.reason === "Work complete"
  // event.wasClean === true (clean close)
};

// in case connection is broken
socket.onclose = event => {
  // event.code === 1006
  // event.reason === ""
  // event.wasClean === false (no closing frame)
};
```

* the full list of **`code values`** can be found in **RFC6455 §7.4.1.**:
* -> 1000 – the default, normal closure (used if no code supplied),
* -> 1006 – no way to set such code manually, indicates that the connection was lost (no close frame).
There are other codes like:

* -> 1001 – the party is going away, e.g. server is shutting down, or a browser leaves the page,
* -> 1009 – the message is too big to process,
* -> 1011 – unexpected error on server,
* -> …and so on.
* _WebSocket codes are somewhat like HTTP codes, but different. In particular, codes lower than 1000 are reserved, there’ll be an error if we try to set such a code_

# Connection state
* -> to get connection state, additionally there’s **socket.readyState** property with values:
* **0** – **`CONNECTING`**: the connection has not yet been established,
* **1** – **`OPEN`**: communicating,
* **2** – **`CLOSING`**: the connection is closing,
* **3** – **`CLOSED`**: the connection is closed

=====================================================
# Chat App Example
* _using browser **`WebSocket API`** and Node.js **`WebSocket module`**_

* a **<form/>** to **`send messages`** and a **<div/>** for **`incoming messages`**
```html
<!-- message form -->
<form name="publish">
  <input type="text" name="message">
  <input type="submit" value="Send">
</form>

<!-- div with messages -->
<div id="messages"></div>
```

* Open connection; **socket.send(message)** for the message on **`form submission event`**;  **append data to div#messages** on **`incoming message event`**
```js
let socket = new WebSocket("wss://javascript.info/article/websocket/chat/ws");

// send message from the form
document.forms.publish.onsubmit = function() {
  let outgoingMessage = this.message.value;

  socket.send(outgoingMessage);
  return false;
};

// message received - show the message in div#messages
socket.onmessage = function(event) {
  let message = event.data;

  let messageElem = document.createElement('div');
  messageElem.textContent = message;
  document.getElementById('messages').prepend(messageElem);
}
```

* The server-side algorithm will be:
* -> Create clients = new Set() – a set of sockets.
* -> for each accepted websocket, add it to the set clients.add(socket) and set message event listener to get its messages.
* -> When a message is received: iterate over clients and send it to everyone.
* -> When a connection is closed: clients.delete(socket)

```js
const ws = new require('ws');
const wss = new ws.Server({noServer: true});

const clients = new Set();

http.createServer((req, res) => {
  // here we only handle websocket connections
  // in real project we'd have some other code here to handle non-websocket requests
  wss.handleUpgrade(req, req.socket, Buffer.alloc(0), onSocketConnect);
});

function onSocketConnect(ws) {
  clients.add(ws);

  ws.on('message', function(message) {
    message = message.slice(0, 50); // max message length will be 50

    for(let client of clients) {
      client.send(message);
    }
  });

  ws.on('close', function() {
    clients.delete(ws);
  });
}
```
