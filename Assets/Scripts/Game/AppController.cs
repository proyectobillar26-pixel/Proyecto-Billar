using System.Collections;
using Billar.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Billar.Game
{
    /// <summary>
    /// Arma la camara y el lienzo, y se encarga de ir de una pantalla a otra.
    /// Es el unico objeto que sobrevive entre pantallas.
    /// </summary>
    public sealed class AppController : MonoBehaviour
    {
        private GameObject _screenRoot;
        private GameObject _worldRoot;
        private Text _toast;
        private Coroutine _toastRoutine;

        public Canvas Canvas { get; private set; }

        public Camera MainCamera { get; private set; }

        private void Awake()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            BuildCamera();
            DeviceQuality.Apply();
            BuildCanvas();
            BuildEventSystem();
            BuildToast();
        }

        private void Start()
        {
            if (AuthService.TryRestoreSession())
            {
                ShowHome();
            }
            else
            {
                ShowLogin();
            }
        }

        private void BuildCamera()
        {
            var go = new GameObject("Camara Principal");
            go.transform.SetParent(transform, false);
            MainCamera = go.AddComponent<Camera>();
            MainCamera.orthographic = true;
            MainCamera.orthographicSize = 1f;
            MainCamera.clearFlags = CameraClearFlags.SolidColor;
            MainCamera.backgroundColor = Theme.Background;
            MainCamera.allowMSAA = true;
            MainCamera.allowHDR = false;
            MainCamera.transform.position = new Vector3(0f, 0f, -10f);
            go.tag = "MainCamera";
        }

        private void BuildCanvas()
        {
            var go = new GameObject("Lienzo");
            go.transform.SetParent(transform, false);

            Canvas = go.AddComponent<Canvas>();
            Canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            Canvas.pixelPerfect = false;

            var scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = UIKit.ReferenceResolution;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            go.AddComponent<GraphicRaycaster>();
        }

        private void BuildEventSystem()
        {
            if (EventSystem.current != null)
            {
                return;
            }

            var go = new GameObject("Sistema de Eventos");
            go.transform.SetParent(transform, false);
            go.AddComponent<EventSystem>();
            go.AddComponent<StandaloneInputModule>();
        }

        private void BuildToast()
        {
            _toast = UIKit.Label(Canvas.transform, "Aviso", string.Empty, Theme.BodySize, Theme.Cream);
            RectTransform rt = UIKit.Corner(_toast.gameObject, new Vector2(0.5f, 0f), new Vector2(0f, 26f), 900f, 44f);
            rt.pivot = new Vector2(0.5f, 0f);
            _toast.gameObject.SetActive(false);
            _toast.transform.SetAsLastSibling();
        }

        /// <summary>Mensaje corto en la parte de abajo.</summary>
        public void Toast(string message, float seconds = 2.4f)
        {
            if (_toast == null)
            {
                return;
            }

            _toast.text = message;
            _toast.gameObject.SetActive(true);
            _toast.transform.SetAsLastSibling();

            if (_toastRoutine != null)
            {
                StopCoroutine(_toastRoutine);
            }

            _toastRoutine = StartCoroutine(HideToast(seconds));
        }

        private IEnumerator HideToast(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            if (_toast != null)
            {
                _toast.gameObject.SetActive(false);
            }

            _toastRoutine = null;
        }

        /// <summary>Limpia la pantalla anterior y devuelve el contenedor de la nueva.</summary>
        public Transform NewScreen(string name, bool matchOverlay = false)
        {
            if (_screenRoot != null)
            {
                Destroy(_screenRoot);
            }

            if (_worldRoot != null)
            {
                Destroy(_worldRoot);
                _worldRoot = null;
            }

            _screenRoot = UIKit.Node(name, Canvas.transform);
            UIKit.Stretch(_screenRoot);
            _screenRoot.transform.SetAsFirstSibling();

            if (matchOverlay)
            {
                // La mesa se ve por la camara: el HUD va encima, sin tapar el pano.
                MainCamera.backgroundColor = Theme.Hex("#1A120C");
            }
            else
            {
                Image background = UIKit.Panel(_screenRoot.transform, "FondoClub", Color.white, 0);
                background.sprite = ClubArt.ClubBackdrop();
                background.type = Image.Type.Simple;
                background.preserveAspect = false;
                UIKit.Stretch(background.gameObject);
                background.raycastTarget = false;
                MainCamera.backgroundColor = Theme.Background;
            }

            return _screenRoot.transform;
        }

        /// <summary>Contenedor para los objetos de mundo (la mesa).</summary>
        public Transform NewWorld(string name)
        {
            if (_worldRoot != null)
            {
                Destroy(_worldRoot);
            }

            _worldRoot = new GameObject(name);
            _worldRoot.transform.SetParent(transform, false);
            return _worldRoot.transform;
        }

        // ----- Navegacion -----

        public void ShowLogin() => LoginScreen.Build(this);

        public void ShowRegister() => RegisterScreen.Build(this);

        public void ShowHome() => HomeScreen.Build(this);

        // Las demás pantallas (tienda, álbum, perfil, preparación, mesa y resultado)
        // se agregan en las siguientes semanas.

        /// <summary>Cierra la sesión y vuelve al login (RF-05).</summary>
        public void LogoutAndGoToLogin()
        {
            if (AuthService.Logout())
            {
                ShowLogin();
                Toast("Sesión cerrada correctamente.");
            }
            else
            {
                Toast("No se pudo cerrar la sesión. Inténtalo de nuevo.");
            }
        }
    }
}
