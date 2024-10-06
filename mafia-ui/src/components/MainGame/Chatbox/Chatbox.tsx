//ORIGINAL VERSION
/*import React, { useState } from 'react';
import styles from './Chatbox.module.scss';
import classNames from 'classnames/bind';

const cn = classNames.bind(styles);

const Chatbox: React.FC = () => {
  // State to store chat messages
  const [messages, setMessages] = useState<string[]>([]);

  // State to store the current input value
  const [inputValue, setInputValue] = useState('');

  // Handle input change
  const handleInputChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setInputValue(event.target.value);
  };

  // Handle sending a message
  const handleSendMessage = () => {
    if (inputValue.trim()) {
      setMessages([...messages, inputValue]); // Add the message to the chat
      setInputValue(''); // Clear the input after sending
    }
  };

  return (
    <div className={cn('chatbox')}>
      <div className={cn('chat-display')}>
        {messages.map((message, index) => (
          <p key={index} className={cn('chat-message')}>
            {message}
          </p>
        ))}
      </div>
      <div className={cn('chat-input-area')}>
        <input
          type="text"
          value={inputValue}
          onChange={handleInputChange}
          placeholder="Type a message..."
          className={cn('chat-input')}
        />
        <button onClick={handleSendMessage} className={cn('send-button')}>
          Send
        </button>
      </div>
    </div>
  );
};

export default Chatbox;*/



import React, { useState } from 'react';
import styles from './Chatbox.module.scss';
import classNames from 'classnames/bind';
import { useDayNight } from '../DayNightContext/DayNightContext.tsx'; 

const cn = classNames.bind(styles);

const Chatbox: React.FC = () => {
  const { isDay } = useDayNight(); 
  const [messages, setMessages] = useState<string[]>([]);
  const [inputValue, setInputValue] = useState('');

  const handleInputChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setInputValue(event.target.value);
  };

  const handleSendMessage = () => {
    if (inputValue.trim() && isDay) {
      setMessages([...messages, inputValue]);
      setInputValue('');
    }
  };

  return (
    <div className={cn('chatbox')}>
      <div className={cn('chat-display')}>
        {messages.map((message, index) => (
          <p key={index} className={cn('chat-message')}>
            {message}
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






//SECOND VERSION
/*import React, { useState, useEffect, useRef } from 'react';
import styles from './Chatbox.module.scss';
import classNames from 'classnames/bind';

const cn = classNames.bind(styles);

const Chatbox: React.FC = () => {
  const [messages, setMessages] = useState<string[]>([]);
  const [inputValue, setInputValue] = useState('');
  const ws = useRef<WebSocket | null>(null); // WebSocket reference

  // Initialize WebSocket connection
  useEffect(() => {
    ws.current = new WebSocket('ws://localhost:5000/ws'); // adjust the URL according to your server

    ws.current.onmessage = (event) => {
      const message = event.data;
      setMessages((prevMessages) => [...prevMessages, message]); // Add incoming messages to chat
    };

    return () => {
      ws.current?.close();
    };
  }, []);

  // Handle input change
  const handleInputChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setInputValue(event.target.value);
  };

  // Handle sending a message
  const handleSendMessage = () => {
    if (inputValue.trim() && ws.current?.readyState === WebSocket.OPEN) {
      ws.current.send(inputValue); // Send message to server
      setInputValue(''); // Clear the input after sending
    }
  };

  return (
    <div className={cn('chatbox')}>
      <div className={cn('chat-display')}>
        {messages.map((message, index) => (
          <p key={index} className={cn('chat-message')}>
            {message}
          </p>
        ))}
      </div>
      <div className={cn('chat-input-area')}>
        <input
          type="text"
          value={inputValue}
          onChange={handleInputChange}
          placeholder="Type a message..."
          className={cn('chat-input')}
        />
        <button onClick={handleSendMessage} className={cn('send-button')}>
          Send
        </button>
      </div>
    </div>
  );
};

export default Chatbox;*/

/*import React, { useState, useEffect, useRef } from 'react';
import styles from './Chatbox.module.scss';
import classNames from 'classnames/bind';

const cn = classNames.bind(styles);

const Chatbox: React.FC = () => {
  // State to store chat messages
  const [messages, setMessages] = useState<string[]>([]);
  const [inputValue, setInputValue] = useState('');
  const webSocket = useRef<WebSocket | null>(null); // WebSocket reference

  // Initialize WebSocket connection when the component mounts
  useEffect(() => {
    webSocket.current = new WebSocket('ws://localhost:your_port/ws');

    // When receiving a message from the WebSocket server
    webSocket.current.onmessage = (event) => {
      const message = event.data;
      setMessages((prevMessages) => [...prevMessages, message]); // Update messages state
    };

    return () => {
      webSocket.current?.close(); // Cleanup WebSocket on component unmount
    };
  }, []);

  // Handle input change
  const handleInputChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setInputValue(event.target.value);
  };

  // Handle sending a message
  const handleSendMessage = () => {
    if (inputValue.trim() && webSocket.current?.readyState === WebSocket.OPEN) {
      webSocket.current.send(inputValue); // Send message to WebSocket server
      setInputValue(''); // Clear input after sending
    }
  };

  return (
    <div className={cn('chatbox')}>
      <div className={cn('chat-display')}>
        {messages.map((message, index) => (
          <p key={index} className={cn('chat-message')}>
            {message}
          </p>
        ))}
      </div>
      <div className={cn('chat-input-area')}>
        <input
          type="text"
          value={inputValue}
          onChange={handleInputChange}
          placeholder="Type a message..."
          className={cn('chat-input')}
        />
        <button onClick={handleSendMessage} className={cn('send-button')}>
          Send
        </button>
      </div>
    </div>
  );
};

export default Chatbox;*/

