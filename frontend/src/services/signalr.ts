import * as signalR from '@microsoft/signalr';
import { storageService } from './storage';

const HUB_URL = 'http://172.20.10.3:5000/hubs/ride';

export interface LocationUpdatePayload {
  rideId?: string;
  latitude: number;
  longitude: number;
  speed: number;
  heading: number | null;
}

export interface PeerRider {
  userId: string;
  latitude: number;
  longitude: number;
  speed: number;
  heading: number;
  timestamp: string;
}

export const navigationHub = new signalR.HubConnectionBuilder()
  .withUrl(HUB_URL, {
    accessTokenFactory: async () => {
      const token = await storageService.getToken();
      return token || '';
    },
    transport: signalR.HttpTransportType.WebSockets,
    skipNegotiation: false,
  })
  .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
  .configureLogging(signalR.LogLevel.Warning)
  .build();

// İstemci timeout toleransı
navigationHub.serverTimeoutInMilliseconds = 120000;
navigationHub.keepAliveIntervalInMilliseconds = 15000;

export const startSignalRConnection = async (defaultRideId: string = 'general-ride') => {
  try {
    if (navigationHub.state === signalR.HubConnectionState.Disconnected) {
      await navigationHub.start();
      console.log('⚡ SignalR Hub bağlantısı başarılı.');
      await joinRideGroup(defaultRideId);
    }
  } catch (error) {
    console.error('SignalR bağlantı hatası:', error);
  }
};

export const joinRideGroup = async (rideId: string) => {
  if (navigationHub.state === signalR.HubConnectionState.Connected && rideId) {
    try {
      await navigationHub.invoke('JoinRideGroup', rideId);
      console.log(`🏍️ ${rideId} sürüş grubuna katılındı.`);
    } catch (err) {
      console.error('Gruba katılma hatası:', err);
    }
  }
};

export const leaveRideGroup = async (rideId: string) => {
  if (navigationHub.state === signalR.HubConnectionState.Connected && rideId) {
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

export const onReceiveLocationUpdate = (callback: (rider: PeerRider) => void) => {
  navigationHub.off('ReceiveLocationUpdate');
  navigationHub.on('ReceiveLocationUpdate', callback);
};

export const onUserLeftGroup = (callback: (userId: string) => void) => {
  navigationHub.off('UserLeft');
  navigationHub.on('UserLeft', callback);
};

export const stopSignalRConnection = async () => {
  try {
    if (navigationHub.state === signalR.HubConnectionState.Connected) {
      navigationHub.off('ReceiveLocationUpdate');
      navigationHub.off('UserLeft');
      await leaveRideGroup('general-ride');
      await navigationHub.stop();
      console.log('SignalR bağlantısı durduruldu.');
    }
  } catch (error) {
    console.error('SignalR kapatma hatası:', error);
  }
};