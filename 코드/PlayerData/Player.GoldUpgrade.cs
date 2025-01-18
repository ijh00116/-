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
		
        public static class GoldUpgrade
        {
			public class GoldupgradeValueForLvType
            {
				public int lv;
				public double cost;
            }

			private static Dictionary<GoldUpgradeKey, UnityAction> _onChangeMap = new Dictionary<GoldUpgradeKey, UnityAction>();
            public static UnityAction<GoldUpgradeKey> UpdateSyncactions;
            public static UnityAction OnResetUpgrade;

            private static Dictionary<GoldUpgradeKey, double> _upgradeValue = new Dictionary<GoldUpgradeKey, double>();

			public static bool canUpgrade = false;
			public static void Init()
            {
				_upgradeValue.Clear();

                for (int i = Cloud.goldUpgrade.upgradeLevels.Count; i < StaticData.Wrapper.goldUpgradedatas.Length; i++)
                {
                    Cloud.goldUpgrade.upgradeLevels.Add(0);
                }

                for (int i = 0; i < Cloud.goldUpgrade.upgradeLevels.Count; i++)
                {
                    var dataValue = GetValue((GoldUpgradeKey)i);
					_upgradeValue.Add((GoldUpgradeKey)i, dataValue);
                }

                Cloud.goldUpgrade.UpdateHash();
            }

			public static void BindOnChange(GoldUpgradeKey key, UnityAction action)
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

			public static int GetMaxLevel(GoldUpgradeKey key)
			{
				var id = StaticData.Wrapper.goldUpgradedatas[(int)key].sequenceId;

				return StaticData.Wrapper.upgradeSequences[id].maxLevel;
			}

			public static bool GetIsMaxLevel(GoldUpgradeKey key)
			{
				var id = StaticData.Wrapper.goldUpgradedatas[(int)key].sequenceId;
				var currLv = GetLevel(key);

				return currLv >= GetMaxLevel(key);
			}

			public static int GetLevel(GoldUpgradeKey key)
			{
				return Cloud.goldUpgrade.upgradeLevels[(int)key];
			}

			public static string GetMaxLevelText(GoldUpgradeKey key)
			{
				var id = StaticData.Wrapper.goldUpgradedatas[(int)key].sequenceId;
				return string.Format(GetMaxLevel(key).ToString());
			}

			public static double GetCost(GoldUpgradeKey key)
			{
				var lv = GetLevel(key);
				return Math.Floor(GetCosts(key)[lv].cost);
			}

			public static GoldupgradeValueForLvType GetMaxCostLv(GoldUpgradeKey key)
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

			public static GoldupgradeValueForLvType GetCostLv(GoldUpgradeKey key,int count)
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

			public static double GetPossibleLvToCost(GoldUpgradeKey key)
			{
				var lv = GetLevel(key);
				return Math.Floor(GetCosts(key)[lv].cost);
			}

			public static DataCommonUpgradeSequence.Cache[] GetCosts(GoldUpgradeKey key)
			{
				var id = StaticData.Wrapper.goldUpgradedatas[(int)key].sequenceId;
				return StaticData.Wrapper.upgradeSequences[id].Cached;
			}

			public static double GetValue(GoldUpgradeKey key)
			{
				var lv = GetLevel(key);
				return GetValues(key)[lv].value;
			}

			public static double GetValue(GoldUpgradeKey key,int _lv)
			{
				return GetValues(key)[_lv].value;
			}

			public static void Upgrade(GoldUpgradeKey key)
			{
				CheckHash(Cloud.goldUpgrade);
				var datavalue = Cloud.goldUpgrade.upgradeLevels[(int)key]++;

				_upgradeValue[key] = datavalue;

				if (_onChangeMap.ContainsKey(key))
					_onChangeMap[key]?.Invoke();

				UpdateSyncactions?.Invoke(key);

				UpdateSyncData();
			}

			public static void Upgrade(GoldUpgradeKey key,int Count)
			{
				CheckHash(Cloud.goldUpgrade);
				var datavalue = Cloud.goldUpgrade.upgradeLevels[(int)key]+= Count;

				_upgradeValue[key] = datavalue;

				if (_onChangeMap.ContainsKey(key))
					_onChangeMap[key]?.Invoke();

				UpdateSyncactions?.Invoke(key);

				UpdateSyncData();
			}

			public static void TemporaryUpgrade(GoldUpgradeKey key)
			{
				var datavalue = Cloud.goldUpgrade.upgradeLevels[(int)key]++;

				_upgradeValue[key] = datavalue;

				if (_onChangeMap.ContainsKey(key))
					_onChangeMap[key]?.Invoke();

				UpdateSyncactions?.Invoke(key);
			}

			public static void TemporaryUpgrade(GoldUpgradeKey key, int Count)
			{
				var datavalue = Cloud.goldUpgrade.upgradeLevels[(int)key]+= Count;

				_upgradeValue[key] = datavalue;

				if (_onChangeMap.ContainsKey(key))
					_onChangeMap[key]?.Invoke();

				UpdateSyncactions?.Invoke(key);
			}

			public static void UpdateSyncData()
            {
				Cloud.goldUpgrade
				.UpdateHash()
				.SetDirty(true);

				Player.Unit.StatusSync();

				LocalSaveLoader.SaveUserCloudData();
			}

			private static DataCommonUpgradeSequence.Cache[] GetValues(GoldUpgradeKey key)
			{
				var id = StaticData.Wrapper.goldUpgradedatas[(int)key].sequenceId;
				return StaticData.Wrapper.upgradeSequences[id].Cached;
			}
		}
    }
}