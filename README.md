# 🏍️ moto-nav
Motosiklet kullanıcıları için topluluk destekli (crowdsourced) akıllı navigasyon, canlı engel uyarı sistemi ve sürüş paylaşım platformu.

---

## 🎯 Projenin Amacı
Türkiye'deki motosiklet sürücülerinin karşılaştığı yol engellerini (çukur, tümsek, bozuk asfalt, ters yön, gizli radar vb.) anlık olarak harita üzerinde görebilmelerini, birbirlerini uyarmalarını, viraj kalitesi yüksek/manzaralı motor rotalarını keşfetmelerini ve sürüşlerini kaydedip sosyal ortamda paylaşabilmelerini sağlamak.

---

## 🛠️ Teknoloji Yığını (Tech Stack)

| Katman | Teknoloji / Kütüphane | Açıklama |
| :--- | :--- | :--- |
| **Backend Framework** | .NET 8 / 9 Web API | Yüksek performanslı API servisleri ve iş kuralları |
| **Database** | PostgreSQL + PostGIS | Coğrafi ve mekansal (Spatial) veri yönetimi |
| **Spatial Engine** | NetTopologySuite | .NET tarafında PostGIS koordinat/şekil hesaplamaları |
| **Real-time Engine** | SignalR (WebSocket) | Batarya dostu, düşük gecikmeli canlı uyarı akışı |
| **Mobile App** | React Native | iOS ve Android için cross-platform canlı navigasyon |
| **Web Client** | React.js | Rota keşif paneli ve yönetim ekranı |
| **Map & Navigation** | OpenStreetMap (OSM) / Leaflet | Ücretsiz, açık kaynaklı harita ve altlık servisleri |

---

## 📁 Proje Klasör Yapısı (Monorepo & Clean Architecture)

