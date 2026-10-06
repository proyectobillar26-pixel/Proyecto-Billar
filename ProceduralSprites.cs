using System.Collections.Generic;
using Billar.Core;
using UnityEngine;

namespace Billar.UI
{
    /// <summary>
    /// Dibuja por codigo todo lo que el juego necesita: bolas, troneras, paneles.
    /// De esta forma el proyecto no depende de ningun archivo de arte y se puede
    /// abrir en cualquier maquina sin importar nada.
    ///
    /// Los sprites miden 1x1 unidad de mundo: el tamano real se ajusta con la
    /// escala del objeto.
    /// </summary>
    public static class ProceduralSprites
    {
        private static readonly Dictionary<string, Sprite> Cache = new Dictionary<string, Sprite>();

        public static bool TryGetCached(string key, out Sprite sprite) => Cache.TryGetValue(key, out sprite);

        public static Sprite BakeRaw(Color32[] pixels, int width, int height, string key, int border = 0, TextureWrapMode wrap = TextureWrapMode.Clamp, float pixelsPerUnit = 0f)
        {
            return Build(pixels, width, height, key, border, pixelsPerUnit, wrap);
        }

        public static void ClearCache()
        {
            foreach (KeyValuePair<string, Sprite> pair in Cache)
            {
                if (pair.Value == null)
                {
                    continue;
                }

                if (pair.Value.texture != null)
                {
                    Object.Destroy(pair.Value.texture);
                }

                Object.Destroy(pair.Value);
            }

            Cache.Clear();
        }

        /// <summary>Mapa de 3x5 pixeles por digito, para escribir el numero de la bola.</summary>
        private static readonly byte[][] Digits =
        {
            new byte[] { 7, 5, 5, 5, 7 }, // 0
            new byte[] { 2, 6, 2, 2, 7 }, // 1
            new byte[] { 7, 1, 7, 4, 7 }, // 2
            new byte[] { 7, 1, 7, 1, 7 }, // 3
            new byte[] { 5, 5, 7, 1, 1 }, // 4
            new byte[] { 7, 4, 7, 1, 7 }, // 5
            new byte[] { 7, 4, 7, 5, 7 }, // 6
            new byte[] { 7, 1, 1, 1, 1 }, // 7
            new byte[] { 7, 5, 7, 5, 7 }, // 8
            new byte[] { 7, 5, 7, 1, 7 }, // 9
        };

        private static Color32[] _scratch;
        private static int _scratchSize;

        public static void Prewarm(int ballSize)
        {
            Solid();
            Circle(Color.white, "pocket");
            Circle(Color.white, "diamond");
            Circle(Color.white, "aim");
            Circle(Color.white, "ghost");
            Circle(Color.white, "avatar");
            Circle(Color.white, "emblem");
            Rounded(10);
            Rounded(12);
            Rounded(14);
            Rounded(16);
            Rounded(18);
            Rounded(20);
            Rounded(22);

            for (int i = 0; i < 16; i++)
            {
                Ball(i, ballSize);
            }
        }

        public static Sprite Ball(int number) => Ball(number, PerfConfig.BallSpriteSize);

