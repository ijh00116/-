using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using BlackTree.Bundles;
using BlackTree.Model;
using BlackTree.Definition;
using DG.Tweening;

namespace BlackTree.Core
{
    public class ControllerRuneInventory
    {
        ViewCanvasPetInventory _viewPetInventory;

        ControllerRuneEquipSlot[] equipedRuneSlotList;

        Player.Rune.RuneCacheData currentTouchedRune;
        CancellationTokenSource _cts;

        private List<ControllerRuneSlot> runeSlotList = new List<ControllerRuneSlot>();

        Vector2 runeAbilDescNormal = new Vector2(0, 377);
        Vector2 runeAbilDescDown = Vector2.zero;
        public ControllerRuneInventory(Transform parent,CancellationTokenSource cts)
        {
            Player.Rune.SlotTouched += TouchRuneSlot;
            Player.Rune.onAfterEquip += AfterEquipSync;

            _cts = cts;
            _viewPetInventory = ViewCanvas.Get<ViewCanvasPetInventory>();

            Init();

            int runeIndex = 0;
            for (int i = 0; i < runeSlotList.Count; i++)
            {
                var runeData = Player.Rune.Get(i);
                if (runeData.IsUnlocked)
                {
                    runeIndex = i;
                }
            }

            _viewPetInventory.PetButton.onClick.AddListener(()=> {
                _viewPetInventory.TopSelector.Show(0);
                _viewPetInventory.petWindow.SetActive(true);
                _viewPetInventory.runeWindow.SetActive(false);
            });
            _viewPetInventory.RuneButton.onClick.AddListener(() => {
                RuneBtnPush();
            });


            Player.Option.ContentUnlockUpdate += LockUpdate;
            LockUpdate();

            TouchRuneSlot(runeIndex);

            Player.Rune.onUpdateSync += SyncViewer;

            _viewPetInventory.PetButton.onClick?.Invoke();

            _viewPetInventory.expandAbilDescDownBtn.onClick.AddListener(DownDescWindow);
            _viewPetInventory.expandAbilDescUpBtn.onClick.AddListener(UpDescWindow);

            Player.Rune.SyncAllData();
        }

        void RuneBtnPush()
        {
            int unlockLv = StaticData.Wrapper.ingameLockData[(int)LockedUIType.RuneSummonUnlock].unLockLevel;
            if (Player.Cloud.userLevelData.currentLevel < unlockLv)
            {
                LocalizeDescKeys localizedKey = LocalizeDescKeys.Etc_UnlockAfterLevel;
                int levelValue = Model.Player.Option.ContentUIUnlockLevel(LockedUIType.RuneSummonUnlock);
                string localizedvalue = string.Format(StaticData.Wrapper.localizeddesclist[(int)localizedKey].StringToLocal, levelValue);
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
            }
            else
            {
                _viewPetInventory.TopSelector.Show(1);
                _viewPetInventory.petWindow.SetActive(false);
                _viewPetInventory.runeWindow.SetActive(true);
            }
        }

        void LockUpdate()
        {
            int summonRuneLv = StaticData.Wrapper.ingameLockData[(int)LockedUIType.RuneSummonUnlock].unLockLevel;
           
            if (Player.Cloud.userLevelData.currentLevel >= summonRuneLv)
            {
                _viewPetInventory.runeBtnLocked.SetActive(false);
            }
            else
            {
                _viewPetInventory.runeBtnLocked.SetActive(true);
            }
        }

        void DownDescWindow()
        {
            _viewPetInventory.abilDescWindow.DOAnchorPos(runeAbilDescDown, 0.25f).SetEase(Ease.OutQuad).SetUpdate(true).OnComplete(() =>
            {
                _viewPetInventory.expandAbilDescDownBtn.gameObject.SetActive(false);
                _viewPetInventory.expandAbilDescUpBtn.gameObject.SetActive(true);
            });
        }

