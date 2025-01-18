using BlackTree.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BlackTree.Model;
using BlackTree.Definition;
using System.Collections;
using DG.Tweening;

namespace BlackTree.Bundles
{
    public class ViewCanvasContents : ViewCanvas
    {
        public BTButton EnterRPDungeon;
        public BTButton EnterEXPDungeon;
        public BTButton EnterAwakeDungeon;
        public BTButton EnterRiftDungeon;
        public BTButton EnterRuneDungeon;

        public BTButton detailCloseBtn;
        public RectTransform totalDetailWindow;
        public Image detailBlackBG;
        [Header("rewardSlot prefab")]
        public ViewGoodRewardSlot rewardSlotPrefab;
        public ViewGoodRewardSlot rewardSlotPrefab_pet;

        [Header("RPDungeon detail")]
        public GameObject RPDetailWindow;
        public TMP_Text bestLevel_RP;
        public TMP_Text currentLevel_RP;
        public Transform rewardslotParent_RP;
        public BTButton nextLevel_RP;
        public BTButton prevLevel_RP;
        public BTButton enterRPDungeon;
        public BTButton perishRPDungeon;
        public GameObject cantEnterRPDungeon;
        public GameObject cantPerishRPDungeon;
        public ObtainGood RPdungeonTicketObtain;

        [Header("exp dungeon detail")]
        public GameObject expDetailWindow;
        public TMP_Text currentLevel_exp;
        public TMP_Text bestObtainExp;
        public Transform rewardslotParent_exp;
        public BTButton nextLevel_exp;
        public BTButton prevLevel_exp;
        public BTButton enterexpDungeon;
        public BTButton perishexpDungeon;
        public GameObject cantEnterExpDungeon;
        public GameObject cantPerishExpDungeon;
        public ObtainGood expdungeonTicketObtain;

        [Header("Awake dungeon detail")]
        public GameObject awakedetailWindow;
        public TMP_Text bestLeveldesc_awake;
        public Transform rewardslotParent_awake;
        public BTButton EnterDungeon_awake;
        public BTButton PerishDungeon_awake;
        public GameObject cantEnterAwakeDungeon;
        public GameObject cantPerishAwakeDungeon;
        public ObtainGood awakeObtain;

        [Header("RiftDungeon detail")]
        public GameObject riftDetailWindow;
        public TMP_Text bestLevel_rift;
        public TMP_Text currentLevel_rift;
        public Transform rewardslotParent_rift;
        public BTButton nextLevel_rift;
        public BTButton prevLevel_rift;
        public BTButton enterRiftDungeon;
        public BTButton perishRiftDungeon;
        public GameObject cantEnterRiftDungeon;
        public GameObject cantPerishRiftDungeon;
        public ObtainGood riftObtain;

        [Header("RuneDungeon detail")]
        public GameObject runeDetailWindow;
        public TMP_Text bestLevel_rune;
        public TMP_Text recentRewardText;
        public Transform runeslotParent_rune;
        public BTButton enterRuneDungeon;
        public GameObject cantEnterRuneDungeon;
        public ObtainGood runeObtain;

        //rewardTable
        public BTButton openRewardTableWindowBtn;
        public GameObject rewardTableWindow;
        public Transform rewardslotParentInTableWindow;
        public ViewStageRewardSlot chapterRewardSlotPrefab;
        public ViewGoodRewardSlot rewardslotPrefab;
        public BTButton[] closeBtnInRewardTable;

        public BTButton[] closeBtn;

        [Header("PerishRewardWindow")]
        public GameObject perishWindow;
        public Transform rewardSlotParentinPerish;
        public TMP_Text perishDesc;

        public BTButton[] perishWindowCloseBtn;

        public RectTransform arrowObject;
        private Coroutine _guidedArrowCo = null;

        Vector2 normalArrowPos = new Vector2(0, 220);
        Vector2 upPos = new Vector2(0, 230);
        Vector2 downPos = new Vector2(0, 210);

        public GameObject ExpLockedObj;
        public GameObject AwakeLockedObj;
        public GameObject RiftLockedObj;
        public GameObject RPLockedObj;
        public GameObject runeLockedObj;

        public BTButton[] goodsBtn;

        [Header("localizing")]
        public TMP_Text expTitle;
        public TMP_Text expDesc;
        public TMP_Text awakeTitle;
        public TMP_Text awakeDesc;
        public TMP_Text riftTitle;
        public TMP_Text riftDesc;
        public TMP_Text rpTitle;
        public TMP_Text rpDesc;
        public TMP_Text runeTitle;
        public TMP_Text runeDesc;

        public TMP_Text rpDungeonDetailTxt;
        public TMP_Text expDungeonDetailTxt;
        public TMP_Text awakeDungeonDetailTxt;
        public TMP_Text riftDungeonDetailTxt;
        public TMP_Text runeDungeonDetailTxt;

        public TMP_Text rpClearTxt;
        public TMP_Text rpEnterTxt;

        public TMP_Text expClearTxt;
        public TMP_Text expEnterTxt;

        public TMP_Text awakeClearTxt;
        public TMP_Text awakeEnterTxt;

        public TMP_Text riftClearTxt;
        public TMP_Text riftEnterTxt;

