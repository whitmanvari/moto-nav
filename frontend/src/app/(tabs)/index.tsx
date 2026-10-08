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
} from '../../services/signalr';
import { hazardService, HazardItem, HazardType } from '../../services/hazard.service';
import { HazardModal } from '../../components/ui/HazardModal';

export default function DashboardScreen() {
    const router = useRouter();
    const { speed, location, errorMsg } = useSpeedometer();
    const mapRef = useRef<MapView>(null);

    // Tehlike Durumları
    const [hazards, setHazards] = useState<HazardItem[]>([]);
    const [isHazardModalVisible, setIsHazardModalVisible] = useState(false);

    // 1. Ekran açıldığında SignalR başlat ve aktif tehlikeleri çek
    useEffect(() => {
        startSignalRConnection();
        loadHazards();

        return () => {
            stopSignalRConnection();
        };
    }, []);

    const loadHazards = async () => {
        const list = await hazardService.getActiveHazards();
        setHazards(list);
    };

    // 2. Harita kamerasını takip et ve canlı GPS telemetrisini gönder
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
                rideId: 'general-ride',
                latitude: location.latitude,
                longitude: location.longitude,
                speed: speed,
                heading: location.heading,
            });
        }
    }, [location, speed]);

    // Yeni Tehlike Bildir
    const handleReportHazard = async (type: HazardType, label: string) => {
        if (!location) {
            Alert.alert('Hata', 'Konum henüz alınamadı.');
            return;
        }

        setIsHazardModalVisible(false);

        try {
            const newHazard = await hazardService.reportHazard({
                type,
                title: label,
                latitude: location.latitude,
                longitude: location.longitude,
            });

            setHazards((prev) => [newHazard, ...prev]);
            Alert.alert('Başarılı', `${label} bildirildi!`);
        } catch {
            Alert.alert('Bilgi', 'Tehlike bildirimi gönderilemedi.');
        }
    };

    const handleLogout = async () => {
        await stopSignalRConnection();
        await storageService.removeToken();
        router.replace('/(auth)/login' as any);
    };

    return (
        <View style={styles.container}>
            {/* Canlı Karanlık Harita Katmanı */}
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
                        <View style={styles.markerContainer}>
                            <View style={styles.markerHalo} />
                            <View style={styles.markerCore} />
                        </View>
                    </Marker>
                )}

                {/* Haritadaki Tehlike Pinleri */}
                {hazards.map((item) => (
                    <Marker
                        key={item.id}
                        coordinate={{ latitude: item.latitude, longitude: item.longitude }}
                        title={item.title}
                    >
                        <View style={styles.hazardPin}>
                            <Text style={styles.hazardPinText}>⚠️</Text>
                        </View>
                    </Marker>
                ))}
            </MapView>

            {/* Üst Header (Floating Bar) */}
            <SafeAreaView style={styles.topOverlay}>
                <View style={styles.header}>
                    <View>
                        <Text style={styles.brandTitle}>MOTO-NAV</Text>
                        <Text style={styles.statusText}>
                            {errorMsg ? `⚠️ ${errorMsg}` : '● Canlı Navigasyon Aktif'}
                        </Text>
                    </View>
                    <TouchableOpacity onPress={handleLogout} style={styles.logoutBtn}>
                        <Text style={styles.logoutText}>Çıkış</Text>
                    </TouchableOpacity>
                </View>
            </SafeAreaView>

            {/* Sağ Yüzen Hızlı Tehlike Bildir Butonu */}
            <TouchableOpacity
                style={styles.floatingHazardBtn}
                onPress={() => setIsHazardModalVisible(true)}
            >
                <Text style={styles.floatingHazardIcon}>⚠️</Text>
            </TouchableOpacity>

            {/* Alt Kokpit Paneli (HUD Hız Kartı) */}
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
                        <Text style={styles.hudLabel}>DURUM</Text>
                        <Text style={styles.statusValue}>{speed > 0 ? 'Sürüşte' : 'Hazır'}</Text>
                        <Text style={styles.subInfo}>
                            {hazards.length > 0 ? `${hazards.length} aktif tehlike bildirimi` : 'Tehlike bildirimi yok'}
                        </Text>
                    </View>
                </View>
            </View>

            {/* Tehlike Seçim Penceresi */}
            <HazardModal
                visible={isHazardModalVisible}
                onClose={() => setIsHazardModalVisible(false)}
                onSelectHazard={handleReportHazard}
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
    markerContainer: {
        alignItems: 'center',
        justifyContent: 'center',
        width: 40,
        height: 40,
    },
    markerHalo: {
        position: 'absolute',
        width: 32,
        height: 32,
        borderRadius: 16,
        backgroundColor: 'rgba(56, 189, 248, 0.3)',
    },
    markerCore: {
        width: 16,
        height: 16,
        borderRadius: 8,
        backgroundColor: '#38BDF8',
        borderWidth: 2,
        borderColor: '#FFFFFF',
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