import { createContext } from 'react';
import {
  GameStage,
  GameState,
  GameStateContextProviderProps,
} from './types.ts';

const defaultState: GameState = {
  gameStage: GameStage.Login,
  username: '',
  role: '',
};

export const GameStateContext = createContext<GameState>(defaultState);

export const GameStateContextProvider = ({
  children,
}: GameStateContextProviderProps) => {
  return (
    <GameStateContext.Provider value={defaultState}>
      {children}
    </GameStateContext.Provider>
  );
};
