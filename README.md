# Phantoms API

.NET 10 ilə yazılmış REST API. PostgreSQL verilənlər bazası, JWT autentifikasiya, rol və icazə sistemi var.

---

## Lazım olan proqramlar

Bunları **bir dəfə** yüklə, qur, qurtardı.

| Proqram | Link |
|---------|------|
| **Git** | https://git-scm.com/downloads |
| **Docker Desktop** | https://www.docker.com/products/docker-desktop |

> **.NET SDK lazım deyil.** Hər şey Docker içində işləyir.

---

## Docker ilə işlətmək (tövsiyə olunan üsul)

### 1. Layihəni klonla

```bash
git clone https://github.com/MahirSafar/Phantoms-PA202.git
cd Phantoms-PA202
```

---

### 2. `.env` faylı yarat

`.env.example` adlı fayl var — onu kopyala:

**Windows:**
```powershell
copy .env.example .env
```

**Mac / Linux:**
```bash
cp .env.example .env
```

---

### 3. `.env` faylını aç və şifrələri dəyiş

Notepad və ya istənilən redaktor ilə `.env` faylını aç.  
Bu **3 yeri** mütləq dəyiş:

```env
POSTGRES_PASSWORD=oz_sifren_buraya

ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=PhantomsDb;Username=phantoms_user;Password=oz_sifren_buraya

JwtSettings__Key=en_az_32_simvol_random_bir_sey_yaz_buraya
```

> ⚠️ `POSTGRES_PASSWORD` ilə `ConnectionStrings`-dəki şifrə **eyni** olmalıdır.

---

### 4. Docker-i qaldır

```bash
docker compose up --build -d
```

Bu əmr:
- PostgreSQL verilənlər bazasını qaldırır
- API-ni build edib işə salır
- pgAdmin idarəetmə panelini qaldırır
- Cədvəlləri özü yaradır
- Default admin hesabını özü yaradır

İlk dəfə bir az uzun çəkə bilər (3-5 dəq) — paketlər yüklənir.

---

### 5. Qurtardı! Bunlara gir:

| Nə | URL |
|----|-----|
| **API (Swagger)** | http://localhost:8080/swagger |
| **pgAdmin** (DB idarəetmə) | http://localhost:5050 |

**Default admin hesabı:**
```
Email:    admin@phantoms.com
Şifrə:    Admin123!
```

---

## Yenilik gəldikdə (güncəlləmə)

Başqa biri kod dəyişib push edib, sən yükləmək istəyirsən:

```bash
git pull
docker compose up --build -d
```

Bütün başqa şeylər (migration, seed) avtomatik işləyir.

---

## Faydalı əmrlər

```bash
# Konteynerlərin vəziyyətinə bax
docker compose ps

# API-nin loglarına bax (xəta axtarırsansa)
docker compose logs -f api

# Hər şeyi dayandır
docker compose down

# Hər şeyi sil və sıfırdan başla (DB-dəki data da silinir!)
docker compose down -v
```

---

## pgAdmin ilə verilənlər bazasına bağlan

1. http://localhost:5050 aç
2. Giriş et:
   - Email: `admin@phantoms.com`
   - Şifrə: `admin`
3. Sol tərəfdə **"Add New Server"** düyməsinə bas
4. Bunları daxil et:
   - **Name:** Phantoms (istədiyin ad)
   - **Host:** `phantoms_postgres`
   - **Port:** `5432`
   - **Database:** `PhantomsDb`
   - **Username:** `phantoms_user`
   - **Password:** `.env`-də yazdığın şifrə

---

## Visual Studio ilə lokal işlətmək (Docker olmadan)

> Bu üsul üçün Docker-dəki PostgreSQL işləməlidir.  
> Yəni əvvəlcə `docker compose up -d` ilə DB-ni qaldır, sonra bu addımları et.

### Lazım olan əlavə proqram

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

### Addımlar

**1.** `src/Presentation/Phantoms.API/` qovluğunda `appsettings.Development.json` adlı fayl yarat.

> Bu fayl `.gitignore`-dadır — Git-ə getmir, hər developer özü yaradır.

**2.** İçinə bunu yaz (şifrəni `.env`-dəki ilə eyni yaz):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=PhantomsDb;Username=phantoms_user;Password=BURAYA_OZ_SIFREN"
  },
  "JwtSettings": {
    "Key": "BURAYA_OZ_JWT_KEY_EN_AZ_32_SIMVOL",
    "Issuer": "PhantomsAPI",
    "Audience": "PhantomsClients",
    "AccessTokenExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  },
  "SmtpSettings": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "UserName": "",
    "Password": "",
    "FromEmail": "",
    "FromName": "Phantoms",
    "UseSsl": true
  },
  "GoogleAuthSettings": {
    "ClientId": "",
    "ClientSecret": ""
  },
  "ClientBaseUrl": "http://localhost:3000"
}
```

**3.** `Phantoms.slnx` faylını Visual Studio ilə aç → **F5** bas.

API işə düşür → Swagger avtomatik açılır.

---

## Layihənin strukturu

```
src/
├── Core/
│   ├── Phantoms.Domain          → Cədvəllər, sabitlər (biznes qaydaları)
│   └── Phantoms.Application     → Biznes məntiqi, komandalar, sorğular
│
├── Infrastructure/
│   ├── Phantoms.Infrastructure  → Email, JWT, Google login xidmətləri
│   └── Phantoms.Persistence     → Verilənlər bazası, migrasiyalar
│
└── Presentation/
    └── Phantoms.API             → HTTP endpoint-lər, controller-lər
```

---

## API endpoint-ləri

Tam siyahı üçün Swagger-ə bax: http://localhost:8080/swagger

| Qrup | URL | Nə edir |
|------|-----|---------|
| Auth | `/api/auth` | Qeydiyyat, giriş, şifrə sıfırla, token yenilə |
| Products | `/api/products` | Məhsulları gör, əlavə et, dəyiş, sil |
| Admin | `/api/admin` | İstifadəçiləri idarə et, rol ver/al |
| Roles | `/api/roles` | Rol yarat, icazə ver/al |
