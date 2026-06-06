import React, { useState } from 'react';
import { useSignUp } from '../hooks/useSignUp';
import type { SignUpRequest } from '../dtos/SignUpRequest';
import { useNavigate } from 'react-router-dom';
import styles from './IdentityForm.module.css';

const SignUpForm = () => {
    const [firstName, setFirstName] = useState('');
    const [lastName, setLastName] = useState('');
    const [userName, setUserName] = useState('');
    const [password, setPassword] = useState('');
    const [reEnteredPassword, setReEnteredPassword] = useState('');
    const { signUp, isLoading, error } = useSignUp();
    const navigate = useNavigate();

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        const request: SignUpRequest = { firstName, lastName, userName, password, reEnteredPassword };
        const success = await signUp(request);

        if (success) {
            navigate('/login');
        }
    };

    return (
        <div className={styles.pageWrapper}>
            <div className={styles.container}>
                <h1>Create an Account</h1>
                <form onSubmit={handleSubmit}>
                    <div className={styles.formGroup}>
                        <label>First Name</label>
                        <input value={firstName} onChange={e => setFirstName(e.target.value)} required />
                    </div>
                    <div className={styles.formGroup}>
                        <label>Last Name</label>
                        <input value={lastName} onChange={e => setLastName(e.target.value)} required />
                    </div>
                    <div className={styles.formGroup}>
                        <label>Username</label>
                        <input value={userName} onChange={e => setUserName(e.target.value)} required />
                    </div>
                    <div className={styles.formGroup}>
                        <label>Password</label>
                        <input value={password} onChange={e => setPassword(e.target.value)} type="password" required />
                    </div>
                    <div className={styles.formGroup}>
                        <label>Re-enter Password</label>
                        <input value={reEnteredPassword} onChange={e => setReEnteredPassword(e.target.value)} type="password" required />
                    </div>
                    <button className={styles.submitButton} type="submit" disabled={isLoading}>
                        {isLoading ? 'Signing up...' : 'Sign Up'}
                    </button>
                    {error && <p className={styles.errorMessage}>{error}</p>}
                </form>
            </div>
        </div>
    );
};

export default SignUpForm;