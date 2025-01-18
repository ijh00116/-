using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BlackTree.Core;
using BlackTree.Model;

namespace BlackTree.Bundles
{
    public class ViewStageRewardSlot : MonoBehaviour
    {
        public TMP_Text title;
        public Transform rewardSlotParent;

        [HideInInspector] public List<ViewGoodRewardSlot> rewardSlotList = new List<ViewGoodRewardSlot>();

    }

}
