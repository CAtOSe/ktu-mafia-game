import {useEffect, useState} from "react";
import useWebSocket, {ReadyState} from "react-use-websocket";

const socketUrl = 'ws://localhost:5141/ws';

function Login() {
    const { sendMessage, lastMessage, readyState } = useWebSocket(socketUrl);
    const [loggedIn, setLoggedIn] = useState(false);
    const [players, setPlayers] = useState<string[]>([]); // players list

    useEffect(() => {
        if (!lastMessage) return;
        console.log(`received: ${lastMessage.data}`);
        if (lastMessage.data === 'logged-in') {
            setLoggedIn(true);
        }

        // Apdorojame naujų žaidėjų prisijungimus
        if (lastMessage.data.startsWith('new-player:')) {
            const newPlayer = lastMessage.data.split(':')[1]; // Gauta informacija apie naują žaidėją
            setPlayers(prevPlayers => [...prevPlayers, newPlayer]); // Atnaujiname žaidėjų sąrašą
        }
    }, [lastMessage]);

    const sendLogin = () => {
        if (readyState !==  ReadyState.OPEN) return;
        sendMessage('login');
    }

    return (
        <div>
        <h2>Enter game</h2>
        {(!loggedIn && <button onClick={sendLogin}>Login</button>)}
        {(loggedIn && <p>Logged in</p>)}
            
            {/* Rodo prisijungusius žaidėjus, jei jie yra */}
            {players.length > 0 && (
                <div>
                    <h3>Players in the game:</h3>
                    <ul>
                        {players.map((player, index) => (
                            <li key={index}>{player}</li>
                        ))}
                    </ul>
                </div>
            )}
        </div>
    );
}

export default Login;
