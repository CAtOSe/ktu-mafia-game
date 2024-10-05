import React from 'react';
import '../MainGame.module.scss';

type RoleListProps = {
  deadPlayers: string[];
};

const Graveyard: React.FC<RoleListProps> = ({ deadPlayers }) => {
  deadPlayers = ['John (Killer)', 'Mark (Townsfolk)', 'Elizabeth (Doctor)'];
  return (
    <div className="list-box alive-player-list">
      <h3>Graveyard</h3>
      <ul>
        {deadPlayers.slice(0, 15).map((player, index) => (
          <li key={index}>{player}</li>
        ))}
      </ul>
    </div>
  );
};

export default Graveyard;
