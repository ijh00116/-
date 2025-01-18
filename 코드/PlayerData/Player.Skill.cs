using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Definition;
using UnityEngine.Events;
using System;
using BlackTree.Core;

namespace BlackTree.Model
{
    public static partial class Player
    {
        public static class Skill
        {
            public static Dictionary<SkillKey, SkillCacheData> skillCaches = new Dictionary<SkillKey, SkillCacheData>();
            private static Dictionary<SkillAbilityKey, double> passiveskillValue = new Dictionary<SkillAbilityKey, double>();

            public static System.Action<SkillKey> onAfterEquip;
            public static System.Action<SkillKey> onAfterUnEquip;
            public static System.Action<SkillKey> onUpdateSync;

            public static System.Action changePresetUpdate;

            public static UnityAction<SkillKey> SlotTouched;
            public static UnityAction<SkillKey> EquipedSlotTouched;

            public static System.Action<SkillKey,SkillKey> UpdateSkillCacheDataInEquipedSlot;

            public static System.Action<SkillKey,bool> SkillActivate;
            //deck
            public static int CurrentSkillInvenindex = 0;

            //skill관련
            public static HashSet<int> _inputSkillSlotIndexes = new HashSet<int>();

            public static SkillKey currentEquipwaitSkill;
            public static UnityAction<int> equipslotEnduceForchange;
            public static UnityAction equipslotEnduceOff;

            //awake
            public static ControllerSkillSubUnit skillSubUnitObject;
            public static UnityAction subUnitOffCallback;

            public static SkillCacheData currentTouchedSkill;

            public static List<double> skillDamageList = new List<double>();
            public static double currentBestDmgIngame;

            public static Dictionary<int, int> skillTotalLevel = new Dictionary<int, int>();
            public static Dictionary<int, int> skillAwakeTotalLevel = new Dictionary<int, int>();
            public class SkillCacheData
            {
                public readonly SkillTableData tabledataSkill;
                public readonly UserSkillInfo userSkilldata;
                public bool waitForuseSkill=false;
                public bool IsUnlocked => userSkilldata.Obtaincount > 0 || userSkilldata.level > 1;
                public bool canEnforce => userSkilldata.Obtaincount >= GetAmountForLevelUp();

                public float SkillValue(int awakeindex,int skillValueIndex=0)
                {
                    int skillLevel = userSkilldata.level;

                    float _value = 0;
                    if (awakeindex == 0)
                    {
                        _value= tabledataSkill.skillStartValue_0[skillValueIndex] + tabledataSkill.skillDeltaValue_0[skillValueIndex] * skillLevel;
                    }
                    else if (awakeindex == 1)
                    {
                        if(userSkilldata.AwakeLv>=1)
                        {
                            _value = tabledataSkill.skillStartValue_1[skillValueIndex] + tabledataSkill.skillDeltaValue_1[skillValueIndex] * skillLevel;
                        }
                        else
                        {
                            _value = 0;
                        }
                    }
                    else
                    {
                        if (userSkilldata.AwakeLv >= 2)
                        {
                            _value = tabledataSkill.skillStartValue_2[skillValueIndex] + tabledataSkill.skillDeltaValue_2[skillValueIndex] * skillLevel;
                        }
                        else
                        {
                            _value = 0;
                        }   
                    }

                    return _value;
                }
               

                public int GetAmountForLevelUp()
                {
                    int amount= StaticData.Wrapper.skillAmountTableData[userSkilldata.level-1].amountForLvUp;
                    
                    return amount;
                }

                public bool isMaxLevel()
                {
                    return userSkilldata.level >= tabledataSkill.maxLevel;
                }

                public int GetAwakeStoneAmountForAwake()
                {
                    int amount = 0;
                    if (userSkilldata.AwakeLv==0)
                    {
                        amount = StaticData.Wrapper.skillAwakeTableData[tabledataSkill.grade - 1].needAwakeAmount_0;
                    }
                    else
                    {
                        amount = StaticData.Wrapper.skillAwakeTableData[tabledataSkill.grade - 1].needAwakeAmount_1;
                    }
                    
                    return amount;
                }

