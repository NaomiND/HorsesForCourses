Opleidingen
Een opleiding:
[ ] Heeft een naam.
[ ] wordt ingepland over een bepaalde periode met een start- en einddatum.
[ ] heeft vaste lesmomenten, bijvoorbeeld op maandag en woensdag van 10u tot 12u.
[ ] heeft enkel les op weekdagen (maandag t.e.m. vrijdag).
[ ] plant lessen uitsluitend binnen de kantooruren (tussen 9u00 en 17u00).
[ ] moet in totaal minstens één uur duren.
[ ] vereist een lijst van coach-competenties.
[ ] wordt begeleid door exact één coach.
[ ] is ongeldig zolang er geen lesmomenten zijn toegevoegd.
[ ] kan pas een coach toegewezen krijgen nadat de opleiding als geldig en definitief is bevestigd.
[ ] laat geen wijzigingen aan het lesrooster meer toe zodra een coach is toegewezen.

Coaches
Een coach:
[ ] beschikt over een lijst van vaardigheden (competenties).
[ ] is slechts geschikt voor opleidingen waarvoor hij of zij alle vereiste competenties bezit.
[ ] kan niet worden toegewezen aan overlappende opleidingen en moet dus beschikbaar zijn op de ingeplande momenten.
[ ] Interactie met het Domein
[ ] Om de interactie met het domein mogelijk te maken via een (toekomstige) Web API, voorzie je de volgende gefaseerde operaties:

[ ] Coach registreren: maak een coach aan met naam en e-mailadres.
[ ] Coach Competenties toevoegen/verwijderen
[ ] Cursus aanmaken: maak een lege cursus aan met naam en periode.
[ ] Cursus Competenties toevoegen/verwijderen
[ ] Cursus Lesmomenten toevoegen/verwijderen (dag, beginuur, einduur).
[ ] Cursus bevestigen: markeer de cursus als geldig en definitief, mits hij aan alle voorwaarden voldoet (inclusief minstens één lesmoment).
[ ] Coach toewijzen: wijs een coach toe, enkel indien de cursus bevestigd is en de coach geschikt én beschikbaar is.
[ ] Denk bij elk van deze stappen aan geldige foutmeldingen, domeinvalidaties en het garanderen van een consistente toestand.

Architectuur: Layered Architecture (Separation of Concerns)
    - Presentation Layer: buitenste laag. Verantwoordelijk voor communicatie met de buitenwereld. Voor APIen DTO's.
    - Application Layer: De regisseur. De nodige stappen om een taak uit te voeren, zoals ophalen van domeinobject, uitvoeren van een actie, opslaan resultaat. Deze laag bevat Services die de use cases implementeren. Deze services gebruiken de domeinobjecten. Ze praten niet rechtstreeks met de database, maar gebruiken Repository Interfaces.
    - Domain Layer: Het hart. Complexe businesslogica, regels en kernelementen van het systeem (klassen). Volledig onafhankelijk van de andere lagen. Principes van DDD (Domain Driven Design)
    - Infrastructure Layer: De technicus. Implementeert vb database-toegang, communicatie met externe systemen. (voorlopig niet nodig).

DOMAIN LAYER
- coach: entity & aggregate root
    |_ properties (Id, Name, Email) X 
    |_ collections: lijst competences X
    |_ methods: add, remove, required(has all) logica of coach juiste comp heeft

- course: entity & aggregate root
    |_ properties (Id, Name, PlanningPeriod, AssignedCoach (optie), Status)
    |_ collections: lijst competences, lijst timeslot
    |_ methodes: add, remove, addScheduledTimeSlot (validatie, weekdagen, kantooruren, geen overlap), removeTS, confirm(valideert alle regels, alles ok =>status confirmed), assignChoach (na confirmed course=> status finalized, slaat coachID, daarna course Immutable qua planning)
    
- Value Objects (kleine, onveranderlijke objecten, vertegenwoordigen concept uit domein)
    |_ Period: Heeft een StartDate en EndDate. Bevat logica om te valideren dat de einddatum na de startdatum komt.
    |_ TimeSlot: Heeft een StartTime en EndTime. Bevat logica om de duur te berekenen en te valideren (bv. minstens één uur).
    |_ ScheduledTimeSlot: Combineert een DayOfWeek (bv. maandag) met een TimeSlot. Bevat de validatieregels (enkel weekdag, binnen kantooruren 9u-17u).
    |_ Competency: Een simpele string of een object met een Name. Is onveranderlijk.

APPLICATION LAYER 
- CoachService
    |_ RegisterCoach(name, email): nieuw Coach object aanmaken, vragen aan de repository om het op te slaan.
    |_ AddCompetencyToCoach(coachId, competency): Haalt de Coach op via de repository, roept de AddCompetency methode op de Coach aan, en slaat de gewijzigde Coach weer op.

- CourseService
    |_ CreateCourse(name, startDate, endDate): Maakt een nieuwe Course aan en slaat deze op.
    |_ AddScheduledTimeSlotToCourse(CourseId, day, startTime, endTime): Haalt de Course op, roept de AddScheduledTimeSlot methode aan, en slaat de Course weer op.
    |_ ConfirmCourse(CourseId): Haalt de Course op, roept de Confirm() methode aan, en slaat de Course weer op.
    |_ AssignCoachToCourse(CourseId, coachId): 
                Haalt de Course en de Coach op via hun repositories. 
                Controleert of de Course de status Confirmed heeft.
                Controleert of de Coach alle vereiste competenties heeft (coach.HasAllCompetencies(...)).
                Controleert de beschikbaarheid van de coach (dit is complexere logica die mogelijk een Domain Service nodig heeft, zie hieronder).
                Als alles oké is, roept het Course.AssignCoach(coach) aan en slaat de Course op.

- Domain Service (Voor logica die niet logisch in één enkele aggregate past)
    |_CoachAvailabilityService:
                IsCoachAvailableForCourse(coach, Course): Deze service haalt alle andere opleidingen op waar de coach al aan toegewezen is en controleert of de lesmomenten van de nieuwe Course overlappen met de lesmomenten van de bestaande opleidingen.

INFRASTRUCTURE LAYER
Repositories: Concrete implementaties van de repository interfaces uit de Application Layer (bv. EntityFrameworkCoachRepository die met een database praat).
Hier komt de code die de database connectie, tabellen, etc. beheert.

PRESENTATION LAYER (Web API):
Controllers: bv. CoachesController, TrainingsController.
DTOs (Data Transfer Objects): Simpele klassen om data van en naar de API te sturen, bv. CreateCoachDto, TrainingDetailsDto. De controllers vertalen de DTOs naar commando's voor de Application Layer en vertalen de resultaten (of fouten) terug naar HTTP-responses.

dateOnly
