import React from 'react';
import './Header.scss';
import MafiaGameLogo from '../Icons/MafiaGameLogo.png';

interface HeaderProps {
    username: string;
    title: string;
    onLogout: () => void; // Function to handle logout
}

const Header: React.FC<HeaderProps> = ({ title, username, onLogout }) => {
    return (
        <header className="header">
            <div className="header-left">
                <img src={MafiaGameLogo} className="app-icon" />
                <h2>{title}</h2>
            </div>
            <div className="header-right">
                <span>{username}</span>
                <button onClick={onLogout} className="logout-button">Logout</button>
            </div>
        </header>
    );
};

export default Header;
