import React from 'react';
import './RoleInformation.css';
import './MainGame.css';

const RoleInformation: React.FC = () => {
  return (
    <div className="gradient-border-element">
      <h2 className="username">Username</h2>
      <p className="role-name">Role Name</p>
      <div className="info-section">
        <p><strong>Alignment:</strong> [Your alignment]</p>
        <p><strong>Ability:</strong> [Your ability]</p>
        <p><strong>Goal:</strong> [Your goal]</p>
        <p><strong>Item:</strong> [Item in hand]</p>
      </div>
    </div>
  );
};

export default RoleInformation;
