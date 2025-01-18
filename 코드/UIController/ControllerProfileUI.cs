using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using BlackTree.Definition;
using Cysharp.Threading.Tasks;
using System;

namespace BlackTree.Core
{
    public class ControllerProfileUI
    {
        enum DetailAbilityType
        {
            Total,Gold_0,Gold_1,Stat_0,Awake,Equip,Pet,Advance,Research_0, Research_1,Etc,End
        }
        private ViewCanvasProfile _viewProfile;
        private CancellationTokenSource _cts;

        private readonly int _index = 5;

        DateTime unlocktime;

        List<ViewAbilityDesc> normalAbilDescList = new List<ViewAbilityDesc>();
        List<ViewAbilityDesc> goldAbilDescList = new List<ViewAbilityDesc>();
        List<ViewAbilityDesc> goldsecondAbilDescList = new List<ViewAbilityDesc>();
        List<ViewAbilityDesc> statAbilDescList = new List<ViewAbilityDesc>();
        List<ViewAbilityDesc> awakeAbilDescList = new List<ViewAbilityDesc>();
        List<ViewAbilityDesc> itemAbilDescList = new List<ViewAbilityDesc>();
        List<ViewAbilityDesc> petAbilDescList = new List<ViewAbilityDesc>();
        List<ViewAbilityDesc> advanceAbilDescList = new List<ViewAbilityDesc>();
        List<ViewAbilityDesc> researchAbilDescList = new List<ViewAbilityDesc>();
        List<ViewAbilityDesc> researchSecondAbilDescList = new List<ViewAbilityDesc>();
        List<ViewAbilityDesc> shieldAbilDescatGhostList = new List<ViewAbilityDesc>();
        public ControllerProfileUI(Transform parent, CancellationTokenSource cts)
        {
            _cts = cts;
            _viewProfile = ViewCanvas.Create<ViewCanvasProfile>(parent);

            for(int i=0; i< _viewProfile.closeButton.Length; i++)
            {
                int index = i;
                _viewProfile.closeButton[index].onClick.AddListener(() => {
                    _viewProfile.blackBG.PopupCloseColorFade();
                    _viewProfile.Wrapped.CommonPopupCloseAnimationUp(() => {
                        _viewProfile.SetVisible(false);
                    });
                });
            }
            

            _viewProfile.BindOnChangeVisible(o =>
            {
                if (o)
                {
                    SetProfileWindow();
                }
            });

            spriteindex = 0;

            Player.Level.callbackGetExp += SetProfileWindow;

            unlocktime = Extension.GetDateTimeByIsoString(Model.Player.Cloud.optiondata.nicknameChangeUnlocktime);
            Main().Forget();
            TimeUpdate().Forget();

            _viewProfile.openNicknameChangeBtn.onClick.AddListener(()=> { _viewProfile.nicknameChangeWindow.SetActive(true); });
            _viewProfile.confirmNicknamewindowBtn.onClick.AddListener(NicknameChange);

            for(int i=0; i< _viewProfile.closeNicknamewindowBtn.Length; i++)
            {
                _viewProfile.closeNicknamewindowBtn[i].onClick.AddListener(() => { _viewProfile.nicknameChangeWindow.SetActive(false); });
            }

            _viewProfile.openDetailWindowBtn.onClick.AddListener(() => {
                if (Player.Guide.currentGuideQuest == QuestGuideType.OpenProfileDetail)
                {
                    Player.Guide.QuestGuideProgress(Player.Guide.currentGuideQuest);
                }
                Player.Quest.TryCountUp(QuestType.OpenProfileDetail, 1);

                for (int i = 0; i < (int)AwakeUpgradeKey.End; i++)
                {
                    awakeAbilDescList[i].Init((AwakeUpgradeKey)i);
                }
                for (int i = 0; i < (int)ShieldAbilityTypes.End; i++)
                {
                    shieldAbilDescatGhostList[i].Init((ShieldAbilityTypes)i);
                }

                for (int i = 0; i < (int)CharacterAbilityTypes.End; i++)
                {
                    normalAbilDescList[i].Init((CharacterAbilityTypes)i);
                }

                for (int i = 0; i < (int)GoldUpgradeKey.End; i++)
                {
                    goldAbilDescList[i].Init((GoldUpgradeKey)i);
                }

                for (int i = 0; i < (int)StatAbilityTypes.End; i++)
                {
                    statAbilDescList[i].Init((StatAbilityTypes)i);
                }
                for (int i = 0; i < (int)AwakeUpgradeKey.End; i++)
                {
                    awakeAbilDescList[i].Init((AwakeUpgradeKey)i);
                }

                for (int i = 0; i < (int)ItemAbilityTypes.End; i++)
                {
                    itemAbilDescList[i].Init((ItemAbilityTypes)i);
                }

                for (int i = 0; i < (int)PetAbilityTypes.End; i++)
                {
                    petAbilDescList[i].Init((PetAbilityTypes)i);
                }

                int index = 0;
                for (int i = 0; i < (int)AdvancementAbilityKey.End; i++)
                {
                    if (i == (int)AdvancementAbilityKey.IncreaseSwordAttack_2 || i == (int)AdvancementAbilityKey.IncreaseBowAttack_2 || i == (int)AdvancementAbilityKey.IncreaseMaxHp_2 ||
                  i == (int)AdvancementAbilityKey.IncreaseMaxShield_2 || i == (int)AdvancementAbilityKey.IncreasePetDmg_2 || i == (int)AdvancementAbilityKey.IncreaseMegaDmg_2)
                    {
                        
                        continue;
                    }

                    advanceAbilDescList[index].Init((AdvancementAbilityKey)i);
                    index++;
                }

                for (int i = 0; i < (int)(ResearchUpgradeKey.SwordAttackIncrease_2)-1; i++)
                {
                  
                    researchAbilDescList[i].Init((ResearchUpgradeKey)(i+1));
                }

                index = 0;
                for (int i = (int)ResearchUpgradeKey.SwordAttackIncrease_2; i < (int)ResearchUpgradeKey.End; i++)
                {
                    researchSecondAbilDescList[index].Init((ResearchUpgradeKey)i);
                    index++;
                }

                for (int i = 0; i < (int)ShieldAbilityTypes.End; i++)
                {
                    shieldAbilDescatGhostList[i].Init((ShieldAbilityTypes)i);
                }

                _viewProfile.detailAbilityWindow.SetActive(true);
                _viewProfile.descContent.SetContentScrollOffsetToTop();
            });

            for (int i = 0; i < _viewProfile.closeDetailWindowBtn.Length; i++)
            {
                _viewProfile.closeDetailWindowBtn[i].onClick.AddListener(() => { _viewProfile.detailAbilityWindow.SetActive(false); });
            }

            _viewProfile.uiNickname.text = Player.Cloud.optiondata.nickname;

            for(int i=0; i<(int)CharacterAbilityTypes.End; i++)
            {
                var obj = UnityEngine.Object.Instantiate(_viewProfile.abilSlotPrefab);
                obj.transform.SetParent(_viewProfile.AbilityDescScrollArray[(int)DetailAbilityType.Total], false);

                obj.Init((CharacterAbilityTypes)i);
                normalAbilDescList.Add(obj);
            }

            for (int i = 0; i < (int)GoldUpgradeKey.End; i++)
            {
                var obj = UnityEngine.Object.Instantiate(_viewProfile.abilSlotPrefab);
                obj.transform.SetParent(_viewProfile.AbilityDescScrollArray[(int)DetailAbilityType.Gold_0], false);

                obj.Init((GoldUpgradeKey)i);
                goldAbilDescList.Add(obj);
            }

            for (int i = 0; i < (int)Tier2GoldUpgradeKey.End; i++)
            {
                var obj = UnityEngine.Object.Instantiate(_viewProfile.abilSlotPrefab);
                obj.transform.SetParent(_viewProfile.AbilityDescScrollArray[(int)DetailAbilityType.Gold_1], false);

                obj.Init((Tier2GoldUpgradeKey)i);
                goldsecondAbilDescList.Add(obj);
            }

            for (int i = 0; i < (int)StatAbilityTypes.End; i++)
            {
                var obj = UnityEngine.Object.Instantiate(_viewProfile.abilSlotPrefab);
                obj.transform.SetParent(_viewProfile.AbilityDescScrollArray[(int)DetailAbilityType.Stat_0], false);

                obj.Init((StatAbilityTypes)i);
                statAbilDescList.Add(obj);
            }
            for (int i = 0; i < (int)AwakeUpgradeKey.End; i++)
            {
                var obj = UnityEngine.Object.Instantiate(_viewProfile.abilSlotPrefab);
                obj.transform.SetParent(_viewProfile.AbilityDescScrollArray[(int)DetailAbilityType.Awake], false);

                obj.Init((AwakeUpgradeKey)i);
                awakeAbilDescList.Add(obj);
            }

            for (int i = 0; i < (int)ItemAbilityTypes.End; i++)
            {
                var obj = UnityEngine.Object.Instantiate(_viewProfile.abilSlotPrefab);
                obj.transform.SetParent(_viewProfile.AbilityDescScrollArray[(int)DetailAbilityType.Equip], false);

                obj.Init((ItemAbilityTypes)i);
                itemAbilDescList.Add(obj);
            }

            for (int i = 0; i < (int)PetAbilityTypes.End; i++)
            {
                var obj = UnityEngine.Object.Instantiate(_viewProfile.abilSlotPrefab);
                obj.transform.SetParent(_viewProfile.AbilityDescScrollArray[(int)DetailAbilityType.Pet], false);

                obj.Init((PetAbilityTypes)i);
                petAbilDescList.Add(obj);
            }

          
            for (int i = 0; i < (int)AdvancementAbilityKey.End; i++)
            {
                if(i==(int)AdvancementAbilityKey.IncreaseSwordAttack_2|| i == (int)AdvancementAbilityKey.IncreaseBowAttack_2 || i == (int)AdvancementAbilityKey.IncreaseMaxHp_2 ||
                    i == (int)AdvancementAbilityKey.IncreaseMaxShield_2|| i == (int)AdvancementAbilityKey.IncreasePetDmg_2 || i == (int)AdvancementAbilityKey.IncreaseMegaDmg_2)
                {
                    continue;
                }
                var obj = UnityEngine.Object.Instantiate(_viewProfile.abilSlotPrefab);
                obj.transform.SetParent(_viewProfile.AbilityDescScrollArray[(int)DetailAbilityType.Advance], false);

                obj.Init((AdvancementAbilityKey)i);
                advanceAbilDescList.Add(obj);
            }

            for (int i = 1; i < (int)ResearchUpgradeKey.SwordAttackIncrease_2; i++)
            {
                if((ResearchUpgradeKey)i==ResearchUpgradeKey.ExpandResearchSlot)
                {
                    continue;
                }
                var obj = UnityEngine.Object.Instantiate(_viewProfile.abilSlotPrefab);
                obj.transform.SetParent(_viewProfile.AbilityDescScrollArray[(int)DetailAbilityType.Research_0], false);

                obj.Init((ResearchUpgradeKey)i);
                researchAbilDescList.Add(obj);
            }

            for (int i = (int)ResearchUpgradeKey.SwordAttackIncrease_2; i < (int)ResearchUpgradeKey.End; i++)
            {
                var obj = UnityEngine.Object.Instantiate(_viewProfile.abilSlotPrefab);
                obj.transform.SetParent(_viewProfile.AbilityDescScrollArray[(int)DetailAbilityType.Research_1], false);

                obj.Init((ResearchUpgradeKey)i);
                researchSecondAbilDescList.Add(obj);
            }

            for (int i = 0; i < (int)ShieldAbilityTypes.End; i++)
            {
                var obj = UnityEngine.Object.Instantiate(_viewProfile.abilSlotPrefab);
                obj.transform.SetParent(_viewProfile.AbilityDescScrollArray[(int)DetailAbilityType.Etc], false);

                obj.Init((ShieldAbilityTypes)i);
                shieldAbilDescatGhostList.Add(obj);
            }
        }


