using BlackTree.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BlackTree.Model;

namespace BlackTree.Bundles
{
    public class ViewCanvasEnterClickerDungeon : ViewCanvas
    {
        public TMP_Text title;
        public TMP_Text desc;
        public TMP_Text currentRSPotionCount;
        public BTButton enterBtn;

        public Transform rewardslotParent;
        public ViewGoodRewardSlot rewardslot_forPet;
        public ViewGoodRewardSlot rewardslot;

        public BTButton[] closeBtn;

        [Header("rateTable")]
        public BTButton openRateTableWindow;
        public BTButton[] closeRateTableWindow;
        public GameObject rateTableWindow;
        public ClickDungeonRateTableSlot rateTableSlot;
        public Transform slotParent;
        public TMP_Text currentdifficulty;
    }
}
