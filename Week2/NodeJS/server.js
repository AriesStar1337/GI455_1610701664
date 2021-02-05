var websocket = require('ws');

var callbackInityServer = ()=>{
    console.log("Aries's Server is running.");
}

var websocketServer = new websocket.Server({port:25500}, callbackInityServer);

var wsList = [];

websocketServer.on("connection", (ws, rq)=>{

    wsList.push(ws);

    console.log("Client " + UserId(ws) + " has connected!");

    ws.on("message", (data)=>{
        console.log("Send from [Client " + UserId(ws) + "] - " + data);
        Boardcast(ws, data);
    });

    ws.on("close", ()=>{
        console.log("Client " + UserId(ws) + " has disconnected!");
        wsList = ArraryRemove(wsList, ws);
    });
});

function ArraryRemove(arr, value)
{
    return arr.filter((element)=>{
        return element != value;
    });
}

function Boardcast(ws, data)
{
    for(var i = 0; i < wsList.length; i++)
    {
        if(wsList[i] != ws)
        {
            wsList[i].send(data);
        }
    }
}

function UserId(ws)
{
    return wsList.indexOf(ws);
}