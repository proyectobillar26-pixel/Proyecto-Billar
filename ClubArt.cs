using Billar.Core;
using UnityEngine;

namespace Billar.UI
{
    /// <summary>
    /// Arte del club: pano, madera, tacos distintos y fondo de las pantallas.
    /// Se genera por codigo para parecerse a los mockups.
    /// </summary>
    public static class ClubArt
    {
        public static Sprite Felt()
        {
            const int w = 1024;
            const int h = 512;
            const string key = "felt_v4";
            if (TryGet(key, out Sprite cached))
            {
                return cached;
            }

            var pixels = new Color32[w * h];
            Color a = Theme.Hex("#27B85A");
            Color b = Theme.Hex("#0F7A36");
            Color edge = Theme.Hex("#0A4E26");

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    float u = x / (float)(w - 1);
                    float v = y / (float)(h - 1);
                    float weave = (Mathf.Sin(x * 0.085f) + Mathf.Sin(y * 0.09f)) * 0.018f;
                    float nap = Mathf.Sin((x * 0.021f) + (y * 0.017f)) * Mathf.Sin((x * 0.013f) - (y * 0.019f)) * 0.03f;
                    float vignette = Mathf.Clamp01(1.18f - (Vector2.Distance(new Vector2(u, v), new Vector2(0.5f, 0.5f)) * 0.72f));
                    Color mid = Color.Lerp(b, a, 0.42f + (v * 0.28f) + nap);
                    mid = Color.Lerp(edge, mid, vignette);
                    mid.r = Mathf.Clamp01(mid.r + weave);
                    mid.g = Mathf.Clamp01(mid.g + weave * 0.6f);
                    pixels[(y * w) + x] = mid;
                }
            }

