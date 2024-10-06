import React, { useState, useContext } from 'react';
import styles from './Chatbox.module.scss';
import classNames from 'classnames/bind';
import { GameStateContext } from '../../../contexts/GameStateContext/GameStateContext.tsx'; 
import { useDayNight } from '../DayNightContext/DayNightContext.tsx'; 

const cn = classNames.bind(styles);

interface ChatMessage {
  sender?: string; // Optional, since server messages may not have a sender
  content: string;
  recipient?: string; // If sending to everyone, is left empty
  timeSent?: number; // Time in seconds since the game started
  category: 'player' | 'dead-player' | 'night-start' | 'night-action' | 'night-notification' | 'day-start' | 'day-action' | 'day-notification' | 'server';
}

const Chatbox: React.FC = () => {
  const {
    gameState: { username },
  } = useContext(GameStateContext); 

  // State to store chat messages as an array of Message objects
  const [messages, setMessages] = useState<ChatMessage[]>([]);

  // State to store the current input value
  const [inputValue, setInputValue] = useState('');

  const { isDay } = useDayNight(); 

  // Handle input change
  const handleInputChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setInputValue(event.target.value);
  };

  // Handle sending a message
  const handleSendMessage = () => {
    if (inputValue.trim() && isDay) {
      const newMessage: ChatMessage = {
        sender: username,
        content: inputValue, 
        category: 'dead-player'
      };
      setMessages([...messages, newMessage]); 
      setInputValue(''); // Clear the input after sending
    }
  };

  return (
    <div className={cn('chatbox')}>
      <div className={cn('chat-display')}>
        {messages.map((message, index) => (
          <p
          key={index}
          className={cn('chat-message', {
            'player-message': message.category === 'player', // (Alive players messages)
            'dead-player-message': message.category === 'dead-player', // (Dead players messages)
            'night-start-message': message.category === 'night-start',   // (NIGHT 1)
            'night-action-message': message.category === 'night-action',   // (You have chosen to heal John)
            'night-notification-message': message.category === 'night-notification',   // (You have been killed by the killer)
            'day-start-message': message.category === 'day-start',       // (DAY 1)
            'day-action-message': message.category === 'day-action',   // (You have voted for John)
            'day-notification-message': message.category === 'day-notification',   // (John has been executed by the town)
            'server-message': message.category === 'server', // (Other server messages)
          })}
        >
          {message.category === 'player' && (
            <>
              <span className={cn('chat-username')}>{message.sender}</span> {/* (Player username) */}
              : <span className={cn('chat-content')}>{message.content}</span> {/* (Player message content) */}
            </>
          )}
          {message.category === 'dead-player' && (
            <>
              <span className={cn('chat-username-dead')}>{message.sender}</span> {/* (Dead player username) */}
              : <span className={cn('chat-content')}>{message.content}</span> {/* (Player message content) */}
            </>
          )}
          {message.category !== 'player' && message.category !== 'dead-player' && (
            <span className={cn('')}>{message.content}</span> // (Other categories display content only)
          )}
        </p>
        ))}
      </div>
      <div className={cn('chat-input-area')}>
        <input
          type="text"
          value={inputValue}
          onChange={handleInputChange}
          placeholder={isDay ? "Type a message..." : "You can't chat during the night!"}
          className={cn('chat-input')}
          disabled={!isDay}
        />
        <button onClick={handleSendMessage} className={cn('send-button')} disabled={!isDay}>
          Send
        </button>
      </div>
    </div>
  );
};

export default Chatbox;