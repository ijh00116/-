using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using BlackTree.Bundles;
using BlackTree.Model;
using BlackTree.Definition;

namespace BlackTree.Core
{
    public class ControllerPetInventory
    {
        public const int _index = 3;
        ViewCanvasPetInventory _viewPetInventory;

        ControllerPetEquipslot[] equipedpetSlotList;

        Player.Pet.PetCacheData currentTouchedPet;
        CancellationTokenSource _cts;

        private List<ControllerPetSlot> petSlotList = new List<ControllerPetSlot>();
        int currentPresetIndex;
        public ControllerPetInventory(Transform parent,CancellationTokenSource cts)
        {
            currentPresetIndex = Player.Cloud.userPetEquipinfo.currentEquipPetDeckIndex;

            Player.Pet.SlotTouched += TouchPetSlot;
            Player.Pet.onAfterEquip += AfterEquipSync;
            Player.Pet.changePetPresetUpdate += UpdateChangePreset;

            _cts = cts;
            _viewPetInventory = ViewCanvas.Create<ViewCanvasPetInventory>(parent);
            Init();

            for(int i=0; i<_viewPetInventory.closeBtn.Length; i++)
            {
                int index = i;
                _viewPetInventory.closeBtn[index].onClick.AddListener(() => MainNav.CloseMainUIWindow());
            }

            MainNav.onChange += UpdateViewVisible;

            int petIndex = 0;
            for(int i=0; i< petSlotList.Count; i++)
            {
                var petData=Player.Pet.Get(i);
                if(petData.IsUnlocked)
                {
                    petIndex = i;
                }
            }

            _viewPetInventory.Init();

            TouchPetSlot(petIndex);

            Player.Pet.onUpdateSync += SyncViewer;

            Player.Pet.TotalLevelCalculate();

            for (int i = 0; i < _viewPetInventory.PresetOnList.Count; i++)
            {
                _viewPetInventory.PresetOnList[i].SetActive(false);
            }
            _viewPetInventory.PresetOnList[currentPresetIndex].SetActive(true);

            _viewPetInventory.presetBtn.onClick.AddListener(ChangeSkillPresetIndex);

            _viewPetInventory.petSkillUnlock_title.text=StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_PetSkill].StringToLocal;
            _viewPetInventory.petskillUnlock_BtnText.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Unlock].StringToLocal;
            _viewPetInventory.petDa_Title.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Decomposition].StringToLocal;
            _viewPetInventory.petDa_BtnText.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Decomposition].StringToLocal;

            _viewPetInventory.runeEquipEffectBtn.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_EquipEffect].StringToLocal;
            _viewPetInventory.runeDetailDesc.text = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_SkillAwakeDecomposition].StringToLocal;
            _viewPetInventory.runeEquipBtn.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Equip].StringToLocal;
            _viewPetInventory.runeenforceBtn.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Enforce].StringToLocal;
            _viewPetInventory.runeDaBtn.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Decomposition].StringToLocal;
            _viewPetInventory.runeAllEnforceBtn.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_AllEnforce].StringToLocal;
        }

        void Init()
        {
            for (var i = 0; i < StaticData.Wrapper.petdatas.Length; i++)
            {
                int petID = i;
                petSlotList.Add(new ControllerPetSlot(petID));
            }

            equipedpetSlotList = new ControllerPetEquipslot[_viewPetInventory.equipedSlotList.Length];

            int index = 0;
            foreach (var _petID in Player.Cloud.userPetEquipinfo.deckList[Player.Cloud.userPetEquipinfo.currentEquipPetDeckIndex].petIDlist)
            {
                equipedpetSlotList[index] = new ControllerPetEquipslot(_petID, _viewPetInventory.equipedSlotList[index], index, _cts);
                index++;
            }
            _viewPetInventory.PetImage.gameObject.SetActive(false);
            _viewPetInventory.EquipPetBtn.onClick.AddListener(CurrentPetEquip);
            _viewPetInventory.enforcePetBtn.onClick.AddListener(CurrentPetEnforce);
            _viewPetInventory.AllEnforceBtn.onClick.AddListener(AllEnforce);
            _viewPetInventory.DisAssembleBtn.onClick.AddListener(OpenPopupDisassembleWindow);
            _viewPetInventory.disassembleBtnInWindow.onClick.AddListener(DisassembleCurrentPet);


            for (int i = 0; i < _viewPetInventory.closeDisassembleWindowBtn.Length; i++)
            {
                _viewPetInventory.closeDisassembleWindowBtn[i].onClick.AddListener(() => {
                    _viewPetInventory.disAssembleWindow.SetActive(false);
                });
            }

            Player.Pet.equipslotEnduceForchange += EquipInduce;
            Player.Pet.equipslotEnduceOff += EnduceOff;

            int slotindex = 0;
            foreach (var _petID in Player.Pet.currentPetContainer())
            {
                var equipedslotview = equipedpetSlotList[slotindex];
                equipedslotview.SyncViewer(_petID);
                slotindex++;
            }

            _viewPetInventory.petSkillUnlockOpenBtn.onClick.AddListener(PetSkillUnlockWindowOn);
            for(int i=0; i<_viewPetInventory.closeSkillUnlockWindowBtns.Length; i++)
            {
                _viewPetInventory.closeSkillUnlockWindowBtns[i].onClick.AddListener(()=>
                _viewPetInventory.petSkillUnlockWindow.SetActive(false));
            }
            _viewPetInventory.petSkillUnlockBtn.onClick.AddListener(PetSkillUnlockProcess);
        }


        void SyncViewer(int _petId)
        {
            bool isReddot=false;
            bool isPetReddot = false;
            bool isRuneReddot = false;
            for (int i=0; i<petSlotList.Count; i++)
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
        }

        public void PetSkillUnlockWindowOn()
        {
            _viewPetInventory.petSkillUnlockWindow.SetActive(true);

            string cooltimeLocalize = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Cooltime].StringToLocal;
            string effectTimeLocalize = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_EffectTime].StringToLocal;

            string coolTime = string.Format(cooltimeLocalize, currentTouchedPet.tabledata.skillCoolTime);
            string effectTime = string.Format(effectTimeLocalize, currentTouchedPet.tabledata.skillEffectTime);
            string skilldesc;
            if (currentTouchedPet.tabledata.skillEffectTime > 0)
            {
                skilldesc = StaticData.Wrapper.localizeddesclist[(int)currentTouchedPet.tabledata.petSkillDesc].StringToLocal + effectTime + coolTime;
            }
            else
            {
                skilldesc = StaticData.Wrapper.localizeddesclist[(int)currentTouchedPet.tabledata.petSkillDesc].StringToLocal + coolTime;
            }
            _viewPetInventory.petSkillDescInUnlockWindow.text= string.Format(skilldesc, currentTouchedPet.GetSkillPureValue());
            double Goodsamount = currentTouchedPet.tabledata.unlockAwakeStoneAmount;
            _viewPetInventory.awakeStoneAmountInUnlockWindow.text = Goodsamount.ToNumberString();

            if (Player.Good.Get(GoodsKey.SkillAwakeStone) >= currentTouchedPet.tabledata.unlockAwakeStoneAmount)
            {
                _viewPetInventory.petSkillUnlockBtn.enabled = true;
                _viewPetInventory.ignoreUnlockBtnObj.SetActive(false);
            }
            else
            {
                _viewPetInventory.petSkillUnlockBtn.enabled = false;
                _viewPetInventory.ignoreUnlockBtnObj.SetActive(true);
            }
        }

        public void PetSkillUnlockProcess()
        {
            if(Player.Good.Get(GoodsKey.SkillAwakeStone)>=currentTouchedPet.tabledata.unlockAwakeStoneAmount)
            {
                currentTouchedPet.userPetdata.unlockSkillLevel = 1;
                Player.Cloud.petdata.UpdateHash().SetDirty(true);
                _viewPetInventory.SetPetDetailInfo(currentTouchedPet);
                _viewPetInventory.petSkillUnlockWindow.SetActive(false);

                string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_PetSkillUnlock].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
            }
            else
            {
                string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_AwakeNotenough].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
            }

        }

        void TouchPetSlot(int _petID)
        {
            currentTouchedPet = Player.Pet.Get(_petID);
            _viewPetInventory.SetPetDetailInfo(currentTouchedPet);

            Player.Pet.equipslotEnduceOff?.Invoke();

            Player.Pet.currentEquipwaitPetID = -1;

            _viewPetInventory.EquipPetBtn.enabled = currentTouchedPet.IsUnlocked;
        }

        void AfterEquipSync(int _petID)
        {
            int index = 0;
            foreach (var petID in Player.Pet.currentPetContainer())
            {
                var equipedslotview = equipedpetSlotList[index];
                if (_petID == petID)
                {
                    equipedslotview.SyncViewer(_petID);
                }
                index++;
            }
        }

        void UpdateChangePreset()
        {
            int index = 0;
            foreach (var petID in Player.Pet.currentPetContainer())
            {
                var equipedslotview = equipedpetSlotList[index];
                equipedslotview.SyncViewer(petID);
                index++;
            }

            currentPresetIndex = Player.Cloud.userPetEquipinfo.currentEquipPetDeckIndex;

            for (int i = 0; i < _viewPetInventory.PresetOnList.Count; i++)
            {
                _viewPetInventory.PresetOnList[i].SetActive(false);
            }
            _viewPetInventory.PresetOnList[currentPresetIndex].SetActive(true);

            for (int i = 0; i < StaticData.Wrapper.petdatas.Length; i++)
            {
                Player.Pet.onUpdateSync(i);
            }
        }


        void CurrentPetEquip()
        {
            Player.Pet.Equip(currentTouchedPet.tabledata.index);
            if (Player.Guide.currentGuideQuest == QuestGuideType.PetEquip)
            {
                Player.Guide.QuestGuideProgress(Player.Guide.currentGuideQuest);
            }
        }
        void CurrentPetEnforce()
        {
            if(currentTouchedPet.userPetdata.Obtaincount>= StaticData.Wrapper.petAmountTableData[currentTouchedPet.userPetdata.level].amountForLvUp)
            {
                Player.Pet.Enforce(currentTouchedPet.tabledata.index);
             
                Player.Pet.SyncData(currentTouchedPet.tabledata.index);
                Player.Pet.onUpdateSync?.Invoke(currentTouchedPet.tabledata.index);
                Player.Pet.TotalLevelCalculate();

                Player.Cloud.petdata.UpdateHash().SetDirty(true);
                LocalSaveLoader.SaveUserCloudData();
                _viewPetInventory.SetPetDetailInfo(currentTouchedPet);
            }
            else
            {
                string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_PetNotenough].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
            }
        }

        void AllEnforce()
        {
            foreach(var petdata in Player.Pet.petCaches)
            {
                var petcache = petdata.Value;
                while (true)
                {
                    if (petcache.userPetdata.Obtaincount >= StaticData.Wrapper.petAmountTableData[petcache.userPetdata.level].amountForLvUp)
                    {
                        if(petcache.IsMaxLevel()==false)
                        {
                            Player.Pet.Enforce(petcache.tabledata.index);
                        }
                        else
                        {
                            Player.Pet.SyncData(petcache.tabledata.index);
                            Player.Pet.onUpdateSync?.Invoke(petcache.tabledata.index);
                            break;
                        }
                        
                    }
                    else
                    {
                        Player.Pet.SyncData(petcache.tabledata.index);
                        Player.Pet.onUpdateSync?.Invoke(petcache.tabledata.index);
                        break;
                    }
                }
                
            }


            Player.Pet.TotalLevelCalculate();

            _viewPetInventory.SetPetDetailInfo(currentTouchedPet);

            Player.Cloud.petdata.UpdateHash().SetDirty(true);
            LocalSaveLoader.SaveUserCloudData();
        }

        void OpenPopupDisassembleWindow()
        {
            if (currentTouchedPet.IsMaxLevel() == false)
            {
                return;
            }
            
            _viewPetInventory.disAssembleWindow.SetActive(true);

            _viewPetInventory.skillIconImageFrame_da.sprite = InGameResourcesBundle.Loaded.weaponGradeInnerFrameSprite[currentTouchedPet.tabledata.grade - 1];
            _viewPetInventory.skillIconImage_da.sprite = _viewPetInventory.currentAnimspriteInfo.animInfo.spriteList[0];

            int eachRewardAmount = StaticData.Wrapper.petDisassembleDatas[currentTouchedPet.tabledata.grade - 1].rewardAmount;
            int amount = currentTouchedPet.userPetdata.Obtaincount;

            double skillawakeAmount = eachRewardAmount * amount;
            _viewPetInventory.skillDisassembleDesc.text = string.Format(StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.UI_PetDisAssembleDesc].StringToLocal,
                amount, skillawakeAmount.ToNumberString());

            _viewPetInventory.obtainSkillawakeStoneAmount.text = skillawakeAmount.ToNumberString();

        }

        void DisassembleCurrentPet()
        {
            Player.Pet.Disassemble(currentTouchedPet.tabledata.index);

            string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_Decompose].StringToLocal;
            ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);

            _viewPetInventory.disAssembleWindow.SetActive(false);
        }

        void EquipInduce(int _index)
        {
            int index = 0;
            foreach (var _petID in Player.Cloud.userPetEquipinfo.deckList[Player.Pet.CurrentPetInvenindex].petIDlist)
            {
                var equipedslot = _viewPetInventory.equipedSlotList[index];
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
            for (int i = 0; i < _viewPetInventory.equipedSlotList.Length; i++)
            {
                _viewPetInventory.equipedSlotList[i].StopEnduceaction();
            }
        }

        private void UpdateViewVisible()
        {
            if (MainNav.SelectTabIndex == _index)
            {
                if (_viewPetInventory.IsVisible)
                {
                    _viewPetInventory.blackBG.PopupCloseColorFade();
                    _viewPetInventory.Wrapped.CommonPopupCloseAnimationDown(() => {
                        _viewPetInventory.SetVisible(false);
                    });
                }
                else
                {
                    _viewPetInventory.SetVisible(true);
                    _viewPetInventory.blackBG.PopupOpenColorFade();
                    _viewPetInventory.Wrapped.CommonPopupOpenAnimationUp(()=> {

                        if (Player.Guide.currentGuideQuest == QuestGuideType.PetEquip)
                        {
                            Player.Guide.QuestGuideProgress(Player.Guide.currentGuideQuest);
                        }
                    });
                }
            }
            else
            {
                _viewPetInventory.blackBG.PopupCloseColorFade();
                _viewPetInventory.Wrapped.CommonPopupCloseAnimationDown(() => {
                    _viewPetInventory.SetVisible(false);
                });
            }
        }

        void ChangeSkillPresetIndex()
        {
            currentPresetIndex++;
            if (currentPresetIndex >= Constraints.petEquipPresetCount)
            {
                currentPresetIndex = 0;
            }

            Player.Pet.ChangeCurrentPetPreset(currentPresetIndex);
        }
    }
}
