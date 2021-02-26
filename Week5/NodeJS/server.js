const app = require('express')();
const server = require('http').Server(app);
const { fail } = require('assert');
const { Console } = require('console');
const websocket = require('ws');
const wss = new websocket.Server({server});
const sqlite3 = require('sqlite3').verbose();

server.listen(process.env.PORT || 8080, ()=>{
    console.log("Server start at port "+server.address().port);
});

var roomList = [];

let db = new sqlite3.Database('./database/chatDB.db', sqlite3.OPEN_CREATE | sqlite3.OPEN_READWRITE, (err)=>{
    if(err) throw err;
    console.log('Database Online!');
})

wss.on("connection", (ws)=>{
    
    //Lobby
    var callbackMsg = {
        eventName:"Connection",
        eventAction:"ConnectServer",
        eventResult:"success"
    }
    var toJsonStr = JSON.stringify(callbackMsg);
    ws.send(toJsonStr);
    
    console.log("client connected.");
    //Reception
    ws.on("message", (data)=>{
        console.log("send from client :" + data);

        //========== Convert jsonStr into jsonObj =======
        var toJsonObj = { 
            data:"",
        }
        toJsonObj = JSON.parse(data);
        //===============================================

        if(toJsonObj.eventName == "Login")
        {
            var sqlAccount = "SELECT * FROM UserData WHERE UserName='" + toJsonObj.userName + "' AND Password='" + toJsonObj.password + "'";
            db.all(sqlAccount, (err, rows)=>
            {
                if(err)
                {
                    console.log(err);
                }
                else
                {
                    if(rows.length > 0)
                    {
                        console.log(rows);
                        var callbackMsg = {
                            eventName:"Account",
                            eventAction:"Login",
                            eventResult:"success",
                            userName:rows[0].UserName,
                            displayName:rows[0].DisplayName
                        }
                        ws.children = toJsonObj.userName;
                        console.log("Login Success!");
                    }
                    else
                    {
                        var callbackMsg = {
                            eventName:"Account",
                            eventAction:"Login",
                            eventResult:"fail",
                        }
                        console.log("Login Fail!");
                    }
                    var toJsonStr = JSON.stringify(callbackMsg);
                    ws.send(toJsonStr);
                    console.log(rows);
                }
            })
        }
        else if(toJsonObj.eventName == "Regis")
        {

            var sqlInsert = "INSERT INTO UserData (UserName, Password, DisplayName) VALUES ('" + toJsonObj.userName + "', '" + toJsonObj.password + "', '" + toJsonObj.displayName + "')"
            
            db.all(sqlInsert, (err, rows)=>
            {
                if(err)
                {
                    var callbackMsg = {
                        eventName:"Account",
                        eventAction:"Regis",
                        eventResult:"fail"
                    }
                    var toJsonStr = JSON.stringify(callbackMsg);
                    ws.send(toJsonStr);
                    console.log("Regis Fail!");
                }
                else
                {
                    var callbackMsg = {
                        eventName:"Account",
                        eventAction:"Login",
                        eventResult:"success",
                        userName:toJsonObj.userName,
                        displayName:toJsonObj.displayName
                    }
                    ws.children = toJsonObj.userName;
                    var toJsonStr = JSON.stringify(callbackMsg);
                    ws.send(toJsonStr);
                    console.log("Regis Success!");
                }
            })
        }
        else if(toJsonObj.eventName == "CreateRoom")
        {
            var isFoundRoom = false;
            for(var i = 0; i < roomList.length; i++)
            {
                if(roomList[i].roomName == toJsonObj.roomName)
                {
                    isFoundRoom = true;
                    break;
                }
            }
            if(isFoundRoom == true)
            {
                var callbackMsg = {
                    eventName:"Room",
                    eventAction:"CreateRoom",
                    eventResult:"fail"
                }
                var toJsonStr = JSON.stringify(callbackMsg);
                ws.send(toJsonStr);
                console.log("client create room fail.");
            }
            else
            {
                var newRoom = {
                    roomName: toJsonObj.roomName,
                    wsList: [],
                    userList: []
                }
                newRoom.wsList.push(ws);
                newRoom.userList.push(ws.children);


                roomList.push(newRoom);

                var callbackMsg = {
                    eventName:"Room",
                    eventAction:"CreateRoom",
                    eventResult:"success",
                    roomName:toJsonObj.roomName
                }
                var toJsonStr = JSON.stringify(callbackMsg);
                ws.send(toJsonStr);
                console.log(roomList);
                console.log("client create room success.");
            }
        }
        else if(toJsonObj.eventName == "JoinRoom")
        {
            console.log("client request join room.");
            
            if(roomList.length > 0)
            {
                for(var i = 0; i < roomList.length; i++)
                {
                    if(roomList[i].roomName == toJsonObj.roomName)
                    {
                        roomList[i].wsList.push(ws);
                        roomList[i].userList.push(ws.children);

                        var callbackMsg = {
                            eventName:"Room",
                            eventAction:"JoinRoom",
                            eventResult:"success",
                            roomName:toJsonObj.roomName
                        }
                        var toJsonStr = JSON.stringify(callbackMsg);
                        ws.send(toJsonStr);
                        console.log("client join room success.")
                        console.log(roomList);
                        break;
                    }
                    if(i == roomList.length - 1)
                    {
                        var callbackMsg = {
                            eventName:"Room",
                            eventAction:"JoinRoom",
                            eventResult:"fail"
                        }
                        var toJsonStr = JSON.stringify(callbackMsg);
                        ws.send(toJsonStr);
                        console.log("client join room fail.")
                        break;
                    }
                }
            }
            else
            {
                var callbackMsg = {
                    eventName:"Room",
                    eventAction:"JoinRoom",
                    eventResult:"fail"
                }
                var toJsonStr = JSON.stringify(callbackMsg);
                ws.send(toJsonStr);
                console.log("client join room fail.")
            }
        }
        else if(toJsonObj.eventName == "LeaveRoom")
        {
            var isLeaveSuccess = false;
            for(var i = 0; i < roomList.length; i++)
            {
                for(var j = 0; j < roomList[i].wsList.length; j++)
                {
                    if(ws == roomList[i].wsList[j])
                    {
                        roomList[i].wsList.splice(j, 1);

                        if(roomList[i].wsList.length <= 0)
                        {
                            roomList.splice(i, 1);
                        }
                        isLeaveSuccess = true;
                        break;
                    }
                }
            }
            if(isLeaveSuccess)
            {
                var callbackMsg = {
                    eventName:"Room",
                    eventAction:"LeaveRoom",
                    eventResult:"success"
                }
                var toJsonStr = JSON.stringify(callbackMsg);
                ws.send(toJsonStr);
                console.log("leave room success.");
            }
            else
            {
                var callbackMsg = {
                    eventName:"Room",
                    eventAction:"LeaveRoom",
                    eventResult:"fail"
                }
                var toJsonStr = JSON.stringify(callbackMsg);
                ws.send(toJsonStr);
                console.log("leave room fail.");
            }
        }
        else if(toJsonObj.eventName == "Chat")
        {
            var sqlName = "SELECT DisplayName FROM UserData WHERE UserName='" + ws.children + "'";
            db.all(sqlName, (err, rows)=>
            {
                if(err)
                {
                    console.log(err);
                }
                else
                {
                    if(rows.length > 0)
                    {
                        console.log(rows);
                        var callbackMsg = {
                            eventName:"Chat",
                            eventResult:"success",
                            senderID:ws.children,
                            senderName:rows[0].DisplayName,
                            senderMessage:toJsonObj.senderMessage
                        }
                        console.log("Chat Success!");
                    }
                    else
                    {
                        var callbackMsg = {
                            eventName:"Chat",
                            eventResult:"fail",
                        }
                        console.log("Chat Fail!");
                    }
                    var toJsonStr = JSON.stringify(callbackMsg);
                    console.log(toJsonStr);
                    Boardcast(ws, toJsonStr);
                }
            })
        }
    });

    ws.on("close", ()=>{
        console.log("client disconnected.");
        for(var i = 0; i < roomList.length; i++)
        {
            for(var j = 0; j < roomList[i].wsList.length; j++)
            {
                if(ws == roomList[i].wsList[j])
                {
                    roomList[i].wsList.splice(j, 1);

                    if(roomList[i].wsList.length <= 0)
                    {
                        roomList.splice(i, 1);
                    }
                    break;
                }
            }
        }
    });
});

function Boardcast(ws, message)
{
    var selectionRoomIndex = -1;

    for(var i = 0; i < roomList.length; i++){
        for(var j = 0; j < roomList[i].wsList.length; j++){
            if(ws == roomList[i].wsList[j]){
                selectionRoomIndex = i;
                break;
            }
        }
    }

    for(var i = 0; i < roomList[selectionRoomIndex].wsList.length; i++)
    {
        roomList[selectionRoomIndex].wsList[i].send(message);
    }
}