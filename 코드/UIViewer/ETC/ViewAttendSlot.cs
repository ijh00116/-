using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BlackTree.Core;
using BlackTree.Model;

namespace BlackTree.Bundles
{
    public class ViewAttendSlot : ViewBase
    {
        public BTButton attendTouch;
        public Image rewardImage;
        public TMP_Text amountText;
        public TMP_Text dayText;

        public GameObject RewardAlreadyGet;
        public GameObject TodayRewardPossible;
        public GameObject FutureRewardFrame;

        public void SetButtonData(Definition.AttendInfoData attendTabledata, int slotindex)
        {
            //rewardText.text = StaticData.Wrapper.localizednamelist[(int)Definition.LocalizeNameKeys.OfflineReward_GetReward].StringToLocal;
            amountText.text = $"x{attendTabledata.amount}";
            dayText.text = $"{slotindex + 1}";

            rewardImage.sprite = GoodResourcesBundle.Loaded.goodSprites[(int)attendTabledata.goodsType];

            RewardAlreadyGet.SetActive(false);
            TodayRewardPossible.SetActive(false);
            FutureRewardFrame.SetActive(false);


            if (slotindex <= Player.Cloud.attendData.lastRewardIndex)//past
            {
                RewardAlreadyGet.gameObject.SetActive(true);
            }
            else if ((Player.Cloud.attendData.lastRewardIndex + 1 == slotindex) && IsNextDay())//current
            {
                TodayRewardPossible.SetActive(true);
            }
            else//future
            {
                FutureRewardFrame.SetActive(true);
            }
        }

        bool IsNextDay()
        {
            var time = Extension.GetServerTime();
            return Player.Cloud.attendData.lastRewardedDay < time.DayOfYear;
        }

        public void StartConfirmCheck(System.Action callback)
        {
            RewardAlreadyGet.SetActive(true);
            attendTouch.enabled = false;
        }

       
    }

}
