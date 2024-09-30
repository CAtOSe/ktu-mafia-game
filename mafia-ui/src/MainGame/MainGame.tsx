import React from 'react';
import DayTracker from './DayTracker/DayTracker.tsx';
import RoleInformation from './RoleInformation/RoleInformation.tsx';
import RoleList from './RoleList/RoleList.tsx';
import Graveyard from './Graveyard/Graveyard.tsx';
import './MainGame.css';
import AlivePlayersList from './AlivePlayersList/AlivePlayersList.tsx';
import Chatbox from './Chatbox/Chatbox.tsx';

const MainGame: React.FC = () => {
  return (
    <div className="main-game">
      <div className="role-list"><RoleList roles = {[]} /></div>
      <div className="role-list"><Graveyard deadPlayers = {[]} /></div>
      <div className="day-tracker"><DayTracker /></div>
      <div className="role-information">
        <RoleInformation />

      </div>
      <div className="alive-players-list"><AlivePlayersList players = {[]} /></div>
      <div className="chatbox-container"><Chatbox/></div>
    </div>
  );
};

export default MainGame;
