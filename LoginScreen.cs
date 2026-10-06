using Billar.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Billar.Game
{
    /// <summary>Pantalla de inicio de sesión, al estilo del mockup.</summary>
    public static class LoginScreen
    {
        public static void Build(AppController app)
        {
            Transform root = app.NewScreen("Login");

            Image card = UIKit.Panel(root, "Tarjeta", Theme.Hex("#0B0C0F"), 22);
            UIKit.Place(card.gameObject, 0f, -8f, 560f, 620f);
            UIKit.Outline(card.transform, Theme.Gold, 22);

            Brand.Build(card.transform, 0f, 214f, "Club de billar");

            InputField email = UIKit.Field(card.transform, "Correo", "correo@email.com", InputField.ContentType.EmailAddress);
            UIKit.Place(email.gameObject, 0f, 86f, 460f, 54f);

            InputField password = UIKit.Field(card.transform, "Clave", "Contraseña", InputField.ContentType.Password);
            UIKit.Place(password.gameObject, 0f, 20f, 460f, 54f);

            Text error = UIKit.Label(card.transform, "Error", string.Empty, Theme.SmallSize, Theme.Danger);
            UIKit.Place(error.gameObject, 0f, -28f, 480f, 28f);

            bool remember = false;
            Button rememberBtn = UIKit.Button(card.transform, "Recordar", "☐  Recordarme", Theme.Fade(Color.white, 0f), Theme.Muted, 14, 0);
            UIKit.Place(rememberBtn.gameObject, -140f, -58f, 200f, 28f);
            UIKit.OnClick(rememberBtn, () =>
            {
                remember = !remember;
                UIKit.SetCaption(rememberBtn, remember ? "☑  Recordarme" : "☐  Recordarme");
            });

            Button forgot = UIKit.Button(card.transform, "Olvido", "¿Olvidaste tu contraseña?", Theme.Fade(Color.white, 0f), Theme.Gold, 14, 0);
            UIKit.Place(forgot.gameObject, 140f, -58f, 240f, 28f);
            UIKit.OnClick(forgot, () => app.Toast("Recuperación de clave: próximamente."));

            Button enter = UIKit.Button(card.transform, "Entrar", "INICIAR SESIÓN", Theme.Gold, Theme.Hex("#1A1408"), Theme.BodySize, 24);
            UIKit.Place(enter.gameObject, 0f, -110f, 460f, 58f);
            Image gold = enter.GetComponent<Image>();
            gold.sprite = ClubArt.GoldBar();
            gold.type = Image.Type.Sliced;

            Button register = UIKit.Button(card.transform, "Registro", "¿No tienes cuenta?  Regístrate", Theme.Fade(Color.white, 0f), Theme.Gold, Theme.SmallSize, 0);
            UIKit.Place(register.gameObject, 0f, -250f, 460f, 36f);

            Text orLine = UIKit.Label(card.transform, "O", "o continúa con", 14, Theme.Muted);
            UIKit.Place(orLine.gameObject, 0f, -168f, 300f, 22f);

            Button google = UIKit.Button(card.transform, "Google", "Google", Theme.PanelSoft, Theme.Cream, 16, 12);
            UIKit.Place(google.gameObject, -110f, -206f, 200f, 44f);
            UIKit.OnClick(google, () => app.Toast("Inicio con Google: próximamente."));

            Button discord = UIKit.Button(card.transform, "Discord", "Discord", Theme.PanelSoft, Theme.Cream, 16, 12);
            UIKit.Place(discord.gameObject, 110f, -206f, 200f, 44f);
            UIKit.OnClick(discord, () => app.Toast("Inicio con Discord: próximamente."));

            // RF-04: si la sesión guardada ya no es válida, se avisa aquí.
            string notice = AuthService.TakePendingNotice();
            if (!string.IsNullOrEmpty(notice))
            {
                error.text = notice;
            }

            UIKit.OnClick(enter, () =>
            {
                if (AuthService.Login(email.text, password.text, out string message))
                {
                    app.ShowHome();
                    app.Toast("Sesión iniciada correctamente.");
                }
                else
                {
                    error.text = message;
                }
            });

            UIKit.OnClick(register, app.ShowRegister);
        }
    }

    public static class Brand
    {
        public static void Build(Transform parent, float x, float y, string subtitle)
        {
            Image ball = UIKit.Panel(parent, "Bola8", Color.white, 0);
            ball.sprite = ProceduralSprites.Ball(8);
            ball.type = Image.Type.Simple;
            UIKit.Place(ball.gameObject, x, y, 86f, 86f);

            Text title = UIKit.Label(parent, "Titulo", "BILLAR", Theme.TitleSize, Theme.Gold, TextAnchor.MiddleCenter, FontStyle.Bold);
            UIKit.Place(title.gameObject, x, y - 72f, 420f, 56f);

            Image diamond = UIKit.Panel(parent, "Rombo", Theme.Gold, 0);
            diamond.sprite = ProceduralSprites.Circle(Color.white, "diamond");
            diamond.type = Image.Type.Simple;
            UIKit.Place(diamond.gameObject, x, y - 104f, 10f, 10f);

            if (!string.IsNullOrEmpty(subtitle))
            {
                Text sub = UIKit.Label(parent, "Subtitulo", subtitle, Theme.SmallSize, Theme.Muted);
                UIKit.Place(sub.gameObject, x, y - 128f, 420f, 28f);
            }
        }
    }
}
