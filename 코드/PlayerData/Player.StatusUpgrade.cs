using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Core;
using UnityEngine.Events;
using BlackTree.Definition;
using System;

namespace BlackTree.Model
{
    public static partial class Player
    {
        public static class StatusUpgrade
		{
			public class StatupgradeValueForLvType
			{
				public int lv;
				public double cost;
			}

			private static Dictionary<StatusUpgradeKey, UnityAction> _onTestChangeMap = new Dictionary<StatusUpgradeKey, UnityAction>();
            public static UnityAction<StatusUpgradeKey> UpdateSyncactions;
            private static Dictionary<StatusUpgradeKey, double> upgradeValue;

			public const int MaxGradeLevel = 37;

			public const int tier1GradeLevel = 22;

			public static bool canUpgrade = false;
			public static void Init()
			{
				for (int i = Cloud.statusUpgrade.upgradeLevels.Count; i < StaticData.Wrapper.statusUpgradedatas.Length; i++)
				{
					Cloud.statusUpgrade.upgradeLevels.Add(0);
				}
				upgradeValue = new Dictionary<StatusUpgradeKey, double>();

				for (int i = 0; i < Cloud.statusUpgrade.upgradeLevels.Count; i++)
				{
					var valuedata = Player.StatusUpgrade.GetValue((StatusUpgradeKey)i);
					upgradeValue.Add((StatusUpgradeKey)i, valuedata);
				}

				Cloud.statusUpgrade.UpdateHash();
			}

			public static void BindOnChange(StatusUpgradeKey key, UnityAction action)
			{
				if (!_onTestChangeMap.ContainsKey(key))
				{
					_onTestChangeMap.Add(key, action);
				}
				else
				{
					_onTestChangeMap[key] += action;
				}
			}

			public static int GetMaxLevel(StatusUpgradeKey key)
			{
				var id = StaticData.Wrapper.statusUpgradedatas[(int)key].sequenceId;

				return StaticData.Wrapper.upgradeSequences[id].maxLevel;
			}

			public static bool GetIsMaxLevel(StatusUpgradeKey key)
			{
				var id = StaticData.Wrapper.statusUpgradedatas[(int)key].sequenceId;
				var currLv = GetLevel(key);

				return currLv >= GetMaxLevel(key);
			}

			public static int GetLevel(StatusUpgradeKey key)
			{
				return Cloud.statusUpgrade.upgradeLevels[(int)key];
			}

			public static string GetMaxLevelText(StatusUpgradeKey key)
			{
				return string.Format(GetMaxLevel(key).ToString());
			}

			public static double GetCost(StatusUpgradeKey key)
			{
				var id = StaticData.Wrapper.statusUpgradedatas[(int)key].sequenceId;
				return StaticData.Wrapper.upgradeSequences[id].startCost;
			}

			public static StatupgradeValueForLvType GetMaxCostLv(StatusUpgradeKey key)
			{
				StatupgradeValueForLvType tempdata = new StatupgradeValueForLvType();
				int lv = GetLevel(key);
				double maxCost = 0;
				while (true)
				{
					if (lv >= GetMaxLevel(key))
					{
						break;
					}

					maxCost += GetCosts(key)[lv].cost;
					lv++;

					if (maxCost + GetCosts(key)[lv].cost > Player.Good.Get(GoodsKey.StatusPoint))
						break;
				}

				tempdata.lv = lv;
				tempdata.cost = maxCost;

				return tempdata;
			}

			public static StatupgradeValueForLvType GetCostLv(StatusUpgradeKey key, int count)
			{
				StatupgradeValueForLvType tempdata = new StatupgradeValueForLvType();
				int lv = GetLevel(key);
				double maxCost = 0;
				int _count = 0;
				while (true)
				{
					if (lv >= GetMaxLevel(key))
					{
						break;
					}
					maxCost += GetCosts(key)[lv].cost;
					lv++;
					_count++;

					if (_count >= count)
						break;
					if (maxCost + GetCosts(key)[lv].cost > Player.Good.Get(GoodsKey.StatusPoint))
						break;
				}

				tempdata.lv = lv;
				tempdata.cost = maxCost;

				return tempdata;
			}


			public static DataCommonUpgradeSequence.Cache[] GetCosts(StatusUpgradeKey key)
			{
				var id = StaticData.Wrapper.statusUpgradedatas[(int)key].sequenceId;
				return StaticData.Wrapper.upgradeSequences[id].Cached;
			}

			public static double GetValue(StatusUpgradeKey key)
			{
				//var id = StaticData.Wrapper.statusUpgradedatas[(int)key].sequenceId;
				//Cloud.statusUpgrade.upgradeLevels[(int)key] = StaticData.Wrapper.upgradeSequences[id].maxLevel-1;
				var lv = GetLevel(key);
				return GetValues(key)[lv].value;
			}

			private static DataCommonUpgradeSequence.Cache[] GetValues(StatusUpgradeKey key)
			{
				var id = StaticData.Wrapper.statusUpgradedatas[(int)key].sequenceId;
				return StaticData.Wrapper.upgradeSequences[id].Cached;
			}

			public static void Upgrade(StatusUpgradeKey key)
			{
				CheckHash(Cloud.statusUpgrade);
				var level = Cloud.statusUpgrade.upgradeLevels[(int)key]++;

				Player.Quest.TryCountUp(QuestType.StatLevel, 1);

				upgradeValue[key] = level;

				if (_onTestChangeMap.ContainsKey(key))
					_onTestChangeMap[key]?.Invoke();

				UpdateSyncactions?.Invoke(key);

				UpdateSyncData();
			}

			public static void Upgrade(StatusUpgradeKey key, int Count)
			{
				CheckHash(Cloud.statusUpgrade);
				var level = Cloud.statusUpgrade.upgradeLevels[(int)key]+= Count;

				Player.Quest.TryCountUp(QuestType.StatLevel, Count);

				upgradeValue[key] = level;

				if (_onTestChangeMap.ContainsKey(key))
					_onTestChangeMap[key]?.Invoke();

				UpdateSyncactions?.Invoke(key);

				UpdateSyncData();
			}

			public static void TemporaryUpgrade(StatusUpgradeKey key)
			{
				var level = Cloud.statusUpgrade.upgradeLevels[(int)key]++;

				Player.Quest.TryCountUp(QuestType.StatLevel, 1);

				upgradeValue[key] = level;

				if (_onTestChangeMap.ContainsKey(key))
					_onTestChangeMap[key]?.Invoke();

				UpdateSyncactions?.Invoke(key);
			}

			public static void TemporaryUpgrade(StatusUpgradeKey key, int Count)
			{
				var level = Cloud.statusUpgrade.upgradeLevels[(int)key]+= Count;

				Player.Quest.TryCountUp(QuestType.StatLevel, Count);

				upgradeValue[key] = level;

				if (_onTestChangeMap.ContainsKey(key))
					_onTestChangeMap[key]?.Invoke();

				UpdateSyncactions?.Invoke(key);
			}

			public static void UpdateSyncData()
			{
				Cloud.statusUpgrade
					.UpdateHash()
					.SetDirty(true);

				Player.Unit.StatusSync();

				LocalSaveLoader.SaveUserCloudData();
			}
		}
    }
}