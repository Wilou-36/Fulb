using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Banque.Models
{
    // ══════════════════════════════════════════════════════════════════════════
    //  Cryptowallet — Modèle représentant un portefeuille de cryptomonnaie
    //  Table SQL correspondante : crypto_wallets
    //
    //  Relation :
    //    Account 1 ────── * Cryptowallet  (Asso 3 : un compte peut avoir
    //                                      plusieurs wallets, un par devise)
    //    FK : crypto_wallets.account_id → accounts.id  (CASCADE DELETE)
    //
    //  Contrainte unique en base : (account_id, symbole)
    //  → Un compte ne peut posséder qu'un seul wallet par cryptomonnaie.
    //    Ex : impossible d'avoir deux lignes BTC pour le même compte.
    //
    //  Précision : DECIMAL(20,8) — 8 décimales, standard Bitcoin (Satoshi).
    // ══════════════════════════════════════════════════════════════════════════
    public class Cryptowallet
    {
        // ── Clé primaire ──────────────────────────────────────────────────────
        // Correspond à la colonne INT IDENTITY(1,1) PRIMARY KEY de crypto_wallets
        public int Id { get; set; }

        // ── Clé étrangère → accounts.id ───────────────────────────────────────
        /// <summary>
        /// Identifiant du compte bancaire propriétaire de ce portefeuille.
        /// FK : crypto_wallets.account_id → accounts.id
        /// Comportement : CASCADE DELETE
        /// (si le compte est supprimé, tous ses wallets le sont aussi)
        /// </summary>
        public int AccountId { get; set; }

        // ── Données du portefeuille ───────────────────────────────────────────

        /// <summary>
        /// Symbole de la cryptomonnaie, toujours en majuscules.
        /// Exemples : BTC, ETH, SOL, USDT, BNB.
        /// Partie de la contrainte unique (account_id, symbole) en base.
        /// Colonne NVARCHAR(10) NOT NULL en base.
        /// </summary>
        public string Symbole { get; set; } = string.Empty;

        /// <summary>
        /// Quantité de cryptomonnaie détenue dans ce portefeuille.
        /// Précision 8 décimales (DECIMAL(20,8)) — correspond au standard Satoshi (Bitcoin).
        /// ⚠ Ne peut jamais être négative — vérification assurée par CryptoService.VendreCrypto().
        /// Mis à jour via CryptoRepository.UpdateQuantite() après chaque achat/vente.
        /// </summary>
        public decimal Quantite { get; set; } = 0.00000000m;

        // ── Timestamps ────────────────────────────────────────────────────────

        /// <summary>Date et heure de création du wallet (UTC). Premier achat de cette devise.</summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date et heure de la dernière mise à jour de la quantité (UTC).
        /// Actualisé à chaque achat ou vente via CryptoRepository.UpdateQuantite().
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // ── Propriété de navigation : parent (Asso 3) ─────────────────────────
        /// <summary>
        /// Objet Account propriétaire de ce wallet.
        /// Chargé en jointure si besoin — null par défaut dans les repositories.
        /// Permet d'accéder à account.Solde, account.NumAccount etc.
        /// </summary>
        public Account Account { get; set; }

        // ── Helpers : calculs de valorisation ────────────────────────────────

        /// <summary>
        /// Calcule la valeur en euros de ce wallet selon un cours donné.
        /// Formule : Quantite × coursActuel
        /// Utilisé par CryptoService.GetValeurTotalePortefeuille() pour
        /// calculer la valeur totale du portefeuille crypto d'un compte.
        /// </summary>
        /// <param name="coursActuel">
        /// Prix unitaire actuel de la cryptomonnaie en euros.
        /// Fourni par l'API CoinGecko ou par le dictionnaire de cours dans FrmCrypto.
        /// </param>
        /// <returns>Valeur du portefeuille en euros (DECIMAL).</returns>
        public decimal ValeurEnEuros(decimal coursActuel) =>
            Quantite * coursActuel;

        // ── Affichage ─────────────────────────────────────────────────────────
        /// <summary>
        /// Représentation lisible de l'objet Cryptowallet.
        /// Affiche le symbole et la quantité avec 8 décimales (format F8).
        /// </summary>
        public override string ToString() =>
            $"[Wallet #{Id}] {Symbole} — Quantité : {Quantite:F8}";
    }
}