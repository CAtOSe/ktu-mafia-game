import styles from './WaitingLobbyV2.module.scss';
import classNames from 'classnames/bind';
import Button from '../Button/Button.tsx';
import { ButtonStyle } from '../Button/types.ts';
import { useContext, useEffect, useState } from 'react';
import { GameStateContext } from '../../contexts/GameStateContext/GameStateContext.tsx';
import createMessage from '../../helpers/createMessage.ts';
import { RequestMessages } from '../../types.ts';
import WebsocketContext from '../../contexts/WebSocketContext/WebsocketContext.ts';

const cn = classNames.bind(styles);

const WaitingLobbyV2 = () => {
  const websocket = useContext(WebsocketContext);
  const {
    gameState: { players, isHost },
  } = useContext(GameStateContext);
  const [countdownTime, setCountdownTime] = useState<number>(-1);
  const [difficulty, setDifficulty] = useState<string>('basic');

  useEffect(() => {
    if (!websocket) return;

    const subId = websocket.subscribe({
      id: 'lobby',
      onReceive: (message) => {
        console.log(message);
        if (!message.arguments?.[0]) return;
        setCountdownTime(Number(message.arguments[0]));
      },
      filter: ['start-countdown'],
    });

    return () => {
      websocket.unsubscribe(subId);
    };
  }, [websocket]);

  useEffect(() => {
    if (countdownTime <= 0) return;

    const timeout = setTimeout(() => {
      setCountdownTime(countdownTime - 1000);
    }, 1000);

    return () => {
      clearTimeout(timeout);
    };
  }, [countdownTime]);

  const hasEnoughPlayers = players.length >= 3;
  const statusMessage = hasEnoughPlayers
    ? isHost
      ? 'You can start the game.'
      : 'Waiting for the host to start the game.'
    : 'Waiting for players to join.';

  const startGame = () => {
    websocket?.sendMessage(
      createMessage(RequestMessages.StartGame, [difficulty]),
    );
  };

  return (
    <div className={cn('waiting-lobby')}>
      <p className={cn('waiting-lobby__status')}>{statusMessage}</p>
      <ul className={cn('waiting-lobby__player-list')}>
        {players.map((player) => (
          <li key={player} className={cn('waiting-lobby__player')}>
            {player}
          </li>
        ))}
      </ul>
      {isHost && (
        <div className={cn('waiting-lobby__mode-selector')}>
          <p>Difficulty level:</p>
          <Button
            onClick={() => setDifficulty('basic')}
            style={
              difficulty === 'basic' ? ButtonStyle.Glow : ButtonStyle.Basic
            }
          >
            Basic
          </Button>
          <Button
            onClick={() => setDifficulty('advanced')}
            style={
              difficulty === 'advanced' ? ButtonStyle.Glow : ButtonStyle.Basic
            }
          >
            Advanced
          </Button>
        </div>
      )}
      {isHost &&
        (hasEnoughPlayers ? (
          <Button style={ButtonStyle.Glow} onClick={startGame}>
            Start Game
          </Button>
        ) : (
          <p className={cn('waiting-lobby__player-notice')}>
            Need at least 3 players to start the game
          </p>
        ))}
      {countdownTime > 0 && (
        <div className={cn('waiting-lobby__counter')}>
          <p>Game starting in...</p>
          <p className={cn('waiting-lobby__counter-time')}>
            {Math.trunc(countdownTime / 1000)}
          </p>
        </div>
      )}
    </div>
  );
};

export default WaitingLobbyV2;
