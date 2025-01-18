using BlackTree.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BlackTree.Model;

namespace BlackTree.Bundles
{
    public class ViewCanvasDungeonIngame : ViewCanvas
    {

        [Header("InGame_Gold")]
        public GameObject inGameUI_Gold;
        public Slider currentKillEnemySlider;
        public TMP_Text currentKillEnemy_gold;
        public Slider dungeonTime_gold;
        public TMP_Text dungeonTimeText_gold;

        [Header("InGame_exp")]
        public GameObject inGameUI_Exp;
        public Slider dungeonTimeSlider_exp;
        public TMP_Text dungeonTimeText_exp;
        public TMP_Text killenemyCount;

        [Header("InGame_awake")]
        public GameObject inGameUI_awake;
        public TMP_Text totalDmg;
        public Slider  dungeonTimeSlider_awake;
        public TMP_Text dungeonTimeText_awake;

        [Header("InGame_Rift")]
        public GameObject inGameUI_Rift;

        public Slider dungeonWaveSlider_Rift;

        public Slider currentTimeSlider_Rift;
        public TMP_Text dungeonTimeText_Rift;

        [Header("InGame_rune")]
        public GameObject inGameUI_Rune;
        public Slider dungeonTimeSlider_rune;
        public TMP_Text dungeonTimeText_rune;
        public TMP_Text currentLevelCount_rune;

        [Header("ExitWindow")]
        public GameObject exitWindow;

        public GameObject clearWindow;
        public ViewGoodRewardSlot rewardSlotPrefab;
        public TMP_Text popupTitleText_clear;
        public TMP_Text clearLevelDesc_clear;
        public TMP_Text backtoMain_text_clear;
        public Transform rewardSlotParent_clear;

        public GameObject failWindow;
        public TMP_Text popupTitleText_fail;
        public TMP_Text LevelDesc_fail;
        public TMP_Text backtoMain_text_fail;

        public BTButton[] EndDungeonBtn;

        [Header("OutWindow")]
        public GameObject outWindow;
        public BTButton openOutWindowBtn;
        public BTButton[] cancelOutBtn;
        public BTButton confirmOutBtn;

        [Header("localize")]
        public TMP_Text outDungeonTitle;
        public TMP_Text outDungeonDesc;
        public TMP_Text outDungeonCancel;
        public TMP_Text outDungeonOut;

        public TMP_Text exitWindow_success;
        public TMP_Text exitWindow_fail;
        public TMP_Text exitWindowTitletxt_success;
        public TMP_Text exitWindowTitletxt_fail;
        public TMP_Text exitWindowContinue_success;
        public TMP_Text exitWindowContinue_fail;
    }
}

