import React, { useRef, useEffect, useState } from 'react';
import { View, Text, StyleSheet, TouchableOpacity, SafeAreaView, Alert } from 'react-native';
import { WebView } from 'react-native-webview';
import { useRouter } from 'expo-router';
import { storageService } from '../../services/storage';
import { useSpeedometer } from '../../hooks/useSpeedometer';
import {
    startSignalRConnection,
    stopSignalRConnection,
    sendLocationUpdate,
    joinRideGroup,
    leaveRideGroup,
    onReceiveLocationUpdate,
    onUserLeftGroup,
    PeerRider,
} from '../../services/signalr';
import { hazardService, HazardItem } from '../../services/hazard.service';
import { HazardModal } from '../../components/ui/HazardModal';
import { GroupRideModal } from '../../components/ui/GroupRideModal';

export default function DashboardScreen() {
    const router = useRouter();
    const { speed, location, errorMsg } = useSpeedometer();
    const webViewRef = useRef<WebView>(null);

    // Tehlike Durumları
    const [hazards, setHazards] = useState<HazardItem[]>([]);
    const [isHazardModalVisible, setIsHazardModalVisible] = useState(false);
    const [hazardsLoaded, setHazardsLoaded] = useState(false);

    // Grup Sürüşü Durumları
    const [currentRideId, setCurrentRideId] = useState<string>('general-ride');
    const [groupJoinCode, setGroupJoinCode] = useState<string | null>(null);
    const [isGroupModalVisible, setIsGroupModalVisible] = useState(false);
    const [peerRiders, setPeerRiders] = useState<Record<string, PeerRider>>({});

    // 1. Ekran açıldığında SignalR başlat ve dinleyicileri bağla
    useEffect(() => {
        startSignalRConnection(currentRideId);

        onReceiveLocationUpdate((rider) => {
            setPeerRiders((prev) => ({
                ...prev,
                [rider.userId]: rider,
            }));
        });

        onUserLeftGroup((userId) => {
            setPeerRiders((prev) => {
                const updated = { ...prev };
                delete updated[userId];
                return updated;
            });
        });

        return () => {
            stopSignalRConnection();
        };
    }, []);

    // 2. GPS konumu ilk geldiğinde yakındaki tehlikeleri backend'den çek
    useEffect(() => {
        if (location && !hazardsLoaded) {
            loadNearbyHazards(location.latitude, location.longitude);
            setHazardsLoaded(true);
        }
    }, [location, hazardsLoaded]);

    const loadNearbyHazards = async (lat: number, lng: number) => {
        const list = await hazardService.getNearbyHazards(lat, lng);
        setHazards(list);
    };

    // 3. Canlı konumu haritaya gönder ve gruba SignalR ile ilet
    useEffect(() => {
        if (location) {
            const lat = location.latitude;
            const lng = location.longitude;
            const heading = location.heading || 0;

            webViewRef.current?.injectJavaScript(`
                if (window.updateMyLocation) {
                    window.updateMyLocation(${lat}, ${lng}, ${heading});
                }
                true;
            `);

            sendLocationUpdate({
                rideId: currentRideId,
                latitude: location.latitude,
                longitude: location.longitude,
                speed: speed,
                heading: location.heading,
            });
        }
    }, [location, speed, currentRideId]);

    // Tehlikeleri haritaya aktar
    useEffect(() => {
        if (hazards.length > 0) {
            webViewRef.current?.injectJavaScript(`
                if (window.updateHazards) {
                    window.updateHazards(${JSON.stringify(hazards)});
                }
                true;
            `);
        }
    }, [hazards]);

    // Diğer motorcuları haritaya aktar
    useEffect(() => {
        const list = Object.values(peerRiders);
        webViewRef.current?.injectJavaScript(`
            if (window.updatePeers) {
                window.updatePeers(${JSON.stringify(list)});
            }
            true;
        `);
    }, [peerRiders]);

    // Yeni Tehlike Bildir
    const handleReportHazard = async (type: string, label: string) => {
        if (!location) {
            Alert.alert('Hata', 'GPS konumu henüz alınamadı.');
            return;
        }

        setIsHazardModalVisible(false);

        try {
            await hazardService.reportHazard({
                condition: type,
                description: label,
                latitude: location.latitude,
                longitude: location.longitude,
                expiryHours: 6,
            });

            await loadNearbyHazards(location.latitude, location.longitude);
            Alert.alert('Başarılı', `${label} bildirildi!`);
        } catch {
            Alert.alert('Hata', 'Tehlike bildirimi sunucuya iletilemedi.');
        }
    };

    // Yeni Gruba Dahil Ol
    const handleGroupJoined = async (group: { rideId: string; title: string; joinCode: string }) => {
        await leaveRideGroup(currentRideId);
        setPeerRiders({});
        setCurrentRideId(group.rideId);
        setGroupJoinCode(group.joinCode);
        await joinRideGroup(group.rideId);
    };

    const handleLogout = async () => {
        await stopSignalRConnection();
        await storageService.removeToken();
        router.replace('/(auth)/login' as any);
    };

    const otherRidersList = Object.values(peerRiders);

    const initialLat = location?.latitude || 41.0082;
    const initialLng = location?.longitude || 28.9784;

    // Yüzde yüz ücretsiz ve API Keysiz OpenStreetMap + Karanlık CSS Filtresi
    const leafletHTML = `
      <!DOCTYPE html>
      <html>
      <head>
        <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no" />
        <link rel="stylesheet" href="https://unpkg.com/leaflet@1.9.4/dist/leaflet.css" />
        <script src="https://unpkg.com/leaflet@1.9.4/dist/leaflet.js"></script>
        <style>
          body, html, #map { margin: 0; padding: 0; width: 100%; height: 100%; background: #0b0f19; }
          .leaflet-control-attribution { display: none !important; }
          
          /* OpenStreetMap için Karanlık Gece Modu Filtresi */
          .leaflet-tile-pane {
            filter: brightness(0.6) invert(1) contrast(3) hue-rotate(200deg) saturate(0.3) brightness(0.7);
          }

          .my-marker {
            width: 22px; height: 22px; border-radius: 50%;
            background: #38bdf8; border: 3px solid #ffffff;
            box-shadow: 0 0 15px #38bdf8;
          }
          .hazard-icon { font-size: 24px; text-align: center; }
          .peer-icon { font-size: 22px; }
        </style>
      </head>
      <body>
        <div id="map"></div>
        <script>
          const map = L.map('map', { zoomControl: false }).setView([${initialLat}, ${initialLng}], 16);

          // Kesinlikle API KEY İSTEMEYEN Resmi OpenStreetMap Katmanı
          L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
            maxZoom: 19
          }).addTo(map);

          // Canlı Konum Noktası
          const myIcon = L.divIcon({ className: 'my-marker', iconSize: [22, 22], iconAnchor: [11, 11] });
          let myMarker = L.marker([${initialLat}, ${initialLng}], { icon: myIcon }).addTo(map);

          window.updateMyLocation = function(lat, lng, heading) {
            myMarker.setLatLng([lat, lng]);
            map.panTo([lat, lng], { animate: true });
          };

          // Tehlike Pinleri
          let hazardLayer = L.layerGroup().addTo(map);
          window.updateHazards = function(items) {
            hazardLayer.clearLayers();
            items.forEach(h => {
              const icon = L.divIcon({ className: 'hazard-icon', html: '⚠️', iconSize: [24, 24], iconAnchor: [12, 12] });
              L.marker([h.latitude, h.longitude], { icon: icon }).bindPopup(h.description || h.condition).addTo(hazardLayer);
            });
          };

          // Gruptaki Diğer Motorcular
          let peerLayer = L.layerGroup().addTo(map);
          window.updatePeers = function(peers) {
            peerLayer.clearLayers();
            peers.forEach(p => {
              const icon = L.divIcon({ className: 'peer-icon', html: '🏍️', iconSize: [24, 24], iconAnchor: [12, 12] });
              L.marker([p.latitude, p.longitude], { icon: icon }).addTo(peerLayer);
            });
          };
        </script>
      </body>
      </html>
    `;

    return (
        <View style={styles.container}>
            {/* Ücretsiz Karanlık Harita */}
            <WebView
                ref={webViewRef}
                originWhitelist={['*']}
                source={{ html: leafletHTML }}
                style={StyleSheet.absoluteFill}
                scrollEnabled={false}
            />

            {/* Üst Header */}
            <SafeAreaView style={styles.topOverlay}>
                <View style={styles.header}>
                    <View>
                        <Text style={styles.brandTitle}>MOTO-NAV</Text>
                        <Text style={styles.statusText}>
                            {errorMsg
                                ? `⚠️ ${errorMsg}`
                                : groupJoinCode
                                    ? `● Konvoy: ${groupJoinCode}`
                                    : '● Genel Sürüş Aktif'}
                        </Text>
                    </View>

                    <View style={styles.topActions}>
                        <TouchableOpacity
                            onPress={() => setIsGroupModalVisible(true)}
                            style={styles.groupBtn}
                        >
                            <Text style={styles.groupBtnText}>👥 Konvoy</Text>
                        </TouchableOpacity>

                        <TouchableOpacity onPress={handleLogout} style={styles.logoutBtn}>
                            <Text style={styles.logoutText}>Çıkış</Text>
                        </TouchableOpacity>
                    </View>
                </View>
            </SafeAreaView>

            {/* Sağ Yüzen Tehlike Butonu */}
            <TouchableOpacity
                style={styles.floatingHazardBtn}
                onPress={() => setIsHazardModalVisible(true)}
            >
                <Text style={styles.floatingHazardIcon}>⚠️</Text>
            </TouchableOpacity>

            {/* Alt Kokpit HUD */}
            <View style={styles.bottomHud}>
                <View style={styles.hudCard}>
                    <View style={styles.speedSection}>
                        <Text style={styles.hudLabel}>HIZ</Text>
                        <View style={styles.speedRow}>
                            <Text style={styles.speedValue}>{speed}</Text>
                            <Text style={styles.unitText}>KM/S</Text>
                        </View>
                    </View>

                    <View style={styles.divider} />

                    <View style={styles.infoSection}>
                        <Text style={styles.hudLabel}>GRUP SÜRÜŞÜ</Text>
                        <Text style={styles.statusValue}>
                            {otherRidersList.length > 0
                                ? `${otherRidersList.length + 1} Sürücü Canlı`
                                : 'Yalnız Sürüş'}
                        </Text>
                        <Text style={styles.subInfo}>
                            {groupJoinCode ? `Oda: ${groupJoinCode}` : 'Genel Oda'}
                        </Text>
                    </View>
                </View>
            </View>

            {/* Tehlike Modalı */}
            <HazardModal
                visible={isHazardModalVisible}
                onClose={() => setIsHazardModalVisible(false)}
                onSelectHazard={handleReportHazard}
            />

            {/* Grup Sürüşü Modalı */}
            <GroupRideModal
                visible={isGroupModalVisible}
                onClose={() => setIsGroupModalVisible(false)}
                onJoined={handleGroupJoined}
            />
        </View>
    );
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
        backgroundColor: '#090D16',
    },
    topOverlay: {
        position: 'absolute',
        top: 0,
        left: 0,
        right: 0,
        zIndex: 10,
    },
    header: {
        flexDirection: 'row',
        justifyContent: 'space-between',
        alignItems: 'center',
        marginHorizontal: 20,
        marginTop: 10,
        padding: 16,
        borderRadius: 16,
        backgroundColor: 'rgba(15, 23, 42, 0.85)',
        borderWidth: 1,
        borderColor: 'rgba(56, 189, 248, 0.2)',
    },
    brandTitle: {
        color: '#38BDF8',
        fontSize: 22,
        fontWeight: '900',
        letterSpacing: 2,
    },
    statusText: {
        color: '#10B981',
        fontSize: 11,
        fontWeight: '600',
        marginTop: 2,
    },
    topActions: {
        flexDirection: 'row',
        alignItems: 'center',
        gap: 8,
    },
    groupBtn: {
        backgroundColor: '#7C3AED',
        paddingHorizontal: 12,
        paddingVertical: 6,
        borderRadius: 8,
    },
    groupBtnText: {
        color: '#FFFFFF',
        fontSize: 12,
        fontWeight: '700',
    },
    logoutBtn: {
        backgroundColor: '#1E293B',
        paddingHorizontal: 12,
        paddingVertical: 6,
        borderRadius: 8,
    },
    logoutText: {
        color: '#EF4444',
        fontSize: 12,
        fontWeight: '700',
    },
    floatingHazardBtn: {
        position: 'absolute',
        right: 20,
        bottom: 125,
        backgroundColor: '#EF4444',
        width: 54,
        height: 54,
        borderRadius: 27,
        justifyContent: 'center',
        alignItems: 'center',
        borderWidth: 2,
        borderColor: 'rgba(255, 255, 255, 0.3)',
        shadowColor: '#EF4444',
        shadowOpacity: 0.5,
        shadowRadius: 8,
        elevation: 8,
        zIndex: 10,
    },
    floatingHazardIcon: {
        fontSize: 24,
    },
    bottomHud: {
        position: 'absolute',
        bottom: 34,
        left: 20,
        right: 20,
        zIndex: 10,
    },
    hudCard: {
        flexDirection: 'row',
        alignItems: 'center',
        backgroundColor: 'rgba(15, 23, 42, 0.92)',
        borderRadius: 24,
        paddingVertical: 18,
        paddingHorizontal: 24,
        borderWidth: 1,
        borderColor: 'rgba(56, 189, 248, 0.25)',
    },
    speedSection: {
        flex: 1,
        alignItems: 'center',
    },
    hudLabel: {
        color: '#64748B',
        fontSize: 11,
        fontWeight: '800',
        letterSpacing: 1.5,
        marginBottom: 4,
    },
    speedRow: {
        flexDirection: 'row',
        alignItems: 'baseline',
    },
    speedValue: {
        color: '#F8FAFC',
        fontSize: 48,
        fontWeight: '900',
    },
    unitText: {
        color: '#38BDF8',
        fontSize: 13,
        fontWeight: '800',
        marginLeft: 6,
    },
    divider: {
        width: 1,
        height: '70%',
        backgroundColor: '#1E293B',
        marginHorizontal: 16,
    },
    infoSection: {
        flex: 1,
        alignItems: 'flex-start',
    },
    statusValue: {
        color: '#F8FAFC',
        fontSize: 18,
        fontWeight: '700',
    },
    subInfo: {
        color: '#94A3B8',
        fontSize: 12,
        marginTop: 2,
    },
});