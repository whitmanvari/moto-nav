import api from './api';
import { AuthResponseDto, LoginRequestDto, RegisterRequestDto, UserProfile } from '../types/auth';

export const authService = {
    async login(payload: LoginRequestDto): Promise<AuthResponseDto> {
        const response = await api.post<AuthResponseDto>('/Auth/login', payload);
        return response.data;
    },

    async register(payload: RegisterRequestDto): Promise<AuthResponseDto> {
        const response = await api.post<AuthResponseDto>('/Auth/register', payload);
        return response.data;
    },

    async getMe(): Promise<UserProfile> {
        const response = await api.get<UserProfile>('/Auth/me');
        return response.data;
    },

    async refreshToken(token: string): Promise<AuthResponseDto> {
        const response = await api.post<AuthResponseDto>('/Auth/refresh-token', { token });
        return response.data;
    },
};