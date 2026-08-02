using System;
using UnityEngine;

namespace RPGCombat.Ads
{
    //transporta eventos
    [CreateAssetMenu(fileName = "AdEventChannel", menuName = "RPGCombat/Ad Event Channel")]
    public class AdEventChannelSo : ScriptableObject
    {
        public event Action OnWatchAdRequested;
        public event Action OnRewardGranted;
        public event Action OnRewardExpired;

        public void RaiseWatchAdRequested() => OnWatchAdRequested?.Invoke();
        public void RaiseRewardGranted() => OnRewardGranted?.Invoke();
        public void RaiseRewardExpired() => OnRewardExpired?.Invoke();
    }
}