import React from 'react';
import { useNavigate } from 'react-router-dom';
import styles from './SettingsPage.module.css';
import { paths } from '../../routes/paths';

const SettingsPage = () => {
    const navigate = useNavigate();

    return (
        <div className={styles.pageWrapper}>
            <div className={styles.container}>
                <h1>Settings</h1>
                <ul className={styles.settingsList}>
                    <li onClick={() => navigate(paths.editProfile)}>
                        Edit Profile
                    </li>
                </ul>
            </div>
        </div>
    );
};

export default SettingsPage;