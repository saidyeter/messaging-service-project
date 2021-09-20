FROM node:16-alpine

ENV NO_UPDATE_NOTIFIER true

WORKDIR /usr/src/app

COPY package.json .
COPY package-lock.json .

RUN npm install --no-optional

COPY . .

CMD [ "npm" , "start"] 