const socketUrl = "ws://localhost:4000/ws"

//import WebSocket from "ws";

const serverAddress = socketUrl;


export function connect(accessToken,onNewMessage,onConnectionClose) {
    const ws = new WebSocket(serverAddress);

    ws.addEventListener('open', function () {
        ws.send("token:"+accessToken);
    });

    ws.addEventListener('message', function (msg) {
        onNewMessage(msg)
    });
    
    ws.addEventListener('close', function (msg) {
        onConnectionClose()
    });
}