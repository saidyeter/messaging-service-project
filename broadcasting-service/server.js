const SOCKET_SERVER_PORT = process.env.SOCKET_SERVER_PORT || 5000
const REDIS_SERVER_HOST = process.env.REDIS_SERVER_HOST || "127.0.0.1"
const REDIS_SERVER_PORT = process.env.REDIS_SERVER_PORT || 6379
const REDIS_CHANNEL = process.env.REDIS_CHANNEL || "ch"
const JWT_SECRET = process.env.JWT_SECRET || "b04203df-a1b7-4aea-afb7-8bf42de8d8ee"

var os = require("os");
var hostname = os.hostname();

const WebSocket = require('ws')
const jwt = require("jsonwebtoken");

const redis = require("redis");
const client = redis.createClient({
    host: REDIS_SERVER_HOST,
    port: REDIS_SERVER_PORT
});

const wss = new WebSocket.Server({
    port: SOCKET_SERVER_PORT
})

wss.on('connection', function (socket) {
    console.log(`a connection established to '${hostname}'`);
    socket.on('message', function (message) {
        const msgParts = message.toString().split(':')
        if (msgParts.length > 1 && msgParts[0] == 'token') {
            const token = msgParts[1]
            const parseTokenRes = parseToken(token)
            if (!parseTokenRes.IsValid) {
                socket.send(`invalid token : ${parseTokenRes.Error}`)
                socket.close()
                console.log("connection closed");
            }
            else {
                socket['UserName'] = parseTokenRes.UserName
                socket['Id'] = parseTokenRes.Id
                console.log(`${parseTokenRes.UserName} connected to '${hostname}'`);
            }
        }
        else {
            socket.send("invalid connection")
            socket.close()
            console.log("connection closed");
        }
    })
})


client.on("error", error => {
    console.error(error);
});

client.on('message', (channel, messageFromRedisChannel) => {
    console.log(`'${messageFromRedisChannel}' from '${channel}' channel`);
    const messageDataFromRedisChannel = JSON.parse(messageFromRedisChannel)

    const receiverUser = Array.from(wss.clients).find(x => x['UserName'] == messageDataFromRedisChannel.receiverUser)
    if (receiverUser) {
        const messageToReceiverSocket = {
            sender: messageDataFromRedisChannel.senderUser,
            messageId: messageDataFromRedisChannel.messageId
        }
        receiverUser.send(JSON.stringify(messageToReceiverSocket))
    }
    else{
        console.log(`${messageDataFromRedisChannel.receiverUser} couldnt found in wss clients`);
    }
})
client.subscribe(REDIS_CHANNEL)


function parseToken(token) {
    try {
        const decodedToken = jwt.verify(token, JWT_SECRET);
        return {
            IsValid: true,
            UserName: decodedToken.UserName,
            Id: decodedToken.Id
        }
    } catch (error) {
        return {
            IsValid: false,
            Error: error
        }
    }
}