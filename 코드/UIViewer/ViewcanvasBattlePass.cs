using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BlackTree.Core;
using TMPro;
using AssetKits.ParticleImage;

namespace BlackTree.Bundles
{
    public class ViewcanvasBattlePass : ViewCanvas
    {
        public BTButton[] closeBtns;

        public BTSelector topBtnSelectors;

        public BTButton levelBtn;
        public BTButton chapterBtn;

        public BTButton nextBtn;
        public TMP_Text currentTierText;
        public BTButton prevBtn;

        public BTButton purchaseBtn;
        public TMP_Text purchasePrice;

        public BattlePassSlot normalSlot;
        public BattlePassSlot premiumSlot;

        public Transform normalSlotParent;
        public Transform prmiumSlotParent;
        public Slider passSlider;
        public RectTransform sliderObject;

        public ScrollRect scrollrect;

        [Header("∑Œƒ√∂Û¿Ã¬°")]
        public TMP_Text titleTxt;
        public TMP_Text levelTxt;
        public TMP_Text levelTxt_on;
        public TMP_Text chapterTxt;
        public TMP_Text chapterTxt_on;
    }

}
