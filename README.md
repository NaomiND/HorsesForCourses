## Opleidingen
Een opleiding:
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
Een coach:
[X] beschikt over een lijst van vaardigheden (competenties).
[X] is slechts geschikt voor opleidingen waarvoor hij of zij alle vereiste competenties bezit.
[X] kan niet worden toegewezen aan overlappende opleidingen en moet dus beschikbaar zijn op de ingeplande momenten.
[ ] Interactie met het Domein
[ ] Om de interactie met het domein mogelijk te maken via een (toekomstige) Web API, voorzie je de volgende gefaseerde operaties:

[X] Coach registreren: maak een coach aan met naam en e-mailadres.
[X] Coach Competenties toevoegen/verwijderen
[X] Cursus aanmaken: maak een lege cursus aan met naam en periode.
[X] Cursus Competenties toevoegen/verwijderen
[X] Cursus Lesmomenten toevoegen/verwijderen (dag, beginuur, einduur).
[X] Cursus bevestigen: markeer de cursus als geldig en definitief, mits hij aan alle voorwaarden voldoet (inclusief minstens één lesmoment).
[X] Coach toewijzen: wijs een coach toe, enkel indien de cursus bevestigd is en de coach geschikt én beschikbaar is.
[x] Denk bij elk van deze stappen aan geldige foutmeldingen, domeinvalidaties en het garanderen van een consistente toestand.

# Architectuur: Layered Architecture (Separation of Concerns)
    - Presentation Layer: buitenste laag. Verantwoordelijk voor communicatie met de buitenwereld. Voor APIen DTO's.
    - Application Layer: De regisseur. De nodige stappen om een taak uit te voeren, zoals ophalen van domeinobject, uitvoeren van een actie, opslaan resultaat. Deze laag bevat Services die de use cases implementeren. Deze services gebruiken de domeinobjecten. Ze praten niet rechtstreeks met de database, maar gebruiken Repository Interfaces.
    - Domain Layer: Het hart. Complexe businesslogica, regels en kernelementen van het systeem (klassen). Volledig onafhankelijk van de andere lagen. Principes van DDD (Domain Driven Design)
    - Infrastructure Layer: De technicus. Implementeert vb database-toegang, communicatie met externe systemen. (voorlopig niet nodig).

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

**APPLICATION LAYER**  - TO DO!
- CoachService
    └── RegisterCoach(name, email): nieuw Coach object aanmaken, vragen aan de repository om het op te slaan.
    └── AddCompetencyToCoach(coachId, competency): Haalt de Coach op via de repository, roept de AddCompetency methode op de Coach aan, en slaat de gewijzigde Coach weer op.

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
Repositories: Concrete implementaties van de repository interfaces uit de Application Layer (bv. EntityFrameworkCoachRepository die met een database praat).
Hier komt de code die de database connectie, tabellen, etc. beheert.

**PRESENTATION LAYER (Web API)**
Controllers: bv. CoachesController, CoursesController.
DTOs (Data Transfer Objects): Simpele klassen om data van en naar de API te sturen, bv. CreateCoachDto, CourseDetailsDto. De controllers vertalen de DTOs naar commando's voor de Application Layer en vertalen de resultaten (of fouten) terug naar HTTP-responses.
