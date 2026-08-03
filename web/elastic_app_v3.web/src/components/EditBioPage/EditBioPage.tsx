import styles from './EditBio.module.css';
import Button from '../Buttons/Button';
import { useState } from 'react';
import { Fragment } from 'react'

const EditBioPage = () => {
    const [bio, setBio] = useState('Your bio goes here');
    const [isEditing, setIsEditing] = useState(false);
    
    return (
        <div className={styles.page}>
            <div className={styles.bioBox} >
                { isEditing ? (
                    <Fragment>
                        <textarea 
                            className={styles.bioContent} 
                            value={bio}
                            onChange={(e) => 
                                setBio(e.target.value)}
                            autoFocus
                        ></textarea>
                        <Button title="Save"   style={styles.saveButton} />
                        <Button title="Cancel" style={styles.cancelButton} />
                    </Fragment>
                ) : (
                    <p className={styles.bioContent} 
                       onClick={() => setIsEditing(true)}
                    >
                        {bio}
                    </p>
                )}
        
            </div>
        </div>
    )
}

export default EditBioPage;