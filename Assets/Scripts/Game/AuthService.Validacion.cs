using System;

namespace Billar.Game
{
    // RF-02 Validación de registro (responsable: Jader).
    public static partial class AuthService
    {
        private const int MinPasswordLength = 6;
        private const int MinNickLength = 3;

        /// <summary>
        /// Comprueba correo, apodo y contraseña antes de crear la cuenta (RF-02).
        /// Recibe el apodo y el correo ya recortados y el correo en minúsculas.
        /// </summary>
        private static bool ValidateRegistration(
            string nick,
            string email,
            string password,
            string confirm,
            UserDatabase database,
            out string error)
        {
            if (!LooksLikeEmail(email))
            {
                error = "Ingresa un correo electrónico válido.";
                return false;
            }

            if (nick.Length == 0)
            {
                error = "El apodo no puede estar vacío.";
                return false;
            }

            if (nick.Length < MinNickLength)
            {
                error = $"El apodo necesita al menos {MinNickLength} letras.";
                return false;
            }

            if (!HasUppercase(password) || !HasSpecialCharacter(password))
            {
                error = "La contraseña debe tener al menos una letra mayúscula y un carácter especial.";
                return false;
            }

            if (password.Length < MinPasswordLength)
            {
                error = $"La contraseña necesita al menos {MinPasswordLength} caracteres.";
                return false;
            }

            if (password != confirm)
            {
                error = "Las dos contraseñas no coinciden.";
                return false;
            }

            foreach (UserRecord user in database.users)
            {
                if (user.email == email)
                {
                    error = "Este correo ya está registrado.";
                    return false;
                }

                if (string.Equals(user.nick, nick, StringComparison.OrdinalIgnoreCase))
                {
                    error = "Ese apodo ya está ocupado.";
                    return false;
                }
            }

            error = null;
            return true;
        }

        private static bool HasUppercase(string value)
        {
            foreach (char c in value)
            {
                if (char.IsUpper(c))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasSpecialCharacter(string value)
        {
            foreach (char c in value)
            {
                if (!char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool LooksLikeEmail(string email)
        {
            int at = email.IndexOf('@');
            int dot = email.LastIndexOf('.');
            return at > 0 && dot > at + 1 && dot < email.Length - 1 && !email.Contains(" ");
        }
    }
}
