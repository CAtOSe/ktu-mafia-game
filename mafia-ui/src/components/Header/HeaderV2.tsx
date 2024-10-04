import { useContext } from 'react';
import { GameStateContext } from '../../contexts/GameStateContext/GameStateContext.tsx';
import styles from './HeaderV2.module.scss';
import classNames from 'classnames/bind';
import MafiaGameLogo from '../../icons/MafiaGameLogo.png';
import { GameStage } from '../../contexts/GameStateContext/types.ts';
import Button from '../Button/Button.tsx';
import { ButtonStyle } from '../Button/types.ts';

const cn = classNames.bind(styles);

const HeaderV2 = () => {
  const { username, gameStage } = useContext(GameStateContext);

  const headerTitle = (currentGameStage: GameStage) => {
    switch (currentGameStage) {
      case GameStage.Login:
        return 'Login';
      case GameStage.Lobby:
        return 'Lobby';
      case GameStage.Running:
        return 'Game';
      default:
        return 'Error';
    }
  };

  const showLogout =
    username.length > 0 &&
    (gameStage === GameStage.Running || gameStage === GameStage.Lobby);

  return (
    <div className={cn('header')}>
      <div className={cn('header__section')}>
        <img
          src={MafiaGameLogo}
          className={cn('header__icon')}
          alt={'Mafia Logo'}
        />
        <h2 className={cn('header__title')}>{headerTitle(gameStage)}</h2>
      </div>
      <div className={cn('header__section')}>
        {showLogout && (
          <>
            <p className={cn('header__username')}>Jonas{username}</p>
            <Button style={ButtonStyle.Glow}>Logout</Button>
          </>
        )}
      </div>
    </div>
  );
};

export default HeaderV2;
