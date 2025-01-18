using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Definition;
using BlackTree.Core;

namespace BlackTree.Model
{
    public enum MenuType
    {
        Option=0,
        Mail,
        Attend,
        Inform,
        
        End
    }
    public static partial class Player
    {
        public static class Option
        {
            public static System.Action ContentUnlockUpdate;

            public static System.Action<bool> autoBossActive;

            public static System.Action<MenuType,bool> menuRedDotCallback;

            public static System.Action<bool> questRedDotCallback;

            public static System.Action MailUpdate;

            public static System.Action<bool> MenuContentBarOff;

            public static System.Action stageUIOff;

            public static System.Action stageAllReward;
            public static System.Action AnotherDaySetting;
            public static bool isAnotherDay()
            {
                int today = Extension.GetServerTime().DayOfYear;
                if (today != Player.Cloud.optiondata.lastLoginedDay)
                {
                    return true;
                }

                return false;
            }

            public static void SetLoginedDay()
            {
                int today = Extension.GetServerTime().DayOfYear;

                if(Player.Cloud.optiondata.lastLoginedDay!=today)
                {
                    Player.Cloud.optiondata.lastLoginedDay = today;

                    Player.Cloud.optiondata.UpdateHash().SetDirty(true);
                    LocalSaveLoader.SaveUserCloudData();
                }
                
            }


            public static ContentState IsSkillSlotUnlocked(int slotindex)
            {
                var tabledata = StaticData.Wrapper.skillLockTabledata[slotindex];
                ContentState state = ContentState.Locked;

                switch (tabledata.lockType)
                {
                    case ContentLockType.UnitLevel:
                        if (Player.Cloud.userLevelData.currentLevel >= tabledata.unlockLevel)
                        {
                            state = ContentState.UnLocked;
                        }
                        break;
                    case ContentLockType.MainMissionLevel:
                        if (Player.Cloud.playingRecord.mainQuest.id >= tabledata.unlockLevel)
                        {
                            state = ContentState.UnLocked;
                        }
                        break;
                    case ContentLockType.ChapterLevel:
                        if (Player.Cloud.field.chapter >= tabledata.unlockLevel)
                        {
                            state = ContentState.UnLocked;
                        }
                        break;
                    default:
                        break;
                }
                return state;
            }

            public static string SkillUnlockMessage(int slotindex)
            {
                var tabledata = StaticData.Wrapper.skillLockTabledata[slotindex];
                string temp = null;

                string localized = "";
                switch (tabledata.lockType)
                {
                    case ContentLockType.UnitLevel:
                        if (Player.Cloud.userLevelData.currentLevel < tabledata.unlockLevel)
                        {
                            localized = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_UnlockLevel].StringToLocal;
                            temp = string.Format(localized, tabledata.unlockLevel);
                        }
                        break;
                    case ContentLockType.MainMissionLevel:
                        if (Player.Cloud.playingRecord.mainQuest.id < tabledata.unlockLevel)
                        {
                            localized = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_UnlockQuest].StringToLocal;
                            temp = string.Format(localized, tabledata.unlockLevel);
                        }
                        break;
                    case ContentLockType.ChapterLevel:
                        if (Player.Cloud.field.chapter < tabledata.unlockLevel)
                        {
                            localized = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_UnlockChapter].StringToLocal;
                            temp = string.Format(localized, tabledata.unlockLevel);
                        }
                        break;
                    default:
                        break;
                }
                return temp;
            }

            public static bool IsContentUIUnlocked(LockedUIType uiLockType)
            {
                bool isComplete = false;
                var uiLockTypeData= StaticData.Wrapper.ingameLockData[(int)uiLockType];
                switch (uiLockTypeData.lockType)
                {
                    case ContentLockType.UnitLevel:
                        if (Player.Cloud.userLevelData.currentLevel >= uiLockTypeData.unLockLevel)
                        {
                            isComplete = true;
                        }
                        break;
                    case ContentLockType.MainMissionLevel:
                        if (Player.Cloud.playingRecord.mainQuest.id >= uiLockTypeData.unLockLevel)
                        {
                            isComplete = true;
                        }
                        break;
                    case ContentLockType.ChapterLevel:
                        if (Player.Cloud.field.chapter >= uiLockTypeData.unLockLevel)
                        {
                            isComplete = true;
                        }
                        break;
                    default:
                        break;
                }
                return isComplete;
            }

            public static int ContentUIUnlockLevel(LockedUIType uiLockType)
            {
                bool isComplete = false;
                var uiLockTypeData = StaticData.Wrapper.ingameLockData[(int)uiLockType];
                return uiLockTypeData.unLockLevel;
            }

            public static ContentState IsPetSlotUnlocked(int slotindex)
            {
                var tabledata = StaticData.Wrapper.petLockTabledata[slotindex];
                ContentState state = ContentState.Locked;

                switch (tabledata.lockType)
                {
                    case ContentLockType.UnitLevel:
                        if (Player.Cloud.userLevelData.currentLevel >= tabledata.unlockLevel)
                        {
                            state = ContentState.UnLocked;
                        }
                        break;
                    case ContentLockType.MainMissionLevel:
                        if (Player.Cloud.playingRecord.mainQuest.id +1>= tabledata.unlockLevel)
                        {
                            state = ContentState.UnLocked;
                        }
                        break;
                    case ContentLockType.ChapterLevel:
                        if (Player.Cloud.field.chapter+1 >= tabledata.unlockLevel)
                        {
                            state = ContentState.UnLocked;
                        }
                        break;
                    default:
                        break;
                }
                return state;
            }

            public static ContentState IsRuneSlotUnlocked(int slotindex)
            {
                var tabledata = StaticData.Wrapper.runeLockTabledata[slotindex];
                ContentState state = ContentState.Locked;

                switch (tabledata.lockType)
                {
                    case ContentLockType.UnitLevel:
                        if (Player.Cloud.userLevelData.currentLevel >= tabledata.unlockLevel)
                        {
                            state = ContentState.UnLocked;
                        }
                        break;
                    case ContentLockType.MainMissionLevel:
                        if (Player.Cloud.playingRecord.mainQuest.id + 1 >= tabledata.unlockLevel)
                        {
                            state = ContentState.UnLocked;
                        }
                        break;
                    case ContentLockType.ChapterLevel:
                        if (Player.Cloud.field.chapter + 1 >= tabledata.unlockLevel)
                        {
                            state = ContentState.UnLocked;
                        }
                        break;
                    default:
                        break;
                }
                return state;
            }
        }
    }
}