using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using BlackTree.Bundles;
using BlackTree.Definition;

namespace BlackTree.Core
{
    public enum MainNavigationType
    {
        Upgrade=0,
        Item,Skill,Pet,Spawn,Shop,Contents,End
    }
    public static class MainNav
    {
        public static int SelectTabIndex { get; set; } = -1;
        public static int BeforeSelectTabIndex { get; set; } = -1;
        public static UnityAction onChange;
        public static void SetTabIndex(int index)
        {
            if(BeforeSelectTabIndex== SelectTabIndex)
            {
                BeforeSelectTabIndex = -1;
                
            }
            else
            {
                BeforeSelectTabIndex = SelectTabIndex;
                AudioManager.Instance.SetLowPassFilterValue(false);
                Model.Player.Quest.otherUIActive?.Invoke(false);
            }

            if(index<0)
            {
                AudioManager.Instance.SetLowPassFilterValue(false);
                Model.Player.Quest.otherUIActive?.Invoke(false);
            }
            else
            {
                if(SelectTabIndex==index&& BeforeSelectTabIndex>=0)
                {
                    AudioManager.Instance.SetLowPassFilterValue(false);
                    Model.Player.Quest.otherUIActive?.Invoke(false);
                }
                else
                {
                    AudioManager.Instance.SetLowPassFilterValue(true);
                    Model.Player.Quest.otherUIActive?.Invoke(true);
                }
                
            }
            
            SelectTabIndex = index;
            onChange?.Invoke();
            Model.Player.Option.MenuContentBarOff?.Invoke(false);
        }

        public static void CloseMainUIWindow()
        {
            SetTabIndex(-1);
        }
        public static void MainUISetting(bool Active)
        {
            ViewCanvas.Get<ViewCanvasMainQuest>().SetVisible(Active);
            ViewCanvas.Get<ViewCanvasMainWave>().SetVisible(Active);
            ViewCanvas.Get<ViewCanvasMainIcons>().SetVisible(Active);
            ViewCanvas.Get<ViewCanvasMainNav>().SetVisible(Active);
          
        }
        public static void MainSkillUISetting(bool Active)
        {
            ViewCanvas.Get<ViewCanvasMainEquipSkill>().SetVisible(Active);
        }
    }

    public class ControllerMainNav
    {
        public readonly ViewCanvasMainNav _viewCanvasMainNav;

        public readonly ViewLoading _viewLoading;
        public ControllerMainNav(Transform parent)
        {
            _viewCanvasMainNav = ViewCanvas.Create<ViewCanvasMainNav>(parent);
            _viewLoading = ViewCanvas.Create<ViewLoading>(parent);
            ViewCanvas.Create<ViewCanvasToastMessage>(parent);

            
            for (var i = 0; i < _viewCanvasMainNav.viewMainNavButtons.Length; i++)
            {
                var index = i;
                _viewCanvasMainNav.viewMainNavButtons[i].Init(index);
                _viewCanvasMainNav.viewMainNavButtons[i].button.onClick.AddListener(() =>
                {
                    if(_viewCanvasMainNav.viewMainNavButtons[index].isUnlocked==false)
                    {
                        _viewCanvasMainNav.PopupToastMessageBtnLocked(index);
                    }
                    else
                    {
                        MainNav.SetTabIndex(index);
                    }
                    
                });
            }
            for (var i = 0; i < _viewCanvasMainNav.viewMainNavButtons.Length; i++)
            {
                _viewCanvasMainNav.viewMainNavButtons[i].SetOnOff(false);
            }

            ViewCanvas.Create<ViewCanvasGuideDialogue>(parent);

            MainNav.onChange += SetButtonsUI;
        }

        //void PopupToastMessageBtnLocked(int Index)
        //{
        //    LocalizeDescKeys localizedKey = LocalizeDescKeys.Etc_UnlockAfterMission;
        //    int levelValue = 0;
        //    if (Index == ControllerUnitUpgrade._index)
        //    {
        //        localizedKey = LocalizeDescKeys.Etc_UnlockAfterMission;
        //        levelValue = Model.Player.Option.ContentUIUnlockLevel(LockedUIType.MainNavUpgradeIcon);
        //    }
        //    if (Index == ControllerEquipInventory._index)
        //    {
        //        localizedKey= LocalizeDescKeys.Etc_UnlockAfterMission;
        //        levelValue = Model.Player.Option.ContentUIUnlockLevel(LockedUIType.MainNavItemIcon);
        //    }
        //    if (Index == ControllerSkillInventory._index)
        //    {
        //        localizedKey = LocalizeDescKeys.Etc_UnlockAfterMission;
        //        levelValue = Model.Player.Option.ContentUIUnlockLevel(LockedUIType.MainNavSkillIcon);
        //    }
        //    if (Index == ControllerPetInventory._index)
        //    {
        //        localizedKey = LocalizeDescKeys.Etc_UnlockAfterMission;
        //        levelValue = Model.Player.Option.ContentUIUnlockLevel(LockedUIType.MainNavPetIcon);
        //    }
        //    if (Index == ControllerItemSummon._index)
        //    {
        //        localizedKey = LocalizeDescKeys.Etc_UnlockAfterMission;
        //        levelValue = Model.Player.Option.ContentUIUnlockLevel(LockedUIType.MainNavSummonIcon);
        //    }
        //    if (Index == ControllerInAppShop.Index)
        //    {
        //        localizedKey = LocalizeDescKeys.Etc_UnlockAfterMission;
        //        levelValue = Model.Player.Option.ContentUIUnlockLevel(LockedUIType.MainNavInAppPurchaseIcon);
        //    }
        //    if (Index == ControllerContents._index)
        //    {
        //        localizedKey = LocalizeDescKeys.Etc_UnlockAfterMission;
        //        levelValue = Model.Player.Option.ContentUIUnlockLevel(LockedUIType.MainNavContentIcon);
        //    }
        //    string localizedvalue = string.Format(StaticData.Wrapper.localizeddesclist[(int)localizedKey].StringToLocal,
        //        levelValue);
        //    ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
        //}

        void SetButtonsUI()
        {
            for (var i = 0; i < _viewCanvasMainNav.viewMainNavButtons.Length; i++)
            {
                int index = i;

                if(index == MainNav.SelectTabIndex)
                {
                    if(MainNav.SelectTabIndex== MainNav.BeforeSelectTabIndex)
                    {
                        _viewCanvasMainNav.viewMainNavButtons[index].SetOnOff(false);
                    }
                    else
                    {
                        _viewCanvasMainNav.viewMainNavButtons[index].SetOnOff(true);
                    }
                }
                else
                {
                    _viewCanvasMainNav.viewMainNavButtons[index].SetOnOff(false);
                }
                
            }
        }
    }

}
