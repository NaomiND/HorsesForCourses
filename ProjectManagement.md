## Authentication / Authorization
- [ ] Anonieme gebruiker kan **Index**-pagina zien
- [ ] Anonieme gebruiker krijgt geen toegang tot de methodes van de CoachesController (route: **/Coaches**)
- [ ] Anonieme gebruiker kan wel **/Coaches/Index** openen
- [ ] Ingelogde gebruiker krijgt toegang tot alle methodes van de CoachesController


# geen prioriteit
- [X] Fout bij aanmaken coach: Naam moet een voor- en achternaam bevatten. (Parameter 'fullName') : parametertekst wil ik weg
- [] Story 5: comma seperated skills aan te passen die userfriendly is. zie Abigail : met aparte modelview, en hierbij kan partial view gebruikt worden
- [] bij coaches toevoegen hoeveel courses die heeft. Tellen mag niet via allcourses want dit is veel te impactvol voor de db. doe dit bv enkel met id. 
- [X] zorg dat coach.course werkt 
- [X] Foutafhandeling voor domein-validaties geïmplementeerd in controllers via try-catch, wat de correcte aanpak is voor het retourneren van een view met ModelState.

# Architectuur: Layered Architecture (Separation of Concerns)  => meer naar clean architecture (onion architecture)
    - Presentation Layer: buitenste laag. Verantwoordelijk voor communicatie met de buitenwereld. Voor API en DTO's.
    - Application Layer: De regisseur. De nodige stappen om een taak uit te voeren, zoals ophalen van domeinobject, uitvoeren van een actie, opslaan resultaat. Deze laag bevat Services die de use cases implementeren. Deze services gebruiken de domeinobjecten. Ze praten niet rechtstreeks met de database, maar gebruiken Repository Interfaces.
    - Domain Layer: Het hart. Complexe businesslogica, regels en kernelementen van het systeem (klassen). Volledig onafhankelijk van de andere lagen. Principes van DDD (Domain Driven Design)
    - Infrastructure Layer: De technicus. Implementeert vb database-toegang, communicatie met externe systemen.

**DOMAIN LAYER** ok
- coach: entity & aggregate root
    └── properties: Id, Name, Email  
    └── collections: list competences 
    └── methods: create, add comp, remove comp, required(coach has req. comp) 

- course: entity & aggregate root
    └── properties: Id, Name, PlanningPeriod, AssignedCoach(optie)
    └── collections: list competences, list ScheduledTimeSlot (lesmomenten)
    └── methods: create, add comp, remove comp, addScheduledTimeSlot , removeTS, confirm(alles ok =>status confirmed), 
    assignCoach (status finalized: slaat coachID op + Na toewijzing → immutable planning)
    Een opleiding is ongeldig zolang er geen lesmomenten zijn
    
- Value Objects (kleine, onveranderlijke objecten, vertegenwoordigen concept uit domein)
    └── Period: StartDate en EndDate. (met logica + validatie)
    └── TimeSlot: StartTime en EndTime. ( met logica + validatie bv min 1u)
    └── ScheduledTimeSlot: Combineert een Weekday (Enum) (bv. maandag) met een TimeSlot. Met validatie: weekdagen, kantooruren, geen overlap

**APPLICATION LAYER**  
- CoachService
    └── RegisterCoach(name, email): nieuw Coach object aanmaken, vragen aan de repository om het op te slaan.
    └── Add/RemoveCompetencyToCoach(coachId, competency): Haalt de Coach op via de repository, roept de Add/Remove methode op Coach aan, en slaat de gewijzigde Coach weer op.

- CourseService
    └── CreateCourse(name, startDate, endDate): Maakt een nieuwe Course aan en slaat deze op.
    └── AddScheduledTimeSlotToCourse(CourseId, day, startTime, endTime): Haalt de Course op, roept de AddScheduledTimeSlot methode aan, en slaat de Course weer op.
    └── ConfirmCourse(CourseId): Haalt de Course op, roept de Confirm() methode aan, en slaat de Course weer op.
    └── AssignCoachToCourse(CourseId, coachId): 
                Haalt de Course en de Coach op via hun repositories. 
                Controleert of de Course de status Confirmed heeft.
                Controleert of de Coach alle vereiste competenties heeft (coach.HasAllCompetencies(...)).
                Controleert de beschikbaarheid van de coach (dit is complexere logica die mogelijk een Domain Service nodig heeft, zie hieronder).
                Als alles oké is, roept het Course.AssignCoach(coach) aan en slaat de Course op.

