/*import React, { useContext } from 'react';
import styles from './AlivePlayerList.module.scss';
import classNames from 'classnames/bind';
import { GameStateContext } from '../../../contexts/GameStateContext/GameStateContext';

const cn = classNames.bind(styles);

const AlivePlayersList: React.FC = () => {
  const { gameState } = useContext(GameStateContext);

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
              <button className="action-button">Action</button>
            </li>
          ))}
        </ul>
      ) : (
        <p>No players are currently alive.</p>
      )}
    </div>
  );
};

export default AlivePlayersList;*/

import React, { useContext } from 'react';
import styles from './AlivePlayerList.module.scss';
import classNames from 'classnames/bind';
import { GameStateContext } from '../../../contexts/GameStateContext/GameStateContext';

const cn = classNames.bind(styles);

const AlivePlayersList: React.FC = () => {
  const { gameState } = useContext(GameStateContext);
  //const { role } = gameState;

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
              {gameState.role == "Killer" && (
                <button className="action-button">Action</button>
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
