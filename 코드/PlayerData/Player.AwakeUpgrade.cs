using System;
using System.Collections;
using System.Collections.Generic;
using BlackTree.Core;
using BlackTree.Definition;
using UnityEngine.Events;

namespace BlackTree.Model
{
    public static partial class Player
    {
        public static class AwakeUpgrade
        {
            public class AwakeupgradeValueForLvType
            {
                public int lv;
                public double cost;
            }

            private static Dictionary<AwakeUpgradeKey, UnityAction> _onChangeMap = new Dictionary<AwakeUpgradeKey, UnityAction>();
            public static UnityAction<AwakeUpgradeKey> UpdateSyncactions;
            public static UnityAction OnResetUpgrade;

            private static Dictionary<AwakeUpgradeKey, double> _upgradeValue = new Dictionary<AwakeUpgradeKey, double>();

            public static int[] awakeGradeMaxLevel = new int[] {50,100,150,200,250,300 };

            public static UnityAction<int> onAfterAdvanceUpgrade;
            public static UnityAction<int> onAfterAdvanceChanged;

            public static Dictionary<AdvancementAbilityKey, double> _advanceAbilityData;

            public static bool canUpgrade = false;
            public static void Init()
            {
                _upgradeValue.Clear();

                for (int i = Cloud.awakeUpgrade.upgradeLevels.Count; i < StaticData.Wrapper.awakeUpgradedatas.Length; i++)
                {
                    Cloud.awakeUpgrade.upgradeLevels.Add(0);
                }

                for (int i = 0; i < Cloud.awakeUpgrade.upgradeLevels.Count; i++)
                {
                    var dataValue = GetValue((AwakeUpgradeKey)i);
                    _upgradeValue.Add((AwakeUpgradeKey)i, dataValue);
                }

                //advance
                for(int i=Cloud.userAdvancedata.AdvanceInfo.Count; i<StaticData.Wrapper.advancementDatas.Length; i++)
                {
                    AdvanceInfo advanceInfo = new AdvanceInfo();
                    Cloud.userAdvancedata.AdvanceInfo.Add(advanceInfo);
                }

                bool ismaxlv = true;
                for (int i = 0; i < StaticData.Wrapper.awakeUpgradedatas.Length; i++)
                {
                    AwakeUpgradeKey key = StaticData.Wrapper.awakeUpgradedatas[i].key;
                    ismaxlv = Player.AwakeUpgrade.GetIsMaxLevel(key);
                    if (ismaxlv == false)
                        break;
                }

                bool isAdvanceGrade_3 = false;
                if(Cloud.userAdvancedata.Grade == 3)
                {
                    for (int i = 0; i < StaticData.Wrapper.advancementDatas.Length; i++)
                    {
                        if (StaticData.Wrapper.advancementDatas[i].grade == Cloud.userAdvancedata.Grade)
                        {
                            if (Cloud.userAdvancedata.AdvanceInfo[i].isAdvanced)
                            {
                                isAdvanceGrade_3 = true;
                                break;
                            }
                        }
                    }
                }
                if(isAdvanceGrade_3)
                {
                    Cloud.userAdvancedata.Grade++;
                }
               
                _advanceAbilityData = new Dictionary<AdvancementAbilityKey, double>();

                for(int i=0; i<(int)AdvancementAbilityKey.End; i++)
                {
                    _advanceAbilityData.Add((AdvancementAbilityKey)i, 0);
                }

                AdvanceDataSync();
                Cloud.awakeUpgrade.UpdateHash();

            
            }

            public static void Upgrade(AwakeUpgradeKey key)
            {
                CheckHash(Cloud.awakeUpgrade);
                var datavalue = Cloud.awakeUpgrade.upgradeLevels[(int)key]++;

                Player.Quest.TryCountUp(QuestType.AwakeUpgrade, 1);

                _upgradeValue[key] = datavalue;

                if (_onChangeMap.ContainsKey(key))
                    _onChangeMap[key]?.Invoke();

                UpdateSyncactions?.Invoke(key);

                UpdateSyncData();
            }

            public static void Upgrade(AwakeUpgradeKey key, int Count)
            {
                CheckHash(Cloud.awakeUpgrade);
                var datavalue = Cloud.awakeUpgrade.upgradeLevels[(int)key]+= Count;

                Player.Quest.TryCountUp(QuestType.AwakeUpgrade, Count);

                _upgradeValue[key] = datavalue;

                if (_onChangeMap.ContainsKey(key))
                    _onChangeMap[key]?.Invoke();

                UpdateSyncactions?.Invoke(key);

                UpdateSyncData();
            }

            public static void TemporaryUpgrade(AwakeUpgradeKey key)
            {
                var datavalue = Cloud.awakeUpgrade.upgradeLevels[(int)key]++;

                Player.Quest.TryCountUp(QuestType.AwakeUpgrade, 1);

                _upgradeValue[key] = datavalue;

                if (_onChangeMap.ContainsKey(key))
                    _onChangeMap[key]?.Invoke();

                UpdateSyncactions?.Invoke(key);

            }

            public static void TemporaryUpgrade(AwakeUpgradeKey key, int Count)
            {
                var datavalue = Cloud.awakeUpgrade.upgradeLevels[(int)key]+= Count;

                Player.Quest.TryCountUp(QuestType.AwakeUpgrade, Count);

                _upgradeValue[key] = datavalue;

                if (_onChangeMap.ContainsKey(key))
                    _onChangeMap[key]?.Invoke();

                UpdateSyncactions?.Invoke(key);
            }

