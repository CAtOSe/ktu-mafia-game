import React from 'react';
import DayTracker from './DayTracker/DayTracker.tsx';
import RoleInformation from './RoleInformation/RoleInformation.tsx';
import RoleList from './RoleList/RoleList.tsx';
import Graveyard from './Graveyard/Graveyard.tsx';
import AlivePlayersList from './AlivePlayersList/AlivePlayersList.tsx';
import Chatbox from './Chatbox/Chatbox.tsx';
import styles from './MainGame.module.scss';
import classNames from 'classnames/bind';

const cn = classNames.bind(styles);

const MainGame: React.FC = () => {
  return (
    <div className={cn('main-game')}>
      <div className={cn('role-list')}>
        <RoleList roles={[]} />
      </div>
      <div className={cn('role-list')}>
        <Graveyard deadPlayers={[]} />
      </div>
      <div className={cn('day-tracker')}>
        <DayTracker />
      </div>
      <div className={cn('role-information')}>
        <RoleInformation />
      </div>
      <div className={cn('alive-players-list')}>
        <AlivePlayersList players={[]} />
      </div>
      <div className={cn('chatbox-container')}>
        <Chatbox />
      </div>
    </div>
  );
};

export default MainGame;