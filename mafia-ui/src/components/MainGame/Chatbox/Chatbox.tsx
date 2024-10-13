import React, { useState, useContext } from 'react';
import styles from './Chatbox.module.scss';
import classNames from 'classnames/bind';
import { GameStateContext } from '../../../contexts/GameStateContext/GameStateContext.tsx';
import { useDayNight } from '../../../contexts/DayNightContext/useDayNight.ts';
import createMessage from "../../../helpers/createMessage.ts";
import {RequestMessages} from "../../../types.ts";
import WebsocketContext from "../../../contexts/WebSocketContext/WebsocketContext.ts";

const cn = classNames.bind(styles);

interface ChatMessage {
  sender?: string; // Optional, since server messages may not have a sender
  content: string;
  recipient?: string; // If sending to everyone, is left empty
  timeSent?: number; // Time in seconds since the game started
  category:
    | 'player'
    | 'deadPlayer'
    | 'nightStart'
    | 'nightAction'
    | 'nightNotification'
    | 'dayStart'
    | 'dayAction'
    | 'dayNotification'
    | 'server';
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
  const websocket = useContext(WebsocketContext);

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
        category: 'player',
      };
      setMessages([...messages, newMessage]);
      setInputValue('');
      if (websocket) {
        const message = createMessage(RequestMessages.Chat, [
          inputValue,
          '',
          'player',
        ]); // Create the message
        websocket.sendMessage(message);
      }
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
              'dead-player-message': message.category === 'deadPlayer', // (Dead players messages)
              'night-start-message': message.category === 'nightStart', // (NIGHT 1)
              'night-action-message': message.category === 'nightAction', // (You have chosen to heal John)
              'night-notification-message':
                message.category === 'nightNotification', // (You have been killed by the killer)
              'day-start-message': message.category === 'dayStart', // (DAY 1)
              'day-action-message': message.category === 'dayAction', // (You have voted for John)
              'day-notification-message':
                message.category === 'dayNotification', // (John has been executed by the town)
              'server-message': message.category === 'server', // (Other server messages)
            })}
          >
            {message.category === 'player' && (
              <>
                <span className={cn('chat-username')}>{message.sender}</span>{' '}
                {/* (Player username) */}:{' '}
                <span className={cn('chat-content')}>{message.content}</span>{' '}
                {/* (Player message content) */}
              </>
            )}
            {message.category === 'deadPlayer' && (
              <>
                <span className={cn('chat-username-dead')}>
                  {message.sender}
                </span>{' '}
                {/* (Dead player username) */}:{' '}
                <span className={cn('chat-content')}>{message.content}</span>{' '}
                {/* (Player message content) */}
              </>
            )}
            {message.category !== 'player' &&
              message.category !== 'deadPlayer' && (
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
          placeholder={
            isDay ? 'Type a message...' : "You can't chat during the night!"
          }
          className={cn('chat-input')}
          disabled={!isDay}
        />
        <button
          onClick={handleSendMessage}
          className={cn('send-button')}
          disabled={!isDay}
        >
          Send
        </button>
      </div>
    </div>
  );
};

export default Chatbox;
