import { get } from '../api/apiClient';
import { useState } from 'react';

type ProfilePictureUrls = {
    preSignedUrl: string;
    objectUrl: string;
};

type State = {
    isLoading: boolean;
    error: string | null;
};

export const useGetProfilePictureUploadUrls = () => {
    const [state, setState] = useState<State>({ isLoading: false, error: null });

    const getUploadUrl = async (): Promise<ProfilePictureUrls | null> => {
        setState({ isLoading: true, error: null });

        const result = await get<ProfilePictureUrls>(
            'http://localhost:8081/elastic-app/v1/profile/picture-urls'
        );

        if (!result.success) {
            setState({ isLoading: false, error: result.error });
            return null;
        }

        setState({ isLoading: false, error: null });
        return result.body;
    };

    return { getUploadUrl, ...state };
};