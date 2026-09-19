# 🏍️ moto-nav

Motosiklet kullanıcıları için topluluk destekli (crowdsourced) akıllı navigasyon, canlı engel/tehlike uyarı sistemi, konvoy takibi ve sürüş telemetrisi platformu.

---

## 🎯 Projenin Amacı
Türkiye'deki motosiklet sürücülerinin karşılaştığı yol engellerini (çukur, mıcır, döküntü, yağ birikintisi, gizli radar, yan rüzgar koridorları vb.) anlık olarak harita üzerinde görebilmelerini, birbirlerini uyarmalarını, güvenli park alanları ve motor dostu mekanları keşfetmelerini, toplu sürüşlerde (konvoy) anlık telemetri paylaşabilmelerini ve sürüşlerini kaydedip sosyal akışta sergileyebilmelerini sağlamak.

---

## 🛠️ Teknoloji Yığını (Tech Stack)

| Katman | Teknoloji / Kütüphane | Açıklama |
| :--- | :--- | :--- |
| **Backend Framework** | .NET 8 / 9 Web API | Clean Architecture prensipleriyle geliştirilmiş modüler REST API |
| **Database** | PostgreSQL + PostGIS | Coğrafi ve mekansal (Spatial) veri yönetimi |
| **Spatial Engine** | NetTopologySuite | .NET tarafında `Point` ve `LineString` koordinat/yarıçap (ST_DWithin) hesaplamaları |
| **Real-time Engine** | SignalR (WebSocket) | `RideHub` üzerinden düşük gecikmeli konvoy takibi ve canlı telemetri yayını |
| **Security & Auth** | ASP.NET Core Identity + JWT | Claim bazlı (`NameIdentifier`) güvenli kimlik doğrulama |
| **API Documentation** | Scalar OpenAPI | Modern, interaktif API test ve referans arayüzü |
| **Mobile Client** | React Native / Expo | iOS ve Android için eldiven/kask dostu canlı navigasyon istemcisi |
| **Web Client** | React.js + Tailwind CSS | Rota planlama, telemetri inceleme ve topluluk yönetim paneli |

---

## 📁 Proje Klasör Yapısı (Clean Architecture & Monorepo)

```text
moto-nav/
├── src/
│   ├── backend/                        # .NET Web API ve Clean Architecture Katmanları
│   │   ├── 1. Core/
│   │   │   ├── MotoNav.Domain/         # Saf Entity modelleri (Hazards, Rides, Users, Spots, Operations)
│   │   │   └── MotoNav.Application/    # DTOs, Interfaces, Repository Sözleşmeleri
│   │   ├── 2. Infrastructure/
│   │   │   ├── MotoNav.Persistence/    # EF Core, PostGIS Type Mappings, Repository Implementasyonları
│   │   │   └── MotoNav.Infrastructure/ # Harici Servisler, Token Üretimi, Güvenlik
│   │   └── 3. Presentation/
│   │       └── MotoNav.API/            # Controllers (14 Modül) ve SignalR Hubs (RideHub)
│   ├── mobile/                         # React Native mobil navigasyon uygulaması
│   └── web/                            # React.js web yönetim ve keşif paneli
├── docs/                               # Mimari şemalar, Postman/Scalar testleri
└── README.md                           # Proje dokümantasyonu

```

🧩 Sistem Modülleri (Backend Durumu: %100 Tamamlandı)
Kimlik & Motosikletçi Profili (Auth, UserProfiles)

JWT tabanlı güvenli oturum yönetimi.

Kan grubu, acil durum irtibatı, itibar puanı (reputation) ve toplam sürüş mesafesi takibi.

Garaj & Periyodik Bakım (Motorcycles, Maintenance)

Garajdaki motorların cc, kategori ve depo hacmi bazlı yönetimi.

Yağ, filtre, zincir yağlama gibi periyodik görev takip sayaçları ve servis fatura kayıtları.

Mekansal Rota & Harita Noktaları (Routes, Spots, MechanicReviews, ParkingZones)

PostGIS spatial yarıçap (nearby) sorguları.

Motorcu dostu mekanlar, güvenilir tamirci değerlendirmeleri ve kamera/zemin halkalı güvenli park alanları.

Sürüş Güvenliği & Tehlike Bildirimleri (Hazards, RoadConditions, WindHazards, SosAlerts)

Süre aşımı (ExpiresAt) olan dinamik yol durumu bildirimleri (mıcır, yol çalışması, buzlanma).

Viyadük ve köprüler için yan rüzgar tehlike koridoru uyarıları.

Tek tıkla koordinatlı acil durum (SOS) çağrısı yayını.

Sosyal Akış, Telemetri & Sesli Komutlar (TripLogs, GroupRides, VoiceCommands)

LineString GPS rotası, irtifa kazanımı, ortalama/maksimum hız içeren sürüş günlüğü.

JoinCode ile konvoya katılma ve RideHub üzerinden milisaniyelik canlı konum yayını.

Kask interkomu üzerinden verilen sesli komutların niyet analizi (Intent) günlüğü.

📌 Proje Yol Haritası
🚀 Backend & Veritabanı Katmanı (Tamamlandı)
[x] .NET 8/9 Clean Architecture mimarisi ve PostgreSQL + PostGIS bağlantısı

[x] NetTopologySuite ile coğrafi veri modelleri (Point, LineString)

[x] JWT kimlik doğrulama ve Claim bazlı rol/kullanıcı eşleme altyapısı

[x] 14 Controller modülünün CRUD ve mekansal sorgu uçlarının yazılması

[x] Scalar API Reference entegrasyonu ve uçtan uca testlerin tamamlanması

[x] SignalR RideHub canlı telemetri ve konvoy WebSocket altyapısı

📱 Frontend (Mobil & Web) Katmanı (Sıradaki Aşama)
[ ] Expo / React Native mobil çatısının kurulması ve dark mode (yüksek kontrast) tema entegrasyonu

[ ] JWT oturum saklama ve Axios/SignalR istemci servislerinin bağlanması

[ ] Canlı OpenStreetMap harita ekranı ve GPS sürüş takip arayüzü

[ ] Eldivenle kullanıma uygun "Hızlı Engel Bildir" ve "SOS" sürüş modları

[ ] Konvoy canlı takip ekranı (Grup sürüşü odası)

[ ] React.js web rota keşif ve yönetim paneli

💡 İnovatif Özellikler (Backlog & Gelecek Planı)
🎙️ İnterkom Ses Entegrasyonu: Mobil istemcide arka planda çalışan Speech-to-Text motoru ile ekrana dokunmadan "Önümde engel var mı?" sorgusu.

🌧️ Islak Sürüş Bakım Tetikleyicisi: Yağmurlu sürüşün ardından otomatik zincir bakım hatırlatıcısı tetikleme.

🚨 Otomatik Kaza Algılama: İvmeölçer ve jiroskop verilerinden sert darbe/düşme tespiti ve acil kişilere SMS iletimi.











