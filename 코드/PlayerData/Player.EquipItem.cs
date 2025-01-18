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
        public static class Equip
        {
            public static EquipType currentselectEquipType;
            public static int currentSelectIdx;
            public const int CountForCombine =5;
        }
        
        public class EquipData
        {
            public EquipType equipType;
            public readonly UserEquipdata userEquipdata;
            public readonly EquipTableData tabledata;

            public Dictionary<EquipAbilityKey, double> _equipabilitycaches;
            public Dictionary<EquipAbilityKey, double> _possessabilitycaches;

            public int AwakeMaxLevel()
            {
                return tabledata.awakeLevelList.Length;
            }

            public bool IsMaxLevel()
            {
                return userEquipdata.Obtainlv >= tabledata.maxLevel;
            }

            public ItemState CurrentItemState()
            {
                ItemState tempstate = ItemState.normal;
                //각성 조건 달성
                if (userEquipdata.Obtainlv == tabledata.maxLevel)
                {
                    tempstate = ItemState.canCombine;
                }
                else
                {
                    tempstate = ItemState.normal;
                }
              
                return tempstate;
            }
            public bool IsEquiped { 
                get {
                    int index=-1;
                    switch (equipType)
                    {
                        case EquipType.Weapon:
                            index = Player.Cloud.weapondata.currentEquipIndex;
                            break;
                        case EquipType.Armor:
                            index = Player.Cloud.armordata.currentEquipIndex;
                            break;
                        case EquipType.Bow:
                            index = Player.Cloud.staffdata.currentEquipIndex;
                            break;
                        default:
                            break;
                    }
                    return tabledata.index==index;
                } 
            } 
            public bool IsUnlocked => userEquipdata.Obtaincount > 0 || userEquipdata.Obtainlv > 0;

            public EquipData(EquipType _equipType,UserEquipdata savedata, EquipTableData tabledata)
            {
                equipType = _equipType;
                userEquipdata = savedata;
                this.tabledata = tabledata;

                if (userEquipdata == null)
                {
                    userEquipdata = new UserEquipdata();
                }

                _equipabilitycaches = new Dictionary<EquipAbilityKey, double>();

                for (int i = 0; i < tabledata.equipabilitylist.Length; i++)
                {
                    _equipabilitycaches.Add(tabledata.equipabilitylist[i], 0);
                }

                _possessabilitycaches = new Dictionary<EquipAbilityKey, double>();

                for (int i = 0; i < tabledata.possessabilitylist.Length; i++)
                {
                    _possessabilitycaches.Add(tabledata.possessabilitylist[i], 0);
                }

                SyncAbilityValue();
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
                         double abilValue = tabledata.equipabilitystartvalue[i] + tabledata.equipabilitydeltavalue[i] * userEquipdata.Obtainlv;
                         _equipabilitycaches[tabledata.equipabilitylist[i]] = abilValue;
                    }
                }

                for (int i = 0; i < tabledata.possessabilitylist.Length; i++)
                {
                    if (_possessabilitycaches.ContainsKey(tabledata.possessabilitylist[i]))
                    {
                        _possessabilitycaches[tabledata.equipabilitylist[i]] = 0;
                    }
                }
                for (int i = 0; i < tabledata.possessabilitylist.Length; i++)
                {
                    if (_possessabilitycaches.ContainsKey(tabledata.possessabilitylist[i]))
                    {
                         double abilValue = tabledata.possessabilitystartvalue[i] + tabledata.possessabilitydeltavalue[i] * userEquipdata.Obtainlv;
                         _possessabilitycaches[tabledata.possessabilitylist[i]] = abilValue;
                    }
                }
            }
        }

        public static class EquipItem
        {
            public static Dictionary<EquipType, List<EquipData>> equipCaches = new Dictionary<EquipType, List<EquipData>>();
            public static Dictionary<EquipAbilityKey, double> _equipAbilitycaches; //key: 장비 토탈 능력치, value: 장비 능력치 값
            public static Dictionary<EquipAbilityKey, double> _possessAbilitycaches; //key: 장비 토탈 능력치, value: 장비 능력치 값

            public static Dictionary<EquipType, int> currentEquipIndex = new Dictionary<EquipType, int>();

            public static Action<EquipType,int> onUpdateSync;
            public static Action<EquipType,int> EquipedSlotTouched;

            public static int starterPackageItemIndex = 16;

            public static Dictionary<int, int> weaponTotalLevel = new Dictionary<int, int>();
            public static Dictionary<int, int> staffTotalLevel = new Dictionary<int, int>();
            public static Dictionary<int, int> armorTotalLevel = new Dictionary<int, int>();
            public static void Init()
            {
                currentEquipIndex.Add(EquipType.Weapon, Cloud.weapondata.currentEquipIndex);
                currentEquipIndex.Add(EquipType.Bow, Cloud.staffdata.currentEquipIndex);
                currentEquipIndex.Add(EquipType.Armor, Cloud.armordata.currentEquipIndex);
                

                for (int i = Cloud.weapondata.itemdatas.Count; i < StaticData.Wrapper.weapondatas.Length; i++)
                {
                    Cloud.weapondata.itemdatas.Add(new UserEquipdata());
                }
                for (int i = Cloud.armordata.itemdatas.Count; i < StaticData.Wrapper.armordatas.Length; i++)
                {
                    Cloud.armordata.itemdatas.Add(new UserEquipdata());
                }
                for (int i = Cloud.staffdata.itemdatas.Count; i < StaticData.Wrapper.staffdatas.Length; i++)
                {
                    Cloud.staffdata.itemdatas.Add(new UserEquipdata());
                }
                
                var weaponlist = new List<EquipData>();
                for (int i = 0; i < StaticData.Wrapper.weapondatas.Length; i++)
                {
                    var wd = new EquipData(EquipType.Weapon,Cloud.weapondata.itemdatas[i], StaticData.Wrapper.weapondatas[i]);
                    weaponlist.Add(wd);
                }
                equipCaches.Add(EquipType.Weapon, weaponlist);

                var armorList= new List<EquipData>();
                for (int i = 0; i < StaticData.Wrapper.armordatas.Length; i++)
                {
                    var wd = new EquipData(EquipType.Armor, Cloud.armordata.itemdatas[i], StaticData.Wrapper.armordatas[i]);
                    armorList.Add(wd);
                }
                equipCaches.Add(EquipType.Armor, armorList);

                var bowList = new List<EquipData>();
                for (int i = 0; i < StaticData.Wrapper.staffdatas.Length; i++)
                {
                    var wd = new EquipData(EquipType.Bow, Cloud.staffdata.itemdatas[i], StaticData.Wrapper.staffdatas[i]);
                    bowList.Add(wd);
                }
                equipCaches.Add(EquipType.Bow, bowList);


                _equipAbilitycaches = new Dictionary<EquipAbilityKey, double>();
                for(int i=0; i<(int)EquipAbilityKey.END; i++)
                {
                    _equipAbilitycaches.Add((EquipAbilityKey)i, 0);
                }

                _possessAbilitycaches = new Dictionary<EquipAbilityKey, double>();
                for (int i = 0; i < (int)EquipAbilityKey.END; i++)
                {
                    _possessAbilitycaches.Add((EquipAbilityKey)i, 0);
                }

                foreach (var _equipedData in currentEquipIndex)
                {
                    var equip = Get(_equipedData.Key, _equipedData.Value);
                    for (int i = 0; i < equip.tabledata.equipabilitylist.Length; i++)
                    {
                        if (equip.userEquipdata.AwakeLv >= i)
                        {
                            var abiltype = equip.tabledata.equipabilitylist[i];
                            _equipAbilitycaches[abiltype] += equip._equipabilitycaches[abiltype];
                        }
                    }
                }

                foreach (var _obtainItem in equipCaches)
                {
                    for (int i = 0; i < _obtainItem.Value.Count; i++)
                    {
                        var item = _obtainItem.Value[i];
                        if (item.IsUnlocked)
                        {
                            for (int j = 0; j < item.tabledata.possessabilitylist.Length; j++)
                            {
                                if (item.userEquipdata.AwakeLv >= j)
                                {
                                    var abiltype = item.tabledata.possessabilitylist[j];
                                    _possessAbilitycaches[abiltype] += item._possessabilitycaches[abiltype];
                                }
                                
                            }
                        }
                    }
                }

                for(int i=0; i<StaticData.Wrapper.weapondatas.Length; i++)
                {
                    if (weaponTotalLevel.ContainsKey(StaticData.Wrapper.weapondatas[i].grade)==false)
                    {
                        weaponTotalLevel.Add(StaticData.Wrapper.weapondatas[i].grade, 0);
                    }
                }
                for (int i = 0; i < StaticData.Wrapper.staffdatas.Length; i++)
                {
                    if (staffTotalLevel.ContainsKey(StaticData.Wrapper.staffdatas[i].grade) == false)
                    {
                        staffTotalLevel.Add(StaticData.Wrapper.staffdatas[i].grade, 0);
                    }
                }
                for (int i = 0; i < StaticData.Wrapper.armordatas.Length; i++)
                {
                    if (armorTotalLevel.ContainsKey(StaticData.Wrapper.armordatas[i].grade) == false)
                    {
                        armorTotalLevel.Add(StaticData.Wrapper.armordatas[i].grade, 0);
                    }
                }

               
            }

            public static void TotalLevelCalculate()
            {
                for (int i = 0; i < StaticData.Wrapper.weapondatas.Length; i++)
                {
                    if (weaponTotalLevel.ContainsKey(StaticData.Wrapper.weapondatas[i].grade) == false)
                    {
                        weaponTotalLevel.Add(StaticData.Wrapper.weapondatas[i].grade, 0);
                    }
                    else
                    {
                        weaponTotalLevel[StaticData.Wrapper.armordatas[i].grade] = 0;
                    }
                }
                for (int i = 0; i < StaticData.Wrapper.staffdatas.Length; i++)
                {
                    if (staffTotalLevel.ContainsKey(StaticData.Wrapper.staffdatas[i].grade) == false)
                    {
                        staffTotalLevel.Add(StaticData.Wrapper.staffdatas[i].grade, 0);
                    }
                    else
                    {
                        staffTotalLevel[StaticData.Wrapper.armordatas[i].grade] = 0;
                    }
                }
                for (int i = 0; i < StaticData.Wrapper.armordatas.Length; i++)
                {
                    if (armorTotalLevel.ContainsKey(StaticData.Wrapper.armordatas[i].grade) == false)
                    {
                        armorTotalLevel.Add(StaticData.Wrapper.armordatas[i].grade, 0);
                    }
                    else
                    {
                        armorTotalLevel[StaticData.Wrapper.armordatas[i].grade] = 0;
                    }
                }
                foreach (var _obtainItem in equipCaches)
                {
                    switch (_obtainItem.Key)
                    {
                        case EquipType.Weapon:
                            for (int i = 0; i < _obtainItem.Value.Count; i++)
                            {
                                var item = _obtainItem.Value[i];
                                if (item.IsUnlocked)
                                {
                                    if (item.userEquipdata.Obtainlv > 0)
                                    {
                                        weaponTotalLevel[item.tabledata.grade] += item.userEquipdata.Obtainlv;
                                    }
                                }
                            }
                            break;
                        case EquipType.Bow:
                            for (int i = 0; i < _obtainItem.Value.Count; i++)
                            {
                                var item = _obtainItem.Value[i];
                                if (item.IsUnlocked)
                                {
                                    if (item.userEquipdata.Obtainlv > 0)
                                    {
                                        staffTotalLevel[item.tabledata.grade] += item.userEquipdata.Obtainlv;
                                    }
                                }
                            }
                            break;
                        case EquipType.Armor:
                            for (int i = 0; i < _obtainItem.Value.Count; i++)
                            {
                                var item = _obtainItem.Value[i];
                                if (item.IsUnlocked)
                                {
                                    if (item.userEquipdata.Obtainlv > 0)
                                    {
                                        armorTotalLevel[item.tabledata.grade] += item.userEquipdata.Obtainlv;
                                    }
                                }
                            }
                            break;
                        case EquipType.END:
                            break;
                        default:
                            break;
                    }

                }
                Player.Rune.SyncAllData();
            }

            public static EquipData Get(EquipType _type,int index)
            {
                return equipCaches[_type][index];
            }

            public static void Obtain(EquipType _type,int index, int count)
            {
                var cache = Get(_type,index);
                var obtainCount = cache.userEquipdata.Obtaincount;

                cache.userEquipdata.Obtaincount += count;

                if (cache.userEquipdata.AwakeLv < cache.tabledata.awakeLevelList.Length)
                {
                    if (cache.userEquipdata.Obtainlv == cache.tabledata.awakeLevelList[cache.userEquipdata.AwakeLv])
                    {
                        cache.userEquipdata.AwakeLv++;
                    }
                }

                switch (_type)
                {
                    case EquipType.Weapon:
                        Cloud.weapondata.UpdateHash().SetDirty(true);
                        break;
                    case EquipType.Bow:
                        Cloud.staffdata.UpdateHash().SetDirty(true);
                        break;
                    case EquipType.Armor:
                        Cloud.armordata.UpdateHash().SetDirty(true);
                        break;
                    case EquipType.END:
                        break;
                    default:
                        break;
                }
    
                LocalSaveLoader.SaveUserCloudData();

                SyncData(_type, index); 

                onUpdateSync?.Invoke(_type,index);
            }

            public static void Equip(EquipType _type, int index)
            {
                currentEquipIndex[_type] = index;
                switch (_type)
                {
                    case EquipType.Weapon:
                        Cloud.weapondata.currentEquipIndex = index;
                        break;
                    case EquipType.Armor:
                        Cloud.armordata.currentEquipIndex = index;
                        break;
                    case EquipType.Bow:
                        Cloud.staffdata.currentEquipIndex = index;
                        break;
                    default:
                        break;
                }
                switch (_type)
                {
                    case EquipType.Weapon:
                        Cloud.weapondata.UpdateHash().SetDirty(true);
                        break;
                    case EquipType.Bow:
                        Cloud.staffdata.UpdateHash().SetDirty(true);
                        break;
                    case EquipType.Armor:
                        Cloud.armordata.UpdateHash().SetDirty(true);
                        break;
                    case EquipType.END:
                        break;
                    default:
                        break;
                }


                LocalSaveLoader.SaveUserCloudData();

                SyncData(_type,index);

                Player.Quest.TryCountUp(QuestType.EquipItemUse, 1);

                onUpdateSync?.Invoke(_type, index);
            }

            public static void Enforce(EquipType _type, int index)
            {
                var equipdata = Get(_type,index);

                if (equipdata.CurrentItemState() != ItemState.normal)
                {
                    return;
                }
                    
                
                equipdata.userEquipdata.Obtaincount -= StaticData.Wrapper.itemAmountTableData[equipdata.userEquipdata.Obtainlv].amountForLvUp;
                equipdata.userEquipdata.Obtainlv++;

                if (equipdata.userEquipdata.AwakeLv<equipdata.tabledata.awakeLevelList.Length)
                {
                    if (equipdata.userEquipdata.Obtainlv == equipdata.tabledata.awakeLevelList[equipdata.userEquipdata.AwakeLv])
                    {
                        equipdata.userEquipdata.AwakeLv++;
                    }
                }

            }

            public static void UpdateAfterEnforce(EquipType _type,int index)
            {
                var equipdata = Get(_type, index);

                equipdata.SyncAbilityValue();

                switch (_type)
                {
                    case EquipType.Weapon:
                        Cloud.weapondata.UpdateHash().SetDirty(true);
                        break;
                    case EquipType.Bow:
                        Cloud.staffdata.UpdateHash().SetDirty(true);
                        break;
                    case EquipType.Armor:
                        Cloud.armordata.UpdateHash().SetDirty(true);
                        break;
                    case EquipType.END:
                        break;
                    default:
                        break;
                }
                LocalSaveLoader.SaveUserCloudData();

                SyncData(_type, index);

                onUpdateSync?.Invoke(_type, index);
            }

    

            public static void SyncItemAbility()
            {
                for (int i = 0; i < (int)EquipAbilityKey.END; i++)
                {
                    _equipAbilitycaches[(EquipAbilityKey)i] = 0;
                }

                foreach(var _equipedData in currentEquipIndex)
                {
                    var equip = Get(_equipedData.Key, _equipedData.Value);
                    for (int i = 0; i < equip.tabledata.equipabilitylist.Length; i++)
                    {
                        if (equip.userEquipdata.AwakeLv >= i)
                        {
                            var abiltype = equip.tabledata.equipabilitylist[i];
                            _equipAbilitycaches[abiltype] += equip._equipabilitycaches[abiltype];
                        }
                            
                    }
                }

                for (int i = 0; i < (int)EquipAbilityKey.END; i++)
                {
                    _possessAbilitycaches[(EquipAbilityKey)i] = 0;
                }

                foreach (var _obtainItem in equipCaches)
                {
                    for(int i=0; i< _obtainItem.Value.Count; i++)
                    {
                        var item = _obtainItem.Value[i];
                        if(item.IsUnlocked)
                        {
                            for (int j = 0; j < item.tabledata.possessabilitylist.Length; j++)
                            {
                                if (item.userEquipdata.AwakeLv >= j)
                                {
                                    var abiltype = item.tabledata.possessabilitylist[j];
                                    _possessAbilitycaches[abiltype] += item._possessabilitycaches[abiltype];
                                }
                            }
                        }
                    }
                }

                Player.Unit.StatusSync();
            }

            public static void SyncTotalView(EquipType _type)
            {
                foreach (var _equipdata in equipCaches[_type])
                {
                    onUpdateSync(_type, _equipdata.tabledata.index);
                }
            }

            public static void SyncData(EquipType _type, int index)
            {
                Get(_type,index).SyncAbilityValue();

                SyncItemAbility();
            }

            static float[] randomHeroChance = new float[] { 0.5f, 0.3f, 0.15f, 0.05f};

            static int GetindexForChance(int arraylength)
            {
                float max = 0;
                for(int i=0; i< arraylength; i++)
                {
                    max += randomHeroChance[i];
                }
                float x = UnityEngine.Random.Range(0f, max);

                float totalvalue = 0;
                int index = -1;
                for (int i = 0; i < randomHeroChance.Length; i++)
                {
                    totalvalue += randomHeroChance[i];
                    if (x < totalvalue)
                    {
                        index = i;
                        break;
                    }
                }
                return index;
            }
 
            public static int GetRandomindex(EquipType _type)
            {
                int index = 0;
                int randomGrade = 0;
                int[] options=null;
                int itemindex = -1;
                int itemgradeIndex = 0;
                switch (_type)
                {
                    case EquipType.Weapon:
                        index = Cloud.weapondata.summonLevel - 1;
                        randomGrade = StaticData.Wrapper.dataChance[index].GetindexForChance();
                        options = StaticData.Wrapper.weapondatas.Where(o => o.grade == randomGrade + 1).Select(o => o.index).ToArray();
                        break;
                    case EquipType.Bow:
                        index = Cloud.weapondata.summonLevel - 1;
                        randomGrade = StaticData.Wrapper.dataChance[index].GetindexForChance();
                        options = StaticData.Wrapper.staffdatas.Where(o => o.grade == randomGrade + 1).Select(o => o.index).ToArray();
                        break;
                    case EquipType.Armor:
                        index = Cloud.weapondata.summonLevel - 1;
                        randomGrade = StaticData.Wrapper.dataChance[index].GetindexForChance();
                        options = StaticData.Wrapper.armordatas.Where(o => o.grade == randomGrade + 1).Select(o => o.index).ToArray();
                        break;
                    case EquipType.END:
                        break;
                    default:
                        break;
                }

                Array.Sort(options);
                itemgradeIndex = GetindexForChance(options.Length);
                itemindex = options[itemgradeIndex];

                return itemindex;
            }

            public static void UpdateExp(int exp)
            {
                if(Cloud.weapondata.summonLevel>=StaticData.Wrapper.dataChance.Length)
                {
                    return;
                }
                Cloud.weapondata.currentSummonExp += exp;
                var maxexp= StaticData.Wrapper.dataMaxExp[Cloud.weapondata.summonLevel - 1].maxExp;
                if (Cloud.weapondata.currentSummonExp>= maxexp)
                {
                    if (Cloud.weapondata.summonLevel<=StaticData.Wrapper.dataMaxExp.Length)
                    {
                        Cloud.weapondata.summonLevel++;
                        Cloud.weapondata.currentSummonExp = Cloud.weapondata.currentSummonExp-maxexp;
                    }
                }

                if (Cloud.weapondata.summonLevel >= StaticData.Wrapper.dataChance.Length)
                {
                    Cloud.weapondata.currentSummonExp = 0;
                }

                Cloud.weapondata.UpdateHash().SetDirty(true);
                LocalSaveLoader.SaveUserCloudData();
            }

           
        }
    }
}
