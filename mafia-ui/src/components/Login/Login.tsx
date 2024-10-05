import { useContext, useEffect, useState } from 'react';
import WaitingLobby from '../WaitingLobby/WaitingLobby.tsx';
import classNames from 'classnames/bind';
import styles from './Login.module.scss';
import { useNavigate } from 'react-router-dom';
import { Message } from '../../contexts/WebSocketContext/types.ts';
import WebsocketContext from '../../contexts/WebSocketContext/WebsocketContext.ts';

const cn = classNames.bind(styles);

function Login() {
  // const { sendMessage, lastMessage, readyState } = useWebSocket(socketUrl);
  const { sendMessage, subscribe, unsubscribe } = useContext(WebsocketContext);
  const [loggedIn, setLoggedIn] = useState(false);
  const [players, setPlayers] = useState<string[]>([]);
  const [username, setUsername] = useState(''); // Save username from input field
  const [error, setError] = useState<string | null>(null); // Store error message
  const minimalInputChars = 4;
  const maximalInputChars = 10;
  const navigate = useNavigate();
  const [lastMessage, setLastMessage] = useState<Message>();

  useEffect(() => {
    subscribe({
      id: 'login',
      onReceive: (message) => {
        console.log(message.base);
        console.log(message.arguments);
        setLastMessage(message);
      },
    });

    return () => {
      unsubscribe('login');
    };
  }, [subscribe, unsubscribe]);

  useEffect(() => {
    if (!lastMessage) return;
    if (lastMessage.base === 'logged-in') {
      setLoggedIn(true);
    }

    // Process message with whole players list
    if (lastMessage.base === 'player-list-update') {
      const playersList = lastMessage.arguments ?? [];
      setPlayers(playersList);
    }

    if (lastMessage.base === 'role-assigned') {
      navigate('/game', {
        state: {
          username,
          players,
          role: lastMessage.arguments?.[0] ?? 'unknown',
        },
      });
    }

    // Handle error messages
    if (lastMessage.base === 'error') {
      const errorMessage = lastMessage.error ?? 'Unknown Error';
      setError(errorMessage);
    }
  }, [navigate, lastMessage, username, players]);

  const sendLogin = () => {
    // Check if the username length is valid
    if (
      username.length < minimalInputChars ||
      username.length > maximalInputChars
    ) {
      setError(
        `Username must be between ${minimalInputChars} and ${maximalInputChars} characters.`,
      );
      return;
    }

    // Clear the error message if validation is successful
    setError(null);

    // if (readyState !== ReadyState.OPEN) return;

    sendMessage(`login:${username}`); // save and send username
  };

  return (
    <div className={cn('container')}>
      <div className={cn('content')}>
        {!loggedIn ? (
          <div>
            <h2>Enter Game</h2>
            <input
              type="text"
              className={cn('input')}
              placeholder="Enter username"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
            />
            <button className={cn('glow-button')} onClick={sendLogin}>
              Login
            </button>
            {/* Display error message if there's any */}
            {error && <p className={cn('error-message')}>{error}</p>}
          </div>
        ) : (
          <WaitingLobby
            players={players}
            username={username}
            sendMessage={sendMessage}
            lastMessage={lastMessage}
          />
        )}
      </div>
    </div>
  );
}

export default Login;