                public bool canAwake()
                {
                    bool possible = Player.Good.Get(GoodsKey.SkillAwakeStone) >= GetAwakeStoneAmountForAwake();
                    return possible;
                }

                public float leftCooltime { 
                    get {
                        float lefttime = 0;
                        lefttime = StaticData.Wrapper.skillDatas[(int)tabledataSkill.skillKey].coolTime - elapsedCooltime;
                        return lefttime;
                    } }
                public float elapsedCooltime
                {
                    get
                    {
                        float lefttime = 0;
                        if (Battle.Field.IsMainSceneState)
                        {
                            lefttime = userSkilldata.elapsedCooltime;
                        }
                        else
                        {
                            lefttime = userSkilldata.elapsedCooltimeinContent;
                        }
                        return lefttime;
                    }
                    set
                    {
                        if (Battle.Field.IsMainSceneState)
                        {
                            userSkilldata.elapsedCooltime = value;
                        }
                        else
                        {
                            userSkilldata.elapsedCooltimeinContent = value;
                        }
                    }
                }

                public bool IsEquiped => currentEquipContainsSkill(tabledataSkill.skillKey);

                public int EquipedIndex = -1;


                public SkillCacheData(SkillTableData tabledata,UserSkillInfo userCloudData)
                {
                    tabledataSkill = tabledata;
                    userSkilldata = userCloudData;
                }
            }

            public static List<SkillKey> currentSkillContainer()
            {
                return Cloud.userSkillEquipinfo.deckList[Cloud.userSkillEquipinfo.currentEquipSkillIndex].skills;
            }

            public static bool currentEquipContainsSkill(SkillKey _key)
            {
                return currentSkillContainer().Contains(_key);
            }
            public static void Init()
            {
                currentEquipwaitSkill = SkillKey.None;

                CurrentSkillInvenindex = Cloud.userSkillEquipinfo.currentEquipSkillIndex;

                for(int i=0; i<Constraints.skillEquipPresetCount; i++)
                {
                    if(Cloud.userSkillEquipinfo.deckList.Count<=i)
                    {
                        EquipedSkillList newskillEquipSet = new EquipedSkillList();
                        Cloud.userSkillEquipinfo.deckList.Add(newskillEquipSet);
                    }
                }

                for(int i=Cloud.skilldata.skillUnlockState.Count; i<StaticData.Wrapper.skillLockTabledata.Length; i++)
                {
                    Cloud.skilldata.skillUnlockState.Add(false);
                }

                for(int i=Cloud.skilldata.collection.Count; i<StaticData.Wrapper.skillDatas.Length; i++)
                {
                    Cloud.skilldata.collection.Add(new UserSkillInfo());
                }
                for(int i=0; i<Cloud.skilldata.collection.Count; i++)
                {
                    var data = StaticData.Wrapper.skillDatas[i];
                    skillCaches.Add(data.skillKey, new SkillCacheData(data,Cloud.skilldata.collection[i]));
                }

                int invenIndex = 0;
                foreach(var _skilldata in Cloud.userSkillEquipinfo.deckList)
                {
                    for(int i=0; i<Constraints.skillEquipmax; i++)
                    {
                        SkillKey keydata = SkillKey.None;
                        if(i>=_skilldata.skills.Count)
                        {
                            _skilldata.skills.Add(SkillKey.None);
                            keydata = SkillKey.None;
                        }
                        else
                        {
                            keydata = _skilldata.skills[i];
                        }
                        if (invenIndex == Cloud.userSkillEquipinfo.currentEquipSkillIndex)
                        {
                            //currentEquipedskillContainer.Add(i, keydata);
                            if (keydata != SkillKey.None)
                            {
                                Get(keydata).EquipedIndex = i;
                            }
                        }
                    }
                    invenIndex++;
                }

                //initiallize

                for(int i=0; i<(int)SkillAbilityKey.End; i++)
                {
                    if (passiveskillValue.ContainsKey((SkillAbilityKey)i))
                    {
                        passiveskillValue[(SkillAbilityKey)i] = 0;
                    }
                    else
                    {
                        passiveskillValue.Add((SkillAbilityKey)i, 0);
                    }
                }

                for (int i = 0; i < (int)SkillKey.End; i++)
                {
                    skillDamageList.Add(0);
                }
                skillDamageList.Add(0);
                currentBestDmgIngame = 0;

                for (int i = 0; i < StaticData.Wrapper.skillDatas.Length; i++)
                {
                    if (skillTotalLevel.ContainsKey(StaticData.Wrapper.skillDatas[i].grade) == false)
                    {
                        skillTotalLevel.Add(StaticData.Wrapper.skillDatas[i].grade, 0);
                    }
                    if (skillAwakeTotalLevel.ContainsKey(StaticData.Wrapper.skillDatas[i].grade) == false)
                    {
                        skillAwakeTotalLevel.Add(StaticData.Wrapper.skillDatas[i].grade, 0);
                    }
                }
                //TotalLevelCalculate();
            }

