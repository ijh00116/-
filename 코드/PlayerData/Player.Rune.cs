using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Definition;
using UnityEngine.Events;
using System;
using BlackTree.Core;
using BlackTree.Bundles;

namespace BlackTree.Model
{
    public static partial class Player
    {
        public static class Rune
        {
            public class RuneCacheData
            {
                public readonly UserRuneInfo userRunedata;
                public readonly RuneTableData tabledata;

                public Dictionary<RuneAbilityKey, double> _equipabilitycaches;

                public bool isActivated = false;
                public int AwakeMaxLevel()
                {
                    return tabledata.awakeLevelList.Length;
                }

                public bool IsMaxLevel()
                {
                    return userRunedata.Obtainlv >= tabledata.maxLevel;
                }

                public ItemState CurrentItemState()
                {
                    ItemState tempstate = ItemState.normal;
                    //각성 조건 달성
                    if (userRunedata.Obtainlv == tabledata.maxLevel)
                    {
                        tempstate = ItemState.canCombine;
                    }
                    else
                    {
                        tempstate = ItemState.normal;
                    }

                    return tempstate;
                }

                public bool IsEquiped => currentEquipContainsSkill(tabledata.index);

                public int EquipedIndex = -1;

                public bool IsUnlocked => userRunedata.Obtaincount > 0 || userRunedata.Obtainlv > 0;
                public static bool currentEquipContainsSkill(int index)
                {
                    return currentRuneContainer().Contains(index);
                }

                public RuneCacheData(UserRuneInfo savedata, RuneTableData _tabledata)
                {
                    userRunedata = savedata;
                    this.tabledata = _tabledata;

                    if (userRunedata == null)
                    {
                        userRunedata = new UserRuneInfo();
                    }

                    _equipabilitycaches = new Dictionary<RuneAbilityKey, double>();

                    for (int i = 0; i < tabledata.equipabilitylist.Length; i++)
                    {
                        _equipabilitycaches.Add(tabledata.equipabilitylist[i], 0);
                    }


                    //SyncAbilityValue();
                }

                public void SyncAbilityValue()
                {
                    for (int i = 0; i < tabledata.equipabilitylist.Length; i++)
                    {
                        if (_equipabilitycaches.ContainsKey(tabledata.equipabilitylist[i]))
                        {
                            _equipabilitycaches[tabledata.equipabilitylist[i]] = 0;
                        }
                    }
                    for (int i = 0; i < tabledata.equipabilitylist.Length; i++)
                    {
                        if (_equipabilitycaches.ContainsKey(tabledata.equipabilitylist[i]))
                        {
                            double abilValue = tabledata.equipabilitystartvalue[i] + tabledata.equipabilitydeltavalue[i] * userRunedata.Obtainlv;
                            _equipabilitycaches[tabledata.equipabilitylist[i]] = abilValue;
                            
                        }
                    }
                }
            }


            public static Dictionary<int, RuneCacheData> runeCaches = new Dictionary<int, RuneCacheData>();
            public static Dictionary<RuneAbilityKey, double> _equipAbilitycaches;

            public static System.Action<int> onAfterEquip;
            public static System.Action<int> onAfterUnEquip;
            public static System.Action<int> onUpdateSync;

            public static Action<int> SlotTouched;
            public static int CurrentRuneInvenindex = 0;

            public static int currentEquipwaitRuneID;
            public static Action<int> equipslotEnduceForchange;
            public static Action equipslotEnduceOff;

