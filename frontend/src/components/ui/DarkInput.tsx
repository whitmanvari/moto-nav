import React from 'react';
import { TextInput, TextInputProps, StyleSheet, View, Text } from 'react-native';

interface Props extends TextInputProps {
    label?: string;
    error?: string;
}

export const DarkInput: React.FC<Props> = ({ label, error, style, ...rest }) => (
    <View style={styles.container}>
        {label && <Text style={styles.label}>{label}</Text>}
        <TextInput
            placeholderTextColor="#64748B"
            style={[styles.input, error ? styles.inputError : null, style]}
            {...rest}
        />
        {error && <Text style={styles.errorText}>{error}</Text>}
    </View>
);

const styles = StyleSheet.create({
    container: { marginBottom: 16, width: '100%' },
    label: { color: '#94A3B8', fontSize: 13, marginBottom: 6, fontWeight: '600' },
    input: {
        backgroundColor: '#0F172A',
        borderColor: '#1E293B',
        borderWidth: 1.5,
        borderRadius: 12,
        color: '#F8FAFC',
        paddingHorizontal: 16,
        paddingVertical: 14,
        fontSize: 15,
    },
    inputError: { borderColor: '#EF4444' },
    errorText: { color: '#EF4444', fontSize: 12, marginTop: 4 },
});