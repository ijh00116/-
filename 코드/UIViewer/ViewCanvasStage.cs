using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BlackTree.Core;
using TMPro;
using BlackTree.Model;
using BlackTree.Definition;

namespace BlackTree.Bundles
{
    public class ViewCanvasStage : ViewCanvas
    {
        public TMP_Text currentChapterStage_title;
        public TMP_Text selectedChapter;
        public BTButton prevChapter;
        public BTButton nextChapter;

        [Header("스테이지 슬롯 프리팹")]
        public ScrollRect stageSlotView;

        public BTButton[] closeBtn;
        public ViewStageScrollView stageScrollview;
        public ScrollRect scroll;
        public BTButton allRewardBtn;

        [Header("phase 슬롯")]
        public ScrollRect phaseScroll;
        public ViewPhaseBtn phasePrefab;
        public BTSelector phaseSelector;

        [Header("챕터별 나오는 아이템")]
        public BTButton rewardItemWindowBtn;
        public BTButton[] rewardItemWindowCloseBtns;
        public GameObject chapterItemTableWindow;
        public ScrollRect rewardSlotParent;
        public ViewStageRewardSlot chapterRewardSlotPrefab;
        public ViewGoodRewardSlot rewardslotPrefab;
        public ViewGoodRewardSlot rewardslotPrefab_forPet;
        public ViewGoodRewardSlot rewardslotPrefab_forSkill;

        [Header("localize")]
        public TMP_Text titleTxt;
        public TMP_Text rewardTableTxt;
        public TMP_Text allrewardTxt;
        public TMP_Text rewardWindowTitleTxt;

    }
}

