using RPGCombat.Ads;
using RPGCombat.Characters;
using RPGCombat.Combat;
using RPGCombat.Grid;
using System.Collections.Generic;
using UnityEngine;

public class AdRewardPresenter : MonoBehaviour
{
    [Header("Canal de Eventos")]
    [SerializeField] private AdEventChannelSo adEventChannel;

    [Header("Sistemas Inyectados/Referenciados")]
    [SerializeField] private AdsManager adsManager;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private EnemyAI enemyAI;
    [SerializeField] private TurnManager turnManager;

    private void OnEnable()
    {
        adEventChannel.OnWatchAdRequested += HandleWatchAdRequested;
        adEventChannel.OnRewardGranted += HandleRewardGranted;
    }

    private void OnDisable()
    {
        adEventChannel.OnWatchAdRequested -= HandleWatchAdRequested;
        adEventChannel.OnRewardGranted -= HandleRewardGranted;
    }

    private void HandleWatchAdRequested()
    {
        adsManager.ShowRewarded(OnAdCompletedCallback);
    }

    // Callback interno que se pasa a Unity Ads
    private void OnAdCompletedCallback()
    {
        adEventChannel.RaiseRewardGranted();
    }

    //Reacción a la entrega de la recompensa
    private void HandleRewardGranted()
    {
        List<Enemy> aliveEnemies = turnManager.GetAliveEnemies();
        List<ICharacter> alivePlayers = turnManager.GetAlivePlayers();

        Dictionary<Enemy, Vector2Int> predictions = enemyAI.GetPredictedPositions(aliveEnemies, alivePlayers);

        gridManager.ShowPredictedMovement(predictions);

        turnManager.ActivateEnemyReveal(1);
    }
}

