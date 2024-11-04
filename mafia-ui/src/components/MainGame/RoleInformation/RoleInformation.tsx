import React from 'react';
import styles from './RoleInformation.module.scss';
import classNames from 'classnames/bind';
import { useContext } from 'react';
import { GameStateContext } from '../../../contexts/GameStateContext/GameStateContext.tsx';

const cn = classNames.bind(styles);

const RoleInformation: React.FC = () => {
  const { gameState } = useContext(GameStateContext);
  const { username, role, roleAlignment, roleGoal, roleAbility } = gameState;

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
      </div>
    </div>
  );
};
export default RoleInformation;
