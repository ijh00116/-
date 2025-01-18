using BlackTree.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BlackTree.Model;

namespace BlackTree.Bundles
{
    public class ViewCanvasQuest : ViewCanvas
    {
        public BTSelector panelSelector;
        public BTSelector buttonSelector;

        public BTButton dailyQuestBtn;
        public GameObject dailyRedDot;
        public BTButton repeatQuestBtn;
        public GameObject repeatRedDot;

        public ScrollRect dailyScrollRect;
        public ScrollRect repeatScrollRect;

        public GameObject dailyPanel;
        public GameObject repeatPanel;

        public BTButton[] closeBtn;

        public Slider dailyQuestCompleteCountSlider;
        public TMP_Text dailyQuestCompleteQuestTitle;
        public TMP_Text dailyQuestCompleteCountText;
        public BTButton dailyQuestCompleteQuestRewardBtn;
        public GameObject cantdailyQuestCompleteQuestRewardObj;
        public GameObject alReadyrewardCheck;
        public GameObject dailyQuestCompleteRedDot;

        public TMP_Text TotalRecieveBtnText;
        public BTButton TotalRecieveBtn;

        public ViewQuestSlot questSlotPrefab;

        public BTButton TotalRecieveBtn_repeat;

        public TMP_Text titleTxt;
        
        public TMP_Text dailyquest;
        public TMP_Text dailyquest_on;
        public TMP_Text repeatquest;
        public TMP_Text repeatquest_on;
        public TMP_Text dailyComplete;
        public TMP_Text allReward;
    }
}
