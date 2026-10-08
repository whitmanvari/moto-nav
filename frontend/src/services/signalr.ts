import * as signalR from '@microsoft/signalr';
import { storageService } from './storage';

const HUB_URL = 'http://192.168.1.5:5000/hubs/ride';

export interface LocationUpdatePayload {
  rideId?: string;
  latitude: number;
  longitude: number;
  speed: number;
  heading: number | null;
}

export const navigationHub = new signalR.HubConnectionBuilder()
  .withUrl(HUB_URL, {
    accessTokenFactory: async () => {
      const token = await storageService.getToken();
      return token || '';
    },
    transport: signalR.HttpTransportType.WebSockets,
  })
  .withAutomaticReconnect()
  .configureLogging(signalR.LogLevel.Warning)
  .build();

export const startSignalRConnection = async () => {
  try {
    if (navigationHub.state === signalR.HubConnectionState.Disconnected) {
      await navigationHub.start();
      console.log('⚡ SignalR Hub bağlantısı başarılı.');

      // Şimdilik varsayılan bir genel sürüş odasına katılalım
      await joinRideGroup('general-ride');
    }
  } catch (error) {
    console.error('SignalR bağlantı hatası:', error);
  }
};

export const joinRideGroup = async (rideId: string) => {
  if (navigationHub.state === signalR.HubConnectionState.Connected) {
    try {
      await navigationHub.invoke('JoinRideGroup', rideId);
      console.log(`🏍️ ${rideId} sürüş grubuna katılındı.`);
    } catch (err) {
      console.error('Gruba katılma hatası:', err);
    }
  }
};

export const leaveRideGroup = async (rideId: string) => {
  if (navigationHub.state === signalR.HubConnectionState.Connected) {
    try {
      await navigationHub.invoke('LeaveRideGroup', rideId);
    } catch (err) {
      console.error('Gruptan ayrılma hatası:', err);
    }
  }
};

export const sendLocationUpdate = async (data: LocationUpdatePayload) => {
  if (navigationHub.state === signalR.HubConnectionState.Connected) {
    try {
      // Backend: SendLocationUpdate(string rideId, double latitude, double longitude, double speed, double heading)
      await navigationHub.invoke(
        'SendLocationUpdate',
        data.rideId || 'general-ride',
        data.latitude,
        data.longitude,
        data.speed,
        data.heading || 0
      );
    } catch (error) {
      console.error('Konum yayını gönderilemedi:', error);
    }
  }
};

export const stopSignalRConnection = async () => {
  try {
    if (navigationHub.state === signalR.HubConnectionState.Connected) {
      await leaveRideGroup('general-ride');
      await navigationHub.stop();
      console.log('SignalR bağlantısı durduruldu.');
    }
  } catch (error) {
    console.error('SignalR kapatma hatası:', error);
  }
};