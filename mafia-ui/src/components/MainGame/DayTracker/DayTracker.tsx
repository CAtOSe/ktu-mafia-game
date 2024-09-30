import React, { useState, useEffect } from 'react';
import './DayTracker.module.scss';

const DayTracker: React.FC = () => {
  const [isDay, setIsDay] = useState(true); // Is it Day or is it Night?
  const [phase, setPhase] = useState(1); // Day/night number (Day 1, Night 1, Day 2...)
  const [timeRemaining, setTimeRemaining] = useState(15); // Time remaining until other phase

  useEffect(() => {
    const timer = setInterval(() => {
      setTimeRemaining((prevTime) => {
        if (prevTime <= 1) {
          clearInterval(timer);
          toggleDayNight(); // Switch phase when time runs out
          // Night will last 30s, Day will last 1min30s: 1min for discussion, 30s for voting:
          return isDay ? 15 : 30; // Currently times are shorter for testing
        }
        return prevTime - 1;
      });
    }, 1000);

    return () => clearInterval(timer);
  }, [isDay]);

  const toggleDayNight = () => {
    setIsDay(!isDay); // Switch phase
    setPhase((prevPhase) => (isDay ? prevPhase + 1 : prevPhase)); // Increment phase on new day
  };

  return (
    <div>
      <div className={`circle ${isDay ? 'day' : 'night'}`}>
        <p className="phase-text">
          {isDay ? `Day ${phase}` : `Night ${phase}`}
        </p>
        <p className="timer-text">{timeRemaining}</p>
      </div>
    </div>
  );
};

export default DayTracker;
