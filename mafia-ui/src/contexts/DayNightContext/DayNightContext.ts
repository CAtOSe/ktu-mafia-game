import { createContext } from 'react';
import { DayNightContextProps } from './types.ts';

const DayNightContext = createContext<DayNightContextProps | undefined>(
  undefined,
);

export default DayNightContext;
