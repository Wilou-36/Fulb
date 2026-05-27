using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Banque.Models
{
    // ══════════════════════════════════════════════════════════════════════════
    //  User — Modèle représentant un client de la banque
    //  Table SQL correspondante : users
    //
    //  Relations :
    //    User 1 ────────── * Account   (Asso 1 : un client possède plusieurs comptes)
    //
    //  Remarque sécurité : le champ Password doit toujours être stocké sous
    //  forme de hash (BCrypt) en base. Ne jamais stocker en clair en production.
    // ══════════════════════════════════════════════════════════════════════════
    public class User
    {
        // ── Clé primaire ──────────────────────────────────────────────────────
        // Correspond à la colonne INT IDENTITY(1,1) PRIMARY KEY de la table users
        public int Id { get; set; }

        // ── Données personnelles ──────────────────────────────────────────────

        /// <summary>Nom de famille du client. Colonne NOT NULL en base.</summary>
        public string Nom { get; set; } = string.Empty;

        /// <summary>Prénom du client. Colonne NOT NULL en base.</summary>
        public string Prenom { get; set; } = string.Empty;

        /// <summary>
        /// Adresse email du client. Colonne UNIQUE NOT NULL en base.
        /// Sert d'identifiant de connexion dans FrmLogin.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Mot de passe du client.
        /// ⚠ Doit être haché avec BCrypt avant insertion en base.
        /// Ne jamais comparer en clair en production.
        /// </summary>
        public string Password { get; set; } = string.Empty;

        // ── Timestamps ────────────────────────────────────────────────────────
        // Gérés automatiquement — initialisés à l'heure UTC courante

        /// <summary>Date et heure de création de l'enregistrement (UTC).</summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date et heure de la dernière modification (UTC).
        /// Mis à jour à chaque appel à UserRepository.Update().
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // ── Propriété de navigation : Asso 1 ─────────────────────────────────
        /// <summary>
        /// Liste des comptes bancaires rattachés à cet utilisateur.
        /// Relation 1-* : un utilisateur peut avoir plusieurs comptes.
        /// Chargée en jointure si besoin (non chargée par défaut dans les repositories).
        /// FK : accounts.user_id → users.id  (CASCADE DELETE)
        /// </summary>
        public ICollection<Account> Accounts { get; set; } = new List<Account>();

        // ── Affichage ─────────────────────────────────────────────────────────
        /// <summary>
        /// Représentation lisible de l'objet User.
        /// Utilisée dans les logs et les MessageBox de débogage.
        /// </summary>
        public override string ToString() =>
            $"[User #{Id}] {Prenom} {Nom} — {Email}";
    }
}