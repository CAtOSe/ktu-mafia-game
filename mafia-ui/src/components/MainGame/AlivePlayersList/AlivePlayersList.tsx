import React, { useContext } from 'react';
import styles from './AlivePlayerList.module.scss';
import classNames from 'classnames/bind';
import { GameStateContext } from '../../../contexts/GameStateContext/GameStateContext.tsx';

const cn = classNames.bind(styles);

type AlivePlayersListProps = {
  players: string[];
};

const AlivePlayersList: React.FC<AlivePlayersListProps> = () => {
  const { gameState } = useContext(GameStateContext);

  // Safely access gameState properties with optional chaining and default to empty arrays if undefined
  const players = gameState?.players ?? [];
  const isAlive = gameState?.isAlive ?? [];

  // Ensure players and isAlive arrays are the same length
  const alivePlayers = players.filter((_, index) => isAlive[index]);

  return (
    <div className={cn('alive-player-list')}>
      <h3>Alive Players</h3>
      {alivePlayers.length > 0 ? (
        <ul>
          {alivePlayers.slice(0, 15).map((player, index) => (
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

export default AlivePlayersList;
