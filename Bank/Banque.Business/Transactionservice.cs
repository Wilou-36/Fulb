using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bank.Banque.Data;
using Bank.Banque.Models;

namespace Bank.Banque.Business
{
    // ══════════════════════════════════════════════════════════════════════════
    //  TransactionService — Couche métier des transactions bancaires
    //  Responsabilité : orchestrer les virements entre comptes et fournir
    //  l'historique des opérations financières avec des calculs agrégés.
    //  Règle : toute opération qui touche plusieurs comptes doit passer ici.
    // ══════════════════════════════════════════════════════════════════════════
    internal class TransactionService
    {
        // ── Dépendances injectées ─────────────────────────────────────────────
        // Repository des transactions : INSERT et SELECT sur la table transactions
        private readonly TransactionRepository _transactionRepo = new TransactionRepository();

        // Repository des comptes : nécessaire pour vérifier les soldes
        // et mettre à jour les deux comptes lors d'un virement
        private readonly AccountRepository _accountRepo = new AccountRepository();

        // ═════════════════════════════════════════════════════════════════════
        //  VIREMENT ENTRE COMPTES
        // ═════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Effectue un virement bancaire d'un compte source vers un compte destinataire.
        /// Opération en deux temps : débit source + crédit destination.
        /// Deux transactions sont enregistrées : une [SORTANT] et une [ENTRANT].
        /// </summary>
        /// <param name="accountSourceId">Compte émetteur (sera débité).</param>
        /// <param name="accountDestId">Compte bénéficiaire (sera crédité).</param>
        /// <param name="montant">Montant du virement en euros (doit être > 0).</param>
        /// <param name="details">Libellé optionnel du virement.</param>
        /// <exception cref="ArgumentException">
        /// Levée si les deux comptes sont identiques, si le montant est invalide,
        /// ou si l'un des comptes n'existe pas en base.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Levée si le solde du compte source est insuffisant.
        /// </exception>
        public void EffectuerVirement(int accountSourceId, int accountDestId, decimal montant, string details = null)
        {
            // ── Validations préalables ────────────────────────────────────────

            // Règle métier : impossible de virer vers le même compte
            if (accountSourceId == accountDestId)
                throw new ArgumentException("Le compte source et le compte destinataire doivent être différents.");

            // Le montant doit être strictement positif
            if (montant <= 0)
                throw new ArgumentException("Le montant du virement doit être strictement positif.");

            // Chargement du compte source — null si inexistant
            var source = _accountRepo.GetById(accountSourceId)
                ?? throw new ArgumentException($"Compte source #{accountSourceId} introuvable.");

            // Chargement du compte destinataire — null si inexistant
            var destination = _accountRepo.GetById(accountDestId)
                ?? throw new ArgumentException($"Compte destinataire #{accountDestId} introuvable.");

            // Vérification du solde disponible sur le compte source
            if (source.Solde < montant)
                throw new InvalidOperationException(
                    $"Solde insuffisant. Disponible : {source.Solde:C2}, demandé : {montant:C2}.");

            // ── Mise à jour des deux soldes ───────────────────────────────────

            // Débit du compte source : solde source diminue du montant
            _accountRepo.UpdateSolde(accountSourceId, source.Solde - montant);

            // Crédit du compte destinataire : solde destination augmente du montant
            _accountRepo.UpdateSolde(accountDestId, destination.Solde + montant);

            // ── Horodatage commun aux deux transactions ────────────────────────
            // On utilise le même DateTime pour lier les deux écritures comptables
            var now = DateTime.UtcNow;
            var detail = details ?? $"Virement {source.NumAccount} → {destination.NumAccount}";

            // ── Écriture comptable côté ÉMETTEUR ─────────────────────────────
            // Enregistrement de la transaction sortante sur le compte source
            _transactionRepo.Add(new Transaction
            {
                AccountId = accountSourceId,
                Type = TransactionType.VIREMENT, // enum → 'VIREMENT' en base
                Montant = montant,
                DateTransaction = now,
                Details = $"[SORTANT] {detail}",   // préfixe pour distinguer sens
                CreatedAt = now,
                UpdatedAt = now,
            });

            // ── Écriture comptable côté BÉNÉFICIAIRE ─────────────────────────
            // Enregistrement de la transaction entrante sur le compte destination
            _transactionRepo.Add(new Transaction
            {
                AccountId = accountDestId,
                Type = TransactionType.VIREMENT,
                Montant = montant,
                DateTransaction = now,
                Details = $"[ENTRANT] {detail}",   // préfixe opposé
                CreatedAt = now,
                UpdatedAt = now,
            });
        }

