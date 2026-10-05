using UnityEngine;

namespace Billar.Game
{
    // RF-04 Sesión persistente (responsable: Miguel).
    public static partial class AuthService
    {
        /// <summary>Aviso pendiente de mostrar en el login (sesión vencida).</summary>
        public static string PendingNotice { get; private set; }

        /// <summary>Vuelve a entrar solo si la sesión anterior sigue guardada (RF-04).</summary>
        public static bool TryRestoreSession()
        {
            string email = PlayerPrefs.GetString(SessionKey, string.Empty);
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }

            foreach (UserRecord user in Load().users)
            {
                if (user.email == email)
                {
                    Current = user;
                    return true;
                }
            }

            // Había una sesión guardada pero ya no es válida.
            PlayerPrefs.DeleteKey(SessionKey);
            PlayerPrefs.Save();
            PendingNotice = "Tu sesión ha expirado. Inicia sesión nuevamente.";
            return false;
        }

        /// <summary>Entrega el aviso pendiente una sola vez.</summary>
        public static string TakePendingNotice()
        {
            string notice = PendingNotice;
            PendingNotice = null;
            return notice;
        }
    }
}
