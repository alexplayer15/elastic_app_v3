import { patch } from '../api/apiClient';
import { useState } from 'react';

type State = {
    isLoading: boolean;
    error: string | null;
};

export const useSaveProfilePicture = () => {
    const [state, setState] = useState<State>({ isLoading: false, error: null });

    const saveProfilePicture = async (objectUrl: string): Promise<boolean> => {
        setState({ isLoading: true, error: null });

        const result = await patch<{ objectUrl: string }, void>(
            'http://localhost:8081/elastic-app/v1/profile/picture',
            { objectUrl }
        );

        if (!result.success) {
            setState({ isLoading: false, error: result.error });
            return false;
        }

        setState({ isLoading: false, error: null });
        return true;
    };

    return { saveProfilePicture, ...state };
};