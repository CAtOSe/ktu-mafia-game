import React, { useState } from 'react';
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

export default Chatbox;
