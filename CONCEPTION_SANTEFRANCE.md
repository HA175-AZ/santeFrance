# Document de Conception - SanteFrance

## ?? Table des matières
1. [Vue d'ensemble du projet](#vue-densemble-du-projet)
2. [Architecture technique](#architecture-technique)
3. [Modèles de données](#modèles-de-données)
4. [Contrôleurs et Routes](#contrôleurs-et-routes)
5. [Vues et Interface utilisateur](#vues-et-interface-utilisateur)
6. [Sécurité et Authentification](#sécurité-et-authentification)
7. [Fonctionnalités principales](#fonctionnalités-principales)
8. [Gestion des fichiers](#gestion-des-fichiers)
9. [Diagrammes](#diagrammes)
10. [Technologies utilisées](#technologies-utilisées)
11. [Points d'amélioration futurs](#points-damélioration-futurs)
12. [Contact et Maintenance](#contact-et-maintenance)

---

## ?? Vue d'ensemble du projet

### Description
**SanteFrance** est une plateforme web de gestion de rendez-vous médicaux destinée aux étudiants internationaux en France. Elle permet de :
- Rechercher et consulter des médecins
- Prendre des rendez-vous en ligne avec calendrier interactif
- Gérer son profil étudiant
- Administrer la plateforme (médecins, étudiants, rendez-vous)

### Public cible
- **Étudiants internationaux** : utilisateurs principaux cherchant des soins médicaux
- **Administrateurs** : gestionnaires de la plateforme

### Objectifs
- Faciliter l'accès aux soins pour les étudiants étrangers
- Centraliser les informations sur les médecins (langues, spécialités, tiers-payant)
- Simplifier la prise de rendez-vous
- Fournir des ressources sur le système de santé français

---

## ??? Architecture technique

### Type de projet
**ASP.NET Core MVC** (.NET 8 / C# 12.0)

### Pattern architectural
**MVC (Model-View-Controller)** avec Entity Framework Core pour l'accès aux données

### Structure des dossiers
santeFrance/ ??? Controllers/          # Logique métier et actions ?   ??? AccountController.cs ?   ??? AdminController.cs ?   ??? HomeController.cs ?   ??? StudentController.cs ??? Models/              # Entités et modèles de données ?   ??? Admin.cs ?   ??? Medecin.cs ?   ??? RendezVous.cs ?   ??? User.cs ?   ??? ErrorViewModel.cs ??? Views/               # Interface utilisateur (Razor) ?   ??? Account/ ?   ?   ??? Login.cshtml ?   ?   ??? Register.cshtml ?   ??? Admin/ ?   ?   ??? Dashboard.cshtml ?   ?   ??? Medecins.cshtml ?   ?   ??? AjouterMedecin.cshtml ?   ?   ??? ModifierMedecin.cshtml ?   ?   ??? Etudiants.cshtml ?   ?   ??? RendezVous.cshtml ?   ??? Home/ ?   ?   ??? Index.cshtml ?   ?   ??? Medecins.cshtml ?   ?   ??? ComprendreLaSante.cshtml ?   ?   ??? SecuriteSociale.cshtml ?   ?   ??? Urgences.cshtml ?   ?   ??? SanteMentale.cshtml ?   ?   ??? Privacy.cshtml ?   ??? Student/ ?   ?   ??? Dashboard.cshtml ?   ?   ??? Profil.cshtml ?   ?   ??? MesDemarches.cshtml ?   ?   ??? MesRendezVous.cshtml ?   ?   ??? PrendreRendezVous.cshtml ?   ?   ??? CalendrierMedecin.cshtml ?   ??? Shared/ ?       ??? _Layout.cshtml ?       ??? _Footer.cshtml ?       ??? Error.cshtml ?       ??? _ValidationScriptsPartial.cshtml ?       ??? _ViewImports.cshtml ?       ??? _ViewStart.cshtml ??? Data/                # Contexte de base de données ?   ??? ApplicationDbContext.cs ??? Migrations/          # Migrations Entity Framework ?   ??? 20260204222913_InitialCreateWithTypeRdv.cs ??? wwwroot/            # Ressources statiques ?   ??? css/ ?   ?   ??? site.css ?   ?   ??? student.css ?   ??? js/ ?   ??? lib/ ?   ??? uploads/        # Photos uploadées ?       ??? medecins/ ?       ??? photos/ ??? Program.cs          # Point d'entrée et configuration ??? appsettings.json    # Configuration ??? .gitignore

---

## ?? Modèles de données

### Diagramme relationnel
???????????????????????         ????????????????????????         ??????????????????????? ?      User           ?         ?    RendezVous        ?         ?      Medecin        ? ???????????????????????         ????????????????????????         ??????????????????????? ? Id (PK)             ??????????? UserId (FK)          ?         ? Id (PK)             ? ? Nom                 ?         ? MedecinId (FK)       ??????????? Nom                 ? ? Prenom              ?         ? DateHeure            ?         ? Prenom              ? ? Email               ?         ? DureeMinutes         ?         ? Specialite          ? ? MotDePasse          ?         ? TypeRdv              ?         ? Adresse             ? ? Telephone           ?         ? Statut               ?         ? Ville               ? ? DateNaissance       ?         ? Motif                ?         ? CodePostal          ? ? Nationalite         ?         ? Notes                ?         ? Telephone           ? ? Universite          ?         ? DateCreation         ?         ? Email               ? ? NumeroSecu          ?         ? DateModification     ?         ? Langues             ? ? ACarteVitale        ?         ????????????????????????         ? AccepteCarteVitale  ? ? Adresse             ?                                          ? TiersPayant         ? ? CodePostal          ?                                          ? PhotoUrl            ? ? Ville               ?                                          ? Description         ? ? PhotoUrl            ?                                          ? Horaires            ? ? DateInscription     ?                                          ? EstActif            ? ???????????????????????                                          ? DateAjout           ? ???????????????????????
??????????????????????? ?      Admin          ? ??????????????????????? ? Id (PK)             ? ? Nom                 ? ? Prenom              ? ? Email               ? ? MotDePasse          ? ? Role                ? ? EstActif            ? ? DateCreation        ? ? DerniereConnexion   ? ???????????????????????

### 1. **User** (Étudiant)
Représente un étudiant inscrit sur la plateforme.

| Propriété | Type | Contrainte | Description |
|-----------|------|------------|-------------|
| Id | int | PK | Identifiant unique |
| Nom | string(100) | Required | Nom de famille |
| Prenom | string(100) | Required | Prénom |
| Email | string | Required, EmailAddress | Email unique |
| MotDePasse | string | Required | Hash BCrypt |
| Telephone | string | Phone, Optional | Numéro de téléphone |
| DateNaissance | DateTime | Required | Date de naissance |
| Nationalite | string(100) | Required | Pays d'origine |
| Universite | string(200) | Optional | Établissement |
| NumeroSecu | string(20) | Optional | Numéro sécu française |
| ACarteVitale | bool | Default: false | Possession carte vitale |
| Adresse | string(200) | Optional | Adresse postale |
| CodePostal | string(10) | Optional | Code postal |
| Ville | string(100) | Optional | Ville |
| PhotoUrl | string(500) | Optional | Chemin photo profil |
| DateInscription | DateTime | Auto | Date de création compte |
| RendezVous | ICollection | Navigation | Liste des RDV |

### 2. **Medecin**
Représente un praticien disponible sur la plateforme.

| Propriété | Type | Contrainte | Description |
|-----------|------|------------|-------------|
| Id | int | PK | Identifiant unique |
| Nom | string(100) | Required | Nom de famille |
| Prenom | string(100) | Required | Prénom |
| Specialite | string(150) | Required | Spécialité médicale |
| Adresse | string(200) | Required | Adresse cabinet |
| Ville | string(100) | Required | Ville |
| CodePostal | string(10) | Required | Code postal |
| Telephone | string(20) | Phone, Optional | Téléphone cabinet |
| Email | string | EmailAddress, Optional | Email contact |
| Langues | string | Required | Langues parlées (CSV) |
| AccepteCarteVitale | bool | Default: true | Accepte carte vitale |
| TiersPayant | bool | Default: false | Pratique tiers-payant |
| PhotoUrl | string(500) | Optional | Photo du médecin |
| Description | string(1000) | Optional | Présentation |
| Horaires | string | Optional | Horaires d'ouverture |
| EstActif | bool | Default: true | Médecin actif |
| DateAjout | DateTime | Auto | Date d'ajout |
| RendezVous | ICollection | Navigation | Liste des RDV |

### 3. **RendezVous**
Représente un rendez-vous entre un étudiant et un médecin.

| Propriété | Type | Contrainte | Description |
|-----------|------|------------|-------------|
| Id | int | PK | Identifiant unique |
| UserId | int | FK, Required | Référence User |
| MedecinId | int | FK, Required | Référence Medecin |
| DateHeure | DateTime | Required | Date et heure du RDV |
| DureeMinutes | int | Default: 30 | Durée en minutes |
| TypeRdv | string(20) | Default: "Sur place" | Type (Sur place/Téléconsultation) |
| Statut | string(50) | Default: "En attente" | Statut du RDV |
| Motif | string(500) | Optional | Raison de consultation |
| Notes | string(1000) | Optional | Notes complémentaires |
| DateCreation | DateTime | Auto | Date de création |
| DateModification | DateTime | Optional | Dernière modification |
| User | User | Navigation | Étudiant |
| Medecin | Medecin | Navigation | Médecin |

**Statuts possibles** : `En attente`, `Confirmé`, `Annulé`, `Terminé`

### 4. **Admin**
Représente un administrateur de la plateforme.

| Propriété | Type | Contrainte | Description |
|-----------|------|------------|-------------|
| Id | int | PK | Identifiant unique |
| Nom | string(100) | Required | Nom |
| Prenom | string(100) | Required | Prénom |
| Email | string | Required, EmailAddress | Email de connexion |
| MotDePasse | string | Required | Hash BCrypt |
| Role | string(50) | Default: "Admin" | Rôle (Admin/SuperAdmin) |
| EstActif | bool | Default: true | Compte actif |
| DateCreation | DateTime | Auto | Date de création |
| DerniereConnexion | DateTime | Optional | Dernière connexion |

---

## ?? Contrôleurs et Routes

### 1. **AccountController**
Gestion de l'authentification et des sessions.

| Action | Méthode | Route | Description |
|--------|---------|-------|-------------|
| Register | GET | `/Account/Register` | Affiche formulaire inscription |
| Register | POST | `/Account/Register` | Traite inscription étudiant |
| Login | GET | `/Account/Login` | Affiche formulaire connexion |
| Login | POST | `/Account/Login` | Authentifie utilisateur/admin |
| Logout | GET | `/Account/Logout` | Déconnexion |

**Sécurité** :
- Hash des mots de passe avec **BCrypt.Net**
- Sessions ASP.NET Core (`HttpContext.Session`)
- Distinction Admin/Étudiant via paramètre `userType`

---

### 2. **HomeController**
Pages publiques accessibles sans authentification.

| Action | Méthode | Route | Description |
|--------|---------|-------|-------------|
| Index | GET | `/` | Page d'accueil |
| ComprendreLaSante | GET | `/Home/ComprendreLaSante` | Guides santé |
| Medecins | GET | `/Home/Medecins` | Recherche médecins publique |
| SecuriteSociale | GET | `/Home/SecuriteSociale` | Infos sécu sociale |
| Urgences | GET | `/Home/Urgences` | Numéros d'urgence |
| SanteMentale | GET | `/Home/SanteMentale` | Ressources santé mentale |
| Privacy | GET | `/Home/Privacy` | Politique confidentialité |
| Error | GET | `/Home/Error` | Page d'erreur |

**Filtres de recherche médecins** :
- `search` : Nom, prénom, spécialité
- `ville` : Ville ou code postal
- `langue` : Langue parlée
- `tiersPayant` : true/false

---

### 3. **StudentController**
Espace étudiant (authentification requise).

| Action | Méthode | Route | Description |
|--------|---------|-------|-------------|
| Dashboard | GET | `/Student/Dashboard` | Tableau de bord étudiant |
| Profil | GET | `/Student/Profil` | Affiche profil |
| UpdateProfil | POST | `/Student/UpdateProfil` | Modifie profil |
| UploadPhoto | POST | `/Student/UploadPhoto` | Upload photo profil |
| SupprimerPhoto | POST | `/Student/SupprimerPhoto` | Supprime photo |
| MesDemarches | GET | `/Student/MesDemarches` | Ressources démarches |
| MesRendezVous | GET | `/Student/MesRendezVous` | Liste RDV étudiant |
| AnnulerRendezVous | POST | `/Student/AnnulerRendezVous` | Annule RDV (JSON) |
| PrendreRendezVous | GET | `/Student/PrendreRendezVous?medecinId=X` | Formulaire RDV |
| PrendreRendezVous | POST | `/Student/PrendreRendezVous` | Crée RDV |
| CalendrierMedecin | GET | `/Student/CalendrierMedecin?medecinId=X` | Vue calendrier |
| GetCalendarEvents | GET | `/Student/GetCalendarEvents?medecinId=X` | Événements calendrier (JSON) |
| PrendreRendezVousCalendrier | POST | `/Student/PrendreRendezVousCalendrier` | Crée RDV depuis calendrier (JSON) |

**Validations RDV** :
- Empêche RDV dans le passé
- Détecte conflits (médecin/étudiant occupé)
- Annulation impossible si RDV déjà passé

---

### 4. **AdminController**
Panneau d'administration (authentification admin requise).

| Action | Méthode | Route | Description |
|--------|---------|-------|-------------|
| Dashboard | GET | `/Admin/Dashboard` | Statistiques globales |
| Medecins | GET | `/Admin/Medecins` | Liste médecins |
| AjouterMedecin | GET | `/Admin/AjouterMedecin` | Formulaire ajout |
| AjouterMedecin | POST | `/Admin/AjouterMedecin` | Crée médecin |
| ModifierMedecin | GET | `/Admin/ModifierMedecin/5` | Formulaire modification |
| ModifierMedecin | POST | `/Admin/ModifierMedecin/5` | Modifie médecin |
| SupprimerPhotoMedecin | POST | `/Admin/SupprimerPhotoMedecin?id=5` | Supprime photo (JSON) |
| SupprimerMedecin | POST | `/Admin/SupprimerMedecin/5` | Supprime médecin (JSON) |
| Etudiants | GET | `/Admin/Etudiants` | Liste étudiants |
| RendezVous | GET | `/Admin/RendezVous` | Liste tous RDV |
| ChangerStatutRdv | POST | `/Admin/ChangerStatutRdv` | Change statut RDV (JSON) |

**Statistiques Dashboard** :
- Nombre total de médecins
- Nombre total d'étudiants
- Nombre total de RDV
- RDV en attente

---

## ??? Vues et Interface utilisateur

### Pages publiques (Layout principal)

#### `/Views/Home/Index.cshtml`
- Présentation de la plateforme
- Appels à l'action (inscription, recherche médecin)
- Navigation vers ressources santé

#### `/Views/Home/Medecins.cshtml`
- Barre de recherche multi-critères
- Grille/Liste de médecins avec :
  - Photo, nom, spécialité
  - Adresse, ville
  - Langues, tiers-payant
  - Bouton "Prendre RDV"

#### Pages informatives
- `/Views/Home/ComprendreLaSante.cshtml` : Guides santé
- `/Views/Home/SecuriteSociale.cshtml` : Infos sécu
- `/Views/Home/Urgences.cshtml` : Numéros urgence
- `/Views/Home/SanteMentale.cshtml` : Ressources psychologiques

---

### Espace Étudiant

#### `/Views/Student/Dashboard.cshtml`
- Message de bienvenue personnalisé
- Résumé des 5 prochains RDV
- Accès rapides (Profil, Mes RDV, Médecins)

#### `/Views/Student/Profil.cshtml`
- Photo de profil (upload/suppression)
- Formulaire édition informations personnelles
- Statistiques (nombre de RDV)

#### `/Views/Student/MesRendezVous.cshtml`
- Liste complète des RDV (passés/futurs)
- Filtres par statut
- Actions : Annuler (si futur)

#### `/Views/Student/CalendrierMedecin.cshtml`
- **Calendrier interactif** (FullCalendar.js)
- Affiche créneaux occupés en rouge
- Clic sur créneau libre ? Modal prise RDV
- Validation en temps réel

---

### Espace Administration

#### `/Views/Admin/Dashboard.cshtml`
- 4 cartes statistiques (médecins, étudiants, RDV, en attente)
- Graphiques (optionnel)
- Liens vers gestion

#### `/Views/Admin/Medecins.cshtml`
- Tableau des médecins avec photo
- Actions : Modifier, Supprimer
- Bouton "+ Ajouter un médecin"

#### `/Views/Admin/AjouterMedecin.cshtml`
- Formulaire complet avec sections :
  - ?? Photo
  - ?? Infos personnelles
  - ?? Contact
  - ?? Infos complémentaires

#### `/Views/Admin/ModifierMedecin.cshtml`
- Similaire à AjouterMedecin
- Pré-rempli avec données existantes
- Option suppression photo
- Bouton suppression médecin

#### `/Views/Admin/Etudiants.cshtml`
- Tableau étudiants avec :
  - Photo, nom, email
  - Université, nationalité
  - Nombre de RDV

#### `/Views/Admin/RendezVous.cshtml`
- Tableau tous RDV avec :
  - Étudiant, médecin
  - Date/heure, type, statut
  - Sélecteur de statut (En attente ? Confirmé/Annulé/Terminé)

---

### Layout commun

#### `/Views/Shared/_Layout.cshtml`
- Header avec navigation (Home, Médecins, Comprendre la santé)
- Menu conditionnel selon authentification :
  - **Non connecté** : Connexion, Inscription
  - **Étudiant** : Dashboard, Profil, Mes RDV, Déconnexion
  - **Admin** : Panel Admin, Déconnexion
- Footer avec liens légaux

#### `/Views/Shared/_Footer.cshtml`
- Informations de contact
- Liens utiles
- Mentions légales

---

## ?? Sécurité et Authentification

### Gestion des sessions
- **Technologie** : `HttpContext.Session` (ASP.NET Core)
- **Données stockées** :
  - `UserId` / `AdminId` : Identifiant
  - `UserName` / `AdminName` : Nom complet
  - `UserEmail` / `AdminEmail` : Email
  - `UserType` : "Etudiant" ou "Admin"

### Méthodes de protection

#### Dans StudentController
private bool IsLoggedIn() { return HttpContext.Session.GetInt32("UserId") != null; }
private async Task<User?> GetCurrentUser() { var userId = HttpContext.Session.GetInt32("UserId"); if (userId == null) return null; return await _context.Users.FindAsync(userId); }

#### Dans AdminController
private bool IsAdminLoggedIn() { return HttpContext.Session.GetInt32("AdminId") != null; }

### Protection des routes
**Toutes** les actions des contrôleurs `Student` et `Admin` vérifient l'authentification :
if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

### Hachage des mots de passe
- **Bibliothèque** : BCrypt.Net
- **Enregistrement** : `BCrypt.HashPassword(motDePasse)`
- **Vérification** : `BCrypt.Verify(motDePasse, hash)`

### Protection CSRF
- Utilisation de `[ValidateAntiForgeryToken]` sur toutes les actions POST modifiant des données
- Token anti-forgery dans les formulaires Razor

### Validation des entrées
- Validation côté client avec jQuery Validation
- Validation côté serveur avec Data Annotations
- Protection contre les injections SQL via Entity Framework (requêtes paramétrées)
- Échappement automatique des sorties Razor (protection XSS)

---

## ?? Fonctionnalités principales

### 1. Recherche de médecins
- **Localisation** : Recherche par ville ou code postal
- **Spécialité** : Filtrage par type de praticien
- **Langues** : Important pour étudiants internationaux
- **Options** : Tiers-payant, carte vitale
- **Résultats** : Affichage avec photo, contact, horaires

### 2. Prise de rendez-vous

#### Mode formulaire classique
1. Sélection médecin
2. Choix date/heure manuel
3. Saisie type RDV et motif
4. Validation

#### Mode calendrier interactif
1. Affichage agenda du médecin (FullCalendar.js)
2. Créneaux occupés en rouge
3. Clic sur créneau libre ? Modal
4. Validation instantanée avec détection conflits

### 3. Gestion du profil étudiant
- Modification informations personnelles
- Upload photo profil (JPG, PNG, GIF, WebP, max 5 Mo)
- Ajout numéro sécurité sociale
- Mise à jour coordonnées

### 4. Gestion des RDV
- Visualisation liste complète
- Annulation (si futur et non annulé)
- Historique des RDV passés
- Statuts colorés (En attente, Confirmé, Annulé, Terminé)

### 5. Administration

#### Gestion médecins
- CRUD complet
- Upload photo médecin
- Gestion langues (CSV)
- Activation/désactivation

#### Gestion étudiants
- Consultation liste
- Statistiques par étudiant

#### Gestion RDV
- Vue globale tous RDV
- Changement de statut
- Filtres et tri

---

## ?? Gestion des fichiers

### Upload de photos

#### Médecins
- **Dossier** : `/wwwroot/uploads/medecins/`
- **Nommage** : `med_{GUID}.{extension}`
- **Formats acceptés** : .jpg, .jpeg, .png, .gif, .webp
- **Taille max** : 5 Mo
- **Gestion** :
  - Upload lors ajout/modification
  - Suppression ancienne photo lors remplacement
  - Suppression lors suppression médecin

#### Étudiants
- **Dossier** : `/wwwroot/uploads/photos/`
- **Nommage** : `user_{userId}_{GUID}.{extension}`
- **Formats acceptés** : .jpg, .jpeg, .png, .gif, .webp
- **Taille max** : 5 Mo

### Code de gestion (AdminController)
private async Task<string> SaveMedecinPhoto(IFormFile photo) { var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" }; var extension = Path.GetExtension(photo.FileName).ToLowerInvariant();
if (!allowedExtensions.Contains(extension))
    throw new InvalidOperationException("Format non supporté");

var uploadsDir = Path.Combine(_environment.WebRootPath, "uploads", "medecins");
if (!Directory.Exists(uploadsDir))
    Directory.CreateDirectory(uploadsDir);

var fileName = $"med_{Guid.NewGuid():N}{extension}";
var filePath = Path.Combine(uploadsDir, fileName);

using (var stream = new FileStream(filePath, FileMode.Create))
{
    await photo.CopyToAsync(stream);
}

return $"/uploads/medecins/{fileName}";
}
private void DeletePhotoFile(string photoUrl) { var filePath = Path.Combine(_environment.WebRootPath, photoUrl.TrimStart('/')); if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath); }

---

## ?? Diagrammes

### Flux d'authentification
???????????????????????????????????????????????????????????????? ?                    Flux d'Authentification                    ? ????????????????????????????????????????????????????????????????
[Utilisateur] ??? /Account/Login
       ?
       ?
???????????????????
? Type utilisateur??
???????????????????
          ?
  ?????????????????
  ?               ?
Étudiant         Admin ?               ? ?               ? [Vérif User]   [Vérif Admin] ?               ? ?               ? ??????????      ?????????? ?Valide? ?      ?Valide? ? ??????????      ?????????? ?               ? Oui ? Non       Oui ? Non ?  ?            ?  ? ?  ??????????????  ? ?       ?          ? ?   [Erreur]       ? ?                  ? ?                  ? [Session UserId]  [Session AdminId] ?                  ? ?                  ? [Student/Dashboard] [Admin/Dashboard]

### Flux de prise de RDV avec calendrier
???????????????????????????????????????????????????????????????? ?              Flux de Prise de Rendez-vous                     ? ????????????????????????????????????????????????????????????????
[Étudiant clique médecin] ? ? [Student/CalendrierMedecin] ? ? [Chargement AJAX GetCalendarEvents] ? ? [Affichage créneaux occupés (rouge)] ? ? ???????????????? ? Clic créneau?? ???????????????? ? ??????????? ?         ? Occupé     Libre ?         ? ?         ? [Bloqué]  [Modal] ? ? [Type RDV + Motif] ? ? [POST PrendreRendezVousCalendrier] ? ? ????????????????? ?  Validations  ? ?- Date passée? ? ?- Conflit?     ? ????????????????? ? ????????????? ?           ? Erreur        OK ?           ? ?           ? [Retour]  [Création BDD] ? ? [Success JSON] ? ? [Rechargement calendrier]

### Architecture MVC
?????????????????????????????????????????????????????????????????? ?                     Architecture MVC                           ? ??????????????????????????????????????????????????????????????????
?????????????
?Navigateur ?
?????????????
      ? HTTP Request
      ?
????????????????
? Contrôleur   ?
? - Account    ?
? - Home       ?
? - Student    ?
? - Admin      ?
????????????????
       ?
       ?
?????????????????????
?ApplicationDbContext?
?????????????????????
       ? Entity Framework Core
       ?
???????????????
? SQL Server  ?
? - Users     ?
? - Medecins  ?
? - RendezVous?
? - Admins    ?
???????????????
      ? Retour Entités
      ?
????????????????
?  Modèles     ?
? - User       ?
? - Medecin    ?
? - RendezVous ?
? - Admin      ?
????????????????
       ? ViewBag/Model
       ?
????????????????
? Vue Razor    ?
? - Layout     ?
? - Pages      ?
????????????????
       ? HTML/CSS/JS
       ?
?????????????
?Navigateur ?
?????????????

### Diagramme de classes simplifié
????????????????????              ???????????????????? ?      User        ?              ?     Medecin      ? ????????????????????              ???????????????????? ? + Id: int        ?              ? + Id: int        ? ? + Nom: string    ?              ? + Nom: string    ? ? + Prenom: string ?              ? + Prenom: string ? ? + Email: string  ?              ? + Specialite     ? ? + MotDePasse     ?              ? + Ville: string  ? ? + DateNaissance  ?              ? + Langues: string? ? + Nationalite    ?              ? + CarteVitale    ? ? + PhotoUrl       ?              ? + TiersPayant    ? ? + RendezVous     ?              ? + PhotoUrl       ? ????????????????????              ???????????????????? ?                                 ? ? 1                            1  ? ?                                 ? ??????????           ?????????????? ?           ? ?     *   * ? ????????????????????????? ?    RendezVous         ? ????????????????????????? ? + Id: int             ? ? + UserId: int (FK)    ? ? + MedecinId: int (FK) ? ? + DateHeure: DateTime ? ? + DureeMinutes: int   ? ? + TypeRdv: string     ? ? + Statut: string      ? ? + Motif: string       ? ? + DateCreation        ? ?????????????????????????
???????????????????? ?      Admin       ? ???????????????????? ? + Id: int        ? ? + Nom: string    ? ? + Email: string  ? ? + MotDePasse     ? ? + Role: string   ? ? + EstActif: bool ? ????????????????????

---

## ?? Technologies utilisées

### Backend
- **Langage** : C# 12.0
- **Framework** : ASP.NET Core 8.0 MVC
- **ORM** : Entity Framework Core 8.0
- **Base de données** : SQL Server (LocalDB en développement)
- **Authentification** : BCrypt.Net-Next 4.0
- **Sessions** : ASP.NET Core Session Middleware

### Frontend
- **Moteur de template** : Razor Views (.cshtml)
- **HTML5** avec sémantique moderne
- **CSS3** personnalisé (`site.css`, `student.css`)
- **JavaScript** : Vanilla JS + Fetch API
- **Calendrier** : FullCalendar.js v6
- **Validation client** : jQuery Validation + Unobtrusive Validation
- **Design** : CSS Flexbox/Grid + responsive

### Configuration (Program.cs)
var builder = WebApplication.CreateBuilder(args);
// Configuration de la base de données builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer( builder.Configuration.GetConnectionString("DefaultConnection") ));
// Configuration des sessions builder.Services.AddSession(options => { options.IdleTimeout = TimeSpan.FromMinutes(30); options.Cookie.HttpOnly = true; options.Cookie.IsEssential = true; options.Cookie.SecurePolicy = CookieSecurePolicy.Always; });
// Configuration MVC builder.Services.AddControllersWithViews();
var app = builder.Build();
// Configuration du pipeline HTTP if (!app.Environment.IsDevelopment()) { app.UseExceptionHandler("/Home/Error"); app.UseHsts(); }
app.UseHttpsRedirection(); app.UseStaticFiles(); app.UseRouting(); app.UseSession(); app.UseAuthorization();
app.MapControllerRoute( name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();

### Chaîne de connexion (appsettings.json)
{ "ConnectionStrings": { "DefaultConnection": "Server=(localdb)\mssqllocaldb;Database=SanteFranceDb;Trusted_Connection=true;MultipleActiveResultSets=true" }, "Logging": { "LogLevel": { "Default": "Information", "Microsoft.AspNetCore": "Warning" } }, "AllowedHosts": "*" }

### Packages NuGet installés
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" /> <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" /> <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.0" /> <PackageReference Include="BCrypt.Net-Next" Version="4.0.3" /> <PackageReference Include="Microsoft.AspNetCore.Session" Version="2.2.0" />

### Outils de développement
- **Visual Studio 2022** (v17.8 ou supérieur)
- **SQL Server Management Studio** (SSMS) 19.0
- **Git** 2.43 avec GitHub Desktop
- **Postman** v10.0 (tests API JSON)
- **Browser DevTools** (Chrome/Edge)

---

## ?? Points d'amélioration futurs

### Fonctionnalités
1. **Notifications** :
   - Emails de confirmation RDV (SendGrid/SMTP)
   - Rappels 24h avant par email
   - SMS via Twilio
   - Notifications push web

2. **Paiement en ligne** :
   - Intégration Stripe
   - Gestion paiements partiels
   - Remboursements
   - Factures PDF automatiques

3. **Messagerie** :
   - Chat en temps réel (SignalR)
   - Téléconsultation vidéo (WebRTC)
   - Partage de documents sécurisé
   - Historique conversations

4. **Multilangue** :
   - Ressources .resx
   - Français, Anglais, Espagnol, Arabe
   - Détection automatique navigateur
   - Sélecteur de langue

5. **Analytics** :
   - Tableau de bord statistiques avancé
   - Graphiques interactifs (Chart.js)
   - Export Excel/CSV
   - Prédictions ML (taux d'occupation)

6. **Géolocalisation** :
   - Carte interactive (Google Maps API)
   - Calcul itinéraire
   - Tri par distance
   - Géofencing notifications

### Technique
1. **API REST** :
   - Séparation frontend/backend
   - Endpoints documentés (Swagger)
   - Versioning API (v1, v2)
   - Application mobile (Xamarin/MAUI)

2. **Authentification moderne** :
   - JWT tokens (refresh + access)
   - OAuth2 (Google, Facebook, Microsoft)
   - Two-Factor Authentication (2FA)
   - Identity Server

3. **Tests** :
   - Tests unitaires (xUnit)
   - Tests d'intégration (WebApplicationFactory)
   - Tests E2E (Selenium)
   - Couverture > 80%

4. **CI/CD** :
   - GitHub Actions pipelines
   - Tests automatiques
   - Déploiement Azure App Service
   - Environnements (Dev/Staging/Prod)

5. **Sécurité renforcée** :
   - Rate limiting (AspNetCoreRateLimit)
   - Logs d'audit (Serilog)
   - Conformité RGPD
   - Certificats SSL Let's Encrypt
   - Content Security Policy (CSP)

6. **Performance** :
   - Cache distribué (Redis)
   - CDN pour assets statiques
   - Lazy loading images
   - Pagination côté serveur
   - Compression Gzip

---

## ?? Contact et Maintenance

### Informations projet
- **Nom** : SanteFrance
- **Version** : 1.0.0
- **Statut** : En développement actif
- **Licence** : À définir

### Dépôt Git
- **Repository** : https://github.com/HA175-AZ/santeFrance
- **Branche principale** : `main`
- **Branches secondaires** :
  - `develop` : développement en cours
  - `feature/*` : nouvelles fonctionnalités
  - `hotfix/*` : corrections urgentes

### Structure d'équipe recommandée
- **Chef de projet** : Planification, coordination
- **Backend Developer** : API, logique métier, base de données
- **Frontend Developer** : UI/UX, Razor, CSS/JS
- **DevOps Engineer** : Déploiement, monitoring, CI/CD
- **QA Tester** : Tests manuels/automatisés, validation

### Convention de commits
feat: Ajout nouvelle fonctionnalité fix: Correction bug docs: Modification documentation style: Formatage code (sans changement logique) refactor: Refactorisation code test: Ajout tests chore: Tâches maintenance (packages, config)

### Workflow Git
1. Créer branche depuis `develop`
2. Implémenter fonctionnalité
3. Commit réguliers avec messages clairs
4. Pull Request vers `develop`
5. Code review
6. Merge après validation
7. Déploiement vers `main` après tests

---

## ?? Licence et Droits

*À définir selon les besoins du projet (MIT, Apache 2.0, propriétaire, etc.)*

### Mentions légales
- Propriété intellectuelle : [À définir]
- Protection des données : Conformité RGPD
- Hébergement : [À définir]
- Responsable traitement données : [À définir]

---

## ?? Annexes

### Glossaire
- **CRUD** : Create, Read, Update, Delete
- **MVC** : Model-View-Controller
- **ORM** : Object-Relational Mapping
- **CSRF** : Cross-Site Request Forgery
- **XSS** : Cross-Site Scripting
- **JWT** : JSON Web Token
- **API** : Application Programming Interface
- **RGPD** : Règlement Général sur la Protection des Données

### Références
- [Documentation ASP.NET Core](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [BCrypt.Net](https://github.com/BcryptNet/bcrypt.net)
- [FullCalendar.js](https://fullcalendar.io/)

---

**Document généré le** : 15 avril 2026  
**Version** : 1.0  
**Projet** : SanteFrance - Plateforme de gestion de rendez-vous médicaux  
**Technologies** : ASP.NET Core 8.0, C# 12.0, Entity Framework Core, SQL Server  
**Auteur** : Équipe SanteFrance  
**Dernière mise à jour** : 15 avril 2026

