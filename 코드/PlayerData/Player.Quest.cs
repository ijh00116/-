using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Definition;
using UnityEngine.Events;
using System;
using BlackTree.Core;
using BlackTree.Bundles;
using Cysharp.Threading.Tasks;

namespace BlackTree.Model
{
    public enum PlayingRecordType
    {
        Daily,
        MainRepeat,
        SubRepeat,
    }
    public static partial class Player
    {
        public static class Quest
        {
            public static UnityAction onChangeMainQuest;
            public static UnityAction<QuestType> onChangeDailyRepeatQuest;
            public static UnityAction<QuestType> onChangeAfterRecieveDailyRepeatQuest;
            public static UnityAction onCompleteQuest;

            public static int mainQuestCurrentId => Cloud.playingRecord.mainQuest.id;
            public static QuestType CurrentMainQuestType => Cloud.playingRecord.mainQuest.questType;

            public static int mainQuestLoopStartIndex= 128;
            public static int dailyQuestMaxCount = 9;
            public const int attendQuestIndex = 12;
            public const int ReviewPopup = 50;
            public static Action<bool> otherUIActive;
            public static string GetCurrentDesc()
            {
                return StaticData.Wrapper.mainRepeatQuest[(int)mainQuestCurrentId].questType.ToString();
            }
            public static void Init()
            {
                Cloud.playingRecord.UpdateHash();

                //main init
                if(Cloud.playingRecord.mainQuest.id<0)
                {
                    Cloud.playingRecord.mainQuest.Init(StaticData.Wrapper.mainRepeatQuest[0]);
                }
                else
                {
                    if(Cloud.playingRecord.mainQuest.questType != StaticData.Wrapper.mainRepeatQuest[Cloud.playingRecord.mainQuest.id].questType)
                    {
                        Cloud.playingRecord.mainQuest.questType = StaticData.Wrapper.mainRepeatQuest[Cloud.playingRecord.mainQuest.id].questType;
                    }
                }

                for(int i=Cloud.playingRecord.dailyQuestCollections.Count; i<StaticData.Wrapper.dailyQuest.Length; i++)
                {
                    Cloud.playingRecord.dailyQuestCollections.Add(new PlayingQuestRecord()
                    {
                        questType = StaticData.Wrapper.dailyQuest[i].questType,
                        playingCount = 0,
                        id=i,
                        isRewarded=false,
                        isAdRewarded = false
                }) ;
                }

                for (int i = Cloud.playingRecord.repeatQuestCollections.Count; i < StaticData.Wrapper.subRepeatQuest.Length; i++)
                {
                    Cloud.playingRecord.repeatQuestCollections.Add(new PlayingQuestRecord()
                    {
                        questType = StaticData.Wrapper.subRepeatQuest[i].questType,
                        playingCount = 0,
                        id = i,
                        isRewarded = false,
                        isAdRewarded = false
                    });
                }

            }

            public static void TryCountUp(QuestType questType, int count,int subcount=0)
            {
                if (questType == CurrentMainQuestType)
                {
                    Cloud.playingRecord.mainQuest.playingCount += count;
                 
                    Cloud.playingRecord
                        .UpdateHash()
                        .SetDirty(true);

                    
                    LocalSaveLoader.SaveUserCloudData();
                    onChangeMainQuest?.Invoke();                                        
                }

                bool isTriggered = false;
                for(int i=0; i< Cloud.playingRecord.dailyQuestCollections.Count; i++)
                {
                    if(Cloud.playingRecord.dailyQuestCollections[i].questType==questType)
                    {
                        isTriggered = true;
                        Cloud.playingRecord.dailyQuestCollections[i].CountUp(count);
                        break;
                    }
                }
                for (int i = 0; i < Cloud.playingRecord.repeatQuestCollections.Count; i++)
                {
                    if (Cloud.playingRecord.repeatQuestCollections[i].questType == questType)
                    {
                        isTriggered = true;
                        Cloud.playingRecord.repeatQuestCollections[i].CountUp(count);
                        break;
                    }
                }
                if (isTriggered)
                    onChangeDailyRepeatQuest?.Invoke(questType);

                Cloud.playingRecord.UpdateHash().SetDirty(true);
                LocalSaveLoader.SaveUserCloudData();
            }

