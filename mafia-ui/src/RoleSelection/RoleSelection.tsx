import { useEffect, useState } from 'react';
import './RoleSelection.css'; // Add styles for animations

const roles = ['Townie', 'Mafia', 'Doctor', 'Sheriff', 'Investigator', 'Jester', 'Werewolf', 'Godfather'];

// Define which roles are townsfolk and which are evil
const townsfolkRoles = ['Townie', 'Doctor', 'Sheriff', 'Investigator'];
//const evilRoles = ['Mafia', 'Jester', 'Werewolf', 'Godfather'];

interface RoleSelectionProps {
    targetRole: string;
}

const RoleSelection: React.FC<RoleSelectionProps> = ({ targetRole }) => {
    const [currentRole, setCurrentRole] = useState<string>(roles[0]);
    const [isRandomizing, setIsRandomizing] = useState<boolean>(true);

    // Determine if the role is townsfolk or evil
    const isTownsfolk = townsfolkRoles.includes(targetRole);
    const roleClass = isTownsfolk ? 'townsfolk' : 'evil';

    useEffect(() => {
        let interval: number;
        let timeout: number;

        if (isRandomizing) {
            const speed = 100;

            interval = window.setInterval(() => {
                const randomIndex = Math.floor(Math.random() * roles.length);
                setCurrentRole(roles[randomIndex]);
            }, speed);

            timeout = window.setTimeout(() => {
                clearInterval(interval);
                slowDownAnimation(speed);
            }, 2000); // Randomize for 2 seconds before slowing down
        }

        const slowDownAnimation = (initialSpeed: number) => {
            let speed = initialSpeed;
            const slowDownInterval = window.setInterval(() => {
                speed += 100;
                clearInterval(interval);
                interval = window.setInterval(() => {
                    const randomIndex = Math.floor(Math.random() * roles.length);
                    setCurrentRole(roles[randomIndex]);
                }, speed);

                if (speed >= 600) {
                    clearInterval(slowDownInterval);
                    window.setTimeout(() => {
                        setCurrentRole(targetRole);
                        setIsRandomizing(false);
                    }, 600);
                }
            }, 500);
        };

        return () => {
            clearInterval(interval);
            clearTimeout(timeout);
        };
    }, [isRandomizing, targetRole]);

    return (
        <div className="role-selection-container">
            <div className={`role-box ${isRandomizing ? 'spinning' : ''} ${!isRandomizing ? roleClass : ''} ${!isRandomizing ? 'glow' : ''}`}>
                <p>{currentRole}</p>
            </div>
            {!isRandomizing && (
                <div className="final-role-message">
                    <p>Your role is: <strong className={`role-name ${roleClass}`}>{targetRole}</strong></p>
                </div>
            )}
        </div>
    );
};

export default RoleSelection;
