import React from 'react';
import '../MainGame.css';

type RoleListProps = {
  roles: string[];
};

const RoleList: React.FC<RoleListProps> = ({ roles }) => {
     roles = [
        'Killer', 
        'Consort', 
        'Townsfolk', 
        'Townsfolk', 
        'Townsfolk', 
        'Townsfolk', 
        'Townsfolk', 
        'Townsfolk', 
        'Sheriff', 
        'Escort', 
        'Doctor', 
        'Any', 
        'Any', 
        'Any', 
        'Serial Killer'
      ];
  return (
    <div className="list-box gradient-border-element">
      <h3>Role List</h3>
      <ul>
        {roles.slice(0, 15).map((role, index) => (
          <li key={index}>
            {role}
          </li>
        ))}
      </ul>
    </div>
  );
};

export default RoleList;
