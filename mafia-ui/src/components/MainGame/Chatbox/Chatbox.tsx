import React, { useContext, useRef } from 'react';
import styles from './Chatbox.module.scss';
import classNames from 'classnames/bind';
import { GameStateContext } from '../../../contexts/GameStateContext/GameStateContext.tsx';
import { useDayNight } from '../../../contexts/DayNightContext/useDayNight.ts';
import createMessage from '../../../helpers/createMessage.ts';
import { RequestMessages } from '../../../types.ts';
import WebsocketContext from '../../../contexts/WebSocketContext/WebsocketContext.ts';
import { useForm } from 'react-hook-form';
import Button from '../../Button/Button.tsx';

const cn = classNames.bind(styles);
interface ChatForm {
  message: string;
}

const Chatbox: React.FC = () => {
  const {
    gameState: { chatMessages, isAlive },
  } = useContext(GameStateContext);
  const chatBoxRef = useRef<HTMLDivElement>(null);
  // State to store chat messages as an array of Message objects
  const { isDay } = useDayNight();
  const websocket = useContext(WebsocketContext);
  const { register, handleSubmit, setValue } = useForm<ChatForm>();

  // Handle sending a message
  const handleSendMessage = (formValues: ChatForm) => {
    if (formValues.message.trim() && isDay) {
      const messageCategory = isAlive ? 'player' : 'deadPlayer';
      if (websocket) {
        const message = createMessage(RequestMessages.Chat, [
          formValues.message.trim(),
          'everyone',
          messageCategory,
        ]); // Create the message
        websocket.sendMessage(message);
      }
      setValue('message', '');
    }
  };

  return (
    <div className={cn('chatbox')}>
      <div className={cn('chat-display')} ref={chatBoxRef}>
        <div className={cn('chat-item-container')}>
          {chatMessages.map((message, index) => (
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
      </div>
      <form
        className={cn('chat-input-area')}
        onSubmit={handleSubmit(handleSendMessage)}
      >
        <input
          type="text"
          placeholder={
            isDay ? 'Type a message...' : "You can't chat during the night!"
          }
          className={cn('chat-input')}
          disabled={!isDay}
          {...register('message')}
        />
        <Button className={cn('send-button')} disabled={!isDay}>
          Send
        </Button>
      </form>
    </div>
  );
};

export default Chatbox;
