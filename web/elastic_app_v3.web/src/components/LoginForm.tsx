import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useLogin } from '../hooks/useLogin';
import type { LoginRequest } from '../dtos/LoginRequest';
import styles from './IdentityForm.module.css';
import { paths } from '../routes/paths';

const LoginForm = () => {
    const [userName, setUserName] = useState('');
    const [password, setPassword] = useState('');
    const { login, isLoading, error } = useLogin();
    const navigate = useNavigate();

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        const request: LoginRequest = { userName, password };
        const success = await login(request);

        if (success) {
            navigate(paths.home);
        }
    };

    return (
        <div className={styles.pageWrapper}>
            <div className={styles.container}>
                <h1>Login To Your Account</h1>
                <form onSubmit={handleSubmit}>
                    <div className={styles.formGroup}>
                        <label>Username</label>
                        <input value={userName} onChange={e => setUserName(e.target.value)} type="text" required />
                    </div>
                    <div className={styles.formGroup}>
                        <label>Password</label>
                        <input value={password} onChange={e => setPassword(e.target.value)} type="password" required />
                    </div>
                    <button className={styles.submitButton} type="submit" disabled={isLoading}>
                        {isLoading ? 'Logging in...' : 'Login'}
                    </button>
                    {error && <p className={styles.errorMessage}>{error}</p>}
                </form>
            </div>
        </div>
    );
};

export default LoginForm;