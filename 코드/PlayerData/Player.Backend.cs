using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Definition;
using UnityEngine.Events;
using System;
using BlackTree.Core;
using BlackTree.Bundles;
using LitJson;
using BackEnd;
using Cysharp.Threading.Tasks;

namespace BlackTree.Model
{
    public static partial class Player
    {
        public static class BackendData
        {
            public enum NormalRankingType
            {
                LevelRanking=0,
                StageRanking,
                RaidRanking,
            }
            [Serializable]
            public class BackendString
            {
                public string S;
            }
            [Serializable]
            public class BackendDouble
            {
                public double N;
            }
            [Serializable]
            public class BackEndGameInfoData
            {
                public BackendString owner_inDate;
                public BackendString client_date;
                public BackendString inDate;
                public BackendString updatedAt;
            }
            [Serializable]
            public class BackendRankingTableData
            {
                public BackendString rankType;
                public BackendString uuid;
                public BackendString title;
            }

            [Serializable]
            public class BackendRankData
            {
                public BackendString nickname;
                public BackendDouble score;
                public BackendDouble rank;
                public BackendDouble UserLevel;
                public Player.BackendData.NormalRankingType rankType;
            }

            public static BackEndGameInfoData raidDmgData;
            public static BackEndGameInfoData normalGameData;

            public const string raidTableName="RaidDmgData";
            public const string normalRankingTableName = "NormalRanking";

            public static List<BackendRankingTableData> rankinfoList = new List<BackendRankingTableData>();
            public const int indexConstForraidRanking=100;

            public static List<BackendRankData> raidRankList = new List<BackendRankData>();
            public static List<BackendRankData> levelRankList = new List<BackendRankData>();
            public static List<BackendRankData> stageRankList = new List<BackendRankData>();

            public static BackendRankData myRaidRankInfo=null;
            public static BackendRankData mylevelRankInfo;
            public static BackendRankData mystageRankInfo;

            public static BackendRankingTableData currentRaidRankData;

            public static float currentNormalRankInitTime = 0;
            public static float InitNormalRankTime = 1800;//3600000

            //Player.BackendData.NormalRankingType currentActivateRankType;
            public static void FirstUserSetting()
            {
                BackEnd.Param param_ranking = new BackEnd.Param();
                param_ranking.Add("StageScore", 0);
                param_ranking.Add("UserLevel", 1);
                BackEnd.Backend.GameData.Insert(normalRankingTableName, param_ranking);
            }

            public static void FirstRaidSetting()
            {
                BackEnd.Param param = new BackEnd.Param();
                param.Add("RaidScore", 0);
                param.Add("UserLevel", 1);
    
                BackEnd.Backend.GameData.Insert(raidTableName, param);

                Player.Cloud.userRaidRankingData.isRaidFirstSet = true;
            }

            /// <summary>
            /// call after load all data
            /// </summary>
            public static void BackendInit()
            {
                BackEnd.Where where = new BackEnd.Where();
                where.Equal("owner_inDate", BackEnd.Backend.UserInDate);

                //raid는 추후 작업후 주석 풀기
                var bro = BackEnd.Backend.GameData.GetMyData(raidTableName, where, 10);
                JsonData raidgameinfodata = bro.GetReturnValuetoJSON()["rows"][0];
                raidDmgData = JsonUtility.FromJson<BackEndGameInfoData>(raidgameinfodata.ToJson().ToString());

                var Ingamebro = BackEnd.Backend.GameData.GetMyData(normalRankingTableName, where, 10);
                JsonData ingameinfodata = Ingamebro.GetReturnValuetoJSON()["rows"][0];
                normalGameData = JsonUtility.FromJson<BackEndGameInfoData>(ingameinfodata.ToJson().ToString());

                JsonData json = BackEnd.Backend.URank.User.GetRankTableList().GetReturnValuetoJSON()["rows"];

                foreach (LitJson.JsonData data in json)
                {
                    var rankingTableData = JsonUtility.FromJson<BackendRankingTableData>(data.ToJson().ToString());
                    rankinfoList.Add(rankingTableData);
                }
            }

