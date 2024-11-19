import React, { useContext, useEffect, useState } from 'react';
import styles from './RoleInformation.module.scss';
import classNames from 'classnames/bind';
import { GameStateContext } from '../../../contexts/GameStateContext/GameStateContext.tsx';
import RoleImage from '../RoleImage/RoleImage';

const cn = classNames.bind(styles);

const RoleInformation: React.FC = () => {
  const { gameState } = useContext(GameStateContext);
  const { username, role, roleAlignment, roleGoal, roleAbility } = gameState;
  const [roleName, setRoleName] = useState<string | null>(null);

  useEffect(() => {
    if (gameState?.role) {
      setRoleName(gameState.role);
    } else {
      fetch('http://localhost:5141/api/gamecontrol/playerRole')
        .then((res) => res.json())
        .then((data) => setRoleName(data.role))
        .catch((err) => console.error('Failed to fetch role:', err));
    }
  }, [gameState]);

  return (
    <div className={cn('gradient-border-element')}>
      <div className={cn('info-section')}>
        <ul className={cn('info-list')}>
          <li>
            <strong>Username:</strong> {username}
          </li>
          <li>
            <strong>Role:</strong> {role}
          </li>
          <li>
            <strong>Alignment:</strong> {roleAlignment}
          </li>
          <li>
            <strong>Ability:</strong> {roleAbility}
          </li>
          <li>
            <strong>Goal:</strong> {roleGoal}
          </li>
        </ul>
        <div className={cn('role-image-container')}>
          {roleName ? <RoleImage roleName={roleName} /> : <p>Loading role...</p>}
        </div>
      </div>
    </div>
  );
};

export default RoleInformation;