        public TMP_Text runeEnterTxt;
        public TMP_Text expDungeonDescText;
        public TMP_Text riftDungeonDescText;
        public TMP_Text rewardTableBtnTxt;
        public TMP_Text runeRewardWindowTitle;

        private void Awake()
        {
            Player.Guide.tutorialConfigAction += StartPoint;
            Player.Guide.TutorialEndcallback += EndTutorial;

            Player.Guide.questGuideAction += StartQuestPoint;
            Player.Guide.QuestGuideEndcallback += EndTutorial;

            arrowObject.gameObject.SetActive(false);
        }

        void StartQuestPoint(QuestGuideTypeConfigure questConfig)
        {
            if (IsQuestGuide(questConfig) == false)
            {
                arrowObject.gameObject.SetActive(false);
                if (_guidedArrowCo != null)
                    StopCoroutine(_guidedArrowCo);
                return;
            }

            arrowObject.gameObject.SetActive(false);
            SetArrowObjectPos_QuestGuide(questConfig);

            if (_guidedArrowCo != null)
                StopCoroutine(_guidedArrowCo);

            _guidedArrowCo = StartCoroutine(RoutineMoveArrow());
        }
        void EndTutorial()
        {
            if (_guidedArrowCo != null)
                StopCoroutine(_guidedArrowCo);
            arrowObject.gameObject.SetActive(false);
        }
        void StartPoint(Model.TutorialDescData tutoData)
        {
            if (tutoData.descKey != LocalizeDescKeys.None)
            {
                arrowObject.gameObject.SetActive(false);
                return;
            }
            if (IsPointTutorial(tutoData) == false)
            {
                arrowObject.gameObject.SetActive(false);
                return;
            }

            SetArrowObjectPos(tutoData);

            arrowObject.gameObject.SetActive(true);

            if (_guidedArrowCo != null)
                StopCoroutine(_guidedArrowCo);

            _guidedArrowCo = StartCoroutine(RoutineMoveArrow());
        }

        bool IsQuestGuide(QuestGuideTypeConfigure questGuideTypeConfigure)
        {
            bool isDialogTuto = false;
            if (questGuideTypeConfigure == QuestGuideTypeConfigure.RPDungeonClearPoint_1
                || questGuideTypeConfigure == QuestGuideTypeConfigure.ExpDungeonClearPoint_1)
            {
                isDialogTuto = true;
            }
            return isDialogTuto;
        }

        private void SetArrowObjectPos_QuestGuide(QuestGuideTypeConfigure questGuideTypeConfigure)
        {
            if (questGuideTypeConfigure == QuestGuideTypeConfigure.RPDungeonClearPoint_1)
            {
                arrowObject.transform.SetParent(EnterRPDungeon.transform, false);
                arrowObject.anchoredPosition = normalArrowPos;
            }
            if (questGuideTypeConfigure == QuestGuideTypeConfigure.ExpDungeonClearPoint_1)
            {
                arrowObject.transform.SetParent(EnterEXPDungeon.transform, false);
                arrowObject.anchoredPosition = normalArrowPos;
            }
        }


        bool IsPointTutorial(Model.TutorialDescData tutoData)
        {
            bool isDialogTuto = false;
            if (tutoData.tutoConfig == TutorialTypeConfigure.RPDungeonClearPoint_1
                || tutoData.tutoConfig == TutorialTypeConfigure.ExpDungeonClearPoint_1
                || tutoData.tutoConfig == TutorialTypeConfigure.AwakeDungeonClearPoint_1)
            {
                isDialogTuto = true;
            }

            return isDialogTuto;
        }

        private void SetArrowObjectPos(Model.TutorialDescData tutoData)
        {
            if (tutoData.tutoConfig == TutorialTypeConfigure.RPDungeonClearPoint_1)
            {
                arrowObject.transform.SetParent(EnterRPDungeon.transform, false);
                arrowObject.anchoredPosition = normalArrowPos;
            }
            if (tutoData.tutoConfig == TutorialTypeConfigure.ExpDungeonClearPoint_1)
            {
                arrowObject.transform.SetParent(EnterEXPDungeon.transform, false);
                arrowObject.anchoredPosition = normalArrowPos;
            }
            if (tutoData.tutoConfig == TutorialTypeConfigure.AwakeDungeonClearPoint_1)
            {
                arrowObject.transform.SetParent(EnterAwakeDungeon.transform, false);
                arrowObject.anchoredPosition = normalArrowPos;
            }
        }

        enum moveType
        {
            up,down
        }
        moveType movetype;
        float currenttime;
        private IEnumerator RoutineMoveArrow()
        {
            movetype = moveType.up;
            currenttime = 0;
            while (true)
            {
                switch (movetype)
                {
                    case moveType.up:
                        currenttime += Time.deltaTime * 2;
                        arrowObject.anchoredPosition = Vector2.Lerp(downPos, upPos, currenttime);
                        if(currenttime>=1)
                        {
                            movetype = moveType.down;
                            currenttime = 0;
                        }
                            break;
                    case moveType.down:
                        currenttime += Time.deltaTime * 2;
                        arrowObject.anchoredPosition = Vector2.Lerp(upPos, downPos,  currenttime);
                        if (currenttime >= 1)
                        {
                            movetype = moveType.up;
                            currenttime = 0;
                        }
                        break;
                    default:
                        break;
                }
                yield return null;
            }
        }
    }
}
