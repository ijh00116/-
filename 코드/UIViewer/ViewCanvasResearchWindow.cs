using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTree.Bundles
{
    public class ViewCanvasResearchWindow : ViewCanvas
    {
        public BTButton[] closeBtn;

        public ScrollRect researchData_scrollRect;
        public ScrollRect currentResearchData_scrollRect;

        public ViewResearchDataSlot researchDataSlotPrefab;
        public ViewCurrentResearchSlot currentResearchDataSlotPrefab;

        public GameObject recommendPlusSlot;

        [Header("detailwindow")]
        public GameObject detailTotalWindow;
        public Image detailWindowBlackBg;
        public RectTransform detailWindowPopup;
        public BTButton[] detailCloseBtn;

        public BTButton startCurrentSelectResearchBtn;
        public Image currentSelectResearchIcon;
        public TMP_Text currentSelectResearchTitle;
        public TMP_Text currentSelectResearchDesc;
        public TMP_Text currentSelectResearchTime;
        public TMP_Text currentSelectResearchCost;
        public TMP_Text currentSelectResearchLevel;

        public TMP_Text tierText;
        public BTButton nextTierBtn;
        public BTButton prevTierBtn;

        [Header("Money")]
        [SerializeField] public ObtainGood obtainResearchPotion;

        public BTButton goodsBtn;

        [Header("∑Œƒ√∂Û¿Ã¬°")]
        public TMP_Text titleTxt;
        public TMP_Text slotExpandNeeded;
        public TMP_Text researchDetailTitle;
        public TMP_Text researchDetailBtnTxt;

    }
}