            public static void UpdateSyncData()
            {
                Cloud.awakeUpgrade
                 .UpdateHash()
                 .SetDirty(true);

                Player.Unit.StatusSync();

                LocalSaveLoader.SaveUserCloudData();
            }

            
            public static void Advance(int index)
            {
                int grade = StaticData.Wrapper.advancementDatas[index].grade;
                //if (grade>Player.Cloud.userAdvancedata.Grade)
                //{
                //    return;
                //}
                for (int i=0; i < StaticData.Wrapper.advancementDatas.Length; i++)
                {
                    if(grade== StaticData.Wrapper.advancementDatas[i].grade)
                    {
                        Cloud.userAdvancedata.AdvanceInfo[i].isAdvanced = false;
                    }
                }

                Cloud.userAdvancedata.AdvanceInfo[index].isAdvanced = true;
                //능력치 추가
                AdvanceDataSync();
                //능력치 추가
                Cloud.userAdvancedata.Grade++;
                if (Cloud.userAdvancedata.Grade >= awakeGradeMaxLevel.Length)
                {
                    Cloud.userAdvancedata.Grade = awakeGradeMaxLevel.Length - 1;
                }
                
                onAfterAdvanceUpgrade?.Invoke(index);
                Player.Unit.StatusSync();

                Cloud.userAdvancedata.UpdateHash().SetDirty(true);
                LocalSaveLoader.SaveUserCloudData();
            }

            public static void AdvanceDataSync()
            {
                for (int i = 0; i < (int)AdvancementAbilityKey.End; i++)
                {
                    _advanceAbilityData[(AdvancementAbilityKey)i] = 0;
                }

                for (int i = 0; i < Cloud.userAdvancedata.AdvanceInfo.Count; i++)
                {
                    var advanceData = Cloud.userAdvancedata.AdvanceInfo[i];
                    var tabledata = StaticData.Wrapper.advancementDatas[i];
                    if (advanceData.isAdvanced)
                    {
                        for (int j = 0; j < tabledata.abilityType.Length; j++)
                        {
                            _advanceAbilityData[tabledata.abilityType[j]] += tabledata.abilityValue[j];
                        }
                    }
                }
            }
            public static void ChangeAdvance(int index)
            {
               
                int grade = StaticData.Wrapper.advancementDatas[index].grade;
                for (int i = 0; i < StaticData.Wrapper.advancementDatas.Length; i++)
                {
                    if (grade == StaticData.Wrapper.advancementDatas[i].grade)
                    {
                        Cloud.userAdvancedata.AdvanceInfo[i].isAdvanced = false;
                    }
                }

                Cloud.userAdvancedata.AdvanceInfo[index].isAdvanced = true;
                //능력치 추가
                AdvanceDataSync();
                //능력치 추가
                onAfterAdvanceUpgrade?.Invoke(index);
                onAfterAdvanceChanged?.Invoke(index);

                Player.Unit.StatusSync();

                Cloud.userAdvancedata.UpdateHash().SetDirty(true);
                LocalSaveLoader.SaveUserCloudData();
            }

            public static int GetMaxLevel(AwakeUpgradeKey key)
            {
                int maxlevel = awakeGradeMaxLevel[Cloud.userAdvancedata.Grade];
                return maxlevel;
            }

            public static bool GetIsMaxLevel(AwakeUpgradeKey key)
            {
                var id = StaticData.Wrapper.awakeUpgradedatas[(int)key].sequenceId;
                var currLv = GetLevel(key);

                return currLv >= GetMaxLevel(key);
            }

            public static int GetLevel(AwakeUpgradeKey key)
            {
                return Cloud.awakeUpgrade.upgradeLevels[(int)key];
            }
            public static AwakeupgradeValueForLvType GetMaxCostLv(AwakeUpgradeKey key)
            {
                AwakeupgradeValueForLvType tempdata = new AwakeupgradeValueForLvType();
                int lv = GetLevel(key);
                double maxCost = 0;
                while (true)
                {
                    if (lv >= GetMaxLevel(key))
                    {
                        break;
                    }

                    maxCost += GetCost(key, lv);
                    lv++;

                    if (maxCost + GetCost(key,lv) > Player.Good.Get(GoodsKey.AwakeStone))
                        break;
                }

                tempdata.lv = lv;
                tempdata.cost = maxCost;

                return tempdata;
            }

            public static AwakeupgradeValueForLvType GetCostLv(AwakeUpgradeKey key, int count)
            {
                AwakeupgradeValueForLvType tempdata = new AwakeupgradeValueForLvType();
                int lv = GetLevel(key);
                double maxCost = 0;
                int _count = 0;
                while (true)
                {
                    if (lv >= GetMaxLevel(key))
                    {
                        break;
                    }
                    maxCost += GetCost(key,lv);
                    lv++;
                    _count++;

                    if (_count >= count)
                        break;
                    if (maxCost + GetCost(key, lv) > Player.Good.Get(GoodsKey.AwakeStone))
                        break;
                }

                tempdata.lv = lv;
                tempdata.cost = maxCost;

                return tempdata;
            }


            public static double GetCost(AwakeUpgradeKey key)
            {
                var lv = GetLevel(key);
                return Math.Floor(GetData(key)[lv].cost);
            }

            public static double GetCost(AwakeUpgradeKey key,int lv)
            {
                return Math.Floor(GetData(key)[lv].cost);
            }

            public static double GetValue(AwakeUpgradeKey key)
            {
                var lv = GetLevel(key);
                return GetData(key)[lv].value;
            }
            public static double GetValue(AwakeUpgradeKey key, int _lv)
            {
                return GetData(key)[_lv].value;
            }

            public static DataCommonUpgradeSequence.Cache[] GetData(AwakeUpgradeKey key)
            {
                var id = StaticData.Wrapper.awakeUpgradedatas[(int)key].sequenceId;
                return StaticData.Wrapper.upgradeSequences[id].Cached;
            }

        }
    }
}
