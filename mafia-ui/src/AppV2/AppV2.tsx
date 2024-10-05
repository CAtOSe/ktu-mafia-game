
import { GameStateContextProvider } from '../contexts/GameStateContext/GameStateContext.tsx';
import GameLayout from '../components/GameLayout/GameLayout.tsx';
import WebsocketContextProvider from '../contexts/WebSocketContext/WebsocketContextProvider.tsx';
import { DayNightProvider } from '../components/MainGame/DayNightContext/DayNightContext.tsx'; // Importuojame DayNight kontekstą

const AppV2 = () => {
  return (
    <WebsocketContextProvider>
      <GameStateContextProvider>
        <DayNightProvider> {/* Apvyniojame su DayNightProvider */}
          <GameLayout />
        </DayNightProvider>
      </GameStateContextProvider>
    </WebsocketContextProvider>
  );
};

export default AppV2;