            public static int companioncount;
            public static void Init()
            {
                CurrentRuneInvenindex = Cloud.runeEquipedInfo.currentEquipRuneIndex;

                for (int i = 0; i < Constraints.runeEquipPresetCount; i++)
                {
                    if (Cloud.runeEquipedInfo.deckList.Count <= i)
                    {
                        EquipedRuneList EquipSet = new EquipedRuneList();
                        Cloud.runeEquipedInfo.deckList.Add(EquipSet);
                    }
                }

                for (int i = Cloud.runeData.collection.Count; i < StaticData.Wrapper.runedatas.Length; i++)
                {
                    Cloud.runeData.collection.Add(new UserRuneInfo());
                }

                for (int i = 0; i < Cloud.runeData.collection.Count; i++)
                {
                    int index = i;
                    var data = StaticData.Wrapper.runedatas[i];
                    runeCaches.Add(index, new RuneCacheData(Cloud.runeData.collection[i],data));
                }

                int invenIndex = 0;
                foreach (var _runedata in Cloud.runeEquipedInfo.deckList)
                {
                    for (int i = 0; i < Constraints.runeEquipmax; i++)
                    {
                        int id = 0;
                        if (i >= _runedata.runes.Count)
                        {
                            _runedata.runes.Add(-1);
                            id = -1;
                        }
                        else
                        {
                            id = _runedata.runes[i];
                        }
                        if (invenIndex == Cloud.runeEquipedInfo.currentEquipRuneIndex)
                        {
                            if (id != -1)
                            {
                                Get(id).EquipedIndex = i;
                            }
                        }
                    }
                    invenIndex++;
                }

                _equipAbilitycaches = new Dictionary<RuneAbilityKey, double>();
                for (int i = 0; i < (int)RuneAbilityKey.END; i++)
                {
                    _equipAbilitycaches.Add((RuneAbilityKey)i, 0);
                }

                var currentEquipRuneList = currentRuneContainer();
                foreach (var _equipedID in currentEquipRuneList)
                {
                    if (_equipedID == -1)
                        continue;
                    var equipRune = Get(_equipedID);
                    for (int i = 0; i < equipRune.tabledata.equipabilitylist.Length; i++)
                    {
                        if (equipRune.IsEquiped)
                        {
                            var abiltype = equipRune.tabledata.equipabilitylist[i];
                            _equipAbilitycaches[abiltype] += equipRune._equipabilitycaches[abiltype];
                        }

                    }
                }
            }

            public static RuneCacheData Get(int id)
            {
                if (id == -1)
                    return null;
                return runeCaches[id];
            }
            public static List<int> currentRuneContainer()
            {
                return Cloud.runeEquipedInfo.deckList[Cloud.runeEquipedInfo.currentEquipRuneIndex].runes;
            }

            public static bool currentEquipContainsRune(int id)
            {
                return currentRuneContainer().Contains(id);
            }

            public static void Obtain(int _ID, int count)
            {
                var cache = Get(_ID);
                var obtainCount = cache.userRunedata.Obtaincount;

                cache.userRunedata.Obtaincount += count;

                Cloud.runeData.UpdateHash().SetDirty(true);
                LocalSaveLoader.SaveUserCloudData();

                SyncData(_ID);

                onUpdateSync?.Invoke(_ID);
            }

            public static void Equip(int _ID)
            {
                if (Get(_ID).IsEquiped)
                    return;
                int index = 0;
                bool empty = false;
                foreach (var _slot in currentRuneContainer())
                {
                    if (_slot == -1)
                    {
                        empty = true;
                        break;
                    }
                    index++;
                }

                if (HasEmptySlot() == false)
                {
                    currentEquipwaitRuneID= _ID;
                    equipslotEnduceForchange?.Invoke(index);
                    return;
                }

                var runedata = Get(_ID);

                if (index < Constraints.runeEquipmax && empty)
                {
                    var currentskilllist = currentRuneContainer();
                    currentskilllist[index] = _ID;
                    runedata.EquipedIndex = index;
                    onAfterEquip?.Invoke(_ID);
                }
                else
                {
                    currentEquipwaitRuneID = _ID;
                    equipslotEnduceForchange?.Invoke(index);
                }

                SyncData(_ID);

                //Player.Quest.TryCountUp(QuestType.EquipPet, 1);

                onUpdateSync(_ID);

                Cloud.runeData.UpdateHash().SetDirty(true);
                Cloud.runeEquipedInfo.UpdateHash().SetDirty(true);
                LocalSaveLoader.SaveUserCloudData();
            }

