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

### 🧩 Temel Sistem Modülleri ve İşleyiş Mantığı

1. **Topluluk Tabanlı Yol Engelleri (Crowdsourcing Hazards)**
   * Sürücüler haritaya anlık engel ekleyebilir (çukur, tümsek, bozuk asfalt, yağ birikintisi, radar vb.).
   * **Doğrulama Sistemi:** Bölgeden geçen diğer sürücüler oy verir; güvenilirlik puanı düşen ihbarlar haritadan otomatik temizlenir.

2. **Canlı Konvoy ve Telemetri Motoru (Real-Time Convoy Tracking)**
   * Sürücüler SignalR WebSocket tüneli (`RideHub`) üzerinden odaya bağlanır.
   * Konvoydaki tüm sürücülerin anlık enlem, boylam, hız ve rota pusula açısı milisaniyelik gecikmeyle haritada güncellenir.

3. **Mekansal Yakınlık ve Rota Güvenliği (Spatial Proximity Alerts)**
   * NetTopologySuite ve PostGIS `ST_DWithin` fonksiyonları ile sürücünün bulunduğu konuma göre 5 km / 15 km yarıçapındaki tehlikeler taranır.
   * Viyadük ve köprü geçişlerindeki kritik yan rüzgar koridorları (`WindHazards`) harita üzerinde dinamik uyarı üretir.

4. **Sürüş Günlüğü ve Garaj (TripLog & Garage)**
   * Sürüş esnasındaki tüm GPS koordinatları `LineString` geometrisi olarak kaydedilir; irtifa kazanımı, ortalama ve azami hız telemetrisi saklanır.
   * Kullanıcılar garajlarına motorlarını ekleyebilir, periyodik bakım aralıklarını ve servis geçmişini takip edebilir.

5. **Kask İçi Sesli Komut Günlüğü (Voice Commands)**
   * Sürüş sırasında eldivenle ekrana dokunmadan interkom üzerinden verilen sesli niyetler (`Intent`) ve komut koordinatları sisteme işlenir.

---

### 📌 Sürüm 1.0 (MVP) Yol Haritası

* **Aşama 1: Altyapı ve Güvenlik**
  * [x] .NET Clean Architecture çözümünün ve katmanlarının kurulması
  * [x] PostgreSQL + PostGIS bağlantılarının kurulması (EF Core + NetTopologySuite)
  * [x] ASP.NET Core Identity ve JWT tabanlı kimlik doğrulama altyapısı

* **Aşama 2: Coğrafi Veri, Rota ve Güvenlik API'leri**
  * [x] Mekanlar, servis yorumları ve güvenli motosiklet park alanları modülleri
  * [x] Tehlike, mıcır ve yol durumu bildirimleri için PostGIS spatial yarıçap sorguları
  * [x] Viyadük/köprü yan rüzgar tehlike koridoru hesaplamaları

* **Aşama 3: Canlı İletişim ve Sosyal Özellikler**
  * [x] SignalR `RideHub` üzerinden konvoy canlı konum ve yön yayını
  * [x] Sürücü profili, garaj yönetimi ve LineString GPS iziyle sürüş kaydı (TripLog)
  * [x] Kask interkomu sesli komut telemetri günlüğü
  * [x] 14 API modülünün Scalar üzerinde uçtan uca test edilmesi

* **Aşama 4: Mobil ve Web İstemci (Aktif Aşama)**
  * [ ] React Native / Expo mobil navigasyon çatısının oluşturulması
  * [ ] OpenStreetMap harita ekranı ve canlı GPS takibinin bağlanması
  * [ ] Eldiven dostu büyük butonlu "Hızlı Bildir" ve "SOS" sürüş modları
  * [ ] Konvoy canlı harita ekranı (SignalR dinleyicisi)
  * [ ] Web yönetim ve rota keşif paneli











