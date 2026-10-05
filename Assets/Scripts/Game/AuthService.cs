using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Billar.Game
{
    [Serializable]
    public sealed class UserRecord
    {
        public string nick;
        public string email;
        public string salt;
        public string hash;
    }

    [Serializable]
    public sealed class UserDatabase
    {
        public List<UserRecord> users = new List<UserRecord>();
    }

    /// <summary>
    /// Cuentas locales del celular (RF-01 a RF-05). La contraseña nunca se guarda
    /// tal cual: se guarda su hash con sal (RNF-04).
    ///
    /// La clase está dividida por requisito (partial), un archivo por responsable:
    ///   AuthService.cs              Datos y utilidades comunes
    ///   AuthService.Registro.cs     RF-01  Registro
    ///   AuthService.Validacion.cs   RF-02  Validación de registro
    ///   AuthService.Login.cs        RF-03  Inicio de sesión
    ///   AuthService.Sesion.cs       RF-04  Sesión persistente
    ///   AuthService.CerrarSesion.cs RF-05  Cerrar sesión
    ///
    /// Cuando exista el servidor, esta clase es la única que hay que cambiar
    /// para que el registro y el login vayan contra internet.
    /// </summary>
    public static partial class AuthService
    {
        private const string DatabaseKey = "billar.users";
        private const string SessionKey = "billar.session";

        public static UserRecord Current { get; private set; }

        public static bool IsLoggedIn => Current != null;

        private static string Hash(string password, string salt)
        {
            using (var sha = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(salt + "|" + password);
                byte[] digest = sha.ComputeHash(bytes);
                var builder = new StringBuilder(digest.Length * 2);
                foreach (byte b in digest)
                {
                    builder.Append(b.ToString("x2"));
                }

                return builder.ToString();
            }
        }

        private static UserDatabase Load()
        {
            string json = PlayerPrefs.GetString(DatabaseKey, string.Empty);
            if (string.IsNullOrEmpty(json))
            {
                return new UserDatabase();
            }

            try
            {
                return JsonUtility.FromJson<UserDatabase>(json) ?? new UserDatabase();
            }
            catch (Exception)
            {
                return new UserDatabase();
            }
        }

        private static void Save(UserDatabase database)
        {
            PlayerPrefs.SetString(DatabaseKey, JsonUtility.ToJson(database));
            PlayerPrefs.Save();
        }
    }
}
