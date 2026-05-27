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
    //  CryptoService — Couche métier des cryptomonnaies
    //  Responsabilité : gérer l'achat, la vente et la consultation des
    //  portefeuilles crypto rattachés à un compte bancaire.
    //
    //  Flux ACHAT  : compte euros ──(débit)──► wallet crypto ──(crédit)
    //  Flux VENTE  : wallet crypto ──(débit)──► compte euros ──(crédit)
    //  Dans les deux cas, une Transaction est enregistrée pour la traçabilité.
    // ══════════════════════════════════════════════════════════════════════════
    internal class CryptoService
    {
        // ── Dépendances injectées ─────────────────────────────────────────────
        // Repository des wallets : CRUD sur la table crypto_wallets
        private readonly CryptoRepository _cryptoRepo = new CryptoRepository();

        // Repository des comptes : nécessaire pour débiter/créditer en euros
        private readonly AccountRepository _accountRepo = new AccountRepository();

        // Repository des transactions : pour enregistrer chaque opération crypto
        private readonly TransactionRepository _transactionRepo = new TransactionRepository();

        // ═════════════════════════════════════════════════════════════════════
        //  ACHAT DE CRYPTOMONNAIE
        // ═════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Achète une quantité de cryptomonnaie pour un compte bancaire donné.
        /// Débite le compte en euros du coût total (quantité × cours)
        /// et crédite le wallet crypto correspondant.
        /// Si aucun wallet n'existe pour ce symbole, il est créé automatiquement.
        /// </summary>
        /// <param name="accountId">Identifiant du compte bancaire source (en euros).</param>
        /// <param name="symbole">Symbole de la crypto en majuscules (ex : BTC, ETH, SOL).</param>
        /// <param name="quantite">Nombre d'unités à acheter (précision 8 décimales).</param>
        /// <param name="coursActuel">Prix unitaire de la crypto en euros au moment de l'achat.</param>
        /// <returns>
        /// L'objet <see cref="Cryptowallet"/> mis à jour ou créé après l'achat.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Levée si un paramètre est invalide ou si le compte est introuvable.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Levée si le solde en euros est insuffisant pour couvrir le coût total.
        /// </exception>
        public Cryptowallet AcheterCrypto(int accountId, string symbole, decimal quantite, decimal coursActuel)
        {
            // ── Validation des paramètres d'entrée ────────────────────────────
            ValiderParametresCrypto(symbole, quantite, coursActuel);

            // Chargement du compte bancaire — null si inexistant
            var account = _accountRepo.GetById(accountId)
                ?? throw new ArgumentException($"Compte #{accountId} introuvable.");

            // Calcul du coût total de la transaction : quantité × cours
            decimal coutTotal = quantite * coursActuel;

            // Règle métier : le solde doit couvrir l'intégralité du coût
            if (account.Solde < coutTotal)
                throw new InvalidOperationException(
                    $"Solde insuffisant. Disponible : {account.Solde:C2}, coût : {coutTotal:C2}.");

            // ── Débit du compte en euros ──────────────────────────────────────
            // Le compte perd le montant équivalent en euros
            _accountRepo.UpdateSolde(accountId, account.Solde - coutTotal);

            // ── Mise à jour ou création du wallet crypto ──────────────────────
            // Recherche d'un wallet existant pour ce compte et ce symbole
            // (contrainte unique : account_id + symbole)
            var wallet = _cryptoRepo.GetByAccountIdAndSymbole(accountId, symbole);

            if (wallet is null)
            {
                // Aucun wallet existant → création d'un nouveau portefeuille
                wallet = new Cryptowallet
                {
                    AccountId = accountId,
                    Symbole = symbole.ToUpper(), // stockage toujours en majuscules
                    Quantite = quantite,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };
                // Insertion en base — Add() retourne l'Id généré
                wallet.Id = _cryptoRepo.Add(wallet);
            }
            else
            {
                // Wallet existant → on incrémente la quantité détenue
                _cryptoRepo.UpdateQuantite(wallet.Id, wallet.Quantite + quantite);
                wallet.Quantite += quantite; // mise à jour de l'objet en mémoire
            }

            // ── Enregistrement de la transaction dans l'historique ────────────
            // Permet de tracer chaque achat avec le détail du cours au moment de l'achat
            _transactionRepo.Add(new Transaction
            {
                AccountId = accountId,
                Type = TransactionType.ACHAT_CRYPTO,
                Montant = coutTotal,
                DateTransaction = DateTime.UtcNow,
                // Détail lisible : quantité + symbole + cours unitaire
                Details = $"Achat {quantite:F8} {symbole.ToUpper()} @ {coursActuel:C2}/unité",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            });

            return wallet;
        }

        // ═════════════════════════════════════════════════════════════════════
        //  VENTE DE CRYPTOMONNAIE
        // ═════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Vend une quantité de cryptomonnaie et crédite le compte bancaire en euros.
        /// Décrémente le wallet crypto du montant vendu
        /// et crédite le compte du produit de la vente (quantité × cours).
        /// </summary>
        /// <param name="accountId">Compte bancaire destinataire des euros issus de la vente.</param>
        /// <param name="symbole">Symbole de la crypto à vendre (ex : ETH, SOL).</param>
        /// <param name="quantite">Quantité à vendre (doit être ≤ quantité détenue).</param>
        /// <param name="coursActuel">Prix unitaire en euros au moment de la vente.</param>
        /// <returns>
        /// L'objet <see cref="Cryptowallet"/> mis à jour après la vente.
        /// </returns>
        /// <exception cref="ArgumentException">Paramètres invalides ou compte introuvable.</exception>
        /// <exception cref="InvalidOperationException">
        /// Levée si aucun wallet n'existe ou si la quantité disponible est insuffisante.
        /// </exception>
        public Cryptowallet VendreCrypto(int accountId, string symbole, decimal quantite, decimal coursActuel)
        {
            // ── Validation des paramètres d'entrée ────────────────────────────
            ValiderParametresCrypto(symbole, quantite, coursActuel);

            // Chargement du compte bancaire bénéficiaire de la vente
            var account = _accountRepo.GetById(accountId)
                ?? throw new ArgumentException($"Compte #{accountId} introuvable.");

            // Vérification de l'existence du wallet pour ce symbole
            var wallet = _cryptoRepo.GetByAccountIdAndSymbole(accountId, symbole)
                ?? throw new InvalidOperationException(
                    $"Aucun wallet {symbole.ToUpper()} trouvé pour le compte #{accountId}.");

            // Règle métier : impossible de vendre plus que ce que l'on possède
            if (wallet.Quantite < quantite)
                throw new InvalidOperationException(
                    $"Quantité insuffisante. Disponible : {wallet.Quantite:F8} {symbole.ToUpper()}, " +
                    $"demandée : {quantite:F8}.");

            // Calcul du produit de la vente en euros
            decimal montantEuros = quantite * coursActuel;

            // ── Débit du wallet crypto ────────────────────────────────────────
            // La quantité détenue dans le portefeuille diminue
            _cryptoRepo.UpdateQuantite(wallet.Id, wallet.Quantite - quantite);
            wallet.Quantite -= quantite; // mise à jour de l'objet en mémoire

            // ── Crédit du compte bancaire en euros ────────────────────────────
            // Le compte reçoit l'équivalent en euros de la vente
            _accountRepo.UpdateSolde(accountId, account.Solde + montantEuros);

            // ── Enregistrement de la transaction dans l'historique ────────────
            _transactionRepo.Add(new Transaction
            {
                AccountId = accountId,
                Type = TransactionType.VENTE_CRYPTO,
                Montant = montantEuros,
                DateTransaction = DateTime.UtcNow,
                // Détail lisible : quantité vendue + cours au moment de la vente
                Details = $"Vente {quantite:F8} {symbole.ToUpper()} @ {coursActuel:C2}/unité",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            });

            return wallet;
        }

        // ═════════════════════════════════════════════════════════════════════
        //  CONSULTATION DES WALLETS
        // ═════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Retourne la liste de tous les portefeuilles crypto d'un compte bancaire.
        /// Chaque ligne représente une devise différente (BTC, ETH, etc.).
        /// </summary>
        /// <param name="accountId">Identifiant du compte (FK account_id).</param>
        /// <returns>
        /// Liste des wallets triée par symbole. Vide si aucun achat n'a été effectué.
        /// </returns>
        public List<Cryptowallet> GetWallets(int accountId)
        {
            // Délègue au repository — ORDER BY symbole appliqué en SQL
            return _cryptoRepo.GetByAccountId(accountId);
        }

        /// <summary>
        /// Retourne le portefeuille d'une cryptomonnaie spécifique pour un compte.
        /// </summary>
        /// <param name="accountId">Identifiant du compte.</param>
        /// <param name="symbole">Symbole de la crypto recherchée (ex : BTC).</param>
        /// <returns>L'objet <see cref="Cryptowallet"/> correspondant.</returns>
        /// <exception cref="InvalidOperationException">
        /// Levée si aucun wallet n'existe pour ce couple (compte, symbole).
        /// </exception>
        public Cryptowallet GetWallet(int accountId, string symbole)
        {
            // Exploite la contrainte unique (account_id, symbole) de la table
            return _cryptoRepo.GetByAccountIdAndSymbole(accountId, symbole)
                ?? throw new InvalidOperationException(
                    $"Aucun wallet {symbole.ToUpper()} pour le compte #{accountId}.");
        }

        /// <summary>
        /// Calcule la valeur totale en euros de l'ensemble du portefeuille crypto
        /// d'un compte, en appliquant les cours actuels fournis.
        /// Utile pour l'affichage du tableau de bord et les statistiques.
        /// </summary>
        /// <param name="accountId">Identifiant du compte.</param>
        /// <param name="cours">
        /// Dictionnaire associant chaque symbole à son cours actuel en euros.
        /// Exemple : { "BTC" → 62000.00m, "ETH" → 3100.00m }
        /// Les symboles absents du dictionnaire sont ignorés dans le calcul.
        /// </param>
        /// <returns>Valeur totale du portefeuille en euros.</returns>
        public decimal GetValeurTotalePortefeuille(int accountId, Dictionary<string, decimal> cours)
        {
            decimal total = 0;

            // Parcours de tous les wallets du compte
            foreach (var wallet in _cryptoRepo.GetByAccountId(accountId))
            {
                // On ne calcule que si le cours est fourni pour ce symbole
                // TryGetValue évite une KeyNotFoundException
                if (cours.TryGetValue(wallet.Symbole, out decimal coursActuel))
                {
                    // ValeurEnEuros() est défini dans le modèle CryptoWallet :
                    // return Quantite * coursActuel
                    total += wallet.ValeurEnEuros(coursActuel);
                }
            }

            return total;
        }

        // ═════════════════════════════════════════════════════════════════════
        //  MÉTHODES PRIVÉES / HELPERS
        // ═════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Valide les trois paramètres communs aux opérations d'achat et de vente.
        /// Centralisé ici pour éviter la duplication dans AcheterCrypto() et VendreCrypto().
        /// </summary>
        /// <param name="symbole">Ne doit pas être vide ou null.</param>
        /// <param name="quantite">Doit être strictement positive.</param>
        /// <param name="cours">Doit être strictement positif.</param>
        /// <exception cref="ArgumentException">Levée dès le premier paramètre invalide.</exception>
        private static void ValiderParametresCrypto(string symbole, decimal quantite, decimal cours)
        {
            // Vérification du symbole : ne doit pas être null, vide ou blanc
            if (string.IsNullOrWhiteSpace(symbole))
                throw new ArgumentException("Le symbole de la cryptomonnaie est obligatoire.");

            // La quantité doit être un nombre positif non nul
            if (quantite <= 0)
                throw new ArgumentException("La quantité doit être strictement positive.");

            // Le cours ne peut pas être nul ou négatif (prix d'une crypto)
            if (cours <= 0)
                throw new ArgumentException("Le cours doit être strictement positif.");
        }
    }
}