using System;
using System.Collections.Generic;
using BlackTree.Definition;
using UnityEngine.Events;
using UnityEngine;
using BlackTree.Model;
using BlackTree.Core;

namespace BlackTree.Model
{
    public class TutorialDescData
    {
        public TutorialTypeConfigure tutoConfig;
        public LocalizeDescKeys descKey;
    }
    public static partial class Player
    {
        public static class Guide
        {
            public static Dictionary<TutorialType, Queue<TutorialDescData>> currentTutorialQueue = new Dictionary<TutorialType, Queue<TutorialDescData>>();
            public static Action<TutorialDescData> tutorialConfigAction;
            public static Action TutorialEndcallback;
            public static TutorialType currentTutorial;

            public static Dictionary<QuestGuideType, List<QuestGuideTypeConfigure>> tableQuestGuideData = new Dictionary<QuestGuideType, List<QuestGuideTypeConfigure>>();
            public static Queue<QuestGuideTypeConfigure> currentProgressGuide = new Queue<QuestGuideTypeConfigure>();
            public static Action<QuestGuideTypeConfigure> questGuideAction;
            public static Action QuestGuideEndcallback;
            public static QuestGuideType currentGuideQuest;

            public static void Init()
            {
                for(int i= Cloud.tutorialData.tutorialType.Count; i<(int)TutorialType.End; i++)
                {
                    Cloud.tutorialData.tutorialType.Add((TutorialType)i);
                    Cloud.tutorialData.tutorialCleared.Add(false);
                }

                for (int i = 0; i < (int)TutorialType.End; i++)
                {
                    FirstSetTutorial((TutorialType)i);
                }

                for(int i=0; i<StaticData.Wrapper.questGuidedata.Length; i++)
                {
                    var data = StaticData.Wrapper.questGuidedata[i];
                    List<QuestGuideTypeConfigure> configlist = new List<QuestGuideTypeConfigure>();
                    for (int j=0; j<data.questGuideConfig.Length; j++)
                    {
                        configlist.Add(data.questGuideConfig[j]);
                    }
                    tableQuestGuideData.Add(data.questGuideType, configlist);
                }
            }
            #region tutorial
            
            public static LocalizeDescKeys GetDialog(TutorialDescData type)
            {
                return type.descKey;
            }

            public static void FirstSetTutorial(TutorialType tutorialtype)
            {
                if (Cloud.tutorialData.tutorialCleared[(int)tutorialtype])
                    return;

                Queue<TutorialDescData> tutoqueue;
                if (currentTutorialQueue.ContainsKey(tutorialtype))
                {
                    tutoqueue = currentTutorialQueue[tutorialtype];
                }
                else
                {
                    tutoqueue = new Queue<TutorialDescData>();
                    currentTutorialQueue.Add(tutorialtype,tutoqueue);
                }
                tutoqueue.Clear();

                for(int i=0; i< StaticData.Wrapper.tutorialData[(int)tutorialtype].tutorialConfig.Length; i++)
                {
                    var tutoConfig = StaticData.Wrapper.tutorialData[(int)tutorialtype].tutorialConfig[i];
                    var desc = StaticData.Wrapper.tutorialData[(int)tutorialtype].dialogDesc[i];
                    TutorialDescData data = new TutorialDescData() { tutoConfig = tutoConfig, descKey = desc };
                    tutoqueue.Enqueue(data);
                }
            }

            public static TutorialDescData StartTutorial(TutorialType tutorialtype, bool start = false)
            {
                if(start)
                {
                    currentTutorial = tutorialtype;
#if UNITY_EDITOR
                                //BackEnd.Param param = new BackEnd.Param();
                                //param.Add("TutorialType", tutorialtype.ToString()+"_0");
                                //Player.BackendData.LogEvent("TutorialClear", param);
#elif UNITY_ANDROID
                                BackEnd.Param param = new BackEnd.Param();
                                param.Add("TutorialType", tutorialtype.ToString()+"_0");
                                Player.BackendData.LogEvent("ChapterInfo", param);
#else
#endif
                }
                if (currentTutorial != tutorialtype)
                    return null;
                if (Cloud.tutorialData.tutorialCleared[(int)tutorialtype])
                    return null;
                if (currentTutorialQueue[tutorialtype].Count <= 0)
                {
                    TutorialEndcallback?.Invoke();
                    currentTutorial = TutorialType.None;

                    Time.timeScale = 1;
                    Cloud.tutorialData.tutorialCleared[(int)tutorialtype] = true;

                    Cloud.tutorialData.UpdateHash().SetDirty(true);
                    LocalSaveLoader.SaveUserCloudData();

#if UNITY_EDITOR
                    //BackEnd.Param param = new BackEnd.Param();
                    //param.Add("TutorialType", tutorialtype.ToString() + "_1");
                    //Player.BackendData.LogEvent("TutorialClear", param);
#elif UNITY_ANDROID
                                  BackEnd.Param param = new BackEnd.Param();
                    param.Add("TutorialType", tutorialtype.ToString() + "_1");
                    Player.BackendData.LogEvent("TutorialClear", param);
#else
#endif

                    return null;
                }

                var tutoConfigType = currentTutorialQueue[tutorialtype].Dequeue();
                tutorialConfigAction?.Invoke(tutoConfigType);

                return tutoConfigType;
            }

            #endregion

            #region quest
            public static void QuestSetQueue(QuestGuideType questGuideType)
            {
                if(currentGuideQuest!=QuestGuideType.None)
                {
                    QuestGuideEndcallback?.Invoke();
                }
                currentProgressGuide.Clear();

                for (int i = 0; i < tableQuestGuideData[questGuideType].Count; i++)
                {
                    var data = tableQuestGuideData[questGuideType];
                    currentProgressGuide.Enqueue(data[i]);
                }
            }

            public static void  QuestGuideProgress(QuestGuideType questguidetype, bool start = false)
            {
                if (start)
                {
                    currentGuideQuest = questguidetype;
                }
                if (currentGuideQuest != questguidetype)
                    return;
            
                if (currentProgressGuide.Count <= 0)
                {
                    QuestGuideEndcallback?.Invoke();
                    currentGuideQuest = QuestGuideType.None;

                    return;
                }

                var questConfigType = currentProgressGuide.Dequeue();
                questGuideAction?.Invoke(questConfigType);
            }
            #endregion
        }
    }
}