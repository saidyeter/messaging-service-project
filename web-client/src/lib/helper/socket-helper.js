const socketUrl = "ws://localhost:4000/ws"

//import WebSocket from "ws";

const serverAddress = socketUrl;


const keyName = "accesstoken"
function saveAccessToken(token) {
    sessionStorage.setItem(keyName,token)
}
function getAccessToken() {
    return sessionStorage.getItem(keyName)
}

export function connect( onNewMessage, onConnectionClose) {
    const accessToken= getAccessToken()
    const ws = new WebSocket(serverAddress);

    ws.addEventListener('open', function () {
        ws.send("token:" + accessToken);
    });

    ws.addEventListener('message', function (msg) {
        onNewMessage(msg)
    });

    ws.addEventListener('close', function (msg) {
        onConnectionClose()
    });
}