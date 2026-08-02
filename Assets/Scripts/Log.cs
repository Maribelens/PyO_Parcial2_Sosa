using System.Diagnostics;
using UnityEngine;

namespace RPGCombat.Utils
{
    public static class Log
    {
        // Se incluirá en el Editor y en Builds de Desarrollo de Android/iOS
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void Info(string message)
        {
            UnityEngine.Debug.Log(message);
        }

        //logs que SIEMPRE aparezcan incluso en producción (como errores críticos)
        public static void Error(string message)
        {
            UnityEngine.Debug.LogError(message);
        }
    }
}
