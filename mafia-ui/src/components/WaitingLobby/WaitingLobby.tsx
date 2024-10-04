import { useState, useEffect } from 'react';
import Header from '../Header/Header.tsx';
import { useNavigate } from 'react-router-dom'; // Import React Router's useNavigate
import './WaitingLobby.scss';
import { Message } from '../../contexts/WebSocketContext/types.ts';

interface WaitingLobbyProps {
  players: string[];
  username: string | null;
  sendMessage: (message: string) => void;
  lastMessage: Message;
}

function WaitingLobby({
  players,
  username,
  sendMessage,
  lastMessage,
}: WaitingLobbyProps) {
  const [gameStarting, setGameStarting] = useState(false);
  const [timeRemaining, setTimeRemaining] = useState<number | null>(null);
  const [countdownInterval, setCountdownInterval] = useState<number | null>(
    null,
  );
  const [role, setRole] = useState<string>();
  const [currentPlayers, setCurrentPlayers] = useState<string[]>(players); // Track current players
  const [host, setHost] = useState<string | null>(
    players.length > 0 ? players[0] : null,
  ); // Track host

  const navigate = useNavigate(); // Get the navigate function from React Router
  const startCountdown = () => {
    setTimeRemaining(5); // Set initial countdown to 5 seconds

    const interval = setInterval(() => {
      setTimeRemaining((prevTime) => {
        if (prevTime && prevTime <= 1) {
          clearInterval(interval);
          sendMessage('start-game');
          setGameStarting(true); // Set game starting state

          setTimeRemaining(0);
          return 0;
        }
        return (prevTime || 0) - 1; // Decrement countdown
      });
    }, 1000);

    setCountdownInterval(interval);
  };

  // Handle receiving the "game-started" message from the backend
  useEffect(() => {
    if (lastMessage.base === 'game-started') {
      setGameStarting(true);
    }
    if (lastMessage.base === 'role-assigned') {
      setRole(lastMessage.arguments?.[0] ?? 'Unknown');
    }
  }, [lastMessage]);

  useEffect(() => {
    if (gameStarting) {
      // Heading to "Game" compponent with username and players
      navigate('/game', { state: { username, players, role } });
    }
  }, [gameStarting, navigate, username, players]);

  useEffect(() => {
    // Check for host changes
    const checkHost = () => {
      if (!currentPlayers.includes(host || '')) {
        // If the current host is no longer in the player list, assign a new host
        const newHost = currentPlayers[0] || null;
        setHost(newHost);
      }
    };

    checkHost();
  }, [currentPlayers, host]);

  useEffect(() => {
    // Update the list of current players when the players prop changes
    setCurrentPlayers(players);
  }, [players]);

  const cancelCountdown = () => {
    if (countdownInterval) {
      clearInterval(countdownInterval); // Clear the countdown
      setCountdownInterval(null); // Reset the interval ID
      setTimeRemaining(null); // Reset the countdown
    }
  };

  const handleLogout = () => {
    console.log('Logout clicked');
  };

  return (
    <div>
      <Header
        title="Waiting Lobby"
        username={username || 'Unknown Player'}
        onLogout={handleLogout}
      />

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
                            {/* Show "(host)" next to the host name*/}
                            {player === host && <span> (host)</span>}
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

              {/* Show the Start button only for the host */}
              {players.length >= 3 ? (
                username === host ? (
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
                  <p className="waiting-message">
                    Waiting for the host to start the game...
                  </p>
                )
              ) : (
                <p className="waiting-message">
                  Waiting for more players (at least 3)
                </p>
              )}

              {timeRemaining !== null && (
                <p>Game starts in {timeRemaining} seconds...</p>
              )}
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
