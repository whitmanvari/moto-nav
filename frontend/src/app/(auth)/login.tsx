import React, { useState } from 'react';
import {
    View,
    Text,
    StyleSheet,
    TouchableOpacity,
    Alert,
    KeyboardAvoidingView,
    Platform,
    ScrollView,
} from 'react-native';
import { useRouter } from 'expo-router';
import { DarkInput } from '../../components/ui/DarkInput';
import { DarkButton } from '../../components/ui/DarkButton';
import { authService } from '../../services/auth.service';
import { storageService } from '../../services/storage';

export default function LoginScreen() {
    const router = useRouter();
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [loading, setLoading] = useState(false);

    const handleLogin = async () => {
        if (!email || !password) {
            Alert.alert('Eksik Bilgi', 'Lütfen e-posta ve şifrenizi girin.');
            return;
        }

        setLoading(true);
        try {
            const result = await authService.login({
                email: email.trim(),
                password: password.trim(),
            });

            await storageService.setToken(result.token);
            Alert.alert('Giriş Başarılı', `Hoş geldin, ${result.username || 'Sürücü'}!`);
            // Ana gösterge paneline yönlendirme
            router.replace('/(tabs)' as any);
        } catch (error: any) {
            const message = error.response?.data?.message || 'E-posta veya şifre hatalı.';
            Alert.alert('Giriş Başarısız', message);
        } finally {
            setLoading(false);
        }
    };

    return (
        <KeyboardAvoidingView
            behavior={Platform.OS === 'ios' ? 'padding' : 'height'}
            style={styles.container}
        >
            <ScrollView contentContainerStyle={styles.scroll} keyboardShouldPersistTaps="handled">
                <View style={styles.header}>
                    <Text style={styles.brandTitle}>MOTO-NAV</Text>
                    <Text style={styles.subtitle}>Gece Sürüşüne Hazır Ol</Text>
                </View>

                <View style={styles.card}>
                    <DarkInput
                        label="E-Posta"
                        placeholder="ornek@motonav.com"
                        value={email}
                        onChangeText={setEmail}
                        keyboardType="email-address"
                        autoCapitalize="none"
                    />

                    <DarkInput
                        label="Şifre"
                        placeholder="••••••••"
                        value={password}
                        onChangeText={setPassword}
                        secureTextEntry
                    />

                    <DarkButton title="Giriş Yap" onPress={handleLogin} loading={loading} />
                </View>

                <View style={styles.footer}>
                    <Text style={styles.footerText}>Hesabın yok mu? </Text>
                    <TouchableOpacity onPress={() => router.push('/(auth)/register' as any)}>
                        <Text style={styles.linkText}>Kayıt Ol</Text>
                    </TouchableOpacity>
                </View>
            </ScrollView>
        </KeyboardAvoidingView>
    );
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
        backgroundColor: '#090D16',
    },
    scroll: {
        flexGrow: 1,
        justifyContent: 'center',
        padding: 24,
    },
    header: {
        alignItems: 'center',
        marginBottom: 36,
    },
    brandTitle: {
        color: '#38BDF8',
        fontSize: 34,
        fontWeight: '900',
        letterSpacing: 2,
    },
    subtitle: {
        color: '#64748B',
        fontSize: 14,
        marginTop: 6,
    },
    card: {
        backgroundColor: '#0F172A',
        borderRadius: 20,
        padding: 24,
        borderWidth: 1,
        borderColor: '#1E293B',
    },
    footer: {
        flexDirection: 'row',
        justifyContent: 'center',
        marginTop: 28,
    },
    footerText: {
        color: '#64748B',
        fontSize: 14,
    },
    linkText: {
        color: '#38BDF8',
        fontSize: 14,
        fontWeight: '700',
    },
});