        int spriteindex = 0;
        async UniTaskVoid Main()
        {
            while (true)
            {
                if (_viewProfile.IsVisible)
                {
                    _viewProfile.unitImage.sprite = InGameResourcesBundle.Loaded.unitUIAnimSprite[spriteindex];
                    spriteindex++;
                    if (spriteindex >= InGameResourcesBundle.Loaded.unitUIAnimSprite.Length)
                    {
                        spriteindex = 0;
                    }
                }
                await UniTask.DelayFrame(4);
            }
        }
        async UniTaskVoid TimeUpdate()
        {
            while (true)
            {
                if(_viewProfile.IsVisible)
                {
                    var time = System.DateTime.Now;
                    var lefttime = unlocktime - time;
                    string localizedesc = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_ChangePossible].StringToLocal;
                    string timetext = string.Format("{0:D2}:{1:D2}:{2:D2} "+localizedesc, lefttime.Hours, lefttime.Minutes, lefttime.Seconds);

                    if(lefttime.TotalSeconds<=0)
                    {
                        if (_viewProfile.cantNicknameChangeObj.activeInHierarchy)
                            _viewProfile.cantNicknameChangeObj.SetActive(false);
                        string localized = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_NickNameChangePossible].StringToLocal;
                        _viewProfile.leftTime.text = localized;
                    }
                    else
                    {
                        if (_viewProfile.cantNicknameChangeObj.activeInHierarchy==false)
                            _viewProfile.cantNicknameChangeObj.SetActive(true);
                        _viewProfile.leftTime.text = timetext;
                    }
                    
                }
        
