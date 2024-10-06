import React from 'react';
import styles from './RoleInformation.module.scss';
import classNames from 'classnames/bind';
import { useContext } from 'react';
import { GameStateContext } from '../../../contexts/GameStateContext/GameStateContext.tsx';

const cn = classNames.bind(styles);

const RoleInformation: React.FC = () => {
  const { gameState } = useContext(GameStateContext);
  const { username, role } = gameState; 

  let alignment: string;
  let goal: string;
  let primaryAbility: string;
  let item: string = "None";

  if (role === 'Citizen') {
    alignment = 'Town';
    primaryAbility = 'Vote';
    goal = 'Eliminate mafia';
  } else if (role === 'Killer') {
    alignment = 'Mafia';
    primaryAbility = 'Kill';
    goal = 'Kill all town members';
  } else {
    alignment = 'Unknown';
    primaryAbility = 'Unknown';
    goal = 'Unknown';
  }

  return (
    <div className={cn('gradient-border-element')}>
      <div className={cn('info-section')}>
        <ul className={cn('info-list')}>
          <li><strong>Username:</strong> {username}</li>
          <li><strong>Role:</strong> {role}</li>
          <li><strong>Alignment:</strong> {alignment}</li>
          <li><strong>Ability:</strong> {primaryAbility}</li>
          <li><strong>Goal:</strong> {goal}</li>
          <li><strong>Item:</strong> {item}</li>
        </ul>
      </div>
    </div>
  );
};
export default RoleInformation;
