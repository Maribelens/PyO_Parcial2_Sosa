using UnityEngine;
using UnityEngine.Advertisements;
using System;
using RPGCombat.Combat;
using RPGCombat.Grid;
using RPGCombat.Characters;
using System.Collections.Generic;
using UnityEngine.TextCore.Text;
using RPGCombat.Player;
using System.Collections;
using RPGCombat.Ads;

public class RewardedManager : AdBaseManager
{
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
        if (_adUnitId.Equals(adUnitId) && state.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            _onRewarded?.Invoke();
            _onRewarded = null;
        }
        onAdLoadedChanged?.Invoke(false);
        LoadAd();
        bannerManager.ShowBanner();
    }
}