# 🏛 BanqueNet Pro — Documentation

> Application bancaire desktop développée en **C# WinForms (.NET 6)** avec **SQL Server**.  
> Architecture en couches : `Banque.UI` · `Banque.Business` · `Banque.Data` · `Banque.Models` · `Banque.Config`

---

## 📋 Sommaire

1. [Prérequis](#prérequis)
2. [Récupération du code](#récupération-du-code)
3. [Structure du projet](#structure-du-projet)
4. [Configuration de la base de données](#configuration-de-la-base-de-données)
   - [Méthode 1 — fichier appsettings.json](#méthode-1--fichier-appsettingsjson)
   - [Méthode 2 — interface graphique](#méthode-2--interface-graphique-frmparamconnexion)
   - [Méthode 3 — variables d'environnement](#méthode-3--variables-denvironnement)
   - [SQL Server sur la même machine](#cas-a--sql-server-sur-la-même-machine)
   - [SQL Server sur une VM distante](#cas-b--sql-server-sur-une-vm-distante)
5. [Initialisation de la base](#initialisation-de-la-base)
6. [Lancer l'application](#lancer-lapplication)
7. [Compte de test](#compte-de-test)
8. [API CoinGecko — cours crypto en temps réel](#api-coingecko--cours-crypto-en-temps-réel)
9. [Déploiement sur un nouveau poste](#déploiement-sur-un-nouveau-poste)
10. [Dépannage](#dépannage)

---

## Prérequis

| Outil | Version minimale | Lien |
|---|---|---|
| .NET SDK | 6.0 | https://dotnet.microsoft.com/download |
| Visual Studio | 2022 (Community) | https://visualstudio.microsoft.com |
| SQL Server | 2019 / 2022 | https://www.microsoft.com/sql-server |
| SQL Server Management Studio | 19+ (SSMS) | https://aka.ms/ssmsfullsetup |
| Git | 2.x | https://git-scm.com |

> 💡 SQL Server peut être installé sur la **même machine** ou sur une **VM distante**.  
> L'édition **Express** (gratuite) est suffisante pour ce projet.

---

## Récupération du code

### 1. Cloner le dépôt

```bash
git clone https://github.com/votre-utilisateur/BanqueNetPro.git
```

> ⚠️ Remplacez l'URL par celle de votre dépôt Git réel.

### 2. Se placer dans le dossier du projet

```bash
cd BanqueNetPro
```

### 3. Restaurer les dépendances NuGet

```bash
dotnet restore
```

### 4. Vérifier que le projet compile

```bash
dotnet build
```

Résultat attendu :
```
Build succeeded.
0 Warning(s)
0 Error(s)
```

---

## Structure du projet

```
BanqueWinForms/
│
├── Banque.UI/                      ← Formulaires WinForms
│   ├── FrmLogin.cs                 Connexion utilisateur
│   ├── FrmInscription.cs           Création de compte
│   ├── FrmDashboard.cs             Tableau de bord principal
│   ├── FrmRetrait.cs               Dépôt et retrait
│   ├── FrmVirement.cs              Virement entre comptes
│   ├── FrmCrypto.cs                Gestion des cryptomonnaies
│   └── FrmParamConnexion.cs        ⚙️  Configuration de la base de données
│
├── Banque.Business/                ← Logique métier
│   ├── AccountService.cs           Dépôt, retrait, création de compte
│   ├── TransactionService.cs       Virements et historique
│   ├── CryptoService.cs            Achat et vente de crypto
│   └── CryptoApiService.cs         🌐 Cours en temps réel (API CoinGecko)
│
├── Banque.Data/                    ← Accès base de données
│   ├── DbConnection.cs             ⚙️  Connexion SQL (lit AppConfig)
│   ├── UserRepository.cs           CRUD table users
│   ├── AccountRepository.cs        CRUD table accounts
│   ├── TransactionRepository.cs    CRUD table transactions
│   └── CryptoRepository.cs         CRUD table crypto_wallets
│
├── Banque.Models/                  ← Classes métier
│   ├── User.cs
│   ├── Account.cs
│   ├── Transaction.cs              Contient aussi l'enum TransactionType
│   └── CryptoWallet.cs
│
├── Banque.Config/                  ← Configuration centralisée
│   └── AppConfig.cs                Lit appsettings.json + variables d'env.
│
├── database/
│   └── init_db.sql                 Script d'initialisation SQL Server
│
├── appsettings.json                ⚙️  SEUL fichier à modifier pour changer de BDD
├── Program.cs                      Point d'entrée → lance FrmLogin
└── Bank.csproj
```

> ⚠️ Dans Visual Studio, `appsettings.json` doit avoir la propriété  
> **"Copier dans le répertoire de sortie"** → **"Copier si plus récent"**  
> pour être disponible à côté du `.exe`.

---

## Configuration de la base de données

La connexion est gérée par `AppConfig.cs` selon cet ordre de priorité :

```
Variables d'environnement   ← priorité maximale
          ↓
   appsettings.json          ← modification manuelle ou via interface graphique
          ↓
    Valeurs par défaut        ← fallback si fichier absent
```

---

### Méthode 1 — fichier appsettings.json

Ouvrez `appsettings.json` avec le Bloc-notes et modifiez les paramètres :

```json
{
  "Database": {
    "Host":     "ADRESSE_IP_OU_HOSTNAME",
    "Name":     "F_bank",
    "User":     "F_Admin",
    "Password": "Fu1b@nk",
    "Port":     1433,
    "TrustServerCertificate": true,
    "ConnectTimeout": 30
  },
  "Application": {
    "Nom":     "FulBank",
    "Version": "1.0.0"
  }
}
```

**Exemples de valeur pour `Host` :**

| Scénario | Valeur `Host` |
|---|---|
| SQL Server local (défaut) | `127.0.0.1` ou `localhost` |
| Instance nommée locale | `localhost\\SQLEXPRESS` |
| VM sur réseau local | `192.168.1.50` |
| VM avec instance nommée | `192.168.1.50\\SQLEXPRESS` |
| Hostname réseau | `NOM-MACHINE` |

---

### Méthode 2 — interface graphique (FrmParamConnexion)

Au lancement de l'application, cliquer sur le bouton **⚙ Paramètres** dans `FrmLogin`.

Le formulaire permet de :
- Modifier l'IP, le port, le nom de base, le login et le mot de passe
- Voir l'aperçu de la chaîne de connexion en temps réel
- Tester la connexion (retour visuel vert / rouge instantané)
- Sauvegarder dans `appsettings.json` sans recompiler ni redémarrer

---

### Méthode 3 — variables d'environnement

Utile pour les déploiements automatisés ou les VM gérées par un administrateur.  
Les variables écrasent toujours le fichier JSON.

```cmd
setx DB_HOST "192.168.1.50"
setx DB_NAME "F_bank"
setx DB_USER "F_Admin"
setx DB_PASS "Fu1b@nk"
setx DB_PORT "1433"
```

---

### Cas A — SQL Server sur la même machine

#### Étape 1 — Vérifier que SQL Server est démarré

```powershell
Get-Service -Name 'MSSQL*'
```

Le service `SQL Server (MSSQLSERVER)` ou `SQL Server (SQLEXPRESS)` doit être **En cours d'exécution**.

#### Étape 2 — Activer l'authentification SQL Server

1. Ouvrez **SSMS** → connectez-vous en Windows Authentication
2. Clic droit sur le serveur → **Properties** → onglet **Security**
3. Cochez **SQL Server and Windows Authentication mode**
4. Cliquez **OK** → redémarrez le service SQL Server

#### Étape 3 — Activer TCP/IP sur le port 1433

1. Ouvrez **SQL Server Configuration Manager**
2. `SQL Server Network Configuration` → `Protocols for MSSQLSERVER`
3. Clic droit sur **TCP/IP** → **Enable**
4. Double-clic → onglet **IP Addresses** → `IPAll` → `TCP Port` = **1433**
5. Redémarrez le service SQL Server

---

### Cas B — SQL Server sur une VM distante

#### Étape 1 — Récupérer l'adresse IP de la VM

Sur la **VM**, ouvrir un terminal :
```cmd
ipconfig
```
Noter l'adresse IPv4, par exemple : `192.168.1.50`

#### Étape 2 — Ouvrir le port 1433 dans le pare-feu de la VM

```powershell
# PowerShell en administrateur sur la VM
New-NetFirewallRule `
  -DisplayName "SQL Server Port 1433" `
  -Direction Inbound `
  -Protocol TCP `
  -LocalPort 1433 `
  -Action Allow
```

#### Étape 3 — Vérifier la connectivité depuis votre poste

```powershell
Test-NetConnection -ComputerName 192.168.1.50 -Port 1433
```

Résultat attendu : `TcpTestSucceeded : True`

#### Étape 4 — Autoriser les connexions distantes dans SQL Server

Dans **SSMS** sur la VM :
1. Clic droit sur le serveur → **Properties** → **Connections**
2. Cochez **Allow remote connections to this server**
3. Redémarrez le service SQL Server

---

## Initialisation de la base

### Exécuter le script SQL

1. Ouvrez **SSMS** → connectez-vous en `sa` ou Windows Authentication
2. `Fichier` → `Ouvrir` → sélectionnez `database/init_db.sql`
3. Cliquez **Exécuter (F5)**

Le script crée automatiquement :

| Objet | Détail |
|---|---|
| Base de données | `F_bank` |
| Table | `users` |
| Table | `accounts` |
| Table | `transactions` (avec contrainte CHECK sur le type) |
| Table | `crypto_wallets` (contrainte unique account_id + symbole) |
| Login SQL Server | `F_Admin` / `Fu1b@nk` |
| Utilisateur DB | `F_Admin` dans `F_bank` |
| Droits | SELECT, INSERT, UPDATE, DELETE sur toutes les tables |
| Données de test | `demo@banque.fr` / `demo1234` — solde 1 500 € |

> ✅ Le script est **idempotent** : il peut être ré-exécuté sans risque (`IF NOT EXISTS`).

### Vérification après exécution

```sql
USE F_bank;
SELECT * FROM users;
SELECT * FROM accounts;
```

---

## Lancer l'application

### Via Visual Studio

1. Ouvrez `Bank.csproj` dans Visual Studio 2022
2. Vérifiez que `Program.cs` pointe sur `FrmLogin` :
   ```csharp
   Application.Run(new FrmLogin());
   ```
3. Appuyez sur **F5**

### Via ligne de commande

```bash
dotnet run --project Bank.csproj
```

---

## Compte de test

| Champ | Valeur |
|---|---|
| Email | `demo@banque.fr` |
| Mot de passe | `demo1234` |
| Solde initial | `1 500,00 €` |

> ⚠️ En production, les mots de passe doivent être hachés avec **BCrypt**.

---

## API CoinGecko — cours crypto en temps réel

Les cours des cryptomonnaies sont récupérés en temps réel via l'API publique CoinGecko,
gérée par `Banque.Business/CryptoApiService.cs`.

**Endpoint utilisé :**
```
GET https://api.coingecko.com/api/v3/coins/markets
    ?vs_currency=eur
    &ids=bitcoin,ethereum,solana,tether,binancecoin
```

**Fonctionnement :**
- Appel automatique à l'ouverture de `FrmCrypto`
- Cache de **5 minutes** en mémoire pour éviter la surcharge de l'API
- En cas d'échec réseau → cours de secours utilisés (l'application continue)
- Bouton **"↻ Actualiser les cours"** dans `FrmCrypto` pour forcer un rafraîchissement

**Limites de l'API gratuite :** 10 à 30 requêtes par minute — largement suffisant.  
Aucune clé API nécessaire.

---

## Déploiement sur un nouveau poste

### Fichiers à copier

```
MonDossier/
├── FulBank.exe
├── appsettings.json    ← ⚙️ seul fichier à adapter
└── (dlls .NET)
```

### Procédure

1. Copier les fichiers sur le nouveau poste
2. Ouvrir `appsettings.json` avec le Bloc-notes
3. Modifier uniquement `"Host"` avec l'IP ou le nom du serveur SQL
4. Lancer `FulBank.exe`
5. Si besoin, utiliser le bouton **⚙ Paramètres** dans l'écran de connexion
   pour tester et ajuster les paramètres via l'interface graphique

---

## Dépannage

### ❌ "A network-related error occurred"

SQL Server inaccessible. Vérifier dans l'ordre :
- Le service SQL Server est démarré (`services.msc`)
- L'IP dans `appsettings.json` est correcte
- Le port 1433 est ouvert dans le pare-feu
- TCP/IP est activé dans SQL Server Configuration Manager

```powershell
Test-NetConnection -ComputerName 127.0.0.1 -Port 1433
```

---

### ❌ "Login failed for user 'F_Admin'"

- Le script `init_db.sql` n'a pas été exécuté
- L'authentification SQL Server n'est pas activée (revoir Cas A — Étape 2)
- Mot de passe incorrect dans `appsettings.json`

---

### ❌ "Cannot open database F_bank"

La base n'existe pas encore. Exécuter `database/init_db.sql` dans SSMS.

---

### ❌ L'application s'ouvre sur Form1

`Program.cs` pointe encore sur `Form1` :
```csharp
Application.Run(new FrmLogin()); // ← remplacer Form1 par FrmLogin
```
Supprimer ensuite `Form1.cs` du projet.

---

### ❌ appsettings.json non trouvé au lancement

Le fichier n'est pas copié dans `bin/Debug/`. Dans Visual Studio :
1. Clic droit sur `appsettings.json` → **Propriétés**
2. **"Copier dans le répertoire de sortie"** → **"Copier si plus récent"**

---

*FulBank — Documentation v2.0*