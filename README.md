# Messaging Service Project


This repository contains an app that is written for messaging. Also contains its dependencies. 

The app is an ASP.NET Core Web API application and written in .Net 5.0.

This app provides sending message and accessing message history by logging in after register.

Methods are explained in depth below.

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
> Details coming...


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