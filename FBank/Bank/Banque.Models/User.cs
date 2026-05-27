using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Banque.Models
{
    public class User
    {
        // ── Clé primaire ──────────────────────────────────────────────────────
        public int Id { get; set; }

        // ── Données personnelles ──────────────────────────────────────────────
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;   // stocké haché (bcrypt)

        // ── Timestamps ───────────────────────────────────────────────────────
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // ── Navigation : Asso 1 — Un user possède plusieurs comptes ──────────
        /// <summary>
        /// Liste des comptes bancaires rattachés à cet utilisateur.
        /// (FK : accounts.user_id → users.id)
        /// </summary>
        public ICollection<Account> Accounts { get; set; } = new List<Account>();

        // ── Affichage ─────────────────────────────────────────────────────────
        public override string ToString() =>
            $"[User #{Id}] {Prenom} {Nom} — {Email}";
    }
}
