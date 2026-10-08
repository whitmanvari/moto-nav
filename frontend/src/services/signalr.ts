import * as signalR from '@microsoft/signalr';

const HUB_URL = 'http://192.168.1.10:5000/hubs/navigation';

export const navigationHub = new signalR.HubConnectionBuilder()
  .withUrl(HUB_URL)
  .withAutomaticReconnect()
  .build();

export const startSignalRConnection = async () => {
  try {
    if (navigationHub.state === signalR.HubConnectionState.Disconnected) {
      await navigationHub.start();
      console.log('SignalR Hub bağlantısı başarılı.');
    }
  } catch (error) {
    console.error('SignalR bağlantı hatası:', error);
  }
};