            static BackendRankingTableData GetRaidRankuuid(int chapterlv)
            {
                //raid 작업 후 추후 주석 풀기
                //return null;

                int index = (int)(chapterlv / indexConstForraidRanking);

                var titlename = StaticData.Wrapper.backEndRaidRankingNameDatas[index].titleName;

                var data = rankinfoList.Find(o => o.title.S == titlename);

                return data;
            }

            static BackendRankingTableData GetRankuuid(string titleName)
            {
                var data = rankinfoList.Find(o => o.title.S == titleName);

                return data;
            }

            public static void RaidRankingUpdate(double score)
            {
#if UNITY_EDITOR
#else
                string rowUUID = raidDmgData.inDate.S;
                BackEnd.Param param = new BackEnd.Param();
                param.Add("RaidScore", score);            
                param.Add("UserLevel", Player.Cloud.userLevelData.currentLevel);
   
                currentRaidRankData = GetRaidRankuuid(Cloud.field.bestChapter);

                //rank update
                if(Player.Cloud.userRaidData.currentRegisteredRankUUID!= currentRaidRankData.uuid.S &&string.IsNullOrEmpty(Player.Cloud.userRaidData.currentRegisteredRankUUID)==false)
                {
                    BackEnd.Param beforeparam = new BackEnd.Param();
                    beforeparam.Add("RaidScore", -1);
                    beforeparam.Add("UserLevel", Player.Cloud.userLevelData.currentLevel);
        
                    BackEnd.Backend.URank.User.UpdateUserScore(Player.Cloud.userRaidData.currentRegisteredRankUUID, raidTableName, rowUUID, beforeparam);
                }

                var bro= BackEnd.Backend.URank.User.UpdateUserScore(currentRaidRankData.uuid.S,raidTableName, rowUUID,  param);
                var status = bro.GetStatusCode();

                if(status.Equals("204"))
                {
                    Player.Cloud.userRaidData.currentRegisteredRankUUID= currentRaidRankData.uuid.S;
                    Player.Cloud.userRaidData.currentRegisteredRankTitle = currentRaidRankData.title.S;
                    LocalSaveLoader.SaveUserCloudData();
                    Player.Cloud.userRaidData.SetDirty(true).UpdateHash();
                }

                //Debug.Log(string.Format("랭킹 업뎃 결과: statuscode:{0}", status));
#endif
            }

            /// <summary>
            /// 노말 랭킹 업데이트 등록
            /// </summary>
            /// <param name="titleName"></param>
            public static void NormalRankingUpdate(string titleName)
            {
#if UNITY_EDITOR
                //if (Player.Cloud.field.bestChapter < Battle.Field.RankingUnlockedChapterIndex)
                //    return;

                //string rowUUID = normalGameData.inDate.S;
                //BackEnd.Param param = new BackEnd.Param();
                //if (titleName == StaticData.Wrapper.ingameRankingNameData[(int)NormalRankingType.LevelRanking].titleName)
                //{
                //    int _score = Player.Cloud.userLevelData.currentLevel;
                //    param.Add("UserLevel", _score);
                //}
                //else
                //{
                //    int _score = Player.Cloud.field.bestChapter * 100 + Player.Cloud.field.bestStage;
                //    param.Add("StageScore", _score);
                //}

                //var rankData = GetRankuuid(titleName);


                //BackEnd.Backend.URank.User.UpdateUserScore(rankData.uuid.S, normalRankingTableName, rowUUID, param, (bro) => {
                //    var status = bro.GetStatusCode();

                //    if (status.Equals("204"))
                //    {
                //        LocalSaveLoader.SaveUserCloudData();
                //        Player.Cloud.userRaidData.SetDirty(true).UpdateHash();
                //    }

                //    //Debug.Log(string.Format("랭킹 업뎃 결과: statuscode:{0}", status));
                //});
#else
                if (Player.Cloud.field.bestChapter < Battle.Field.RankingUnlockedChapterIndex)
                    return;

                //초기화 후 랭크 다시 갱신시 검사
                if (titleName == StaticData.Wrapper.ingameRankingNameData[(int)NormalRankingType.StageRanking].titleName)
                {
                    var rankDataStageRank = GetRankuuid(StaticData.Wrapper.ingameRankingNameData[(int)NormalRankingType.StageRanking].titleName);
                    var bro = Backend.URank.User.GetMyRank(rankDataStageRank.uuid.S);
                    if (bro.GetStatusCode() == "404")
                    {
                        Player.Cloud.field.bestChapterForBackEndRanking = Player.Cloud.field.chapter;
                        Player.Cloud.field.bestStageForBackEndRanking = Player.Cloud.field.stage;
                        Player.Cloud.field.SetDirty(true).UpdateHash();
                    }
                    if (bro.GetStatusCode() == "200")
                    {
                        JsonData ranklistData = bro.GetReturnValuetoJSON()["rows"][0];

                        mystageRankInfo = JsonUtility.FromJson<BackendRankData>(ranklistData.ToJson().ToString());
                        mystageRankInfo.rankType = NormalRankingType.StageRanking;

                        if (Player.Cloud.field.bestChapterForBackEndRanking * 100 + Player.Cloud.field.bestStageForBackEndRanking <= mystageRankInfo.score.N)
                        {
                            return;
                        }
                    }
                }


                string rowUUID = normalGameData.inDate.S;
                BackEnd.Param param = new BackEnd.Param();
                if(titleName==StaticData.Wrapper.ingameRankingNameData[(int)NormalRankingType.LevelRanking].titleName)
                {
                    int _score = Player.Cloud.userLevelData.currentLevel;
                    param.Add("UserLevel", _score);
                }
                else
                {
                    int _score = Player.Cloud.field.bestChapterForBackEndRanking * 100+ Player.Cloud.field.bestStageForBackEndRanking;
                    param.Add("StageScore", _score);
                }
                
                var rankData = GetRankuuid(titleName);


                BackEnd.Backend.URank.User.UpdateUserScore(rankData.uuid.S, normalRankingTableName, rowUUID, param,(bro)=> {
                    var status = bro.GetStatusCode();

                    if (status.Equals("204"))
                    {
                        LocalSaveLoader.SaveUserCloudData();
                        Player.Cloud.userRaidData.SetDirty(true).UpdateHash();
                    }

                    //Debug.Log(string.Format("랭킹 업뎃 결과: statuscode:{0}", status));
                });
#endif
            }


