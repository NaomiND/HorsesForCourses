1. - De [invarianten](../1.TheStables/readme.md) zoals geïmplementeerd in het domein, dienen door minstens één test gedocumenteerd te worden (meerdere liefst om *edge-cases* te verduidelijken).
   - Een markdown document dient te worden opgeleverd aan de hoofd aandeelhouder via een bestand in de root van de *solution*: `domain-invariants.md`. Daarin voor elke *invariant*:
     - een korte beschrijving 
     - link(s) naar de test code

# Invariants
## EmailAddress
Een geldig e-mailadres mag niet leeg of 'null' zijn en kan enkel via de 'create()'-methode worden aangemaakt.
De structuur van het e-mailadres wort gevalideerd met 'System.Net.Mail.MailAddress'. Deze klasse controleert of een e-mailadres syntactisch juist is volgens een eenvoudige subset van de e-mailstandaarden (niet volledig RFC 5322). 

[E-mailvalidatie tests](HorsesForCourses.Tests/CoachRegistrationTests.cs)Aan