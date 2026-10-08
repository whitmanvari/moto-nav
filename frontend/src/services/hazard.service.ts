import api from './api';

export type HazardType = 'Pothole' | 'SlipperyRoad' | 'Wind' | 'Accident' | 'Police';

export interface HazardItem {
    id: string;
    type: HazardType;
    title: string;
    description?: string;
    latitude: number;
    longitude: number;
    createdAt: string;
}

export interface CreateHazardDto {
    type: HazardType;
    title: string;
    description?: string;
    latitude: number;
    longitude: number;
}

export const hazardService = {
    // Yakındaki veya tüm aktif tehlikeleri çek
    async getActiveHazards(): Promise<HazardItem[]> {
        try {
            const response = await api.get<HazardItem[]>('/roadconditions');
            return response.data;
        } catch (error) {
            console.warn('Tehlikeler yüklenemedi (offline fallback kullanılabilir):', error);
            return [];
        }
    },

    // Tek dokunuşla tehlike bildir
    async reportHazard(dto: CreateHazardDto): Promise<HazardItem> {
        const response = await api.post<HazardItem>('/roadconditions', dto);
        return response.data;
    },
};