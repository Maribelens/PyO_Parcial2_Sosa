using UnityEngine;
using UnityEngine.Advertisements;
using System;
using RPGCombat.Combat;
using RPGCombat.Grid;

public class RewardedManager : AdBaseManager
{
    [Header ("Reward")]
    [SerializeField] TurnManager turnManager;
    [SerializeField] GridManager gridManager;

    private Action _onRewarded;
    private Action<bool> onAdLoadedChanged;

    public override void OnUnityAdsAdLoaded(string placementId)
    {
        base.OnUnityAdsAdLoaded(placementId);
        onAdLoadedChanged?.Invoke(true);
    }

    public void LoadRewarded() => LoadAd();

    public void ShowRewarded(Action onRewarded)
    {
        _onRewarded = onRewarded;
        if (adLoaded)
            Advertisement.Show(adUnitId, this);
    }

    public override void OnUnityAdsShowComplete(string _adUnitId, UnityAdsShowCompletionState state)
    {
        if (_adUnitId.Equals(adUnitId) && state.Equals(UnityAdsCompletionState.COMPLETED))
        {
            _onRewarded?.Invoke();
            _onRewarded = null;
            Debug.Log("Recompensa otorgada");
        }
        onAdLoadedChanged?.Invoke(false);
        LoadAd();
        bannerManager.ShowBanner();
    }

    public void OnRewardedAdCompleted()
    {
        int revealDuration = 2; // dura 2 rondas completas
        gridManager.RevealEnemyPositions(turnManager.GetAliveEnemies(), revealDuration);
        turnManager.ActivateEnemyReveal(revealDuration);
    }
}
