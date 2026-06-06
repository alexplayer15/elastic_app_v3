import { useState } from 'react';
import type {LoginRequest} from "../dtos/LoginRequest";
import {post} from "../api/apiClient";

type LoginState = {
    isLoading: boolean;
    error: string | null;
};

export const useLogin = () => {
    const [state, setState] = useState<LoginState>({ isLoading: false, error: null });

    const login = async (request: LoginRequest): Promise<boolean> => {
        setState({ isLoading: true, error: null });

        const result = await post<LoginRequest, void>('http://localhost:8081/elastic-app/v1/user/login', request);
        if (!result.success) {
            setState({ isLoading: false, error: result.error });
            return false;
        }

        setState({ isLoading: false, error: null });
        return true;
    };

    return { login, ...state };
};