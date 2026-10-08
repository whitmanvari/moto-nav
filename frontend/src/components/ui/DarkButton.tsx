import React from 'react';
import { TouchableOpacity, Text, ActivityIndicator, StyleSheet } from 'react-native';

interface Props {
    title: string;
    onPress: () => void;
    loading?: boolean;
}

export const DarkButton: React.FC<Props> = ({ title, onPress, loading }) => (
    <TouchableOpacity
        style={[styles.button, loading && styles.disabled]}
        onPress={onPress}
        disabled={loading}
        activeOpacity={0.8}
    >
        {loading ? (
            <ActivityIndicator color="#030712" />
        ) : (
            <Text style={styles.text}>{title}</Text>
        )}
    </TouchableOpacity>
);

const styles = StyleSheet.create({
    button: {
        backgroundColor: '#38BDF8',
        paddingVertical: 15,
        borderRadius: 12,
        alignItems: 'center',
        width: '100%',
        shadowColor: '#38BDF8',
        shadowOpacity: 0.35,
        shadowOffset: { width: 0, height: 4 },
        shadowRadius: 10,
        elevation: 5,
    },
    disabled: { opacity: 0.6 },
    text: { color: '#030712', fontSize: 16, fontWeight: '700' },
});