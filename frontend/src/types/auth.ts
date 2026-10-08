export interface LoginRequestDto {
    email: string;
    password: string;
}

export interface RegisterRequestDto {
    username: string;
    email: string;
    password: string;
    firstName?: string;
    lastName?: string;
    motorcycleModel?: string;
}

export interface AuthResponseDto {
    token: string;
    refreshToken?: string;
    expiration: string;
    userId: string;
    email: string;
    username: string;
}

export interface UserProfile {
    id: string;
    email: string;
    username: string;
    motorcycleModel?: string;
}