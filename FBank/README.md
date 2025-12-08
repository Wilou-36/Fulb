Here’s an improved version of the README.md file that incorporates the new content while maintaining the existing structure and information. The flow and coherence of the document have been enhanced for better readability.


# Nom du Projet

## Description

Ce projet est une application de gestion qui permet aux administrateurs de se connecter et de gérer les données via une interface utilisateur.

## Fonctionnalités implémentées

Ce projet contient les éléments suivants, déjà mis en oeuvre :

- **Connexion à une base de données MySQL via une classe utilitaire `DbManager` (`FBank\database\db.cs`).**
  - `DbManager` lit prioritairement la chaîne de connexion nommée `DefaultConnection` dans `App.config` (provider `MySql.Data.MySqlClient`).
  - Si `DefaultConnection` est absente, `DbManager` construit la chaîne depuis les variables d'environnement : `DB_HOST`, `DB_PORT`, `DB_NAME`, `DB_USER`, `DB_PASS`, `DB_CHARSET`.
  - Méthodes exposées : `GetConnection()`, `TestConnection(out string)`, `ExecuteQuery(...)`, `ExecuteNonQuery(...)`, `ExecuteScalar(...)`.

- **Formulaire d'authentification administrateur (`FBank\loginAdmin.cs`).**
  - Contrôles attendus : `textBox1` (nom), `txtPass` (mot de passe), `button1` / `btnConnexion` (bouton de connexion).
  - Lors de la connexion, `AuthenticateAdmin` exécute une requête paramétrée vers la table `admin` :
    - Requête utilisée : `SELECT COUNT(*) FROM admin WHERE nom = @nom AND mot_de_passe_hash = @pwd`.
  - Si l'authentification réussie, ouverture de `PageAdmin` et masquage du formulaire de connexion.
  - Le `TextBox` `txtPass` masque les caractères saisis (propriété `PasswordChar` = `'*'`) ; la police des `TextBox` est forcée à 12pt au runtime.

- **Exemple de configuration et schéma de base de données :**

### Exemples SQL


CREATE TABLE admin (
  id_admin INT UNSIGNED NOT NULL AUTO_INCREMENT,
  nom VARCHAR(100) NOT NULL,
  mot_de_passe_hash VARCHAR(255) NOT NULL,
  date_creation DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id_admin)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Exemple d'insertion (mot de passe en clair dans l'exemple: remplacez par un hachage sécurisé)
INSERT INTO admin (nom, mot_de_passe_hash) VALUES ('will', '1234');


Remarque : le champ `mot_de_passe_hash` doit contenir un hachage sécurisé (PBKDF2 / bcrypt / Argon2). La colonne est définie ici avec une longueur 255 pour stocker des hachés modernes.

- **Fichier `App.config` (optionnel) :**


<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <connectionStrings>
    <!-- Exemple MySQL (prioritaire si présent) -->
    <add name="DefaultConnection" connectionString="Server=192.168.56.154;Port=3306;Database=Fulb;Uid=Fulb;Pwd=Fulb;Charset=utf8mb4;SslMode=Preferred;" providerName="MySql.Data.MySqlClient" />
  </connectionStrings>
</configuration>


- **Variables d'environnement prises en charge (utilisées si `DefaultConnection` absente) :**
  - `DB_HOST` (ex. `192.168.56.154`)
  - `DB_PORT` (ex. `3306`)
  - `DB_NAME` (ex. `Fulb`)
  - `DB_USER` (ex. `Fulb`)
  - `DB_PASS` (ex. `Fulb`)
  - `DB_CHARSET` (ex. `utf8mb4`)

- **Dépendances NuGet :**
  - `MySql.Data` — installer via __Tools > NuGet Package Manager__ ou `Install-Package MySql.Data`.

## Instructions d'installation et exécution

1. Installer le package NuGet `MySql.Data` si nécessaire.
2. Configurer la connexion :
   - **Option A (recommandée en développement)** : modifier `App.config` et définir `DefaultConnection`.
   - **Option B** : définir les variables d'environnement listées ci-dessus.
3. S'assurer que la table `admin` existe et contient au moins un compte administrateur.
4. Lancer l'application ; utiliser l'écran de connexion (`loginAdmin`) pour se connecter en tant qu'administrateur.

## Sécurité et recommandations

- Ne stockez jamais de mots de passe en clair. Remplacez immédiatement le stockage actuel par un hachage sécurisé (PBKDF2, bcrypt ou Argon2) et mettez à jour les méthodes d'authentification pour comparer les hachés.
- Activez TLS/SSL entre l'application et la base de données si possible (paramètre `SslMode` dans la chaîne de connexion MySQL).
- Protégez `App.config` et les éventuels fichiers contenant des secrets ; privilégiez l'utilisation de variables d'environnement en production.
- Validez et nettoyez toujours les entrées utilisateurs côté serveur.

## Points à améliorer / prochaines étapes

- Migration des mots de passe vers un système de hachage sécurisé.
- Méthodes async (`ExecuteQueryAsync`, etc.) pour améliorer la réactivité UI.
- Centraliser la gestion d'utilisateur (CRUD) et l'interface d'administration (`PageAdmin`).
- Ajout d'audit/log et gestion des erreurs centralisée.

## Comment démarrer en développement

1. **Installation des dépendances :**
   - Ouvrez le terminal et exécutez la commande suivante pour installer le package NuGet :
     ```bash
     Install-Package MySql.Data
     ```

2. **Configuration de l'environnement :**
   - Assurez-vous que les variables d'environnement nécessaires sont définies, ou modifiez le fichier `App.config` pour inclure votre chaîne de connexion.

3. **Lancement de l'application :**
   - Compilez et exécutez le projet dans votre IDE. Utilisez l'écran de connexion pour tester l'authentification.

## Intégration du hachage sécurisé

- Implémentez le hachage PBKDF2/bcrypt pour sécuriser les mots de passe. Assurez-vous de migrer les comptes existants vers ce nouveau système de hachage.

## Documentation des contrôles WinForms

- Pour faciliter le travail avec le designer, une documentation des contrôles WinForms (noms et événements) sera ajoutée dans une version future.

---

Si vous avez des questions ou des suggestions, n'hésitez pas à les partager !


This revised README.md file maintains the original structure while enhancing clarity and coherence. It also includes the new sections and suggestions for future improvements, making it more comprehensive for users and developers.