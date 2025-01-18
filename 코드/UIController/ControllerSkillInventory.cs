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
    public class ControllerSkillInventory
    {
        public const int _index = 2;
        ViewCanvasSkillInventory _viewSkillInventory;

        ControllerSkillSlotEquiped[] equipedskillSlotList;

        CancellationTokenSource _cts;

        private List<ControllerSkillSlot> skillslotList = new List<ControllerSkillSlot>();

        int currentPresetIndex;
        public ControllerSkillInventory(Transform parent,CancellationTokenSource cts)
        {
            currentPresetIndex = Player.Cloud.userSkillEquipinfo.currentEquipSkillIndex;
            _cts = cts;
            _viewSkillInventory = ViewCanvas.Create<ViewCanvasSkillInventory>(parent);
            _viewSkillInventory.Init();
            Init();

            MainNav.onChange += UpdateViewVisible;

            for (int i = 0; i < _viewSkillInventory.closeBtn.Length; i++)
            {
                int index = i;
                _viewSkillInventory.closeBtn[index].onClick.AddListener(() => MainNav.CloseMainUIWindow());
            }

            Player.Skill.SlotTouched += TouchskillSlot;
            Player.Skill.onAfterEquip += AfterEquipSync;
            Player.Skill.changePresetUpdate += AllSlotUpdateSync;

            Player.Skill.onUpdateSync += SyncUIViewer;
            Player.Skill.onUpdateSync += (skillkey) => {
                if(Player.Skill.currentTouchedSkill == Player.Skill.Get(skillkey))
                {
                    _viewSkillInventory.SetSkillDetailInfo(Player.Skill.currentTouchedSkill);
                }
            };

            SkillKey skillKey = SkillKey.SwordExplode;
            for(int i=0; i<(int)SkillKey.End; i++)
            {
                var skilldata = Player.Skill.Get((SkillKey)(i));
                if(skilldata.userSkilldata.Unlock)
                {
                    skillKey = (SkillKey)i;
                }
            }

            _viewSkillInventory.skillawakeTitle.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_SkillAwake].StringToLocal;
            _viewSkillInventory.skillawakeBtnTitle.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_ItemAwake].StringToLocal;

            _viewSkillInventory.skilldisassembleTitle.text=StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Decomposition].StringToLocal;
            _viewSkillInventory.skilldisassembleBtnTitle.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Decomposition].StringToLocal;

            TouchskillSlot(skillKey);

            Player.Skill.TotalLevelCalculate();
        }

        void Init()
        {
            for (var i = 0; i < StaticData.Wrapper.skillDatas.Length; i++)
            {
                skillslotList.Add(new ControllerSkillSlot(StaticData.Wrapper.skillDatas[i].skillKey));
            }
            for(int i=0; i< skillslotList.Count; i++)
            {
                skillslotList[i]._viewslot.transform.SetAsLastSibling();
            }

            equipedskillSlotList = new ControllerSkillSlotEquiped[_viewSkillInventory.equipedSlotList.Length];

            int index = 0;
            foreach(var _key in Player.Cloud.userSkillEquipinfo.deckList[Player.Cloud.userSkillEquipinfo.currentEquipSkillIndex].skills)
            {
                equipedskillSlotList[index] = new ControllerSkillSlotEquiped(_key, _viewSkillInventory.equipedSlotList[index], index, _cts);
                index++;
            }

            _viewSkillInventory.equipSkillBtn.onClick.AddListener(CurrentSkillEquip);
            _viewSkillInventory.enforceSkillBtn.onClick.AddListener(CurrentSkillEnforce);
            _viewSkillInventory.totalSkillBtn.onClick.AddListener(TotalSkillEnforce);
            _viewSkillInventory.awakeSkillBtn.onClick.AddListener(OpenAwakeDetailWindow);
            _viewSkillInventory.disassembleBtn.onClick.AddListener(OpenPopupDisassembleWindow);
            _viewSkillInventory.disassembleBtnInWindow.onClick.AddListener(DisassembleCurrentSkill);

            for (int i=0; i< _viewSkillInventory.closeDisassembleWindowBtn.Length; i++)
            {
                _viewSkillInventory.closeDisassembleWindowBtn[i].onClick.AddListener(() => {
                    _viewSkillInventory.disAssembleWindow.SetActive(false);
                });
            }

            Player.Skill.equipslotEnduceForchange += EquipInduce;
            Player.Skill.equipslotEnduceOff += EnduceOff;

            int slotindex = 0;
            foreach (var _key in Player.Skill.currentSkillContainer())
            {
                var equipedslotview = equipedskillSlotList[slotindex];
                equipedslotview.SyncViewer(_key);
                slotindex++;
            }

            _viewSkillInventory.obtainSkillAwakeGood.Init();
            Player.ControllerGood.BindOnChange(GoodsKey.SkillAwakeStone, () =>
            {
                UpdateGoodView(GoodsKey.SkillAwakeStone);
            });
            UpdateGoodView(GoodsKey.SkillAwakeStone);

            Player.Option.ContentUnlockUpdate += LockUpdate;
            LockUpdate();

            _viewSkillInventory.awakeConfirmBtninDetail.onClick.AddListener(CurrentSkillAwake);

            for(int i=0; i< _viewSkillInventory.closeAwakeDetailWindowBtn.Length; i++)
            {
                _viewSkillInventory.closeAwakeDetailWindowBtn[i].onClick.AddListener(CloseAwakeDetailWindow);
            }

            for(int i=0; i<_viewSkillInventory.PresetOnList.Count; i++)
            {
                _viewSkillInventory.PresetOnList[i].SetActive(false);
            }
            _viewSkillInventory.PresetOnList[currentPresetIndex].SetActive(true);

            _viewSkillInventory.presetBtn.onClick.AddListener(ChangeSkillPresetIndex);

            _viewSkillInventory.goodsBtn.onClick.AddListener(() => {
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasInventory>().SetVisible(true);
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasInventory>().blackBG.PopupOpenColorFade();
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasInventory>().Wrapped.CommonPopupOpenAnimationDown();
            });
        }

        void LockUpdate()
        {
            int skillawakeUnlockLv = StaticData.Wrapper.ingameLockData[(int)LockedUIType.SkillAwakeQuestUnlock].unLockLevel;
            if (Player.Quest.mainQuestCurrentId >= skillawakeUnlockLv)
            {
                _viewSkillInventory.awakeSkillBtn.enabled = true;
                _viewSkillInventory.skillAwakeLockObj.SetActive(false);
            }
            else
            {
                _viewSkillInventory.awakeSkillBtn.enabled = true;
                _viewSkillInventory.skillAwakeLockObj.SetActive(true);
            }

        }


        void ChangeSkillPresetIndex()
        {
            currentPresetIndex++;
            if (currentPresetIndex >= Constraints.skillEquipPresetCount)
            {
                currentPresetIndex = 0;
            }

         
            Player.Skill.ChangeCurrentSkillPreset(currentPresetIndex);
        }

        private void UpdateGoodView(GoodsKey key)
        {
            if(_viewSkillInventory.obtainSkillAwakeGood.GoodsType==key)
            {
                var _value= Player.ControllerGood.GetValue(key);
                _viewSkillInventory.obtainSkillAwakeGood.StartRoutineUpdateView(_value);
            }
        }

        void TouchskillSlot(Definition.SkillKey skillkey)
        {
            Player.Skill.currentTouchedSkill= Player.Skill.Get(skillkey);
            _viewSkillInventory.SetSkillDetailInfo(Player.Skill.currentTouchedSkill);

            Player.Skill.equipslotEnduceOff?.Invoke();

            Player.Skill.currentEquipwaitSkill =Definition.SkillKey.None;

            SyncUIViewer(skillkey);
        }

        void CurrentSkillEquip()
        {
            if(Player.Skill.currentTouchedSkill.IsEquiped==false)
                Player.Skill.Equip(Player.Skill.currentTouchedSkill.tabledataSkill.skillKey);

            if (Player.Guide.currentGuideQuest == QuestGuideType.SkillEquip)
            {
                Player.Guide.QuestGuideProgress(Player.Guide.currentGuideQuest);
            }
        }

        void CurrentSkillEnforce()
        {
            if (Player.Skill.currentTouchedSkill.isMaxLevel())
                return;
            if (Player.Skill.currentTouchedSkill.userSkilldata.Obtaincount< Player.Skill.currentTouchedSkill.GetAmountForLevelUp())
            {
                //Debug.Log($"개수가 {Player.Skill.currentTouchedSkill.GetAmountForLevelUp()}보다 작으므로 땡");
                return;
            }
            if (Player.Skill.currentTouchedSkill.userSkilldata.Unlock)
            {
                Player.Skill.Enforce(Player.Skill.currentTouchedSkill.tabledataSkill.skillKey);
            }
        }

        void OpenPopupDisassembleWindow()
        {
            if (Player.Skill.currentTouchedSkill.isMaxLevel()==false)
            {
                return;
            }
            _viewSkillInventory.disAssembleWindow.SetActive(true);

            _viewSkillInventory.skillIconImage_da.sprite = InGameResourcesBundle.Loaded.skillIcon[(int)Player.Skill.currentTouchedSkill.tabledataSkill.skillKey];

            int eachRewardAmount = StaticData.Wrapper.skillDisassembleDatas[Player.Skill.currentTouchedSkill.tabledataSkill.grade - 1].rewardAmount;
            int amount = Player.Skill.currentTouchedSkill.userSkilldata.Obtaincount;

            double skillawakeAmount = eachRewardAmount * amount;
            _viewSkillInventory.skillDisassembleDesc.text = string.Format(StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.UI_SkillDisAssembleDesc].StringToLocal,
                amount, skillawakeAmount);

            _viewSkillInventory.obtainSkillawakeStoneAmount.text = skillawakeAmount.ToNumberString();

        }

        void DisassembleCurrentSkill()
        {
            //int eachRewardAmount = StaticData.Wrapper.skillDisassembleDatas[Player.Skill.currentTouchedSkill.tabledataSkill.grade - 1].rewardAmount;
            //int amount = Player.Skill.currentTouchedSkill.userSkilldata.Obtaincount;

            //double skillawakeAmount = eachRewardAmount * amount;

            //Player.ControllerGood.Earn(GoodsKey.SkillAwakeStone, skillawakeAmount);
            //Player.Skill.currentTouchedSkill.userSkilldata.Obtaincount = 0;

            Player.Skill.DisAssemble(Player.Skill.currentTouchedSkill.tabledataSkill.skillKey);

            string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_Decompose].StringToLocal;
            ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);

            _viewSkillInventory.disAssembleWindow.SetActive(false);
        }

        void TotalSkillEnforce()
        {
            Player.Skill.AllEnforceWithoutSync();

        }

        void CurrentSkillAwake()
        {
            if(Player.Skill.currentTouchedSkill.tabledataSkill.skillKey==SkillKey.GuidedMissile&& Player.Cloud.playingRecord.isFreeSkillAwakeComplete == false)
            {
                //스킬 튜토리얼 삼연격인 경우
                Player.Guide.StartTutorial(Definition.TutorialType.SkillAwake);
                //스킬 튜토리얼 삼연격인 경우
            }
            else
            {
                if (Player.Skill.currentTouchedSkill.canAwake() == false)
                {
                    //Debug.Log($"개수가 {Player.Skill.currentTouchedSkill.GetAwakeStoneAmountForAwake()}보다 작으므로 땡");
                    return;
                }
            }
     
            if (Player.Skill.currentTouchedSkill.userSkilldata.Unlock)
            {
                Player.Skill.Awake(Player.Skill.currentTouchedSkill.tabledataSkill.skillKey);
            }

            //AwakeDetailUIUpdate();
            _viewSkillInventory.AwakeDetailWindow.SetActive(false);
        }

        void AfterEquipSync(Definition.SkillKey skillkey)
        {
            int index = 0;
            foreach(var _key in Player.Skill.currentSkillContainer())
            {
                var equipedslotview = equipedskillSlotList[index];
                if (skillkey==_key)
                {
                    equipedslotview.SyncViewer(_key);
                }
                index++;
            }
        }

        void AllSlotUpdateSync()
        {
            int index = 0;
            foreach (var _key in Player.Skill.currentSkillContainer())
            {
                var equipedslotview = equipedskillSlotList[index];
                equipedslotview.SyncViewer(_key);
                index++;
            }

            currentPresetIndex = Player.Cloud.userSkillEquipinfo.currentEquipSkillIndex;

            for (int i = 0; i < _viewSkillInventory.PresetOnList.Count; i++)
            {
                _viewSkillInventory.PresetOnList[i].SetActive(false);
            }
            _viewSkillInventory.PresetOnList[currentPresetIndex].SetActive(true);

            for (int i = 0; i < (int)SkillKey.End; i++)
            {
                Player.Skill.onUpdateSync((SkillKey)i);
            }

        }

        void SyncUIViewer(Definition.SkillKey key)
        {
            if (Player.Skill.currentTouchedSkill==null)
            {
                _viewSkillInventory.equipSkillBtn.enabled=false;
                _viewSkillInventory.equipInActive.SetActive(true);
            }
            else
            {
                if (Player.Skill.currentTouchedSkill.IsEquiped)
                {
                    _viewSkillInventory.equipSkillBtn.enabled = false;
                    _viewSkillInventory.equipInActive.SetActive(true);
                }
                else
                {
                    if (Player.Skill.currentTouchedSkill.IsUnlocked)
                    {
                        _viewSkillInventory.equipSkillBtn.enabled = true;
                        _viewSkillInventory.equipInActive.SetActive(false);
                    }
                    else
                    {
                        _viewSkillInventory.equipSkillBtn.enabled = false;
                        _viewSkillInventory.equipInActive.SetActive(true);
                    }
                }
             

                _viewSkillInventory.disassembleBtn.gameObject.SetActive(false);

                if (Player.Skill.currentTouchedSkill.isMaxLevel())
                {
                    _viewSkillInventory.enforceSkillBtn.gameObject.SetActive(false);
                    _viewSkillInventory.disassembleBtn.gameObject.SetActive(true);

                    if (Player.Skill.currentTouchedSkill.userSkilldata.Obtaincount>0)
                    {
                        _viewSkillInventory.disassembleInActive.gameObject.SetActive(false);
                    }
                    else
                    {
                        _viewSkillInventory.disassembleInActive.gameObject.SetActive(true);
                    }
                }
                else
                {
                    _viewSkillInventory.enforceSkillBtn.gameObject.SetActive(true);
                    _viewSkillInventory.enforceInActive.SetActive(!Player.Skill.currentTouchedSkill.canEnforce);
                }

                if (Player.Skill.currentTouchedSkill.userSkilldata.AwakeLv >= 2)
                {
                    _viewSkillInventory.awakeSkillBtn.enabled = false;
                    _viewSkillInventory.awakeInActive.SetActive(true);
                   
                }
                else
                {
                    if (Player.Skill.currentTouchedSkill.IsUnlocked)
                    {
                        _viewSkillInventory.awakeSkillBtn.enabled = true;
                        _viewSkillInventory.awakeInActive.SetActive(false);
                    }
                    else
                    {
                        _viewSkillInventory.awakeSkillBtn.enabled = false;
                        _viewSkillInventory.awakeInActive.SetActive(true);
                    }
                }
                _viewSkillInventory.skilllockImage.gameObject.SetActive(!Player.Skill.currentTouchedSkill.userSkilldata.Unlock);
            }

            bool isReddot = false;
            for (int i = 0; i < skillslotList.Count; i++)
            {
                if( skillslotList[i]._skillCache.userSkilldata.Obtaincount>= skillslotList[i]._skillCache.GetAmountForLevelUp())
                {
                    
                    isReddot = true;
                    break;
                }
            }

            ViewCanvas.Get<ViewCanvasMainNav>().viewMainNavButtons[(int)MainNavigationType.Skill].ActivateNotification(isReddot);

        }

        void EquipInduce(int _index)
        {
            int index = 0;
            foreach (var _key in Player.Cloud.userSkillEquipinfo.deckList[Player.Skill.CurrentSkillInvenindex].skills)
            {
                var equipedslot = _viewSkillInventory.equipedSlotList[index];
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
            for(int i=0;i< _viewSkillInventory.equipedSlotList.Length; i++)
            {
                _viewSkillInventory.equipedSlotList[i].StopEnduceaction();
            }
        }


        private void UpdateViewVisible()
        {
            if (MainNav.SelectTabIndex == _index)
            {
                if (_viewSkillInventory.IsVisible)
                {
                    _viewSkillInventory.blackBG.PopupCloseColorFade();
                    _viewSkillInventory.Wrapped.CommonPopupCloseAnimationDown(() => {
                        _viewSkillInventory.SetVisible(false);
                    });
                }
                else
                {
                    _viewSkillInventory.SetVisible(true);

                    _viewSkillInventory.blackBG.PopupOpenColorFade();
                    _viewSkillInventory.Wrapped.CommonPopupOpenAnimationUp(() => {
                        if (Player.Guide.currentTutorial == TutorialType.SkillAwake)
                        {
                            Player.Guide.StartTutorial(Player.Guide.currentTutorial);
                        }

                        if (Player.Guide.currentGuideQuest == QuestGuideType.SkillEquip)
                        {
                            Player.Guide.QuestGuideProgress(Player.Guide.currentGuideQuest);
                        }
                    });
                }
            }
            else
            {
                _viewSkillInventory.blackBG.PopupCloseColorFade();
                _viewSkillInventory.Wrapped.CommonPopupCloseAnimationDown(() => {
                    _viewSkillInventory.SetVisible(false);
                });
            }
        }

        void OpenAwakeDetailWindow()
        {
            int skillawakeUnlockLv = StaticData.Wrapper.ingameLockData[(int)LockedUIType.SkillAwakeQuestUnlock].unLockLevel;
            if (Player.Quest.mainQuestCurrentId <skillawakeUnlockLv)
            {
                LocalizeDescKeys localizedKey = LocalizeDescKeys.Etc_UnlockAfterMission;
                int levelValue = Model.Player.Option.ContentUIUnlockLevel(LockedUIType.SkillAwakeQuestUnlock);
                string localizedvalue = string.Format(StaticData.Wrapper.localizeddesclist[(int)localizedKey].StringToLocal, levelValue);
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);

                return;
            }

            AwakeDetailUIUpdate();
            _viewSkillInventory.AwakeDetailWindow.SetActive(true);
        }

        void AwakeDetailUIUpdate()
        {
            var _skillcache = Player.Skill.currentTouchedSkill;
            int skillvalueIndex = _skillcache.userSkilldata.AwakeLv + 1;

            string localizingdesc = StaticData.Wrapper.localizeddesclist[(int)_skillcache.tabledataSkill.SkillDesc[skillvalueIndex]].StringToLocal;

            string valueData = null;
            if (skillvalueIndex == 1)
            {
                if (_skillcache.tabledataSkill.skillStartValue_0.Length == 1)
                {
                    valueData = string.Format(localizingdesc, "N");
                    //valueData = string.Format(localizingdesc, _skillcache.SkillValue(skillvalueIndex));
                }
                else
                {
                    valueData = string.Format(localizingdesc, "N","N");
                    //valueData = string.Format(localizingdesc, _skillcache.SkillValue(skillvalueIndex), _skillcache.SkillValue(skillvalueIndex, 1));
                }

            }
            else if (skillvalueIndex == 2)
            {
                if (_skillcache.tabledataSkill.skillStartValue_1.Length == 1)
                {
                    valueData = string.Format(localizingdesc, "N");
                    //valueData = string.Format(localizingdesc, _skillcache.SkillValue(skillvalueIndex));
                }
                else
                {
                    valueData = string.Format(localizingdesc, "N", "N");
                    //valueData = string.Format(localizingdesc, _skillcache.SkillValue(skillvalueIndex), _skillcache.SkillValue(skillvalueIndex, 1));
                }
            }
         


            _viewSkillInventory.skillIcon.sprite = InGameResourcesBundle.Loaded.skillIcon[(int)_skillcache.tabledataSkill.skillKey];
            _viewSkillInventory.skillIconBG.sprite = InGameResourcesBundle.Loaded.weaponGradeInnerFrameSprite[(int)_skillcache.tabledataSkill.grade - 1];

            _viewSkillInventory.awakeStoneCost.text = string.Format(Player.Skill.currentTouchedSkill.GetAwakeStoneAmountForAwake().ToString());
            _viewSkillInventory.awakeDesc.text = string.Format(valueData);

            bool canAwake = Player.Skill.currentTouchedSkill.canAwake();

            if (Player.Cloud.playingRecord.isFreeSkillAwakeComplete == false)
            {
                if (Player.Skill.currentTouchedSkill.tabledataSkill.skillKey == SkillKey.GuidedMissile)
                {
                    _viewSkillInventory.awakeConfirmBtninDetail.enabled = true;
                    _viewSkillInventory.cantawakeBtnImageinDetail.SetActive(false);

                    string localizedesc_2 = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_FreeForBattlePass].StringToLocal;
                    _viewSkillInventory.awakeStoneCost.text = localizedesc_2;
                }
                else
                {
                    _viewSkillInventory.awakeConfirmBtninDetail.enabled = canAwake;
                    _viewSkillInventory.cantawakeBtnImageinDetail.SetActive(!canAwake);
                }
            }
            else
            {
                if (Player.Skill.currentTouchedSkill.userSkilldata.AwakeLv >= 2)
                {
                    _viewSkillInventory.awakeConfirmBtninDetail.enabled = false;
                }
                else
                {

                    _viewSkillInventory.awakeConfirmBtninDetail.enabled = canAwake;
                    _viewSkillInventory.cantawakeBtnImageinDetail.SetActive(!canAwake);
                }
            }
        }

        void CloseAwakeDetailWindow()
        {
            _viewSkillInventory.AwakeDetailWindow.SetActive(false);
        }
    }

}
