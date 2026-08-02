using UnityEngine;
using UnityEngine.UI;

namespace RPGCombat.UI
{
    [RequireComponent(typeof(CanvasScaler))]
    public class ScreenAdapterUI : MonoBehaviour
    {
        [SerializeField] private RectTransform background;
        private bool _isPortrait;

        private void Awake()
        {
            var canvasScaler = GetComponent<CanvasScaler>();

#if UNITY_WEBGL
        canvasScaler.referenceResolution = new Vector2(1920, 1080);
        Screen.orientation = ScreenOrientation.LandscapeLeft;
#elif UNITY_ANDROID
            canvasScaler.referenceResolution = new Vector2(1080, 1920);
            Screen.orientation = ScreenOrientation.AutoRotation;
#endif
        }

        private void Update()
        {
            bool portrait = Screen.height > Screen.width;

            if (portrait != _isPortrait)
            {
                _isPortrait = portrait;
                AdaptLayout(portrait);
            }
        }

        private void AdaptLayout(bool portrait)
        {
            if (background == null) return;

            background.offsetMin = portrait ? new Vector2(60, 60) : new Vector2(120, 40);
            background.offsetMax = portrait ? new Vector2(-60, -60) : new Vector2(-120, -40);
        }
    }

}