# Messaging Service Project

This repository contains an app that is written for messaging. Also contains its dependencies.  

The app is an ASP.NET Core Web API application and written in .Net 5.0.  

This app provides sending message and accessing message history by logging in after register.  
*Methods are explained in depth below.*  

This app uses MongoDB to store message data and uses ElasticSearch to store logs. To see logs, Kibana is used.  

The app is placed behind NGINX. Thus, NGINX can load-balance when app is scaled.

The app can be run on Docker. Only requirement is Docker Compose.

## Requirements

 - Docker desktop (for Docker Compose) [🡥](https://www.docker.com/products/docker-desktop)
 - .Net 5.0 (to run tests) [🡥](https://dotnet.microsoft.com/download/dotnet/5.0) 

## Repository Usage

 - Clone this repository to local 
 - To run tests, use `dotnet test ./messaging-service/MessagingService.Api.Test/MessagingService.Api.Test.csproj`
 - Use `docker-compose up` 
 - To scale app with n instance, use `docker-compose up --scale messaging-api=n` (eg. `docker-compose up --scale messaging-api=4` for 4 instances)


## API Methods Usage

When docker-compose runs; app runs on `http://localhost:4000` (NGINX is configured on 4000). 

Swagger is open on `http://localhost:4000/swagger/index.html` and can be closed for production. 

### Auth Methods

#### Register `POST /Auth/Register`

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
> User id returns only for information, cannot be used any methods.

#### Login `POST /Auth/Login`

Request body example:
```json
{
  "userName": "saidyeter",
  "password": "SuperSecretP4ssw0rd"
}
```
When username and password is correct, response returns as `OK(200)` with an access token to use in other methods.
Otherwise response returns with an error message as `BadRequest(400)`.

### Message Methods

> In message methods; for auhtorization, request needs an `Authorization` header that has an access token which can be obtainable from `/Auth/Login`.  
> Access token needs to be used with `bearer` keyword and a white space.  
> Otherwise message methods return `Unauthorized(401)`.

#### Send Message `POST /Messages​/SendMessage`

Request body example:
```json
{
  "receiverUser": "saidyeter",
  "message": "Hi there!"
}
```
When receiverUser does exist and didn't block sender, response returns as `Created(201)` with message id.  
Otherwise response returns with an error message as `BadRequest(400)`.
> Message id can be used in `/Messages/​GetMessage` to retrieve message detail.

#### Get Message `GET /Messages/​GetMessage/{messageId}`

Request has `messageId` parameter. With specifying correct `messageId` this method returns message details as `OK(200)` if logged in user is sender or receiver of message.
Otherwise response returns with an error message as `BadRequest(400)`.

#### Get Older Messages `GET /Messages/GetOlderMessagesFrom/{messageId}`

Request has `messageId` parameter. With specifying correct `messageId`, this method returns older message id list than *the message* as `OK(200)` if logged in user is sender or receiver of *the message* .  
Returning message id list is conversation between logged in user and opponent user of *the message*  
> *the message* : message that has id specified in parameters
Method returns certain amount messages with chronological order. To acces older than messages from response, this method needs to be called again with oldest message id from recent response.  

On any error, response returns with an error message as `BadRequest(400)`.
> Message ids from response can be used in `/Messages/​GetMessage` to retrieve message detail.

#### Get Latest Message  `GET /Messages/GetLatestMessageBetween/{opponent}`

Request has `opponent` parameter. With specifying correct `opponent`, this method returns a message id between logged in user and `opponent` as `OK(200)`.
Otherwise response returns with an error message as `BadRequest(400)`.
> Message id can be used in `/Messages/​GetMessage` to retrieve message detail and can be used in `/Messages/GetOlderMessagesFrom` to retrieve older messages.

#### ​Block User `POST /Messages/​BlockUser`

Request body example:
```json
{
  "opponent": "saidyeter"
}
```
This method provides to block a user for receiving new messages from.  
When opponent does exist, response returns as `OK(200)`.  
On any error, response returns with an error message as `BadRequest(400)`.

## Authors

 - [Said Yeter](https://github.com/kordiseps)

## Licence

MIT License

Copyright (c) 2021 Said Yeter

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.