            return Bake(pixels, w, h, key, 0, 480f);
        }

        public static Sprite Wood()
        {
            const int w = 1024;
            const int h = 256;
            const string key = "wood_v4";
            if (TryGet(key, out Sprite cached))
            {
                return cached;
            }

            var pixels = new Color32[w * h];
            Color dark = Theme.Hex("#3E2212");
            Color mid = Theme.Hex("#7A4A28");
            Color light = Theme.Hex("#A86A38");

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    float band = Mathf.Sin((x * 0.045f) + (Mathf.Sin(x * 0.011f) * 1.8f)) * 0.5f + 0.5f;
                    float grain = Mathf.Sin((x * 0.37f) + (y * 0.08f)) * 0.06f;
                    Color c = Color.Lerp(dark, light, band);
                    c = Color.Lerp(c, mid, 0.38f + grain);
                    if (y < 3 || y > h - 4)
                    {
                        c = Color.Lerp(c, Theme.Hex("#2A180C"), 0.45f);
                    }

                    pixels[(y * w) + x] = c;
                }
            }

            return Bake(pixels, w, h, key, 0, 420f);
        }

        public static Sprite ClubBackdrop()
        {
            const int w = 1600;
            const int h = 900;
            const string key = "club_bg_v4";
            if (TryGet(key, out Sprite cached))
            {
                return cached;
            }

            var pixels = new Color32[w * h];
            Color room = Theme.Hex("#07080A");
            Color felt = Theme.Hex("#167A3A");
            Color feltDark = Theme.Hex("#0B4A24");
            Color lamp = Theme.Hex("#F3D27A");

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    float u = x / (float)(w - 1);
                    float v = y / (float)(h - 1);
                    Color c = room;

                    // Pano abajo, como en el mockup.
                    if (v < 0.42f)
                    {
                        float t = v / 0.42f;
                        c = Color.Lerp(feltDark, felt, t + (Mathf.Sin(x * 0.02f) * 0.04f));
                        if (v > 0.36f)
                        {
                            c = Color.Lerp(Theme.Hex("#4A3018"), c, (0.42f - v) / 0.06f);
                        }
                    }

                    // Lamparas.
                    float d1 = Vector2.Distance(new Vector2(u, v), new Vector2(0.22f, 0.86f));
                    float d2 = Vector2.Distance(new Vector2(u, v), new Vector2(0.78f, 0.88f));
                    float glow = Mathf.Clamp01(1f - (Mathf.Min(d1, d2) * 4.2f));
                    c = Color.Lerp(c, lamp, glow * 0.22f);

                    // Viñeta.
                    float vig = Mathf.Clamp01(1.1f - (Vector2.Distance(new Vector2(u, v), new Vector2(0.5f, 0.45f)) * 1.15f));
                    c = Color.Lerp(Color.black, c, 0.35f + (vig * 0.65f));
                    pixels[(y * w) + x] = c;
                }
            }

            return Bake(pixels, w, h, key);
        }

        /// <summary>Taco con punta, virola, grain, wrap, aros e inlays distintos segun el id.</summary>
        public static Sprite Cue(string id)
        {
            string key = "cue_v5_" + id;
            if (TryGet(key, out Sprite cached))
            {
                return cached;
            }

            CueLook look = LookFor(id);
            const int w = 1024;
            const int h = 128;
            var pixels = new Color32[w * h];
            float midY = (h - 1) * 0.5f;

            for (int x = 0; x < w; x++)
            {
                float t = x / (float)(w - 1);
                Color body = look.Shaft;
                float radius = look.Thick * 0.26f;
                bool metallic = false;

                if (t < 0.028f)
                {
                    body = look.Tip;
                    radius = look.Thick * 0.20f;
                }
                else if (t < 0.062f)
                {
                    body = look.Ferrule;
                    radius = look.Thick * 0.22f;
                    metallic = true;
                }
                else if (t < 0.54f)
                {
                    float grain = 0.55f + (Mathf.Sin((x * 0.35f) + (Hash(x, 4) * 6f)) * 0.18f) + (Hash(x, 9) * 0.08f);
                    body = Color.Lerp(look.ShaftDark, look.Shaft, Mathf.Clamp01(grain));
                    radius = Mathf.Lerp(look.Thick * 0.22f, look.Thick * 0.30f, (t - 0.062f) / 0.48f);

                    if (look.Points && t > 0.16f && t < 0.48f)
                    {
                        float cycle = ((t - 0.16f) % 0.085f) / 0.085f;
                        float half = 1f - (cycle * 1.15f);
                        if (half > 0f)
                        {
                            body = Color.Lerp(body, look.PointColor, 0.72f);
                        }
                    }
                }
                else if (t < 0.69f)
                {
                    radius = look.Thick * 0.33f;
                    int style = look.WrapStyle;
                    if (style == 1)
                    {
                        body = Color.Lerp(look.Wrap, look.ButtDark, Hash(x, x / 3) * 0.45f);
                    }
                    else if (style == 2)
                    {
                        bool check = (((x / 6) + ((int)midY / 6)) % 2) == 0;
                        body = check ? look.Wrap : Color.Lerp(look.Wrap, Color.black, 0.4f);
                    }
                    else
                    {
                        float spiral = Mathf.Sin((x * 0.9f) + 0.4f);
                        body = spiral > 0f
                            ? look.Wrap
                            : Color.Lerp(look.Wrap, Color.white, 0.16f);
                    }
                }
                else if (t < 0.96f)
                {
                    float grain = Hash(x, 2) * 0.22f;
                    body = Color.Lerp(look.Butt, look.ButtDark, grain);
                    radius = Mathf.Lerp(look.Thick * 0.34f, look.Thick * 0.42f, (t - 0.69f) / 0.27f);

                    if (look.Inlay && (int)(t * 80f) % 7 == 0)
                    {
                        body = look.RingColor;
                    }
                }
                else
                {
                    body = look.Bumper;
                    radius = look.Thick * 0.40f;
                }

                if (Near(t, 0.062f) || Near(t, 0.54f) || Near(t, 0.69f) || Near(t, look.Ring) || Near(t, look.Ring + 0.035f))
                {
                    body = look.RingColor;
                    radius += 0.03f;
                    metallic = true;
                }

                for (int y = 0; y < h; y++)
                {
                    float ny = Mathf.Abs(y - midY) / midY;
                    if (ny > radius)
                    {
                        pixels[(y * w) + x] = new Color32(0, 0, 0, 0);
                        continue;
                    }

                    float shade = Mathf.Lerp(1.22f, 0.42f, ny);
                    float spec = Mathf.Clamp01(1f - Mathf.Abs(ny - 0.22f) * 7f);
                    Color c = new Color(body.r * shade, body.g * shade, body.b * shade, 1f);
                    c = Color.Lerp(c, Color.white, spec * (metallic ? 0.55f : 0.26f));
                    pixels[(y * w) + x] = c;
                }
            }

            return Bake(pixels, w, h, key);
        }

        public static Sprite CardFace()
        {
            const int w = 160;
            const int h = 240;
            const string key = "card_face_v1";
            if (TryGet(key, out Sprite cached))
            {
                return cached;
            }

            var pixels = new Color32[w * h];
            Color fill = Theme.Hex("#0C1A14");
            Color gold = Theme.Gold;
            Color inner = Theme.Hex("#143024");

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    bool border = x < 6 || x > w - 7 || y < 6 || y > h - 7;
                    bool innerLine = x == 10 || x == w - 11 || y == 10 || y == h - 11;
                    Color c = fill;
                    if (x > 14 && x < w - 15 && y > 14 && y < h - 15)
                    {
                        c = Color.Lerp(fill, inner, 0.45f + (Hash(x, y) * 0.08f));
                    }

                    if (innerLine)
                    {
                        c = Theme.Fade(gold, 0.45f);
                    }

                    if (border)
                    {
                        c = gold;
                    }

                    pixels[(y * w) + x] = c;
                }
            }

            return Bake(pixels, w, h, key, 8);
        }

        public static Sprite GoldBar()
        {
            const int w = 128;
            const int h = 32;
            const string key = "gold_bar_v2";
            if (TryGet(key, out Sprite cached))
            {
                return cached;
            }

            var pixels = new Color32[w * h];
            Color top = Theme.Hex("#F0D56A");
            Color mid = Theme.Gold;
            Color bot = Theme.Hex("#8A6A18");

            for (int y = 0; y < h; y++)
            {
                float t = y / (float)(h - 1);
                Color c = t < 0.45f ? Color.Lerp(top, mid, t / 0.45f) : Color.Lerp(mid, bot, (t - 0.45f) / 0.55f);
                for (int x = 0; x < w; x++)
                {
                    pixels[(y * w) + x] = c;
                }
            }

            return Bake(pixels, w, h, key);
        }

        public static Sprite CardPortrait(string id)
        {
            string key = "card_port_" + id;
            if (TryGet(key, out Sprite cached))
            {
                return cached;
            }

            const int w = 168;
            const int h = 248;
            var pixels = new Color32[w * h];
            Color fill = Theme.Hex("#0C1A14");
            Color inner = Theme.Hex("#163C28");
            Color gold = Theme.Gold;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    Color c = fill;
                    if (x > 12 && x < w - 13 && y > 12 && y < h - 13)
                    {
                        c = Color.Lerp(fill, inner, 0.4f + (Hash(x, y) * 0.1f));
                    }

                    bool border = x < 7 || x > w - 8 || y < 7 || y > h - 8;
                    bool innerLine = x == 12 || x == w - 13 || y == 12 || y == h - 13;
                    if (innerLine)
                    {
                        c = Theme.Fade(gold, 0.55f);
                    }

                    if (border)
                    {
                        c = gold;
                    }

                    pixels[(y * w) + x] = c;
                }
            }

            DrawCardEmblem(pixels, w, h, id);
            return Bake(pixels, w, h, key, 8);
        }

        public static Sprite ModeIcon(string id)
        {
            string key = "mode_" + id;
            if (TryGet(key, out Sprite cached))
            {
                return cached;
            }

            const int s = 160;
            var pixels = new Color32[s * s];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = new Color32(0, 0, 0, 0);
            }

            if (id == "rapida")
            {
                DrawCueStickIcon(pixels, s, 28, 28, 132, 132, "basico");
                DrawCueStickIcon(pixels, s, 132, 28, 28, 132, "fuerza");
            }
            else if (id == "ranked")
            {
                DrawTrophy(pixels, s);
            }
            else if (id == "privada")
            {
                DrawLock(pixels, s);
            }
            else
            {
                DrawDisc(pixels, s, 80, 80, 48, Theme.Hex("#F4EEE0"));
                DrawDisc(pixels, s, 64, 96, 7, Theme.Hex("#C0473B"));
            }

            return Bake(pixels, s, s, key);
        }

        public static Sprite HudIcon(string id)
        {
            string key = "hud_" + id;
            if (TryGet(key, out Sprite cached))
            {
                return cached;
            }

            const int s = 96;
            var pixels = new Color32[s * s];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = new Color32(0, 0, 0, 0);
            }

            Color gold = Theme.Gold;
            Color cream = Theme.Cream;

            if (id == "coin")
            {
                DrawDisc(pixels, s, 40, 52, 22, Theme.Hex("#8A6A18"));
                DrawDisc(pixels, s, 56, 44, 24, gold);
                DrawDisc(pixels, s, 56, 44, 12, Theme.Hex("#F0D56A"));
            }
            else if (id == "gear")
            {
                DrawDisc(pixels, s, 48, 48, 28, gold);
                DrawDisc(pixels, s, 48, 48, 12, Theme.Hex("#0B0C0F"));
                FillRect(pixels, s, 44, 12, 52, 84, gold);
                FillRect(pixels, s, 12, 44, 84, 52, gold);
            }
            else if (id == "plus")
            {
                DrawDisc(pixels, s, 48, 48, 28, gold);
                FillRect(pixels, s, 44, 22, 52, 74, Theme.Hex("#1A1408"));
                FillRect(pixels, s, 22, 44, 74, 52, Theme.Hex("#1A1408"));
            }
            else if (id == "home")
            {
                FillRect(pixels, s, 28, 40, 68, 78, gold);
                for (int i = 0; i < 36; i++)
                {
                    float t = i / 35f;
                    float x = 18 + (t * 60f);
                    float y = 48 - (Mathf.Abs(0.5f - t) * 36f);
                    DrawDisc(pixels, s, x, y, 4f, gold);
                }
            }
            else if (id == "shop")
            {
                FillRect(pixels, s, 24, 38, 72, 70, gold);
                FillRect(pixels, s, 20, 30, 76, 40, gold);
                DrawDisc(pixels, s, 36, 76, 8, gold);
                DrawDisc(pixels, s, 60, 76, 8, gold);
            }
            else if (id == "cards")
            {
                FillRect(pixels, s, 28, 22, 58, 74, gold);
                FillRect(pixels, s, 40, 16, 72, 70, Theme.Hex("#F0D56A"));
                FillRect(pixels, s, 44, 22, 68, 64, Theme.Hex("#0B0C0F"));
            }
            else if (id == "user")
            {
                DrawDisc(pixels, s, 48, 34, 16, gold);
                DrawDisc(pixels, s, 48, 72, 24, gold);
            }
            else
            {
                DrawDisc(pixels, s, 48, 48, 20, cream);
            }

            return Bake(pixels, s, s, key);
        }

        public static Sprite PowerGradient()
        {
            const string key = "power_grad_v1";
            if (TryGet(key, out Sprite cached))
            {
                return cached;
            }

            const int w = 256;
            const int h = 16;
            var pixels = new Color32[w * h];
            Color a = Theme.Hex("#3FA06A");
            Color b = Theme.Hex("#D4AF37");
            Color c = Theme.Hex("#C0473B");

            for (int x = 0; x < w; x++)
            {
                float t = x / (float)(w - 1);
                Color col = t < 0.55f
                    ? Color.Lerp(a, b, t / 0.55f)
                    : Color.Lerp(b, c, (t - 0.55f) / 0.45f);
                for (int y = 0; y < h; y++)
                {
                    float shade = y < 4 ? 1.12f : (y > h - 4 ? 0.78f : 1f);
                    pixels[(y * w) + x] = new Color(col.r * shade, col.g * shade, col.b * shade, 1f);
                }
            }

            return Bake(pixels, w, h, key, 4);
        }

        public static Sprite Avatar(bool human)
        {
            string key = human ? "avatar_you" : "avatar_rival";
            if (TryGet(key, out Sprite cached))
            {
                return cached;
            }

            const int s = 96;
            var pixels = new Color32[s * s];
            Color skin = human ? Theme.Hex("#D4A574") : Theme.Hex("#8D6A4A");
            Color hair = human ? Theme.Hex("#2A1C12") : Theme.Hex("#111111");
            Color shirt = human ? Theme.Hex("#1A3A2A") : Theme.Hex("#3A1A1A");
            float cx = 47.5f;
            float cy = 47.5f;

            for (int y = 0; y < s; y++)
            {
                for (int x = 0; x < s; x++)
                {
                    float dx = x - cx;
                    float dy = y - cy;
                    float dist = Mathf.Sqrt((dx * dx) + (dy * dy));
                    if (dist > 46f)
                    {
                        pixels[(y * s) + x] = new Color32(0, 0, 0, 0);
                        continue;
                    }

                    Color c = shirt;
                    if (y > 58)
                    {
                        c = shirt;
                    }
                    else if (y > 28)
                    {
                        c = skin;
                    }
                    else
                    {
                        c = hair;
                    }

                    if (dist > 40f)
                    {
                        c = Theme.Gold;
                    }

                    float shade = Mathf.Clamp01(1.1f - (dist / 80f));
                    pixels[(y * s) + x] = new Color(c.r * shade, c.g * shade, c.b * shade, 1f);
                }
            }

            return Bake(pixels, s, s, key);
        }

        public static void Prewarm()
        {
            Felt();
            Wood();
            ClubBackdrop();
            CardFace();
            GoldBar();
            PowerGradient();
            Avatar(true);
            Avatar(false);
            ModeIcon("rapida");
            ModeIcon("ranked");
            ModeIcon("privada");
            ModeIcon("practica");
            HudIcon("coin");
            HudIcon("gear");
            HudIcon("plus");
            HudIcon("home");
            HudIcon("shop");
            HudIcon("cards");
            HudIcon("user");
            Cue("basico");
            Cue("control");
            Cue("fuerza");
            Cue("efecto");
            Cue("precision");
        }

        private struct CueLook
        {
            public Color Tip;
            public Color Ferrule;
            public Color Shaft;
            public Color ShaftDark;
            public Color Wrap;
            public Color Butt;
            public Color ButtDark;
            public Color Bumper;
            public Color RingColor;
            public float Ring;
            public float Thick;
            public int WrapStyle;
            public bool Points;
            public Color PointColor;
            public bool Inlay;
        }

        private static CueLook LookFor(string id)
        {
            switch (id)
            {
                case "control":
                    return new CueLook
                    {
                        Tip = Theme.Hex("#2F6BFF"),
                        Ferrule = Theme.Hex("#F4EFE4"),
                        Shaft = Theme.Hex("#E2C48A"),
                        ShaftDark = Theme.Hex("#C9A66A"),
                        Wrap = Theme.Hex("#1F7A68"),
                        Butt = Theme.Hex("#245C4E"),
                        ButtDark = Theme.Hex("#14362E"),
                        Bumper = Theme.Hex("#0E0E10"),
                        RingColor = Theme.Hex("#D8DDE2"),
                        Ring = 0.82f,
                        Thick = 1.0f,
                        WrapStyle = 0,
                        Points = false,
                        PointColor = Color.white,
                        Inlay = false,
                    };
                case "fuerza":
                    return new CueLook
                    {
                        Tip = Theme.Hex("#1E1E22"),
                        Ferrule = Theme.Hex("#E7D7B0"),
                        Shaft = Theme.Hex("#C48A4A"),
                        ShaftDark = Theme.Hex("#9A6230"),
                        Wrap = Theme.Hex("#4A1C16"),
                        Butt = Theme.Hex("#8B2A1F"),
                        ButtDark = Theme.Hex("#4A120E"),
                        Bumper = Theme.Hex("#1A0A08"),
                        RingColor = Theme.Hex("#C47A2A"),
                        Ring = 0.84f,
                        Thick = 1.22f,
                        WrapStyle = 1,
                        Points = false,
                        PointColor = Color.white,
                        Inlay = true,
                    };
                case "efecto":
                    return new CueLook
                    {
                        Tip = Theme.Hex("#6C3BFF"),
                        Ferrule = Theme.Hex("#F7F2E8"),
                        Shaft = Theme.Hex("#D7B37A"),
                        ShaftDark = Theme.Hex("#B08950"),
                        Wrap = Theme.Hex("#4B3A86"),
                        Butt = Theme.Hex("#3A2A6E"),
                        ButtDark = Theme.Hex("#1C1438"),
                        Bumper = Theme.Hex("#120C22"),
                        RingColor = Theme.Hex("#C9B6FF"),
                        Ring = 0.80f,
                        Thick = 1.02f,
                        WrapStyle = 2,
                        Points = false,
                        PointColor = Color.white,
                        Inlay = true,
                    };
                case "precision":
                    return new CueLook
                    {
                        Tip = Theme.Hex("#F2E27A"),
                        Ferrule = Theme.Hex("#FFF8E8"),
                        Shaft = Theme.Hex("#F0D9A0"),
                        ShaftDark = Theme.Hex("#D4B56E"),
                        Wrap = Theme.Hex("#1A1A1C"),
                        Butt = Theme.Hex("#16120A"),
                        ButtDark = Theme.Hex("#0A0804"),
                        Bumper = Theme.Hex("#D4AF37"),
                        RingColor = Theme.Hex("#F0D56A"),
                        Ring = 0.86f,
                        Thick = 0.94f,
                        WrapStyle = 0,
                        Points = true,
                        PointColor = Theme.Hex("#1A1408"),
                        Inlay = true,
                    };
                default:
                    return new CueLook
                    {
                        Tip = Theme.Hex("#3A3A40"),
                        Ferrule = Theme.Hex("#F3EBDA"),
                        Shaft = Theme.Hex("#D2B07A"),
                        ShaftDark = Theme.Hex("#B08952"),
                        Wrap = Theme.Hex("#1C1C1E"),
                        Butt = Theme.Hex("#6A4328"),
                        ButtDark = Theme.Hex("#3E2616"),
                        Bumper = Theme.Hex("#111111"),
                        RingColor = Theme.Hex("#E6D3A2"),
                        Ring = 0.83f,
                        Thick = 1.0f,
                        WrapStyle = 0,
                        Points = false,
                        PointColor = Color.white,
                        Inlay = false,
                    };
            }
        }

        private static bool Near(float t, float mark) => Mathf.Abs(t - mark) < 0.008f;

        private static void DrawDisc(Color32[] px, int s, float cx, float cy, float r, Color color)
        {
            int x0 = Mathf.Max(0, (int)(cx - r - 1));
            int x1 = Mathf.Min(s - 1, (int)(cx + r + 1));
            int y0 = Mathf.Max(0, (int)(cy - r - 1));
            int y1 = Mathf.Min(s - 1, (int)(cy + r + 1));
            for (int y = y0; y <= y1; y++)
            {
                for (int x = x0; x <= x1; x++)
                {
                    float dx = x - cx;
                    float dy = y - cy;
                    float dist = Mathf.Sqrt((dx * dx) + (dy * dy));
                    if (dist > r)
                    {
                        continue;
                    }

                    float shade = Mathf.Lerp(1.15f, 0.55f, dist / r);
                    float a = Mathf.Clamp01(r - dist + 0.5f);
                    Color c = new Color(color.r * shade, color.g * shade, color.b * shade, a);
                    Blend(px, s, s, x, y, c);
                }
            }
        }

        private static void FillRect(Color32[] px, int s, int x0, int y0, int x1, int y1, Color color)
        {
            x0 = Mathf.Clamp(x0, 0, s - 1);
            y0 = Mathf.Clamp(y0, 0, s - 1);
            x1 = Mathf.Clamp(x1, 0, s - 1);
            y1 = Mathf.Clamp(y1, 0, s - 1);
            for (int y = y0; y <= y1; y++)
            {
                for (int x = x0; x <= x1; x++)
                {
                    Blend(px, s, s, x, y, color);
                }
            }
        }

        private static void Blend(Color32[] px, int w, int h, int x, int y, Color color)
        {
            if ((uint)x >= (uint)w || (uint)y >= (uint)h)
            {
                return;
            }

            int i = (y * w) + x;
            Color dest = px[i];
            float a = color.a;
            Color mixed = new Color(
                dest.r + ((color.r - dest.r) * a),
                dest.g + ((color.g - dest.g) * a),
                dest.b + ((color.b - dest.b) * a),
                Mathf.Max(dest.a, a));
            px[i] = mixed;
        }

        private static void DrawCueStickIcon(Color32[] px, int s, int x0, int y0, int x1, int y1, string cueId)
        {
            CueLook look = LookFor(cueId);
            int steps = 90;
            for (int i = 0; i < steps; i++)
            {
                float t = i / (float)(steps - 1);
                float x = Mathf.Lerp(x0, x1, t);
                float y = Mathf.Lerp(y0, y1, t);
                Color c = look.Shaft;
                float r = 2.2f;
                if (t < 0.08f)
                {
                    c = look.Tip;
                    r = 1.7f;
                }
                else if (t < 0.16f)
                {
                    c = look.Ferrule;
                    r = 1.9f;
                }
                else if (t > 0.55f && t < 0.72f)
                {
                    c = look.Wrap;
                    r = 2.6f;
                }
                else if (t >= 0.72f)
                {
                    c = look.Butt;
                    r = 3.0f;
                }

                DrawDisc(px, s, x, y, r, c);
            }
        }

        private static void DrawTrophy(Color32[] px, int s)
        {
            FillRect(px, s, 48, 38, 112, 88, Theme.Gold);
            FillRect(px, s, 40, 48, 48, 78, Theme.GoldDim);
            FillRect(px, s, 112, 48, 120, 78, Theme.GoldDim);
            FillRect(px, s, 72, 88, 88, 112, Theme.Hex("#8A6A18"));
            FillRect(px, s, 52, 112, 108, 128, Theme.Gold);
            DrawDisc(px, s, 80, 64, 18, Theme.Hex("#16191D"));
            DrawDisc(px, s, 80, 64, 8, Theme.Hex("#F4EEE0"));
        }

        private static void DrawLock(Color32[] px, int s)
        {
            for (int a = 0; a < 180; a++)
            {
                float rad = a * Mathf.Deg2Rad;
                float x = 80 + (Mathf.Cos(rad) * 22);
                float y = 58 + (Mathf.Sin(rad) * 22);
                DrawDisc(px, s, x, y, 4.2f, Theme.Gold);
            }

            FillRect(px, s, 50, 70, 110, 128, Theme.Gold);
            FillRect(px, s, 58, 78, 102, 120, Theme.Hex("#1A1408"));
            DrawDisc(px, s, 80, 96, 8, Theme.Gold);
            FillRect(px, s, 77, 96, 83, 112, Theme.Gold);
        }

        private static void DrawCardEmblem(Color32[] px, int w, int h, string id)
        {
            int cx = w / 2;
            int cy = (h / 2) + 8;

            switch (id)
            {
                case "impulso":
                    DrawDisc(px, w, cx, cy, 28, Theme.Hex("#F4EEE0"));
                    FillRect(px, w, cx - 4, cy + 8, cx + 4, cy + 36, Theme.Gold);
                    FillRect(px, w, cx - 14, cy + 20, cx + 14, cy + 28, Theme.Gold);
                    break;
                case "curva":
                    DrawDisc(px, w, cx, cy, 26, Theme.Hex("#F4EEE0"));
                    for (int i = 0; i < 40; i++)
                    {
                        float t = i / 39f;
                        float x = cx + 8 + (Mathf.Cos(t * 3f) * 22);
                        float y = cy - 10 + (t * 40f);
                        DrawDisc(px, w, x, y, 3.2f, Theme.Gold);
                    }
                    break;
                case "laser":
                    FillRect(px, w, 28, cy - 3, w - 28, cy + 3, Theme.Gold);
                    DrawDisc(px, w, 36, cy, 8, Theme.Hex("#F4EEE0"));
                    DrawDisc(px, w, w - 36, cy, 6, Theme.Hex("#C0473B"));
                    break;
                case "segunda_chance":
                    DrawCueStickIcon(px, w, 40, 90, 128, 170, "basico");
                    DrawCueStickIcon(px, w, 128, 90, 40, 170, "control");
                    break;
                case "perdon":
                    FillRect(px, w, cx - 22, cy - 8, cx + 22, cy + 28, Theme.Hex("#1F7A4A"));
                    DrawDisc(px, w, cx, cy - 8, 22, Theme.Hex("#1F7A4A"));
                    FillRect(px, w, cx - 18, cy - 4, cx + 18, cy + 22, Theme.Gold);
                    break;
                case "freno":
                    FillRect(px, w, cx - 28, cy - 10, cx + 28, cy + 10, Theme.Hex("#C0473B"));
                    FillRect(px, w, cx - 22, cy - 4, cx + 22, cy + 4, Theme.Hex("#1A1408"));
                    break;
                case "colchon":
                    FillRect(px, w, 36, cy - 16, w - 36, cy + 16, Theme.Hex("#7A4A28"));
                    FillRect(px, w, 42, cy - 8, w - 42, cy + 8, Theme.Hex("#1B9A48"));
                    break;
                case "tronera_ancha":
                    DrawDisc(px, w, cx, cy, 34, Theme.Gold);
                    DrawDisc(px, w, cx, cy, 22, Theme.Hex("#1A120C"));
                    DrawDisc(px, w, cx, cy, 14, Theme.Hex("#050308"));
                    break;
                default:
                    DrawDisc(px, w, cx, cy, 26, Theme.Hex("#16191D"));
                    break;
            }
        }

        private static float Hash(int x, int y)
        {
            int n = (x * 374761393) + (y * 668265263);
            n = (n ^ (n >> 13)) * 1274126177;
            return ((n & 0x7fffffff) / 2147483647f);
        }

        private static bool TryGet(string key, out Sprite sprite) => ProceduralSprites.TryGetCached(key, out sprite);

        private static Sprite Bake(Color32[] pixels, int w, int h, string key, int border = 0, float ppu = 0f)
        {
            TextureWrapMode wrap = (key.StartsWith("felt") || key.StartsWith("wood"))
                ? TextureWrapMode.Repeat
                : TextureWrapMode.Clamp;
            return ProceduralSprites.BakeRaw(pixels, w, h, key, border, wrap, ppu);
        }
    }
}
