using UnityEngine;
using UnityEngine.Advertisements;

public abstract class AdBaseManager : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [Header("IDs")]
    [SerializeField] private string _androidAdUnitId;
    [SerializeField] private string _iOSAdUnitId;

    protected string adUnitId;
    protected IBannerManager bannerManager;
    //protected BannerManager bannerManager;
    public bool adLoaded { get; private set; }

    protected virtual void Awake()
    {
#if UNITY_ANDROID
        adUnitId = _androidAdUnitId;
#elif UNITY_IOS
        AdUnitId = _iOSAdUnitId;
#elif UNITY_EDITOR
        AdUnitId = _androidAdUnitId;
#endif
    }

    public virtual void Initialize(IBannerManager banner) 
    {
        bannerManager = banner;
        Advertisement.Load(adUnitId, this);
    }

    public void LoadAd() => Advertisement.Load(adUnitId, this);

    public virtual void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log($"[{GetType().Name}] Load ok");
        adLoaded = true;
    }

    public virtual void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogWarning($"[{GetType().Name}] Load failed {adUnitId} - {error} - {message}");
    }

    public abstract void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState state);

    public virtual void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.LogWarning($"Show failed {adUnitId}: {message}");
    }

    public virtual void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log($"{GetType().Name} empezo a mostrarse");
        adLoaded = false;
        bannerManager.HideBanner();
    }

    public virtual void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log($"[{GetType().Name}] Click registrado");
    }
}