        // ═════════════════════════════════════════════════════════════════════
        //  HISTORIQUE DES TRANSACTIONS
        // ═════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Retourne l'historique complet des transactions d'un compte,
        /// trié de la plus récente à la plus ancienne (ORDER BY date_transaction DESC).
        /// </summary>
        /// <param name="accountId">Identifiant du compte (FK account_id).</param>
        /// <returns>
        /// Liste de toutes les transactions : dépôts, retraits, virements, crypto.
        /// Liste vide si aucune opération n'a encore été effectuée.
        /// </returns>
        public List<Transaction> GetHistorique(int accountId)
        {
            // Délègue directement au repository — le tri est géré en SQL
            return _transactionRepo.GetByAccountId(accountId);
        }

        /// <summary>
        /// Retourne les transactions d'un compte filtrées par type d'opération.
        /// Utile pour afficher uniquement les virements ou les opérations crypto.
        /// </summary>
        /// <param name="accountId">Identifiant du compte.</param>
        /// <param name="type">
        /// Type souhaité : DEPOT, RETRAIT, VIREMENT, ACHAT_CRYPTO ou VENTE_CRYPTO.
        /// </param>
        /// <returns>Liste filtrée, triée par date décroissante.</returns>
        public List<Transaction> GetHistoriqueParType(int accountId, TransactionType type)
        {
            // Le filtre WHERE type = @type est appliqué directement en SQL
            return _transactionRepo.GetByAccountIdAndType(accountId, type);
        }

        /// <summary>
        /// Retourne une transaction spécifique par son identifiant primaire.
        /// </summary>
        /// <param name="transactionId">Clé primaire de la transaction.</param>
        /// <returns>L'objet <see cref="Transaction"/> correspondant.</returns>
        /// <exception cref="ArgumentException">Levée si la transaction est introuvable.</exception>
        public Transaction GetTransaction(int transactionId)
        {
            return _transactionRepo.GetById(transactionId)
                ?? throw new ArgumentException($"Transaction #{transactionId} introuvable.");
        }

        // ═════════════════════════════════════════════════════════════════════
        //  CALCULS AGRÉGÉS (UTILES POUR LES RELEVÉS ET TABLEAUX DE BORD)
        // ═════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Calcule la somme totale des opérations créditrices sur un compte
        /// pour une période donnée (dépôts, virements entrants, ventes crypto).
        /// </summary>
        /// <param name="accountId">Identifiant du compte.</param>
        /// <param name="debut">Date de début de la période (inclusive).</param>
        /// <param name="fin">Date de fin de la période (inclusive).</param>
        /// <returns>Total des crédits en euros sur la période.</returns>
        public decimal TotalCredits(int accountId, DateTime debut, DateTime fin)
        {
            decimal total = 0;

            // Parcours de toutes les transactions du compte
            foreach (var t in _transactionRepo.GetByAccountId(accountId))
            {
                // IsCredit est calculé depuis le type (DEPOT, VIREMENT [ENTRANT], VENTE_CRYPTO)
                // On filtre également par la plage de dates
                if (t.IsCredit && t.DateTransaction >= debut && t.DateTransaction <= fin)
                    total += t.Montant;
            }

            return total;
        }

        /// <summary>
        /// Calcule la somme totale des opérations débitrices sur un compte
        /// pour une période donnée (retraits, virements sortants, achats crypto).
        /// </summary>
        /// <param name="accountId">Identifiant du compte.</param>
        /// <param name="debut">Date de début de la période (inclusive).</param>
        /// <param name="fin">Date de fin de la période (inclusive).</param>
        /// <returns>Total des débits en euros sur la période.</returns>
        public decimal TotalDebits(int accountId, DateTime debut, DateTime fin)
        {
            decimal total = 0;

            // IsDebit = !IsCredit — tout ce qui n'est pas un crédit est un débit
            foreach (var t in _transactionRepo.GetByAccountId(accountId))
            {
                if (t.IsDebit && t.DateTransaction >= debut && t.DateTransaction <= fin)
                    total += t.Montant;
            }

            return total;
        }
    }
}