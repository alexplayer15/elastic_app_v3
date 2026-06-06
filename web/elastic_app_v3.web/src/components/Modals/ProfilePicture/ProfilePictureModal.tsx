import React from 'react';
import { createPortal } from 'react-dom';
import styles from './ProfilePictureModal.module.css';

type Props = {
    onClose: () => void;
};

const ProfilePictureModal = ({ onClose }: Props) => {
    return createPortal(
        <div className={styles.overlay} onClick={onClose}>
            <div className={styles.modal} onClick={e => e.stopPropagation()}>
                <h2>Profile Picture</h2>
                <div className={styles.placeholder}>
                    No profile picture set
                </div>
                <div className={styles.actions}>
                    <button>Upload New</button>
                    <button>Remove</button>
                </div>
                <button onClick={onClose}>Cancel</button>
            </div>
        </div>,
        document.body
    );
};

export default ProfilePictureModal;