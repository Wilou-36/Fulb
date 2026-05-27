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
    /// Logique métier relative aux comptes bancaires.
    /// Fait le lien entre la couche UI et AccountRepository.
    /// </summary>
    internal class AccountService
    {
        private readonly AccountRepository _accountRepo = new AccountRepository();
        private readonly UserRepository _userRepo = new UserRepository();

        // ── Création d'un compte ──────────────────────────────────────────────
        /// <summary>
        /// Crée un nouveau compte bancaire pour un utilisateur existant.
        /// Génère automatiquement un numéro de compte unique.
        /// </summary>
        /// <exception cref="ArgumentException">Si l'utilisateur n'existe pas.</exception>
        public Account CreerCompte(int userId)
        {
            var user = _userRepo.GetById(userId)
                ?? throw new ArgumentException($"Utilisateur #{userId} introuvable.");

            var account = new Account
            {
                UserId = userId,
                NumAccount = GenererNumeroCompte(),
                Solde = 0.00m,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            account.Id = _accountRepo.Add(account);
            return account;
        }

        // ── Consultation ──────────────────────────────────────────────────────
        /// <summary>Retourne un compte par son Id.</summary>
        /// <exception cref="ArgumentException">Si le compte n'existe pas.</exception>
        public Account GetCompte(int accountId)
        {
            return _accountRepo.GetById(accountId)
                ?? throw new ArgumentException($"Compte #{accountId} introuvable.");
        }

        /// <summary>Retourne tous les comptes d'un utilisateur.</summary>
        public List<Account> GetComptesByUser(int userId)
        {
            return _accountRepo.GetByUserId(userId);
        }

        /// <summary>Retourne le solde actuel d'un compte.</summary>
        public decimal GetSolde(int accountId)
        {
            return GetCompte(accountId).Solde;
        }

        // ── Dépôt ─────────────────────────────────────────────────────────────
        /// <summary>
        /// Effectue un dépôt sur un compte et retourne la transaction créée.
        /// </summary>
        /// <exception cref="ArgumentException">Si le montant est invalide.</exception>
        public Transaction Deposer(int accountId, decimal montant, string details = null)
        {
            ValiderMontant(montant);

            var account = GetCompte(accountId);
            var nouveauSolde = account.Solde + montant;
            _accountRepo.UpdateSolde(accountId, nouveauSolde);

            var transaction = new Transaction
            {
                AccountId = accountId,
                Type = TransactionType.DEPOT,
                Montant = montant,
                DateTransaction = DateTime.UtcNow,
                Details = details ?? $"Dépôt de {montant:C2}",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            var transactionRepo = new TransactionRepository();
            transaction.Id = transactionRepo.Add(transaction);
            return transaction;
        }

        // ── Retrait ───────────────────────────────────────────────────────────
        /// <summary>
        /// Effectue un retrait sur un compte et retourne la transaction créée.
        /// </summary>
        /// <exception cref="ArgumentException">Si le montant est invalide.</exception>
        /// <exception cref="InvalidOperationException">Si le solde est insuffisant.</exception>
        public Transaction Retirer(int accountId, decimal montant, string details = null)
        {
            ValiderMontant(montant);

            var account = GetCompte(accountId);

            if (account.Solde < montant)
                throw new InvalidOperationException(
                    $"Solde insuffisant. Solde actuel : {account.Solde:C2}, montant demandé : {montant:C2}.");

            _accountRepo.UpdateSolde(accountId, account.Solde - montant);

            var transaction = new Transaction
            {
                AccountId = accountId,
                Type = TransactionType.RETRAIT,
                Montant = montant,
                DateTransaction = DateTime.UtcNow,
                Details = details ?? $"Retrait de {montant:C2}",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            var transactionRepo = new TransactionRepository();
            transaction.Id = transactionRepo.Add(transaction);
            return transaction;
        }

        // ── Helpers privés ────────────────────────────────────────────────────
        private static void ValiderMontant(decimal montant)
        {
            if (montant <= 0)
                throw new ArgumentException("Le montant doit être strictement positif.");
        }

        /// <summary>
        /// Génère un numéro de compte interne unique au format FR + 16 chiffres.
        /// </summary>
        private static string GenererNumeroCompte()
        {
            var rng = new Random();
            return "FR" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                        + rng.Next(100, 999).ToString();
        }
    }
}
