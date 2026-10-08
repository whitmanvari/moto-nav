import { useState, useEffect } from 'react';
import * as Location from 'expo-location';

export interface LocationData {
    latitude: number;
    longitude: number;
    heading: number | null;
}

export function useSpeedometer() {
    const [speed, setSpeed] = useState<number>(0);
    const [location, setLocation] = useState<LocationData | null>(null);
    const [errorMsg, setErrorMsg] = useState<string | null>(null);

    useEffect(() => {
        let subscriber: Location.LocationSubscription | null = null;

        const startTracking = async () => {
            const { status } = await Location.requestForegroundPermissionsAsync();
            if (status !== 'granted') {
                setErrorMsg('Konum izni reddedildi');
                return;
            }

            subscriber = await Location.watchPositionAsync(
                {
                    accuracy: Location.Accuracy.BestForNavigation,
                    timeInterval: 500,
                    distanceInterval: 1,
                },
                (loc) => {
                    // Hız kontrolü (m/s -> km/s)
                    const rawSpeed = loc.coords.speed;
                    if (rawSpeed !== null && rawSpeed > 0) {
                        setSpeed(Math.round(rawSpeed * 3.6));
                    } else {
                        setSpeed(0);
                    }

                    // Koordinatlar & Sürüş yön açısı (heading)
                    setLocation({
                        latitude: loc.coords.latitude,
                        longitude: loc.coords.longitude,
                        heading: loc.coords.heading,
                    });
                }
            );
        };

        startTracking();

        return () => {
            if (subscriber) {
                subscriber.remove();
            }
        };
    }, []);

    return { speed, location, errorMsg };
}