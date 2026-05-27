using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bank.Banque.Business
{
    // ══════════════════════════════════════════════════════════════════════════
    //  CryptoApiService — Récupération des cours en temps réel via CoinGecko
    //  API utilisée : https://api.coingecko.com/api/v3/coins/markets
    //  Gratuite, sans clé API, limite : ~10-30 requêtes/minute
    //
    //  Principe :
    //    FrmCrypto appelle GetCours() au chargement → reçoit un dictionnaire
    //    { "BTC" → 62000.00m, "ETH" → 3100.00m, ... } avec les cours réels.
    //    Ce dictionnaire remplace le dictionnaire statique _cours de FrmCrypto.
    //
    //  Cache intégré (5 minutes) :
    //    Évite de surcharger l'API à chaque ouverture du formulaire.
    //    Les cours sont rafraîchis uniquement si le cache est expiré.
    // ══════════════════════════════════════════════════════════════════════════
    internal class CryptoApiService
    {
        // ── Client HTTP partagé (pattern recommandé : une seule instance) ─────
        private static readonly HttpClient _http = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(10) // timeout de 10s pour éviter les blocages UI
        };

        // ── Cache en mémoire ──────────────────────────────────────────────────
        private static Dictionary<string, decimal> _cache = null;
        private static DateTime _cacheTime = DateTime.MinValue;
        private static readonly TimeSpan _cacheDuree = TimeSpan.FromMinutes(5);

        // ── Symboles suivis ───────────────────────────────────────────────────
        // Identifiants CoinGecko (ids) et leurs symboles correspondants
        private static readonly Dictionary<string, string> _ids = new Dictionary<string, string>
        {
            { "bitcoin",  "BTC"  },
            { "ethereum", "ETH"  },
            { "solana",   "SOL"  },
            { "tether",   "USDT" },
            { "binancecoin", "BNB" }
        };

        // ══════════════════════════════════════════════════════════════════════
        //  MÉTHODE PRINCIPALE
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Retourne un dictionnaire { symbole → cours en euros } depuis CoinGecko.
        /// Utilise le cache pendant 5 minutes pour éviter les appels répétés.
        /// En cas d'erreur réseau, retourne les cours de secours (fallback).
        /// </summary>
        /// <returns>
        /// Dictionnaire : { "BTC" → 62000.00m, "ETH" → 3100.00m, ... }
        /// Jamais null — retourne les cours de fallback si l'API est inaccessible.
        /// </returns>
        public static Dictionary<string, decimal> GetCours()
        {
            // Retourne le cache si encore valide (< 5 minutes)
            if (_cache != null && DateTime.UtcNow - _cacheTime < _cacheDuree)
                return _cache;

            try
            {
                // Construction de l'URL avec les ids CoinGecko séparés par des virgules
                string ids = string.Join(",", _ids.Keys);
                string url = $"https://api.coingecko.com/api/v3/coins/markets" +
                             $"?vs_currency=eur&ids={ids}&order=market_cap_desc" +
                             $"&per_page=10&page=1&sparkline=false";

                // Appel synchrone (WinForms n'est pas async par défaut)
                // Pour une meilleure UX, envisager un BackgroundWorker
                var response = _http.GetStringAsync(url).Result;

                // Parsing du JSON retourné par CoinGecko
                var cours = ParseResponse(response);

                // Mise en cache si le parsing a réussi
                if (cours.Count > 0)
                {
                    _cache = cours;
                    _cacheTime = DateTime.UtcNow;
                    return cours;
                }
            }
            catch
            {
                // Silencieux : erreur réseau, timeout, rate limit (429)...
                // On retourne le fallback sans bloquer l'application
            }

            // En cas d'échec → cours de secours pour que l'application continue
            return GetFallback();
        }

        /// <summary>
        /// Version asynchrone recommandée pour éviter de bloquer le thread UI.
        /// Appeler depuis un BackgroundWorker ou une méthode async.
        /// </summary>
        public static async Task<Dictionary<string, decimal>> GetCoursAsync()
        {
            if (_cache != null && DateTime.UtcNow - _cacheTime < _cacheDuree)
                return _cache;

            try
            {
                string ids = string.Join(",", _ids.Keys);
                string url = $"https://api.coingecko.com/api/v3/coins/markets" +
                             $"?vs_currency=eur&ids={ids}&order=market_cap_desc" +
                             $"&per_page=10&page=1&sparkline=false";

                var response = await _http.GetStringAsync(url);
                var cours = ParseResponse(response);

                if (cours.Count > 0)
                {
                    _cache = cours;
                    _cacheTime = DateTime.UtcNow;
                    return cours;
                }
            }
            catch { }

            return GetFallback();
        }

        // ══════════════════════════════════════════════════════════════════════
        //  PARSING JSON
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Parse la réponse JSON de CoinGecko et construit le dictionnaire de cours.
        /// Format attendu : tableau d'objets avec "symbol" et "current_price".
        /// Exemple : [{ "symbol": "btc", "current_price": 61234.5 }, ...]
        /// </summary>
        private static Dictionary<string, decimal> ParseResponse(string json)
        {
            var result = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            var doc = JsonDocument.Parse(json);

            foreach (var item in doc.RootElement.EnumerateArray())
            {
                // Lecture du symbole (ex: "btc") → passage en majuscules ("BTC")
                if (!item.TryGetProperty("symbol", out var symProp)) continue;
                string symbole = symProp.GetString()?.ToUpper();
                if (string.IsNullOrEmpty(symbole)) continue;

                // Lecture du cours actuel en euros
                if (!item.TryGetProperty("current_price", out var priceProp)) continue;
                decimal prix = priceProp.GetDecimal();

                result[symbole] = prix;
            }

            return result;
        }

        // ══════════════════════════════════════════════════════════════════════
        //  COURS DE SECOURS (FALLBACK)
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Cours de secours utilisés si l'API est inaccessible.
        /// Retournés à la place d'une exception pour ne pas bloquer l'application.
        /// Ces valeurs sont approximatives — elles signalent à l'utilisateur
        /// que les cours ne sont pas en temps réel (via le label "cours simulés").
        /// </summary>
        private static Dictionary<string, decimal> GetFallback() =>
            new Dictionary<string, decimal>
            {
                { "BTC",  62000.00m },
                { "ETH",   3100.00m },
                { "SOL",    145.00m },
                { "USDT",     1.00m },
                { "BNB",    580.00m }
            };
    }
}