        void UpDescWindow()
        {
            _viewPetInventory.abilDescWindow.DOAnchorPos(runeAbilDescNormal, 0.25f).SetEase(Ease.OutQuad).SetUpdate(true).OnComplete(() =>
            {
                _viewPetInventory.expandAbilDescDownBtn.gameObject.SetActive(true);
                _viewPetInventory.expandAbilDescUpBtn.gameObject.SetActive(false);
            });
        }
        void Init()
        {
            for (var i = 0; i < StaticData.Wrapper.runedatas.Length; i++)
            {
                int _ID = i;
                runeSlotList.Add(new ControllerRuneSlot(_ID));
            }

            equipedRuneSlotList = new ControllerRuneEquipSlot[_viewPetInventory.equipedSlotList_rune.Length];

            int index = 0;
            foreach (var _ID in Player.Cloud.runeEquipedInfo.deckList[Player.Cloud.runeEquipedInfo.currentEquipRuneIndex].runes)
            {
                equipedRuneSlotList[index] = new ControllerRuneEquipSlot(_ID, _viewPetInventory.equipedSlotList_rune[index], index, _cts);
                index++;
            }
            _viewPetInventory.RuneImage.gameObject.SetActive(false);
            _viewPetInventory.EquipRuneBtn.onClick.AddListener(CurrentRuneEquip);
            _viewPetInventory.enforceRuneBtn.onClick.AddListener(CurrentRuneEnforce);
            _viewPetInventory.AllEnforceBtn_rune.onClick.AddListener(AllEnforce);
            _viewPetInventory.DisAssembleBtn_rune.onClick.AddListener(OpenPopupRuneDisassembleWindow);
            _viewPetInventory.disassembleBtnInWindow_rune.onClick.AddListener(DisassembleCurrentRune);


            for (int i = 0; i < _viewPetInventory.closeDisassembleWindowBtn_rune.Length; i++)
            {
                _viewPetInventory.closeDisassembleWindowBtn_rune[i].onClick.AddListener(() => {
                    _viewPetInventory.disAssembleWindow_rune.SetActive(false);
                });
            }

            Player.Rune.equipslotEnduceForchange += EquipInduce;
            Player.Rune.equipslotEnduceOff += EnduceOff;

            int slotindex = 0;
            foreach (var _ID in Player.Rune.currentRuneContainer())
            {
                var equipedslotview = equipedRuneSlotList[slotindex];
                equipedslotview.SyncViewer(_ID);
                index++;
            }

            abilDescSet();
        }

        void SyncViewer(int _Id)
        {

            bool isReddot = false;
            bool isPetReddot = false;
            bool isRuneReddot = false;
            for (int i = 0; i < StaticData.Wrapper.petdatas.Length; i++)
            {
                var petData = Player.Pet.Get(i);
                if (petData.IsMaxLevel())
                {
                    if (petData.userPetdata.Obtaincount > 0)
                    {
                        isReddot = true;
                        isPetReddot = true;
                        break;
                    }
                }
                else
                {
                    if (petData.userPetdata.Obtaincount > (float)(StaticData.Wrapper.petAmountTableData[petData.userPetdata.level].amountForLvUp))
                    {
                        isReddot = true;
                        isPetReddot = true;
                        break;
                    }
                }
            }

            for (int i = 0; i < StaticData.Wrapper.runedatas.Length; i++)
            {
                var runeData = Player.Rune.Get(i);
                if (runeData.IsMaxLevel())
                {
                    if (runeData.userRunedata.Obtaincount > 0)
                    {
                        isReddot = true;
                        isRuneReddot = true;
                        break;
                    }
                }
                else
                {
                    if (runeData.userRunedata.Obtaincount > (float)(StaticData.Wrapper.runeAmountTableData[runeData.userRunedata.Obtainlv].amountForLvUp))
                    {
                        isReddot = true;
                        isRuneReddot = true;
                        break;
                    }
                }
            }

            _viewPetInventory.petReddot.SetActive(isPetReddot);
            _viewPetInventory.runeReddot.SetActive(isRuneReddot);

            var mainNavView = ViewCanvas.Get<ViewCanvasMainNav>();
            mainNavView.viewMainNavButtons[(int)MainNavigationType.Pet].ActivateNotification(isReddot);

            abilDescSet();
        }

        void abilDescSet()
        {
            string abildesc = "";
            int index = 0;
            var container = Player.Rune.currentRuneContainer();

            for (int i = 0; i < container.Count; i++)
            {
                index++;
                if (container[i] < 0)
                    continue;
                var runedata = Player.Rune.Get(container[i]);

                string desc =string.Format("{0}. ", index)+ _viewPetInventory.RuneDesc(runedata);

                abildesc += (desc + "\n");
            }

            _viewPetInventory.abilDesc.text = abildesc;
        }

        void TouchRuneSlot(int _ID)
        {
            currentTouchedRune = Player.Rune.Get(_ID);
            _viewPetInventory.SetRuneDetailInfo(currentTouchedRune);

            Player.Rune.equipslotEnduceOff?.Invoke();

            Player.Rune.currentEquipwaitRuneID = -1;

            _viewPetInventory.EquipRuneBtn.enabled = currentTouchedRune.IsUnlocked;
        }

