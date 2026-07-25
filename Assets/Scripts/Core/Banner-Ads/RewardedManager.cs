using UnityEngine;
using UnityEngine.Advertisements;
using System;

public class RewardedManager : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    private Action _onRewarded;
    public Action<bool> onAdLoadedChanged;

    //[SerializeField] AdsManager adsManager;
    [SerializeField] BannerManager bannerManager;

    [Header("IDs")]
    [SerializeField] private string _androidAdUnitId = "Rewarded_Android";
    [SerializeField] private string _iOSAdUnitId = "Rewarded_iOS";
    string adUnitId = null;

    public bool adLoaded { get; private set; }
    //private BannerManager bannerManager;

    private void Awake()
    {
#if UNITY_ANDROID
    adUnitId = _androidAdUnitId;
#elif UNITY_IOS
    adUnitId = _iOSAdUnitId;
#elif UNITY_EDITOR
        adUnitId = _androidAdUnitId;
#endif    
    }

    public void Initialize(BannerManager banner)
    {
        this.bannerManager = banner;
        Advertisement.Load(adUnitId, this);
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log("Interstinitial Load ok");
        adLoaded = true;
        onAdLoadedChanged?.Invoke(true);
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogWarning($"Interstinitial Load failed {adUnitId} - {error.ToString()} - {message}"); ;
    }

    public void LoadRewarded()
    {
        Advertisement.Load(adUnitId, this);
    }

    public void ShowRewarded(Action onRewarded)
    {
        _onRewarded = onRewarded;
        if (adLoaded)
        {
            _onRewarded = onRewarded;
            bannerManager.HideBanner();
            Advertisement.Show(adUnitId, this);
        }
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.LogWarning($"Show failed {adUnitId}: {message}");
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log("Empezo a mostrar el interstinitial con exito");
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log("Clickearon el interstinitial!! %%%%%%%%%%%%%%");
    }

    public void OnUnityAdsShowComplete(string _adUnitId, UnityAdsShowCompletionState showCompletionState)
    {

        if (_adUnitId.Equals(adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            _onRewarded?.Invoke();
            _onRewarded = null;
            Debug.Log("Recompensa otorgada");
        }

        //cargando el proximo ad
        adLoaded = false;
        onAdLoadedChanged?.Invoke(false);
        Advertisement.Load(adUnitId, this);
        bannerManager.ShowBanner();
        //adsManager.ShowBanner();
    }
}
