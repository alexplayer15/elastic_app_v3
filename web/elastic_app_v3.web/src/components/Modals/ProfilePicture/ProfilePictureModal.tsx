import React, { useRef, useState } from 'react';
import { createPortal } from 'react-dom';
import { useGetProfilePictureUploadUrls } from '../../../hooks/useGetProfilePictureUploadUrls';
import { useSaveProfilePicture } from '../../../hooks/useSaveProfilePicture';
import styles from './ProfilePictureModal.module.css';

type ModalStep = 'view' | 'preview' | 'uploading';

type Props = {
    onClose: () => void;
    onUploadSuccess: (objectUrl: string) => void;
};

const ProfilePictureModal = ({ onClose, onUploadSuccess }: Props) => {
    const [step, setStep] = useState<ModalStep>('view');
    const [previewUrl, setPreviewUrl] = useState<string | null>(null);
    const [selectedFile, setSelectedFile] = useState<File | null>(null);
    const [uploadUrls, setUploadUrls] = useState<{ preSignedUrl: string; objectUrl: string } | null>(null);
    const [error, setError] = useState<string | null>(null);

    const fileInputRef = useRef<HTMLInputElement>(null);
    const { getUploadUrl } = useGetProfilePictureUploadUrls();
    const { saveProfilePicture } = useSaveProfilePicture();

    const handleUploadClick = async () => {
        setError(null);
        const urls = await getUploadUrl();

        if (!urls) {
            setError('Unable to prepare upload. Please try again.');
            return;
        }

        setUploadUrls(urls);
        fileInputRef.current?.click();
    };

    const handleFileSelect = (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (!file) return;

        setSelectedFile(file);
        setPreviewUrl(URL.createObjectURL(file));
        setStep('preview');
    };

    const handleConfirm = async () => {
        if (!selectedFile || !uploadUrls) return;

        setStep('uploading');
        setError(null);

        try {
            const s3Response = await fetch(uploadUrls.preSignedUrl, {
                method: 'PUT',
                body: selectedFile,
            });

            if (!s3Response.ok) {
                setError('Upload failed. Please try again.');
                setStep('view');
                return;
            }
        } catch {
            setError('Upload failed. Please try again.');
            setStep('view');
            return;
        }

        const saved = await saveProfilePicture(uploadUrls.objectUrl);

        if (!saved) {
            setError('Photo uploaded but could not be saved. Please try again.');
            setStep('view');
            return;
        }

        onUploadSuccess(uploadUrls.objectUrl);
        onClose();
    };

    return createPortal(
        <div className={styles.overlay} onClick={onClose}>
            <div className={styles.modal} onClick={e => e.stopPropagation()}>

                {step === 'view' && (
                    <>
                        <h2>Profile Picture</h2>
                        <div className={styles.placeholder}>No profile picture set</div>
                        {error && <p className={styles.error}>{error}</p>}
                        <div className={styles.actions}>
                            <button onClick={handleUploadClick}>Upload New</button>
                            <button>Remove</button>
                        </div>
                        <button onClick={onClose}>Cancel</button>
                    </>
                )}

                {step === 'preview' && (
                    <>
                        <h2>Preview</h2>
                        <img src={previewUrl!} alt="Preview" className={styles.previewImage} />
                        <div className={styles.actions}>
                            <button onClick={handleConfirm}>Confirm</button>
                            <button onClick={() => setStep('view')}>Back</button>
                        </div>
                    </>
                )}

                {step === 'uploading' && (
                    <p>Uploading your photo...</p>
                )}

                <input
                    type="file"
                    accept="image/*"
                    ref={fileInputRef}
                    style={{ display: 'none' }}
                    onChange={handleFileSelect}
                />
            </div>
        </div>,
        document.body
    );
};

export default ProfilePictureModal;