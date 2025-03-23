===============================================================================
# Message Model
* -> there are two commonly messaging models, the **`point-to-point`** and the **`publish/subscribe model`**
* -> both of these messaging models are based on the **`message queue`** (first-in-fist-out) - the sent messages are ordered in the message queue except it has higher priority

===============================================================================
# 'Point-to-Point' Messaging Model
* -> the **`message sent from the message sender to only one receiver`** even if **many message receivers are listening in the same message queue**
* _there are two types of "point-to-point" messaging, **fire-and-forget**(one-way) messaging and **request/reply**(request-response) messaging_
* _the difference between "fire-and-forget" and "request/reply" is **Does the message sender cares about the status of the sending message**_

## Fire-and-forget
* -> the message sender **does not wait for any response from the message queue**
* -> it doesn't care did the message queue receive the message or not
* => in this model, the Originator and the Recipient would have **`no interaction at all`**

## request/reply
* -> the message sender sends a message on one queue, and then it **`waits for the response from the receiver`**
* -> with this model, the message sender cares about **`the message status that's it received or not yet`**

===============================================================================
# 'Publish/Subscribe' Messaging model
* -> the publisher produces messages to a topic then all subscribers which subscribed to that topic will receive the sending messages and consume them
* _in this model, multiple Subscriber can consume the same single message_
* _the Publisher doesn't need to know the receiver (like in **Poin-to-Point**)_
* => provides a high **decoupling** for the application
