import React, { useContext, useState } from 'react';
import styles from './DayTracker.module.scss';
import classNames from 'classnames/bind';
import { useDayNight } from '../../../contexts/DayNightContext/useDayNight.ts';
import { GameStateContext } from '../../../contexts/GameStateContext/GameStateContext.tsx';

const cn = classNames.bind(styles);

const DayTracker: React.FC = () => {
  const { isDay, timeRemaining, phase, isPaused: contextPaused } = useDayNight();
  const { gameState: { isHost } } = useContext(GameStateContext);
  const [isPaused, setIsPaused] = useState(contextPaused);
  const displayTime = Math.trunc(timeRemaining / 1000);

  const togglePauseResume = async () => {
    console.log('Sending pause/resume request...');
    setIsPaused(!isPaused); // Changing local button status

    try {
      const response = await fetch('http://localhost:5141/api/gamecontrol/toggle', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ pause: !contextPaused }), // Sending status change
      });

      if (response.ok) {
        const result = await response.text();
        console.log('Server response:', result);
      } else {
        console.error('Request failed:', response.statusText);
      }
    } catch (error) {
      console.error('Error sending request:', error);
    }
  };

  return (
    <div className={cn('wrapper')}>
      <div className={cn('circle', { day: isDay, night: !isDay })}>
        <p className={cn('phase-text')}>
          {isDay ? `Day ${phase}` : `Night ${phase}`}
        </p>
        <p className={cn('timer-text')}>{displayTime}</p>
      </div>
      {isHost && ( // Host can see the button
        <button onClick={togglePauseResume} className={cn('pause-play-button')}>
          {isPaused ? '▶' : '⏸'}
        </button>
      )}
    </div>
  );
};

export default DayTracker;
