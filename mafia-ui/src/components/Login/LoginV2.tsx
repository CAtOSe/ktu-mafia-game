import styles from './LoginV2.module.scss';
import classNames from 'classnames/bind';
import Button from '../Button/Button.tsx';
import { ButtonStyle } from '../Button/types.ts';

const cn = classNames.bind(styles);

const Login = () => {
  return (
    <div className={cn('login')}>
      <div className={cn('login__form')}>
        <h3 className={cn('login__heading')}>Login</h3>
        <p className={cn('login__info')}>Please enter your username.</p>
        <input className={cn('login__input')} />
        <p className={cn('login__error')}>
          Username cannot be shorter than 4 characters
        </p>
        <Button style={ButtonStyle.Glow}>Login</Button>
      </div>
    </div>
  );
};

export default Login;
