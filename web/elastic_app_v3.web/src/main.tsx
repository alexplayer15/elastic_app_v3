import React from 'react';
import './identityStyle.css';
import { createRoot } from 'react-dom/client';
import { BrowserRouter } from 'react-router-dom';
import App from './App';

const root = document.getElementById('root');
createRoot(root!).render(
    <BrowserRouter>
        <App />
    </BrowserRouter>
);