        public static Sprite Ball(int number, int size)
        {
            if (size < 32)
            {
                size = 32;
            }

            string key = "ball_" + number + "_" + size;
            if (Cache.TryGetValue(key, out Sprite cached))
            {
                return cached;
            }

            Color32[] pixels = RentPixels(size * size);
            float radius = size * 0.5f - 0.5f;
            float center = (size - 1) * 0.5f;
            float feather = Mathf.Max(2.8f, size * 0.018f);

            Color main = Theme.BallColor(number);
            bool striped = Theme.IsStriped(number);
            Color shell = striped ? Theme.Hex("#F5F1E6") : main;
            Color patch = Theme.Hex("#FAF7EF");
            var lightDir = new Vector2(-0.45f, 0.62f).normalized;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x - center;
                    float dy = y - center;
                    float dist = Mathf.Sqrt((dx * dx) + (dy * dy));
                    int index = (y * size) + x;
                    float alpha = Mathf.Clamp01((radius - dist) / feather);
                    if (alpha <= 0.001f)
                    {
                        pixels[index] = new Color32(0, 0, 0, 0);
                        continue;
                    }

                    Color color = shell;

                    if (striped)
                    {
                        float stripe = 1f - Smooth01(Mathf.Abs(dy), radius * 0.44f, radius * 0.50f);
                        color = Color.Lerp(shell, main, stripe);
                    }

                    float patchRadius = radius * 0.33f;
                    if (number != 0)
                    {
                        float inPatch = 1f - Smooth01(dist, patchRadius - 1.2f, patchRadius + 1.2f);
                        color = Color.Lerp(color, patch, inPatch);
                    }

                    float nx = dx / Mathf.Max(0.001f, radius);
                    float ny = dy / Mathf.Max(0.001f, radius);
                    float facing = Mathf.Clamp01(Vector2.Dot(new Vector2(nx, ny), lightDir) * 0.5f + 0.62f);
                    float edge = Mathf.Clamp01(1f - Mathf.Pow(Mathf.Clamp01(dist / radius), 6f));
                    float shade = Mathf.Lerp(0.55f, 1.12f, facing) * Mathf.Lerp(0.78f, 1f, edge);
                    color = new Color(
                        Mathf.Clamp01(color.r * shade),
                        Mathf.Clamp01(color.g * shade),
                        Mathf.Clamp01(color.b * shade),
                        alpha);

                    float spec = Mathf.Clamp01(1f - (new Vector2(nx + 0.36f, ny - 0.42f).magnitude * 3.1f));
                    color = Color.Lerp(color, Color.white, spec * 0.55f * alpha);
                    pixels[index] = color;
                }
            }

            if (number > 0)
            {
                DrawNumber(pixels, size, number);
            }

