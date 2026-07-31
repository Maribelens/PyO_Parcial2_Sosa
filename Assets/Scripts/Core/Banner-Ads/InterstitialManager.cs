using UnityEngine;
using UnityEngine.Advertisements;

public class InterstitialManager : AdBaseManager
{
    public bool adLoaded { get; private set; }
    public void LoadInterstitial() => LoadAd();

    public void ShowInterstitial()
    {
        if (adLoaded)
            Advertisement.Show(adUnitId, this);
    }

    public override void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState state)
    {
        Debug.Log("Termino de ver el interstitial");
        LoadAd();
        bannerManager.ShowBanner();
    }
}
////    protected override void OnShowComplete(UnityAdsShowCompletionState completionState)
////    {
////        Debug.Log("Terminó de ver el interstitial");
////    }
////}
