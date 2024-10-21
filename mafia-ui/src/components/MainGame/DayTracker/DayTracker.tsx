import React from 'react';
import styles from './DayTracker.module.scss'; 
import classNames from 'classnames/bind';
import { useDayNight } from '../../../contexts/DayNightContext/useDayNight.ts';

const cn = classNames.bind(styles);

const DayTracker: React.FC = () => {
  const { isDay, timeRemaining, phase } = useDayNight(); // Get phase from context
  const displayTime = Math.trunc(timeRemaining / 1000);

  return (
    <div>
      <div className={cn('circle', { day: isDay, night: !isDay })}>
        <p className={cn('phase-text')}>
          {isDay ? `Day ${phase}` : `Night ${phase}`}
        </p>
        <p className={cn('timer-text')}>{displayTime}</p>
      </div>
    </div>
  );
};

export default DayTracker;