            public static void TotalLevelCalculate()
            {

                for (int i = 0; i < StaticData.Wrapper.skillDatas.Length; i++)
                {
                    if (skillTotalLevel.ContainsKey(StaticData.Wrapper.skillDatas[i].grade) == false)
                    {
                        skillTotalLevel.Add(StaticData.Wrapper.skillDatas[i].grade, 0);
                    }
                    else
                    {
                        skillTotalLevel[StaticData.Wrapper.skillDatas[i].grade] = 0;
                    }

                    if (skillAwakeTotalLevel.ContainsKey(StaticData.Wrapper.skillDatas[i].grade) == false)
                    {
                        skillAwakeTotalLevel.Add(StaticData.Wrapper.skillDatas[i].grade, 0);
                    }
                    else
                    {
                        skillAwakeTotalLevel[StaticData.Wrapper.skillDatas[i].grade] = 0;
                    }
                }

                foreach (var _obtainskill in skillCaches)
                {
                    var item = _obtainskill.Value;
                    if (item.IsUnlocked)
                    {
                        if (item.userSkilldata.level > 0)
                        {
                            skillTotalLevel[item.tabledataSkill.grade] += item.userSkilldata.level;
                        }
                        if (item.userSkilldata.AwakeLv > 0)
                        {
                            skillAwakeTotalLevel[item.tabledataSkill.grade] += item.userSkilldata.AwakeLv;
                        }
                    }
                }
                Player.Rune.SyncAllData();
            }

            public static void SkillCoolTimeInit()
            {
                foreach(var skillcache in skillCaches)
                {
                    if(skillcache.Value.IsEquiped)
                    {
                        skillcache.Value.userSkilldata.elapsedCooltimeinContent = skillcache.Value.tabledataSkill.coolTime;
                    }
                }
            }

            public static SkillCacheData Get(SkillKey key)
            {
                if (key == SkillKey.None)
                    return null;
                return skillCaches[key];
            }

  
            public static int GetRandomindex()
            {
                var index = Cloud.skilldata.summonLevel - 1;
                int randomGrade = StaticData.Wrapper.skilldataChance[index].GetindexForChance();
                var options = StaticData.Wrapper.skillDatas.Where(o => o.grade == randomGrade + 1).Select(o => o.index).ToArray();

                var weaponindex = options[UnityEngine.Random.Range(0, options.Length)];

                return weaponindex;
            }

