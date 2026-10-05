using Billar.Core;
using Billar.UI;
using UnityEngine;

namespace Billar.Game
{
    /// <summary>
    /// En el editor siempre usa calidad alta. En el celular solo baja si es muy flojo.
    /// Vuelve a aplicar cada Play: si Unity no recarga el dominio, si no se haría
    /// con los sprites chicos del primer arranque.
    /// </summary>
    public static class DeviceQuality
    {
        public static bool Applied { get; private set; }

        public static void Apply()
        {
            ProceduralSprites.ClearCache();

            if (Application.isEditor || Application.platform != RuntimePlatform.Android)
            {
                PerfConfig.ApplyHigh();
            }
            else if (IsLowEndPhone())
            {
                PerfConfig.ApplyLowEnd();
            }
            else
            {
                PerfConfig.ApplyMid();
            }

            QualitySettings.SetQualityLevel(5, true);
            QualitySettings.vSyncCount = 0;
            QualitySettings.antiAliasing = PerfConfig.LowEnd ? 0 : 4;
            QualitySettings.shadows = ShadowQuality.Disable;
            QualitySettings.shadowDistance = 0f;
            QualitySettings.pixelLightCount = 0;
            QualitySettings.anisotropicFiltering = PerfConfig.LowEnd
                ? AnisotropicFiltering.Disable
                : AnisotropicFiltering.Enable;
            QualitySettings.softParticles = false;
            QualitySettings.realtimeReflectionProbes = false;
            QualitySettings.billboardsFaceCameraPosition = false;
            QualitySettings.lodBias = 1f;
            QualitySettings.particleRaycastBudget = 0;
            QualitySettings.skinWeights = SkinWeights.OneBone;
            QualitySettings.maxQueuedFrames = 2;

            Application.targetFrameRate = PerfConfig.TargetFrameRate;
            Application.backgroundLoadingPriority = ThreadPriority.Low;
            Application.runInBackground = false;

            if (!Application.isEditor
                && Application.platform == RuntimePlatform.Android
                && PerfConfig.ResolutionScale < 0.99f)
            {
                int w = Mathf.Max(960, Mathf.RoundToInt(Screen.width * PerfConfig.ResolutionScale));
                int h = Mathf.Max(540, Mathf.RoundToInt(Screen.height * PerfConfig.ResolutionScale));
                Screen.SetResolution(w, h, true);
            }

            try
            {
                ProceduralSprites.Prewarm(PerfConfig.BallSpriteSize);
                ClubArt.Prewarm();
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("No se pudieron precargar los sprites: " + ex.Message);
            }

            Applied = true;
        }

        private static bool IsLowEndPhone()
        {
            if (Application.platform != RuntimePlatform.Android)
            {
                return false;
            }

            int ram = SystemInfo.systemMemorySize;
            int cores = SystemInfo.processorCount;
            int gpuRam = SystemInfo.graphicsMemorySize;

            return (ram > 0 && ram < 2500)
                || (cores > 0 && cores <= 4 && ram > 0 && ram < 3500)
                || (gpuRam > 0 && gpuRam < 256);
        }
    }
}