            public static void UnEquip(int _ID)
            {
                var _data = Get(_ID);

                int index = 0;
                foreach (var _slot in currentRuneContainer())
                {
                    if (_slot == _ID)
                    {
                        break;
                    }
                    index++;
                }
                _data.EquipedIndex = -1;
                Cloud.runeEquipedInfo.deckList[Cloud.runeEquipedInfo.currentEquipRuneIndex].runes[index] = -1;

                Cloud.runeEquipedInfo.UpdateHash().SetDirty(true);
                Cloud.runeData.UpdateHash().SetDirty(true);

                onUpdateSync(_ID);
            }

            public static void Enforce(int _ID)
            {
                var _data = Get(_ID);

                int amountForLvUP = StaticData.Wrapper.runeAmountTableData[_data.userRunedata.Obtainlv].amountForLvUp;

                _data.userRunedata.Obtaincount -= amountForLvUP;
                _data.userRunedata.Obtainlv++;

          
            }

            public static void Disassemble(int _ID)
            {
                var _data = Get(_ID);

                int eachRewardAmount = StaticData.Wrapper.runeDisassembleDatas[_data.tabledata.grade - 1].rewardAmount;
                int amount = _data.userRunedata.Obtaincount;

                double awakeAmount = eachRewardAmount * amount;

                Player.ControllerGood.Earn(GoodsKey.SkillAwakeStone, awakeAmount);
                _data.userRunedata.Obtaincount = 0;

                Cloud.runeData.UpdateHash().SetDirty(true);
                LocalSaveLoader.SaveUserCloudData();

                SyncData(_ID);

                onUpdateSync?.Invoke(_ID);
            }

            public static bool HasEmptySlot()
            {
                var container = currentRuneContainer();
                int index = 0;

                bool Empty = false;
                for (int i = 0; i < container.Count; i++)
                {
                    index = i;
                    var isunlock = Player.Option.IsRuneSlotUnlocked(index);
                    bool isEmpty = container[i] == -1;
                    if (isunlock == ContentState.UnLocked && isEmpty)
                    {
                        Empty = true;
                        break;
                    }
                }
                return Empty;
            }

            public static void SyncData(int index)
            {
                if (_equipAbilitycaches == null)
                    return;
                Get(index).SyncAbilityValue();

                SyncRuneAbility();
            }

            public static void SyncAllData()
            {
                if (_equipAbilitycaches == null)
                    return;
                foreach(var _data in runeCaches)
                {
                    _data.Value.SyncAbilityValue();
                }
                SyncRuneAbility();
            }

