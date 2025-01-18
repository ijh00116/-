using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using BlackTree.Core;
using UnityEngine.UI;

namespace BlackTree.Bundles
{
    public class ViewCanvasVipWindow : ViewCanvas
    {
        public Transform slotParent;
        public ViewGoodRewardSlot rewardSlotPrefab;

        public BTButton getRewardBtn;

        public BTButton closeBtn;

        [SerializeField] public ViewPackageRewardSlot vipFixedgoldRewardSlot;
        [SerializeField] public ViewPackageRewardSlot vipFixedExpRewardSlot;
        [SerializeField] public ViewPackageRewardSlot vipFixedDungeonRewardSlot;

        [SerializeField] public Transform vipfixedRewardParent;
        [SerializeField] public ViewPackageRewardSlot vipDailyFixedRewardSlotPrefab;

        public GameObject alreadyRewarded;
    }
}

