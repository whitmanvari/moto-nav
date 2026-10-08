import React, { useState } from 'react';
import {
    View,
    Text,
    StyleSheet,
    Modal,
    TextInput,
    TouchableOpacity,
    Alert,
    ActivityIndicator,
} from 'react-native';
import { groupRideService, GroupRideResponse } from '../../services/groupRide.service';

interface GroupRideModalProps {
    visible: boolean;
    onClose: () => void;
    onJoined: (group: { rideId: string; title: string; joinCode: string }) => void;
}

export const GroupRideModal: React.FC<GroupRideModalProps> = ({ visible, onClose, onJoined }) => {
    const [tab, setTab] = useState<'create' | 'join'>('create');
    const [groupTitle, setGroupTitle] = useState('');
    const [joinCode, setJoinCode] = useState('');
    const [loading, setLoading] = useState(false);

    const handleCreate = async () => {
        if (!groupTitle.trim()) {
            Alert.alert('Uyarı', 'Lütfen sürüş grubu için bir başlık girin.');
            return;
        }

        try {
            setLoading(true);
            const res: GroupRideResponse = await groupRideService.createGroupRide(groupTitle);
            Alert.alert('Grup Başlatıldı!', `Katılım Kodun: ${res.joinCode}`);
            onJoined({ rideId: res.id, title: res.title, joinCode: res.joinCode });
            setGroupTitle('');
            onClose();
        } catch {
            Alert.alert('Hata', 'Grup sürüşü oluşturulamadı.');
        } finally {
            setLoading(false);
        }
    };

    const handleJoin = async () => {
        if (!joinCode.trim()) {
            Alert.alert('Uyarı', 'Lütfen 4 haneli katılım kodunu girin.');
            return;
        }

        try {
            setLoading(true);
            const formattedCode = joinCode.startsWith('MOTO-') ? joinCode : `MOTO-${joinCode}`;
            await groupRideService.joinByCode(formattedCode);
            Alert.alert('Başarılı', `${formattedCode} grubuna katıldınız!`);
            onJoined({ rideId: formattedCode, title: 'Grup Sürüşü', joinCode: formattedCode });
            setJoinCode('');
            onClose();
        } catch {
            Alert.alert('Hata', 'Grup bulunamadı veya katılım başarısız.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <Modal visible={visible} transparent animationType="fade" onRequestClose={onClose}>
            <TouchableOpacity style={styles.backdrop} activeOpacity={1} onPress={onClose}>
                <View style={styles.card}>
                    <Text style={styles.title}>GRUP SÜRÜŞÜ (KONVOY)</Text>

                    {/* Sekmeler */}
                    <View style={styles.tabContainer}>
                        <TouchableOpacity
                            style={[styles.tabBtn, tab === 'create' && styles.tabBtnActive]}
                            onPress={() => setTab('create')}
                        >
                            <Text style={[styles.tabText, tab === 'create' && styles.tabTextActive]}>
                                Grup Kur
                            </Text>
                        </TouchableOpacity>
                        <TouchableOpacity
                            style={[styles.tabBtn, tab === 'join' && styles.tabBtnActive]}
                            onPress={() => setTab('join')}
                        >
                            <Text style={[styles.tabText, tab === 'join' && styles.tabTextActive]}>
                                Koda Katıl
                            </Text>
                        </TouchableOpacity>
                    </View>

                    {tab === 'create' ? (
                        <View style={styles.form}>
                            <Text style={styles.label}>Sürüş Başlığı</Text>
                            <TextInput
                                style={styles.input}
                                placeholder="Örn: Gece Sahil Turu"
                                placeholderTextColor="#64748B"
                                value={groupTitle}
                                onChangeText={setGroupTitle}
                            />
                            <TouchableOpacity
                                style={styles.actionBtn}
                                onPress={handleCreate}
                                disabled={loading}
                            >
                                {loading ? (
                                    <ActivityIndicator color="#090D16" />
                                ) : (
                                    <Text style={styles.actionBtnText}>KOD OLUŞTUR VE BAŞLAT</Text>
                                )}
                            </TouchableOpacity>
                        </View>
                    ) : (
                        <View style={styles.form}>
                            <Text style={styles.label}>Katılım Kodu</Text>
                            <TextInput
                                style={[styles.input, styles.codeInput]}
                                placeholder="MOTO-1234 veya 1234"
                                placeholderTextColor="#64748B"
                                autoCapitalize="characters"
                                value={joinCode}
                                onChangeText={setJoinCode}
                            />
                            <TouchableOpacity
                                style={styles.actionBtn}
                                onPress={handleJoin}
                                disabled={loading}
                            >
                                {loading ? (
                                    <ActivityIndicator color="#090D16" />
                                ) : (
                                    <Text style={styles.actionBtnText}>KONVOYA KATIL</Text>
                                )}
                            </TouchableOpacity>
                        </View>
                    )}

                    <TouchableOpacity style={styles.cancelBtn} onPress={onClose}>
                        <Text style={styles.cancelText}>Kapat</Text>
                    </TouchableOpacity>
                </View>
            </TouchableOpacity>
        </Modal>
    );
};

const styles = StyleSheet.create({
    backdrop: {
        flex: 1,
        backgroundColor: 'rgba(2, 6, 23, 0.8)',
        justifyContent: 'center',
        alignItems: 'center',
        padding: 24,
    },
    card: {
        width: '100%',
        backgroundColor: '#0F172A',
        borderRadius: 24,
        padding: 24,
        borderWidth: 1,
        borderColor: 'rgba(168, 85, 247, 0.35)',
        alignItems: 'center',
    },
    title: {
        color: '#F8FAFC',
        fontSize: 18,
        fontWeight: '900',
        letterSpacing: 1.5,
        marginBottom: 20,
    },
    tabContainer: {
        flexDirection: 'row',
        backgroundColor: '#1E293B',
        borderRadius: 12,
        padding: 4,
        width: '100%',
        marginBottom: 20,
    },
    tabBtn: {
        flex: 1,
        paddingVertical: 10,
        alignItems: 'center',
        borderRadius: 8,
    },
    tabBtnActive: {
        backgroundColor: '#A855F7',
    },
    tabText: {
        color: '#94A3B8',
        fontSize: 13,
        fontWeight: '700',
    },
    tabTextActive: {
        color: '#FFFFFF',
    },
    form: {
        width: '100%',
    },
    label: {
        color: '#94A3B8',
        fontSize: 12,
        fontWeight: '600',
        marginBottom: 8,
        textTransform: 'uppercase',
    },
    input: {
        backgroundColor: '#1E293B',
        borderRadius: 14,
        paddingHorizontal: 16,
        paddingVertical: 14,
        color: '#F8FAFC',
        fontSize: 15,
        borderWidth: 1,
        borderColor: 'rgba(255, 255, 255, 0.1)',
        marginBottom: 16,
    },
    codeInput: {
        letterSpacing: 2,
        fontWeight: '800',
        textAlign: 'center',
    },
    actionBtn: {
        backgroundColor: '#38BDF8',
        borderRadius: 14,
        paddingVertical: 16,
        alignItems: 'center',
        marginTop: 4,
    },
    actionBtnText: {
        color: '#090D16',
        fontWeight: '900',
        fontSize: 14,
        letterSpacing: 1,
    },
    cancelBtn: {
        marginTop: 18,
        paddingVertical: 8,
    },
    cancelText: {
        color: '#64748B',
        fontSize: 13,
        fontWeight: '700',
    },
});