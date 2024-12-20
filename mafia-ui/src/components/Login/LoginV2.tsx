import styles from './LoginV2.module.scss';
import classNames from 'classnames/bind';
import Button from '../Button/Button.tsx';
import { ButtonStyle } from '../Button/types.ts';
import { useContext } from 'react';
import { SubmitHandler, useForm } from 'react-hook-form';
import createMessage from '../../helpers/createMessage.ts';
import { RequestMessages } from '../../types.ts';
import { GameStateContext } from '../../contexts/GameStateContext/GameStateContext.tsx';
import WebsocketContext from '../../contexts/WebSocketContext/WebsocketContext.ts';

interface LoginForm {
  username: string;
}

const cn = classNames.bind(styles);

const Login = () => {
  const websocket = useContext(WebsocketContext);
  const { updateGameState } = useContext(GameStateContext);
  const { register, handleSubmit } = useForm<LoginForm>();

  const onSubmit: SubmitHandler<LoginForm> = (form) => {
    const username = form.username;
    updateGameState({ username });
    const message = createMessage(RequestMessages.Login, [username]);
    websocket?.sendMessage(message);
  };

  return (
    <form className={cn('login')} onSubmit={handleSubmit(onSubmit)}>
      <h3 className={cn('login__heading')}>Login</h3>
      <p className={cn('login__info')}>Please enter your username.</p>
      <input
        className={cn('login__input')}
        {...register('username')}
        data-test-id="username-field"
      />
      <p className={cn('login__error')}>
        Username cannot be shorter than 4 characters
      </p>
      <Button style={ButtonStyle.Glow} data-test-id="login-button">
        Login
      </Button>
    </form>
  );
};

export default Login;
