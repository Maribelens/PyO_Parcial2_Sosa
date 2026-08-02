using UnityEngine;
using UnityEngine.UI;

namespace RPGCombat.UI
{
    public static class CanvasGroupExtensionsUI
    {
        //funciones de utilidad sin estado accesible/modificable
        public static void SetState(this CanvasGroup canvasGroup, bool state)
        {
            if (canvasGroup == null) return;
            canvasGroup.alpha = state ? 1f : 0f;
            canvasGroup.interactable = state;
            canvasGroup.blocksRaycasts = state;
        }
    }
}
