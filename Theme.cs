using UnityEngine;

namespace Billar.UI
{
    /// <summary>Colores y medidas comunes, sacados de los mockups del proyecto.</summary>
    public static class Theme
    {
        public static readonly Color Background = Hex("#07080A");
        public static readonly Color Panel = Hex("#0C0D10");
        public static readonly Color PanelSoft = Hex("#14161C");
        public static readonly Color Felt = Hex("#1B9A48");
        public static readonly Color FeltDark = Hex("#0E6B32");
        public static readonly Color Rail = Hex("#7A4A28");
        public static readonly Color RailDark = Hex("#3E2414");
        public static readonly Color Gold = Hex("#D4AF37");
        public static readonly Color GoldDim = Hex("#8A6A18");
        public static readonly Color Cream = Hex("#F6EFE0");
        public static readonly Color Muted = Hex("#9AA3AD");
        public static readonly Color Danger = Hex("#C0473B");
        public static readonly Color Success = Hex("#3FA06A");

        public const int TitleSize = 46;
        public const int HeadingSize = 30;
        public const int BodySize = 22;
        public const int SmallSize = 18;

        private static Font _font;

        /// <summary>Fuente incorporada de Unity. No hace falta importar nada.</summary>
        public static Font Font
        {
            get
            {
                if (_font != null)
                {
                    return _font;
                }

                _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                if (_font == null)
                {
                    _font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                }

                if (_font == null)
                {
                    _font = Font.CreateDynamicFontFromOSFont("Arial", 16);
                }

                return _font;
            }
        }

        public static Color Hex(string value)
        {
            return ColorUtility.TryParseHtmlString(value, out Color color) ? color : Color.magenta;
        }

        public static Color Fade(Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }

        private static readonly Color[] SuitColors =
        {
            Hex("#F2C230"), // 1 y 9  amarillo
            Hex("#1F4FA8"), // 2 y 10 azul
            Hex("#C8322B"), // 3 y 11 rojo
            Hex("#6B3FA0"), // 4 y 12 morado
            Hex("#E07B26"), // 5 y 13 naranja
            Hex("#1E7A45"), // 6 y 14 verde
            Hex("#7B2B2B"), // 7 y 15 vino
        };

        /// <summary>Color de cada bola, siguiendo los colores clasicos del billar.</summary>
        public static Color BallColor(int number)
        {
            if (number == 0)
            {
                return Hex("#F7F4EC");
            }

            if (number == 8)
            {
                return Hex("#16191D");
            }

            int index = (number - 1) % 7;
            if (number > 8)
            {
                index = (number - 9) % 7;
            }

            return SuitColors[index];
        }

        /// <summary>true si la bola es rayada (9 a 15).</summary>
        public static bool IsStriped(int number) => number > 8;
    }
}
