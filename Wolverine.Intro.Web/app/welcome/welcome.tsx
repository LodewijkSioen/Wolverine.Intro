import { useEffect, useReducer, useState } from "react";
import * as signalR from "@microsoft/signalr";

const messageReducer = (state: string[], action: string) => {
  return [...state, action];
}

export function Welcome() {
  const [connection, setConnection] = useState<signalR.HubConnection>();
  const [messages, messageDispatch] = useReducer(messageReducer, ["hello from react"]);


  useEffect(() => {
    var conn = new signalR.HubConnectionBuilder()
      .withUrl("https://localhost:7136/_realtime")
      .withAutomaticReconnect()
      .build();

      conn.start().then(() =>{
        setConnection(conn);
       });
    }, []);

    useEffect(() => {
      connection?.on("ReceiveMessage", (json) => {
        console.log("Message recieved")
        const message = JSON.parse(json);
        messageDispatch(message.data.message)
      });      
    }, [connection])

    const buttonClick = async () => {
      const message = {
        type: 'request_message', 
        data: { message: 'hello'}
      };

      await connection?.invoke("ReceiveMessage", JSON.stringify(message));
      console.log("Message sent")
    }

  return (
    <div>
      <input type="button" onClick={buttonClick} value="clieck meeee" disabled={connection === undefined} />
      <ul>
        {messages.map((m, i)=> 
          <li key={i}>{m}</li>
        )}
      </ul>
    </div>
  );
}