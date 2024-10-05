import styles from './GameLayout.module.scss';
import classNames from 'classnames/bind';
import HeaderV2 from '../Header/HeaderV2.tsx';
import { useContext } from 'react';
import { GameStateContext } from '../../contexts/GameStateContext/GameStateContext.tsx';
import { GameStage } from '../../contexts/GameStateContext/types.ts';
import LoginV2 from '../Login/LoginV2.tsx';
import WaitingLobbyV2 from '../WaitingLobby/WaitingLobbyV2.tsx';
import MainGame from '../MainGame/MainGame.tsx';

const cn = classNames.bind(styles);

const GameLayout = () => {
  const {
    gameState: { gameStage },
  } = useContext(GameStateContext);

  const output = (currentGameStage: GameStage) => {
    switch (currentGameStage) {
      case GameStage.Connecting:
        return <p>Connecting...</p>;
      case GameStage.Login:
        return <LoginV2 />;
      case GameStage.Lobby:
        return <WaitingLobbyV2 />;
      case GameStage.Running:
        return <MainGame />;
      default:
        return <p>Unknown game stage</p>;
    }
  };

  return (
    <div className={cn('game-layout')}>
      <HeaderV2 />
      <div className={cn('game-layout__inner')}>{output(gameStage)}</div>
    </div>
  );
};

export default GameLayout;
