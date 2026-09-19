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













