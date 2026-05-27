using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Banque.Models
{
    // ══════════════════════════════════════════════════════════════════════════
    //  Account — Modèle représentant un compte bancaire
    //  Table SQL correspondante : accounts
    //
    //  Relations :
    //    User    1 ────── * Account      (Asso 1 : parent — FK user_id)
    //    Account 1 ────── * Transaction  (Asso 2 : enfant — FK account_id)
    //    Account 1 ────── * Cryptowallet (Asso 3 : enfant — FK account_id)
    //
    //  Règle métier : le solde ne peut jamais être négatif.
    //  Toutes les modifications du solde passent par AccountRepository.UpdateSolde().
    // ══════════════════════════════════════════════════════════════════════════
    public class Account
    {
        // ── Clé primaire ──────────────────────────────────────────────────────
        // Correspond à la colonne INT IDENTITY(1,1) PRIMARY KEY de la table accounts
        public int Id { get; set; }

        // ── Clé étrangère → users.id ──────────────────────────────────────────
        /// <summary>
        /// Identifiant de l'utilisateur propriétaire du compte.
        /// FK : accounts.user_id → users.id
        /// Comportement : CASCADE UPDATE / CASCADE DELETE
        /// (si l'utilisateur est supprimé, tous ses comptes le sont aussi)
        /// </summary>
        public int UserId { get; set; }

        // ── Données du compte ─────────────────────────────────────────────────

        /// <summary>
        /// Numéro de compte unique généré automatiquement.
        /// Format interne : FR + timestamp Unix + 3 chiffres aléatoires.
        /// Exemple : FR173892748123456
        /// Colonne NVARCHAR(34) UNIQUE NOT NULL en base.
        /// </summary>
        public string NumAccount { get; set; } = string.Empty;

        /// <summary>
        /// Solde actuel du compte en euros.
        /// Colonne DECIMAL(15,2) avec DEFAULT 0.00 en base.
        /// ⚠ Ne doit jamais être modifié directement — passer par AccountService.
        /// </summary>
        public decimal Solde { get; set; } = 0.00m;

        // ── Timestamps ────────────────────────────────────────────────────────

        /// <summary>Date et heure de création du compte (UTC).</summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date et heure de la dernière mise à jour (UTC).
        /// Actualisé à chaque UpdateSolde() dans AccountRepository.
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // ── Propriété de navigation : parent (Asso 1) ─────────────────────────
        /// <summary>
        /// Objet User propriétaire du compte.
        /// Chargé en jointure si besoin — null par défaut dans les repositories.
        /// Permet d'accéder à user.Nom, user.Email etc. sans requête supplémentaire.
        /// </summary>
        public User User { get; set; }

        // ── Propriété de navigation : enfants (Asso 2) ───────────────────────
        /// <summary>
        /// Historique complet des transactions liées à ce compte.
        /// Relation 1-* : un compte peut avoir de nombreuses transactions.
        /// FK : transactions.account_id → accounts.id  (RESTRICT DELETE)
        /// Non chargée par défaut — utiliser TransactionService.GetHistorique().
        /// </summary>
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

        // ── Propriété de navigation : enfants (Asso 3) ───────────────────────
        /// <summary>
        /// Portefeuilles de cryptomonnaies rattachés à ce compte.
        /// Relation 1-* : un compte peut avoir un wallet par devise (BTC, ETH...).
        /// FK : crypto_wallets.account_id → accounts.id  (CASCADE DELETE)
        /// Non chargée par défaut — utiliser CryptoService.GetWallets().
        /// </summary>
        public ICollection<Cryptowallet> CryptoWallets { get; set; } = new List<Cryptowallet>();

        // ── Affichage ─────────────────────────────────────────────────────────
        /// <summary>
        /// Représentation lisible de l'objet Account.
        /// Utilisée dans les ComboBox, logs et MessageBox.
        /// </summary>
        public override string ToString() =>
            $"[Account #{Id}] {NumAccount} — Solde : {Solde:C2}";
    }
}