import React, { useState } from 'react';
import { User } from 'lucide-react';
import styles from './HomePage.module.css';
import { useNavigate } from 'react-router-dom';
import { paths } from '../../routes/paths';
const HomePage = () => {
    const [isOpen, setIsOpen] = useState(false);
    const navigate = useNavigate();
    
    return (
        <div>
            <div className={styles.profileButton}>
                <button onClick={() => setIsOpen(!isOpen)}>
                    <User size={24} />
                </button>
                {isOpen && (
                    <div className={styles.dropdown}>
                        <ul>
                            <li onClick={() => navigate(paths.settings)}>Settings</li>
                        </ul>
                    </div>
                )}
            </div>
            <div className={styles.container}>
                <h1>Welcome Home!</h1>
                <p id="message"></p>
            </div>
        </div>
    );
};

export default HomePage;