import { useLocation } from 'react-router-dom';
import classNames from 'classnames/bind';
import styles from './Game.module.scss';

const cn = classNames.bind(styles);

interface GameProps {
  username: string;
  players: string[];
  role: string;
}

const Game: React.FC = () => {
  const location = useLocation();
  const { username, players, role } = location.state as GameProps;

  return (
    <div className={cn('game-container')}>
      <div className={cn('game-content')}>
        <h2 className={cn('game-header')}>Welcome, {username}!</h2>
        <p>Your role: {role}</p> {/*Display the current user's role*/}
        <h3>Current Players and Roles:</h3>
        <ul className={cn('player-list')}>
          {players.map((player, index) => (
            <li key={index}>
              {player}
              {/*Display role when assigned*/}
            </li>
          ))}
        </ul>
      </div>
    </div>
  );
};

export default Game;
