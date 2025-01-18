using System;
using System.Collections.Generic;
using BlackTree.Definition;
using UnityEngine.Events;
using UnityEngine;
using BlackTree.Model;
using BlackTree.Core;

namespace BlackTree.Model
{
	[Serializable]
	public struct GoodsValue
	{
		public GoodsKey key;
		public double value;

		public GoodsValue(GoodsValue goodKey)
		{
			key = goodKey.key;
			value = goodKey.value;
		}
		public GoodsValue(GoodsKey key, double value)
		{
			this.key = key;
			this.value = value;
		}
	}

	public static partial class Player
    {
        public static class Good
        {
			public static double Get(GoodsKey key)
			{
				var sd = Cloud.good.collection;
				double value = Cloud.good.collection[(int)key].value;
				return value;
			}
			public static void Set(GoodsKey key, double value)
			{
				CheckHash(Cloud.good);
				Cloud.good.collection[(int)key].value = value;
				Cloud.good
					.UpdateHash()
					.SetDirty(true);

				if (ControllerGood.onChange.ContainsKey(key))
					ControllerGood.onChange[key]?.Invoke();

				LocalSaveLoader.SaveUserCloudData();
			}
		}

		public static class ControllerGood
        {
			public static UnityAction<GoodsValue, Vector2, int> onDrop;
			public static Action<GoodsValue, Vector2, int, float, bool> onEffect;
			public static UnityAction onAfterDrop;
			public static Dictionary<GoodsKey, UnityAction> onChange = new Dictionary<GoodsKey, UnityAction>();

			public static bool IsCanBuy(GoodsKey key, double cost)
			{
				return GetValue(key) >= cost;
			}

			public static double GetValue(GoodsKey key)
			{
				return Good.Get(key);
			}

			public static void BindOnChange(GoodsKey key, UnityAction action)
			{
				if (!onChange.ContainsKey(key))
				{
					onChange.Add(key, action);
				}
				else
				{
					onChange[key] += action;
				}
			}

			public static void Consume(GoodsKey key, double value)
			{
				if (value == 0) return;
				double _value = Good.Get(key) - value;
				Good.Set(key, _value);
			}

			public static void Earn(GoodsKey key, double value)
			{
				double calculated = value;
				if (value == 0) return;

				//switch (key)
				//{
				//	case GoodsKey.Coin:
				//		value *=
				//			(1 + Player.StatusUpgrade.GetValue(StatusUpgradeKey.CoinGetIncrease));
				//		var bonusChance
				//			= Player.StatusUpgrade.GetValue(StatusUpgradeKey.Coin2xChanceUpgrade);

				//		if (UnityEngine.Random.Range(0f, 1f) < bonusChance)
				//			value *= 2;

				//		break;
				//}
				float goldVipTimes = 1;
				float awakeVipTimes= 1;
				if (Player.Cloud.inAppPurchase.purchaseVip==1)
                {
					goldVipTimes = StaticData.Wrapper.vIPFixedRewardsDatas[0].goldFixedRewardTimes;

				}
				if (Player.Cloud.inAppPurchase.purchaseVip == 2)
				{
					goldVipTimes = StaticData.Wrapper.vIPFixedRewardsDatas[1].goldFixedRewardTimes;
				}
				if (Player.Cloud.inAppPurchase.purchaseVip == 3)
				{
					goldVipTimes = StaticData.Wrapper.vIPFixedRewardsDatas[2].goldFixedRewardTimes;
					awakeVipTimes = StaticData.Wrapper.vIPFixedRewardsDatas[2].DungeonFixedRewardTimes;
				}
				if (key==GoodsKey.ResearchPotion)
                {
					//value = value * (1 + Player.Research.GetValue(ResearchUpgradeKey.IncreaseEarnResearchPotion) * 0.01f);
                }
				if (key == GoodsKey.Coin)
				{
					calculated = calculated * (1 + Player.Pet._equipAbilitycaches[EquipAbilityKey.CoinGetIncrease]* 0.01f)
						* (1 + Player.Research.GetValue(ResearchUpgradeKey.IncreaseEarnGold) * 0.01f)
						* goldVipTimes;
				}
				if (key == GoodsKey.AwakeStone)
				{
					calculated = calculated * awakeVipTimes;
				}


				double _value = Good.Get(key) + calculated;
				Good.Set(key, _value);
			}

