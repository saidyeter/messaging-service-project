
# API Methods Usage

## Connection

When docker-compose runs; app runs on `http://localhost:4000` (NGINX is configured on 4000). 

> Swagger is open on `http://localhost:4000/swagger/index.html` and can be closed for production. 

## Auth Methods

### Register `POST /Auth/Register`

Request body example:

```json
{
  "displayName": "Said Yeter",
  "userName": "saidyeter",
  "eMail": "saidyeter@mail.com",
  "password": "SuperSecretP4ssw0rd"
}
```
When request is invalid or username is taken, response returns with an error message as `BadRequest(400)`.
Otherwise response returns as `Created(201)` with user id. 

> User id returns only for information, cannot be used within any methods.

### Login `POST /Auth/Login`

Request body example:

```json
{
  "userName": "saidyeter",
  "password": "SuperSecretP4ssw0rd"
}
```
When username and password is correct, response returns as `OK(200)` with an access token to use in other methods.
Otherwise response returns with an error message as `BadRequest(400)`.

## Message Methods

> In message methods; request needs an `Authorization` header with an access token which can be obtainable from `/Auth/Login`.  
> Access token needs to be used with `bearer` keyword and a white space.  
> Otherwise message methods return `Unauthorized(401)`.

### Send Message `POST /Messages/SendMessage`

Request body example:

```json
{
  "receiverUser": "saidyeter",
  "message": "Hi there!"
}
```
When receiverUser does exist and didn't block sender, response returns as `Created(201)` with message id.  
Otherwise response returns with an error message as `BadRequest(400)`.

> Message id can be used in `/Messages/GetMessage` to retrieve message detail.

### Get Message `GET /Messages/GetMessage/{messageId}`

Request has `messageId` parameter. With specifying correct `messageId` this method returns message details as `OK(200)` if logged in user is sender or receiver of message.
Otherwise response returns with an error message as `BadRequest(400)`.

### Get Older Messages `GET /Messages/GetOlderMessagesFrom/{messageId}`

Request has `messageId` parameter. If logged in user is sender or receiver of *the message*, this method returns message id list older than *the message* as `OK(200)` with correct `messageId`.  
Returning message id list is conversation between logged in user and opponent user of *the message*  

> *the message* : message that has id specified in parameters  

Method returns certain amount messages with chronological order. To acces older than messages from response, this method needs to be called again with oldest message id from recent response.  

On any error, response returns with an error message as `BadRequest(400)`.

> Message ids from response can be used in `/Messages/GetMessage` to retrieve message detail.

### Get Latest Message  `GET /Messages/GetLatestMessageBetween/{opponent}`

Request has `opponent` parameter. With specifying correct `opponent`, this method returns a message id between logged in user and `opponent` as `OK(200)`.
Otherwise response returns with an error message as `BadRequest(400)`.

> Message id can be used in `/Messages/GetMessage` to retrieve message detail and can be used in `/Messages/GetOlderMessagesFrom` to retrieve older messages.

### Block User `POST /Messages/BlockUser`

Request body example:

```json
{
  "opponent": "saidyeter"
}
```
This method provides to block a user for receiving new messages from.  
When opponent does exist, response returns as `OK(200)`.  
On any error, response returns with an error message as `BadRequest(400)`.