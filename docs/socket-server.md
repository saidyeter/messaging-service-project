
# Socket Server Usage

## Connection
When docker-compose runs; app runs on `ws://localhost:4000/ws` (NGINX is configured on 4000). 

## Usage
Socket server waits for clients to connect. After connection, server waits for an access token as message by client.  
> Access token can be obtainable from Rest API `/Auth/Login`.  
After certain minute, server kills connection of clients that did not send any message.  
Also server kills connection of clients that did not send valid access token.  
> Access token needs to be sent as `token:access-token-here`.  
If a client sends a valid access token, server sends `messageId` and `sender` user name when new message arrives to connected client. 
When an api-client sends message to rest api, rest api publishes a message to certain redis channel like below;  
```json
{
  "senderUser" : "saidyeter",
  "receiverUser": "sevaltorun",
  "messageId" : "1234567"
}
```
Socket server listens that certain redis channel. When a message publish, redis client in socket server searchs in listeners for `receiverUser`.  
If there is any connection by `receiverUser`, server sends `messageId` and `sender` as stringified json like below;
```json
{
  "senderUser" : "saidyeter",
  "messageId" : "1234567"
}
```
