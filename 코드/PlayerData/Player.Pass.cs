using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Definition;
using BlackTree.Core;
using BlackTree.Bundles;

namespace BlackTree.Model
{
    public static partial class Player
    {
        public static class Pass
        {
            const int levelpassCount = 3;
            const int chapterpassCount = 3;

            public static System.Action levelCallBack;
            public static System.Action chapterCallBack;

            public static System.Action<int,bool> rewardedCallback;
            public static void Init()
            {
                for (int i = Cloud.battlepassHistory.levelPassHistory_0.normalRecieved.Count; i < StaticData.Wrapper.levelBattlePass_0.Length; i++)
                {
                    Cloud.battlepassHistory.levelPassHistory_0.normalRecieved.Add(false);
                    Cloud.battlepassHistory.levelPassHistory_0.premiumRecieved.Add(false);
                }
                for (int i = Cloud.battlepassHistory.levelPassHistory_1.normalRecieved.Count; i < StaticData.Wrapper.levelBattlePass_1.Length; i++)
                {
                    Cloud.battlepassHistory.levelPassHistory_1.normalRecieved.Add(false);
                    Cloud.battlepassHistory.levelPassHistory_1.premiumRecieved.Add(false);
                }
                for (int i = Cloud.battlepassHistory.levelPassHistory_2.normalRecieved.Count; i < StaticData.Wrapper.levelBattlePass_2.Length; i++)
                {
                    Cloud.battlepassHistory.levelPassHistory_2.normalRecieved.Add(false);
                    Cloud.battlepassHistory.levelPassHistory_2.premiumRecieved.Add(false);
                }

                for (int i = Cloud.battlepassHistory.chapterPassHistory_0.normalRecieved.Count; i < StaticData.Wrapper.chapterBattlePass_0.Length; i++)
                {
                    Cloud.battlepassHistory.chapterPassHistory_0.normalRecieved.Add(false);
                    Cloud.battlepassHistory.chapterPassHistory_0.premiumRecieved.Add(false);
                }
                for (int i = Cloud.battlepassHistory.chapterPassHistory_1.normalRecieved.Count; i < StaticData.Wrapper.chapterBattlePass_2.Length; i++)
                {
                    Cloud.battlepassHistory.chapterPassHistory_1.normalRecieved.Add(false);
                    Cloud.battlepassHistory.chapterPassHistory_1.premiumRecieved.Add(false);
                }
                for (int i = Cloud.battlepassHistory.chapterPassHistory_2.normalRecieved.Count; i < StaticData.Wrapper.chapterBattlePass_2.Length; i++)
                {
                    Cloud.battlepassHistory.chapterPassHistory_2.normalRecieved.Add(false);
                    Cloud.battlepassHistory.chapterPassHistory_2.premiumRecieved.Add(false);
                }

                for (int i = Cloud.battlepassPurchaseHistory.levelPassPurchased.Count; i < levelpassCount; i++)
                {
                    Cloud.battlepassPurchaseHistory.levelPassPurchased.Add(false);
                }
                for (int i = Cloud.battlepassPurchaseHistory.chapterPassPurchased.Count; i < chapterpassCount; i++)
                {
                    Cloud.battlepassPurchaseHistory.chapterPassPurchased.Add(false);
                }

            }

            public static void GiveReward(ContentLockType locktype,int tier,int index,bool isnormal)
            {
                BattlePassData currentData=null;
                switch (locktype)
                {
                    case ContentLockType.UnitLevel:
                        if(tier==0)
                        {
                            currentData = StaticData.Wrapper.levelBattlePass_0[index];
                            if(isnormal)
                            {
                                Cloud.battlepassHistory.levelPassHistory_0.normalRecieved[index] = true;
                            }
                            else
                            {
                                Cloud.battlepassHistory.levelPassHistory_0.premiumRecieved[index] = true;
                            }
                            Cloud.battlepassHistory.UpdateHash().SetDirty(true);
                        }
                        else if(tier==1)
                        {
                            currentData = StaticData.Wrapper.levelBattlePass_1[index];
                            if (isnormal)
                            {
                                Cloud.battlepassHistory.levelPassHistory_1.normalRecieved[index] = true;
                            }
                            else
                            {
                                Cloud.battlepassHistory.levelPassHistory_1.premiumRecieved[index] = true;
                            }
                            Cloud.battlepassHistory.UpdateHash().SetDirty(true);
                        }
                        else if (tier == 2)
                        {
                            currentData = StaticData.Wrapper.levelBattlePass_2[index];
                            if (isnormal)
                            {
                                Cloud.battlepassHistory.levelPassHistory_2.normalRecieved[index] = true;
                            }
                            else
                            {
                                Cloud.battlepassHistory.levelPassHistory_2.premiumRecieved[index] = true;
                            }
                            Cloud.battlepassHistory.UpdateHash().SetDirty(true);
                        }
                        break;
                    case ContentLockType.ChapterLevel:
                        if (tier == 0)
                        {
                            currentData = StaticData.Wrapper.chapterBattlePass_0[index];
                            if (isnormal)
                            {
                                Cloud.battlepassHistory.chapterPassHistory_0.normalRecieved[index] = true;
                            }
                            else
                            {
                                Cloud.battlepassHistory.chapterPassHistory_0.premiumRecieved[index] = true;
                            }
                            Cloud.battlepassHistory.UpdateHash().SetDirty(true);
                        }
                        else if (tier == 1)
                        {
                            currentData = StaticData.Wrapper.chapterBattlePass_1[index];
                            if (isnormal)
                            {
                                Cloud.battlepassHistory.chapterPassHistory_1.normalRecieved[index] = true;
                            }
                            else
                            {
                                Cloud.battlepassHistory.chapterPassHistory_1.premiumRecieved[index] = true;
                            }
                            Cloud.battlepassHistory.UpdateHash().SetDirty(true);
                        }
                        else if (tier == 2)
                        {
                            currentData = StaticData.Wrapper.chapterBattlePass_2[index];
                            if (isnormal)
                            {
                                Cloud.battlepassHistory.chapterPassHistory_2.normalRecieved[index] = true;
                            }
                            else
                            {
                                Cloud.battlepassHistory.chapterPassHistory_2.premiumRecieved[index] = true;
                            }
                            Cloud.battlepassHistory.UpdateHash().SetDirty(true);
                        }
                        break;
                    default:
                        break;
                }

                if(currentData!=null)
                {
                    if(isnormal)
                    {
                        EarnReward(currentData.goodskey_normal, currentData.goodsAmount_normal);
                    }
                    else
                    {
                        EarnReward(currentData.goodskey_premium, currentData.goodsAmount_premium);
                    }
                }

                rewardedCallback?.Invoke(index,isnormal);
            }

