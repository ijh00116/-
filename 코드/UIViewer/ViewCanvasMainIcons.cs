using BlackTree.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BlackTree.Model;
using BlackTree.Definition;
using System.Collections;
using System.Collections.Generic;

namespace BlackTree.Bundles
{
    public class ViewCanvasMainIcons : ViewCanvas
    {
        public BTButton adBuffButton;

        public GameObject[] buffInactiveObj;

        public BTButton goToClickerDungeon;
        public BTButton goToRaidDungeon;
        public BTButton goResearchWindowBtn;
        public GameObject researchReddotObj;
        public BTButton vipWindowOpenBtn;
        public BTButton freeShopOpenBtn;
        public Image vipIcon;
        public BTButton battlepassBtn;


        public RectTransform arrowObject;
        private Coroutine _guidedArrowCo = null;
        Vector2 leftPos = new Vector2(55, 0);
        Vector2 rightPos = new Vector2(95, 0);

        Vector2 arrowObjectPos = new Vector2(75,0);

        [Header("DmgList")]
        public Transform dmgGraphParent;
        public ViewDmgSlot dmgSlotPrefab;
        public BTButton refresh;
        public TMP_Text timeText;

        public TMP_Text BuffIconTxt;
        public TMP_Text researchIconTxt;
        public TMP_Text EventIconTxt;
        public TMP_Text BattlePassIconTxt;
        public TMP_Text RaidIconTxt;


        private void Awake()
        {
            Player.Guide.tutorialConfigAction += StartPoint;
            Player.Guide.TutorialEndcallback += EndTutorial;

            arrowObject.gameObject.SetActive(false);

            Model.Player.Option.ContentUnlockUpdate += MainIconUpdate;

            BuffIconTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_AdBuff].StringToLocal;
            researchIconTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Research].StringToLocal;
            EventIconTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_FreeShop].StringToLocal;
            BattlePassIconTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_BattlePass].StringToLocal;
            RaidIconTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Raid].StringToLocal;

            MainIconUpdate();
        }

        public void MainIconUpdate()
        {
            int adIcon = StaticData.Wrapper.ingameLockData[(int)LockedUIType.UnlockAdBuff].unLockLevel;
            int researchIcon= StaticData.Wrapper.ingameLockData[(int)LockedUIType.UnlockResearch].unLockLevel;
            if (Player.Quest.mainQuestCurrentId >= adIcon)
            {
                adBuffButton.gameObject.SetActive(true);
            }
            else
            {
                adBuffButton.gameObject.SetActive(false);
            }

            if (Player.Quest.mainQuestCurrentId >= researchIcon)
            {
                goResearchWindowBtn.gameObject.SetActive(true);
            }
            else
            {
                goResearchWindowBtn.gameObject.SetActive(false);
            }

            bool isUnlock = Model.Player.Option.IsContentUIUnlocked(Definition.LockedUIType.MainNavInAppPurchaseIcon);
            battlepassBtn.gameObject.SetActive(isUnlock);
        }
     
        void StartPoint(Model.TutorialDescData tutoData)
        {
            if (tutoData.descKey != LocalizeDescKeys.None)
            {
                arrowObject.gameObject.SetActive(false);
                if (_guidedArrowCo != null)
                    StopCoroutine(_guidedArrowCo);
                return;
            }
            if (IsPointTutorial(tutoData) == false)
            {
                arrowObject.gameObject.SetActive(false);
                if (_guidedArrowCo != null)
                    StopCoroutine(_guidedArrowCo);
                return;
            }

            arrowObject.gameObject.SetActive(true);
            SetArrowObjectPos(tutoData);

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

        bool IsPointTutorial(Model.TutorialDescData tutoData)
        {
            bool isDialogTuto = false;
            if (tutoData.tutoConfig == TutorialTypeConfigure.AdBuffPoint_1||
                tutoData.tutoConfig == TutorialTypeConfigure.RPContentClearPoint_1)
            {
                isDialogTuto = true;
            }
            return isDialogTuto;
        }

        private void SetArrowObjectPos(Model.TutorialDescData tutoData)
        {
            if (tutoData.tutoConfig == TutorialTypeConfigure.AdBuffPoint_1)
            {
                arrowObject.transform.SetParent(adBuffButton.transform, false);
                arrowObject.anchoredPosition = arrowObjectPos;
            }
            if (tutoData.tutoConfig == TutorialTypeConfigure.RaidStartPoint_1)
            {
                arrowObject.transform.SetParent(goToRaidDungeon.transform, false);
                arrowObject.anchoredPosition = arrowObjectPos;
            }
            if (tutoData.tutoConfig == TutorialTypeConfigure.RPContentClearPoint_1)
            {
                arrowObject.transform.SetParent(goResearchWindowBtn.transform, false);
                arrowObject.anchoredPosition = arrowObjectPos;
            }
        }

        enum moveType
        {
            up, down
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
                        arrowObject.anchoredPosition = Vector2.Lerp(leftPos, rightPos, currenttime);
                        if (currenttime >= 1)
                        {
                            movetype = moveType.down;
                            currenttime = 0;
                        }
                        break;
                    case moveType.down:
                        currenttime += Time.deltaTime * 2;
                        arrowObject.anchoredPosition = Vector2.Lerp(rightPos, leftPos, currenttime);
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
