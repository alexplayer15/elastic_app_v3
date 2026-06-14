import type {ProblemDetails} from "../errors/ProblemDetails";

type ApiResult<TResponse> = 
    | { success: true; body: TResponse | null }
    | { success: false; error: string };

export const post = async <TRequest, TResponse>(
    url: string,
    request: TRequest
): Promise<ApiResult<TResponse>> => {
    try {
        const response = await fetch(url, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(request),
            credentials: 'include'
        });

        if (!response.ok) {
            return await handleErrorResponse(response);
        }
        
        return { success: true, body: null };

    } catch {
        return { success: false, error: 'Network error. Please try again.' };
    }
};

export const get = async <TResponse>(
    url: string
): Promise<ApiResult<TResponse>> => {
    try {
        const response = await fetch(url, {
            method: 'GET',
            credentials: 'include',
        });

        if (!response.ok) {
            return await handleErrorResponse(response);
        }

        const data = await response.json() as TResponse;
        
        return { success: true, body: data };

    } catch {
        return { success: false, error: 'Network error. Please try again.' };
    }
};

export const patch = async <TRequest, TResponse>(
    url: string,
    request: TRequest
): Promise<ApiResult<TResponse>> => {
    try {
        const response = await fetch(url, {
            method: 'PATCH',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(request),
            credentials: 'include',
        });

        if (!response.ok) {
            return await handleErrorResponse(response);
        }

        return { success: true, body: null };

    } catch {
        return { success: false, error: 'Network error. Please try again.' };
    }
};

async function handleErrorResponse<TResponse>(response: Response) : Promise<ApiResult<TResponse>> {
    let problem: ProblemDetails | null = null;
    try {
        const json = await response.json();
        problem = isProblemDetails(json) ? json : null;
    } catch {
        problem = null;
    }

    return {
        success: false,
        error: problem
            ? `${problem.title}: ${problem.detail}`
            : `Unexpected error: ${response.status}`,
    };
}
function isProblemDetails(json: unknown): json is ProblemDetails {
    return (
        typeof json === 'object' &&
        json !== null &&
        'title' in json &&
        'detail' in json &&
        'status' in json &&
        'type' in json
    );
}