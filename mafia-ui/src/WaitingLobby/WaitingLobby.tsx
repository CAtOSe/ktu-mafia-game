import { useEffect, useState } from "react";
import Header from '../Header/Header';
import './WaitingLobby.css';

function WaitingLobby() {
    const [players, setPlayers] = useState<string[]>([]);
    const [gameStarting, setGameStarting] = useState(false);
    const [timeRemaining, setTimeRemaining] = useState<number | null>(null); // Countdown to start game
    const [countdownInterval, setCountdownInterval] = useState<number  | null>(null); // To store the countdown interval

    useEffect(() => {
        const mockPlayers = ['Player1 (HOST)', 'Player2', 'Player3', 'Player4'];

        const joinIntervals = mockPlayers.map((player, index) => {
            return setTimeout(() => {
                setPlayers(prevPlayers => [...prevPlayers, player]);
            }, (index + 1) * 1000);
        });

        return () => {
            joinIntervals.forEach(clearTimeout);
            if (countdownInterval) {
                clearInterval(countdownInterval); // Clear the countdown on unmount
            }
        };
    }, []);

    const startCountdown = () => {
        setTimeRemaining(5); // Set initial countdown to 5 seconds

        const interval = setInterval(() => {
            setTimeRemaining(prevTime => {
                if (prevTime && prevTime <= 1) {
                    clearInterval(interval);
                    setGameStarting(true); // Set game starting state
                    return 0;
                }
                return (prevTime || 0) - 1; // Decrement countdown
            });
        }, 1000);

        setCountdownInterval(interval); // Store the interval ID
    };

    const cancelCountdown = () => {
        if (countdownInterval) {
            clearInterval(countdownInterval); // Clear the countdown
            setCountdownInterval(null); // Reset the interval ID
            setTimeRemaining(null); // Reset the countdown
        }
    };

    const handleLogout = () => {
        console.log("Logout clicked");
        // Implement your logout logic here
    };

    return (
        <div>
            <Header title = "Waiting Lobby" username="Player1" onLogout={handleLogout} />
            
            <div className="container"> 
                <div className="content">
                    {!gameStarting ? (
                        <>
                            <h3>Current Players</h3>
                            {players.length > 0 ? (
                                <div className="player-table-container">
                                    <table className="player-table">
                                        <tbody>
                                            {players.map((player, index) => (
                                                <tr key={index}>
                                                    <td>
                                                        {player}
                                                    </td>
                                                </tr>
                                            ))}
                                        </tbody>
                                    </table>
                                </div>
                            ) : (
                                <p>No players joined yet.</p>
                            )}
                            <p className="player-count">Players in lobby: {players.length}</p>

                            {/* Show the Start or Cancel button */}
                            {players.length >= 3 ? (
                                timeRemaining === null ? (
                                    <button className="glow-button" onClick={startCountdown}>
                                        Start Game
                                    </button>
                                ) : (
                                    <button className="glow-button" onClick={cancelCountdown}>
                                        Cancel
                                    </button>
                                )
                            ) : (
                                <p className="waiting-message">Waiting for more players (at least 3)</p>
                            )}

                            {timeRemaining !== null ? (
                                <p>Game starts in {timeRemaining} seconds...</p>
                            ) : null}
                        </>
                    ) : (
                        <p>The game is starting now!</p>
                    )}
                </div>
            </div>
        </div>
    );
}

export default WaitingLobby;
