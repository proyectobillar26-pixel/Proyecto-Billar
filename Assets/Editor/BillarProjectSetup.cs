using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Billar.EditorTools
{
    /// <summary>
    /// Deja el proyecto listo solo: crea la escena, la registra en Build Settings
    /// y aplica los ajustes de Android. Asi no hay que configurar nada a mano
    /// despues de abrir el proyecto por primera vez.
    /// </summary>
    [InitializeOnLoad]
    public static class BillarProjectSetup
    {
        public const string ScenePath = "Assets/Scenes/Main.unity";
        private const string SetupDoneKey = "Billar.SetupDone.v1";
        private const string PackageName = "com.billar.ochobolas";

        static BillarProjectSetup()
        {
            EditorApplication.delayCall += () =>
            {
                if (EditorPrefs.GetBool(SetupDoneKey + Application.dataPath, false))
                {
                    EnsureSceneRegistered();
                    return;
                }

                ConfigureProject();
                EditorPrefs.SetBool(SetupDoneKey + Application.dataPath, true);
            };
        }

        [MenuItem("Billar/Configurar proyecto", false, 0)]
        public static void ConfigureProject()
        {
            EnsureScene();
            EnsureSceneRegistered();
            ApplyPlayerSettings();
            ApplyQualitySettings();
            AssetDatabase.SaveAssets();
            Debug.Log("[Billar] Proyecto configurado: escena Main, orientacion horizontal y ajustes de Android listos.");
        }

        private static void EnsureScene()
        {
            if (File.Exists(ScenePath))
            {
                return;
            }

            string folder = Path.GetDirectoryName(ScenePath);
            if (!string.IsNullOrEmpty(folder) && !Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
                AssetDatabase.Refresh();
            }

            // La escena va vacia a proposito: el juego se construye por codigo
            // desde AppBootstrap, asi no hay nada que se pueda romper al moverlo.
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.Refresh();
        }

        private static void EnsureSceneRegistered()
        {
            if (!File.Exists(ScenePath))
            {
                return;
            }

            EditorBuildSettingsScene[] current = EditorBuildSettings.scenes;
            foreach (EditorBuildSettingsScene s in current)
            {
                if (s.path == ScenePath)
                {
                    return;
                }
            }

            var updated = new EditorBuildSettingsScene[current.Length + 1];
            current.CopyTo(updated, 0);
            updated[current.Length] = new EditorBuildSettingsScene(ScenePath, true);
            EditorBuildSettings.scenes = updated;
        }

        private static void ApplyPlayerSettings()
        {
            PlayerSettings.companyName = "Billar";
            PlayerSettings.productName = "Billar 8 Ball";
            PlayerSettings.bundleVersion = "0.1";

            // La mesa se ve mucho mejor apaisada.
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;

            var android = NamedBuildTarget.Android;
            PlayerSettings.SetApplicationIdentifier(android, PackageName);
            PlayerSettings.SetScriptingBackend(android, ScriptingImplementation.IL2CPP);

            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.Android.bundleVersionCode = 1;
            PlayerSettings.Android.forceInternetPermission = true;
            PlayerSettings.Android.optimizedFramePacing = true;
            PlayerSettings.gcIncremental = true;

            // APK suelto para instalar en el celular, no bundle de Play Store.
            EditorUserBuildSettings.buildAppBundle = false;
        }

        private static void ApplyQualitySettings()
        {
            QualitySettings.vSyncCount = 0;
            QualitySettings.antiAliasing = 0;
            QualitySettings.shadows = ShadowQuality.Disable;
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
            Application.targetFrameRate = 60;
        }

        [MenuItem("Billar/Construir APK", false, 20)]
        public static void BuildApk()
        {
            EnsureScene();
            EnsureSceneRegistered();
            ApplyPlayerSettings();

            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
            {
                bool switched = EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
                if (!switched)
                {
                    EditorUtility.DisplayDialog(
                        "Falta el modulo de Android",
                        "Unity no pudo cambiar a Android.\n\n" +
                        "Abre Unity Hub > Installs > los tres puntos de tu version > Add modules y " +
                        "marca \"Android Build Support\" con el SDK, el NDK y OpenJDK.",
                        "Entendido");
                    return;
                }
            }

            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            string outputFolder = Path.Combine(projectRoot, "Builds");
            Directory.CreateDirectory(outputFolder);
            string apkPath = Path.Combine(outputFolder, "Billar8Ball.apk");

            var options = new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = apkPath,
                target = BuildTarget.Android,
                options = BuildOptions.None,
            };

            UnityEditor.Build.Reporting.BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                Debug.Log($"[Billar] APK listo en: {apkPath}");
                EditorUtility.RevealInFinder(apkPath);
            }
            else
            {
                Debug.LogError($"[Billar] La compilacion fallo: {report.summary.result}");
            }
        }
    }
}
