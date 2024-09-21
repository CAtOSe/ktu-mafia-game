import { useEffect, useState } from "react";
import useWebSocket, { ReadyState } from "react-use-websocket";
import WaitingLobby from "../WaitingLobby/WaitingLobby";
import './Login.css';

const socketUrl = 'ws://localhost:5141/ws';

function Login() {
    const { sendMessage, lastMessage, readyState } = useWebSocket(socketUrl);
    const [loggedIn, setLoggedIn] = useState(false);
    const [existingPlayers, setExistingPlayers] = useState<string[]>([]);  // Players list before log in
    const [newPlayers, setNewPlayers] = useState<string[]>([]);            // New logged in players
    const [username, setUsername] = useState('');                          // Save username from input field

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

        // New players logs
        if (lastMessage.data.startsWith('new-player:')) {
            const newPlayer = lastMessage.data.split(':')[1];
            setNewPlayers(prevPlayers => [...prevPlayers, newPlayer]);
        }
    }, [lastMessage]);

    const sendLogin = () => {
        if (readyState !== ReadyState.OPEN) return;
        sendMessage('login');
    };

    const allPlayers = [...existingPlayers, ...newPlayers];

    return (
        <div className="container">
            <div className="content">
                {!loggedIn ? (
                    <div>
                        <h2>Enter Game</h2>
                        <input
                            type="text"
                            className="input"
                            placeholder="Enter username"
                            value={username}
                            onChange={(e) => setUsername(e.target.value)}
                        />
                        <button className="glow-button" onClick={sendLogin}>Login</button>
                    </div>
                ) : (
                    <WaitingLobby players={allPlayers} username={username} />
                )}
            </div>
        </div>
    );

}

export default Login;