            /// <summary>
            /// 랭크데이터 불러오는 것이니 가끔만 호출(내가 스코어 업뎃 할때나 1시간에 한번정도)
            /// </summary>
            /// <param name="count"></param>
            public static void SetRaidRankList(int count=30)
            {
                if (Player.Cloud.userRaidData.isRaidUnlocked == false)
                    return;

                raidRankList.Clear();

                var bro = BackEnd.Backend.URank.User.GetRankList(Player.Cloud.userRaidData.currentRegisteredRankUUID, count);
                if(bro.GetStatusCode()!="200")
                {
                    //Debug.Log("rank null");
                    return;
                }
                JsonData ranklistData = bro.GetReturnValuetoJSON()["rows"];

                foreach (LitJson.JsonData data in ranklistData)
                {
                    var rankingTableData = JsonUtility.FromJson<BackendRankData>(data.ToJson().ToString());
                    rankingTableData.rankType = NormalRankingType.RaidRanking;
                    raidRankList.Add(rankingTableData);
                }
            }
            /// <summary>
            /// 이것도 가끔만 호출(스코어 업뎃 할떄 or 1시간에 한번정도)
            /// </summary>
            public static void SetMyRaidRank()
            {
                if (Player.Cloud.userRaidData.isRaidUnlocked == false)
                    return;
                var bro = Backend.URank.User.GetMyRank(Player.Cloud.userRaidData.currentRegisteredRankUUID);
                if (bro.GetStatusCode() != "200")
                {
                    //Debug.Log("rank null");
                    return;
                }
                JsonData ranklistData = bro.GetReturnValuetoJSON()["rows"][0];

                myRaidRankInfo = JsonUtility.FromJson<BackendRankData>(ranklistData.ToJson().ToString());
                if(myRaidRankInfo==null)
                {
                    Player.Cloud.userRaidData.bestDamage = 0;
                }
                else
                {
                    myRaidRankInfo.rankType = NormalRankingType.RaidRanking;
                }
                
            }
            public static bool LevelRanklistLoaded = false;
            /// <summary>
            /// level 랭크 리스트 세팅
            /// </summary>
            /// <param name="count"></param>
            public static void SetLevelRankList(int count = 30)
            {
                if (Player.Cloud.field.bestChapter < Battle.Field.RankingUnlockedChapterIndex)
                    return;
                levelRankList.Clear();

                var rankData = GetRankuuid(StaticData.Wrapper.ingameRankingNameData[(int)NormalRankingType.LevelRanking].titleName);
                var bro=BackEnd.Backend.URank.User.GetRankList(rankData.uuid.S, count);
                if (bro.GetStatusCode() != "200")
                {
                    //Debug.Log("rank null");
                    return;
                }
                JsonData ranklistData = bro.GetReturnValuetoJSON()["rows"];

                foreach (LitJson.JsonData data in ranklistData)
                {
                    var rankingTableData = JsonUtility.FromJson<BackendRankData>(data.ToJson().ToString());
                    rankingTableData.rankType = NormalRankingType.LevelRanking;
                    levelRankList.Add(rankingTableData);
                }
            }