            Sprite sprite = Build(pixels, size, size, key);
            return sprite;
        }

        private static void DrawNumber(Color32[] pixels, int size, int number)
        {
            string text = number.ToString();
            int scale = size >= 200 ? 8 : (size >= 140 ? 6 : 4);
            if (text.Length > 1)
            {
                scale = Mathf.Max(3, scale - 3);
            }
            int glyphW = 3 * scale;
            int gap = scale;
            int totalW = (glyphW * text.Length) + (gap * (text.Length - 1));
            int startX = (size - totalW) / 2;
            int startY = (size - (5 * scale)) / 2;
            var ink = new Color32(26, 26, 30, 255);

            for (int c = 0; c < text.Length; c++)
            {
                int digit = text[c] - '0';
                if (digit < 0 || digit > 9)
                {
                    continue;
                }

                byte[] rows = Digits[digit];
                for (int row = 0; row < 5; row++)
                {
                    // La fila 0 del mapa es la de arriba; la textura crece hacia arriba.
                    int bits = rows[4 - row];
                    for (int col = 0; col < 3; col++)
                    {
                        bool on = (bits & (1 << (2 - col))) != 0;
                        if (!on)
                        {
                            continue;
                        }

                        int px0 = startX + (c * (glyphW + gap)) + (col * scale);
                        int py0 = startY + (row * scale);

                        for (int sy = 0; sy < scale; sy++)
                        {
                            for (int sx = 0; sx < scale; sx++)
                            {
                                int px = px0 + sx;
                                int py = py0 + sy;
                                if (px < 0 || px >= size || py < 0 || py >= size)
                                {
                                    continue;
                                }

                                pixels[(py * size) + px] = ink;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>Circulo liso, util para troneras, puntos de mira y adornos.</summary>
        public static Sprite Circle(Color color, string key)
        {
            string cacheKey = "circle256_" + key;
            if (Cache.TryGetValue(cacheKey, out Sprite cached))
            {
                return cached;
            }

            const int size = 256;
            Color32[] pixels = RentPixels(size * size);
            float radius = size * 0.5f - 0.5f;
            float center = (size - 1) * 0.5f;
            const float feather = 3.2f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x - center;
                    float dy = y - center;
                    float dist = Mathf.Sqrt((dx * dx) + (dy * dy));
                    float alpha = Mathf.Clamp01((radius - dist) / feather) * color.a;
                    pixels[(y * size) + x] = new Color(color.r, color.g, color.b, alpha);
                }
            }

            return Build(pixels, size, size, cacheKey);
        }

        /// <summary>Cuadro blanco de 1x1, base para paneles y barras de la interfaz.</summary>
        public static Sprite Solid()
        {
            if (Cache.TryGetValue("solid", out Sprite cached))
            {
                return cached;
            }

            var pixels = new Color32[4 * 4];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = new Color32(255, 255, 255, 255);
            }

            return Build(pixels, 4, 4, "solid");
        }

        /// <summary>Rectangulo con esquinas redondeadas para tarjetas y botones.</summary>
        public static Sprite Rounded(int radius)
        {
            string key = "rounded_" + radius;
            if (Cache.TryGetValue(key, out Sprite cached))
            {
                return cached;
            }

            int size = Mathf.Max(8, radius * 2 + 2);
            Color32[] pixels = RentPixels(size * size);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float cx = Mathf.Clamp(x + 0.5f, radius, size - radius);
                    float cy = Mathf.Clamp(y + 0.5f, radius, size - radius);
                    float dist = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(cx, cy));
                    float alpha = Mathf.Clamp01(radius - dist + 0.5f);
                    pixels[(y * size) + x] = new Color(1f, 1f, 1f, alpha);
                }
            }

            // 100 px por unidad es la referencia del lienzo: asi el radio en
            // pixeles del sprite coincide con el radio que se ve en pantalla.
            Sprite sprite = Build(pixels, size, size, key, border: radius, pixelsPerUnit: 100f);
            return sprite;
        }

        private static float Smooth01(float value, float edge0, float edge1)
        {
            float t = Mathf.Clamp01((value - edge0) / Mathf.Max(0.0001f, edge1 - edge0));
            return t * t * (3f - (2f * t));
        }

        private static Color32[] RentPixels(int count)
        {
            // SetPixels32 exige un arreglo del TAMANO EXACTO de la textura.
            // Si reutilizamos uno mas grande, Unity revienta (rounded_10).
            if (_scratch == null || _scratch.Length != count)
            {
                _scratch = new Color32[count];
                _scratchSize = count;
            }

            return _scratch;
        }

        private static Sprite Build(Color32[] pixels, int width, int height, string key, int border = 0, float pixelsPerUnit = 0f, TextureWrapMode wrap = TextureWrapMode.Clamp)
        {
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode = wrap,
                name = key,
                hideFlags = HideFlags.HideAndDontSave,
            };

            int expected = width * height;
            if (pixels == null || pixels.Length != expected)
            {
                var exact = new Color32[expected];
                if (pixels != null)
                {
                    int copy = pixels.Length < expected ? pixels.Length : expected;
                    System.Array.Copy(pixels, exact, copy);
                }

                pixels = exact;
            }

            texture.SetPixels32(pixels);
            texture.Apply(false, false);

            var rect = new Rect(0f, 0f, width, height);
            var pivot = new Vector2(0.5f, 0.5f);
            Vector4 borders = border > 0
                ? new Vector4(border, border, border, border)
                : Vector4.zero;

            // Por defecto pixelsPerUnit = ancho, para que el sprite mida
            // exactamente 1 unidad de mundo y la escala del objeto de su tamano.
            float ppu = pixelsPerUnit > 0f ? pixelsPerUnit : width;
            Sprite sprite = Sprite.Create(texture, rect, pivot, ppu, 0, SpriteMeshType.FullRect, borders);
            sprite.name = key;
            sprite.hideFlags = HideFlags.HideAndDontSave;
            Cache[key] = sprite;
            return sprite;
        }
    }
}
