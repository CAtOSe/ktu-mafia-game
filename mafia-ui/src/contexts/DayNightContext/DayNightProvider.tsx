import React, { useState, useEffect, ReactNode, useContext } from 'react';
import DayNightContext from './DayNightContext.ts';
import WebsocketContext from '../WebSocketContext/WebsocketContext.ts';
import { Message, ResponseMessages } from '../../types.ts';
import { GameStateContext } from '../GameStateContext/GameStateContext.tsx';

const DayNightProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const websocket = useContext(WebsocketContext);
  const {
    gameState: { isPaused },
  } = useContext(GameStateContext);

  const [isDay, setIsDay] = useState(true); // Day or night state
  const [timeRemaining, setTimeRemaining] = useState(0); // Time left in the phase
  const [phase, setPhase] = useState(1); // Phase starts at Day 1

  useEffect(() => {
    if (!websocket) return;

    const handleMessage = (message: Message) => {
      switch (message.base) {
        case ResponseMessages.PhaseChange: {
          if (message.arguments?.length !== 3) return;

          const newPhase = message.arguments[0];
          const timer = Number(message.arguments[1]);
          const phaseCounter = Number(message.arguments[2]);

          setIsDay(newPhase === 'day');
          setTimeRemaining(timer);
          setPhase(phaseCounter);

          return;
        }

        case ResponseMessages.GameUpdate: {
          if (message.arguments?.length !== 2) return;

          setTimeRemaining(Number(message.arguments[1]));
          return;
        }
      }
    };

    const subId = websocket.subscribe({
      id: 'dayNightContext',
      onReceive: handleMessage,
      filter: [ResponseMessages.PhaseChange, ResponseMessages.GameUpdate],
    });

    return () => websocket.unsubscribe(subId);
  }, [websocket]);

  useEffect(() => {
    if (isPaused) return; // If it is paused do not set a new interval

    const intervalTime = timeRemaining > 1000 ? 1000 : timeRemaining;

    const timer = setInterval(() => {
      setTimeRemaining((prevTime) => {
        if (prevTime > 0) return prevTime - 1000;
        clearInterval(timer);
        return 0;
      });
    }, intervalTime);

    return () => clearInterval(timer); // Deleting interval by changing isPaused or timeRemaining
  }, [isPaused, isDay, timeRemaining]);

  return (
    <DayNightContext.Provider value={{ isDay, timeRemaining, phase, isPaused }}>
      {children}
    </DayNightContext.Provider>
  );
};

export default DayNightProvider;
