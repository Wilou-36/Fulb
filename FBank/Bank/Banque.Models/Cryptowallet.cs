using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Banque.Models
{
    public class Cryptowallet
    {
        // ── Clé primaire ──────────────────────────────────────────────────────
        public int Id { get; set; }

        // ── Clé étrangère → accounts.id ──────────────────────────────────────
        /// <summary>
        /// Identifiant du compte bancaire propriétaire de ce wallet.
        /// FK : crypto_wallets.account_id → accounts.id  (CASCADE DELETE)
        /// </summary>
        public int AccountId { get; set; }

        // ── Données du portefeuille ───────────────────────────────────────────
        /// <summary>
        /// Symbole de la cryptomonnaie en majuscules (ex : BTC, ETH, SOL, USDT).
        /// </summary>
        public string Symbole { get; set; } = string.Empty;

        /// <summary>
        /// Quantité détenue. Précision à 8 décimales (standard Bitcoin).
        /// Ne peut pas être négative.
        /// </summary>
        public decimal Quantite { get; set; } = 0.00000000m;

        // ── Timestamps ───────────────────────────────────────────────────────
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // ── Navigation : parent ───────────────────────────────────────────────
        /// <summary>
        /// Compte bancaire propriétaire (objet complet chargé en jointure).
        /// </summary>
        public Account Account { get; set; }

        // ── Helpers ───────────────────────────────────────────────────────────
        /// <summary>
        /// Calcule la valeur en euros du wallet selon un cours donné.
        /// </summary>
        /// <param name="coursActuel">Cours actuel de la crypto en euros.</param>
        public decimal ValeurEnEuros(decimal coursActuel) =>
            Quantite * coursActuel;

        // ── Affichage ─────────────────────────────────────────────────────────
        public override string ToString() =>
            $"[Wallet #{Id}] {Symbole} — Quantité : {Quantite:F8}";
    }
}
