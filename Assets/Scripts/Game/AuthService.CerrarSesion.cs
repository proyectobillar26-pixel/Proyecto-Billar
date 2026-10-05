using System;
using UnityEngine;

namespace Billar.Game
{
    // RF-05 Cerrar sesión (responsable: Jader).
    public static partial class AuthService
    {
        /// <summary>Cierra la sesión actual (RF-05). Devuelve false si no se pudo.</summary>
        public static bool Logout()
        {
            try
            {
                Current = null;
                PlayerPrefs.DeleteKey(SessionKey);
                PlayerPrefs.Save();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
