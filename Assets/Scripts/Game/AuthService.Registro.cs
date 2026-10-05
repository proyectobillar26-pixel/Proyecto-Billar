using System;
using UnityEngine;

namespace Billar.Game
{
    // RF-01 Registro (responsable: Miguel).
    public static partial class AuthService
    {
        public static bool Register(string nick, string email, string password, string confirm, out string error)
        {
            // RF-01: los tres campos deben venir diligenciados.
            if (string.IsNullOrEmpty(nick) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                error = "Completa todos los campos para registrarte.";
                return false;
            }

            nick = nick.Trim();
            email = email.Trim().ToLowerInvariant();

            UserDatabase database = Load();

            // RF-02: validaciones antes de crear la cuenta.
            if (!ValidateRegistration(nick, email, password, confirm, database, out error))
            {
                return false;
            }

            string salt = Guid.NewGuid().ToString("N");
            var record = new UserRecord
            {
                nick = nick,
                email = email,
                salt = salt,
                hash = Hash(password, salt),
            };

            database.users.Add(record);
            Save(database);

            Current = record;
            PlayerPrefs.SetString(SessionKey, email);
            PlayerPrefs.Save();

            error = null;
            return true;
        }
    }
}
