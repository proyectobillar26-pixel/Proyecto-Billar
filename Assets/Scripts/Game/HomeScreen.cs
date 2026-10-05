using Billar.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Billar.Game
{
    /// <summary>
    /// Pantalla de inicio provisional de la semana 1: muestra el apodo de la
    /// cuenta activa (RF-09) y permite cerrar sesión (RF-05).
    /// Los modos de juego (RF-08) se agregan en la semana 2.
    /// </summary>
    public static class HomeScreen
    {
        public static void Build(AppController app)
        {
            Transform root = app.NewScreen("Inicio");

            Image card = UIKit.Panel(root, "Tarjeta", Theme.Hex("#0B0C0F"), 22);
            UIKit.Place(card.gameObject, 0f, 0f, 560f, 420f);
            UIKit.Outline(card.transform, Theme.Gold, 22);

            Brand.Build(card.transform, 0f, 120f, string.Empty);

            string nick = AuthService.Current != null ? AuthService.Current.nick : string.Empty;
            Text welcome = UIKit.Label(card.transform, "Bienvenida", "Bienvenido, " + nick, Theme.BodySize, Theme.Cream, TextAnchor.MiddleCenter, FontStyle.Bold);
            UIKit.Place(welcome.gameObject, 0f, -60f, 500f, 40f);

            Button logout = UIKit.Button(card.transform, "Salir", "Cerrar sesión", Theme.Fade(Theme.Danger, 0.9f), Theme.Cream, 16, 14);
            UIKit.Place(logout.gameObject, 0f, -140f, 360f, 50f);
            UIKit.OnClick(logout, app.LogoutAndGoToLogin);
        }
    }
}
