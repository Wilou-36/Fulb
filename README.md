# 🏛 Fulbank — Documentation

> Application bancaire desktop développée en **C# WinForms (.NET 6)** avec **SQL Server**.  
> Architecture en couches : `Banque.UI` · `Banque.Business` · `Banque.Data` · `Banque.Models`

---

## 📋 Sommaire

1. [Prérequis](#prérequis)
2. [Récupération du code](#récupération-du-code)
3. [Structure du projet](#structure-du-projet)
4. [Configuration de la base de données](#configuration-de-la-base-de-données)
   - [SQL Server sur la même machine](#cas-1--sql-server-sur-la-même-machine)
   - [SQL Server sur une VM distante](#cas-2--sql-server-sur-une-vm-distante)
5. [Adapter la chaîne de connexion](#adapter-la-chaîne-de-connexion)
6. [Initialisation de la base](#initialisation-de-la-base)
7. [Lancer l'application](#lancer-lapplication)
8. [Compte de test](#compte-de-test)
9. [Dépannage](#dépannage)

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

Ouvrez un terminal (PowerShell, CMD ou Git Bash) et exécutez :

```bash
git clone https://github.com/votre-utilisateur/Fulb.git
```

> ⚠️ Remplacez l'URL par celle de votre dépôt Git réel.

### 2. Se placer dans le dossier du projet

```bash
cd BFulb
```

### 3. Restaurer les dépendances NuGet

```bash
dotnet restore
```

### 4. Vérifier que le projet compile

```bash
dotnet build
```

Si la compilation réussit, vous verrez :
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
├── Banque.UI/                  ← Formulaires WinForms
│   ├── FrmLogin.cs             Connexion utilisateur
│   ├── FrmInscription.cs       Création de compte
│   ├── FrmDashboard.cs         Tableau de bord principal
│   ├── FrmRetrait.cs           Dépôt et retrait
│   ├── FrmVirement.cs          Virement entre comptes
│   └── FrmCrypto.cs            Gestion des cryptomonnaies
│
├── Banque.Business/            ← Logique métier
│   ├── AccountService.cs       Dépôt, retrait, création de compte
│   ├── TransactionService.cs   Virements et historique
│   └── CryptoService.cs        Achat et vente de crypto
│
├── Banque.Data/                ← Accès base de données
│   ├── DbConnection.cs         ⚙️  Chaîne de connexion (à configurer)
│   ├── UserRepository.cs       CRUD table users
│   ├── AccountRepository.cs    CRUD table accounts
│   ├── TransactionRepository.cs CRUD table transactions
│   └── CryptoRepository.cs     CRUD table crypto_wallets
│
├── Banque.Models/              ← Classes métier
│   ├── User.cs
│   ├── Account.cs
│   ├── Transaction.cs
│   └── CryptoWallet.cs
│
├── database/
│   └── init_db.sql             ⚙️  Script d'initialisation SQL Server
│
├── Program.cs                  Point d'entrée → lance FrmLogin
└── Bank.csproj
```

---

## Configuration de la base de données

### Cas 1 — SQL Server sur la même machine

#### Étape 1 — Vérifier que SQL Server est démarré

Dans **Services Windows** (`services.msc`), vérifiez que le service  
`SQL Server (MSSQLSERVER)` ou `SQL Server (SQLEXPRESS)` est en état **En cours d'exécution**.

Ou via PowerShell :
```powershell
Get-Service -Name 'MSSQL*'
```

#### Étape 2 — Activer l'authentification SQL Server

1. Ouvrez **SSMS** et connectez-vous en **Windows Authentication**
2. Clic droit sur le serveur → **Properties**
3. Onglet **Security** → cochez **SQL Server and Windows Authentication mode**
4. Cliquez **OK** puis **redémarrez le service SQL Server**

#### Étape 3 — Activer le port TCP/IP

1. Ouvrez **SQL Server Configuration Manager**
2. `SQL Server Network Configuration` → `Protocols for MSSQLSERVER`
3. Clic droit sur **TCP/IP** → **Enable**
4. Double-clic sur **TCP/IP** → onglet **IP Addresses**  
   → Descendez jusqu'à **IPAll** → `TCP Port` = **1433**
5. Redémarrez le service SQL Server

#### Chaîne de connexion pour localhost

```csharp
_connectionString =
    "Server=127.0.0.1;"      +
    "Database=ful_bank;"     +
    "User Id=bank;"          +
    "Password=Fu1b@nk;"      +
    "TrustServerCertificate=True;";
```

---

### Cas 2 — SQL Server sur une VM distante

#### Étape 1 — Connaître l'adresse IP de la VM

Sur la **VM** (Windows), ouvrez un terminal et exécutez :
```cmd
ipconfig
```
Notez l'adresse IPv4, par exemple : `192.168.1.50`

Si la VM est sur un réseau distant ou VPN, utilisez l'adresse IP fournie par votre administrateur.

#### Étape 2 — Ouvrir le port 1433 sur la VM

Sur la **VM**, dans le **Pare-feu Windows** :

```powershell
# PowerShell (en administrateur sur la VM)
New-NetFirewallRule `
  -DisplayName "SQL Server Port 1433" `
  -Direction Inbound `
  -Protocol TCP `
  -LocalPort 1433 `
  -Action Allow
```

Ou manuellement :
1. `Pare-feu Windows Defender` → `Règles de trafic entrant`
2. `Nouvelle règle` → `Port` → TCP → `1433` → `Autoriser`

#### Étape 3 — Vérifier la connectivité depuis votre machine

Depuis **votre machine** (pas la VM), testez la connexion :

```powershell
# Test de ping
ping 192.168.1.50

# Test du port 1433
Test-NetConnection -ComputerName 192.168.1.50 -Port 1433
```

Résultat attendu :
```
TcpTestSucceeded : True
```

Si `TcpTestSucceeded : False` → le pare-feu de la VM bloque le port (revoir Étape 2).

#### Étape 4 — Activer les connexions distantes dans SQL Server

Sur la **VM**, dans **SSMS** :
1. Clic droit sur le serveur → **Properties** → **Connections**
2. Cochez **Allow remote connections to this server**
3. Redémarrez le service SQL Server

#### Chaîne de connexion pour VM distante

Dans `Banque.Data/DbConnection.cs`, remplacez `127.0.0.1` par l'IP de la VM :

```csharp
_connectionString =
    "Server=192.168.1.50;"   +   // ← IP de votre VM
    "Database=ful_bank;"     +
    "User Id=bank;"          +
    "Password=Fu1b@nk;"      +
    "TrustServerCertificate=True;";
```

> 💡 Si SQL Server utilise une **instance nommée** (ex: `SQLEXPRESS`), utilisez :
> ```csharp
> "Server=192.168.1.50\\SQLEXPRESS;"
> ```

---

## Adapter la chaîne de connexion

Ouvrez le fichier `Banque.Data/DbConnection.cs` et modifiez uniquement le constructeur :

```csharp
private DbConnection()
{
    _connectionString =
        "Server=VOTRE_IP_OU_HOSTNAME;" +  // 127.0.0.1 ou IP VM
        "Database=ful_bank;"            +  // nom de la base
        "User Id=bank;"                 +  // login SQL créé par init_db.sql
        "Password=Fu1b@nk;"             +  // mot de passe du login
        "TrustServerCertificate=True;"; // désactiver si certificat SSL valide
}
```

### Exemples de configurations courantes

| Scénario | Valeur `Server=` |
|---|---|
| Localhost instance par défaut | `127.0.0.1` ou `localhost` |
| Localhost instance nommée | `localhost\SQLEXPRESS` |
| VM sur réseau local | `192.168.1.50` |
| VM avec instance nommée | `192.168.1.50\SQLEXPRESS` |
| Hostname réseau | `NOM-MACHINE\INSTANCE` |

---

## Initialisation de la base

### Exécuter le script SQL

1. Ouvrez **SSMS**
2. Connectez-vous au bon serveur en **sa** ou **Windows Authentication**
3. `Fichier` → `Ouvrir` → `Fichier` → sélectionnez `database/init_db.sql`
4. Cliquez sur **Exécuter** (F5)

Le script crée automatiquement :

| Objet | Détail |
|---|---|
| Base de données | `ful_bank` |
| Table | `users` |
| Table | `accounts` |
| Table | `transactions` |
| Table | `crypto_wallets` |
| Login SQL Server | `bank` / `Fu1b@nk` |
| Utilisateur DB | `bank` dans `ful_bank` |
| Droits | SELECT, INSERT, UPDATE, DELETE |
| Données de test | `demo@banque.fr` / `demo1234` |

> ✅ Le script est **idempotent** : il peut être ré-exécuté sans risque grâce aux blocs `IF NOT EXISTS`.

### Vérification après exécution

Dans SSMS, exécutez :
```sql
USE ful_bank;
SELECT * FROM users;
SELECT * FROM accounts;
```
Vous devez voir l'utilisateur de test et son compte bancaire.

---

## Lancer l'application

### Via Visual Studio

1. Ouvrez `Bank.csproj` dans Visual Studio 2022
2. Vérifiez que `Program.cs` pointe sur `FrmLogin` :
   ```csharp
   Application.Run(new FrmLogin());
   ```
3. Appuyez sur **F5** (ou bouton ▶ Démarrer)

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
> Remplacez la comparaison directe dans `FrmLogin` par `BCrypt.Verify(saisi, stocké)`.

---

## Dépannage

### ❌ Erreur "A network-related error occurred"

```
A network-related or instance-specific error occurred while establishing
a connection to SQL Server.
```

**Causes possibles :**
- SQL Server n'est pas démarré → vérifier `services.msc`
- IP incorrecte dans `DbConnection.cs`
- Port 1433 bloqué par le pare-feu → revoir la section VM
- TCP/IP non activé → revoir SQL Server Configuration Manager

**Diagnostic rapide :**
```powershell
Test-NetConnection -ComputerName 127.0.0.1 -Port 1433
```

---

### ❌ Erreur "Login failed for user 'bank'"

**Causes possibles :**
- Le script `init_db.sql` n'a pas été exécuté
- L'authentification SQL Server n'est pas activée → revoir Étape 2 (Cas 1)
- Mot de passe incorrect dans `DbConnection.cs`

**Solution :** Ré-exécuter `init_db.sql` en administrateur dans SSMS.

---

### ❌ Erreur "Cannot open database ful_bank"

La base n'existe pas encore.  
**Solution :** Exécuter `database/init_db.sql` dans SSMS.

---

### ❌ L'application s'ouvre sur Form1

`Program.cs` pointe encore sur `Form1`.  
**Solution :**
```csharp
// Program.cs
Application.Run(new FrmLogin()); // ← remplacer Form1 par FrmLogin
```
Supprimer ensuite `Form1.cs` du projet.

---

*Fulbank — Documentation v1.0*