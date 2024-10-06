import React, { createContext, useContext, useState, useEffect, ReactNode } from 'react';

interface DayNightContextProps {
  isDay: boolean;
  timeRemaining: number;
  phase: number;
}

const DayNightContext = createContext<DayNightContextProps | undefined>(undefined);

export const useDayNight = () => {
  const context = useContext(DayNightContext);
  if (!context) {
    throw new Error('useDayNight must be used within a DayNightProvider');
  }
  return context;
};

export const DayNightProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const [isDay, setIsDay] = useState(true); // Day or night state
  const [timeRemaining, setTimeRemaining] = useState(15); // Time left in the phase
  const [phase, setPhase] = useState(1); // Phase starts at Day 1

  useEffect(() => {
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

  const toggleDayNight = () => {
    if (!isDay) {
      // Increment phase only when transitioning from night to day
      setPhase((prevPhase) => (isDay ? prevPhase + 1 : prevPhase));
    }
    setIsDay(!isDay); // Toggle day/night
  };

  return (
    <DayNightContext.Provider value={{ isDay, timeRemaining, phase }}>
      {children}
    </DayNightContext.Provider>
  );
};
