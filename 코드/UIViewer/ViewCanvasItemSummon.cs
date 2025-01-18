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
    public enum SummonType
    {
        Item,Skill,Pet,Rune
    }
    public class ViewCanvasItemSummon : ViewCanvas
    {
        public GameObject[] spawnWindows;
        public BTButton[] buttonTabs;
        public BTSelector selectorPanels;

        public ItemSummonSlot weaponSummonslots;
        public ItemSummonSlot skillSummonslots;
        public ItemSummonSlot petSummonslots;
        public ItemSummonSlot runeSummonslots;

        [Header("°¡Ã­ °á°ú ÆË¾÷")]
        public GameObject GachaResultPopup;
        public ViewGachaSlot gachaSlot;
        public ViewGachaSlot gachaSlot_forPet;
        public ScrollRect gachaContent;
        public BTButton[] closeGachaBtn;

        public TMP_Text textIn10summonBtninResult;
        public TMP_Text textIn100summonBtninResult;
        public BTButton summon10BtnInGacharesult;
        public BTButton summon100BtnInGacharesult;

        public BTButton rateTableOpenBtn;
        public GameObject rateTableWindow;
        public SummonRateTableSlot[] ratetableList;
        public BTButton rateTableOffBtn;
        public BTButton nextSummonLevelBtn;
        public BTButton prevSummonLevelBtn;
        public TMP_Text currentSummonLevelInTablewindow;

        public BTButton[] closeBtn;

        public GameObject skillLocked;
        public GameObject petLocked;
        public GameObject runeLocked;

        public RectTransform arrowObject;
        private Coroutine _guidedArrowCo = null;

        public Toggle autoSpawnToggle;
        public GameObject autoSpawnCheckObj;

        Vector2 normalArrowPos = new Vector2(0, 44);
        Vector2 upPos = new Vector2(0, 54);
        Vector2 downPos = new Vector2(0, 34);

        public int normalOrder = 190;

        public TMP_Text equipitemTxt;
        public TMP_Text skillTxt;
        public TMP_Text petTxt;
        public TMP_Text runeTxt;
        public TMP_Text equipitemTxt_on;
        public TMP_Text skillTxt_on;
        public TMP_Text petTxt_on;
        public TMP_Text runeTxt_on;

        public TMP_Text equipSummonTxt;
        public TMP_Text skillSummonTxt;
        public TMP_Text petSummonTxt;
        public TMP_Text runeSummonTxt;
        public TMP_Text rateTableBtnTxt;
        public TMP_Text autoSummon;
        public TMP_Text[] adbtns_20;
        public TMP_Text[] summonbtns_10;
        public TMP_Text[] summonbtns_100;

        public TMP_Text rewardTitle;

        private void Awake()
        {
            Player.Guide.tutorialConfigAction += StartPoint;
            Player.Guide.TutorialEndcallback += EndTutorial;

            Player.Guide.questGuideAction += StartQuestPoint;
            Player.Guide.QuestGuideEndcallback += EndTutorial;

            arrowObject.gameObject.SetActive(false);

            string summon= StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Summon].StringToLocal;
            equipitemTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Equipment].StringToLocal;
            skillTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Skill].StringToLocal;
            petTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Pet].StringToLocal;
            runeTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Rune].StringToLocal;
            equipitemTxt_on.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Equipment].StringToLocal;
            skillTxt_on.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Skill].StringToLocal;
            petTxt_on.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Pet].StringToLocal;
            runeTxt_on.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Rune].StringToLocal;


            equipSummonTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Equipment].StringToLocal+ summon;
            skillSummonTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Skill].StringToLocal + summon;
            petSummonTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Pet].StringToLocal + summon;
            runeSummonTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Rune].StringToLocal + summon;
            rateTableBtnTxt.text= StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Rate].StringToLocal;
            autoSummon.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_AutoSummon].StringToLocal;

            rewardTitle.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Rate].StringToLocal;
            for (int i=0;i< adbtns_20.Length; i++)
            {
                adbtns_20[i].text=string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_ValueCount].StringToLocal,20);
            }
            for (int i = 0; i < summonbtns_10.Length; i++)
            {
                summonbtns_10[i].text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_ValueCount].StringToLocal, 10);
            }
            for (int i = 0; i < summonbtns_100.Length; i++)
            {
                summonbtns_100[i].text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_ValueCount].StringToLocal, 100);
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

            if (questConfig == QuestGuideTypeConfigure.SummonEquip_1)
            {
                buttonTabs[0].onClick?.Invoke();
                Player.Guide.QuestGuideProgress(QuestGuideType.SummonEquip);
            }
            if (questConfig == QuestGuideTypeConfigure.SummonSkill_1)
            {
                buttonTabs[1].onClick?.Invoke();
             
                Player.Guide.QuestGuideProgress(QuestGuideType.SummonSkill);
            }
            if (questConfig == QuestGuideTypeConfigure.SummonPet_1)
            {
                buttonTabs[2].onClick?.Invoke();
        
                Player.Guide.QuestGuideProgress(QuestGuideType.SummonPet);
            }
            //SetArrowObjectPos_QuestGuide(questConfig);

            //if (_guidedArrowCo != null)
            //    StopCoroutine(_guidedArrowCo);

            //_guidedArrowCo = StartCoroutine(RoutineMoveArrow_quest(questConfig));
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
            if (questGuideTypeConfigure == QuestGuideTypeConfigure.SummonEquip_1
                || questGuideTypeConfigure == QuestGuideTypeConfigure.SummonSkill_1
                || questGuideTypeConfigure == QuestGuideTypeConfigure.SummonPet_1)
            {
                isDialogTuto = true;
            }
            return isDialogTuto;
        }

        private void SetArrowObjectPos_QuestGuide(QuestGuideTypeConfigure questGuideTypeConfigure)
        {
            if (questGuideTypeConfigure == QuestGuideTypeConfigure.SummonEquip_1)
            {
                arrowObject.transform.SetParent(buttonTabs[0].transform, false);
                arrowObject.anchoredPosition = normalArrowPos;
            }
            if (questGuideTypeConfigure == QuestGuideTypeConfigure.SummonSkill_1)
            {
                arrowObject.transform.SetParent(buttonTabs[1].transform, false);
                arrowObject.anchoredPosition = normalArrowPos;
            }
            if (questGuideTypeConfigure == QuestGuideTypeConfigure.SummonPet_2)
            {
                arrowObject.transform.SetParent(buttonTabs[2].transform, false);
                arrowObject.anchoredPosition = normalArrowPos;
            }
        }

        bool IsPointTutorial(Model.TutorialDescData tutoData)
        {
            bool isPointTuto = false;
            if (tutoData.tutoConfig == TutorialTypeConfigure.SummonEquipPoint_2
                || tutoData.tutoConfig == TutorialTypeConfigure.SummonSkillPoint_2
                || tutoData.tutoConfig == TutorialTypeConfigure.SummonPetPoint_2)
            {
                isPointTuto = true;
            }

            return isPointTuto;
        }

        private void SetArrowObjectPos(Model.TutorialDescData tutoData)
        {
            if (tutoData.tutoConfig == TutorialTypeConfigure.SummonEquipPoint_2)
            {
                arrowObject.transform.SetParent(buttonTabs[0].transform, false);
                arrowObject.anchoredPosition = normalArrowPos;
            }
            if (tutoData.tutoConfig == TutorialTypeConfigure.SummonSkillPoint_2)
            {
                arrowObject.transform.SetParent(buttonTabs[1].transform, false);
                arrowObject.anchoredPosition = normalArrowPos;
            }

            if (tutoData.tutoConfig == TutorialTypeConfigure.SummonPetPoint_2)
            {
                arrowObject.transform.SetParent(buttonTabs[2].transform, false);
                arrowObject.anchoredPosition = normalArrowPos;
            }
        }

        enum moveType
        {
            up, down
        }
        moveType movetype;
        float currenttime;
        private IEnumerator RoutineMoveArrow(Model.TutorialDescData tutoData)
        {
            yield return null;

            if (tutoData.tutoConfig == TutorialTypeConfigure.SummonEquipPoint_2)
            {
                if (spawnWindows[0].gameObject.activeInHierarchy)
                {
                    Player.Guide.StartTutorial(TutorialType.SummonEquip);
                    yield break;
                }
            }
            if (tutoData.tutoConfig == TutorialTypeConfigure.SummonSkillPoint_2)
            {
                if (spawnWindows[1].gameObject.activeInHierarchy)
                {
                    Player.Guide.StartTutorial(TutorialType.SummonSkill);
                    yield break;
                }
            }
            if (tutoData.tutoConfig == TutorialTypeConfigure.SummonPetPoint_2)
            {
                if (spawnWindows[1].gameObject.activeInHierarchy)
                {
                    Player.Guide.StartTutorial(TutorialType.SummonPet);
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

        private IEnumerator RoutineMoveArrow_quest(QuestGuideTypeConfigure questguideConfig)
        {
            yield return null;

            if (questguideConfig == QuestGuideTypeConfigure.SummonEquip_1)
            {
                if (spawnWindows[0].gameObject.activeInHierarchy)
                {
                    Player.Guide.QuestGuideProgress(QuestGuideType.SummonEquip);
                    yield break;
                }
            }
            if (questguideConfig == QuestGuideTypeConfigure.SummonSkill_1)
            {
                if (spawnWindows[1].gameObject.activeInHierarchy)
                {
                    Player.Guide.QuestGuideProgress(QuestGuideType.SummonSkill);
                    yield break;
                }
            }
            if (questguideConfig == QuestGuideTypeConfigure.SummonPet_2)
            {
                if (spawnWindows[2].gameObject.activeInHierarchy)
                {
                    Player.Guide.QuestGuideProgress(QuestGuideType.SummonPet);
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

        public void Init()
        {
            weaponSummonslots.SyncView(Player.Cloud.weapondata.summonLevel, Player.Cloud.weapondata.currentSummonExp, SummonType.Item);
            skillSummonslots.SyncView(Player.Cloud.skilldata.summonLevel, Player.Cloud.skilldata.currentSummonExp, SummonType.Skill);
            petSummonslots.SyncView(Player.Cloud.petdata.summonLevel, Player.Cloud.petdata.currentSummonExp, SummonType.Pet);
            runeSummonslots.SyncView(Player.Cloud.runeData.summonLevel, Player.Cloud.runeData.currentSummonExp, SummonType.Rune);
        }


        public void Show(int index)
        {
            Player.Summon.currentPopupIndex = index;

            int summonPetLv = StaticData.Wrapper.ingameLockData[(int)LockedUIType.PetSummonUnlock].unLockLevel;
            int summonSkillLv = StaticData.Wrapper.ingameLockData[(int)LockedUIType.SkillSummonUnlock].unLockLevel;
            int summonRuneLv = StaticData.Wrapper.ingameLockData[(int)LockedUIType.RuneSummonUnlock].unLockLevel;

            if (Player.Quest.mainQuestCurrentId >= summonPetLv)
            {
                for (int i = 0; i < spawnWindows.Length; i++)
                {
                    spawnWindows[i].SetActive(index == i);
                }
                selectorPanels.Show(index);
            }
            else
            {
                LocalizeDescKeys localizedKey = LocalizeDescKeys.Etc_UnlockAfterMission;
                string localizedvalue = string.Format(StaticData.Wrapper.localizeddesclist[(int)localizedKey].StringToLocal, summonPetLv);
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
            }
            if (Player.Quest.mainQuestCurrentId >= summonSkillLv)
            {
                for (int i = 0; i < spawnWindows.Length; i++)
                {
                    spawnWindows[i].SetActive(index == i);
                }
                selectorPanels.Show(index);
            }
            else
            {
                LocalizeDescKeys localizedKey = LocalizeDescKeys.Etc_UnlockAfterMission;
                string localizedvalue = string.Format(StaticData.Wrapper.localizeddesclist[(int)localizedKey].StringToLocal, summonSkillLv);
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
            }
            if (Player.Cloud.userLevelData.currentLevel >= summonRuneLv)
            {
                for (int i = 0; i < spawnWindows.Length; i++)
                {
                    spawnWindows[i].SetActive(index == i);
                }
                selectorPanels.Show(index);
            }
            else
            {
                LocalizeDescKeys localizedKey = LocalizeDescKeys.Etc_UnlockAfterLevel;
                string localizedvalue = string.Format(StaticData.Wrapper.localizeddesclist[(int)localizedKey].StringToLocal, summonRuneLv);
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
            }
          


      
        }

        public void SyncItemLevel()
        {
            weaponSummonslots.SyncView(Player.Cloud.weapondata.summonLevel, Player.Cloud.weapondata.currentSummonExp, SummonType.Item);
            skillSummonslots.SyncView(Player.Cloud.skilldata.summonLevel, Player.Cloud.skilldata.currentSummonExp, SummonType.Skill);
            petSummonslots.SyncView(Player.Cloud.petdata.summonLevel, Player.Cloud.petdata.currentSummonExp, SummonType.Pet);
            runeSummonslots.SyncView(Player.Cloud.runeData.summonLevel, Player.Cloud.runeData.currentSummonExp, SummonType.Rune);
        }
    }

}