            public static int CurrentValue(PlayingRecordType recordType, QuestType questType)
            {
                int currentvalue = 0;
                PlayingQuestRecord data = null;
                switch (recordType)
                {
                    case PlayingRecordType.Daily:
                        data = Cloud.playingRecord.dailyQuestCollections.Find(o => o.questType == questType);
                        currentvalue =data.playingCount;
                        break;
                    case PlayingRecordType.MainRepeat:
                        currentvalue = Cloud.playingRecord.mainQuest.playingCount;

                        QuestType mainquesttype = StaticData.Wrapper.mainRepeatQuest[mainQuestCurrentId].questType;
                        if (mainquesttype == QuestType.StatLevel)
                        {
                            int levelValue = 0;
                            for(int i=0;i<Cloud.statusUpgrade.upgradeLevels.Count; i++)
                            {
                                levelValue += Cloud.statusUpgrade.upgradeLevels[i];
                            }
                            currentvalue = levelValue;
                        }
                        else if (mainquesttype == QuestType.UserLevel)
                        {
                            currentvalue = Player.Cloud.userLevelData.currentLevel;
                        }
                        else if (mainquesttype == QuestType.UserStage)
                        {
                            currentvalue = Player.Cloud.field.bestChapter*5+ Player.Cloud.field.bestStage;
                        }
                        else if (mainquesttype == QuestType.GoldUpgrade_Attack)
                        {
                            currentvalue = Cloud.goldUpgrade.upgradeLevels[(int)GoldUpgradeKey.AttackIncrease];
                        }
                        else if (mainquesttype == QuestType.GoldUpgrade_Hprecover)
                        {
                            currentvalue = Cloud.goldUpgrade.upgradeLevels[(int)GoldUpgradeKey.IncreaseHpRecover];
                        }
                        else if (mainquesttype == QuestType.GoldUpgrade_MaxHp)
                        {
                            currentvalue = Cloud.goldUpgrade.upgradeLevels[(int)GoldUpgradeKey.IncreaseHp];
                        }
                        else if (mainquesttype == QuestType.GoldUpgrade_CriRate)
                        {
                            currentvalue = Cloud.goldUpgrade.upgradeLevels[(int)GoldUpgradeKey.IncreaseCriticalRate];
                        }
                        else if (mainquesttype == QuestType.GoldUpgrade_MaxShield)
                        {
                            currentvalue = Cloud.goldUpgrade.upgradeLevels[(int)GoldUpgradeKey.IncreaseShield];
                        }
                        else if (mainquesttype == QuestType.GoldUpgrade_ShieldRecover)
                        {
                            currentvalue = Cloud.goldUpgrade.upgradeLevels[(int)GoldUpgradeKey.IncreaseShieldRecover];
                        }
                        else if (mainquesttype == QuestType.WitchSpeedIncrease)
                        {
                            currentvalue = Cloud.goldUpgrade.upgradeLevels[(int)GoldUpgradeKey.IncreaseMaxAtkSpeedForWitch];
                        }
                        else if (mainquesttype == QuestType.WitchSpeedMaxTimeIncrease)
                        {
                            currentvalue = Cloud.goldUpgrade.upgradeLevels[(int)GoldUpgradeKey.IncreaseCharacterAwakeTime];
                        }
                        else if (mainquesttype == QuestType.ActivateAdBuff)
                        {
                            if (Player.Cloud.inAppPurchase.purchaseAds)
                            {
                                currentvalue = 1;
                            }
                        }

                        break;
                    case PlayingRecordType.SubRepeat:
                        data = Cloud.playingRecord.repeatQuestCollections.Find(o => o.questType == questType);
                        currentvalue = data.playingCount;
                        break;
                }
                return currentvalue;
            }
            public static int GoalValue(PlayingRecordType recordType, QuestType questType)
            {
                DataQuest quest=null;
                int goal=0;
                switch (recordType)
                {
                    case PlayingRecordType.Daily:
                        for (int i = 0; i < StaticData.Wrapper.dailyQuest.Length; i++)
                        {
                            if (StaticData.Wrapper.dailyQuest[i].questType == questType)
                            {
                                quest = StaticData.Wrapper.dailyQuest[i];
                            }
                        }
                        goal = quest.goal;
                        break;
                    case PlayingRecordType.MainRepeat:
                        goal = StaticData.Wrapper.mainRepeatQuest[mainQuestCurrentId].goal;
                        break;
                    case PlayingRecordType.SubRepeat:
                        for (int i = 0; i < StaticData.Wrapper.subRepeatQuest.Length; i++)
                        {
                            if (StaticData.Wrapper.subRepeatQuest[i].questType == questType)
                            {
                                quest = StaticData.Wrapper.subRepeatQuest[i];
                            }
                        }
                        goal = quest.goal;
                        break;
                }
                return goal;
            }


