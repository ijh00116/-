using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using BlackTree.Core;
using UnityEngine.UI;
using BlackTree.Definition;

namespace BlackTree.Bundles
{
    public class ViewCanvasOfflineReward : ViewCanvas
    {
        public GameObject popupWindow;
        public RectTransform popupRect;

        public Transform slotParent;
        public ViewGoodRewardSlot rewardSlotPrefab;
        public ViewGoodRewardSlot rewardSlotPrefab_skill;
        public ViewGoodRewardSlot rewardSlotPrefab_pet;
        public Slider offlineMinuteSlider;
        public BTButton getRewardBtn;
        public BTButton getDoubleRewardBtn;

        public TMP_Text _textTitle;
        public TMP_Text offlineDesc;
        public TMP_Text _textOfflineTime;
        public TMP_Text rewardTxt;

        public void SetOfflineTime(int minute)
        {
            string text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_RewardTime].StringToLocal;
            _textOfflineTime.text = string.Format(text, minute,Model.Player.OfflineReward.maxMinute);

            offlineMinuteSlider.value = (float)minute / (float)Model.Player.OfflineReward.maxMinute;
        }

        public void SetLocalizedText()
        {

        }

        private void OnApplicationPause(bool pause)
        {
            ControllerOfflineReward.OnReward?.Invoke();
        }
    }

}
