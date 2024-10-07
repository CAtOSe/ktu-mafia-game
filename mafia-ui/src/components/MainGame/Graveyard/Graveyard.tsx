import { useContext, useMemo } from 'react';
import { GameStateContext } from '../../../contexts/GameStateContext/GameStateContext.tsx';

const Graveyard = () => {
  const {
    gameState: { alivePlayers, players },
  } = useContext(GameStateContext);

  const deadPlayers = useMemo(
    () => players.filter((p) => !alivePlayers.includes(p)),
    [alivePlayers, players],
  );

  return (
    <div className="list-box alive-player-list">
      <h3>Graveyard</h3>
      <ul>
        {deadPlayers.map((player, index) => (
          <li key={index}>{player}</li>
        ))}
      </ul>
    </div>
  );
};

export default Graveyard;
