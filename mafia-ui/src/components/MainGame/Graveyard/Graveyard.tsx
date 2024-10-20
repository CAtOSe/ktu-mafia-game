import { useContext, useMemo } from 'react';
import { GameStateContext } from '../../../contexts/GameStateContext/GameStateContext.tsx';
import styles from './Graveyard.module.scss'; // Import the new SCSS module
import classNames from 'classnames/bind';

const cn = classNames.bind(styles);

const Graveyard = () => {
  const {
    gameState: { alivePlayers, players },
  } = useContext(GameStateContext);

  const deadPlayers = useMemo(
    () => players.filter((p) => !alivePlayers.includes(p)),
    [alivePlayers, players],
  );

  return (
    <div className={cn('list-box', 'graveyard')}>
      <h3 className={cn('graveyard-title')}>Graveyard</h3>
      <ul className={cn('graveyard-list')}>
        {deadPlayers.map((player, index) => (
          <li key={index} className={cn('graveyard-player')}>
            {player}
          </li>
        ))}
      </ul>
    </div>
  );
};

export default Graveyard;
