import React, { useEffect, useState } from 'react';
import { View, ActivityIndicator, StyleSheet } from 'react-native';
import { Redirect } from 'expo-router';
import { storageService } from '../services/storage';

export default function EntryScreen() {
  const [tokenChecked, setTokenChecked] = useState(false);
  const [hasToken, setHasToken] = useState(false);

  useEffect(() => {
    const checkToken = async () => {
      const token = await storageService.getToken();
      setHasToken(!!token);
      setTokenChecked(true);
    };
    checkToken();
  }, []);

  if (!tokenChecked) {
    return (
      <View style={styles.container}>
        <ActivityIndicator size="large" color="#38BDF8" />
      </View>
    );
  }

  // Token varsa ana uygulamaya, yoksa login'e yönlendirir (kalıcı mimari)
  return hasToken ? <Redirect href={'/(tabs)' as any} /> : <Redirect href={'/(auth)/login' as any} />;
}

const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: '#090D16', justifyContent: 'center', alignItems: 'center' },
});