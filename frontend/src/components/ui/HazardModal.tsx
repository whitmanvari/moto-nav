import React from 'react';
import { View, Text, StyleSheet, Modal, TouchableOpacity } from 'react-native';

export type HazardType = 'Pothole' | 'SlipperyRoad' | 'Wind' | 'Accident';

interface HazardModalProps {
    visible: boolean;
    onClose: () => void;
    onSelectHazard: (type: HazardType, label: string) => void;
}

const HAZARD_OPTIONS: { type: HazardType; label: string; icon: string; color: string }[] = [
    { type: 'Pothole', label: 'Çukur / Bozuk Yol', icon: '⚠️', color: '#F59E0B' },
    { type: 'SlipperyRoad', label: 'Kaygan Zemin / Yağ', icon: '❄️', color: '#38BDF8' },
    { type: 'Wind', label: 'Şiddetli Rüzgar', icon: '💨', color: '#A855F7' },
    { type: 'Accident', label: 'Kaza / Engel', icon: '🚨', color: '#EF4444' },
];

export const HazardModal: React.FC<HazardModalProps> = ({ visible, onClose, onSelectHazard }) => {
    return (
        <Modal visible={visible} transparent animationType="fade" onRequestClose={onClose}>
            <TouchableOpacity style={styles.backdrop} activeOpacity={1} onPress={onClose}>
                <View style={styles.card}>
                    <Text style={styles.title}>TEHLİKE BİLDİR</Text>
                    <Text style={styles.subtitle}>Yakındaki sürücüleri uyarmak için seçin</Text>

                    <View style={styles.grid}>
                        {HAZARD_OPTIONS.map((item) => (
                            <TouchableOpacity
                                key={item.type}
                                style={[styles.hazardBtn, { borderColor: item.color }]}
                                onPress={() => onSelectHazard(item.type, item.label)}
                            >
                                <Text style={styles.hazardIcon}>{item.icon}</Text>
                                <Text style={styles.hazardText}>{item.label}</Text>
                            </TouchableOpacity>
                        ))}
                    </View>

                    <TouchableOpacity style={styles.cancelBtn} onPress={onClose}>
                        <Text style={styles.cancelText}>İptal</Text>
                    </TouchableOpacity>
                </View>
            </TouchableOpacity>
        </Modal>
    );
};

const styles = StyleSheet.create({
    backdrop: {
        flex: 1,
        backgroundColor: 'rgba(2, 6, 23, 0.75)',
        justifyContent: 'center',
        alignItems: 'center',
        padding: 24,
    },
    card: {
        width: '100%',
        backgroundColor: '#0F172A',
        borderRadius: 24,
        padding: 20,
        borderWidth: 1,
        borderColor: 'rgba(56, 189, 248, 0.3)',
        alignItems: 'center',
    },
    title: {
        color: '#F8FAFC',
        fontSize: 18,
        fontWeight: '900',
        letterSpacing: 1.5,
    },
    subtitle: {
        color: '#94A3B8',
        fontSize: 12,
        marginTop: 4,
        marginBottom: 20,
    },
    grid: {
        width: '100%',
        flexDirection: 'row',
        flexWrap: 'wrap',
        justifyContent: 'space-between',
        gap: 12,
    },
    hazardBtn: {
        width: '48%',
        backgroundColor: '#1E293B',
        paddingVertical: 18,
        borderRadius: 16,
        alignItems: 'center',
        borderWidth: 1.5,
    },
    hazardIcon: {
        fontSize: 28,
        marginBottom: 6,
    },
    hazardText: {
        color: '#F8FAFC',
        fontSize: 12,
        fontWeight: '700',
        textAlign: 'center',
    },
    cancelBtn: {
        marginTop: 18,
        paddingVertical: 10,
        paddingHorizontal: 24,
    },
    cancelText: {
        color: '#64748B',
        fontSize: 13,
        fontWeight: '700',
    },
});