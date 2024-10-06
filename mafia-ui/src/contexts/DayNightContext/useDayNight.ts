import { useContext } from 'react';
import DayNightContext from './DayNightContext.ts';

export const useDayNight = () => {
  const context = useContext(DayNightContext);
  if (!context) {
    throw new Error('useDayNight must be used within a DayNightProvider');
  }
  return context;
};