            public static void ChangeCurrentSkillPreset(int index)
            {
                if (index >= Constraints.skillEquipPresetCount)
                    return;

                List<SkillKey> skillKeyList = new List<SkillKey>();
                SkillKey currentKey;
                bool sameskillExist = false;
                for (int i=0; i<currentSkillContainer().Count; i++)
                {
                    currentKey = currentSkillContainer()[i];
                    if (currentKey == SkillKey.None)
                        continue;

                    sameskillExist = false;
                    for (int j=0; j< Cloud.userSkillEquipinfo.deckList[index].skills.Count; j++)
                    {
                        if(currentSkillContainer()[i]== Cloud.userSkillEquipinfo.deckList[index].skills[j])
                        {
                            sameskillExist = true;
                            break;
                        }
                    }
                    if(sameskillExist==false)
                    {
                        skillKeyList.Add(currentKey);
                    }
                }

                for(int i=0; i< skillKeyList.Count; i++)
                {
                    Player.Skill.SkillActivate?.Invoke(skillKeyList[i], false);
                }

                Cloud.userSkillEquipinfo.currentEquipSkillIndex = index;
             
                changePresetUpdate?.Invoke();
            }

            public static void Obtain(SkillKey key,int count)
            {
                var skilldata = Get(key);
                
                skilldata.userSkilldata.Obtaincount += count;
                if(skilldata.userSkilldata.Unlock==false)
                {
                    skilldata.userSkilldata.Unlock = true;
                    skilldata.elapsedCooltime= skilldata.tabledataSkill.coolTime;
                }
                //localsave
                //user ability sync
                //패시브인 경우 바로 적용
                UpdateValue();

                Cloud.skilldata.UpdateHash().SetDirty(true);
                LocalSaveLoader.SaveUserCloudData();

                onUpdateSync(key);
            }

            public static void Enforce(SkillKey key)
            {
                var skilldata = Get(key);
                if (skilldata.userSkilldata.level >= skilldata.tabledataSkill.maxLevel)
                    return;

                skilldata.userSkilldata.Obtaincount -= skilldata.GetAmountForLevelUp();
                skilldata.userSkilldata.level++;

                EventPurchaseSkillPopup(skilldata);

                //localsave
                //user ability sync
                //패시브인 경우 바로 적용
                UpdateValue();

                Cloud.skilldata.UpdateHash().SetDirty(true);
                LocalSaveLoader.SaveUserCloudData();

                onUpdateSync(key);

                TotalLevelCalculate();
            }

            public static void DisAssemble(SkillKey key)
            {
                var skilldata = Get(key);
                if (skilldata.userSkilldata.level < skilldata.tabledataSkill.maxLevel)
                    return;

                int eachRewardAmount = StaticData.Wrapper.skillDisassembleDatas[currentTouchedSkill.tabledataSkill.grade - 1].rewardAmount;
                int amount = currentTouchedSkill.userSkilldata.Obtaincount;

                double skillawakeAmount = eachRewardAmount * amount;

                Player.ControllerGood.Earn(GoodsKey.SkillAwakeStone, skillawakeAmount);
                currentTouchedSkill.userSkilldata.Obtaincount = 0;

                onUpdateSync(key);
            }

            public static void AllEnforceWithoutSync()
            {
                List<SkillKey> purchasePopupEventSkilllist = new List<SkillKey>();

                for(int i=0; i<(int)SkillKey.End; i++)
                {
                    var skillkey = (SkillKey)i;
                    var skilldata = Get(skillkey);

                    while (true)
                    {
                        if (skilldata.userSkilldata.Obtaincount < skilldata.GetAmountForLevelUp())
                        {
                            break;
                        }
                        if(skilldata.isMaxLevel())
                        {
                            break;
                        }
                        skilldata.userSkilldata.Obtaincount -= skilldata.GetAmountForLevelUp();
                        skilldata.userSkilldata.level++;

                        EventPurchaseSkillPopup(skilldata);
                    }

               

                    UpdateSynce(skillkey);
                }

                Cloud.skilldata.UpdateHash().SetDirty(true);
                LocalSaveLoader.SaveUserCloudData();

                TotalLevelCalculate();
            }

