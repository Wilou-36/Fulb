using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Banque.Models
{
    public class Transaction
    {
        // ── Clé primaire ──────────────────────────────────────────────────────
        public int Id { get; set; }

        // ── Clé étrangère → accounts.id ──────────────────────────────────────
        /// <summary>
        /// Identifiant du compte sur lequel porte la transaction.
        /// FK : transactions.account_id → accounts.id  (RESTRICT DELETE)
        /// </summary>
        public int AccountId { get; set; }

        // ── Données de la transaction ─────────────────────────────────────────
        /// <summary>
        /// Nature de l'opération (dépôt, retrait, virement, crypto…).
        /// </summary>
        public TransactionType Type { get; set; }

        /// <summary>
        /// Montant de la transaction en euros. Toujours positif ;
        /// le signe comptable est déterminé par le <see cref="Type"/>.
        /// </summary>
        public decimal Montant { get; set; }

        /// <summary>
        /// Date et heure de l'opération (UTC).
        /// </summary>
        public DateTime DateTransaction { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Description libre ou données complémentaires (ex : IBAN destinataire,
        /// référence virement, symbole crypto…).
        /// </summary>
        public string Details { get; set; }

        // ── Timestamps ───────────────────────────────────────────────────────
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // ── Navigation : parent ───────────────────────────────────────────────
        /// <summary>
        /// Compte bancaire concerné (objet complet chargé en jointure).
        /// </summary>
        public Account Account { get; set; }

        // ── Helpers ───────────────────────────────────────────────────────────
        /// <summary>
        /// Retourne vrai si la transaction crédite le compte (entrée d'argent).
        /// </summary>
        public bool IsCredit =>
            Type == TransactionType.DEPOT ||
            Type == TransactionType.VIREMENT ||
            Type == TransactionType.VENTE_CRYPTO;

        /// <summary>
        /// Retourne vrai si la transaction débite le compte (sortie d'argent).
        /// </summary>
        public bool IsDebit => !IsCredit;

        // ── Affichage ─────────────────────────────────────────────────────────
        public override string ToString() =>
            $"[Transaction #{Id}] {Type} — {Montant:C2} le {DateTransaction:dd/MM/yyyy HH:mm}";
    }

    // ── Enum sorti de la classe pour être accessible globalement ─────────────
    public enum TransactionType
    {
        DEPOT,
        RETRAIT,
        VIREMENT,
        ACHAT_CRYPTO,
        VENTE_CRYPTO
    }
}