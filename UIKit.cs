using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Billar.UI
{
    /// <summary>
    /// Atajos para construir la interfaz por codigo. Toda la UI del juego se
    /// arma con estos metodos, asi no hay prefabs que se puedan romper.
    /// </summary>
    public static class UIKit
    {
        /// <summary>Resolucion de referencia: el celular se juega apaisado.</summary>
        public static readonly Vector2 ReferenceResolution = new Vector2(1280f, 720f);

        public static GameObject Node(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go;
        }

        public static RectTransform Rect(GameObject go) => (RectTransform)go.transform;

        /// <summary>Estira el elemento sobre todo el padre, con margenes opcionales.</summary>
        public static RectTransform Stretch(GameObject go, float left = 0f, float bottom = 0f, float right = 0f, float top = 0f)
        {
            RectTransform rt = Rect(go);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(left, bottom);
            rt.offsetMax = new Vector2(-right, -top);
            return rt;
        }

        /// <summary>Coloca el elemento respecto al centro del padre.</summary>
        public static RectTransform Place(GameObject go, float x, float y, float width, float height)
        {
            RectTransform rt = Rect(go);
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(width, height);
            rt.anchoredPosition = new Vector2(x, y);
            return rt;
        }

        /// <summary>Coloca el elemento anclado a una esquina (0,0 abajo-izquierda; 1,1 arriba-derecha).</summary>
        public static RectTransform Corner(GameObject go, Vector2 anchor, Vector2 offset, float width, float height)
        {
            RectTransform rt = Rect(go);
            rt.anchorMin = anchor;
            rt.anchorMax = anchor;
            rt.pivot = anchor;
            rt.sizeDelta = new Vector2(width, height);
            rt.anchoredPosition = offset;
            return rt;
        }

        public static Image Panel(Transform parent, string name, Color color, int radius = 16)
        {
            GameObject go = Node(name, parent);
            var image = go.AddComponent<Image>();

            if (radius > 0)
            {
                image.sprite = ProceduralSprites.Rounded(radius);
                image.type = Image.Type.Sliced;
            }
            else
            {
                image.sprite = ProceduralSprites.Solid();
            }

            image.color = color;
            return image;
        }

        public static Text Label(
            Transform parent,
            string name,
            string content,
            int size,
            Color color,
            TextAnchor anchor = TextAnchor.MiddleCenter,
            FontStyle style = FontStyle.Normal)
        {
            GameObject go = Node(name, parent);
            var text = go.AddComponent<Text>();
            text.font = Theme.Font;
            text.text = content;
            text.fontSize = size;
            text.color = color;
            text.alignment = anchor;
            text.fontStyle = style;
            text.supportRichText = false;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;
            return text;
        }

        public static Button Button(
            Transform parent,
            string name,
            string caption,
            Color background,
            Color foreground,
            int fontSize = Theme.BodySize,
            int radius = 18)
        {
            Image image = Panel(parent, name, background, radius);
            var button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;

            var colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1.1f, 1.1f, 1.1f, 1f);
            colors.pressedColor = new Color(0.82f, 0.82f, 0.82f, 1f);
            colors.selectedColor = Color.white;
            colors.disabledColor = new Color(1f, 1f, 1f, 0.35f);
            colors.fadeDuration = 0.06f;
            button.colors = colors;

            Text label = Label(image.transform, "Texto", caption, fontSize, foreground, TextAnchor.MiddleCenter, FontStyle.Bold);
            Stretch(label.gameObject, 8f, 4f, 8f, 4f);

            return button;
        }

        public static void SetCaption(Button button, string caption)
        {
            Text label = button.GetComponentInChildren<Text>();
            if (label != null)
            {
                label.text = caption;
            }
        }

        public static InputField Field(
            Transform parent,
            string name,
            string placeholder,
            InputField.ContentType contentType = InputField.ContentType.Standard)
        {
            Image background = Panel(parent, name, Theme.PanelSoft, 12);

            Text text = Label(background.transform, "Texto", string.Empty, Theme.BodySize, Theme.Cream, TextAnchor.MiddleLeft);
            text.supportRichText = false;
            Stretch(text.gameObject, 16f, 6f, 16f, 6f);

            Text hint = Label(background.transform, "Pista", placeholder, Theme.BodySize, Theme.Muted, TextAnchor.MiddleLeft, FontStyle.Italic);
            Stretch(hint.gameObject, 16f, 6f, 16f, 6f);

            var field = background.gameObject.AddComponent<InputField>();
            field.textComponent = text;
            field.placeholder = hint;
            field.targetGraphic = background;
            field.contentType = contentType;
            field.lineType = InputField.LineType.SingleLine;
            field.caretColor = Theme.Gold;
            field.customCaretColor = true;
            field.selectionColor = Theme.Fade(Theme.Gold, 0.35f);

            return field;
        }

        /// <summary>Linea fina de separacion.</summary>
        public static Image Divider(Transform parent, float width)
        {
            Image line = Panel(parent, "Linea", Theme.Fade(Theme.Gold, 0.35f), 0);
            Place(line.gameObject, 0f, 0f, width, 2f);
            return line;
        }

        public static void OnClick(Button button, UnityAction action)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(action);
        }

        /// <summary>
        /// Marco de color alrededor de una tarjeta ya colocada. Se crea como
        /// hermano justo detras de ella para que no le tape el contenido.
        /// </summary>
        public static Image Outline(Transform card, Color color, int radius = 16)
        {
            var cardRect = (RectTransform)card;
            Image outline = Panel(card.parent, "Marco", color, radius);
            outline.raycastTarget = false;

            RectTransform rt = Rect(outline.gameObject);
            rt.anchorMin = cardRect.anchorMin;
            rt.anchorMax = cardRect.anchorMax;
            rt.pivot = cardRect.pivot;
            rt.anchoredPosition = cardRect.anchoredPosition;
            rt.sizeDelta = cardRect.sizeDelta + new Vector2(6f, 6f);
            rt.SetSiblingIndex(cardRect.GetSiblingIndex());

            return outline;
        }
    }
}
