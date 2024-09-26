import { useState, useEffect } from 'react';
import { useLocation } from 'react-router-dom';
import './Game.css';

interface GameProps {
    username: string;
    players: string[];
}

const Game: React.FC = () => {
    const location = useLocation();
    const { username, players } = location.state as GameProps;

    const [roles, setRoles] = useState<{ [player: string]: string }>({}); // Store roles for players

    // Function to assign roles when the game starts
    const assignRoles = () => {
        const shuffledPlayers = [...players]; // Copy and shuffle players
        shuffledPlayers.sort(() => Math.random() - 0.5); // Randomize the player list

        const assignedRoles: { [player: string]: string } = {};
        shuffledPlayers.forEach((player, index) => {
            if (index === 0) {
                assignedRoles[player] = 'Killer'; // First player becomes the Killer
            } else {
                assignedRoles[player] = 'Citizen'; // Rest are Citizens
            }
        });

        setRoles(assignedRoles); // Set roles for all players
    };

    // Run the role assignment when the component mounts (i.e., when the game starts)
    useEffect(() => {
        assignRoles();
    }, [players]);

    return (
        <div className="game-container">
            <div className="game-content">
                <h2 className="game-header">Welcome, {username}!</h2>
                <p>Your role: {roles[username]}</p> { /*Display the current user's role*/ }

<h3>Current Players and Roles:</h3>
<ul className="player-list">
    {players.map((player, index) => (
        <li key={index}>
            {player} - {roles[player] || 'Assigning...'} { /*Display role when assigned*/ }
        </li>
    ))}
</ul>
</div>
</div>
);
};

export default Game;
