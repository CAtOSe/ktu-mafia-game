import React from 'react';
import '../MainGame.module.scss';

type RoleListProps = {
  roles: string[];
};

const RoleList: React.FC<RoleListProps> = ({ roles }) => {
  roles = [
    'Killer',
    'Citizen',
    'Citizen',
    'Citizen',
  ];
  return (
    <div className="list-box alive-player-list">
      <h3>Role List</h3>
      <ul>
        {roles.slice(0, 15).map((role, index) => (
          <li key={index}>{role}</li>
        ))}
      </ul>
    </div>
  );
};

export default RoleList;
