export interface LoginRequest {
    email: string,
    password: string
}

export interface JwtTokenResponse {
    accessToken: string,
    accessTokenExpiresAt: Date
}