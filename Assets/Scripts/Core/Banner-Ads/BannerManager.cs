using UnityEngine;
using UnityEngine.Advertisements;

public class BannerManager : MonoBehaviour, IBannerManager
{
    [Header("IDs")]
    [SerializeField] private string _androidAdUnitId = "Banner_Android";
    [SerializeField] private string _iOSAdUnitId = "Banner_iOS";
    string adUnitId = null;

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

    public void LoadBanner()
    {
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = OnBannerLoad,
            errorCallback = OnBannerLoadError
        };
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        Advertisement.Banner.Load(adUnitId, options);
    }

    private void OnBannerLoad()
    {
        Advertisement.Banner.Show(adUnitId);
    }

    private void OnBannerLoadError(string message)
    {
        Debug.Log($"Banner load error: {message}");
    }

    public void HideBanner()
    {
        Advertisement.Banner.Hide();
    }

    public void ShowBanner()
    {
        Advertisement.Banner.Show(adUnitId);
    }
}