			public static double CoinCalculatedData(double value)
            {
				double calculated = value;

				float goldVipTimes = 1;
				if (Player.Cloud.inAppPurchase.purchaseVip == 1)
				{
					goldVipTimes = StaticData.Wrapper.vIPFixedRewardsDatas[0].goldFixedRewardTimes;

				}
				if (Player.Cloud.inAppPurchase.purchaseVip == 2)
				{
					goldVipTimes = StaticData.Wrapper.vIPFixedRewardsDatas[1].goldFixedRewardTimes;
				}
				if (Player.Cloud.inAppPurchase.purchaseVip == 3)
				{
					goldVipTimes = StaticData.Wrapper.vIPFixedRewardsDatas[2].goldFixedRewardTimes;
				}
				calculated = calculated * (1 + Player.Pet._equipAbilitycaches[EquipAbilityKey.CoinGetIncrease] * 0.01f)
						* (1 + Player.Research.GetValue(ResearchUpgradeKey.IncreaseEarnGold) * 0.01f)
						* goldVipTimes;

				return calculated;
			}

			public static void Earn(GoodsValue goodValue)
			{
				Earn(goodValue.key, goodValue.value);
			}

			public static void Init()
			{
				for (int i = Cloud.good.collection.Count; i < StaticData.Wrapper.goods.Length; i++)
				{
					Cloud.good.collection.Add(new UserGood());
				}

				Cloud.good.UpdateHash();
			}

			public static void Reset(GoodsKey key)
			{
				Good.Set(key, 0);
			}

			public static void EarnGoods(int[] goodsIds, int[] rateIds, Vector2 position)
			{
				for (int i = 0; i < goodsIds.Length; i++)
				{
					var rates = StaticData.Wrapper.goodsratedata[rateIds[i]].Rate;
					float random = UnityEngine.Random.Range(0f, 1f);

					float currentRate = 0;
					int correctindex = 0;
					for (int j = 0; j < rates.Length; j++)
					{
						currentRate += rates[j];
						if (random < currentRate)
						{
							correctindex = j;
							break;
						}
					}
					int goodscount = StaticData.Wrapper.goodsratedata[rateIds[i]].goodsCount[correctindex];
					//Earn((GoodKey)goodsIds[i], goodscount);

					onDrop.Invoke(new GoodsValue((GoodsKey)goodsIds[i], goodscount * MathF.Pow(Battle.Field.CurrentFieldStage+1, 1.02f)), position, UnityEngine.Random.Range(1, 4));
				}
			}

			public static void EarnGood(int goodsIds, int rateIds, Vector2 position)
			{
				var rates = StaticData.Wrapper.goodsratedata[rateIds].Rate;
				float random = UnityEngine.Random.Range(0f, 1f);

				float currentRate = 0;
				int correctindex = 0;
				for (int j = 0; j < rates.Length; j++)
				{
					currentRate += rates[j];
					if (random < currentRate)
					{
						correctindex = j;
						break;
					}
				}
				int goodscount = StaticData.Wrapper.goodsratedata[rateIds].goodsCount[correctindex];

				onDrop.Invoke(new GoodsValue((GoodsKey)goodsIds, goodscount * MathF.Pow(Battle.Field.CurrentFieldStage+1, 1.02f)), position, UnityEngine.Random.Range(1, 4));
			}
			public static void EarnGoodNorate(GoodsKey goodsIds, int goodcount, Vector2 position)
			{
				onDrop.Invoke(new GoodsValue(goodsIds, goodcount), position, UnityEngine.Random.Range(1, 4));
			}

