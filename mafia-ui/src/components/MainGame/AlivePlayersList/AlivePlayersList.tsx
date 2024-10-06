import React, { useContext } from 'react';
import styles from './AlivePlayerList.module.scss';
import classNames from 'classnames/bind';
import { GameStateContext } from '../../../contexts/GameStateContext/GameStateContext';
import WebsocketContext from '../../../contexts/WebSocketContext/WebsocketContext.ts';
import createMessage from '../../../helpers/createMessage.ts';
import { RequestMessages } from '../../../types.ts';

const cn = classNames.bind(styles);

const AlivePlayersList: React.FC = () => {
  const { gameState } = useContext(GameStateContext);
  const websocket = useContext(WebsocketContext);
  //const { role } = gameState;

  const handleActionClick = (targetUsername: string) => {
    const username = gameState?.username; // The username of the current player (the one who is clicking)

    if (username && websocket) {
      const message = createMessage(RequestMessages.NightAction, [
        username,
        targetUsername,
        'kill',
      ]); // Create the message
      websocket.sendMessage(message); // Send the message via WebSocket
    }
  };

  // The 'players' array directly contains the alive players
  const players = gameState?.players ?? [];

  return (
    <div className={cn('alive-player-list')}>
      <h3>Alive Players</h3>
      {players.length > 0 ? (
        <ul>
          {players.slice(0, 15).map((player, index) => (
            <li key={index} className="player-row">
              <span className="player-info">{player}</span>
              {/* Check if the player's role is Killer before rendering the Action button */}
              {gameState.role === 'Killer' &&
                player !== gameState?.username && (
                  <button
                    className="action-button"
                    onClick={() => handleActionClick(player)} // Pass the target player's username
                  >
                    Action
                  </button>
                )}
            </li>
          ))}
        </ul>
      ) : (
        <p>No players are currently alive.</p>
      )}
    </div>
  );
};

export default AlivePlayersList;
