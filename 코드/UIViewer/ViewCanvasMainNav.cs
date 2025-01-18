using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Model;
using BlackTree.Definition;
using BlackTree.Core;

namespace BlackTree.Bundles
{
    public class ViewCanvasMainNav : ViewCanvas
    {
        public ViewMainNavButton[] viewMainNavButtons;

        public RectTransform arrowTotalObject;
        public RectTransform arrowObject;
        private Coroutine _guidedArrowCo = null;
        Vector2 upPos = new Vector2(0, 50);
        Vector2 downPos = new Vector2(0, 0);

        Vector2 arrowObjectPos = new Vector2(0, 140);
        private void Awake()
        {
            Player.Guide.tutorialConfigAction += StartPoint;
            Player.Guide.TutorialEndcallback += EndTutorial;

            Player.Guide.questGuideAction += StartQuestPoint;
            Player.Guide.QuestGuideEndcallback += EndTutorial;

            arrowTotalObject.gameObject.SetActive(false);
        }

        void StartQuestPoint(QuestGuideTypeConfigure questConfig)
        {
            if (IsQuestGuide(questConfig) == false)
            {
                arrowTotalObject.gameObject.SetActive(false);
                if (_guidedArrowCo != null)
                    StopCoroutine(_guidedArrowCo);
                return;
            }

            SetArrowObjectPos_QuestGuide(questConfig);
        }

        void EndTutorial()
        {
            if (_guidedArrowCo != null)
                StopCoroutine(_guidedArrowCo);
            arrowTotalObject.gameObject.SetActive(false);
        }
        void StartPoint(Model.TutorialDescData tutoData)
        {
            if (tutoData.descKey != LocalizeDescKeys.None)
            {
                arrowTotalObject.gameObject.SetActive(false);
                if (_guidedArrowCo != null)
                    StopCoroutine(_guidedArrowCo);
                return;
            }
            if(IsPointTutorial(tutoData)==false)
            {
                arrowTotalObject.gameObject.SetActive(false);
                if (_guidedArrowCo != null)
                    StopCoroutine(_guidedArrowCo);
                return;
            }


            arrowTotalObject.gameObject.SetActive(true);
            SetArrowObjectPos(tutoData);

            if (_guidedArrowCo != null)
                StopCoroutine(_guidedArrowCo);

            _guidedArrowCo = StartCoroutine(RoutineMoveArrow());
        }

        bool IsQuestGuide(QuestGuideTypeConfigure questGuideTypeConfigure)
        {
            bool isDialogTuto = false;
            if (questGuideTypeConfigure==QuestGuideTypeConfigure.RPDungeonClearPoint_0
                || questGuideTypeConfigure == QuestGuideTypeConfigure.ExpDungeonClearPoint_0
                || questGuideTypeConfigure == QuestGuideTypeConfigure.GoldUpgradePoint_1
                || questGuideTypeConfigure == QuestGuideTypeConfigure.StatusUpgradePoint_1
                || questGuideTypeConfigure == QuestGuideTypeConfigure.SummonEquip_0
                || questGuideTypeConfigure == QuestGuideTypeConfigure.SummonSkill_0
                || questGuideTypeConfigure == QuestGuideTypeConfigure.SummonPet_1
                || questGuideTypeConfigure == QuestGuideTypeConfigure.EquipEquip_0
                || questGuideTypeConfigure == QuestGuideTypeConfigure.EquipSkill_0
                || questGuideTypeConfigure == QuestGuideTypeConfigure.EquipPet_0
                || questGuideTypeConfigure == QuestGuideTypeConfigure.OpenProfileDetail_0
                || questGuideTypeConfigure == QuestGuideTypeConfigure.StageReward_0
                || questGuideTypeConfigure == QuestGuideTypeConfigure.QuestComplete_0)
            {
                isDialogTuto = true;
            }
            return isDialogTuto;
        }

