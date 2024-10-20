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
    gameState: { alivePlayers, role, username, isAlive },
  } = useContext(GameStateContext);
  const websocket = useContext(WebsocketContext);
  const { isDay } = useDayNight();

  const handleActionClick = (targetUsername: string, actionType: string) => {
    if (websocket) {
      const message = createMessage(
        actionType === 'vote'
          ? RequestMessages.Vote
          : RequestMessages.NightAction,
        [targetUsername, actionType],
      ); // Create the message
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

              {/* Assassin can select everyone except themselves */}
              {!isDay && isAlive && role === 'Assassin' && player !== username && (
                <Button
                  className="action-button"
                  onClick={() => handleActionClick(player, 'kill')}
                >
                  Kill
                </Button>
              )}

              {/* Tracker can select everyone except themselves */}
              {!isDay && isAlive && role === 'Tracker' && player !== username && (
                <Button
                  className="action-button"
                  onClick={() => handleActionClick(player, 'information')}
                >
                  Track
                </Button>
              )}

              {/* Doctor can select everyone except themselves */}
              {!isDay && isAlive && role === 'Doctor' && player !== username && (
                <Button
                  className="action-button"
                  onClick={() => handleActionClick(player, 'protect')}
                >
                  Protect
                </Button>
              )}

              {/* Soldier can only select themselves */}
              {!isDay && isAlive && role === 'Soldier' && player === username && (
                <Button
                  className="action-button"
                  onClick={() => handleActionClick(player, 'protect')}
                >
                  Use Shield
                </Button>
              )}

              {isDay && isAlive && player !== username && (
                <Button
                  className="action-button"
                  onClick={() => handleActionClick(player, 'vote')}
                >
                  Vote
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
