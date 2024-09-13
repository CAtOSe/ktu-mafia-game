import {useEffect, useState} from "react";
import useWebSocket, {ReadyState} from "react-use-websocket";

const socketUrl = 'ws://localhost:5141/ws';

function Login() {
    const { sendMessage, lastMessage, readyState } = useWebSocket(socketUrl);
    const [loggedIn, setLoggedIn] = useState(false);

    useEffect(() => {
        if (!lastMessage) return;
        console.log(`received: ${lastMessage.data}`);
        if (lastMessage.data === 'login') {
            setLoggedIn(true);
        }
    }, [lastMessage]);

    const sendLogin = () => {
        if (readyState !==  ReadyState.OPEN) return;
        sendMessage('login');
    }

    return (<div>
        <h2>Enter game</h2>
        {(!loggedIn && <button onClick={sendLogin}>Login</button>)}
        {(loggedIn && <p>Logged in</p>)}
    </div>)
}

export default Login;
