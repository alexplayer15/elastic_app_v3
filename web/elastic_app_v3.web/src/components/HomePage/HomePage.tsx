import React, { useState } from 'react';
import { User } from 'lucide-react';
import styles from './HomePage.module.css';
import { useNavigate } from 'react-router-dom';
import subnautica from '../../assets/subnautica.jpg'
import { paths } from '../../routes/paths';
const HomePage = () => {
    const [isOpen, setIsOpen] = useState(false);
    const navigate = useNavigate();
    
    return (
        <div>
            <div className={styles.topRibbon}>
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
            </div>
            <div className={styles.hero}>
                <h1 className={styles.sloganText}>Get Out There!</h1>
            </div>
        </div>
    );
};

export default HomePage;