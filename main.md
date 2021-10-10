# Messaging Service Project

This repository contains an app that is written for messaging. Also contains its dependencies.  

This is an ASP.NET Core Web API application and written in .Net 5.0.  

This app provides sending message and accessing message history by logging in after register.  
*Methods are explained in depth below.*  

Used MongoDB to store message data.  
Used Elastic Search to store logs.  
Used Kibana to examine logs.  

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