            /// <summary>
            /// 이것도 가끔만 호출(스코어 업뎃 할떄 or 1시간에 한번정도) -내 레벨랭크 데이터세팅
            /// </summary>
            public static void SetMyLevelRank()
            {
                if (Player.Cloud.field.bestChapter < Battle.Field.RankingUnlockedChapterIndex)
                    return;

                mylevelRankInfo = null;

                var rankData = GetRankuuid(StaticData.Wrapper.ingameRankingNameData[(int)NormalRankingType.LevelRanking].titleName);
                var bro = Backend.URank.User.GetMyRank(rankData.uuid.S);
                if (bro.GetStatusCode() != "200")
                {
                    //Debug.Log("rank null");
                    return;
                }
                JsonData ranklistData = bro.GetReturnValuetoJSON()["rows"][0];

                mylevelRankInfo = JsonUtility.FromJson<BackendRankData>(ranklistData.ToJson().ToString());
                mylevelRankInfo.rankType = NormalRankingType.LevelRanking;
            }

            public static bool StageRanklistLoaded = false;
            /// <summary>
            /// stage 랭크 리스트 세팅
            /// </summary>
            /// <param name="count"></param>
            public static void SetStageRankList(int count = 30)
            {
                if (Player.Cloud.field.bestChapter < Battle.Field.RankingUnlockedChapterIndex)
                    return;

                stageRankList.Clear();

                var rankData = GetRankuuid(StaticData.Wrapper.ingameRankingNameData[(int)NormalRankingType.StageRanking].titleName);
                var bro=BackEnd.Backend.URank.User.GetRankList(rankData.uuid.S, count);
                if (bro.GetStatusCode() != "200")
                {
                    //Debug.Log("rank null");
                    return;
                }
                JsonData ranklistData = bro.GetReturnValuetoJSON()["rows"];

                foreach (LitJson.JsonData data in ranklistData)
                {
                    var rankingTableData = JsonUtility.FromJson<BackendRankData>(data.ToJson().ToString());
                    rankingTableData.rankType = NormalRankingType.StageRanking;
                    stageRankList.Add(rankingTableData);
                }
            }
            /// <summary>
            /// 이것도 가끔만 호출(스코어 업뎃 할떄 or 1시간에 한번정도)- 내 스테이지 랭크 세팅
            /// </summary>
            public static void SetMyStageRank()
            {
                if (Player.Cloud.field.bestChapter < Battle.Field.RankingUnlockedChapterIndex)
                    return;

                mystageRankInfo = null;
                var rankData = GetRankuuid(StaticData.Wrapper.ingameRankingNameData[(int)NormalRankingType.StageRanking].titleName);
                var bro = Backend.URank.User.GetMyRank(rankData.uuid.S);
                if (bro.GetStatusCode() != "200")
                {
                    //Debug.Log("rank null");
                    return;
                }
                JsonData ranklistData = bro.GetReturnValuetoJSON()["rows"][0];

                mystageRankInfo = JsonUtility.FromJson<BackendRankData>(ranklistData.ToJson().ToString());
                mystageRankInfo.rankType = NormalRankingType.StageRanking;
            }

       
            public static void LogEvent(string key,Param param)
            {
                Backend.GameLog.InsertLog(key, param, (callback) =>
                {
                    // 이후 처리
                    string statuscode = callback.GetStatusCode();
                });
            }
        }
    }
}
