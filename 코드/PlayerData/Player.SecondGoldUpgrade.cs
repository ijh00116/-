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
        public static class SecondGoldUpgrade
        {
            public class GoldupgradeValueForLvType
            {
                public int lv;
                public double cost;
            }

			private static Dictionary<Tier2GoldUpgradeKey, UnityAction> _onChangeMap = new Dictionary<Tier2GoldUpgradeKey, UnityAction>();
			public static UnityAction<Tier2GoldUpgradeKey> UpdateSyncactions;
			public static UnityAction OnResetUpgrade;

			private static Dictionary<Tier2GoldUpgradeKey, double> _upgradeValue = new Dictionary<Tier2GoldUpgradeKey, double>();

			public static bool canUpgrade = false;
			public static void Init()
			{
				_upgradeValue.Clear();

				for (int i = Cloud.goldtiersecUpgrade.upgradeLevels.Count; i < StaticData.Wrapper.tierSecgoldUpgradedatas.Length; i++)
				{
					Cloud.goldtiersecUpgrade.upgradeLevels.Add(0);
				}

				for (int i = 0; i < Cloud.goldtiersecUpgrade.upgradeLevels.Count; i++)
				{
					var dataValue = GetValue((Tier2GoldUpgradeKey)i);
					_upgradeValue.Add((Tier2GoldUpgradeKey)i, dataValue);
				}

				Cloud.goldtiersecUpgrade.UpdateHash();
			}

			public static void BindOnChange(Tier2GoldUpgradeKey key, UnityAction action)
			{
				if (!_onChangeMap.ContainsKey(key))
				{
					_onChangeMap.Add(key, action);
				}
				else
				{
					_onChangeMap[key] += action;
				}
			}

			public static int GetMaxLevel(Tier2GoldUpgradeKey key)
			{
				var id = StaticData.Wrapper.tierSecgoldUpgradedatas[(int)key].sequenceId;

				return StaticData.Wrapper.upgradeSequences[id].maxLevel;
			}

			public static bool GetIsMaxLevel(Tier2GoldUpgradeKey key)
			{
				var id = StaticData.Wrapper.tierSecgoldUpgradedatas[(int)key].sequenceId;
				var currLv = GetLevel(key);

				return currLv >= GetMaxLevel(key);
			}

			public static int GetLevel(Tier2GoldUpgradeKey key)
			{
				return Cloud.goldtiersecUpgrade.upgradeLevels[(int)key];
			}

			public static string GetMaxLevelText(Tier2GoldUpgradeKey key)
			{
				var id = StaticData.Wrapper.tierSecgoldUpgradedatas[(int)key].sequenceId;
				return string.Format(GetMaxLevel(key).ToString());
			}

			public static double GetCost(Tier2GoldUpgradeKey key)
			{
				var lv = GetLevel(key);
				return Math.Floor(GetCosts(key)[lv].cost);
			}

			public static GoldupgradeValueForLvType GetMaxCostLv(Tier2GoldUpgradeKey key)
			{
				GoldupgradeValueForLvType tempdata = new GoldupgradeValueForLvType();
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

					if (maxCost + GetCosts(key)[lv].cost > Player.Good.Get(GoodsKey.Coin))
						break;
				}

				tempdata.lv = lv;
				tempdata.cost = maxCost;

				return tempdata;
			}

			public static GoldupgradeValueForLvType GetCostLv(Tier2GoldUpgradeKey key, int count)
			{
				GoldupgradeValueForLvType tempdata = new GoldupgradeValueForLvType();
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
					if (maxCost + GetCosts(key)[lv].cost > Player.Good.Get(GoodsKey.Coin))
						break;
				}

				tempdata.lv = lv;
				tempdata.cost = maxCost;

				return tempdata;
			}

			public static double GetPossibleLvToCost(Tier2GoldUpgradeKey key)
			{
				var lv = GetLevel(key);
				return Math.Floor(GetCosts(key)[lv].cost);
			}

			public static DataCommonUpgradeSequence.Cache[] GetCosts(Tier2GoldUpgradeKey key)
			{
				var id = StaticData.Wrapper.tierSecgoldUpgradedatas[(int)key].sequenceId;
				return StaticData.Wrapper.upgradeSequences[id].Cached;
			}

			public static double GetValue(Tier2GoldUpgradeKey key)
			{
				var lv = GetLevel(key);
				return GetValues(key)[lv].value;
			}

			public static double GetValue(Tier2GoldUpgradeKey key, int _lv)
			{
				return GetValues(key)[_lv].value;
			}

			public static void Upgrade(Tier2GoldUpgradeKey key)
			{
				CheckHash(Cloud.goldtiersecUpgrade);
				var datavalue = Cloud.goldtiersecUpgrade.upgradeLevels[(int)key]++;

				_upgradeValue[key] = datavalue;

				if (_onChangeMap.ContainsKey(key))
					_onChangeMap[key]?.Invoke();

				UpdateSyncactions?.Invoke(key);

				UpdateSyncData();
			}

			public static void Upgrade(Tier2GoldUpgradeKey key, int Count)
			{
				CheckHash(Cloud.goldtiersecUpgrade);
				var datavalue = Cloud.goldtiersecUpgrade.upgradeLevels[(int)key] += Count;

				_upgradeValue[key] = datavalue;

				if (_onChangeMap.ContainsKey(key))
					_onChangeMap[key]?.Invoke();

				UpdateSyncactions?.Invoke(key);

				UpdateSyncData();
			}

			public static void TemporaryUpgrade(Tier2GoldUpgradeKey key)
			{
				var datavalue = Cloud.goldtiersecUpgrade.upgradeLevels[(int)key]++;

				_upgradeValue[key] = datavalue;

				if (_onChangeMap.ContainsKey(key))
					_onChangeMap[key]?.Invoke();

				UpdateSyncactions?.Invoke(key);

			}

			public static void TemporaryUpgrade(Tier2GoldUpgradeKey key, int Count)
			{
				var datavalue = Cloud.goldtiersecUpgrade.upgradeLevels[(int)key] += Count;

				_upgradeValue[key] = datavalue;

				if (_onChangeMap.ContainsKey(key))
					_onChangeMap[key]?.Invoke();

				UpdateSyncactions?.Invoke(key);

			}

			public static void UpdateSyncData()
			{
				Cloud.goldtiersecUpgrade
				.UpdateHash()
				.SetDirty(true);

				Player.Unit.StatusSync();

				LocalSaveLoader.SaveUserCloudData();
			}

			private static DataCommonUpgradeSequence.Cache[] GetValues(Tier2GoldUpgradeKey key)
			{
				var id = StaticData.Wrapper.tierSecgoldUpgradedatas[(int)key].sequenceId;
				return StaticData.Wrapper.upgradeSequences[id].Cached;
			}
		}
    }
}