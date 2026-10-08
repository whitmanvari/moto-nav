import api from './api';

export interface GroupRideResponse {
    id: string;
    leaderUserId: string;
    title: string;
    joinCode: string;
    isActive: boolean;
    memberCount: number;
    memberUserIds: string[];
}

export interface MemberLiveLocation {
    userId: string;
    latitude: number;
    longitude: number;
    currentSpeedKmh: number;
    isLaggingBehind: boolean;
    timestamp: string;
}

export const groupRideService = {
    // Yeni grup oluştur ve MOTO-XXXX kodu al
    async createGroupRide(title: string): Promise<GroupRideResponse> {
        const response = await api.post<GroupRideResponse>('/grouprides', {
            title,
            scheduledStartTime: new Date().toISOString(),
        });
        return response.data;
    },

    // Katılım kodu ile gruba dahil ol
    async joinByCode(joinCode: string): Promise<void> {
        await api.post('/grouprides/join', null, {
            params: { joinCode: joinCode.trim().toUpperCase() },
        });
    },

    // Gruptaki üyelerin anlık konumlarını çek
    async getLiveLocations(rideId: string): Promise<MemberLiveLocation[]> {
        const response = await api.get<MemberLiveLocation[]>(`/grouprides/${rideId}/locations`);
        return response.data;
    },
};