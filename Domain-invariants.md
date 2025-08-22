1. - De [invarianten](../1.TheStables/readme.md) zoals geïmplementeerd in het domein, dienen door minstens één test gedocumenteerd te worden (meerdere liefst om *edge-cases* te verduidelijken).
   - Een markdown document dient te worden opgeleverd aan de hoofd aandeelhouder via een bestand in de root van de *solution*: `domain-invariants.md`. Daarin voor elke *invariant*:
     - een korte beschrijving 
     - link(s) naar de test code

# Invariants
In dit document staan de voorwaarden beschreven die altijd waar moeten zijn voor een object, ongeacht de staat ervan. Bij elk object vind je een link naar de testfile die deze voorwaarden implementeert. 

## EmailAddress
Een geldig e-mailadres mag niet leeg of 'null' zijn en kan enkel via de 'create()'-methode worden aangemaakt.
De structuur van het e-mailadres wort gevalideerd met 'System.Net.Mail.MailAddress'. Deze klasse controleert of een e-mailadres syntactisch juist is volgens een eenvoudige subset van de e-mailstandaarden (niet volledig RFC 5322). 

[E-mail validatie tests](HorsesForCourses.Tests/Tests/CoachRegistration/EmailAdressTest.cs)


## FullName
De voor- en achternaam mogen niet leeg of 'null' zijn en zijn 'immutable'. De volledige naam moet minstens uit twee delen bestaan.
'From(string)' accepteert alleen strings met minstens twee niet-lege delen. DisplayName en ToString() geven altijd "FirstName LastName" terug.

[Naam validatie tests](HorsesForCourses.Tests/Tests/CoachRegistration/FullNameTest.cs)


## PlanningPeriod
Het PlanningPeriod object vertegenwoordigt een geldige eendagperiode of meerdagenperiode. 
- De einddatum mag niet voor de startdatum liggen. 
- Een datum in de planningsperiode ligt na/is gelijk aan de startdatum en ligt voor/is gelijk aan de einddatum.

[PlanningPeriod validatie tests](HorsesForCourses.Tests/Tests/Planning/PlanningPeriod.cs)


## Timeslots
Een Timeslot object is geldig wanneer het aan volgende voorwaarden voldoet:
- De starttijd moet voor de eindtijd liggen.
- Een tijdslot moet minstens 60 minuten lang zijn. 
- Een tijslot moet binnen de kantooruren vallen (9:00 - 17:00)

[Timeslots validatie tests](HorsesForCourses.Tests/Tests/Planning/Timeslot.cs)


## ScheduledTimeslot

TimeSlot kan niet null zijn moet altijd ee ngeldige instantie zijn.
Day en TimeSlot zijn immutable. 
📌 Samenvatting: Invariants van ScheduledTimeSlot
Invariant	Omschrijving
Enum.IsDefined(typeof(WeekDays), Day)	Day moet een geldige waarde zijn van de WeekDays enum (bijv. Maandag–Vrijdag).
TimeSlot != null	TimeSlot moet altijd een geldige instantie zijn.
Day en TimeSlot zijn immutable	Na initialisatie zijn de properties niet wijzigbaar (init-only).
OverlapsWith vereist Day == other.Day	Alleen als dagen gelijk zijn, wordt TimeSlot.OverlapsWith uitgevoerd.


## Coach
Invarianten: Een competentie kan niet leeg zijn en mag niet dubbel toegevoegd worden. Een competentie moet aanwezig zijn om te worden verwijderd.