- Domain Service (Voor logica die niet logisch in één enkele aggregate past)
    └──CoachAvailabilityService:
                IsCoachAvailableForCourse(coach, Course): Deze service haalt alle andere opleidingen op waar de coach al aan toegewezen is en controleert of de lesmomenten van de nieuwe Course overlappen met de lesmomenten van de bestaande opleidingen.


**INFRASTRUCTURE LAYER** 
Repositories: Concrete implementaties van de repository interfaces uit de Application Layer (bv. EntityFrameworkCoachRepository die met een database praat). Of een tijdelijke opslag via InMemory. 
Hier komt de code die de database connectie, tabellen, etc. beheert.

**PRESENTATION LAYER (Web API + MVC)**

Controllers: Coachesontroller, CourseController.
DTOs (Data Transfer Objects): Data van en naar de API te sturen: CreateCoachDTO, CourseDetailsDto. 
De controllers vertalen de DTOs naar commando's voor de Application Layer en vertalen de resultaten (of fouten) terug naar HTTP-responses.

API Controllers + DTO’s:
Controllers praten alleen via de Application Layer
Duidelijke scheiding van HTTP-laag en domeinlogica
Voorzie foutmeldingen bij invalidaties (bv. 422 BadRequest met uitleg)

# TODO fase 1 CORE
## Opleidingen
[X] Heeft een naam.
[X] wordt ingepland over een bepaalde periode met een start- en einddatum.
[x] heeft vaste lesmomenten, bijvoorbeeld op maandag en woensdag van 10u tot 12u.
[X] heeft enkel les op weekdagen (maandag t.e.m. vrijdag).
[x] plant lessen uitsluitend binnen de kantooruren (tussen 9u00 en 17u00).
[X] moet in totaal minstens één uur duren.
[X] vereist een lijst van coach-competenties.
[X] wordt begeleid door exact één coach.
[X] is ongeldig zolang er geen lesmomenten zijn toegevoegd.
[X] kan pas een coach toegewezen krijgen nadat de opleiding als geldig en definitief is bevestigd.
[X] laat geen wijzigingen aan het lesrooster meer toe zodra een coach is toegewezen.

## Coaches
[X] beschikt over een lijst van vaardigheden (competenties).
[X] is slechts geschikt voor opleidingen waarvoor hij of zij alle vereiste competenties bezit.
[X] kan niet worden toegewezen aan overlappende opleidingen en moet dus beschikbaar zijn op de ingeplande momenten.
[X] Interactie met het Domein
[X] Om de interactie met het domein mogelijk te maken via een (toekomstige) Web API, voorzie je de volgende gefaseerde operaties:

[X] Coach registreren: maak een coach aan met naam en e-mailadres.
[X] Coach Competenties toevoegen/verwijderen
[X] Cursus aanmaken: maak een lege cursus aan met naam en periode.
[X] Cursus Competenties toevoegen/verwijderen
[X] Cursus Lesmomenten toevoegen/verwijderen (dag, beginuur, einduur).
[X] Cursus bevestigen: markeer de cursus als geldig en definitief, mits hij aan alle voorwaarden voldoet (inclusief minstens één lesmoment).
[X] Coach toewijzen: wijs een coach toe, enkel indien de cursus bevestigd is en de coach geschikt én beschikbaar is.
[x] Denk bij elk van deze stappen aan geldige foutmeldingen, domeinvalidaties en het garanderen van een consistente toestand.

# TODO - fase 2 WEBAPI - CONTROLLERS

### 🧩 POST /coaches                OK  
**Als** administrator **Wil ik** een coach kunnen registreren met naam en e-mailadres
**Zodat** deze beschikbaar is voor toekomstige cursussen

### 🧩 POST /coaches/{id}/skills    OK
**Als** administrator **Wil ik** competenties kunnen toevoegen of verwijderen bij een coach
**Zodat** zijn of haar geschiktheid aangepast kan worden

### 🧩 POST /courses                OK  
**Als** cursusverantwoordelijke **Wil ik** een nieuwe cursus kunnen aanmaken met naam en periode
**Zodat** ik nadien het rooster en de vereisten kan invullen

### 🧩 POST /courses/{id}/skills    OK
**Als** cursusverantwoordelijke **Wil ik** competenties kunnen toevoegen of verwijderen bij een cursus
**Zodat** ik kan aangeven wat een coach moet kunnen

