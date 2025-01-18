using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BlackTree.Core;
using BlackTree.Model;

namespace BlackTree.Bundles
{
    public class ViewMailSlot : MonoBehaviour
    {
        public TMP_Text title;
        public BTButton getBtn;
        public Transform rewardSlotParent;
        public GameObject lockObj;
        public TMP_Text lockedText;

        public string mailTitle;
        public int raidMailIndex;
        public string mailIndate;

        public TMP_Text getBtnTxt;

        [HideInInspector] public List<ViewGoodRewardSlot> rewardSlotList = new List<ViewGoodRewardSlot>();


    }

}
