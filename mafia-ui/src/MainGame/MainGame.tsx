import React from 'react';
import DayTracker from './DayTracker';
import RoleInformation from './RoleInformation';
import RoleList from './RoleList';
import Graveyard from './Graveyard';
import './MainGame.css';
import AlivePlayersList from './AlivePlayersList';
import Chatbox from './Chatbox';

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