            public static bool CanRecieve(PlayingRecordType recordType, QuestType questType,bool isAD=false)
            {
#if UNITY_EDITOR
                return true;
#endif
                bool isRewarded = false;
                PlayingQuestRecord data=null;
                switch (recordType)
                {
                    case PlayingRecordType.Daily:
                        if(isAD)
                        {
                            data = Cloud.playingRecord.dailyQuestCollections.Find(o => o.questType == questType);
                            isRewarded = data.isAdRewarded;
                        }
                        else
                        {
                            data = Cloud.playingRecord.dailyQuestCollections.Find(o => o.questType == questType);
                            isRewarded = data.isRewarded;
                        }
                        break;
                }
                if(isRewarded==true)
                {
                    return false;
                }

                bool isClear = false;
                isClear = CurrentValue(recordType, questType) >= GoalValue(recordType, questType);

                return isClear;
            }
            /// <summary>
            /// Only in daily
            /// </summary>
            /// <param name="questType"></param>
            /// <param name="isAD"></param>
            /// <returns></returns>
            public static bool isAlreadyRewarded(QuestType questType,bool isAD)
            {
                bool isRewarded = false;
                PlayingQuestRecord data = null;
                if (isAD)
                {
                    data = Cloud.playingRecord.dailyQuestCollections.Find(o => o.questType == questType);
                    if (data != null)
                        isRewarded = data.isAdRewarded;
                }
                else
                {
                    data = Cloud.playingRecord.dailyQuestCollections.Find(o => o.questType == questType);
                    if(data!=null)
                        isRewarded = data.isRewarded;
                }
                return isRewarded;
            }
            private static void GiveRewardAndNextQuest(PlayingRecordType recordType,QuestType questType,bool isAD=false)
            {
                DataQuest targetQuest=null;
                bool isClear = false;
                switch (recordType)
                {
                    case PlayingRecordType.MainRepeat:
                        targetQuest = StaticData.Wrapper.mainRepeatQuest[Cloud.playingRecord.mainQuest.id];
                        Player.ControllerGood.Earn(targetQuest.rewardGoodkey, targetQuest.rewardGoodValue);

#if UNITY_EDITOR
                        //if (Cloud.playingRecord.mainQuest.id < mainQuestLoopStartIndex)
                        //{
                        //    BackEnd.Param param = new BackEnd.Param();
                        //    param.Add("QuestClearID", Cloud.playingRecord.mainQuest.id.ToString());
                        //    param.Add("userID", Cloud.optiondata.useruuid.ToString());
                        //    Player.BackendData.LogEvent("quest_clear", param);
                        //}
#elif UNITY_ANDROID
                         if (Cloud.playingRecord.mainQuest.id < mainQuestLoopStartIndex)
                        {
                            BackEnd.Param param = new BackEnd.Param();
                            param.Add("QuestClearID", Cloud.playingRecord.mainQuest.id.ToString());
                            param.Add("userID", Cloud.optiondata.useruuid.ToString());
                            Player.BackendData.LogEvent("quest_clear", param);
                        }
#else
                  
#endif

                        int index =Cloud.playingRecord.mainQuest.id+1;
                        if (index >= StaticData.Wrapper.mainRepeatQuest.Length)
                            index = mainQuestLoopStartIndex;
                       
                        DataQuest nextQuest= StaticData.Wrapper.mainRepeatQuest[index];

                        Cloud.playingRecord.mainQuest.Init(nextQuest);

                        if(Cloud.playingRecord.mainQuest.id== attendQuestIndex)
                        {
                            var _viewAttend = ViewCanvas.Get<ViewCanvasAttendance>();
                            _viewAttend.SetVisible(true);
                            _viewAttend.Wrapped.CommonPopupOpenAnimation(() => {
                                //DVA2.Bundles.MainMenu.SetRedDot?.Invoke(_index, IsNextDay());
                            });
                        }
                        if (Cloud.playingRecord.mainQuest.id >= ReviewPopup)
                        {
                            if(Cloud.optiondata.isReview==false && Cloud.optiondata.isGuest==false)
                            {
                                var _viewReview = ViewCanvas.Get<ViewCanvasReview>();
                                _viewReview.SetVisible(true);
                                _viewReview.blackBG.PopupOpenColorFade();
                                _viewReview.Wrapped.CommonPopupOpenAnimation(() => {
                                    //DVA2.Bundles.MainMenu.SetRedDot?.Invoke(_index, IsNextDay());
                                });
                                Cloud.optiondata.isReview = true;
                                Cloud.optiondata.UpdateHash().SetDirty(true);
                            }
                        }
                        StartTutorial();

                        Cloud.playingRecord.PlayingMainQuestCount++;
                        break;
                    case PlayingRecordType.Daily:
                        for (int i = 0; i < StaticData.Wrapper.dailyQuest.Length; i++)
                        {
                            if (StaticData.Wrapper.dailyQuest[i].questType == questType)
                            {
                                targetQuest = StaticData.Wrapper.dailyQuest[i];
                            }
                        }
                        var dailyquestinfo = Cloud.playingRecord.dailyQuestCollections.Find(o => o.questType == questType);
                        
                        if (dailyquestinfo.playingCount >= targetQuest.goal)
                        {
                            Player.ControllerGood.Earn(targetQuest.rewardGoodkey, (isAD)? targetQuest.rewardGoodValue*2:targetQuest.rewardGoodValue);
                            if(isAD)
                            {
                                dailyquestinfo.isAdRewarded = true;
                            }
                            else
                            {
                                dailyquestinfo.isRewarded = true;
                            }
                        }
                      
                        break;
                    case PlayingRecordType.SubRepeat:
                        for (int i = 0; i < StaticData.Wrapper.subRepeatQuest.Length; i++)
                        {
                            if (StaticData.Wrapper.subRepeatQuest[i].questType == questType)
                            {
                                targetQuest = StaticData.Wrapper.subRepeatQuest[i];
                            }
                        }
                        var repeatquestinfo = Cloud.playingRecord.repeatQuestCollections.Find(o => o.questType == questType);

                        if (repeatquestinfo.playingCount >= targetQuest.goal)
                        {
                            Player.ControllerGood.Earn(targetQuest.rewardGoodkey, targetQuest.rewardGoodValue);
                            repeatquestinfo.playingCount -= targetQuest.goal;
                        }
                        break;
                    default:
                        break;
                }
                Player.Quest.TryCountUp(QuestType.QuestClear, 1);
            }

