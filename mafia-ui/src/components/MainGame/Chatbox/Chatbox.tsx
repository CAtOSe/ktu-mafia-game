/* ORIGINAL
import React, { useState, useContext } from 'react';
import styles from './Chatbox.module.scss';
import classNames from 'classnames/bind';
import { GameStateContext } from '../../../contexts/GameStateContext/GameStateContext.tsx';
import { useDayNight } from '../../../contexts/DayNightContext/useDayNight.ts';

const cn = classNames.bind(styles);

interface ChatMessage {
  sender?: string; // Optional, since server messages may not have a sender
  content: string;
  recipient?: string; // If sending to everyone, is left empty
  timeSent?: number; // Time in seconds since the game started
  category:
    | 'player'
    | 'dead-player'
    | 'night-start'
    | 'night-action'
    | 'night-notification'
    | 'day-start'
    | 'day-action'
    | 'day-notification'
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
              'night-start-message': message.category === 'night-start', // (NIGHT 1)
              'night-action-message': message.category === 'night-action', // (You have chosen to heal John)
              'night-notification-message':
                message.category === 'night-notification', // (You have been killed by the killer)
              'day-start-message': message.category === 'day-start', // (DAY 1)
              'day-action-message': message.category === 'day-action', // (You have voted for John)
              'day-notification-message':
                message.category === 'day-notification', // (John has been executed by the town)
              'server-message': message.category === 'server', // (Other server messages)
            })}
          >
            {message.category === 'player' && (
              <>
                <span className={cn('chat-username')}>{message.sender}</span>{' '}
                { (Player username) }:{' '}
                <span className={cn('chat-content')}>{message.content}</span>{' '}
                { (Player message content) }
              </>
            )}
            {message.category === 'dead-player' && (
              <>
                <span className={cn('chat-username-dead')}>
                  {message.sender}
                </span>{' '}
                { (Dead player username) }:{' '}
                <span className={cn('chat-content')}>{message.content}</span>{' '}
                { (Player message content) }
              </>
            )}
            {message.category !== 'player' &&
              message.category !== 'dead-player' && (
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

export default Chatbox;*/

import React, { useState, useContext, useEffect, useCallback } from 'react';
import styles from './Chatbox.module.scss';
import classNames from 'classnames/bind';
import { GameStateContext } from '../../../contexts/GameStateContext/GameStateContext.tsx';
import { useDayNight } from '../../../contexts/DayNightContext/useDayNight.ts';
import WebsocketContext from '../../../contexts/WebSocketContext/WebsocketContext.ts'; // Import WebSocket context


const cn = classNames.bind(styles);

interface ChatMessage {
  sender?: string; // Optional, since server messages may not have a sender
  content: string;
  recipient?: string; // If sending to everyone, is left empty
  timeSent?: number; // Time in seconds since the game started
  category:
    | 'player'
    | 'dead-player'
    | 'night-start'
    | 'night-action'
    | 'night-notification'
    | 'day-start'
    | 'day-action'
    | 'day-notification'
    | 'server';
}

const Chatbox: React.FC = () => {
  const {
    gameState: { username },
  } = useContext(GameStateContext);
  const websocket = useContext(WebsocketContext); // WebSocket context

  const [messages, setMessages] = useState<ChatMessage[]>([]);
  const [inputValue, setInputValue] = useState('');
  const { isDay } = useDayNight();

  // Handle input change
  const handleInputChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setInputValue(event.target.value);
  };

  // Function to handle incoming WebSocket messages
  const handleWebSocketMessage = useCallback(
    (data: any) => {
      try {
        const receivedMessage: ChatMessage = JSON.parse(data);
        console.log('Received WebSocket message:', receivedMessage);

        // Checking if the message are player
        if (receivedMessage.category === 'player') {
          setMessages((prevMessages) => [...prevMessages, receivedMessage]);
        } else {
          console.log('Received non-player message:', receivedMessage);
        }
      } catch (error) {
        console.error('Failed to parse WebSocket message:', error);
      }
    },
    []
  );

  // Subscribe to WebSocket messages
  useEffect(() => {
    if (!websocket) return;

    const subId = websocket.subscribe({
      id: 'chatMessages',
      onReceive: handleWebSocketMessage,
    });

    return () => {
      websocket.unsubscribe(subId);
    };
  }, [websocket, handleWebSocketMessage]);

  // Handle sending a message
  /*const handleSendMessage = () => {
    if (inputValue.trim() && isDay) {
      const newMessage: ChatMessage = {
        sender: username,
        content: inputValue,
        category: 'player',
      };
      
      // Add message to local state
      setMessages([...messages, newMessage]);

      // Send message through WebSocket to other players
      if (websocket && websocket.isOpen) {
        websocket.sendMessage(JSON.stringify(newMessage));
      }

      // Clear the input after sending
      setInputValue('');
    }
  };*/
  const handleSendMessage = () => {
    if (inputValue.trim() && isDay) {
      const newMessage: ChatMessage = {
        sender: username,
        content: inputValue,
        category: 'player',
      };
      console.log('CHATBOX Sending message:', newMessage); 

      setMessages((prevMessages) => [...prevMessages, newMessage]);
      if (websocket && websocket.isOpen) {
        websocket.sendMessage(JSON.stringify(newMessage)); 
      } else {
        console.log('CHATBOX WebSocket is not open'); 
      }

      setInputValue(''); 
    } else {
      console.warn('CHATBOX Input value is empty or it is night'); 
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
              'night-start-message': message.category === 'night-start', // (NIGHT 1)
              'night-action-message': message.category === 'night-action', // (You have chosen to heal John)
              'night-notification-message':
                message.category === 'night-notification', // (You have been killed by the killer)
              'day-start-message': message.category === 'day-start', // (DAY 1)
              'day-action-message': message.category === 'day-action', // (You have voted for John)
              'day-notification-message':
                message.category === 'day-notification', // (John has been executed by the town)
              'server-message': message.category === 'server', // (Other server messages)
            })}
          >
            {message.category === 'player' && (
              <>
                <span className={cn('chat-username')}>{message.sender}</span>{' '}
                
                <span className={cn('chat-content')}>{message.content}</span>{' '}
                
              </>
            )}
            {message.category === 'dead-player' && (
              <>
                <span className={cn('chat-username-dead')}>
                  {message.sender}
                </span>{' '}
                (Dead player username):{' '}
                <span className={cn('chat-content')}>{message.content}</span>{' '}
                (Player message content)
              </>
            )}
            {message.category !== 'player' &&
              message.category !== 'dead-player' && (
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


