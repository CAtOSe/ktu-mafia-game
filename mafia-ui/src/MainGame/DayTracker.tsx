import React, { useState, useEffect } from 'react';
import './DayTracker.css';

const DayTracker: React.FC = () => {
  const [isDay, setIsDay] = useState(true); // Start with day phase
  const [phase, setPhase] = useState(1); // Track day/night number
  const [timeRemaining, setTimeRemaining] = useState(15); // Countdown timer

  useEffect(() => {
    const timer = setInterval(() => {
      setTimeRemaining((prevTime) => {
        if (prevTime <= 1) {
          clearInterval(timer);
          toggleDayNight(); // Switch phase when time runs out
          return isDay ? 15 : 30; // Shorter nights, longer days
        }
        return prevTime - 1;
      });
    }, 1000);

    return () => clearInterval(timer);
  }, [isDay]);

  const toggleDayNight = () => {
    setIsDay(!isDay); // Toggle between day and night
    setPhase((prevPhase) => (isDay ? prevPhase + 1 : prevPhase)); // Increment phase on new day
  };

  return (
    <div className="day-tracker">
      <div className={`circle ${isDay ? 'day' : 'night'}`}>
        <p className="phase-text">{isDay ? `Day ${phase}` : `Night ${phase}`}</p>
        <p className="timer-text">{timeRemaining}</p>
      </div>
    </div>
  );
};

export default DayTracker;
