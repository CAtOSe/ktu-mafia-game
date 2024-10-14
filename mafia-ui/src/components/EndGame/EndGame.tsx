import styles from './EndGame.module.scss';
import classNames from 'classnames/bind';
import { useContext } from 'react';
import { GameStateContext } from '../../contexts/GameStateContext/GameStateContext.tsx';

const cn = classNames.bind(styles);

const EndGame = () => {
  const {
    gameState: { winnerTeam },
  } = useContext(GameStateContext);

  return (
    <div className={cn('end-game')}>
      <h3>The Game has ended</h3>
      <h4>{winnerTeam} has won the game!</h4>
    </div>
  );
};

export default EndGame;
