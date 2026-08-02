using System.Diagnostics;
using UnityEngine;

namespace RPGCombat.Utils
{
    public static class Log
    {
        // Incluido en Editor y en Builds
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void Info(string message)
        {
            UnityEngine.Debug.Log(message);
        }

        // Advertencias
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void Warning(string message)
        {
            UnityEngine.Debug.LogWarning(message);
        }

        //logs que SIEMPRE aparecen incluso en producción (como errores críticos)
        public static void Error(string message)
        {
            UnityEngine.Debug.LogError(message);
        }
    }
}
