import React from 'react';
import './RoleInformation.css'; // Import the CSS file, not the TSX file

const RoleInformation: React.FC = () => {
  return (
    <div className="role-information">
      <h2 className="username">Username</h2>
      <p className="role-name">Role Name</p>
      <div className="info-section">
        <p><strong>Alignment:</strong> [Your alignment]</p>
        <p><strong>Ability:</strong> [Your ability]</p>
        <p><strong>Goal:</strong> [Your goal]</p>
      </div>
    </div>
  );
};

export default RoleInformation;
