export interface LoginResponse{
    accessToken: string,
    refreshToken: string,
    tokenType: string,
    expiresInMinutes?: number,
}