const sqlite3 = require('sqlite3').verbose();
userName = "AzureX";
password = "123456";
displayName = "Aries";

let db = new sqlite3.Database('./database/chatDB.db', sqlite3.OPEN_CREATE | sqlite3.OPEN_READWRITE, (err)=>{
    if(err) throw err;

    console.log('Connected to database.');

    var sqlSelect = "SELECT * FROM UserData WHERE UserName='" + userName + "' AND Password='" + password + "'";
    var sqlInsert = "INSERT INTO UserData (UserName, Password, DisplayName) VALUES ('" + userName + "', '" + password + "', '" + displayName + "')"

    db.all(sqlInsert, (err, rows)=>
    {
        if(err)
        {
            console.log("Registing Fail!");
        }
        else
        {
            console.log("Registing Success!");
        }
    })

    db.all(sqlSelect, (err, rows)=>
    {
        if(err)
        {
            console.log(err);
        }
        else
        {
            if(rows.length > 0)
            {
                console.log("Login Success!");
            }
            else
            {
                console.log("Login Fail!");
            }
            console.log(rows);
        }
    })
})