using System.Collections;
using UnityEngine;
using UnityEngine.Events;
//using DG.Tweening;
using BlackTree.Core;
using System.Collections.Generic;
using BlackTree.Definition;

namespace BlackTree.Bundles
{
    public class ViewMainNavButton : MonoBehaviour
    {
        public NavType navType;
        public TMPro.TMP_Text Slotname;
        public BTButton button;
        public BTSelector selector;
        public RectTransform rectTr;

        public GameObject lockedObj;
        public GameObject redDot;

        public UnityAction OnAddAnimation;

        private Coroutine _coAddAnimation;

        private float _animationScaleSize = 0.5f;

        int myIndex;
        public bool isUnlocked = false;


        public void Init(int index)
        {
            OnAddAnimation += StartRoutineAddAnimation;
            myIndex = index;
            if(myIndex==ControllerUnitUpgrade._index)
            {
                isUnlocked = true;
            }
            Model.Player.Option.ContentUnlockUpdate += UnlockUpdate;

            switch (navType)
            {
                case NavType.Upgrade:
                    Slotname.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Enforce].StringToLocal;
                    break;
                case NavType.Item:
                    Slotname.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Item].StringToLocal;
                    break;
                case NavType.Skill:
                    Slotname.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Skill].StringToLocal;
                    break;
                case NavType.Pet:
                    Slotname.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Pet_Rune].StringToLocal;
                    break;
                case NavType.Summon:
                    Slotname.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Summon].StringToLocal;
                    break;
                case NavType.Shop:
                    Slotname.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Shop].StringToLocal;
                    break;
                case NavType.Content:
                    Slotname.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Contents].StringToLocal;
                    break;
                default:
                    break;
            }
            UnlockUpdate();
        }

        public void ActivateNotification(bool isActive)
        {
            redDot.SetActive(isActive);
        }
        public void UnlockUpdate()
        {
            if (myIndex == ControllerUnitUpgrade._index)
            {
                bool isUnlock = Model.Player.Option.IsContentUIUnlocked(Definition.LockedUIType.MainNavUpgradeIcon);
                lockedObj.gameObject.SetActive(!isUnlock);
                isUnlocked = isUnlock;
            }
            if (myIndex == ControllerEquipInventory._index)
            {
                bool isUnlock = Model.Player.Option.IsContentUIUnlocked(Definition.LockedUIType.MainNavItemIcon);
                lockedObj.gameObject.SetActive(!isUnlock);
                isUnlocked = isUnlock;
            }
            if (myIndex == ControllerSkillInventory._index)
            {
                bool isUnlock = Model.Player.Option.IsContentUIUnlocked(Definition.LockedUIType.MainNavSkillIcon);
                lockedObj.gameObject.SetActive(!isUnlock);
                isUnlocked = isUnlock;
            }
            if (myIndex == ControllerPetInventory._index)
            {
                bool isUnlock = Model.Player.Option.IsContentUIUnlocked(Definition.LockedUIType.MainNavPetIcon);
                lockedObj.gameObject.SetActive(!isUnlock);
                isUnlocked = isUnlock;
            }
            if (myIndex == ControllerItemSummon._index)
            {
                bool isUnlock = Model.Player.Option.IsContentUIUnlocked(Definition.LockedUIType.MainNavSummonIcon);
                lockedObj.gameObject.SetActive(!isUnlock);
                isUnlocked = isUnlock;
            }
            if (myIndex == ControllerInAppShop.Index)
            {
                bool isUnlock = Model.Player.Option.IsContentUIUnlocked(Definition.LockedUIType.MainNavInAppPurchaseIcon);
                lockedObj.gameObject.SetActive(!isUnlock);
                isUnlocked = isUnlock;
            }
            if (myIndex == ControllerContents._index)
            {
                bool isUnlock = Model.Player.Option.IsContentUIUnlocked(Definition.LockedUIType.MainNavContentIcon);
                lockedObj.gameObject.SetActive(!isUnlock);
                isUnlocked = isUnlock;
            }
        }

        public void SetOnOff(bool flag)
        {
            selector.Show(flag ? 0 : 1);
        }

     

        private void StartRoutineAddAnimation()
        {
            //rectTr.DOKill();
            //rectTr.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.1f);
        }
    }
}
