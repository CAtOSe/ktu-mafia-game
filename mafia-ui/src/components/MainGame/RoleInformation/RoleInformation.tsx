import React from 'react';
import styles from './RoleInformation.module.scss';
import classNames from 'classnames/bind';

const cn = classNames.bind(styles);

const RoleInformation: React.FC = () => {
  return (
    <div className={cn('gradient-border-element')}>
      <h2 className={cn('username')}>Username</h2>
      <p className={cn('role-name')}>Role Name</p>
      <div className={cn('info-section')}>
        <p>
          <strong>Alignment:</strong> [Your alignment]
        </p>
        <p>
          <strong>Ability:</strong> [Your ability]
        </p>
        <p>
          <strong>Goal:</strong> [Your goal]
        </p>
        <p>
          <strong>Item:</strong> [Item in hand]
        </p>
      </div>
    </div>
  );
};

export default RoleInformation;
