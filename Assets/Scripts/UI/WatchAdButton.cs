using UnityEngine;
using UnityEngine.UI;
using RPGCombat.Ads;

namespace RPGCombat.UI
{
    // SRP Escucha el click y publica el evento
    // No sabe nada de ads ni de recompensas
    public class WatchAdButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private AdEventChannelSo adEventChannel;

        private void Awake()
        {
            button.onClick.AddListener(OnButtonClicked);
        }

        private void OnEnable()
        {
            adEventChannel.OnRewardExpired += ResetButton;
        }

        private void OnDisable()
        {
            adEventChannel.OnRewardExpired -= ResetButton;
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            button.interactable = false; //evita doble click
            adEventChannel.RaiseWatchAdRequested();
        }

        public void ResetButton() 
        {
            button.interactable = true;
        }
    }
}