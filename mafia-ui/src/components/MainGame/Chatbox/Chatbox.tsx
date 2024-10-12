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
  sender?: string;
  content: string;
  recipient?: string;
  timeSent?: number;
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

  const handleInputChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setInputValue(event.target.value);
  };

  const handleWebSocketMessage = useCallback(
    (event: any) => {
      try {
        console.log('Raw WebSocket message:', event);
        const data = event.data;

        // Log to check if data is undefined
        console.log('Received WebSocket message:', data);

        if (data === undefined || data === null) {
          console.warn('WebSocket data is undefined or null.');
          return; // Skip further processing if the data is invalid
        }
        console.log('Received WebSocket message:', data);

        if (typeof data !== 'string') {
          console.log('Data is not a string. Converting it to string...');
          if (data instanceof Blob) {
            const reader = new FileReader();
            reader.onload = () => {
              const text = reader.result as string;
              handleParsedMessage(text);
            };
            reader.readAsText(data);
            return;
          } else if (data instanceof ArrayBuffer) {
            const text = new TextDecoder('utf-8').decode(data);
            handleParsedMessage(text);
            return;
          } else {
            console.warn('Unhandled WebSocket data type:', data);
            return; // Log and return to handle unknown types
          }
        } else {
          handleParsedMessage(data); // Process the string message
        }
      } catch (error) {
        console.error('Failed to parse WebSocket message:', error);
      }
    },
    []
  );

  const sendTestMessage = () => {
    if (websocket && websocket.isOpen) {
      websocket.sendMessage(JSON.stringify({ test: 'Hello, world!' }));
    }
  };

  // Add a button to your render method
  <button onClick={sendTestMessage}>Send Test Message</button>


  const handleParsedMessage = (data: string) => {
    try {
      let receivedMessage: ChatMessage;

      try {
        receivedMessage = JSON.parse(data);
      } catch (jsonError) {
        const parts = data.split(':');
        if (parts.length > 1) {
          const base = parts[0];
          const args = parts[1].split(';').filter(x => x);

          receivedMessage = {
            category: base === 'Chat' ? 'player' : 'server',
            sender: args[0] || 'Server',
            content: args[1] || '',
          };
        } else {
          throw new Error('Invalid message format');
        }
      }

      if (receivedMessage && receivedMessage.category === 'player') {
        setMessages((prevMessages) => [...prevMessages, receivedMessage]);
      } else {
        console.log('Received non-player message:', receivedMessage);
      }
    } catch (error) {
      console.error('Failed to handle parsed message:', error);
    }
  };

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

  const handleSendMessage = () => {
    if (inputValue.trim() && isDay) {
      /*const newMessage = {
        Base: 'Chat',
        Arguments: [username, inputValue],
      };*/
      const message = JSON.stringify({
        category: 'player',
        sender: 'Server',
        content: 'This is a test message.',
      });

      console.log('CHATBOX Sending message:', message);

      setMessages((prevMessages) => [
        ...prevMessages,
        { sender: username, content: inputValue, category: 'player' },
      ]);

      if (websocket && websocket.isOpen) {
        console.log('CHATBOX WebSocket is connected.');
        websocket.sendMessage(message);
      } else {
        console.log('CHATBOX WebSocket is not connected.');
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
              'player-message': message.category === 'player',
              'dead-player-message': message.category === 'dead-player',
              'night-start-message': message.category === 'night-start',
              'night-action-message': message.category === 'night-action',
              'night-notification-message':
                message.category === 'night-notification',
              'day-start-message': message.category === 'day-start',
              'day-action-message': message.category === 'day-action',
              'day-notification-message':
                message.category === 'day-notification',
              'server-message': message.category === 'server',
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
            {message.category !== 'player' && message.category !== 'dead-player' && (
              <span className={cn('')}>{message.content}</span>
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