            static void EventPurchaseSkillPopup(SkillCacheData skilldata)
            {
                SkillKey key = skilldata.tabledataSkill.skillKey;
                if (skilldata.tabledataSkill.grade <= 2)
                {
                    if (skilldata.userSkilldata.level >= 5)
                    {
                        if (Bundles.ViewCanvas.Get<Bundles.ViewCanvasPackagePopup>().IsVisible == false)
                        {
                            Player.Products.skillPopup?.Invoke(key);
                        }
                    }
                }
                else if (skilldata.tabledataSkill.grade <= 3)
                {
                    if (skilldata.userSkilldata.level >= 4)
                    {
                        if (Bundles.ViewCanvas.Get<Bundles.ViewCanvasPackagePopup>().IsVisible == false)
                        {
                            Player.Products.skillPopup?.Invoke(key);
                        }
                    }
                }
                else if (skilldata.tabledataSkill.grade <= 4)
                {
                    if (skilldata.userSkilldata.level >= 3)
                    {
                        if (Bundles.ViewCanvas.Get<Bundles.ViewCanvasPackagePopup>().IsVisible == false)
                        {
                            Player.Products.skillPopup?.Invoke(key);
                        }
                    }

                }
                else if (skilldata.tabledataSkill.grade <= 5)
                {
                    if (skilldata.userSkilldata.level >= 2)
                    {
                        if (Bundles.ViewCanvas.Get<Bundles.ViewCanvasPackagePopup>().IsVisible == false)
                        {
                            Player.Products.skillPopup?.Invoke(key);
                        }
                    }
                }
                else if (skilldata.tabledataSkill.grade <= 6)
                {
                    if (skilldata.userSkilldata.level >= 2)
                    {
                        if (Bundles.ViewCanvas.Get<Bundles.ViewCanvasPackagePopup>().IsVisible == false)
                        {
                            Player.Products.skillPopup?.Invoke(key);
                        }
                    }
                }
                else if (skilldata.tabledataSkill.grade <= 7)
                {
                    if (skilldata.userSkilldata.level >= 2)
                    {
                        if (Bundles.ViewCanvas.Get<Bundles.ViewCanvasPackagePopup>().IsVisible == false)
                        {
                            Player.Products.skillPopup?.Invoke(key);
                        }
                    }
                }
            }

            public static void UpdateSynce(SkillKey key)
            {
                UpdateValue();

                onUpdateSync(key);
            }

            public static void Awake(SkillKey key)
            {
                var skilldata = Get(key);

                if(key==SkillKey.GuidedMissile && Player.Cloud.playingRecord.isFreeSkillAwakeComplete == false)
                {
                    Player.Cloud.playingRecord.isFreeSkillAwakeComplete = true;
                }
                else
                {
                    Player.ControllerGood.Consume(GoodsKey.SkillAwakeStone, skilldata.GetAwakeStoneAmountForAwake());
                }
                
                skilldata.userSkilldata.AwakeLv++;
                Player.Quest.TryCountUp(QuestType.SkillAwake, 1);

                UpdateValue();

                Cloud.skilldata.UpdateHash().SetDirty(true);
                LocalSaveLoader.SaveUserCloudData();

                onUpdateSync(key);
            }

