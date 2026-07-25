using UnityEngine;
using UnityEngine.Advertisements;

public class InterstinitialManager : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    //[SerializeField] AdsManager adsManager;
    [SerializeField] BannerManager bannerManager;
    [Header("IDs")]
    [SerializeField] private string _androidAdUnitId = "Interstitial_Android";
    [SerializeField] private string _iOSAdUnitId = "Interstitial_iOS";
    string adUnitId = null;

    public bool adLoaded = false;

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
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogWarning($"Interstinitial Load failed {adUnitId} - {error.ToString()} - {message}"); ;
    }

    public void LoadInterstinitial()
    {
        Advertisement.Load(adUnitId, this);
    }

    public void ShowInterstinitial()
    {
        if (adLoaded)
        {
            Advertisement.Show(adUnitId, this);
            bannerManager.HideBanner();
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

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log("Termino de ver el insterstinitial");
        adLoaded = false;
        Advertisement.Load(adUnitId, this);
        bannerManager.ShowBanner();
        //adsManager.ShowBanner();
    }
}
