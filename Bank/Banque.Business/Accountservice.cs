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
    //  AccountService — Couche métier des comptes bancaires
    //  Responsabilité : orchestrer les opérations sur les comptes (création,
    //  consultation, dépôt, retrait) en faisant appel aux repositories.
    //  Ne contient AUCUNE requête SQL directe — délègue tout à Banque.Data.
    // ══════════════════════════════════════════════════════════════════════════
    internal class AccountService
    {
        // ── Dépendances injectées ─────────────────────────────────────────────
        // Repository des comptes : INSERT, SELECT, UPDATE sur la table accounts
        private readonly AccountRepository _accountRepo = new AccountRepository();

        // Repository des utilisateurs : nécessaire pour vérifier l'existence
        // de l'utilisateur avant toute création de compte
        private readonly UserRepository _userRepo = new UserRepository();

        // ═════════════════════════════════════════════════════════════════════
        //  CRÉATION DE COMPTE
        // ═════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Crée un nouveau compte bancaire pour un utilisateur existant.
        /// Génère automatiquement un numéro de compte unique au format FR + timestamp + random.
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur propriétaire du compte.</param>
        /// <returns>L'objet <see cref="Account"/> créé avec son Id généré par la base.</returns>
        /// <exception cref="ArgumentException">
        /// Levée si aucun utilisateur ne correspond à <paramref name="userId"/>.
        /// </exception>
        public Account CreerCompte(int userId)
        {
            // Vérification préalable : l'utilisateur doit exister en base
            // ?? throw = opérateur null-coalescing qui lève une exception si null
            var user = _userRepo.GetById(userId)
                ?? throw new ArgumentException($"Utilisateur #{userId} introuvable.");

            // Construction de l'objet Account avec les valeurs initiales
            var account = new Account
            {
                UserId = userId,
                NumAccount = GenererNumeroCompte(), // numéro unique généré automatiquement
                Solde = 0.00m,                 // solde initial à zéro
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            // Persistance en base — Add() retourne l'Id généré (IDENTITY SQL)
            account.Id = _accountRepo.Add(account);

            return account;
        }

        // ═════════════════════════════════════════════════════════════════════
        //  CONSULTATION
        // ═════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Retourne un compte bancaire par son identifiant.
        /// </summary>
        /// <param name="accountId">Clé primaire du compte.</param>
        /// <returns>L'objet <see cref="Account"/> correspondant.</returns>
        /// <exception cref="ArgumentException">Levée si le compte est introuvable.</exception>
        public Account GetCompte(int accountId)
        {
            // GetById retourne null si inexistant → on lève une exception métier explicite
            return _accountRepo.GetById(accountId)
                ?? throw new ArgumentException($"Compte #{accountId} introuvable.");
        }

        /// <summary>
        /// Retourne la liste de tous les comptes bancaires d'un utilisateur.
        /// Un utilisateur peut posséder plusieurs comptes (relation 1-*).
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur (FK user_id).</param>
        /// <returns>Liste pouvant être vide si aucun compte n'existe.</returns>
        public List<Account> GetComptesByUser(int userId)
        {
            // Délègue directement au repository — pas de logique métier supplémentaire
            return _accountRepo.GetByUserId(userId);
        }

        /// <summary>
        /// Retourne le solde actuel d'un compte bancaire.
        /// Raccourci vers <see cref="GetCompte"/> pour éviter de charger l'objet complet.
        /// </summary>
        /// <param name="accountId">Identifiant du compte.</param>
        /// <returns>Solde en euros (DECIMAL 15,2).</returns>
        public decimal GetSolde(int accountId)
        {
            // Récupère le compte complet puis extrait uniquement le solde
            return GetCompte(accountId).Solde;
        }

        // ═════════════════════════════════════════════════════════════════════
        //  DÉPÔT
        // ═════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Effectue un dépôt sur un compte bancaire.
        /// Crédite le solde et enregistre une transaction de type DEPOT.
        /// </summary>
        /// <param name="accountId">Compte à créditer.</param>
        /// <param name="montant">Montant à déposer (doit être > 0).</param>
        /// <param name="details">Description optionnelle de l'opération.</param>
        /// <returns>La <see cref="Transaction"/> créée et persistée en base.</returns>
        /// <exception cref="ArgumentException">Si le montant est ≤ 0.</exception>
        public Transaction Deposer(int accountId, decimal montant, string details = null)
        {
            // Validation métier : montant strictement positif
            ValiderMontant(montant);

            // Chargement du compte — lève une exception si inexistant
            var account = GetCompte(accountId);

            // Calcul du nouveau solde après dépôt
            var nouveauSolde = account.Solde + montant;

            // Mise à jour du solde en base (UPDATE accounts SET solde = ...)
            _accountRepo.UpdateSolde(accountId, nouveauSolde);

            // Construction de la transaction à enregistrer dans l'historique
            var transaction = new Transaction
            {
                AccountId = accountId,
                Type = TransactionType.DEPOT,  // enum → 'DEPOT' en base
                Montant = montant,
                DateTransaction = DateTime.UtcNow,
                // Si aucun détail fourni, on génère un libellé automatique
                Details = details ?? $"Dépôt de {montant:C2}",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            // Instanciation locale du TransactionRepository pour persister l'opération
            var transactionRepo = new TransactionRepository();
            transaction.Id = transactionRepo.Add(transaction);

            return transaction;
        }

        // ═════════════════════════════════════════════════════════════════════
        //  RETRAIT
        // ═════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Effectue un retrait sur un compte bancaire.
        /// Débite le solde et enregistre une transaction de type RETRAIT.
        /// </summary>
        /// <param name="accountId">Compte à débiter.</param>
        /// <param name="montant">Montant à retirer (doit être > 0 et ≤ solde disponible).</param>
        /// <param name="details">Description optionnelle de l'opération.</param>
        /// <returns>La <see cref="Transaction"/> créée et persistée en base.</returns>
        /// <exception cref="ArgumentException">Si le montant est ≤ 0.</exception>
        /// <exception cref="InvalidOperationException">Si le solde est insuffisant.</exception>
        public Transaction Retirer(int accountId, decimal montant, string details = null)
        {
            // Validation métier : montant strictement positif
            ValiderMontant(montant);

            // Chargement du compte courant
            var account = GetCompte(accountId);

            // Règle métier : on ne peut pas retirer plus que le solde disponible
            if (account.Solde < montant)
                throw new InvalidOperationException(
                    $"Solde insuffisant. Solde actuel : {account.Solde:C2}, montant demandé : {montant:C2}.");

            // Débit du compte : nouveau solde = solde actuel - montant
            _accountRepo.UpdateSolde(accountId, account.Solde - montant);

            // Enregistrement de la transaction de retrait dans l'historique
            var transaction = new Transaction
            {
                AccountId = accountId,
                Type = TransactionType.RETRAIT, // enum → 'RETRAIT' en base
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

        // ═════════════════════════════════════════════════════════════════════
        //  MÉTHODES PRIVÉES / HELPERS
        // ═════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Valide qu'un montant est strictement positif.
        /// Centralisé ici pour éviter la duplication dans Deposer() et Retirer().
        /// </summary>
        /// <param name="montant">Montant à valider.</param>
        /// <exception cref="ArgumentException">Si montant ≤ 0.</exception>
        private static void ValiderMontant(decimal montant)
        {
            if (montant <= 0)
                throw new ArgumentException("Le montant doit être strictement positif.");
        }

        /// <summary>
        /// Génère un numéro de compte bancaire unique.
        /// Format : FR + timestamp Unix en millisecondes + 3 chiffres aléatoires.
        /// Exemple : FR17389274839274123
        /// </summary>
        /// <returns>Chaîne de caractères représentant le numéro de compte.</returns>
        private static string GenererNumeroCompte()
        {
            var rng = new Random();

            // Combinaison timestamp + aléatoire pour garantir l'unicité
            return "FR" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                        + rng.Next(100, 999).ToString();
        }
    }
}