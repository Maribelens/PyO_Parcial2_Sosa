using System.Collections;
using UnityEngine;
using RPGCombat.Combat;
using RPGCombat.Grid;
using RPGCombat.Player;
using RPGCombat.UI;
using RPGCombat.Ads;

namespace RPGCombat
{
    // SRP maneja ciclo de vida completo de la partida (loop de turnos)
    // Es el único script que conoce el orden: Init -> turno jugador 1,2,3 -> turno enemigos -> repetir -> game over
    // DIP toda dependencia viene del Inspector

    public class GameFlowController : MonoBehaviour
    {
        [Header("Dependencias (asignar en Inspector)")]
        [SerializeField] private GameInitializer gameInitializer;
        [SerializeField] private GridManager gridManager;
        [SerializeField] private TurnManager turnManager;
        [SerializeField] private CombatActions combatActions;
        [SerializeField] private EnemyAI enemyAI;
        [SerializeField] private PlayerController playerMovement;
        [SerializeField] private PlayerTurnController turnController;
        [SerializeField] private UiGame gameUI;

        [Header("Ads & Notifications")]
        [SerializeField] private AdEventChannelSo adEventChannel;
        [SerializeField] private AdsManager adsManager;
        [SerializeField] private NotificationManager notificationManager;

        private int currentPlayerTurnIndex = 0;

        private IEnumerator Start()
        {
            // El orden importa: GameInitializer ya corrió su propio Start()
            // (instancia personajes e inyecta GridManager/TurnManager/EnemyAI).
            // inyeccion restante de PlayerController y PlayerTurnController.

            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;

            yield return null;
            playerMovement.Initialize(gridManager);
            turnController.Initialize(gridManager, combatActions, turnManager, playerMovement);
            turnController.OnTurnEnded += OnPlayerTurnEnded;

            gameInitializer.ConnectHPDisplays(gameUI.GetHPDisplays());

            StartNextPlayerTurn();
        }

        private void OnEnable()
        {
            adEventChannel.OnWatchAdRequested += OnWatchAdRequested;
            adEventChannel.OnRewardGranted += OnRewardGranted;
        }

        private void OnDisable()
        {
            adEventChannel.OnWatchAdRequested -= OnWatchAdRequested;
            adEventChannel.OnRewardGranted -= OnRewardGranted;
        }

        private void OnDestroy()
        {
            turnController.OnTurnEnded -= OnPlayerTurnEnded;
        }

        private void OnWatchAdRequested()
        {
#if UNITY_ANDROID
            adsManager.ShowRewarded(() =>
            {
                adEventChannel.RaiseRewardGranted();
            });
#endif
        }

        private void OnRewardGranted()
        {
            var predictions = enemyAI.GetPredictedPositions(
                turnManager.GetAliveEnemies(),
                turnManager.GetAlivePlayers()
            );

            gridManager.ShowPredictedMovement(predictions);
            turnManager.ActivateEnemyReveal(2);
        }

        private void StartNextPlayerTurn()
        {
            var alivePlayers = turnManager.GetAlivePlayers();

            if (currentPlayerTurnIndex >= alivePlayers.Count)
            {
                currentPlayerTurnIndex = 0;
                StartCoroutine(RunEnemyTurn());
                return;
            }

            var nextCharacter = alivePlayers[currentPlayerTurnIndex];
            turnController.StartTurnFor(nextCharacter);
        }

        private void OnPlayerTurnEnded()
        {
            currentPlayerTurnIndex++;

            if (CheckGameOver()) return;

            StartNextPlayerTurn();
        }

        private IEnumerator RunEnemyTurn()
        {
            yield return StartCoroutine(
                enemyAI.ExecuteAllEnemyTurns(turnManager.GetAliveEnemies(), turnManager.GetAlivePlayers())
            );

            if (CheckGameOver()) yield break;

            turnManager.OnRoundEnded();

            StartNextPlayerTurn();
        }

        private bool CheckGameOver()
        {
            if (turnManager.DidPlayersWin())
            {
                gameUI.ShowGameOver(playersWon: true);
#if UNITY_ANDROID
                adsManager.ShowInterstitial();
                notificationManager.ScheduleNotification(
                    "¡Victoria!",
                    "¡Ganaste la batalla! ¿Te animás a jugar de nuevo?",
                    10
                );
#endif
                return true;
            }

            if (turnManager.DidPlayersLose())
            {
                gameUI.ShowGameOver(playersWon: false);
#if UNITY_ANDROID
                adsManager.ShowInterstitial();
                notificationManager.ScheduleNotification(
                    "¡Derrota!",
                    "Tus héroes cayeron en batalla. ¡Volvé a intentarlo!",
                    10
                );
#endif
                return true;
            }

            return false;
        }
    }
}