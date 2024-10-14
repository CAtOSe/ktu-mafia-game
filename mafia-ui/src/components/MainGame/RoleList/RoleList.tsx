import { useContext } from 'react';
import { GameStateContext } from '../../../contexts/GameStateContext/GameStateContext.tsx';

const RoleList = () => {
  const {
    gameState: { inventory },
  } = useContext(GameStateContext);

  return (
    <div className="list-box alive-player-list">
      <h3></h3>
      <ul>
        {inventory.map((item, index) => (
          <li key={index}>{item}</li>
        ))}
      </ul>
    </div>
  );
};

export default RoleList;
