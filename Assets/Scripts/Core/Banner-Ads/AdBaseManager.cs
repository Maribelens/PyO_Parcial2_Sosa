using System;
using UnityEngine;
using UnityEngine.Advertisements;

public abstract class AdBaseManager : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [Header("IDs")]
    [SerializeField] private string _androidAdUnitId;
    [SerializeField] private string _iOSAdUnitId;

    protected string adUnitId;
    public bool adLoaded { get; private set; }

    public event Action OnAdStarted;
    public event Action OnAdCompleted;

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

    public virtual void Initialize()
    {
        LoadAd();
    }

    public void LoadAd() => Advertisement.Load(adUnitId, this);

    public virtual void OnUnityAdsAdLoaded(string placementId)
    {
        adLoaded = true;
    }

    public virtual void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message) 
    {
        adLoaded = false;
    }
    public abstract void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState state);

    public virtual void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message) 
    {
        OnAdCompleted?.Invoke();
    }

    public virtual void OnUnityAdsShowStart(string placementId)
    {
        adLoaded = false;
        OnAdStarted?.Invoke();
    }

    public virtual void OnUnityAdsShowClick(string placementId) { }
}
