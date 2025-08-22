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

[PlanningPeriod validatie tests](HorsesForCourses.Tests/Tests/Planning/PlanningPeriodTests.cs)


## Timeslots
Een Timeslot object is geldig wanneer het aan volgende voorwaarden voldoet:
- De starttijd moet voor de eindtijd liggen.
- Een tijdslot moet minstens 60 minuten lang zijn. 
- Een tijslot moet binnen de kantooruren vallen (9:00 - 17:00).

[Timeslots validatie tests](HorsesForCourses.Tests/Tests/Planning/TimeslotTests.cs)


## ScheduledTimeslot
In dit object kijken we of het gekozen tijdslot geldig is, op een weekdag doorgaat en eventueel overlapt met een ander tijdslot. 
- Een tijdslot kan enkel geboekt worden op werkdagen (maandag - vrijdag).  
- Timeslot moet altijd geldig zijn, de validatie hiervoor gebeurd in het object Timeslots. 
- Een tijdslot kan enkel overlappen met een ander tijdslot als ze op dezelfde dag plaatsvinden. 

[ScheduledTimeslot validatie tests](HorsesForCourses.Tests/Tests/Planning/ScheduledTimeSlotTests.cs)


## Coach
Invarianten: Een competentie kan niet leeg zijn en mag niet dubbel toegevoegd worden. Een competentie moet aanwezig zijn om te worden verwijderd.