            public static void UpdateValue()
            {
                for(int i=0; i<(int)SkillAbilityKey.End; i++)
                {
                    passiveskillValue[(SkillAbilityKey)i] = 0;
                }

                for (int i = 0; i < StaticData.Wrapper.skillDatas.Length; i++)
                {
                    if (StaticData.Wrapper.skillDatas[i].skillType == SkillType.Active)
                        continue;
                    var skilldata = Get(StaticData.Wrapper.skillDatas[i].skillKey);

                    for (int j = 0; j < skilldata.tabledataSkill.skillAbilityList_0.Length; j++)
                    {
                        var skillabilType = skilldata.tabledataSkill.skillAbilityList_0[j];
                        if (skillabilType == SkillAbilityKey.None)
                            continue;
                        if (passiveskillValue.ContainsKey(skillabilType))
                        {
                            passiveskillValue[skillabilType] += skilldata.SkillValue(0, j);
                        }
                        else
                        {
                            passiveskillValue.Add(skillabilType, 0);
                        }
                    }
                    for (int j = 0; j < skilldata.tabledataSkill.skillAbilityList_1.Length; j++)
                    {
                        var skillabilType = skilldata.tabledataSkill.skillAbilityList_1[j];
                        if (skillabilType == SkillAbilityKey.None)
                            continue;
                        if (passiveskillValue.ContainsKey(skillabilType))
                        {
                            passiveskillValue[skillabilType] += skilldata.SkillValue(1, j);
                        }
                        else
                        {
                            passiveskillValue.Add(skillabilType, 0);
                        }
                    }
                    for (int j = 0; j < skilldata.tabledataSkill.skillAbilityList_2.Length; j++)
                    {
                        var skillabilType = skilldata.tabledataSkill.skillAbilityList_2[j];
                        if (skillabilType == SkillAbilityKey.None)
                            continue;
                        if (passiveskillValue.ContainsKey(skillabilType))
                        {
                            passiveskillValue[skillabilType] += skilldata.SkillValue(2, j);
                        }
                        else
                        {
                            passiveskillValue.Add(skillabilType, 0);
                        }
                    }
                }
            }

            public static bool HasEmptySlot()
            {
                var container = currentSkillContainer();
                int index = 0;

                bool Empty=false;
                for(int i=0; i< container.Count; i++)
                {
                    index = i;
                    var isunlock=Player.Option.IsSkillSlotUnlocked(index);
                    bool isEmpty = container[i] == SkillKey.None;
                    if(isunlock==ContentState.UnLocked&& isEmpty)
                    {
                        Empty = true;
                        break;
                    }
                }

                return Empty;
            }

            public static void Equip(SkillKey key)
            {
                if (key == SkillKey.None)
                {
                    return;
                }

                int index = 0;
                bool empty = false;
                foreach (var _slot in currentSkillContainer())
                {
                    if (_slot == SkillKey.None)
                    {
                        empty = true;
                        break;
                    }
                    index++;
                }

                if (HasEmptySlot() == false)
                {
                    //Debug.Log("<color=red>빈 스킬 슬롯 없음</color>");
                    currentEquipwaitSkill = key;
                    equipslotEnduceForchange?.Invoke(index);
                    return;
                }
                    
                
                var skilldata = Get(key);

                if (index < Constraints.skillEquipmax && empty)
                {
                    var currentskilllist = currentSkillContainer();
                    currentskilllist[index] = key;
                    skilldata.EquipedIndex = index;
                    //임시 쿨타임 제거
                    
                    onAfterEquip?.Invoke(key);
                }
                else
                {
                    currentEquipwaitSkill = key;
                    equipslotEnduceForchange?.Invoke(index);
                    //스킬 인벤 꽉참 스킬 장착 유도 이벤트 만들기
                }

                onUpdateSync(key);

                Cloud.skilldata.UpdateHash().SetDirty(true);
                Cloud.userSkillEquipinfo.UpdateHash().SetDirty(true);
                LocalSaveLoader.SaveUserCloudData();


                Player.Quest.TryCountUp(QuestType.EquipSkill, 1);
            }

            public static void UnEquip(SkillKey key)
            {
                if (key == SkillKey.None)
                    return;
                var skilldata = Get(key);

                int index = 0;
                foreach (var _slot in currentSkillContainer())
                {
                    if (_slot == key)
                    {
                        break;
                    }
                    index++;
                }
                skilldata.EquipedIndex = -1;
                Cloud.userSkillEquipinfo.deckList[Cloud.userSkillEquipinfo.currentEquipSkillIndex].skills[index] = SkillKey.None;

                Cloud.userSkillEquipinfo.UpdateHash().SetDirty(true);

                Player.Skill.SkillActivate?.Invoke(key, false);

                onUpdateSync(key);
            }

