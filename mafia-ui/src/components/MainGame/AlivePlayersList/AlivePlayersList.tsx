import React, { useContext, useState } from 'react';
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

  const [actionCounts, setActionCounts] = useState<{ [key: string]: number }>({});

  const handleActionClick = (targetUsername: string, actionType: string) => {
    if (websocket) {
      const message = createMessage(
        actionType === 'vote'
          ? RequestMessages.Vote
          : RequestMessages.NightAction,
        [targetUsername, actionType],
      ); // Create the message
      websocket.sendMessage(message); // Send the message via WebSocket

      // Update the action count 
      setActionCounts((prevCounts: { [x: string]: any; }) => ({
        ...prevCounts,
        [username]: (prevCounts[username] || 0) + 1,
      }));
    }
  };

  return (
    <div className={cn('alive-player-list')}>
      <h3>Alive Players</h3>
      {alivePlayers.length > 0 ? (
        <ul>
          {alivePlayers.map((player) => (
            <li key={player} className={cn('player-row')}>
              <span className="player-info">
                {player === username ? (
                  <>[Actions: {actionCounts[player] || '0'}] {player}</>
                ) : (
                  <>{player}</>
                )}
              </span>

              {!isDay && isAlive && role === 'Assassin' && player !== username && (
                <Button
                  className={cn('action-button', 'night-action-button')}
                  onClick={() => handleActionClick(player, 'kill')}
                >
                  Kill
                </Button>
              )}

              {!isDay && isAlive && (role === 'Poisoner' || role === 'Hemlock') && player !== username && (
                <Button
                  className={cn('action-button', 'night-action-button')}
                  onClick={() => handleActionClick(player, 'poison')}
                >
                  Poison
                </Button>
              )}

              {!isDay && isAlive && role === 'Tracker' && player !== username && (
                <Button
                  className={cn('action-button', 'night-action-button')}
                  onClick={() => handleActionClick(player, 'information')}
                >
                  Track
                </Button>
              )}

              {!isDay && isAlive && role === 'Spy' && player !== username && (
                <Button
                  className={cn('action-button', 'night-action-button')}
                  onClick={() => handleActionClick(player, 'information')}
                >
                  Investigate
                </Button>
              )}

              {!isDay && isAlive && role === 'Doctor' && player !== username && (
                <Button
                  className={cn('action-button', 'night-action-button')}
                  onClick={() => handleActionClick(player, 'protect')}
                >
                  Protect
                </Button>
              )}

              {!isDay && isAlive && role === 'Soldier' && player === username && (
                <Button
                  className={cn('action-button', 'night-action-button')}
                  onClick={() => handleActionClick(player, 'protect')}
                >
                  Use Shield
                </Button>
              )}

              {isDay && isAlive && player !== username && (
                <Button
                  className={cn('action-button', 'vote-button')}
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
