import api from './api';

export interface HazardItem {
    id: string;
    reporterUserId?: string;
    condition: string;
    latitude: number;
    longitude: number;
    description?: string;
    expiresAt: string;
}

export interface CreateHazardDto {
    condition: string; // "Pothole", "SlipperyRoad", "Wind", "Accident" vb.
    latitude: number;
    longitude: number;
    description?: string;
    expiryHours?: number;
}

export const hazardService = {
    // Koordinat bazlı yakındaki (varsayılan 15km) tehlikeleri çek
    async getNearbyHazards(latitude: number, longitude: number, radiusMeters: number = 15000): Promise<HazardItem[]> {
        try {
            const response = await api.get<HazardItem[]>('/roadconditions/nearby', {
                params: {
                    latitude,
                    longitude,
                    radiusMeters,
                },
            });
            return response.data;
        } catch (error) {
            console.warn('Yakındaki tehlikeler yüklenemedi:', error);
            return [];
        }
    },

    // Yeni yol tehlikesi bildir
    async reportHazard(dto: CreateHazardDto): Promise<any> {
        const response = await api.post('/roadconditions', {
            condition: dto.condition,
            latitude: dto.latitude,
            longitude: dto.longitude,
            description: dto.description || 'Sürücü bildirimi',
            expiryHours: dto.expiryHours || 6,
        });
        return response.data;
    },
};