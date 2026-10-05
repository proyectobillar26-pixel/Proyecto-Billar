using UnityEngine;

namespace Billar.Game
{
    /// <summary>
    /// Punto de entrada. La escena del proyecto esta vacia a proposito: el juego
    /// se arma solo desde aqui, asi no hay nada en la escena que se pueda
    /// desconfigurar sin querer.
    /// </summary>
    public static class AppBootstrap
    {
        private static AppController _instance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Launch()
        {
            if (_instance != null)
            {
                return;
            }

            var go = new GameObject("Billar App");
            _instance = go.AddComponent<AppController>();
            Object.DontDestroyOnLoad(go);
        }
    }
}
