import React from 'react';
import './DayTracker.module.scss';
import { useDayNight } from '../../../contexts/DayNightContext/useDayNight.ts';

const DayTracker: React.FC = () => {
  const { isDay, timeRemaining, phase } = useDayNight(); // Get phase from context

  const displayTime = Math.trunc(timeRemaining / 1000);

  return (
    <div>
      <div className={`circle ${isDay ? 'day' : 'night'}`}>
        <p className="phase-text">
          {isDay ? `Day ${phase}` : `Night ${phase}`}
        </p>
        <p className="timer-text">{displayTime}</p>
      </div>
    </div>
  );
};

export default DayTracker;
