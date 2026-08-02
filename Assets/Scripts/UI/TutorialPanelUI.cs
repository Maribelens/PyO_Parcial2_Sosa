using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPGCombat.UI
{
    public class TutorialPanelUI : MonoBehaviour
    {
        [System.Serializable]
        public struct TutorialStep
        {
            public Sprite stepImage;
            [TextArea(3, 5)] public string stepDescription;
        }

        [Header("UI References")]
        [SerializeField] private Image displayImage;
        [SerializeField] private TMP_Text descriptionLabel;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button prevButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private CanvasGroup tutorialCanvasGroup;

        [Header("Content")]
        [SerializeField] private List<TutorialStep> steps;

        private int _currentIndex = 0;

        private void Awake()
        {
            if (tutorialCanvasGroup != null)
                tutorialCanvasGroup.SetState(false);
        }

        private void Start()
        {
            nextButton.onClick.AddListener(NextStep);
            prevButton.onClick.AddListener(PrevStep);
            closeButton.onClick.AddListener(HideTutorial);
        }

        public void ShowTutorial()
        {
            _currentIndex = 0;
            UpdatePage();
            tutorialCanvasGroup.SetState(true);
        }

        public void HideTutorial()
        {
            tutorialCanvasGroup.SetState(false);
        }

        private void NextStep()
        {
            if (_currentIndex < steps.Count - 1)
            {
                _currentIndex++;
                UpdatePage();
            }
        }

        private void PrevStep()
        {
            if (_currentIndex > 0)
            {
                _currentIndex--;
                UpdatePage();
            }
        }

        private void UpdatePage()
        {
            if (steps.Count == 0) return;

            displayImage.sprite = steps[_currentIndex].stepImage;
            descriptionLabel.text = steps[_currentIndex].stepDescription;

            prevButton.interactable = _currentIndex > 0;
            nextButton.interactable = _currentIndex < steps.Count - 1;
        }
    }
}