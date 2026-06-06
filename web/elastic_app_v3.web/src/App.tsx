import React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import { routes } from './routes/routes';

const App = () => (
    <Routes>
        {routes.map(({ path, element: Element }) => (
            <Route key={path} path={path} element={<Element />} />
        ))}
        <Route path="/" element={<Navigate to="/signup" replace />} />
    </Routes>
);

export default App;