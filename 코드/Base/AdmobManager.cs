using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

namespace BlackTree.Core
{
    public class AdmobManager : Monosingleton<AdmobManager>
    {
        private RewardedAd rewardedAd;
        string adTestUnitId = "ca-app-pub-3940256099942544/5224354917";
        string adUnitId = "ca-app-pub-8854120897670877/4832553453";
        string adUnitIdforIOS= "ca-app-pub-8854120897670877/3608778101";
        // Start is called before the first frame update
        void Start()
        {
            MobileAds.Initialize(o => {

                LoadRewardedAd();
            });
        }

        public void LoadRewardedAd()
        {
            // Clean up the old ad before loading a new one.
            if (rewardedAd != null)
            {
                rewardedAd.Destroy();
                rewardedAd = null;
            }

            // create our request used to load the ad.
            var adRequest = new AdRequest();


            // send the request to load the ad.
            string Unitid = adUnitId;
#if UNITY_EDITOR
            Unitid = adTestUnitId;
#elif UNITY_ANDROID
            Unitid = adUnitId;
#elif UNITY_IOS
            Unitid = adUnitIdforIOS;
#endif
            RewardedAd.Load(Unitid, adRequest,
                (RewardedAd ad, LoadAdError error) =>
                {
              // if error is not null, the load request failed.
              if (error != null || ad == null)
                    {
                        Debug.LogError("Rewarded ad failed to load an ad " +
                                       "with error : " + error);
                        return;
                    }

                    Debug.Log("Rewarded ad loaded with response : "
                              + ad.GetResponseInfo());

                    rewardedAd = ad;
                    RegisterReloadHandler(rewardedAd);
                    RegisterEventHandlers(rewardedAd);
                });
        }

        private void RegisterEventHandlers(RewardedAd ad)
        {
            // Raised when the ad is estimated to have earned money.
            ad.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(string.Format("Rewarded ad paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));
            };
            // Raised when an impression is recorded for an ad.
            ad.OnAdImpressionRecorded += () =>
            {
                Debug.Log("Rewarded ad recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            ad.OnAdClicked += () =>
            {
                Debug.Log("Rewarded ad was clicked.");
            };
            // Raised when an ad opened full screen content.
            ad.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("Rewarded ad full screen content opened.");
            };
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Rewarded ad full screen content closed.");
            };
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError("Rewarded ad failed to open full screen content " +
                               "with error : " + error);
            };
        }
        private void RegisterReloadHandler(RewardedAd ad)
        {
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += ()=>
            {
                Debug.Log("Rewarded Ad full screen content closed.");

                // Reload the ad so that we can show another as soon as possible.
                LoadRewardedAd();
            };
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError("Rewarded ad failed to open full screen content " +
                               "with error : " + error);

                // Reload the ad so that we can show another as soon as possible.
                LoadRewardedAd();
            };
        }
        public void ShowRewardedAd(System.Action callback)
        {
            const string rewardMsg =
                "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

            if (rewardedAd != null && rewardedAd.CanShowAd())
            {
                rewardedAd.Show((Reward reward) =>
                {
                    //callback?.Invoke();
                    StartCoroutine(CallbackAfterSec(callback));
                    // TODO: Reward the user.
                });
            }
        }

        WaitForSeconds wait = new WaitForSeconds(0.2f);
        private IEnumerator CallbackAfterSec(System.Action callback)
        {
            yield return wait;

            callback?.Invoke();
        }
    }

}
