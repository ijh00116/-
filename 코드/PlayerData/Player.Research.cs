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
        public static class Research
        {
            private static Dictionary<ResearchUpgradeKey, Action> _onChangeMap = new Dictionary<ResearchUpgradeKey, Action>();
            public static UnityAction<ResearchUpgradeKey> StartUpdateSyncactions;
            public static UnityAction<ResearchUpgradeKey> CompleteUpdateSyncactions;

            private static Dictionary<ResearchUpgradeKey, double> _upgradeValue = new Dictionary<ResearchUpgradeKey, double>();

            public static DateTime currentTime;
            public const int defaultResearchSlotCount = 1;

            public const int maxResearchCount = 3;
            public static int PossibleResearchCount
            {
                get
                {
                    return defaultResearchSlotCount + (int)GetValue(ResearchUpgradeKey.ExpandResearchSlot);
                }
            }

            public static void Init()
            {
                _upgradeValue.Clear();

                for (int i = Cloud.researchData.data.Count; i < StaticData.Wrapper.researchTableDatas.Length; i++)
                {
                    Cloud.researchData.data.Add(new UserResearchUpgradeData() {level=0,scheduledExittime=null });
                }

                for (int i = 0; i < (int)ResearchUpgradeKey.End; i++)
                {
                    var tabledata = Player.Research.GetTableData((ResearchUpgradeKey)i);
                    var tableSequenceData = Player.Research.GetSequence((ResearchUpgradeKey)i);
                    if(i< (int)ResearchUpgradeKey.SwordAttackIncrease_2)
                    {
                       // BlackTree.Model.Player.Cloud.researchData.data[tabledata.index].level = tableSequenceData.maxLevel - 2;
                    }
                    else
                    {
                       // BlackTree.Model.Player.Cloud.researchData.data[tabledata.index].level = tableSequenceData.maxLevel - 2;
                    }
                    
                }

                for (int i = 0; i < StaticData.Wrapper.researchTableDatas.Length; i++)
                {
                    var typekey = StaticData.Wrapper.researchTableDatas[i].researchTypeKey;
                    var dataValue = GetValue(typekey);
                    _upgradeValue.Add(typekey, dataValue);
                }
                int possibleResearchMaxcount= defaultResearchSlotCount+StaticData.Wrapper.researchTableSequence[(int)ResearchUpgradeKey.ExpandResearchSlot].maxLevel;
                for (int i = Cloud.researchData.currentResearchKeylist.Count; i < possibleResearchMaxcount; i++)
                {
                    Cloud.researchData.currentResearchKeylist.Add(ResearchUpgradeKey.None);
                }
                Cloud.researchData.UpdateHash();
            }

            public static int GetMaxLevel(ResearchUpgradeKey key)
            {
                var id = GetTableData(key).index;

                return StaticData.Wrapper.researchTableSequence[id].maxLevel;
            }

            public static bool isMaxLevel(ResearchUpgradeKey key)
            {
                int lev = GetLevel(key);
                return lev>=GetMaxLevel(key);
            }

            public static bool isProgressing(ResearchUpgradeKey key)
            {
                var tabledata = GetTableData(key);
                return Cloud.researchData.data[tabledata.index].upgradeState==ResearchUpgradeStateKey.Progressing;
            }

            public static ResearchTableData GetTableData(ResearchUpgradeKey key)
            {
                ResearchTableData tempdata = null;
                for (int i=0; i< StaticData.Wrapper.researchTableDatas.Length; i++)
                {
                    if(StaticData.Wrapper.researchTableDatas[i].researchTypeKey==key)
                    {
                        tempdata = StaticData.Wrapper.researchTableDatas[i];
                        break;
                    }
                }
                return tempdata;
            }

            public static double GetCost(ResearchUpgradeKey key)
            {
                var lv = GetLevel(key);
                return Math.Floor(GetCache(key)[lv].cost);
            }

            public static int GetLevel(ResearchUpgradeKey key)
            {
                var tabledata = GetTableData(key);
                return Cloud.researchData.data[tabledata.index].level;
            }

            public static double GetTime(ResearchUpgradeKey key)
            {
                var lv = GetLevel(key);
                return GetCache(key)[lv].timeCost;
            }
            public static double GetValue(ResearchUpgradeKey key)
            {
                var lv = GetLevel(key);
                return GetCache(key)[lv].value;
            }

            public static double GetNextValue(ResearchUpgradeKey key)
            {
                var nextlv = GetLevel(key)+1;
                return GetCache(key)[nextlv].value;
            }

            private static DataResearchUpgradeSequence.Cache[] GetCache(ResearchUpgradeKey key)
            {
                var tabledata = GetTableData(key);
                var id = StaticData.Wrapper.researchTableDatas[tabledata.index].index;
                return StaticData.Wrapper.researchTableSequence[id].Cached;
            }
            public static DataResearchUpgradeSequence GetSequence(ResearchUpgradeKey key)
            {
                var tabledata = GetTableData(key);
                var id = StaticData.Wrapper.researchTableDatas[tabledata.index].index;
                return StaticData.Wrapper.researchTableSequence[id];
            }

            public static void StartUpgrade(ResearchUpgradeKey key)
            {
                CheckHash(Cloud.researchData);

                var tabledata = GetTableData(key);
                var tableSequenceData = GetSequence(key);
                var datavalue = Cloud.researchData.data[tabledata.index];
                int levelindex = GetLevel(key);

                DateTime exittime;
                if (key>=ResearchUpgradeKey.SwordAttackIncrease_2)
                {
                    exittime = Extension.GetServerTime().AddMinutes(GetTime(key));
                }
                else
                {
                    exittime = Extension.GetServerTime().AddMinutes(tableSequenceData.ResearchTime_min);
                }
               
                datavalue.SetUpgradeStart(exittime);

                StartUpdateSyncactions?.Invoke(key);

                Cloud.researchData
                    .UpdateHash()
                    .SetDirty(true);

                Player.Quest.TryCountUp(Definition.QuestType.ResearchContentsClear, 1);
            }

            public static bool CanUpgradeComplete(ResearchUpgradeKey key)
            {
                var tabledata = GetTableData(key);

                var datavalue = Cloud.researchData.data[tabledata.index];

                DateTime current = Extension.GetServerTime();
                DateTime exittime = Extension.GetDateTimeByIsoString(datavalue.scheduledExittime);

                return current >= exittime;
            }

            public static void CompleteUpgrade(ResearchUpgradeKey key)
            {
                CheckHash(Cloud.researchData);

                var tabledata = GetTableData(key);
                var tableSequenceData = GetSequence(key);

                var datavalue = Cloud.researchData.data[tabledata.index];

                datavalue.level++;

                datavalue.SetUpgradeComplete();

                CompleteUpdateSyncactions?.Invoke(key);

                Cloud.researchData
                    .UpdateHash()
                    .SetDirty(true);

                Player.Unit.StatusSync();
            }


        }
    }
}
