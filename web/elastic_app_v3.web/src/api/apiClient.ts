import type {ProblemDetails} from "../errors/ProblemDetails";

type ApiResult =
    | { success: true; }
    | { success: false; error: string };

export const post = async <TRequest, TResponse>(
    url: string,
    request: TRequest
): Promise<ApiResult> => {
    try {
        const response = await fetch(url, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(request),
        });

        if (!response.ok) {
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
        
        return { success: true };

    } catch {
        return { success: false, error: 'Network error. Please try again.' };
    }
};
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