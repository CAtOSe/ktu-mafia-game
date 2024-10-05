import React from 'react';
import styles from './Header.module.scss';
import classNames from 'classnames/bind';
import MafiaGameLogo from '../../icons/MafiaGameLogo.png';

const cn = classNames.bind(styles);

interface HeaderProps {
  username: string;
  title: string;
  onLogout: () => void; // Function to handle logout
}

const Header: React.FC<HeaderProps> = ({ title, username, onLogout }) => {
  return (
    <header className={cn('header')}>
      <div className={cn('header-left')}>
        <img src={MafiaGameLogo} className={cn('app-icon')} />
        <h2>{title}</h2>
      </div>
      <div className={cn('header-right')}>
        <span>{username}</span>
        <button onClick={onLogout} className={cn('logout-button')}>
          Logout
        </button>
      </div>
    </header>
  );
};

export default Header;
