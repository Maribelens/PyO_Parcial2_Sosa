using UnityEngine;
using UnityEngine.UI;

namespace RPGCombat.UI
{
    public class CreditsControllerUI : MonoBehaviour
    {
        [Header("Canvas Groups")]
        [SerializeField] private CanvasGroup gameCanvasGroup;
        [SerializeField] private CanvasGroup creditsCanvasGroup;

        [Header("Buttons")]
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button creditsCloseButton;

        //[Header("Canvas")]
        //[SerializeField] private CanvasScaler canvasScaler;

        //[SerializeField] private CanvasGroup gameCanvasGroup;
        //[SerializeField] private CanvasGroup creditsCanvasGroup;

        //[Header("Game Elements")]
        //[SerializeField] private Button creditsButton;
        //[SerializeField] private Button creditsCloseButton;

        //[Header("Layout")]
        //[SerializeField] private RectTransform background;

        //private bool _isPortrait;

        private void Awake()
        {
            creditsButton.onClick.AddListener(ShowCredits);
            creditsCloseButton.onClick.AddListener(HideCredits);

            gameCanvasGroup.SetState(true);
            creditsCanvasGroup.SetState(false);
        }

        private void ShowCredits()
        {
            creditsCanvasGroup.SetState(true);
            creditsButton.interactable = false;
        }

        private void HideCredits()
        {
            creditsCanvasGroup.SetState(false);
            creditsButton.interactable = true;
        }

        private void OnDestroy()
        {
            creditsButton.onClick.RemoveAllListeners();
            creditsCloseButton.onClick.RemoveAllListeners();
        }
    }
}
