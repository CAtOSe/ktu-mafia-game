import {useEffect, useState} from "react";
import useWebSocket, {ReadyState} from "react-use-websocket";

const socketUrl = 'ws://localhost:5141/ws';

function Login() {
    const { sendMessage, lastMessage, readyState } = useWebSocket(socketUrl);
    const [loggedIn, setLoggedIn] = useState(false);
    const [existingPlayers, setExistingPlayers] = useState<string[]>([]);  // Players list before log in
    const [newPlayers, setNewPlayers] = useState<string[]>([]);            // New logged in players

    useEffect(() => {
        if (!lastMessage) return;
        console.log(`received: ${lastMessage.data}`);
        if (lastMessage.data === 'logged-in') {
            setLoggedIn(true);
        }

        // Process message with whole players list
        if (lastMessage.data.startsWith('players-list:')) {
            const playersList = lastMessage.data.split(':')[1].split(','); 
            setExistingPlayers(playersList); 
        }

        // New players loggs
        if (lastMessage.data.startsWith('new-player:')) {
            const newPlayer = lastMessage.data.split(':')[1]; 
            setNewPlayers(prevPlayers => [...prevPlayers, newPlayer]); 
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

            {/* Showing old logs */}
            {existingPlayers.length > 0 && (
                <div>
                    <h3>Players already in the game:</h3>
                    <ul>
                        {existingPlayers.map((player, index) => (
                            <li key={index}>{player}</li>
                        ))}
                    </ul>
                </div>
            )}

            {/* Showing new logs */}
            {newPlayers.length > 0 && (
                <div>
                    <h3>New players:</h3>
                    <ul>
                        {newPlayers.map((player, index) => (
                            <li key={index}>{player}</li>
                        ))}
                    </ul>
                </div>
            )}
        </div>
    );
}

export default Login;
