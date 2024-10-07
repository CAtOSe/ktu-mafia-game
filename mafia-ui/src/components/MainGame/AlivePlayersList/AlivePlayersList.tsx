import React, { useContext } from 'react';
import styles from './AlivePlayerList.module.scss';
import classNames from 'classnames/bind';
import { GameStateContext } from '../../../contexts/GameStateContext/GameStateContext';
import WebsocketContext from '../../../contexts/WebSocketContext/WebsocketContext.ts';
import createMessage from '../../../helpers/createMessage.ts';
import { RequestMessages } from '../../../types.ts';
import { useDayNight } from '../../../contexts/DayNightContext/useDayNight.ts';
import Button from '../../Button/Button.tsx';

const cn = classNames.bind(styles);

const AlivePlayersList: React.FC = () => {
  const {
    gameState: { alivePlayers, role, username },
  } = useContext(GameStateContext);
  const websocket = useContext(WebsocketContext);
  const { isDay } = useDayNight();

  const handleActionClick = (targetUsername: string) => {
    if (websocket) {
      const message = createMessage(RequestMessages.NightAction, [
        targetUsername,
        'kill',
      ]); // Create the message
      websocket.sendMessage(message); // Send the message via WebSocket
    }
  };

  return (
    <div className={cn('alive-player-list')}>
      <h3>Alive Players</h3>
      {alivePlayers.length > 0 ? (
        <ul>
          {alivePlayers.map((player) => (
            <li key={player} className="player-row">
              <span className="player-info">{player}</span>
              {/* Check if the player's role is Killer before rendering the Action button */}
              {!isDay && role === 'Killer' && player !== username && (
                <Button
                  className="action-button"
                  onClick={() => handleActionClick(player)} // Pass the target player's username
                >
                  Kill
                </Button>
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