```text
moto-nav/
├── src/
│   ├── backend/                     # .NET Web API ve Clean Architecture Katmanları
│   │   ├── 1. Core/
│   │   │   ├── MotoNav.Domain/         # Saf Varlıklar (User, Hazard, Route, TripLog, Garage)
│   │   │   └── MotoNav.Application/    # DTOs, Interfaces (IRoutingService, IHazardSpatialRepository)
│   │   ├── 2. Infrastructure/
│   │   │   ├── MotoNav.Persistence/    # EF Core, PostGIS Mappings, Repository implementasyonları
│   │   │   └── MotoNav.Infrastructure/ # OSM Entegrasyonu, JWT, Güvenlik
│   │   └── 3. Presentation/
│   │       └── MotoNav.API/            # Controllers, SignalR Hubs (Canlı Takip/Uyarı)
│   ├── mobile/                      # React Native mobil navigasyon uygulaması
│   └── web/                         # React.js web yönetim & rota keşif paneli
├── docs/                            # Mimari şemalar, API dokümanları ve yol haritası
└── README.md                        # Proje anayasası

🧩 Temel Sistem Modülleri ve İşleyiş Mantığı
1. Topluluk Tabanlı Yol Engelleri (Crowdsourcing Hazards)
 Sürücüler haritaya anlık engel ekleyebilir (Çukur, Tümsek, Bozuk Asfalt, Ters Yön, Radar).
 Doğrulama Sistemi: Bölgeden geçen diğer sürücüler "Doğru / Yol Temizlendi" oylaması yapar. Puanı eksiye düşen engeller haritadan otomatik kaldırılır.
2. Canlı Uyarı Motoru (Spatial Proximity Alerts)
 Mobil uygulama sürekli sunucuya istek atmak (polling) yerine SignalR (WebSocket) tüneli açar.
 Backend, PostGIS'in ⁠ST_DWithin⁠ fonksiyonu ile sürücünün yaklaşmakta olduğu rotayı izler.
 Engel 200 metre kala sunucu tarafından telefona anlık push iletilir ve sesli uyarı verilir.
3. Sürüş Günlüğü ve Garaj (TripLog & Garage)
 Sürücüler "Sürüşü Başlat" diyerek gittikleri virajlı ve manzaralı rotaları birer ⁠LineString⁠ çizgisi olarak kaydedebilir.


 Kullanıcılar garajlarına motorlarını ekleyebilir, profillerinde toplam sürüş mesafelerini ve topluluk puanlarını sergileyebilir.


📌 Sürüm 1.0 (MVP) Yol Haritası
 [ ] Aşama 1: Altyapı ve Veritabanı
 [ ] .NET Clean Architecture çözümünün ve klasörlerin oluşturulması
 [ ] PostgreSQL + PostGIS bağlantılarının kurulması (EF Core + NetTopologySuite)
 [ ] JWT tabanlı kullanıcı kayıt ve giriş sistemi

 [ ] Aşama 2: Coğrafi Veri ve Harita Entegrasyonu
 [ ] React Native üzerinde OpenStreetMap harita ekranının açılması
 [ ] Haritaya engel/not ekleme ve PostGIS üzerinde saklama API'lerinin yazılması
 [ ] Topluluk doğrulama (oylama) mekanizmasının backend'e işlenmesi

 [ ] Aşama 3: Canlı Bildirim ve Rota Motoru
 [ ] SignalR Hub kurulumu ve mobil uygulama tünel bağlantısı
 [ ] Rota üzeri yaklaşan engel algılama altyapısının tamamlanması

 [ ] Aşama 4: Sosyal Özellikler
 [ ] Sürücü profili, garaj ve sürüş kaydetme (TripLog) modüllerinin eklenmesi


💰 Gelecek Dönem Gelir Modeli Planı (Monetization)
 Sponsorlu Mekanlar: Motor dostu kafelerin, tamircilerin ve ekipman mağazalarının harita üzerinde öne çıkarılması.
 Premium Rota Satışları: Özel manzaralı ve virajlı gezi rotalarının (GPX) topluluğa sunulması.
 B2B Kurye Entegrasyonu: Motorlu kurye filoları için engel uyarıları içeren kurumsal paketler.

🎨 Frontend (Mobile & Web) Mimarisi ve Ekran Tasarımı
Frontend mimarimiz iki ana parçadan oluşur:
1. React Native (Mobil): Sürücünün gidonda/pette kullandığı ana navigasyon ve harita uygulaması.
2. React.js (Web Admin & Keşif): Web üzerinden rota inceleme, topluluk engellerini onaylama ve yönetim paneli.
📱 1. React Native Mobil Mimari (src/mobile)
Mobil uygulama tarafında Clean Architecture prensiplerine uygun, durum yönetiminin (State Management) ve harita bileşenlerinin modüler ayrıldığı Feature-Based (Özellik Tabanlı) bir klasör yapısı kullanacağız.
📁 Mobil Klasör Yapısı

src/mobile/
├── assets/                 # Custom markerlar, sesli uyarı dosyaları, ikonlar
├── src/
│   ├── api/                # Axios / RTK Query istemcileri ve backend servisleri
│   ├── components/         # Ortak UI bileşenleri (Button, Input, Card, Modal)
│   ├── constants/          # Renk paleti (High Contrast Dark Mode), Map stilleri
│   ├── features/           # Modüler özellikler
│   │   ├── auth/           # Login / Register ekranları & state
│   │   ├── garage/         # Sürücü profili ve garaj yönetimi
│   │   ├── navigation/     # Canlı GPS, Rota Çizimi ve SignalR Dinleyicisi
│   │   ├── reports/        # Haritaya hızlı engel ekleme (Quick Action Menu)
│   │   └── trip-logger/    # Sürüş kaydetme (TripLog) sayacı ve GPX export
│   ├── hooks/              # Custom Hooks (useGPSLocation, useSignalR, usePermissions)
│   ├── navigation/         # React Navigation (Stack & Tab Bar Yapısı)
│   └── store/              # Redux Toolkit veya Zustand (Global State)
└── App.tsx

🖥️ 2. Mobil Ekranlar ve Kullanıcı Deneyimi (UX/UI)
Sürüş güvenliğini ön planda tutan 4 ana tab (sekme) ve kritik ekranlar:
1. Canlı Harita & Navigasyon Ekranı (Ana Ekran)
 OpenStreetMap + MapLibre/Leaflet: Yüksek performanslı vector harita gösterimi.
 Hızlı Engel Ekle (Sürüş Modu Butonu): Motor sürerken tek tıkla büyük butonlarla "Çukur var", "Radar var" diyebilme menüsü.
 Anlık Hız ve Uyarı Paneli: Ekranın üst kısmında yaklaşan engelleri gösteren yüksek kontrastlı (Kırmızı/Sarı) canlı uyarı kartı.

2. Rota Oluşturucu & Keşfet Ekranı
 Başlangıç ve bitiş noktası seçimi.
 Rota seçenekleri: "En Hızlı Rota" veya "Virajlı / Manzaralı Motor Rotası".
 Rota üzerindeki kayıtlı engellerin önizlemesi.

3. Sürüş Kaydedici (Trip Logger)
 "Sürüşü Başlat / Bitir" butonu.
 Canlı süre, anlık hız, ortalama hız ve katedilen mesafe göstergesi.
 Sürüş bitince rotaya isim verip gizli veya herkese açık paylaşma seçeneği.

4. Sürücü Profili & Garajım
 Sürücü biyografisi, toplam sürülen km ve topluluk güvenilirlik rozetleri.
 Garajdaki motosikletler (Örn: Yamaha MT-07 - 689cc).
 Kaydedilen eski sürüş geçmişi.

⚡ 3. Kritik Mobil Teknolojiler ve Custom Hook'lar
 Harita & Konum: ⁠react-native-geolocation-service⁠ (Arka planda ve ekran kapalıyken GPS takibi için).
 Canlı İletişim: ⁠@microsoft/signalr⁠ (Backend'deki SignalR Hub'a bağlanıp anlık çukur/radar uyarısı almak için).
 State Management: ⁠Zustand⁠ veya ⁠Redux Toolkit⁠ (Karmaşık navigasyon durumlarını hafif bir şekilde yönetmek için).
 Yerel Depolama: ⁠react-native-mmkv⁠ (İnternet çekmediğinde sürüş koordinatlarını telefona hızlıca önbelleklemek için).

🌐 4. React.js Web Yönetim Paneli (src/web)
Web tarafı daha çok arka plan yönetimi ve masaüstünden rota incelemek isteyenler içindir.
 Vite + React + Tailwind CSS: Işık hızında açılan web istemcisi.

 Kullanım Alanları:
 Sürücülerin web üzerinden detaylı GPX rotalarını incelemesi.
 Admin paneli: Asılsız engel ihbarlarını yönetme, kullanıcı engelleme ve topluluk istatistikleri.

📌 Frontend Yol Haritası (MVP Aşamaları)
 [ ] Aşama 1: React Native projesinin ⁠src/mobile⁠ içinde oluşturulması ve Navigation (Tab/Stack) altyapısı.
 [ ] Aşama 2: OpenStreetMap harita bileşeninin eklenmesi ve kullanıcının anlık konumunun haritada gösterilmesi.
 [ ] Aşama 3: Harita üzerine dokunarak veya "Hızlı Ekle" menüsüyle engel ekleme modalının yapılması.
 [ ] Aşama 4: Backend SignalR servisine bağlanıp gelen canlı engelleri haritada pop-up/sesli uyarı olarak gösterme.
 [ ] Aşama 5: Sürüş kaydetme (Trip Log) ekranının ve Garaj/Profil arayüzünün tamamlanması.

🚀 Gelecek Sürümler İçin İnovatif Fikirler (Backlog)
1. 🎙️ Bluetooth Kask İnterkom Sesli Komut & Entegrasyonu
 Problem: Motor sürerken telefon ekranına dokunmak imkansızdır ve hayati tehlike yaratır.
 Çözüm: Cardo, Sena, Knmaster gibi kask interkomlarıyla uyumlu sesli komut sistemi.
 Nasıl Çalışacak? Sürücü butona basıp veya sesle "Hey Moto, çukur ekle" ya da "Önümde ne var?" dediğinde uygulama arka planda GPS konumunu alıp engeli otomatik veritabanına işleyecek ve sürücüye kulaklıktan sesli yanıt verecek ("Çukur eklendi, teşekkürler").
2. 🌧️ Anlık Hava Durumu ve Yol Tutuş Analizi (Asfalt Isısı & Yağmur)
 Problem: Yağmur başladıktan sonraki ilk 15 dakika asfalt üzerindeki yağlar üste çıktığı için motor için buz pistine döner.
 Çözüm: OpenWeather API entegrasyonu ile rota üzerindeki anlık hava durumu ve yağış uyarısı.
 Özellik: Rota çizilirken "Dikkat: Şile yolunda 20 dakika sonra yağmur bekleniyor, virajlarda yol tutuşu düşebilir!" uyarısı basılması.
3. 🚨 SOS & Düşme Algılama (Crash Detection)
 Problem: Tek başına veya tenha yollarda kaza yapan motorcuların yardım çağıramaması.
 Çözüm: Telefonun jiroskop ve ivmeölçer (accelerometer) sensörlerini kullanarak sert düşme/kaza tespiti.
 Nasıl Çalışacak? Uygulama ani ivme kaybı ve devrilme algıladığında ekranda 30 saniyelik bir geri sayım başlatır. Eğer sürücü "İyiyim" butonuna basmazsa, kullanıcının belirlediği acil durum kişilerine veya yakındaki diğer ⁠moto-nav⁠ kullanıcılarına koordinatlı SOS SMS'i/bildirimi gönderir.
4. 🏍️ Grup Sürüşü Modu (Group Riding & Live Tracking)
 Problem: Toplu sürüşlerde (5-10 motor) arkada kalanların kaybolması veya gruptan kopması.
 Çözüm: Tek tıkla "Grup Sürüşü Odası" açma.
 Nasıl Çalışacak? Grup lideri bir oda kodu paylaşır. Harita üzerinde gruptaki tüm arkadaşlarının anlık konumları farklı renklerde ikonlarla görünür. En arkadaki kişi gruptan çok koparsa lidere "Arkada kopma var, yavaşla" uyarısı gider.
5. ⛽ Akıllı Yakıt & Depo Takibi
 Problem: Benzincisi az olan virajlı/dağlık rotalarda yolda kalma korkusu.
 Çözüm: Sürücü profilindeki motorun depo hacmi ve ortalama yakıt tüketimine göre rota üzerinde benzin durağı önerme.
 Nasıl Çalışacak? Motosikletin menzili 200 km ise ve 180 km'lik dağ rotasına giriliyorsa, uygulama "Deponuz bu rota için sınırda. 12. km'deki son benzinlikte depo doldurmanız önerilir" uyarısı basar.
6. 🏆 Topluluk Oyunlaştırması (Gamification & Rozetler)
 Problem: Kullanıcıların haritaya engel ekleme veya oylama yapma motivasyonunu artırmak.
 Çözüm: Katkı sağlayan sürücülere profil rozetleri ve unvanlar verme.
 Örnekler:
 "Yol Muhafızı": 50'den fazla doğru çukur/radar bildirimi yapanlara verilir.
 "Kaptan": 10'dan fazla popüler rota oluşturanlara verilir.
 Puan Mağazası: İleride sponsorlu ekipman mağazalarında bu puanlarla indirim kuponu kazanma şansı.
















