import React, { useState } from 'react';
import styles from './EditProfile.module.css';
import ProfilePictureModal from "../Modals/ProfilePicture/ProfilePictureModal";

const EditProfilePage = () => {
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [profilePictureUrl, setProfilePictureUrl] = useState<string | null>(null);

    return (
        <div className={styles.pageWrapper}>
            <div className={styles.container}>
                <h1>Edit Profile</h1>
                {profilePictureUrl && (
                    <img src={profilePictureUrl} alt="Profile" className={styles.profilePicture} />
                )}
                <button className={styles.editButton} onClick={() => setIsModalOpen(true)}>
                    Edit Profile Picture
                </button>
                {isModalOpen && (
                    <ProfilePictureModal
                        onClose={() => setIsModalOpen(false)}
                        onUploadSuccess={(url) => setProfilePictureUrl(url)}
                    />
                )}
            </div>
        </div>
    );
};

export default EditProfilePage;