            public static void Switch(SkillKey key)
            {
                UnEquip(key);

                UpdateSkillCacheDataInEquipedSlot?.Invoke(key,currentEquipwaitSkill);

                Equip(currentEquipwaitSkill);

                equipslotEnduceOff?.Invoke();

                currentEquipwaitSkill = SkillKey.None;
            }

       
            public static void RegisterSkillInput(int index)
            {
                if(!_inputSkillSlotIndexes.Contains(index))
                {
                    _inputSkillSlotIndexes.Add(index);
                }
            }
            public static HashSet<int> GetAllRegisterSkillInput()
            {
                return _inputSkillSlotIndexes;
            }

            public static void RemoveSkillInput(int key)
            {
                _inputSkillSlotIndexes.Remove(key);
            }

            static bool IsEmptySlotInEquipedSkillSlots()
            {
                int index = 0;
                bool empty = false;
                foreach (var _slot in currentSkillContainer())
                {
                    if (_slot == SkillKey.None)
                    {
                        empty = true;
                        break;
                    }
                    index++;
                }

                if (index < Constraints.skillEquipmax && empty)
                {
                    return true;
                }
                return false;
            }

            public static void UpdateExp(int exp)
            {
                if (Cloud.skilldata.summonLevel >= StaticData.Wrapper.skilldataChance.Length)
                {
                    return;
                }
                Cloud.skilldata.currentSummonExp += exp;
                var maxexp = StaticData.Wrapper.dataMaxExp[Cloud.skilldata.summonLevel - 1].maxExp;
                if (Cloud.skilldata.currentSummonExp > maxexp)
                {
                    if (Cloud.skilldata.summonLevel <= StaticData.Wrapper.dataMaxExp.Length)
                    {
                        Cloud.skilldata.summonLevel++;
                        Cloud.skilldata.currentSummonExp = Cloud.skilldata.currentSummonExp - maxexp;
                    }
                }

                if (Cloud.skilldata.summonLevel >= StaticData.Wrapper.skilldataChance.Length)
                {
                    Cloud.skilldata.currentSummonExp = 0;
                }

                Cloud.skilldata.UpdateHash().SetDirty(true);
                LocalSaveLoader.SaveUserCloudData();
            }

            public static SkillKey RewardToSkill(RewardTypes _type)
            {
                SkillKey skillkey = SkillKey.None;

                switch (_type)
                {
                    case RewardTypes.None:
                        break;
                    case RewardTypes.package_swordfewhit:
                        skillkey = SkillKey.SwordFewHitFire;
                        break;
                    case RewardTypes.package_magicfewhit:
                        skillkey = SkillKey.MagicFewHitFire;
                        break;
                    case RewardTypes.package_setturret:
                        skillkey = SkillKey.SetTurret;
                        break;
                    case RewardTypes.package_companionspawn:
                        skillkey = SkillKey.CompanionSpawn;
                        break;
                    case RewardTypes.package_guidedmissile:
                        skillkey = SkillKey.GuidedMissile;
                        break;
                    case RewardTypes.package_godmode:
                        skillkey = SkillKey.GodMode;
                        break;
                    case RewardTypes.package_summon:
                        skillkey = SkillKey.SummonSubunit;
                        break;
                    case RewardTypes.package_nova:
                        skillkey = SkillKey.NoveForSeconds;
                        break;
                    case RewardTypes.package_meteor:
                        skillkey = SkillKey.SpawnMeteor;
                        break;
                    case RewardTypes.package_multielectric:
                        skillkey = SkillKey.MultipleElectric;
                        break;
                    case RewardTypes.package_skyLight:
                        skillkey = SkillKey.SkyLight;
                        break;
                    default:
                        break;
                }

                return skillkey;
            }

        }
    }
}
