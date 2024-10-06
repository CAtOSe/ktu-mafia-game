import { GameStateContextProvider } from '../contexts/GameStateContext/GameStateContext.tsx';
import GameLayout from '../components/GameLayout/GameLayout.tsx';
import WebsocketContextProvider from '../contexts/WebSocketContext/WebsocketContextProvider.tsx';
import DayNightProvider from '../contexts/DayNightContext/DayNightProvider.tsx';

const AppV2 = () => {
  return (
    <WebsocketContextProvider>
      <GameStateContextProvider>
        <DayNightProvider>
          <GameLayout />
        </DayNightProvider>
      </GameStateContextProvider>
    </WebsocketContextProvider>
  );
};

export default AppV2;