			public static GoodsKey RewardToGoods(RewardTypes _type)
            {
				GoodsKey goodkey = GoodsKey.None;

				switch (_type)
                {
                    case RewardTypes.None:
                        break;
                    case RewardTypes.Coin:
						goodkey = GoodsKey.Coin;
                        break;
                    case RewardTypes.Dia:
						goodkey = GoodsKey.Dia;
						break;
                    case RewardTypes.StatusPoint:
						goodkey = GoodsKey.StatusPoint;
						break;
                    case RewardTypes.AwakeStone:
						goodkey = GoodsKey.AwakeStone;
						break;
                    case RewardTypes.ResearchPotion:
						goodkey = GoodsKey.ResearchPotion;
						break;
					case RewardTypes.skillAwakeStone:
						goodkey = GoodsKey.SkillAwakeStone;
						break;
					case RewardTypes.Exp:
						break;
                    case RewardTypes.Pet:
                        break;
                    case RewardTypes.ADRemove:
                        break;
                    case RewardTypes.Vip_1:
                        break;
                    case RewardTypes.Vip_2:
                        break;
                    case RewardTypes.Vip_3:
                        break;
					case RewardTypes.riftDungeonKey:
						goodkey = GoodsKey.RiftDungeonTicket;
						break;
					default:
                        break;
                }

				return goodkey;
            }

			public static LocalizeNameKeys RewardTolocalize(RewardTypes _type)
            {
				LocalizeNameKeys typename = LocalizeNameKeys.None;
                switch (_type)
                {
                    case RewardTypes.None:
                        break;
                    case RewardTypes.Coin:
						typename = LocalizeNameKeys.Goods_Coin;
                        break;
                    case RewardTypes.Dia:
						typename = LocalizeNameKeys.Goods_Dia;
						break;
                    case RewardTypes.StatusPoint:
						typename = LocalizeNameKeys.Goods_Stat;
						break;
                    case RewardTypes.AwakeStone:
						typename = LocalizeNameKeys.Goods_AwakeStone;
						break;
                    case RewardTypes.ResearchPotion:
						typename = LocalizeNameKeys.Goods_ResearchPotion;
						break;
                    case RewardTypes.Exp:
						typename = LocalizeNameKeys.IncreaseGetExp;
						break;
                    case RewardTypes.Pet:
                        break;
                    case RewardTypes.ADRemove:
						typename = LocalizeNameKeys.Package_ADRemove_0;
						break;
                    case RewardTypes.Vip_1:
						typename = LocalizeNameKeys.Package_Vip_0;
						break;
                    case RewardTypes.Vip_2:
						typename = LocalizeNameKeys.Package_Vip_1;
						break;
                    case RewardTypes.Vip_3:
						typename = LocalizeNameKeys.Package_Vip_2;
						break;
                    case RewardTypes.package_swordfewhit:
						typename = LocalizeNameKeys.Skill_SwordFewHitFire;
						break;
                    case RewardTypes.package_magicfewhit:
						typename = LocalizeNameKeys.Skill_MagicFewHitFire;
						break;
                    case RewardTypes.package_setturret:
						typename = LocalizeNameKeys.Skill_SetTurret;
						break;
                    case RewardTypes.package_companionspawn:
						typename = LocalizeNameKeys.Skill_CompanionSpawn;
						break;
                    case RewardTypes.package_guidedmissile:
						typename = LocalizeNameKeys.Skill_GuidedMissile;
						break;
                    case RewardTypes.package_godmode:
						typename = LocalizeNameKeys.Skill_GodMode;
						break;
                    case RewardTypes.package_summon:
						typename = LocalizeNameKeys.Skill_SummonSubunit;
						break;
                    case RewardTypes.package_nova:
						typename = LocalizeNameKeys.Skill_NoveForSeconds;
						break;
                    case RewardTypes.package_meteor:
						typename = LocalizeNameKeys.Skill_SpawnMeteor;
						break;
                    case RewardTypes.package_multielectric:
						typename = LocalizeNameKeys.Skill_MultipleElectric;
						break;
                    case RewardTypes.skillAwakeStone:
						typename = LocalizeNameKeys.Goods_SkillAwakeStone;
						break;
					case RewardTypes.Weapon_4_4:
						typename = LocalizeNameKeys.Weapon_4GradeLast;
						break;
					case RewardTypes.Staff_4_4:
						typename = LocalizeNameKeys.Staff_4GradeLast;
						break;
					case RewardTypes.Armor_4_4:
						typename = LocalizeNameKeys.Armor_4GradeLast;
						break;
					case RewardTypes.riftDungeonKey:
						typename = LocalizeNameKeys.Goods_RiftDungeonTicket;
						break;
					default:
                        break;
                }

				return typename;
            }
		}
	}
}