            public static void StartTutorial()
            {
                if (Cloud.playingRecord.mainQuest.questType == QuestType.GoldUpgrade_Attack)
                {
                    Player.Guide.StartTutorial(Definition.TutorialType.GoldUpgrade, true);
                }
                if (Cloud.playingRecord.mainQuest.questType == QuestType.StatLevel)
                {
                    Player.Guide.StartTutorial(Definition.TutorialType.StatusUpgrade, true);
                }
                if (Cloud.playingRecord.mainQuest.questType == QuestType.SummonSkill)
                {
                    Player.Guide.StartTutorial(Definition.TutorialType.SummonSkill, true);
                }
                if (Cloud.playingRecord.mainQuest.questType == QuestType.SummonEquip)
                {
                    Player.Guide.StartTutorial(Definition.TutorialType.SummonEquip, true);
                }
                if (Cloud.playingRecord.mainQuest.questType == QuestType.RPDungeonClear)
                {
                    Player.Guide.StartTutorial(Definition.TutorialType.RPDungeonClear, true);
                }
                if (Cloud.playingRecord.mainQuest.questType == QuestType.ExpDungeonClear)
                {
                    Player.Guide.StartTutorial(Definition.TutorialType.ExpDungeonClear, true);
                }
                if (Cloud.playingRecord.mainQuest.questType == QuestType.AwakeDungeonClear)
                {
                    Player.Guide.StartTutorial(Definition.TutorialType.AwakeDungeonClear, true);
                }
                if (Cloud.playingRecord.mainQuest.questType == QuestType.SummonPet)
                {
                    Player.Guide.StartTutorial(Definition.TutorialType.SummonPet, true);
                }


                if (Cloud.playingRecord.mainQuest.questType == QuestType.AwakeWitch)
                {
                    Player.Guide.StartTutorial(Definition.TutorialType.CharacterAwake, true);
                }
                if (Cloud.playingRecord.mainQuest.questType == QuestType.ActivateAdBuff)
                {
                    Player.Guide.StartTutorial(Definition.TutorialType.AdBuff, true);
                }
                if (Cloud.playingRecord.mainQuest.questType == QuestType.AwakeUpgrade)
                {
                    Player.Guide.StartTutorial(Definition.TutorialType.AwakeUpgrade, true);
                }
                if (Cloud.playingRecord.mainQuest.questType == QuestType.SkillAwake)
                {
                    if(Player.Skill.Get(SkillKey.GuidedMissile).IsUnlocked==false)
                    {
                        Player.Skill.Obtain(SkillKey.GuidedMissile, 1);
                    }
                    Player.Guide.StartTutorial(Definition.TutorialType.SkillAwake, true);
                }
                if (Cloud.playingRecord.mainQuest.questType == QuestType.ResearchContentsClear)
                {
                    Player.Guide.StartTutorial(Definition.TutorialType.RPContentClear, true);
                }
                if (Cloud.playingRecord.mainQuest.questType == QuestType.RaidContent)
                {
                    Player.Guide.StartTutorial(Definition.TutorialType.RaidStart, true);
                }
                if (Cloud.playingRecord.mainQuest.questType == QuestType.ActivateAutoSkill)
                {
                    Player.Guide.StartTutorial(Definition.TutorialType.PushAutoSkill, true);
                }
            }
            public static void GiveReward(PlayingRecordType recordType, QuestType questType,bool isAD=false)
            {
                if(CanRecieve(recordType,questType, isAD))
                {
                    GiveRewardAndNextQuest(recordType, questType, isAD);

                    if(recordType==PlayingRecordType.Daily||recordType==PlayingRecordType.SubRepeat)
                    {
                        Player.Quest.TryCountUp(QuestType.QuestDailyComplete, 1);
                    }
                }


                if(recordType==PlayingRecordType.MainRepeat)
                {
                    onChangeMainQuest?.Invoke();
                }
                else
                {
                    onChangeDailyRepeatQuest?.Invoke(questType);
                }

                Player.Option.ContentUnlockUpdate?.Invoke();

                Cloud.playingRecord
                    .UpdateHash()
                    .SetDirty(true);

                LocalSaveLoader.SaveUserCloudData();
            }

