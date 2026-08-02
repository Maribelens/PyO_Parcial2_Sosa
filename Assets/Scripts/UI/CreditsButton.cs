using UnityEngine;
using UnityEngine.UI;

public class CreditsButton : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private CanvasScaler canvasScaler;

    [SerializeField] private CanvasGroup gameCanvasGroup;
    [SerializeField] private CanvasGroup creditsCanvasGroup;

    [Header("Game Elements")]
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button creditsCloseButton;

    [Header("Layout")]
    [SerializeField] private RectTransform background;

    private bool _isPortrait;

    private void Awake()
    {
        AddButtonsListeners();
        SetStateCanvasGroup(gameCanvasGroup, true);
        SetStateCanvasGroup(creditsCanvasGroup, false);

#if UNITY_WEBGL
    canvasScaler.referenceResolution = new Vector2(1920, 1080);
    Screen.orientation = ScreenOrientation.LandscapeLeft;
#elif UNITY_ANDROID
        canvasScaler.referenceResolution = new Vector2(1080, 1920);
        Screen.orientation = ScreenOrientation.AutoRotation;
#endif
    }

    private void AddButtonsListeners()
    {
        creditsButton.onClick.AddListener(ShowCredits);
        creditsCloseButton.onClick.AddListener(HideCredits);
    }

    void Update()
    {
        bool portrait = Screen.height > Screen.width;

        // Solo recalcula si cambió la orientación
        if (portrait != _isPortrait)
        {
            _isPortrait = portrait;
            AdaptLayout(portrait);
        }
    }

    private void AdaptLayout(bool portrait)
    {
        if (portrait)
        {
            // Márgenes para portrait
            background.offsetMin = new Vector2(60, 60);
            background.offsetMax = new Vector2(-60, -60);
        }
        else
        {
            // Márgenes para landscape (más espacio lateral)
            background.offsetMin = new Vector2(120, 40);
            background.offsetMax = new Vector2(-120, -40);
        }
    }

    private void SetStateCanvasGroup(CanvasGroup canvasGroup, bool state)
    {
        // Activa o desactiva visibilidad e interacción de un panel
        canvasGroup.alpha = state ? 1 : 0;
        canvasGroup.interactable = state;
        canvasGroup.blocksRaycasts = state;
    }

    private void ShowCredits()
    {
        SetStateCanvasGroup(creditsCanvasGroup, true);
        creditsButton.interactable = false;
    }

    private void HideCredits()
    {
        SetStateCanvasGroup(creditsCanvasGroup, false);
        creditsButton.interactable = true;
    }

    private void OnDestroy()
    {
        RemoveButtonsListeners();
    }

    private void RemoveButtonsListeners()
    {
        creditsButton.onClick.RemoveAllListeners();
        creditsCloseButton.onClick.RemoveAllListeners();
    }
}    

