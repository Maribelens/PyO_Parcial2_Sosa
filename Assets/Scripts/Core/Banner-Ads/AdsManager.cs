using System;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour, IUnityAdsInitializationListener
{
    [Header("Game ID")]
    private string gameId;
    [SerializeField] private bool isTestMode = true;

    [Header("Ad Managers")]
    [SerializeField] private BannerManager bannerManager;
    [SerializeField] private InterstitialManager interstitialManager;
    [SerializeField] private RewardedManager rewardedManager;

    private void Awake()
    {
#if UNITY_ANDROID
        gameId = "6159286";
#elif UNITY_IOS
    gameId = "6159287";
#elif UNITY_EDITOR
        gameId = "6159286";
#endif

        if (!Advertisement.isInitialized && Advertisement.isSupported)
            Advertisement.Initialize(gameId, isTestMode, this);
    }
    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads inicialization completed");
        bannerManager.LoadBanner();
        interstitialManager.Initialize(bannerManager);
        interstitialManager.LoadInterstitial();
        rewardedManager.Initialize(bannerManager);
        rewardedManager.LoadRewarded();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads inicialization error: {error.ToString()} - {message}");
    }

    public void ShowBanner() => bannerManager.LoadBanner();
    public void HideBanner() => bannerManager.HideBanner();
    public void ShowInterstitial() => interstitialManager.ShowInterstitial();
    public void ShowRewarded(Action onRewarded) => rewardedManager.ShowRewarded(onRewarded);
}