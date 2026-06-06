import { useState } from 'react';
import type { SignUpRequest } from '../dtos/SignUpRequest';
import {post} from "../api/apiClient";

type SignUpState = {
    isLoading: boolean;
    error: string | null;
};

export const useSignUp = () => {
    const [state, setState] = useState<SignUpState>({ isLoading: false, error: null });

    const signUp = async (request: SignUpRequest): Promise<boolean> => {
        setState({ isLoading: true, error: null });

        const result = await post<SignUpRequest, void>('http://localhost:8081/elastic-app/v1/user/signup', request);
        if (!result.success) {
            setState({ isLoading: false, error: result.error });
            return false;
        }

        setState({ isLoading: false, error: null });
        return true;
    };

    return { signUp, ...state };
};