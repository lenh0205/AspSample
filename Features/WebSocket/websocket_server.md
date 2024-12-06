
# WebSocket Server 

## Event
* -> "connection" - On the server side, this is equivalent to the 'connection' event on the WebSocket.Server object. It fires when a new client connects.
* -> "close" - This is handled by the 'close' event on each individual WebSocket connection (the `ws` object in our example).
* -> "message" - This is handled by the 'message' event on each individual WebSocket connection.
* -> "error" - There's an 'error' event both on the WebSocket.Server object (for server-wide errors) and on individual WebSocket connections (for connection-specific errors).

## Example
```js
const WebSocket = require('ws');

const wss = new WebSocket.Server({ port: 8080 });

wss.on('connection', function connection(ws) {
  console.log('New client connected');

  ws.on('message', function incoming(message) {
    console.log('Received: %s', message);
    ws.send(`Server received: ${message}`);
  });

  ws.on('close', function close() {
    console.log('Client disconnected');
  });

  ws.send('Welcome to the WebSocket server!');
});

console.log('WebSocket server is running on ws://localhost:8080');
```

## Create different websocket connection endpoint
* -> this is actually **a combination of `WebSocket` and `HTTP-like routing`**