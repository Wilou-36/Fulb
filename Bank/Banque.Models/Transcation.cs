using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Banque.Models
{
    // ══════════════════════════════════════════════════════════════════════════
    //  Transaction — Modèle représentant une opération financière
    //  Table SQL correspondante : transactions
    //
    //  Relation :
    //    Account 1 ────── * Transaction  (Asso 2 : un compte a plusieurs transactions)
    //    FK : transactions.account_id → accounts.id  (RESTRICT DELETE)
    //
    //  Principe comptable :
    //    Le champ Montant est TOUJOURS positif.
    //    Le sens de l'opération (débit ou crédit) est déterminé par le Type.
    //    IsCredit / IsDebit permettent de connaître ce sens sans condition externe.
    //
    //  Types disponibles : voir enum TransactionType ci-dessous.
    // ══════════════════════════════════════════════════════════════════════════
    public class Transaction
    {
        // ── Clé primaire ──────────────────────────────────────────────────────
        // Correspond à la colonne INT IDENTITY(1,1) PRIMARY KEY de la table transactions
        public int Id { get; set; }

        // ── Clé étrangère → accounts.id ───────────────────────────────────────
        /// <summary>
        /// Identifiant du compte sur lequel porte la transaction.
        /// FK : transactions.account_id → accounts.id
        /// Comportement : RESTRICT DELETE
        /// (un compte ne peut pas être supprimé s'il a des transactions)
        /// </summary>
        public int AccountId { get; set; }

        // ── Données de la transaction ─────────────────────────────────────────

        /// <summary>
        /// Nature de l'opération financière.
        /// Valeurs possibles : DEPOT, RETRAIT, VIREMENT, ACHAT_CRYPTO, VENTE_CRYPTO.
        /// Stocké sous forme de NVARCHAR(20) avec contrainte CHECK en base.
        /// Utilisé par IsCredit / IsDebit pour déterminer le sens comptable.
        /// </summary>
        public TransactionType Type { get; set; }

        /// <summary>
        /// Montant de l'opération en euros.
        /// ⚠ Toujours POSITIF — le sens (débit/crédit) est porté par le Type.
        /// Colonne DECIMAL(15,2) NOT NULL en base.
        /// </summary>
        public decimal Montant { get; set; }

        /// <summary>
        /// Date et heure exacte de l'opération en UTC.
        /// Initialisé à DateTime.UtcNow lors de la création.
        /// Colonne DATETIME2 DEFAULT GETUTCDATE() en base.
        /// </summary>
        public DateTime DateTransaction { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Description libre de l'opération.
        /// Exemples :
        ///   "Dépôt de 500,00 €"
        ///   "[SORTANT] Virement FR1234 → FR5678"
        ///   "Achat 0.00500000 BTC @ 62 000,00 €/unité"
        /// Colonne NVARCHAR(MAX) NULL en base — peut être null.
        /// </summary>
        public string Details { get; set; }

        // ── Timestamps ────────────────────────────────────────────────────────

        /// <summary>Date et heure d'insertion de la ligne en base (UTC).</summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>Date et heure de dernière mise à jour (UTC).</summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // ── Propriété de navigation : parent (Asso 2) ─────────────────────────
        /// <summary>
        /// Objet Account associé à cette transaction.
        /// Chargé en jointure si besoin — null par défaut dans les repositories.
        /// Permet d'accéder à account.NumAccount, account.Solde etc.
        /// </summary>
        public Account Account { get; set; }

        // ── Helpers : sens comptable ──────────────────────────────────────────

        /// <summary>
        /// Indique si la transaction crédite le compte (entrée d'argent).
        /// Retourne true pour : DEPOT, VIREMENT (entrant), VENTE_CRYPTO.
        /// Utilisé dans FrmDashboard pour colorier les lignes en vert.
        /// </summary>
        public bool IsCredit =>
            Type == TransactionType.DEPOT ||
            Type == TransactionType.VIREMENT ||
            Type == TransactionType.VENTE_CRYPTO;

        /// <summary>
        /// Indique si la transaction débite le compte (sortie d'argent).
        /// Inverse logique de IsCredit.
        /// Retourne true pour : RETRAIT, VIREMENT (sortant), ACHAT_CRYPTO.
        /// Utilisé dans FrmDashboard pour colorier les lignes en rouge.
        /// </summary>
        public bool IsDebit => !IsCredit;

        // ── Affichage ─────────────────────────────────────────────────────────
        /// <summary>
        /// Représentation lisible de l'objet Transaction.
        /// Utilisée dans les logs et les outils de débogage.
        /// </summary>
        public override string ToString() =>
            $"[Transaction #{Id}] {Type} — {Montant:C2} le {DateTransaction:dd/MM/yyyy HH:mm}";
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  TransactionType — Enumération des types d'opérations bancaires
    //
    //  Déclaré en dehors de la classe Transaction pour être accessible depuis
    //  tous les services (AccountService, TransactionService, CryptoService)
    //  sans avoir à qualifier le chemin complet Transaction.TransactionType.
    //
    //  Correspondance avec la contrainte CHECK SQL :
    //    CHECK (type IN ('DEPOT','RETRAIT','VIREMENT','ACHAT_CRYPTO','VENTE_CRYPTO'))
    //  La conversion enum ↔ string est gérée par TransactionRepository via .ToString()
    //  et Enum.Parse<TransactionType>() lors de la lecture.
    // ══════════════════════════════════════════════════════════════════════════
    public enum TransactionType
    {
        /// <summary>Versement d'argent sur le compte (crédit).</summary>
        DEPOT,

        /// <summary>Retrait d'argent depuis le compte (débit).</summary>
        RETRAIT,

        /// <summary>
        /// Transfert entre deux comptes.
        /// Génère deux transactions : [SORTANT] sur l'émetteur, [ENTRANT] sur le bénéficiaire.
        /// </summary>
        VIREMENT,

        /// <summary>
        /// Achat de cryptomonnaie financé par le solde en euros (débit).
        /// Le wallet crypto correspondant est crédité.
        /// </summary>
        ACHAT_CRYPTO,

        /// <summary>
        /// Vente de cryptomonnaie convertie en euros (crédit).
        /// Le wallet crypto correspondant est débité.
        /// </summary>
        VENTE_CRYPTO
    }
}