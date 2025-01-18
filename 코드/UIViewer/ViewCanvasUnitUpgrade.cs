using BlackTree.Core;
using BlackTree.Definition;
using BlackTree.Model;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTree.Bundles
{
    public class ViewCanvasUnitUpgrade : ViewCanvas
    {
        public BTSelector goldSelector;
        public BTSelector statSelector;
        public BTSelector awakeSelector;
        public BTButton GoldupBtn;
        public BTButton StatUpBtn;
        public BTButton awakeUpBtn;
        public GameObject goldRedDot;
        public GameObject statusRedDot;
        public GameObject awakeRedDot;

        public GameObject statLockObj;
        public GameObject awakeLockObj;

        public BTButton[] closeButton;

        public RectTransform arrowObject;
        private Coroutine _guidedArrowCo = null;

        Vector2 normalArrowPos = new Vector2(0, 44);
        Vector2 upPos = new Vector2(0, 54);
        Vector2 downPos = new Vector2(0, 34);

        public TMP_Text UpgradeTxt;
        public TMP_Text StatTxt;
        public TMP_Text awakeTxt;
        public TMP_Text UpgradeTxt_on;
        public TMP_Text StatTxt_on;
        public TMP_Text awakeTxt_on;
        private void Awake()
        {
            Player.Guide.tutorialConfigAction += StartPoint;
            Player.Guide.TutorialEndcallback += EndTutorial;

            Player.Guide.questGuideAction += StartQuestPoint;
            Player.Guide.QuestGuideEndcallback += EndTutorial;

            arrowObject.gameObject.SetActive(false);

            Player.Option.ContentUnlockUpdate += LockUpdate;

            UpgradeTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ETC_InitCommonUpgrade].StringToLocal;
            StatTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_StatusUpgrade].StringToLocal;
            awakeTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_AwakeUpgrade].StringToLocal;
            UpgradeTxt_on.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ETC_InitCommonUpgrade].StringToLocal;
            StatTxt_on.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_StatusUpgrade].StringToLocal;
            awakeTxt_on.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_AwakeUpgrade].StringToLocal;
            LockUpdate();
        }

        void LockUpdate()
        {
            int statUnlockLv = StaticData.Wrapper.ingameLockData[(int)LockedUIType.StatusQuestUnlock].unLockLevel;
            int awakeUnlockLv = StaticData.Wrapper.ingameLockData[(int)LockedUIType.AwakeUpgradeQuestUnlock].unLockLevel;
            if (Player.Quest.mainQuestCurrentId >= statUnlockLv)
            {
                statLockObj.SetActive(false);
                StatUpBtn.enabled = true;
            }
            else
            {
                statLockObj.SetActive(true);
                StatUpBtn.enabled = false;
            }

            if (Player.Quest.mainQuestCurrentId >= awakeUnlockLv)
            {
                awakeLockObj.SetActive(false);
                awakeUpBtn.enabled = true;
            }
            else
            {
                awakeLockObj.SetActive(true);
                awakeUpBtn.enabled = false;
            }
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

            _guidedArrowCo = StartCoroutine(RoutineMoveArrow_Quest(questConfig));
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


            if (_guidedArrowCo != null)
                StopCoroutine(_guidedArrowCo);

            _guidedArrowCo = StartCoroutine(RoutineMoveArrow(tutoData));
        }

        bool IsQuestGuide(QuestGuideTypeConfigure questGuideTypeConfigure)
        {
            bool isDialogTuto = false;
            if (questGuideTypeConfigure == QuestGuideTypeConfigure.GoldUpgradePoint_2
                || questGuideTypeConfigure == QuestGuideTypeConfigure.StatusUpgradePoint_2)
            {
                isDialogTuto = true;
            }
            return isDialogTuto;
        }

        private void SetArrowObjectPos_QuestGuide(QuestGuideTypeConfigure questGuideTypeConfigure)
        {
            if (questGuideTypeConfigure == QuestGuideTypeConfigure.GoldUpgradePoint_2)
            {
                arrowObject.transform.SetParent(GoldupBtn.transform, false);
                arrowObject.anchoredPosition = normalArrowPos;
            }
            if (questGuideTypeConfigure == QuestGuideTypeConfigure.StatusUpgradePoint_2)
            {
                arrowObject.transform.SetParent(StatUpBtn.transform, false);
                arrowObject.anchoredPosition = normalArrowPos;
            }
        }


        bool IsPointTutorial(Model.TutorialDescData tutoData)
        {
            bool isDialogTuto = false;
            if (tutoData.tutoConfig == TutorialTypeConfigure.GoldUpgradePoint_2
                || tutoData.tutoConfig == TutorialTypeConfigure.StatusUpgradePoint_2)
            {
                isDialogTuto = true;
            }

            return isDialogTuto;
        }

        private void SetArrowObjectPos(Model.TutorialDescData tutoData)
        {
            if (tutoData.tutoConfig == TutorialTypeConfigure.GoldUpgradePoint_2)
            {
                arrowObject.transform.SetParent(GoldupBtn.transform, false);
                arrowObject.anchoredPosition = normalArrowPos;
            }
            if (tutoData.tutoConfig == TutorialTypeConfigure.StatusUpgradePoint_2)
            {
                arrowObject.transform.SetParent(StatUpBtn.transform, false);
                arrowObject.anchoredPosition = normalArrowPos;
            }
            if (tutoData.tutoConfig == TutorialTypeConfigure.AwakeUpgradePointAwakeBtn_2)
            {
                arrowObject.transform.SetParent(awakeUpBtn.transform, false);
                arrowObject.anchoredPosition = normalArrowPos;
            }
        }

        enum moveType
        {
            up, down
        }
        moveType movetype;
        float currenttime;
        private IEnumerator RoutineMoveArrow(TutorialDescData tutoData)
        {
            yield return null;

            //이미 캔버스가 띄워져 있으면 화살표를 안띄워줘도 되기때문에 
            if (tutoData.tutoConfig == TutorialTypeConfigure.GoldUpgradePoint_2)
            {
                if (ViewCanvas.Get<ViewCanvasGoldUpgrade>().IsVisible)
                {
                    Player.Guide.StartTutorial(TutorialType.GoldUpgrade);
                    yield break;
                }
            }
            if (tutoData.tutoConfig == TutorialTypeConfigure.StatusUpgradePoint_2)
            {
                if (ViewCanvas.Get<ViewCanvasStatusUpgrade>().IsVisible)
                {
                    Player.Guide.StartTutorial(TutorialType.StatusUpgrade);
                    yield break;
                }
            }
            if (tutoData.tutoConfig == TutorialTypeConfigure.AwakeUpgradePointAwakeBtn_2)
            {
                if (ViewCanvas.Get<ViewCanvasStatusUpgrade>().IsVisible)
                {
                    Player.Guide.StartTutorial(TutorialType.AwakeUpgrade);
                    yield break;
                }
            }

            arrowObject.gameObject.SetActive(true);
            movetype = moveType.up;
            currenttime = 0;
            while (true)
            {
                switch (movetype)
                {
                    case moveType.up:
                        currenttime += Time.deltaTime * 2;
                        arrowObject.anchoredPosition = Vector2.Lerp(downPos, upPos, currenttime);
                        if (currenttime >= 1)
                        {
                            movetype = moveType.down;
                            currenttime = 0;
                        }
                        break;
                    case moveType.down:
                        currenttime += Time.deltaTime * 2;
                        arrowObject.anchoredPosition = Vector2.Lerp(upPos, downPos, currenttime);
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

        private IEnumerator RoutineMoveArrow_Quest(QuestGuideTypeConfigure questguideConfig)
        {
            yield return null;

            if (questguideConfig==QuestGuideTypeConfigure.GoldUpgradePoint_2)
            {
                if (ViewCanvas.Get<ViewCanvasGoldUpgrade>().IsVisible)
                {
                    Player.Guide.QuestGuideProgress(QuestGuideType.GoldUpgrade);
                    yield break;
                }
            }
            if (questguideConfig == QuestGuideTypeConfigure.StatusUpgradePoint_2)
            {
                if (ViewCanvas.Get<ViewCanvasStatusUpgrade>().IsVisible)
                {
                    Player.Guide.QuestGuideProgress(QuestGuideType.StatUpgrade);
                    yield break;
                }
            }

            arrowObject.gameObject.SetActive(true);
            movetype = moveType.up;
            currenttime = 0;
            while (true)
            {
                switch (movetype)
                {
                    case moveType.up:
                        currenttime += Time.deltaTime * 2;
                        arrowObject.anchoredPosition = Vector2.Lerp(downPos, upPos, currenttime);
                        if (currenttime >= 1)
                        {
                            movetype = moveType.down;
                            currenttime = 0;
                        }
                        break;
                    case moveType.down:
                        currenttime += Time.deltaTime * 2;
                        arrowObject.anchoredPosition = Vector2.Lerp(upPos, downPos, currenttime);
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
