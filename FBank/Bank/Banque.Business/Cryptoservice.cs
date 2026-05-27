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
    /// Logique métier relative aux opérations sur les cryptomonnaies.
    /// Gère l'achat, la vente et la consultation des portefeuilles crypto.
    /// </summary>
    internal class CryptoService
    {
        private readonly CryptoRepository _cryptoRepo = new CryptoRepository();
        private readonly AccountRepository _accountRepo = new AccountRepository();
        private readonly TransactionRepository _transactionRepo = new TransactionRepository();

        // ── Achat de crypto ───────────────────────────────────────────────────
        /// <summary>
        /// Achète une quantité de cryptomonnaie pour un compte donné.
        /// Débite le solde en euros et crédite le wallet crypto.
        /// </summary>
        /// <param name="accountId">Compte bancaire source.</param>
        /// <param name="symbole">Symbole de la crypto (ex : BTC, ETH).</param>
        /// <param name="quantite">Quantité de crypto à acheter.</param>
        /// <param name="coursActuel">Cours actuel de la crypto en euros.</param>
        /// <exception cref="ArgumentException">Paramètres invalides.</exception>
        /// <exception cref="InvalidOperationException">Solde insuffisant.</exception>
        public Cryptowallet AcheterCrypto(int accountId, string symbole, decimal quantite, decimal coursActuel)
        {
            ValiderParametresCrypto(symbole, quantite, coursActuel);

            var account = _accountRepo.GetById(accountId)
                ?? throw new ArgumentException($"Compte #{accountId} introuvable.");

            decimal coutTotal = quantite * coursActuel;

            if (account.Solde < coutTotal)
                throw new InvalidOperationException(
                    $"Solde insuffisant. Disponible : {account.Solde:C2}, coût : {coutTotal:C2}.");

            // ── Débit du compte en euros ──────────────────────────────────────
            _accountRepo.UpdateSolde(accountId, account.Solde - coutTotal);

            // ── Mise à jour ou création du wallet ─────────────────────────────
            var wallet = _cryptoRepo.GetByAccountIdAndSymbole(accountId, symbole);

            if (wallet is null)
            {
                wallet = new Cryptowallet
                {
                    AccountId = accountId,
                    Symbole = symbole.ToUpper(),
                    Quantite = quantite,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };
                wallet.Id = _cryptoRepo.Add(wallet);
            }
            else
            {
                _cryptoRepo.UpdateQuantite(wallet.Id, wallet.Quantite + quantite);
                wallet.Quantite += quantite;
            }

            // ── Enregistrement de la transaction ──────────────────────────────
            _transactionRepo.Add(new Transaction
            {
                AccountId = accountId,
                Type = TransactionType.ACHAT_CRYPTO,
                Montant = coutTotal,
                DateTransaction = DateTime.UtcNow,
                Details = $"Achat {quantite:F8} {symbole.ToUpper()} @ {coursActuel:C2}/unité",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            });

            return wallet;
        }

        // ── Vente de crypto ───────────────────────────────────────────────────
        /// <summary>
        /// Vend une quantité de cryptomonnaie et crédite le compte en euros.
        /// </summary>
        /// <param name="accountId">Compte bancaire destinataire des euros.</param>
        /// <param name="symbole">Symbole de la crypto à vendre.</param>
        /// <param name="quantite">Quantité à vendre.</param>
        /// <param name="coursActuel">Cours actuel de la crypto en euros.</param>
        /// <exception cref="InvalidOperationException">Quantité insuffisante dans le wallet.</exception>
        public Cryptowallet VendreCrypto(int accountId, string symbole, decimal quantite, decimal coursActuel)
        {
            ValiderParametresCrypto(symbole, quantite, coursActuel);

            var account = _accountRepo.GetById(accountId)
                ?? throw new ArgumentException($"Compte #{accountId} introuvable.");

            var wallet = _cryptoRepo.GetByAccountIdAndSymbole(accountId, symbole)
                ?? throw new InvalidOperationException(
                    $"Aucun wallet {symbole.ToUpper()} trouvé pour le compte #{accountId}.");

            if (wallet.Quantite < quantite)
                throw new InvalidOperationException(
                    $"Quantité insuffisante. Disponible : {wallet.Quantite:F8} {symbole.ToUpper()}, " +
                    $"demandée : {quantite:F8}.");

            decimal montantEuros = quantite * coursActuel;

            // ── Mise à jour du wallet ─────────────────────────────────────────
            _cryptoRepo.UpdateQuantite(wallet.Id, wallet.Quantite - quantite);
            wallet.Quantite -= quantite;

            // ── Crédit du compte en euros ─────────────────────────────────────
            _accountRepo.UpdateSolde(accountId, account.Solde + montantEuros);

            // ── Enregistrement de la transaction ──────────────────────────────
            _transactionRepo.Add(new Transaction
            {
                AccountId = accountId,
                Type = TransactionType.VENTE_CRYPTO,
                Montant = montantEuros,
                DateTransaction = DateTime.UtcNow,
                Details = $"Vente {quantite:F8} {symbole.ToUpper()} @ {coursActuel:C2}/unité",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            });

            return wallet;
        }

        // ── Consultation ──────────────────────────────────────────────────────
        /// <summary>
        /// Retourne tous les wallets crypto d'un compte.
        /// </summary>
        public List<Cryptowallet> GetWallets(int accountId)
        {
            return _cryptoRepo.GetByAccountId(accountId);
        }

        /// <summary>
        /// Retourne un wallet spécifique pour un compte et un symbole donnés.
        /// </summary>
        /// <exception cref="InvalidOperationException">Si le wallet n'existe pas.</exception>
        public Cryptowallet GetWallet(int accountId, string symbole)
        {
            return _cryptoRepo.GetByAccountIdAndSymbole(accountId, symbole)
                ?? throw new InvalidOperationException(
                    $"Aucun wallet {symbole.ToUpper()} pour le compte #{accountId}.");
        }

        /// <summary>
        /// Calcule la valeur totale en euros de tous les wallets d'un compte
        /// selon les cours fournis.
        /// </summary>
        /// <param name="accountId">Identifiant du compte.</param>
        /// <param name="cours">Dictionnaire symbole → cours actuel en euros.</param>
        public decimal GetValeurTotalePortefeuille(int accountId, Dictionary<string, decimal> cours)
        {
            decimal total = 0;
            foreach (var wallet in _cryptoRepo.GetByAccountId(accountId))
            {
                if (cours.TryGetValue(wallet.Symbole, out decimal coursActuel))
                    total += wallet.ValeurEnEuros(coursActuel);
            }
            return total;
        }

        // ── Helpers privés ────────────────────────────────────────────────────
        private static void ValiderParametresCrypto(string symbole, decimal quantite, decimal cours)
        {
            if (string.IsNullOrWhiteSpace(symbole))
                throw new ArgumentException("Le symbole de la cryptomonnaie est obligatoire.");
            if (quantite <= 0)
                throw new ArgumentException("La quantité doit être strictement positive.");
            if (cours <= 0)
                throw new ArgumentException("Le cours doit être strictement positif.");
        }
    }
}
