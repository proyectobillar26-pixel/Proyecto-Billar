using Billar.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Billar.Game
{
    /// <summary>Pantalla de registro (RF-01, RF-02).</summary>
    public static class RegisterScreen
    {
        public static void Build(AppController app)
        {
            Transform root = app.NewScreen("Registro");

            Image card = UIKit.Panel(root, "Tarjeta", Theme.Hex("#0B0C0F"), 22);
            UIKit.Place(card.gameObject, 0f, 0f, 560f, 640f);
            UIKit.Outline(card.transform, Theme.Gold, 22);

            Brand.Build(card.transform, 0f, 240f, "Crea tu cuenta");

            InputField nick = UIKit.Field(card.transform, "Apodo", "Tu apodo");
            UIKit.Place(nick.gameObject, 0f, 100f, 500f, 60f);

            InputField email = UIKit.Field(card.transform, "Correo", "correo@email.com", InputField.ContentType.EmailAddress);
            UIKit.Place(email.gameObject, 0f, 30f, 500f, 60f);

            InputField password = UIKit.Field(card.transform, "Clave", "Contraseña (mayúscula y símbolo)", InputField.ContentType.Password);
            UIKit.Place(password.gameObject, 0f, -40f, 500f, 60f);

            InputField confirm = UIKit.Field(card.transform, "Confirmar", "Repite la contraseña", InputField.ContentType.Password);
            UIKit.Place(confirm.gameObject, 0f, -110f, 500f, 60f);

            Text error = UIKit.Label(card.transform, "Error", string.Empty, Theme.SmallSize, Theme.Danger);
            UIKit.Place(error.gameObject, 0f, -158f, 540f, 40f);

            Button create = UIKit.Button(card.transform, "Crear", "CREAR CUENTA", Theme.Gold, Theme.Hex("#1A1408"), Theme.BodySize, 20);
            UIKit.Place(create.gameObject, 0f, -212f, 460f, 62f);
            Image gold = create.GetComponent<Image>();
            gold.sprite = ClubArt.GoldBar();
            gold.type = Image.Type.Sliced;

            Button back = UIKit.Button(card.transform, "Volver", "¿Ya tienes cuenta?  Inicia sesión", Theme.Fade(Theme.PanelSoft, 0f), Theme.Gold, Theme.SmallSize, 0);
            UIKit.Place(back.gameObject, 0f, -272f, 500f, 40f);

            UIKit.OnClick(create, () =>
            {
                if (AuthService.Register(nick.text, email.text, password.text, confirm.text, out string message))
                {
                    app.Toast("Cuenta creada correctamente. ¡Bienvenido, " + AuthService.Current.nick + "!");
                    app.ShowHome();
                }
                else
                {
                    error.text = message;
                }
            });

            UIKit.OnClick(back, app.ShowLogin);
        }
    }
}