            public static void GiveRewardToRepeat(PlayingRecordType recordType, QuestType questType, bool isAD = false)
            {
                DataQuest targetQuest = null;
                bool isClear = false;
                if(recordType==PlayingRecordType.SubRepeat)
                {
                    for (int i = 0; i < StaticData.Wrapper.subRepeatQuest.Length; i++)
                    {
                        if (StaticData.Wrapper.subRepeatQuest[i].questType == questType)
                        {
                            targetQuest = StaticData.Wrapper.subRepeatQuest[i];
                        }
                    }
                    var repeatquestinfo = Cloud.playingRecord.repeatQuestCollections.Find(o => o.questType == questType);

                    if(repeatquestinfo.playingCount>=targetQuest.goal)
                    {
                        int rewardCount = repeatquestinfo.playingCount / targetQuest.goal;
                        int leftCount= repeatquestinfo.playingCount % targetQuest.goal;

                        Player.ControllerGood.Earn(targetQuest.rewardGoodkey, targetQuest.rewardGoodValue* rewardCount);

                        if (recordType == PlayingRecordType.SubRepeat)
                        {
                            Player.Quest.TryCountUp(QuestType.QuestDailyComplete, rewardCount);
                        }
                        repeatquestinfo.playingCount = leftCount;
                    }
                
                    onChangeDailyRepeatQuest?.Invoke(questType);
                    Player.Quest.TryCountUp(QuestType.QuestClear, 1);
                }
            }
        }
    }
}
