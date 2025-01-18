using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Definition;
using BlackTree.Core;

namespace BlackTree.Model
{
    public static partial class Player
    {
        public static class Level
        {
            public static double currentmaxExp;
            public static double currentExp=>Cloud.userLevelData.currentExp;

            public static System.Action callbackGetExp;

            const float exp_eff = 1.097f;
            const float exp_eff_ForMax = 1.9f;
            const float exp_eff_ForMultiply = 1.003f;

            public const int statusPointForLvup=10;

            public static System.Action levelupCallBack;
            const int levelForfixedmaxExp_2 = 1500;
            const int levelForfixedmaxExp_1 = 1000;
            const int levelForfixedmaxExp_0 = 460;
            static double expForcalculateMaxExp;
            const double killMonsterCountPersec = 12;
            const int startExpTime = 360;
            const int expTimeDelta_0 = 8;
            const int expTimeDelta_1 = 16;
            const int expTimeDelta_2 = 24;
            public static void Init()
            {
                expForcalculateMaxExp = Battle.Field.defaultRewardExpForStage[499];
                
                CalculateMaxExp();

                //currentexp Calculate
                if(Player.Cloud.userLevelData.currentExp> currentmaxExp)
                {
                    int currentLv = Player.Cloud.userLevelData.currentLevel;
                    double originalMaxexp = 100 * (System.Math.Pow(exp_eff, currentLv - 1)) * System.Math.Pow(currentLv, exp_eff_ForMax) * System.Math.Pow(exp_eff_ForMultiply, (float)currentLv / 5.0f);
                    double _valuePer = Player.Cloud.userLevelData.currentExp / originalMaxexp;

                    Player.Cloud.userLevelData.currentExp = _valuePer * currentmaxExp;
                }
            }

            public static void ExpUpAndLvUp(double exp)
            {
                //Debug.Log($"°æÇèÄ¡ È¹µæ:{exp}");
                float expVipTimes = 1;
                if (Player.Cloud.inAppPurchase.purchaseVip == 1)
                {
                    expVipTimes = StaticData.Wrapper.vIPFixedRewardsDatas[0].expFixedRewardTimes;
                }
                if (Player.Cloud.inAppPurchase.purchaseVip == 2)
                {
                    expVipTimes = StaticData.Wrapper.vIPFixedRewardsDatas[1].expFixedRewardTimes;
                }
                if (Player.Cloud.inAppPurchase.purchaseVip == 3)
                {
                    expVipTimes = StaticData.Wrapper.vIPFixedRewardsDatas[2].expFixedRewardTimes;
                }

                double calculatedExp=exp* (1 + Player.Pet._equipAbilitycaches[EquipAbilityKey.GetExpIncrease] * 0.01f)* expVipTimes;
                if (Battle.Field.IsFeverTime)
                {
                    calculatedExp = calculatedExp*Battle.Field.goldexpRate;
                }
                Player.Cloud.userLevelData.currentExp += calculatedExp;

                if (Player.Cloud.userLevelData.currentExp>=currentmaxExp)
                {
                    Player.Cloud.userLevelData.currentExp = 0;
                    Player.Cloud.userLevelData.currentLevel++;

                    Player.Pass.levelCallBack?.Invoke();

                    if (Player.Cloud.userLevelData.currentLevel>=10)
                    {
                        LogToFirebase(Player.Cloud.userLevelData.currentLevel);
                    }
                    

                    Player.Quest.TryCountUp(QuestType.UserLevel, 1);

                    Unit.RecoverForLvUp();
                    levelupCallBack?.Invoke();
                    CalculateMaxExp();
                    Player.ControllerGood.Earn(GoodsKey.StatusPoint, statusPointForLvup);

                    Player.BackendData.NormalRankingUpdate(StaticData.Wrapper.ingameRankingNameData[(int)Player.BackendData.NormalRankingType.LevelRanking].titleName);
               

                    Player.Option.ContentUnlockUpdate?.Invoke();
                }

                Cloud.userLevelData.UpdateHash().SetDirty(true);
                LocalSaveLoader.SaveUserCloudData();

                callbackGetExp?.Invoke();
            }

            public static void LevelUpCallback()
            {
                Player.Quest.TryCountUp(QuestType.UserLevel, 1);

                Unit.RecoverForLvUp();
                levelupCallBack?.Invoke();
                CalculateMaxExp();
                Player.ControllerGood.Earn(GoodsKey.StatusPoint, statusPointForLvup);

                Player.Option.ContentUnlockUpdate?.Invoke();
            }
            static void CalculateMaxExp()
            {
                int currentLv = Player.Cloud.userLevelData.currentLevel;

                if(currentLv<= levelForfixedmaxExp_0)
                {
                    currentmaxExp = 100 * (System.Math.Pow(exp_eff, currentLv - 1)) * System.Math.Pow(currentLv, exp_eff_ForMax) * System.Math.Pow(exp_eff_ForMultiply, (float)currentLv / 5.0f);
                }
                else if(currentLv<=levelForfixedmaxExp_1)
                {
                    int data = currentLv - levelForfixedmaxExp_0;
                    currentmaxExp = expForcalculateMaxExp * (startExpTime + expTimeDelta_0 * data) * killMonsterCountPersec;
                }
                else if (currentLv <= levelForfixedmaxExp_2)
                {
                    int data_0 = levelForfixedmaxExp_1 - levelForfixedmaxExp_0;
                    double beforedataexp = expForcalculateMaxExp * (startExpTime + expTimeDelta_0 * data_0) * killMonsterCountPersec;

                    int data = currentLv - levelForfixedmaxExp_1;
                    currentmaxExp = (expForcalculateMaxExp * (startExpTime + expTimeDelta_1 * data) * killMonsterCountPersec)+ beforedataexp;
                }
                else
                {
                    int data_0 = levelForfixedmaxExp_1 - levelForfixedmaxExp_0;
                    double beforedataexp_0 = expForcalculateMaxExp * (startExpTime + expTimeDelta_0 * data_0) * killMonsterCountPersec;

                    int data_1 = levelForfixedmaxExp_2 - levelForfixedmaxExp_1;
                    double beforedataexp_1 = expForcalculateMaxExp * (startExpTime + expTimeDelta_1 * data_1) * killMonsterCountPersec;

                    int data = currentLv - levelForfixedmaxExp_2;
                    currentmaxExp = (expForcalculateMaxExp * (startExpTime + expTimeDelta_2 * data) * killMonsterCountPersec)+ beforedataexp_0+ beforedataexp_1;
                }


                //Debug.Log($"°æÇèÄ¡ °è»ê ¿Ï·á ¸Æ½º °æÄ¡:{currentmaxExp}");
            }

            static void LogToFirebase(int qIndex)
            {
                FirebaseManager.Instance.LogEvent("Level",
                            new Firebase.Analytics.Parameter("levelData", qIndex.ToString()));
            }
        }
    }
}
