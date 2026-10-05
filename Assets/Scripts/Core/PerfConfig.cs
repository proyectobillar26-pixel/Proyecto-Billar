namespace Billar.Core
{
    /// <summary>
    /// Ajustes de rendimiento. DeviceQuality los baja en celulares flojos
    /// antes de abrir la mesa. El motor de fisica lee estos numeros.
    /// </summary>
    public static class PerfConfig
    {
        public static bool LowEnd;

        /// <summary>Pasos de fisica por segundo. 240 se siente igual y cuesta la mitad que 480.</summary>
        public static int SubstepsPerSecond = 240;

        /// <summary>Fotos que se guardan para animar. La vista interpola entre ellas a 60 FPS.</summary>
        public static int SnapshotsPerSecond = 40;

        public static double MaxSimulationSeconds = 12.0;

        public static int AimDotCount = 16;

        public static int ObjectDotCount = 7;

        /// <summary>Lado en pixeles del sprite de cada bola.</summary>
        public static int BallSpriteSize = 256;

        public static int BotSimulations = 10;

        public static int TargetFrameRate = 60;

        /// <summary>0.75 a 1. En low-end se baja la resolucion de pantalla.</summary>
        public static float ResolutionScale = 1f;

        public static void ApplyLowEnd()
        {
            LowEnd = true;
            SubstepsPerSecond = 180;
            SnapshotsPerSecond = 28;
            MaxSimulationSeconds = 10.0;
            AimDotCount = 11;
            ObjectDotCount = 5;
            BallSpriteSize = 96;
            BotSimulations = 7;
            TargetFrameRate = 60;
            ResolutionScale = 0.85f;
        }

        public static void ApplyMid()
        {
            LowEnd = false;
            SubstepsPerSecond = 240;
            SnapshotsPerSecond = 40;
            MaxSimulationSeconds = 12.0;
            AimDotCount = 16;
            ObjectDotCount = 7;
            BallSpriteSize = 192;
            BotSimulations = 10;
            TargetFrameRate = 60;
            ResolutionScale = 1f;
        }

        public static void ApplyHigh()
        {
            LowEnd = false;
            SubstepsPerSecond = 240;
            SnapshotsPerSecond = 48;
            MaxSimulationSeconds = 12.0;
            AimDotCount = 18;
            ObjectDotCount = 8;
            BallSpriteSize = 256;
            BotSimulations = 12;
            TargetFrameRate = 60;
            ResolutionScale = 1f;
        }
    }
}
