# VolunteerFlow - Platformă de Voluntariat

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/)
[![Docker](https://img.shields.io/badge/Docker-Compose-2496ED)](https://www.docker.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

Platformă web de management al activităților de voluntariat - Proiect Inginerie Software II (IS2).

## Descriere

**VolunteerFlow** este o aplicație REST API dezvoltată în .NET care facilitează colaborarea între administratori și voluntari într-o organizație. Sistemul permite:

- **Administratorilor**: să creeze și să asigneze task-uri, să organizeze ședințe și evenimente, să monitorizeze orele lucrate
- **Voluntarilor**: să răspundă la invitații, să marcheze progresul muncii, să raporteze orele lucrate

## Structură Documentație

📁 **Documentație Proiect:**
- [SRS_SDD.md](SRS_SDD.md) - Specificații și design (Software Requirements Specification + Software Design Document)
- [USER_STORIES_DETAILED.md](USER_STORIES_DETAILED.md) - User stories detaliate cu criterii de acceptare
- [IMPLEMENTATION_CHECKLIST.md](IMPLEMENTATION_CHECKLIST.md) - Checklist complet pentru implementare
- [API_ENDPOINTS.md](API_ENDPOINTS.md) - Documentație completă API endpoints

📊 **Diagrame Mermaid:**
- [diagrams/admin_task_assignment.mmd](diagrams/admin_task_assignment.mmd) - Flux admin: creare task + asignare
- [diagrams/volunteer_task_flow.mmd](diagrams/volunteer_task_flow.mmd) - Flux voluntar: accept/refuz + completare
- [diagrams/meeting_flow.mmd](diagrams/meeting_flow.mmd) - Flux ședință: invitație → răspuns → participare
- [diagrams/database_erd.mmd](diagrams/database_erd.mmd) - Schema bazei de date (ERD)

## Funcționalități Principale

### Pentru Administratori:
- ✅ CRUD complet: Voluntari, Task-uri, Ședințe, Evenimente
- ✅ Asignare task-uri către voluntari
- ✅ Invitare voluntari la ședințe și evenimente
- ✅ Rapoarte detaliate de ore lucrate (filtrate pe interval)

### Pentru Voluntari:
- ✅ Acceptare/Refuzare task-uri asignate
- ✅ Marcare task completat + raportare ore
- ✅ Răspuns la invitații ședințe (Particip/Nu particip)
- ✅ Marcare prezență după ședință
- ✅ Răspuns la invitații evenimente
- ✅ Raportare "eveniment a avut loc / nu a avut loc"

## Tehnologii

- **Backend**: ASP.NET Core Web API (.NET 8)
- **Database**: PostgreSQL (sau SQLite pentru dezvoltare locală)
- **ORM**: Entity Framework Core
- **Authentication**: JWT Bearer Token
- **API Documentation**: Swagger/OpenAPI
- **Containerization**: Docker + Docker Compose
- **Architecture**: REST API, Service Layer Pattern, DTO Pattern

## Cerințe Sistem

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (pentru dezvoltare locală)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (pentru rulare containerizată)
- [PostgreSQL](https://www.postgresql.org/download/) (dacă nu folosiți Docker)

## Instalare și Rulare

### Opțiune 1: Cu Docker (Recomandat)

```bash
# Clonare repository
git clone https://github.com/USERNAME/VolunteerFlow.git
cd VolunteerFlow

# Pornire aplicație (API + DB)
docker compose up --build

# Aplicația va fi disponibilă la:
# API: http://localhost:5000
# Swagger: http://localhost:5000/swagger
```

### Opțiune 2: Local (fără Docker)

```bash
# Clonare repository
git clone https://github.com/USERNAME/VolunteerFlow.git
cd VolunteerFlow/VolunteerFlow.Api

# Configurare connection string în appsettings.json
# "ConnectionStrings": {
#   "DefaultConnection": "Host=localhost;Database=volunteer_db;Username=your_user;Password=your_pass"
# }

# Restaurare pachete
dotnet restore

# Aplicare migrații
dotnet ef database update

# Rulare aplicație
dotnet run

# Aplicația va fi disponibilă la:
# API: http://localhost:5000
# Swagger: http://localhost:5000/swagger
```

## Utilizare Rapidă

### 1. Autentificare

```bash
# Login ca Admin
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@example.com","password":"password123"}'

# Response: { "token": "...", "user": {...} }
```

### 2. Creare Task

```bash
curl -X POST http://localhost:5000/api/tasks \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Organizare eveniment",
    "description": "Pregătire sală și materiale",
    "estimatedHours": 5.0,
    "deadline": "2026-01-15T18:00:00Z"
  }'
```

### 3. Asignare Task

```bash
curl -X POST http://localhost:5000/api/tasks/1/assign \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"volunteerId": 2}'
```

Pentru documentație completă API, vezi [API_ENDPOINTS.md](API_ENDPOINTS.md).

## Structură Bază de Date

**Entități principale:**
- `User` - Utilizatori (Admin + Voluntari)
- `Task` - Task-uri create de admin
- `TaskAssignment` - Legătura task ↔ voluntar (cu status, ore, etc.)
- `Meeting` - Ședințe organizate
- `MeetingInvitation` - Invitații + răspunsuri + prezență
- `Event` - Evenimente organizate
- `EventParticipation` - Participare + raport "a avut loc"

**Relații:**
- User 1:N TaskAssignment
- Task 1:N TaskAssignment
- Meeting 1:N MeetingInvitation
- Event 1:N EventParticipation

Vezi [diagrams/database_erd.mmd](diagrams/database_erd.mmd) pentru schema completă.

## Workflow Dezvoltare

```bash
# Creare branch pentru feature
git checkout -b feature/task-assignments

# Commit + Push
git add .
git commit -m "Implement task assignment logic"
git push origin feature/task-assignments

# Creare Pull Request pe GitHub/GitLab
# Review de la coechipier
# Merge în develop după approve
```

**IMPORTANT:** Toate modificările intră în `develop` doar prin Pull Request + review.

## Testare

### Swagger UI (Recomandat pentru testare manuală)
```
http://localhost:5000/swagger
```

### Postman Collection
(TODO: Adăugați un fișier `.json` cu Postman collection pentru testare)

### Unit Tests
```bash
cd VolunteerFlow.Tests
dotnet test
```

## Echipă

- **Persoana 1** - Backend (DB, Auth, Tasks, Reports) + Docker
- **Persoana 2** - Backend (Meetings, Events) + DTOs + Documentație

## Deadline și Etape

- ✅ **ETAPA I** (07.11 - 14.11.2025): Repository + descriere
- 🔄 **ETAPA II** (14.11 - 28.11.2025): SRS + SDD (2.0p)
- 🔄 **ETAPA III** (28.11 - 16.01.2026): Dezvoltare + deployment (8.0p)
- 📅 **Prezentare**: 16-22.01.2026

## Coordonatori

- **Bogdan Mocanu** - bogdan_costel.mocanu@upb.ro
- **Silviu Pantelimon** - silviu.pantelimon@upb.ro

## Licență

MIT License - vezi [LICENSE](LICENSE) pentru detalii.

## Resurse Utile

- [ASP.NET Core Documentation](https://learn.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [Docker Compose](https://docs.docker.com/compose/)
- [JWT Authentication](https://jwt.io/)
- [REST API Best Practices](https://restfulapi.net/)

---

**Proiect realizat în cadrul cursului Inginerie Software II - Universitatea Politehnica București**