        private void SetArrowObjectPos_QuestGuide(QuestGuideTypeConfigure questGuideTypeConfigure)
        {
            if (questGuideTypeConfigure == QuestGuideTypeConfigure.RPDungeonClearPoint_0
                || questGuideTypeConfigure == QuestGuideTypeConfigure.ExpDungeonClearPoint_0)
            {
                if (viewMainNavButtons[ControllerContents._index].isUnlocked == false)
                {
                    PopupToastMessageBtnLocked(ControllerContents._index);
                }
                else
                {
                    if(ViewCanvas.Get<ViewCanvasContents>().IsVisible==false)
                        MainNav.SetTabIndex(ControllerContents._index);
                }
                //arrowObject.transform.SetParent(viewMainNavButtons[Core.ControllerContents._index].transform, false);
                //arrowObject.anchoredPosition = arrowObjectPos;
            }
            if (questGuideTypeConfigure == QuestGuideTypeConfigure.GoldUpgradePoint_1
               || questGuideTypeConfigure == QuestGuideTypeConfigure.StatusUpgradePoint_1)
            {
                if (viewMainNavButtons[ControllerUnitUpgrade._index].isUnlocked == false)
                {
                    PopupToastMessageBtnLocked(ControllerUnitUpgrade._index);
                }
                else
                {
                    if (ViewCanvas.Get<ViewCanvasUnitUpgrade>().IsVisible == false)
                        MainNav.SetTabIndex(ControllerUnitUpgrade._index);
                }
                //arrowObject.transform.SetParent(viewMainNavButtons[Core.ControllerUnitUpgrade._index].transform, false);
                //arrowObject.anchoredPosition = arrowObjectPos;
            }
            if (questGuideTypeConfigure == QuestGuideTypeConfigure.SummonEquip_0
               || questGuideTypeConfigure == QuestGuideTypeConfigure.SummonSkill_0
               || questGuideTypeConfigure == QuestGuideTypeConfigure.SummonPet_1)
            {
                if (viewMainNavButtons[ControllerItemSummon._index].isUnlocked == false)
                {
                    PopupToastMessageBtnLocked(ControllerItemSummon._index);
                }
                else
                {
                    if (ViewCanvas.Get<ViewCanvasItemSummon>().IsVisible == false)
                    {
                        if (questGuideTypeConfigure == QuestGuideTypeConfigure.SummonEquip_0)
                        {
                            ViewCanvas.Get<ViewCanvasItemSummon>().buttonTabs[0].onClick?.Invoke();
                            Player.Guide.QuestGuideProgress(QuestGuideType.SummonEquip);
                        }
                        if (questGuideTypeConfigure == QuestGuideTypeConfigure.SummonSkill_0)
                        {
                            ViewCanvas.Get<ViewCanvasItemSummon>().buttonTabs[1].onClick?.Invoke();

                            Player.Guide.QuestGuideProgress(QuestGuideType.SummonSkill);
                        }
                        if (questGuideTypeConfigure == QuestGuideTypeConfigure.SummonPet_1)
                        {
                            ViewCanvas.Get<ViewCanvasItemSummon>().buttonTabs[2].onClick?.Invoke();

                            Player.Guide.QuestGuideProgress(QuestGuideType.SummonPet);
                        }
                        MainNav.SetTabIndex(ControllerItemSummon._index);
                    }
                    else
                    {
                        if (questGuideTypeConfigure == QuestGuideTypeConfigure.SummonEquip_0)
                        {
                            ViewCanvas.Get<ViewCanvasItemSummon>().buttonTabs[0].onClick?.Invoke();
                            Player.Guide.QuestGuideProgress(QuestGuideType.SummonEquip);
                        }
                        if (questGuideTypeConfigure == QuestGuideTypeConfigure.SummonSkill_0)
                        {
                            ViewCanvas.Get<ViewCanvasItemSummon>().buttonTabs[1].onClick?.Invoke();

                            Player.Guide.QuestGuideProgress(QuestGuideType.SummonSkill);
                        }
                        if (questGuideTypeConfigure == QuestGuideTypeConfigure.SummonPet_1)
                        {
                            ViewCanvas.Get<ViewCanvasItemSummon>().buttonTabs[2].onClick?.Invoke();

                            Player.Guide.QuestGuideProgress(QuestGuideType.SummonPet);
                        }
                    }
                }
                //arrowObject.transform.SetParent(viewMainNavButtons[Core.ControllerItemSummon._index].transform, false);
                //arrowObject.anchoredPosition = arrowObjectPos;
            }

            if (questGuideTypeConfigure == QuestGuideTypeConfigure.EquipEquip_0)
            {
                if (viewMainNavButtons[ControllerEquipInventory._index].isUnlocked == false)
                {
                    PopupToastMessageBtnLocked(ControllerEquipInventory._index);
                }
                else
                {
                    if (ViewCanvas.Get<ViewCanvasEquipInventory>().IsVisible == false)
                        MainNav.SetTabIndex(ControllerEquipInventory._index);
                }
            }

            if (questGuideTypeConfigure == QuestGuideTypeConfigure.EquipSkill_0)
            {
                if (viewMainNavButtons[ControllerSkillInventory._index].isUnlocked == false)
                {
                    PopupToastMessageBtnLocked(ControllerSkillInventory._index);
                }
                else
                {
                    if (ViewCanvas.Get<ViewCanvasSkillInventory>().IsVisible == false)
                        MainNav.SetTabIndex(ControllerSkillInventory._index);
                }
            }

            if (questGuideTypeConfigure == QuestGuideTypeConfigure.EquipPet_0)
            {
                if (viewMainNavButtons[ControllerPetInventory._index].isUnlocked == false)
                {
                    PopupToastMessageBtnLocked(ControllerPetInventory._index);
                }
                else
                {
                    if (ViewCanvas.Get<ViewCanvasPetInventory>().IsVisible == false)
                        MainNav.SetTabIndex(ControllerPetInventory._index);
                }
            }
            if (questGuideTypeConfigure == QuestGuideTypeConfigure.EquipPet_0)
            {
                if (viewMainNavButtons[ControllerPetInventory._index].isUnlocked == false)
                {
                    PopupToastMessageBtnLocked(ControllerPetInventory._index);
                }
                else
                {
                    if (ViewCanvas.Get<ViewCanvasPetInventory>().IsVisible == false)
                        MainNav.SetTabIndex(ControllerPetInventory._index);
                }
            }

            if (questGuideTypeConfigure == QuestGuideTypeConfigure.StageReward_0)
            {
                if (ViewCanvas.Get<ViewCanvasStage>().IsVisible == false)
                {
                    Bundles.ViewCanvas.Get<Bundles.ViewCanvasStage>().SetVisible(true);
                    Bundles.ViewCanvas.Get<Bundles.ViewCanvasStage>().blackBG.PopupOpenColorFade();
                    Bundles.ViewCanvas.Get<Bundles.ViewCanvasStage>().Wrapped.CommonPopupOpenAnimationDown();
                }
                    
            }

            if (questGuideTypeConfigure == QuestGuideTypeConfigure.QuestComplete_0)
            {
                if (ViewCanvas.Get<ViewCanvasQuest>().IsVisible == false)
                {
                    Bundles.ViewCanvas.Get<Bundles.ViewCanvasQuest>().SetVisible(true);
                    Bundles.ViewCanvas.Get<Bundles.ViewCanvasQuest>().blackBG.PopupOpenColorFade();
                    Bundles.ViewCanvas.Get<Bundles.ViewCanvasQuest>().Wrapped.CommonPopupOpenAnimationDown();
                }

            }

            if (questGuideTypeConfigure == QuestGuideTypeConfigure.OpenProfileDetail_0)
            {
                if (ViewCanvas.Get<ViewCanvasProfile>().IsVisible == false)
                {
                    Bundles.ViewCanvas.Get<Bundles.ViewCanvasProfile>().SetVisible(true);
                    Bundles.ViewCanvas.Get<Bundles.ViewCanvasProfile>().blackBG.PopupOpenColorFade();
                    Bundles.ViewCanvas.Get<Bundles.ViewCanvasProfile>().Wrapped.CommonPopupOpenAnimationDown(()=> {
                        if (Player.Guide.currentGuideQuest == QuestGuideType.OpenProfileDetail)
                        {
                            Player.Guide.QuestGuideProgress(Player.Guide.currentGuideQuest);
                        }
                    });
                }

            }
        }


