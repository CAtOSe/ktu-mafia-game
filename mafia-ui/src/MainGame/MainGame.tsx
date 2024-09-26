import React from 'react';
import DayTracker from './DayTracker'; // Adjust the import path as needed
import RoleInformation from './RoleInformation'; // Adjust the import path as needed
import './MainGame.css'; // Optional: Import styles for MainGame

const MainGame: React.FC = () => {
  return (
    <div className="main-game">
      <DayTracker />
      <RoleInformation />
    </div>
  );
};

export default MainGame;
