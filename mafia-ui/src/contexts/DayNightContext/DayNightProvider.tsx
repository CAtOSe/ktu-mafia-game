import React, { useState, useEffect, ReactNode, useContext } from 'react';
import DayNightContext from './DayNightContext.ts';
import WebsocketContext from '../WebSocketContext/WebsocketContext.ts';
import { Message, ResponseMessages } from '../../types.ts';

const DayNightProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const websocket = useContext(WebsocketContext);

  const [isDay, setIsDay] = useState(true); // Day or night state
  const [timeRemaining, setTimeRemaining] = useState(0); // Time left in the phase
  const [phase, setPhase] = useState(1); // Phase starts at Day 1
  const [isPaused, setIsPaused] = useState(false); // New pause state

  useEffect(() => {
    if (!websocket) return;

    const handleMessage = (message: Message) => {
      // Checking, if `message.arguments` are not `undefined`
      if (message.arguments?.[0] === "gameUpdate" && message.arguments.length >= 3) {
        const status = message.arguments[1];
        const remainingTime = Number(message.arguments[2]);

        setIsPaused(status === "paused"); 
        setTimeRemaining(remainingTime);  // Setting time by server status
        return; 
      }

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
    if (isPaused) return; // If it is paused do not use new interval

    const timer = setInterval(() => {
      setTimeRemaining((prevTime) => {
        if (prevTime > 0) return prevTime - 1000;
        clearInterval(timer);
        return 0;
      });
    }, 1000);

    return () => clearInterval(timer); // Deleting interval by changing isPaused or timeRemaining
  }, [isPaused, isDay, timeRemaining]);

  return (
    <DayNightContext.Provider value={{ isDay, timeRemaining, phase, isPaused }}>
      {children}
    </DayNightContext.Provider>
  );
};

export default DayNightProvider;