        bool IsPointTutorial(Model.TutorialDescData tutoData)
        {
            bool isDialogTuto = false;
            if (tutoData.tutoConfig == TutorialTypeConfigure.RPDungeonClearPoint_0
                || tutoData.tutoConfig == TutorialTypeConfigure.ExpDungeonClearPoint_0
                || tutoData.tutoConfig == TutorialTypeConfigure.GoldUpgradePoint_1
                || tutoData.tutoConfig == TutorialTypeConfigure.StatusUpgradePoint_1
                || tutoData.tutoConfig == TutorialTypeConfigure.AwakeDungeonClearPoint_0
                || tutoData.tutoConfig == TutorialTypeConfigure.SummonSkillPoint_1
                || tutoData.tutoConfig == TutorialTypeConfigure.SummonEquipPoint_1
                || tutoData.tutoConfig == TutorialTypeConfigure.AwakeUpgradePointNavIcon_1
                || tutoData.tutoConfig == TutorialTypeConfigure.SkillAwakePointNavIcon_1
                  || tutoData.tutoConfig == TutorialTypeConfigure.SummonPetPoint_1)
            {
                isDialogTuto = true;
            }
            return isDialogTuto;
        }
        public void PopupToastMessageBtnLocked(int Index)
        {
            LocalizeDescKeys localizedKey = LocalizeDescKeys.Etc_UnlockAfterMission;
            int levelValue = 0;
            if (Index == ControllerUnitUpgrade._index)
            {
                localizedKey = LocalizeDescKeys.Etc_UnlockAfterMission;
                levelValue = Model.Player.Option.ContentUIUnlockLevel(LockedUIType.MainNavUpgradeIcon);
            }
            if (Index == ControllerEquipInventory._index)
            {
                localizedKey = LocalizeDescKeys.Etc_UnlockAfterMission;
                levelValue = Model.Player.Option.ContentUIUnlockLevel(LockedUIType.MainNavItemIcon);
            }
            if (Index == ControllerSkillInventory._index)
            {
                localizedKey = LocalizeDescKeys.Etc_UnlockAfterMission;
                levelValue = Model.Player.Option.ContentUIUnlockLevel(LockedUIType.MainNavSkillIcon);
            }
            if (Index == ControllerPetInventory._index)
            {
                localizedKey = LocalizeDescKeys.Etc_UnlockAfterMission;
                levelValue = Model.Player.Option.ContentUIUnlockLevel(LockedUIType.MainNavPetIcon);
            }
            if (Index == ControllerItemSummon._index)
            {
                localizedKey = LocalizeDescKeys.Etc_UnlockAfterMission;
                levelValue = Model.Player.Option.ContentUIUnlockLevel(LockedUIType.MainNavSummonIcon);
            }
            if (Index == ControllerInAppShop.Index)
            {
                localizedKey = LocalizeDescKeys.Etc_UnlockAfterMission;
                levelValue = Model.Player.Option.ContentUIUnlockLevel(LockedUIType.MainNavInAppPurchaseIcon);
            }
            if (Index == ControllerContents._index)
            {
                localizedKey = LocalizeDescKeys.Etc_UnlockAfterMission;
                levelValue = Model.Player.Option.ContentUIUnlockLevel(LockedUIType.MainNavContentIcon);
            }
            string localizedvalue = string.Format(StaticData.Wrapper.localizeddesclist[(int)localizedKey].StringToLocal,
                levelValue);
            ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
        }
        private void SetArrowObjectPos(Model.TutorialDescData tutoData)
        {
            if (tutoData.tutoConfig == TutorialTypeConfigure.RPDungeonClearPoint_0
                || tutoData.tutoConfig == TutorialTypeConfigure.ExpDungeonClearPoint_0
                || tutoData.tutoConfig == TutorialTypeConfigure.AwakeDungeonClearPoint_0)
            {
                arrowTotalObject.transform.SetParent(viewMainNavButtons[Core.ControllerContents._index].transform, false);
                arrowTotalObject.anchoredPosition = arrowObjectPos;
            }
            if (tutoData.tutoConfig == TutorialTypeConfigure.GoldUpgradePoint_1
             || tutoData.tutoConfig == TutorialTypeConfigure.StatusUpgradePoint_1
              || tutoData.tutoConfig == TutorialTypeConfigure.AwakeUpgradePointNavIcon_1)
            {
                arrowTotalObject.transform.SetParent(viewMainNavButtons[Core.ControllerUnitUpgrade._index].transform, false);
                arrowTotalObject.anchoredPosition = arrowObjectPos;
            }
            if (tutoData.tutoConfig == TutorialTypeConfigure.SummonSkillPoint_1
           || tutoData.tutoConfig == TutorialTypeConfigure.SummonEquipPoint_1
           || tutoData.tutoConfig == TutorialTypeConfigure.SummonPetPoint_1)
            {
                arrowTotalObject.transform.SetParent(viewMainNavButtons[Core.ControllerItemSummon._index].transform, false);
                arrowTotalObject.anchoredPosition = arrowObjectPos;
            }
            if (tutoData.tutoConfig == TutorialTypeConfigure.SkillAwakePointNavIcon_1)
            {
                arrowTotalObject.transform.SetParent(viewMainNavButtons[Core.ControllerSkillInventory._index].transform, false);
                arrowTotalObject.anchoredPosition = arrowObjectPos;
            }
        }

        enum moveType
        {
            up, down
        }
        moveType movetype;
        float currenttime;

        Color alphaColor = new Color(1,1,1,0);
        Color originalColor = Color.white;
        private IEnumerator RoutineMoveArrow()
        {
            movetype = moveType.up;
            currenttime = 0;
            while (true)
            {
                switch (movetype)
                {
                    case moveType.up:
                        currenttime += Time.deltaTime*1.5f;
                        arrowObject.anchoredPosition = Vector2.Lerp(downPos, upPos, currenttime);
                        if (currenttime >= 1)
                        {
                            movetype = moveType.down;
                            currenttime = 0;
                        }
                        break;
                    case moveType.down:
                        currenttime += Time.deltaTime * 1.5f;
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
