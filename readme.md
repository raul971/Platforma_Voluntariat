# VolunteerFlow - Platforma de Voluntariat

Aplicatie REST API pentru gestionarea voluntarilor si task-urilor in cadrul unei organizatii. Sistemul permite administratorilor sa creeze task-uri si evenimente, sa asigneze munca catre voluntari, iar voluntarii pot accepta sau refuza task-uri si pot raporta orele lucrate.

## Descriere

VolunteerFlow este un API web construit in .NET 6 care faciliteaza colaborarea intre administratori si voluntari. Aplicatia foloseste SQLite pentru baza de date si JWT pentru autentificare.

Administratorii pot:
- Crea, modifica si sterge voluntari
- Crea task-uri cu deadline-uri si estimari de ore
- Asigna task-uri catre voluntari
- Crea evenimente si invita voluntari

Voluntarii pot:
- Accepta sau refuza task-urile asignate
- Marca task-urile ca fiind completate si raporta orele lucrate
- Raspunde la invitatii pentru evenimente
- Raporta daca evenimentele la care au participat s-au desfasurat sau nu

## Tehnologii

- Backend: ASP.NET Core Web API (.NET 6)
- Baza de date: SQLite
- ORM: Entity Framework Core
- Autentificare: JWT Bearer Token
- Documentatie API: Swagger/OpenAPI
- Containerizare: Docker + Docker Compose

## Structura Bazei de Date

Aplicatia foloseste 5 tabele principale:

- **User** - Utilizatori (Admin sau Volunteer)
- **TaskModel** - Task-uri create de administratori
- **TaskAssignment** - Legaturi intre task-uri si voluntari (cu status, ore lucrate, etc.)
- **Event** - Evenimente create de administratori
- **EventParticipation** - Participari ale voluntarilor la evenimente

Relatii:
- Un utilizator poate crea multe task-uri (Admin)
- Un task poate fi asignat mai multor voluntari
- Un utilizator poate crea multe evenimente (Admin)
- Un eveniment poate avea multi participanti

## Endpoint-uri API

Aplicatia expune 26 de endpoint-uri organizate astfel:

**Autentificare (2 endpoint-uri)**
- GET /api/auth - verifica daca API-ul functioneaza
- POST /api/auth/login - autentificare utilizator (returneaza token JWT)

**Gestiune Voluntari - doar Admin (5 endpoint-uri)**
- GET /api/volunteers - lista toti voluntarii
- GET /api/volunteers/{id} - detalii despre un voluntar
- POST /api/volunteers - creeaza voluntar nou
- PUT /api/volunteers/{id} - actualizeaza voluntar
- DELETE /api/volunteers/{id} - sterge voluntar

**Task-uri - vizualizare pentru toti, modificare doar Admin (6 endpoint-uri)**
- GET /api/tasks - lista toate task-urile
- GET /api/tasks/{id} - detalii despre un task
- POST /api/tasks - creeaza task nou (Admin)
- PUT /api/tasks/{id} - actualizeaza task (Admin)
- DELETE /api/tasks/{id} - sterge task (Admin)
- POST /api/tasks/{taskId}/assign - asigneaza task unui voluntar (Admin)

**Asignari Task-uri - Voluntari (4 endpoint-uri)**
- GET /api/assignments - lista asignari (Admin vede toate, Volunteer doar ale lui)
- GET /api/assignments/{id} - detalii asignare
- PATCH /api/assignments/{id}/respond - accepta sau refuza task (Volunteer)
- PATCH /api/assignments/{id}/complete - marcheaza task completat si raporteaza ore (Volunteer)

**Evenimente - creare Admin, participare Volunteer (5 endpoint-uri)**
- GET /api/events - lista toate evenimentele
- GET /api/events/{id} - detalii eveniment
- POST /api/events - creeaza eveniment (Admin)
- POST /api/events/{id}/invite - invita voluntari la eveniment (Admin)
- GET /api/events/{id}/participations - lista participanti la eveniment (Admin)

**Participari Evenimente - Voluntari (3 endpoint-uri)**
- GET /api/eventparticipations/my-participations - evenimentele mele (Volunteer)
- PUT /api/eventparticipations/{id}/respond - raspunde la invitatie (Volunteer)
- PUT /api/eventparticipations/{id}/report - raporteaza daca evenimentul a avut loc (Volunteer)

