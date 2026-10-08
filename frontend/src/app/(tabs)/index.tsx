import React, { useRef, useEffect, useState } from 'react';
import { View, Text, StyleSheet, TouchableOpacity, SafeAreaView, Alert } from 'react-native';
import MapView, { Marker, PROVIDER_DEFAULT } from 'react-native-maps';
import { useRouter } from 'expo-router';
import { storageService } from '../../services/storage';
import { useSpeedometer } from '../../hooks/useSpeedometer';
import { darkMapStyle } from '../../constants/darkMapStyle';
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
    const mapRef = useRef<MapView>(null);

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

    // 3. Harita kamerasını takip et ve canlı GPS telemetrisini o anki gruba gönder
    useEffect(() => {
        if (location) {
            if (mapRef.current) {
                mapRef.current.animateToRegion(
                    {
                        latitude: location.latitude,
                        longitude: location.longitude,
                        latitudeDelta: 0.005,
                        longitudeDelta: 0.005,
                    },
                    500
                );
            }

            sendLocationUpdate({
                rideId: currentRideId,
                latitude: location.latitude,
                longitude: location.longitude,
                speed: speed,
                heading: location.heading,
            });
        }
    }, [location, speed, currentRideId]);

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

    return (
        <View style={styles.container}>
            {/* Harita */}
            <MapView
                ref={mapRef}
                style={StyleSheet.absoluteFill}
                provider={PROVIDER_DEFAULT}
                customMapStyle={darkMapStyle}
                userInterfaceStyle="dark"
                showsCompass={false}
                initialRegion={{
                    latitude: location?.latitude || 41.0082,
                    longitude: location?.longitude || 28.9784,
                    latitudeDelta: 0.01,
                    longitudeDelta: 0.01,
                }}
            >
                {/* Kendi Motor Konumumuz */}
                {location && (
                    <Marker
                        coordinate={{
                            latitude: location.latitude,
                            longitude: location.longitude,
                        }}
                        anchor={{ x: 0.5, y: 0.5 }}
                        flat
                        rotation={location.heading || 0}
                    >
                        <View style={styles.myMarkerContainer}>
                            <View style={styles.myMarkerHalo} />
                            <View style={styles.myMarkerCore} />
                        </View>
                    </Marker>
                )}

                {/* Gruptaki Diğer Motorcular */}
                {otherRidersList.map((rider) => (
                    <Marker
                        key={`rider-${rider.userId}`}
                        coordinate={{
                            latitude: Number(rider.latitude),
                            longitude: Number(rider.longitude),
                        }}
                        anchor={{ x: 0.5, y: 0.5 }}
                        flat
                        rotation={rider.heading || 0}
                        title={`Sürücü: ${rider.userId?.substring(0, 6) || 'Motorcu'}`}
                        description={`${Math.round(rider.speed)} km/s`}
                    >
                        <View style={styles.peerMarkerContainer}>
                            <View style={styles.peerMarkerHalo} />
                            <Text style={styles.peerMarkerIcon}>🏍️</Text>
                        </View>
                    </Marker>
                ))}

                {/* Tehlike Pinleri */}
                {hazards.map((item, index) => (
                    <Marker
                        key={item.id ? `hazard-${item.id}` : `hazard-${index}-${item.latitude}`}
                        coordinate={{
                            latitude: Number(item.latitude),
                            longitude: Number(item.longitude),
                        }}
                        title={item.condition}
                        description={item.description}
                    >
                        <View style={styles.hazardPin}>
                            <Text style={styles.hazardPinText}>⚠️</Text>
                        </View>
                    </Marker>
                ))}
            </MapView>

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
    myMarkerContainer: {
        alignItems: 'center',
        justifyContent: 'center',
        width: 40,
        height: 40,
    },
    myMarkerHalo: {
        position: 'absolute',
        width: 32,
        height: 32,
        borderRadius: 16,
        backgroundColor: 'rgba(56, 189, 248, 0.3)',
    },
    myMarkerCore: {
        width: 16,
        height: 16,
        borderRadius: 8,
        backgroundColor: '#38BDF8',
        borderWidth: 2,
        borderColor: '#FFFFFF',
    },
    peerMarkerContainer: {
        alignItems: 'center',
        justifyContent: 'center',
        width: 38,
        height: 38,
    },
    peerMarkerHalo: {
        position: 'absolute',
        width: 34,
        height: 34,
        borderRadius: 17,
        backgroundColor: 'rgba(168, 85, 247, 0.35)',
        borderWidth: 1.5,
        borderColor: '#C084FC',
    },
    peerMarkerIcon: {
        fontSize: 18,
    },
    hazardPin: {
        backgroundColor: '#EF4444',
        padding: 6,
        borderRadius: 20,
        borderWidth: 2,
        borderColor: '#FFFFFF',
    },
    hazardPinText: {
        fontSize: 14,
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