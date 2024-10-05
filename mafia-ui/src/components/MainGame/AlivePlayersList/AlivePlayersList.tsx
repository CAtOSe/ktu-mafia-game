import React from 'react';
import styles from './AlivePlayerList.module.scss';
import classNames from 'classnames/bind';

const cn = classNames.bind(styles);

type AlivePlayersListProps = {
  players: string[];
};

const AlivePlayersList: React.FC<AlivePlayersListProps> = ({ players }) => {
  players = [
    '1 John',
    '2 Mark',
    '3 Luke',
    '4 Adam',
    '5 Carley',
    '6 Brooke',
    '7 Laurie',
    '8 Sullivan',
    '9 Blaire',
    '10 Becca',
    /*'11 Tom',
    '12 Ben',
    '13 Steve',
    '14 Richard',
    '15 Samuel',*/
  ];

  return (
    <div className={cn('alive-player-list')}>
      <h3>Alive Players</h3>
      <ul>
        {players.slice(0, 15).map((player, index) => (
          <li key={index} className="player-row">
            <span className="player-info">{player}</span>
            <button className="action-button">Action</button>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default AlivePlayersList;