**Test (1 endpoint)**
- GET /api/test - verifica daca API-ul functioneaza

**Total: 26 endpoint-uri** (12 GET, 6 POST, 4 PUT, 2 DELETE, 2 PATCH)

## Cerinte Sistem

- .NET 6 SDK (pentru dezvoltare locala)
- Docker Desktop (pentru rulare containerizata)

## Cum se ruleaza

### Varianta 1: Cu Docker (Recomandat)

```bash
# Navigheaza la folderul proiectului
cd Platforma_Voluntariat

# Porneste aplicatia cu Docker Compose
docker-compose up -d

# Aplicatia va porni pe:
# API: http://localhost:5000
# Swagger: http://localhost:5000/swagger
```

Comenzi utile Docker:
```bash
# Opreste aplicatia
docker-compose down

# Rebuild dupa modificari cod
docker-compose up -d --build

# Vezi logs
docker-compose logs -f api
```

### Varianta 2: Local (fara Docker)

```bash
# Navigheaza la folderul aplicatiei
cd Platforma_Voluntariat/src/VolunteerFlow.Api

# Restaureaza pachetele
dotnet restore

# Compileaza aplicatia
dotnet build

# Ruleaza aplicatia
dotnet run

# Aplicatia va porni pe:
# HTTP: http://localhost:5000
# HTTPS: https://localhost:5001
# Swagger: http://localhost:5000/swagger
```

La prima rulare, baza de date SQLite se va crea automat in fisierul `volunteer.db` si va fi populata cu date initiale:
- 1 administrator: email `admin@example.com`, parola `Admin123!`
- 2 voluntari: `john@example.com` si `jane@example.com`, parola `Volunteer123!`

## Cum se testeaza

### Testare cu Swagger (Recomandat)

1. Deschide Swagger UI la adresa: http://localhost:5000/swagger
2. Gaseste endpoint-ul POST /api/auth/login
3. Click pe "Try it out"
4. Introdu credentialele de admin:
```json
{
  "email": "admin@example.com",
  "password": "Admin123!"
}
```
5. Click "Execute" si copiaza token-ul JWT din raspuns
6. Click pe butonul "Authorize" (sus-dreapta)
7. Introdu: `Bearer <token-ul-copiat>`
8. Click "Authorize" apoi "Close"
9. Acum poti testa toate endpoint-urile care necesita autentificare

### Exemplu flow complet:

**1. Admin creeaza un task:**
- POST /api/tasks cu titlu, descriere, ore estimate, deadline

**2. Admin asigneaza task-ul unui voluntar:**
- POST /api/tasks/{taskId}/assign cu ID-ul voluntarului

**3. Voluntarul se autentifica:**
- POST /api/auth/login cu credentialele de voluntar

**4. Voluntarul vede task-urile asignate:**
- GET /api/assignments (cu token-ul de voluntar)

**5. Voluntarul accepta task-ul:**
- PATCH /api/assignments/{id}/respond cu `{"accepted": true}`

**6. Voluntarul completeaza task-ul:**
- PATCH /api/assignments/{id}/complete cu ore lucrate si notite

## Arhitectura Aplicatiei

Aplicatia foloseste Service Layer Pattern pentru separarea logicii:

**Controllere** - Primesc cererile HTTP si returneaza raspunsuri
- AuthController - Autentificare
- VolunteersController - Gestiune voluntari (Admin)
- TasksController - CRUD task-uri
- AssignmentsController - Raspuns la task-uri (Volunteer)
- EventsController - CRUD evenimente
- EventParticipationsController - Participare evenimente (Volunteer)

**Servicii** - Contin logica business
- AuthService - Validare credentiale, generare JWT
- UserService - Gestiune utilizatori
- TaskService - Logica task-uri + asignari
- EventService - Logica evenimente + participari

**Modele** - Entitati baza de date
- User, TaskModel, TaskAssignment, Event, EventParticipation

**DTOs** - Obiecte pentru transfer date intre client si server
- Separa reprezentarea API de structura bazei de date
- Controleaza ce date sunt expuse clientului

Baza de date SQLite se afla in fisierul `volunteer.db` si este gestionata de Entity Framework Core. La pornirea aplicatiei, daca baza nu exista, se creeaza automat impreuna cu datele initiale (1 admin + 2 voluntari).