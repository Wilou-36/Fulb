using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bank.Banque.Data;
using Bank.Banque.Models;

namespace Bank.Banque.Business
{
    /// <summary>
    /// Logique métier relative aux virements et à l'historique des transactions.
    /// </summary>
    internal class TransactionService
    {
        private readonly TransactionRepository _transactionRepo = new TransactionRepository();
        private readonly AccountRepository _accountRepo = new AccountRepository();

        // ── Virement entre comptes ────────────────────────────────────────────
        /// <summary>
        /// Effectue un virement d'un compte source vers un compte destinataire.
        /// Les deux mises à jour de solde sont atomiques (transaction SQL).
        /// </summary>
        /// <exception cref="ArgumentException">Comptes identiques ou montant invalide.</exception>
        /// <exception cref="InvalidOperationException">Solde insuffisant.</exception>
        public void EffectuerVirement(int accountSourceId, int accountDestId, decimal montant, string details = null)
        {
            if (accountSourceId == accountDestId)
                throw new ArgumentException("Le compte source et le compte destinataire doivent être différents.");

            if (montant <= 0)
                throw new ArgumentException("Le montant du virement doit être strictement positif.");

            var source = _accountRepo.GetById(accountSourceId)
                ?? throw new ArgumentException($"Compte source #{accountSourceId} introuvable.");

            var destination = _accountRepo.GetById(accountDestId)
                ?? throw new ArgumentException($"Compte destinataire #{accountDestId} introuvable.");

            if (source.Solde < montant)
                throw new InvalidOperationException(
                    $"Solde insuffisant. Disponible : {source.Solde:C2}, demandé : {montant:C2}.");

            // ── Mise à jour des soldes ────────────────────────────────────────
            _accountRepo.UpdateSolde(accountSourceId, source.Solde - montant);
            _accountRepo.UpdateSolde(accountDestId, destination.Solde + montant);

            var now = DateTime.UtcNow;
            var detail = details ?? $"Virement {source.NumAccount} → {destination.NumAccount}";

            // ── Enregistrement côté émetteur ──────────────────────────────────
            _transactionRepo.Add(new Transaction
            {
                AccountId = accountSourceId,
                Type = TransactionType.VIREMENT,
                Montant = montant,
                DateTransaction = now,
                Details = $"[SORTANT] {detail}",
                CreatedAt = now,
                UpdatedAt = now,
            });

            // ── Enregistrement côté bénéficiaire ──────────────────────────────
            _transactionRepo.Add(new Transaction
            {
                AccountId = accountDestId,
                Type = TransactionType.VIREMENT,
                Montant = montant,
                DateTransaction = now,
                Details = $"[ENTRANT] {detail}",
                CreatedAt = now,
                UpdatedAt = now,
            });
        }

        // ── Historique ────────────────────────────────────────────────────────
        /// <summary>
        /// Retourne l'historique complet des transactions d'un compte,
        /// trié de la plus récente à la plus ancienne.
        /// </summary>
        public List<Transaction> GetHistorique(int accountId)
        {
            return _transactionRepo.GetByAccountId(accountId);
        }

        /// <summary>
        /// Retourne les transactions d'un compte filtrées par type.
        /// </summary>
        public List<Transaction> GetHistoriqueParType(int accountId, TransactionType type)
        {
            return _transactionRepo.GetByAccountIdAndType(accountId, type);
        }

        /// <summary>
        /// Retourne une transaction par son Id.
        /// </summary>
        /// <exception cref="ArgumentException">Si la transaction n'existe pas.</exception>
        public Transaction GetTransaction(int transactionId)
        {
            return _transactionRepo.GetById(transactionId)
                ?? throw new ArgumentException($"Transaction #{transactionId} introuvable.");
        }

        // ── Calculs ───────────────────────────────────────────────────────────
        /// <summary>
        /// Calcule le total des crédits sur un compte pour une période donnée.
        /// </summary>
        public decimal TotalCredits(int accountId, DateTime debut, DateTime fin)
        {
            decimal total = 0;
            foreach (var t in _transactionRepo.GetByAccountId(accountId))
            {
                if (t.IsCredit && t.DateTransaction >= debut && t.DateTransaction <= fin)
                    total += t.Montant;
            }
            return total;
        }

        /// <summary>
        /// Calcule le total des débits sur un compte pour une période donnée.
        /// </summary>
        public decimal TotalDebits(int accountId, DateTime debut, DateTime fin)
        {
            decimal total = 0;
            foreach (var t in _transactionRepo.GetByAccountId(accountId))
            {
                if (t.IsDebit && t.DateTransaction >= debut && t.DateTransaction <= fin)
                    total += t.Montant;
            }
            return total;
        }
    }
}
