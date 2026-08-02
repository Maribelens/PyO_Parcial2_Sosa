using UnityEngine.Advertisements;

public class InterstitialManager : AdBaseManager
{
    public void LoadInterstitial() => LoadAd();

    public void ShowInterstitial()
    {
        if (adLoaded)
            Advertisement.Show(adUnitId, this);
    }

    public override void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState state)
    {
        LoadAd();
    }
}