            public static void EarnReward(RewardTypes rewardType,int amount)
            {
                switch (rewardType)
                {
                    case RewardTypes.None:
                        break;
                    case RewardTypes.Coin:
                        Player.ControllerGood.Earn(GoodsKey.Coin, amount);
                        break;
                    case RewardTypes.Dia:
                        Player.ControllerGood.Earn(GoodsKey.Dia, amount);
                        break;
                    case RewardTypes.StatusPoint:
                        break;
                    case RewardTypes.AwakeStone:
                        Player.ControllerGood.Earn(GoodsKey.AwakeStone, amount);
                        break;
                    case RewardTypes.ResearchPotion:
                        Player.ControllerGood.Earn(GoodsKey.ResearchPotion, amount);
                        break;
                    case RewardTypes.Pet:
                        break;
                    case RewardTypes.package_swordfewhit:
                    case RewardTypes.package_magicfewhit:
                    case RewardTypes.package_setturret:
                    case RewardTypes.package_companionspawn:
                    case RewardTypes.package_guidedmissile:
                    case RewardTypes.package_godmode:
                    case RewardTypes.package_summon:
                    case RewardTypes.package_nova:
                    case RewardTypes.package_meteor:
                    case RewardTypes.package_multielectric:
                        SkillKey skill = Player.Skill.RewardToSkill(rewardType);
                        Player.Skill.Obtain(skill, amount);
                        break;
                    case RewardTypes.skillAwakeStone:
                        GoodsKey goodkey = Player.ControllerGood.RewardToGoods(rewardType);
                        if (goodkey != GoodsKey.None)
                        {
                            Player.ControllerGood.Earn(goodkey, amount);
                        }
                        break;
                    case RewardTypes.riftDungeonKey:
                        Player.ControllerGood.Earn(GoodsKey.RiftDungeonTicket, amount);
                        break;
                    case RewardTypes.RPDungeonTicket:
                        Player.ControllerGood.Earn(GoodsKey.RPDungeonTicket, amount);
                        break;
                    case RewardTypes.ExpDungeonTicket:
                        Player.ControllerGood.Earn(GoodsKey.ExpDungeonTicket, amount);
                        break;
                    case RewardTypes.AwakeDungeonTicket:
                        Player.ControllerGood.Earn(GoodsKey.AwakeDungeonTicket, amount);
                        break;
                    case RewardTypes.Weapon_4_4:
                        Player.EquipItem.Obtain(EquipType.Weapon, 16, amount);
                        break;
                    case RewardTypes.Staff_4_4:
                        Player.EquipItem.Obtain(EquipType.Bow, 16, amount);
                        break;
                    case RewardTypes.Armor_4_4:
                        Player.EquipItem.Obtain(EquipType.Armor, 16, amount);
                        break;
                    case RewardTypes.Weapon_5_0:
                        Player.EquipItem.Obtain(EquipType.Weapon, 20, amount);
                        break;
                    case RewardTypes.Staff_5_0:
                        Player.EquipItem.Obtain(EquipType.Bow, 20, amount);
                        break;
                    case RewardTypes.Armor_5_0:
                        Player.EquipItem.Obtain(EquipType.Armor, 20, amount);
                        break;
                    case RewardTypes.Weapon_6_0:
                        Player.EquipItem.Obtain(EquipType.Weapon, 24, amount);
                        break;
                    case RewardTypes.Staff_6_0:
                        Player.EquipItem.Obtain(EquipType.Bow, 24, amount);
                        break;
                    case RewardTypes.Armor_6_0:
                        Player.EquipItem.Obtain(EquipType.Armor, 24, amount);
                        break;
                    default:
                        break;
                }

                string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.UI_GoodsRewarded].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
            }
        }
    }
}
