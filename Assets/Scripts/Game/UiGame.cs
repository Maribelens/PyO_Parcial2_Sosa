using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using RPGCombat.Characters;
using RPGCombat.Player;

namespace RPGCombat.UI 
{
    // SRP: solo pinta el estado actual en pantalla y traduce clicks de Button
    // en llamadas a PlayerTurnController. No decide reglas de turno ni de combate.


    public class UiGame : MonoBehaviour
    {
        [Header("Dependencias (asignar en Inspector)")]
        [SerializeField] private PlayerTurnController turnController;

        [Header("Panel de turno activo")]
        [SerializeField] private TMP_Text activeCharacterLabel;
        [SerializeField] private TMP_Text phaseLabel;

        [Header("Botones de acción (3 fijos: Melee / Range / Heal)")]
        [SerializeField] private Button meleeButton;
        [SerializeField] private Button rangeButton;
        [SerializeField] private Button healButton;

        [Header("Botones de objetivo (pool fijo, se ocultan los que sobran)")]
        [SerializeField] private List<Button> targetButtons;
        [SerializeField] private List<TMP_Text> targetButtonLabels;

        [Header("HP de todos los personajes")]
        [SerializeField] private List<CharacterHPDisplay> hpDisplays;

        [Header("Panel de fin de partida")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TMP_Text gameOverLabel;

        private List<AvailableAction> currentActions = new();
        private ActionType selectedActionType;

        private void OnEnable()
        {
            turnController.OnPhaseChanged += Refresh;
            turnController.OnTurnEnded += HideTargetButtons;
        }

        private void OnDisable()
        {
            turnController.OnPhaseChanged -= Refresh;
            turnController.OnTurnEnded -= HideTargetButtons;
        }

        private void Start()
        {
            meleeButton.onClick.AddListener(() => SelectActionType(ActionType.Melee));
            rangeButton.onClick.AddListener(() => SelectActionType(ActionType.Range));
            healButton.onClick.AddListener(() => SelectActionType(ActionType.Heal));

            HideTargetButtons();
            gameOverPanel.SetActive(false);
        }

        // Se llama cada vez que cambia la fase del turno (movimiento -> acción)
        public void Refresh()
        {
            UpdateTurnLabels();
            UpdateHPDisplays();

            bool inActionPhase = turnController.CurrentPhase == TurnPhase.Action;
            currentActions = inActionPhase ? turnController.GetAvailableActions() : new List<AvailableAction>();

            UpdateActionButtons();
            HideTargetButtons();
        }

        private void UpdateTurnLabels()
        {
            var active = turnController.ActiveCharacter;
            activeCharacterLabel.text = active != null ? active.CharacterName : "-";
            phaseLabel.text = turnController.CurrentPhase switch
            {
                TurnPhase.Movement => "Moviéndose...",
                TurnPhase.Action => "Elegí una acción",
                _ => ""
            };
        }

        private void UpdateActionButtons()
        {
            meleeButton.gameObject.SetActive(HasAction(ActionType.Melee));
            rangeButton.gameObject.SetActive(HasAction(ActionType.Range));
            healButton.gameObject.SetActive(HasAction(ActionType.Heal));
        }

        private bool HasAction(ActionType type)
        {
            foreach (var action in currentActions)
                if (action.Type == type) return true;
            return false;
        }

        // Click en "Melee attack" / "Range attack" / "Heal": muestra los objetivos válidos
        private void SelectActionType(ActionType type)
        {
            selectedActionType = type;

            List<ICharacter> targets = null;
            foreach (var action in currentActions)
                if (action.Type == type) { targets = action.ValidTargets; break; }

            ShowTargetButtons(targets);
        }

        private void ShowTargetButtons(List<ICharacter> targets)
        {
            HideTargetButtons();
            if (targets == null) return;

            for (int i = 0; i < targets.Count && i < targetButtons.Count; i++)
            {
                var target = targets[i]; // captura local para el closure del listener
                targetButtons[i].gameObject.SetActive(true);
                targetButtonLabels[i].text = $"{target.CharacterName} ({target.CurrentHP}HP)";

                targetButtons[i].onClick.RemoveAllListeners();
                targetButtons[i].onClick.AddListener(() => OnTargetSelected(target));
            }
        }

        private void OnTargetSelected(ICharacter target)
        {
            turnController.ExecuteAction(selectedActionType, target);
        }

        private void HideTargetButtons()
        {
            foreach (var button in targetButtons)
                button.gameObject.SetActive(false);
        }

        private void UpdateHPDisplays()
        {
            foreach (var display in hpDisplays)
                display.Refresh();
        }

        public void ShowGameOver(bool playersWon)
        {
            gameOverPanel.SetActive(true);
            gameOverLabel.text = playersWon ? "¡Victoria!" : "Derrota";
        }
    }
}
