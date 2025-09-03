# Maak een plan 

**Doel:** in kleine, **verticale** stappen een **werkende** MVC-feature opleveren.  

Liever één volledig afgewerkte feature dan drie half werkende stukjes.

**Context:** Bestaande WebAPI blijft werken. MVC komt erbij en **hergebruikt de Service-laag**. Geen duplicatie van domein-/query-/repositorycode.

## Probeer:

* **Verticale slice telkens:** UI (View) => Controller => Service => Repo/Db => terug naar View.
* **WIP-limiet:** max 1 feature tegelijk.
* **Trunk-based of korte feature branches:** klein en vaak integreren.
* **Kleine commits:** elke commit houdt de build groen.
* **CI lokaal simuleren:** `dotnet test` en run de app **voordat** je pusht.
* **Definition of Done** (DoD) is hard; **stop zodra gehaald**.

## Definition of Done (per feature)

Een feature is **Done** wanneer:

1. **Functioneel:** werkt end-to-end in de UI (happy path + relevante fouten).
2. **Dun MVC:** controller bevat **geen** businesslogica; service/queries doen het werk.
3. **Controller Tests:**
   * Unit/integration tests voor Service (en Repo waar zinnig),
   * Controller test (result type, model, redirects, statuscodes).
4. **Validatie & feedback:** invalid input toont duidelijke foutmelding in de View.
5. **Anti-forgery:** POST-acties beschermd.
6. **Routing & navigatie:** link vanaf Index en back-navigatie aanwezig.
7. **Codekwaliteit:** geen duplicatie, duidelijke namen, dode code weg.

> **Stopcriterium:** Zodra DoD gehaald is => **mergen** => volgende slice kiezen.


## Aanpak in *Slices* (Coach Controller voorbeeld, XP style)

>  Kwaliteit > kwantiteit.

### Story 1: Project & DI sanity
* **Doel:** MVC start op, dependencies kloppen, Service-laag bereikbaar.
* **Taken:**
  * Projectreferenties: MVC => Service => Core.
  * DI registraties gelijk aan Api (DbContext, queries, services).
  * Health check route `/health` of gewoon de Home-pagina.
* **DoD:** app start zonder exceptions; `dotnet test` groen.


### Story 2: Coaches **Index** (lijst) (First Steps)
* **Doel:** Eenvoudige lijst van coaches tonen via Service-laag.
* **Acceptatie Criteria:**
  * GET `/Coaches` toont namen + e-mails.
  * Data komt uit Service.
  * Lege lijst toont "Geen coaches".
* **UI:** link naar detail (mag nog 404 linken).
* **DoD:** lijst werkt end-to-end; geen domeinobjecten direct naar View, maar readmodel.


### Story 3: **Register Coach** (GET/POST, validatie, anti-forgery)
* **Doel:** Coach kunnen registreren met feedback op fouten.
* **Acceptatie Criteria:**
  * GET `/Coaches/Register` toont formulier (naam, e-mail).
  * POST met geldige data => redirect naar Index + coach zichtbaar.
  * Ongeldige data (bv. domeinregel) => zelfde view met foutmelding.
  * Anti-forgery actief.
* **UI:** teruglink naar Index.
* **DoD:** volledig werkende registratie met duidelijke foutmeldingen.


### Story 4: **Coach Detail** (read-only)
* **Doel:** Detailpagina met basisinfo.
* **Acceptatie Criteria:**
  * GET `/Coaches/{id}` toont naam, e-mail, (optioneel) skills/cursussen.
  * Niet-gevonden => 404 pagina.
* **DoD:** link vanuit Index werkt; 404 netjes.


### Story 5: **Update Skills** (GET/POST)
* **Doel:** Skills bewerken voor een coach.
* **Acceptatie Criteria:**
  * GET `/Coaches/{id}/UpdateSkills` toont huidige skills.
  * POST met lijst skills => bewaart via Service (bv. `UpdateSkills` use case).
  * Validatiefouten tonen in de view (dup/lege skill filter je in Service).
* **DoD:** round-trip bewerken + feedback.


### Story 6: **Paging op Index** - OK
* **Doel:** Grote lijsten bruikbaar houden.
* **Acceptatie Criteria:**
  * Query accepteert `PageRequest(page, pageSize)`.
  * UI: eenvoudige "Vorige/Volgende".
* **DoD:** lijst navigeert pagina’s; parameters behouden in links.


### Story 7: **Foutafhandeling & UX-polish**
* **Doel:** Consistente fouten en feedback.
* **Acceptatie Criteria:**
  * TempData/ValidationSummary consistent.
  * Algemene errorpagina voor 500; nette 404.
* **DoD:** merkbare verbetering zonder logica in controllers.


## Wat we **niet** doen:
* Geen domeinlogica in MVC controllers of Views.
* Geen directe EF-calls vanuit MVC.
* Geen "quick hacks" die de Service-laag omzeilen.
* Geen halve features mergen. **Stop bij DoD.**


## Done? Kies de volgende story bewust

* Heeft de volgende story **één** duidelijk gebruikersresultaat?
* Past hij binnen de resterende tijd + DoD?
* Zo niet: **kleiner maken** of **overslaan**.


