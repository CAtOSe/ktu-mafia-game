import React, { useContext, useEffect, useState } from 'react';
import DayTracker from './DayTracker/DayTracker.tsx';
import RoleInformation from './RoleInformation/RoleInformation.tsx';
import Graveyard from './Graveyard/Graveyard.tsx';
import AlivePlayersList from './AlivePlayersList/AlivePlayersList.tsx';
import Chatbox from './Chatbox/Chatbox.tsx';
import RoleImage from './RoleImage/RoleImage'; // Import the RoleImage component
import styles from './MainGame.module.scss';
import classNames from 'classnames/bind';
import { GameStateContext } from '../../contexts/GameStateContext/GameStateContext.tsx'; // Example game state context

const cn = classNames.bind(styles);

const MainGame: React.FC = () => {
  const { gameState } = useContext(GameStateContext); // Assuming GameStateContext provides game data
  const [roleName, setRoleName] = useState<string | null>(null);

  useEffect(() => {
    // If gameState contains the current player's role
    if (gameState?.role) {
      setRoleName(gameState.role);
    } else {
      // Optional: Fetch the role name if it's not in the context
      fetch('http://localhost:5141/api/gamecontrol/playerRole') // Replace with actual endpoint
        .then((res) => res.json())
        .then((data) => setRoleName(data.role))
        .catch((err) => console.error('Failed to fetch role:', err));
    }
  }, [gameState]);

  return (
    <div className={cn('main-game')}>
      <div className={cn('role-list')}>
        <Graveyard />
      </div>
      <div className={cn('day-tracker')}>
        <DayTracker />
      </div>
      <div className={cn('role-information')}>
        <RoleInformation />
        {roleName ? <RoleImage roleName={roleName} /> : <p>Loading role...</p>}
      </div>
      <div className={cn('alive-players-list')}>
        <AlivePlayersList />
      </div>
      <div className={cn('chatbox-container')}>
        <Chatbox />
      </div>
    </div>
  );
};

export default MainGame;
