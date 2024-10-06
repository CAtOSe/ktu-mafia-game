import React, { createContext, useContext, useState, useEffect, ReactNode } from 'react';

interface DayNightContextProps {
  isDay: boolean;
  timeRemaining: number;
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
  const [isDay, setIsDay] = useState(true);
  const [timeRemaining, setTimeRemaining] = useState(15);

  useEffect(() => {
    const timer = setInterval(() => {
      setTimeRemaining((prevTime) => {
        if (prevTime <= 1) {
          clearInterval(timer);
          toggleDayNight();
          return isDay ? 15 : 30;
        }
        return prevTime - 1;
      });
    }, 1000);

    return () => clearInterval(timer);
  }, [isDay]);

  const toggleDayNight = () => {
    setIsDay(!isDay);
  };

  return (
    <DayNightContext.Provider value={{ isDay, timeRemaining }}>
      {children}
    </DayNightContext.Provider>
  );
};