        void AfterEquipSync(int _ID)
        {
            int index = 0;
            foreach (var ID in Player.Rune.currentRuneContainer())
            {
                var equipedslotview = equipedRuneSlotList[index];
                if (ID == _ID)
                {
                    equipedslotview.SyncViewer(_ID);
                }
                index++;
            }
        }

        void CurrentRuneEquip()
        {
            Player.Rune.Equip(currentTouchedRune.tabledata.index);
        }
        void CurrentRuneEnforce()
        {
            if (currentTouchedRune.userRunedata.Obtaincount >= StaticData.Wrapper.runeAmountTableData[currentTouchedRune.userRunedata.Obtainlv].amountForLvUp)
            {
                Player.Rune.Enforce(currentTouchedRune.tabledata.index);


                Player.Rune.SyncData(currentTouchedRune.tabledata.index);
                Player.Rune.onUpdateSync?.Invoke(currentTouchedRune.tabledata.index);
                _viewPetInventory.SetRuneDetailInfo(currentTouchedRune);

                Player.Cloud.runeData.UpdateHash().SetDirty(true);
                LocalSaveLoader.SaveUserCloudData();
            }
            else
            {
                string review = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_runeNotEnough].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(review);
            }
        }

        void AllEnforce()
        {
            foreach (var runedata in Player.Rune.runeCaches)
            {
                var runecache = runedata.Value;
                while (true)
                {
                    if (runecache.userRunedata.Obtaincount >= StaticData.Wrapper.runeAmountTableData[runecache.userRunedata.Obtainlv].amountForLvUp)
                    {
                        if (runecache.IsMaxLevel() == false)
                        {
                            Player.Rune.Enforce(runecache.tabledata.index);
                        }
                        else
                        {
                            Player.Rune.SyncData(runecache.tabledata.index);
                            Player.Rune.onUpdateSync?.Invoke(runecache.tabledata.index);
                         
                            break;
                        }

                    }
                    else
                    {
                        Player.Rune.SyncData(runecache.tabledata.index);
                        Player.Rune.onUpdateSync?.Invoke(runecache.tabledata.index);
                        break;
                    }
                }

            }


            _viewPetInventory.SetRuneDetailInfo(currentTouchedRune);

            Player.Cloud.runeData.UpdateHash().SetDirty(true);
            LocalSaveLoader.SaveUserCloudData();
        }

        void OpenPopupRuneDisassembleWindow()
        {
            if (currentTouchedRune.IsMaxLevel() == false)
            {
                return;
            }

            _viewPetInventory.disAssembleWindow_rune.SetActive(true);

            _viewPetInventory.IconImageFrame_rune.sprite = InGameResourcesBundle.Loaded.weaponGradeInnerFrameSprite[currentTouchedRune.tabledata.grade - 1];
            _viewPetInventory.IconImage_rune.sprite = _viewPetInventory.RuneImage.sprite;
            

            int eachRewardAmount = StaticData.Wrapper.runeDisassembleDatas[currentTouchedRune.tabledata.grade - 1].rewardAmount;
            int amount = currentTouchedRune.userRunedata.Obtaincount;

            double awakeAmount = eachRewardAmount * amount;
            _viewPetInventory.DisassembleDesc_rune.text = string.Format(StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.UI_SkillDisAssembleDesc].StringToLocal,
                amount, awakeAmount.ToNumberString());

            _viewPetInventory.obtainSkillawakeStoneAmount_rune.text = awakeAmount.ToNumberString();

        }

        void DisassembleCurrentRune()
        {
            Player.Rune.Disassemble(currentTouchedRune.tabledata.index);

            string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_Decompose].StringToLocal;
            ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);

            _viewPetInventory.disAssembleWindow.SetActive(false);
        }

        void EquipInduce(int _index)
        {
            int index = 0;
            foreach (var _ID in Player.Cloud.runeEquipedInfo.deckList[Player.Rune.CurrentRuneInvenindex].runes)
            {
                var equipedslot = _viewPetInventory.equipedSlotList_rune[index];
                equipedslot.StartEnduceaction();
                index++;
                if (index >= _index)
                {
                    break;
                }
            }
        }


        void EnduceOff()
        {
            for (int i = 0; i < _viewPetInventory.equipedSlotList_rune.Length; i++)
            {
                _viewPetInventory.equipedSlotList_rune[i].StopEnduceaction();
            }
        }

    }
}

