using UnityEngine;

namespace Billar.Game
{
    // RF-03 Inicio de sesión (responsable: Johan).
    public static partial class AuthService
    {
        private const string InvalidCredentials = "Correo o contraseña incorrectos.";

        public static bool Login(string email, string password, out string error)
        {
            email = (email ?? string.Empty).Trim().ToLowerInvariant();
            password = password ?? string.Empty;

            if (email.Length == 0 || password.Length == 0)
            {
                error = "Ingresa tu correo y tu contraseña.";
                return false;
            }

            UserDatabase database = Load();
            foreach (UserRecord user in database.users)
            {
                if (user.email != email)
                {
                    continue;
                }

                if (Hash(password, user.salt) != user.hash)
                {
                    error = InvalidCredentials;
                    return false;
                }

                Current = user;
                PlayerPrefs.SetString(SessionKey, email);
                PlayerPrefs.Save();
                error = null;
                return true;
            }

            // Mismo mensaje que con contraseña errada: no se revela qué correos existen.
            error = InvalidCredentials;
            return false;
        }
    }
}
