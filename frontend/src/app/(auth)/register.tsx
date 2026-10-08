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

export default function RegisterScreen() {
    const router = useRouter();
    const [username, setUsername] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [loading, setLoading] = useState(false);

    const handleRegister = async () => {
        // 1. Temel Doğrulamalar
        if (!username.trim() || !email.trim() || !password.trim()) {
            Alert.alert('Eksik Bilgi', 'Lütfen tüm alanları doldurun.');
            return;
        }

        if (password !== confirmPassword) {
            Alert.alert('Hata', 'Girdiğiniz şifreler birbiriyle eşleşmiyor.');
            return;
        }

        if (password.length < 6) {
            Alert.alert('Zayıf Şifre', 'Şifreniz en az 6 karakter olmalıdır.');
            return;
        }

        setLoading(true);
        try {
            // Backend /api/Auth/register endpoint çağrısı
            await authService.register({
                username: username.trim(),
                email: email.trim(),
                password: password.trim(),
            });

            Alert.alert(
                'Kayıt Başarılı!',
                'Hesabınız oluşturuldu. Şimdi giriş yapabilirsiniz.',
                [
                    {
                        text: 'Giriş Yap',
                        onPress: () => router.replace('/(auth)/login' as any),
                    },
                ]
            );
        } catch (error: any) {
            const message =
                error.response?.data?.message ||
                (typeof error.response?.data === 'string' ? error.response.data : null) ||
                'Kayıt işlemi sırasında bir hata oluştu.';
            Alert.alert('Kayıt Başarısız', message);
        } finally {
            setLoading(false);
        }
    };

    return (
        <KeyboardAvoidingView
            behavior={Platform.OS === 'ios' ? 'padding' : 'height'}
            style={styles.container}
        >
            <ScrollView
                contentContainerStyle={styles.scroll}
                keyboardShouldPersistTaps="handled"
                showsVerticalScrollIndicator={false}
            >
                <View style={styles.header}>
                    <Text style={styles.brandTitle}>MOTO-NAV</Text>
                    <Text style={styles.subtitle}>Ekibe Katıl & Rotanı Çiz</Text>
                </View>

                <View style={styles.card}>
                    <DarkInput
                        label="Kullanıcı Adı / Sürücü Takma Adı"
                        placeholder="orn: Rider01"
                        value={username}
                        onChangeText={setUsername}
                        autoCapitalize="none"
                    />

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

                    <DarkInput
                        label="Şifre Tekrar"
                        placeholder="••••••••"
                        value={confirmPassword}
                        onChangeText={setConfirmPassword}
                        secureTextEntry
                    />

                    <DarkButton
                        title="Kayıt Ol"
                        onPress={handleRegister}
                        loading={loading}
                    />
                </View>

                <View style={styles.footer}>
                    <Text style={styles.footerText}>Zaten hesabın var mı? </Text>
                    <TouchableOpacity onPress={() => router.back()}>
                        <Text style={styles.linkText}>Giriş Yap</Text>
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
        paddingVertical: 40,
    },
    header: {
        alignItems: 'center',
        marginBottom: 28,
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
        marginTop: 24,
        marginBottom: 20,
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