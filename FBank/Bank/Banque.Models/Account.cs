using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Banque.Models
{
    public class Account
    {
        // ── Clé primaire ──────────────────────────────────────────────────────
        public int Id { get; set; }

        // ── Clé étrangère → users.id ─────────────────────────────────────────
        /// <summary>
        /// Identifiant de l'utilisateur propriétaire du compte.
        /// FK : accounts.user_id → users.id  (CASCADE UPDATE / CASCADE DELETE)
        /// </summary>
        public int UserId { get; set; }

        // ── Données du compte ─────────────────────────────────────────────────
        /// <summary>
        /// Numéro de compte unique (format IBAN ou numéro interne).
        /// </summary>
        public string NumAccount { get; set; } = string.Empty;

        /// <summary>
        /// Solde actuel du compte en euros. Ne peut pas être négatif.
        /// </summary>
        public decimal Solde { get; set; } = 0.00m;

        // ── Timestamps ───────────────────────────────────────────────────────
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // ── Navigation : parent ───────────────────────────────────────────────
        /// <summary>
        /// Utilisateur propriétaire (objet complet chargé en jointure).
        /// </summary>
        public User User { get; set; }

        // ── Navigation : Asso 2 — Un compte a plusieurs transactions ─────────
        /// <summary>
        /// Historique des transactions liées à ce compte.
        /// (FK : transactions.account_id → accounts.id)
        /// </summary>
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

        // ── Navigation : Asso 3 — Un compte a plusieurs portefeuilles crypto ─
        /// <summary>
        /// Portefeuilles crypto rattachés à ce compte.
        /// (FK : crypto_wallets.account_id → accounts.id)
        /// </summary>
        public ICollection<Cryptowallet> CryptoWallets { get; set; } = new List<Cryptowallet>();

        // ── Affichage ─────────────────────────────────────────────────────────
        public override string ToString() =>
            $"[Account #{Id}] {NumAccount} — Solde : {Solde:C2}";
    }
}