                await UniTask.Delay(1000);
            }
        }
        public void SetProfileWindow()
        {
            _viewProfile.level.text = Player.Cloud.userLevelData.currentLevel.ToString();
            _viewProfile.exp.text = string.Format("{0}/{1}", ((double)Player.Level.currentExp).ToNumberString(), ((double)Player.Level.currentmaxExp).ToNumberString());
            _viewProfile.swordAtk.text = Player.Unit.SwordAtk.ToNumberString();
            _viewProfile.bowAtk.text = Player.Unit.BowAtk.ToNumberString();
            //_viewProfile.magicAtkDesc.text = string.Format("(<color=#EF4E3A>-{0:N0}%</color>)", Player.Unit.defaultBowRateValue*100); 
            _viewProfile.stage.text =string.Format("{0}-{1}", Battle.Field.CurrentFieldChapter+1, Battle.Field.CurrentFieldStage+1);

            foreach(var equipinfo in Player.EquipItem.currentEquipIndex)
            {
                var item= Player.EquipItem.Get(equipinfo.Key, equipinfo.Value);
                _viewProfile.equiplist[(int)equipinfo.Key].SetHideImage(item.IsUnlocked)
                    .SetLevel(item.userEquipdata.Obtainlv)
                    .SetObtainCount(item)
                    .SetEquip(false)
                    .SetGrade(item.tabledata.grade)
                    .SetupstaticData(item);
            }

            var skilllist = Player.Skill.currentSkillContainer();
            for(int i=0; i< skilllist .Count; i++)
            {
                if(skilllist[i]!= SkillKey.None)
                {
                    var skillCache=Player.Skill.Get(skilllist[i]);
                    _viewProfile.skillslotList[i].skillIcon.gameObject.SetActive(true);
                    _viewProfile.skillslotList[i].skillIcon.sprite = InGameResourcesBundle.Loaded.skillIcon[(int)skillCache.tabledataSkill.skillKey];
                    _viewProfile.skillslotList[i].lockedImage.SetActive(!skillCache.userSkilldata.Unlock);
                    _viewProfile.skillslotList[i].lvText.gameObject.SetActive(skillCache.userSkilldata.Unlock);
                    _viewProfile.skillslotList[i].lvText.text = string.Format("Lv.{0}", skillCache.userSkilldata.level);
                    for (int j = 0; j < _viewProfile.skillslotList[i].gradestars.Length; j++)
                    {
                        _viewProfile.skillslotList[i].gradestars[j].SetActive(j < skillCache.tabledataSkill.grade);
                    }
                }
                else
                {
                    _viewProfile.skillslotList[i].skillIcon.gameObject.SetActive(false);
                    _viewProfile.skillslotList[i].lockedImage.SetActive(true);
                    _viewProfile.skillslotList[i].lvText.gameObject.SetActive(false);
                    for (int j = 0; j < _viewProfile.skillslotList[i].gradestars.Length; j++)
                    {
                        _viewProfile.skillslotList[i].gradestars[j].SetActive(false);
                    }
                }
            }
        }

        void NicknameChange()
        {
            if(_viewProfile.cantNicknameChangeObj.activeInHierarchy)
            {
                string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_CantChange].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
            }
            else
            {
                _viewProfile.confirmNicknamewindowBtn.enabled = false;
                string changedNickname = _viewProfile.inputfield.text;
                var bro = BackEnd.Backend.BMember.UpdateNickname(changedNickname);

                if (bro.IsSuccess())
                {
                    string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_NickChanged].StringToLocal;
                    ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);

                    var datetime = DateTime.Now.AddHours(12);
                    string nowtime = datetime.ToIsoString();
                    Model.Player.Cloud.optiondata.nicknameChangeUnlocktime = nowtime;
                    Model.Player.Cloud.optiondata.nickname = changedNickname;

                    unlocktime = Extension.GetDateTimeByIsoString(Model.Player.Cloud.optiondata.nicknameChangeUnlocktime);

                    Model.Player.Cloud.optiondata.SetDirty(true).UpdateHash();

                    _viewProfile.uiNickname.text = changedNickname;

                    Player.SaveUserDataToFirebaseAndLocal().Forget();
                }
                else
                {
                    string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_OtherNickExist].StringToLocal;
                    ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                }

                _viewProfile.confirmNicknamewindowBtn.enabled = true;
            }
            
            
        }

    }
}