### 🧩 POST /courses/{id}/timeslots OK
**Als** cursusverantwoordelijke **Wil ik** lesmomenten kunnen toevoegen of verwijderen bij een cursus
**Zodat** ik het rooster kan opstellen

### ✅ POST /courses/{id}/confirm   OK
**Als** cursusverantwoordelijke **Wil ik** een cursus kunnen bevestigen
**Zodat** ik zeker weet dat alles in orde is en een coach toegewezen mag worden

### 🧩 POST /courses/{id}/assign-coach  OK
**Als** cursusverantwoordelijke **Wil ik** een coach kunnen toewijzen aan een bevestigde cursus
**Zodat** de cursus daadwerkelijk kan doorgaan met een geschikte coach


### 🧩 Get /coaches         OK   
**Als** administrator **Wil ik** een lijst van alle coaches kunnen opvragen
**Zodat** ik een eenvoudig overzicht heb

### 🧩 Get /coaches/{id}    OK
**Als** administrator **Wil ik** een coach per id kunnen opvragen
**Zodat** deze data beschikbaar is voor weergave en wijziging

### 🧩 Get /courses         OK
**Als** cursusverantwoordelijke **Wil ik** een lijst van alle coaches kunnen opvragen
**Zodat** ik een eenvoudig overzicht heb

### 🧩 Get /courses/{id}    OK
**Als** cursusverantwoordelijke **Wil ik** een cursus per id kunnen opvragen 
**Zodat** deze data beschikbaar is voor weergave en wijziging

## Implementeren EF Core
- [X] Omgeving opzetten voor EF Core : Infrastructure map + nodige packages installeren
- [X] DbContext aanmaken voor coach
- [X] DbContext aanmaken voor course
- [X] Repository interfaces aanmaken in .core : coach
- [x] Repository interfaces aanmaken in .core : course
- [X] EfRepository aanmaken in .infrastructure : coach
- [X] EfRepository aanmaken in .infrastructure : course
- [X] Configureer EF in Program.cs (UseSqlite)
- [X] Pas controllers aan (async) om EF repositories te gebruiken: coach + course
- [X] Run migratie
- [X] Wat is het probleem met de skills (zowel bij course als coach)

# TODO fase 3
- [X] generic id is beter dan een guid id voor deze opdracht
- [X] geen domeinobjecten teruggeven ook niet bij ophalen
- [X] mapper maken voor coach en course
- [X] [HttpPost("{id}/skills")] - aanpassen
- [X] try-catch niet in controllers
- [X] mapstructuur aanpassen

- [X] Guid Id naar int Id
- [X] Competences naar Skills
- [X] RequiredCompetences naar Skills
- [X] POST /coaches : return enkel id; verwijder naam, e-mail, skills
- [X] POST /coaches/{id}/skills : Competences naar Skills
- [X] POST /Course : courseName naar Name en Id verwijderen
- [X] POST /courses/{id}/skills : ReqComp => Skills
- [X] POST /courses/{id}/timeslots : day int => string, tijd naar uur ipv uur en minuten?
- [X] POST /courses/{id}/confirm
- [X] POST /courses/{id}/assign-coach : courseId verwijderen

- [X] GET /coaches: numberOfCoursesAssignedTo toevoegen, skills verwijderen
- [X] GET /coaches/{id} : Toevoegen: "courses": [ { "id": 10, "name": "Advanced C#" }, { "id": 12, "name": "Test-Driven Development" }  ]
- [X] GET /courses : verwijderen: period (enkel start en end), skills, timeslots, IsConfirmed => hasSchedule, assignedCoachId;   
toevoegen: hasCoach : false
- [X] GET /courses/{id} : verwijderen period (enkel start en end), isConfirmed; aanpassen: ScheduledTimeSlots, assignedcoach => coach (met id en naam) 
- [X] Controle of de nieuwe course overlapt met een coach en een bestaande course (methode nog te maken en implementeren)

# ToDO MVC implementatie Coach
- [X] Story 1: Project & DI sanity
- [X] Story 2: Coaches *Index* (lijst) (First Steps)
- [X] Story 3: *Register Coach* (GET/POST, validatie, anti-forgery)
- [X] Story 4: *Coach Detail* (read-only)
- [X] Story 5: *Update Skills* (GET/POST)
- [X] Story 6: *Paging op Index*
- [X] Story 7: *Foutafhandeling & UX-polish*

[Stories](Documentation\MVC_make_a_plan.md)