            static void SyncRuneAbility()
            {
                if (_equipAbilitycaches == null)
                    return;
                for (int i = 0; i < (int)RuneAbilityKey.END; i++)
                {
                    _equipAbilitycaches[(RuneAbilityKey)i] = 0;
                }


                var currentEquipList = currentRuneContainer();
                foreach (var _equipedID in currentEquipList)
                {
                    if (_equipedID == -1)
                        continue;
                    var equipRune = Get(_equipedID);
                    for (int i = 0; i < equipRune.tabledata.equipabilitylist.Length; i++)
                    {
                        if (equipRune.IsEquiped)
                        {
                            var abiltype = equipRune.tabledata.equipabilitylist[i];
                            switch (abiltype)
                            {
                                case RuneAbilityKey.SwordAttackIncrease:
                                case RuneAbilityKey.MagicAttackIncrease:
                                case RuneAbilityKey.SkillAttackIncrease:
                                case RuneAbilityKey.PetAttackIncrease:
                                case RuneAbilityKey.HpIncrease:
                                case RuneAbilityKey.ShieldIncrease:
                                case RuneAbilityKey.ShieldRecoverIncrease:
                                    _equipAbilitycaches[abiltype] += equipRune._equipabilitycaches[abiltype];
                                    break;
                                case RuneAbilityKey.MATKIncreaseWhenUseGuideMissileSkill:
                                    if (Player.Unit.IsSkillActive(Definition.SkillKey.GuidedMissile))
                                    {
                                        _equipAbilitycaches[abiltype] += equipRune._equipabilitycaches[abiltype];
                                    }
                                    break;
                                case RuneAbilityKey.SATKIncreaseWhenUseGodmodeSkill:
                                    if (Player.Unit.IsSkillActive(Definition.SkillKey.GodMode))
                                    {
                                        _equipAbilitycaches[abiltype] += equipRune._equipabilitycaches[abiltype];
                                    }
                                    break;
                                case RuneAbilityKey.SATKIncreaseWhenUseSummonSpawnSkill:
                                         _equipAbilitycaches[abiltype] += equipRune._equipabilitycaches[abiltype]*Player.Rune.companioncount;
                                    break;
                                case RuneAbilityKey.MATKIncreaseWhenUseLaserSkill:
                                    if (Player.Unit.IsSkillActive(Definition.SkillKey.LaserBeam))
                                    {
                                        _equipAbilitycaches[abiltype] += equipRune._equipabilitycaches[abiltype];
                                    }
                                    break;
                                case RuneAbilityKey.ThirdgradeSwordTotalLevelSwordtkIncrease:
                                    _equipAbilitycaches[abiltype] += Player.EquipItem.weaponTotalLevel[3]*equipRune._equipabilitycaches[abiltype];
                                    break;
                                case RuneAbilityKey.ThirdgradeStaffTotalLevelMagictkIncrease:
                                    _equipAbilitycaches[abiltype] += Player.EquipItem.staffTotalLevel[3] * equipRune._equipabilitycaches[abiltype];
                                    break;
                                case RuneAbilityKey.ThirdgradeArmorTotalLevelPettkIncrease:
                                    _equipAbilitycaches[abiltype] += Player.EquipItem.armorTotalLevel[3] * equipRune._equipabilitycaches[abiltype];
                                    break;
                                case RuneAbilityKey.ThirdgradePetTotalLevelSkillAtkIncrease:
                                    _equipAbilitycaches[abiltype] += Player.Pet.petTotalLevel[3] * equipRune._equipabilitycaches[abiltype];
                                    break;
                                case RuneAbilityKey.FourthgradeSwordTotalLevelSwordtkIncrease:
                                    _equipAbilitycaches[abiltype] += Player.EquipItem.weaponTotalLevel[4] * equipRune._equipabilitycaches[abiltype];
                                    break;
                                case RuneAbilityKey.FourthgradeStaffTotalLevelMagictkIncrease:
                                    _equipAbilitycaches[abiltype] += Player.EquipItem.staffTotalLevel[4] * equipRune._equipabilitycaches[abiltype];
                                    break;
                                case RuneAbilityKey.FourthgradeArmorTotalLevelPettkIncrease:
                                    _equipAbilitycaches[abiltype] += Player.EquipItem.armorTotalLevel[4] * equipRune._equipabilitycaches[abiltype];
                                    break;
                                case RuneAbilityKey.FourthgradePetTotalLevelSkillAtkIncrease:
                                    _equipAbilitycaches[abiltype] += Player.Pet.petTotalLevel[4] * equipRune._equipabilitycaches[abiltype];
                                    break;
                                case RuneAbilityKey.FifthgradeSwordTotalLevelSwordtkIncrease:
                                    _equipAbilitycaches[abiltype] += Player.EquipItem.weaponTotalLevel[5] * equipRune._equipabilitycaches[abiltype];
                                    break;
                                case RuneAbilityKey.FifthgradeStaffTotalLevelMagictkIncrease:
                                    _equipAbilitycaches[abiltype] += Player.EquipItem.staffTotalLevel[5] * equipRune._equipabilitycaches[abiltype];
                                    break;
                                case RuneAbilityKey.FifthgradeArmorTotalLevelPettkIncrease:
                                    _equipAbilitycaches[abiltype] += Player.EquipItem.armorTotalLevel[5] * equipRune._equipabilitycaches[abiltype];
                                    break;
                                case RuneAbilityKey.FifthgradePetTotalLevelSkillAtkIncrease:
                                    _equipAbilitycaches[abiltype] += Player.Pet.petTotalLevel[5] * equipRune._equipabilitycaches[abiltype];
                                    break;
                                case RuneAbilityKey.ThirdgradeSkillTotalLevelSwordtkIncrease:
                                    _equipAbilitycaches[abiltype] += Player.Skill.skillTotalLevel[3] * equipRune._equipabilitycaches[abiltype];
                                    break;
                                case RuneAbilityKey.FourthgradeSkillTotalLevelMagictkIncrease:
                                    _equipAbilitycaches[abiltype] += Player.Skill.skillTotalLevel[4] * equipRune._equipabilitycaches[abiltype];
                                    break;
                                case RuneAbilityKey.OneToTwocountForSkillAwake_AtkIncrease:
                                    _equipAbilitycaches[abiltype] += (Player.Skill.skillAwakeTotalLevel[1] + Player.Skill.skillAwakeTotalLevel[2]) * equipRune._equipabilitycaches[abiltype];
                                    break;
                                case RuneAbilityKey.END:
                                    break;
                                default:
                                    break;
                            }

          
                            //_equipAbilitycaches[abiltype] += equipRune._equipabilitycaches[abiltype];
                        }

                    }
                }

                Player.Unit.StatusSync();
            }

