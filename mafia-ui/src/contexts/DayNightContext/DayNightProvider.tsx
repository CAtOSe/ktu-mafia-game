import React, { useState, useEffect, ReactNode } from 'react';
import DayNightContext from './DayNightContext.ts';

const DayNightProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const [isDay, setIsDay] = useState(true); // Day or night state
  const [timeRemaining, setTimeRemaining] = useState(15); // Time left in the phase
  const [phase, setPhase] = useState(1); // Phase starts at Day 1

  useEffect(() => {
    const toggleDayNight = () => {
      if (!isDay) {
        // Increment phase only when transitioning from night to day
        setPhase((prevPhase) => (isDay ? prevPhase + 1 : prevPhase));
      }
      setIsDay(!isDay); // Toggle day/night
    };

    const timer = setInterval(() => {
      setTimeRemaining((prevTime) => {
        if (prevTime <= 1) {
          clearInterval(timer);
          toggleDayNight(); // Toggle between day and night when time runs out
          return isDay ? 15 : 30; // Return the appropriate time for next phase (30s night, 15s day)
        }
        return prevTime - 1;
      });
    }, 1000);

    return () => clearInterval(timer);
  }, [isDay]);

  return (
    <DayNightContext.Provider value={{ isDay, timeRemaining, phase }}>
      {children}
    </DayNightContext.Provider>
  );
};

export default DayNightProvider;