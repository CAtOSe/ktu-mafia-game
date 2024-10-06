import React, { useState, useEffect, ReactNode, useContext } from 'react';
import DayNightContext from './DayNightContext.ts';
import WebsocketContext from '../WebSocketContext/WebsocketContext.ts';
import { Message, ResponseMessages } from '../../types.ts';

const DayNightProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const websocket = useContext(WebsocketContext);

  const [isDay, setIsDay] = useState(true); // Day or night state
  const [timeRemaining, setTimeRemaining] = useState(0); // Time left in the phase
  const [phase, setPhase] = useState(1); // Phase starts at Day 1

  useEffect(() => {
    if (!websocket) return;

    const handleMessage = (message: Message) => {
      if (message.arguments?.length !== 3) return;

      const newPhase = message.arguments[0];
      const timer = Number(message.arguments[1]);
      const phaseCounter = Number(message.arguments[2]);

      setIsDay(newPhase === 'day');
      setTimeRemaining(timer);
      setPhase(phaseCounter);
    };

    const subId = websocket.subscribe({
      id: 'dayNightContext',
      onReceive: handleMessage,
      filter: [ResponseMessages.PhaseChange],
    });

    return () => websocket.unsubscribe(subId);
  }, [websocket]);

  useEffect(() => {
    const timer = setInterval(() => {
      setTimeRemaining((prevTime) => prevTime - 1000);
    }, 1000);

    return () => {
      clearInterval(timer);
    };
  }, [isDay]);

  return (
    <DayNightContext.Provider value={{ isDay, timeRemaining, phase }}>
      {children}
    </DayNightContext.Provider>
  );
};

export default DayNightProvider;