            static float[] randomRuneChance = new float[] { 0.5f, 0.3f, 0.15f, 0.05f };

            static int GetindexForChance(int arraylength)
            {
                float max = 0;
                for (int i = 0; i < arraylength; i++)
                {
                    max += randomRuneChance[i];
                }
                float x = UnityEngine.Random.Range(0f, max);

                float totalvalue = 0;
                int index = -1;
                for (int i = 0; i < randomRuneChance.Length; i++)
                {
                    totalvalue += randomRuneChance[i];
                    if (x < totalvalue)
                    {
                        index = i;
                        break;
                    }
                }
                return index;
            }
            public static int GetRandomindex()
            {
                int index = 0;
                int randomGrade = 0;
                int[] options;
                int itemindex = -1;
                int itemgradeIndex = 0;

                index = Cloud.runeData.summonLevel - 1;
                randomGrade = StaticData.Wrapper.runedataChance[index].GetindexForChance();

                options = StaticData.Wrapper.runedatas.Where(o => o.grade == randomGrade + 1).Select(o => o.index).ToArray();

                Array.Sort(options);
                itemgradeIndex = GetindexForChance(options.Length);
                itemindex = options[itemgradeIndex];

                return itemindex;
            }

            public static void UpdateExp(int exp)
            {
                if (Cloud.runeData.summonLevel >= StaticData.Wrapper.dataChance.Length)
                {
                    return;
                }
                Cloud.runeData.currentSummonExp += exp;
                var maxexp = StaticData.Wrapper.dataMaxExp[Cloud.runeData.summonLevel - 1].maxExp;
                if (Cloud.runeData.currentSummonExp >= maxexp)
                {
                    if (Cloud.runeData.summonLevel <= StaticData.Wrapper.dataMaxExp.Length)
                    {
                        Cloud.runeData.summonLevel++;
                        Cloud.runeData.currentSummonExp = Cloud.runeData.currentSummonExp - maxexp;
                    }
                }

                if (Cloud.runeData.summonLevel >= StaticData.Wrapper.dataChance.Length)
                {
                    Cloud.runeData.currentSummonExp = 0;
                }
                Cloud.runeData.UpdateHash().SetDirty(true);
            }
        }
    }
}
