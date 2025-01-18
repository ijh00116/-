using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using BlackTree.Definition;
using Cysharp.Threading.Tasks;

namespace BlackTree.Core
{
    public class ControllerNormalRanking
    {
        ViewCanvasNormalRanking _viewRanking;
        CancellationTokenSource _cts;
        public bool isLevelRanking;

        List<RankingRewardInfoSlot> rankingRewardSlotList = new List<RankingRewardInfoSlot>();

        List<ViewNormalRankingSlot> levelRankList = new List<ViewNormalRankingSlot>();
        List<ViewNormalRankingSlot> stageRankList = new List<ViewNormalRankingSlot>();
        const int rankCount = 50;
        public ControllerNormalRanking(Transform parent, CancellationTokenSource cts)
        {
            _cts = cts;
            _viewRanking = ViewCanvas.Create<ViewCanvasNormalRanking>(parent);

            _viewRanking.levelRankingBtn.onClick.AddListener(OpenLevelRankingWindow);
            _viewRanking.StageRankingBtn.onClick.AddListener(OpenStageRankingWindow);

            
            for (int i = 0; i < _viewRanking.closeBtn.Length; i++)
            {
                int index = i;
                _viewRanking.closeBtn[index].onClick.AddListener(WindowOff);
            }

            Player.BackendData.SetLevelRankList();
            Player.BackendData.SetMyLevelRank();
            Player.BackendData.SetStageRankList();
            Player.BackendData.SetMyStageRank();

            _viewRanking.BindOnChangeVisible((o) => { 
                if(o)
                {
                    OpenLevelRankingWindow();
                }
            });
        
            _viewRanking.openRewardTableBtn.onClick.AddListener(OpenRankingRewardTable);
            for(int i=0; i< _viewRanking.closeRewardTableBtns.Length; i++)
            {
                _viewRanking.closeRewardTableBtns[i].onClick.AddListener(()=> {
                    _viewRanking.rankingRewardWindow.SetActive(false);
                });
            }

            if(Player.Cloud.field.bestChapter< Battle.Field.RankingUnlockedChapterIndex)
            {
                _viewRanking.lockedObject.SetActive(true);
                _viewRanking.stageRankingWindow.SetActive(false);
                _viewRanking.levelRankingWindow.SetActive(false);
            }
            else
            {
                _viewRanking.lockedObject.SetActive(false);
            }

            string chapterNeedDesc = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_chapterNotenoughForRank].StringToLocal;
            
            _viewRanking.locked_text.text = string.Format(chapterNeedDesc, Battle.Field.RankingUnlockedChapterIndex+1);

            for(int i=0; i< rankCount; i++)
            {
                var obj = UnityEngine.Object.Instantiate(_viewRanking.rankslotPrefab);
                obj.transform.SetParent(_viewRanking.levelRankScrollview.content, false);
                obj.gameObject.SetActive(false);
                levelRankList.Add(obj);
            }

            for (int i = 0; i < rankCount; i++)
            {
                var obj = UnityEngine.Object.Instantiate(_viewRanking.rankslotPrefab);
                obj.transform.SetParent(_viewRanking.stageRankScrollview.content, false);
                obj.gameObject.SetActive(false);
                stageRankList.Add(obj);
            }

            _viewRanking.rewardTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_RewardTable].StringToLocal;
            _viewRanking.titleTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Ranking].StringToLocal;
            _viewRanking.descTxt.text = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_Rankdesc].StringToLocal;
            _viewRanking.levelTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Level].StringToLocal;
            _viewRanking.levelTxt_on.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Level].StringToLocal;
            _viewRanking.chapterTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Chapter].StringToLocal;
            _viewRanking.chapterTxt_on.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Chapter].StringToLocal;
              _viewRanking.rewardTitleTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Chapter].StringToLocal; 
            _viewRanking.rewardDescTxt.text = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_RankRewardDesc].StringToLocal;

            Main().Forget();
        }

        int min = 0;
        int sec = 0;
        async UniTaskVoid Main()
        {
            while (true)
            {
                Player.BackendData.currentNormalRankInitTime+= Time.deltaTime;
               
                min = (int)((Player.BackendData.InitNormalRankTime - Player.BackendData.currentNormalRankInitTime) / 60);
                sec = (int)((Player.BackendData.InitNormalRankTime - Player.BackendData.currentNormalRankInitTime) % 60);
                _viewRanking.timertxt.text = string.Format("{0:D2}:{1:D2}", min, sec);
                UpdateRank();

                await UniTask.Yield(_cts.Token);
            }
        }

        void UpdateRank()
        {
            if (Player.Cloud.field.bestChapter >= Battle.Field.RankingUnlockedChapterIndex)
            {
                if (Player.BackendData.currentNormalRankInitTime >= Player.BackendData.InitNormalRankTime)
                {
                    Player.BackendData.NormalRankingUpdate(StaticData.Wrapper.ingameRankingNameData[(int)Player.BackendData.NormalRankingType.StageRanking].titleName);
                    Player.BackendData.NormalRankingUpdate(StaticData.Wrapper.ingameRankingNameData[(int)Player.BackendData.NormalRankingType.LevelRanking].titleName);
                    Player.BackendData.SetLevelRankList();
                    Player.BackendData.SetMyLevelRank();
                    Player.BackendData.SetStageRankList();
                    Player.BackendData.SetMyStageRank();
                    Player.BackendData.currentNormalRankInitTime = 0;
                }
            }
        }

        void WindowOff()
        {
            _viewRanking.blackBG.PopupCloseColorFade();
            _viewRanking.Wrapped.CommonPopupCloseAnimationUp(() => {
                _viewRanking.SetVisible(false);
            });
        }
        void OpenLevelRankingWindow()
        {
            if (Player.BackendData.mylevelRankInfo == null)
            {
                if (Player.Cloud.field.bestChapter >= Battle.Field.RankingUnlockedChapterIndex)
                {
                    Player.BackendData.NormalRankingUpdate(StaticData.Wrapper.ingameRankingNameData[(int)Player.BackendData.NormalRankingType.LevelRanking].titleName);
                    Player.BackendData.SetLevelRankList();
                    Player.BackendData.SetMyLevelRank();
                }
                else
                {
                    _viewRanking.stageRankingWindow.SetActive(false);
                    _viewRanking.levelRankingWindow.SetActive(false);
                    _viewRanking.lockedObject.SetActive(true);

                    return;
                }
            }
          
            isLevelRanking = true;
            LevelRankingWindowOn();

            _viewRanking.topSelectors.Show(0);
            _viewRanking.stageRankingWindow.SetActive(false);
            _viewRanking.levelRankingWindow.SetActive(true);
            if (Player.Cloud.field.bestChapter < Battle.Field.RankingUnlockedChapterIndex)
            {
                _viewRanking.lockedObject.SetActive(true);
                _viewRanking.stageRankingWindow.SetActive(false);
                _viewRanking.levelRankingWindow.SetActive(false);
            }
            else
            {
                _viewRanking.lockedObject.SetActive(false);
            }

        }
        void OpenStageRankingWindow()
        {
            if (Player.BackendData.mystageRankInfo == null)
            {
                if (Player.Cloud.field.bestChapter >= Battle.Field.RankingUnlockedChapterIndex)
                {
                    Player.BackendData.NormalRankingUpdate(StaticData.Wrapper.ingameRankingNameData[(int)Player.BackendData.NormalRankingType.StageRanking].titleName);
                    Player.BackendData.SetStageRankList();
                    Player.BackendData.SetMyStageRank();
                }
                else
                {
                    return;
                }
            }
            
            isLevelRanking = false;
            StageRankingWindowOn();
            _viewRanking.topSelectors.Show(1);
            _viewRanking.stageRankingWindow.SetActive(true);
            _viewRanking.levelRankingWindow.SetActive(false);
            if (Player.Cloud.field.bestChapter < Battle.Field.RankingUnlockedChapterIndex)
            {
                _viewRanking.lockedObject.SetActive(true);
                _viewRanking.stageRankingWindow.SetActive(false);
                _viewRanking.levelRankingWindow.SetActive(false);
            }
            else
            {
                _viewRanking.lockedObject.SetActive(false);
            }

        }

        void LevelRankingWindowOn()
        {
            for (int i=0; i< levelRankList.Count; i++)
            {
                levelRankList[i].gameObject.SetActive(false);
            }
            for(int i=0; i<Player.BackendData.levelRankList.Count; i++)
            {
                levelRankList[i].gameObject.SetActive(true);
                levelRankList[i].UpdateContent(Player.BackendData.levelRankList[i]);
            }
#if UNITY_EDITOR
#else
     _viewRanking.myLevelRank.UpdateContent(Player.BackendData.mylevelRankInfo);
#endif
        }

        void StageRankingWindowOn()
        {
            for (int i = 0; i < stageRankList.Count; i++)
            {
                stageRankList[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < Player.BackendData.stageRankList.Count; i++)
            {
                stageRankList[i].gameObject.SetActive(true);
                stageRankList[i].UpdateContent(Player.BackendData.stageRankList[i]);
            }
#if UNITY_EDITOR
#else
      _viewRanking.myStageRank.UpdateContent(Player.BackendData.mystageRankInfo);
#endif
        }

        void OpenRankingRewardTable()
        {
            if(isLevelRanking)
            {
                int levelRewardMinIndex = (int)MailRewardType.LevelReward_1;
                for(int i=0; i<StaticData.Wrapper.rankRewardRange.Length; i++)
                {
                    int index = i;
                    RankingRewardInfoSlot obj = null;
                    if (index >= rankingRewardSlotList.Count)
                    {
                        obj = UnityEngine.Object.Instantiate(_viewRanking.rankingRewardslotPrefab);
                        obj.transform.SetParent(_viewRanking.rewardParentScrollRect.content, false);
                        rankingRewardSlotList.Add(obj);
                    }
                    else
                    {
                        obj = rankingRewardSlotList[index];
                    }
                    obj.Init(index, (MailRewardType)(index + levelRewardMinIndex));
                }
            }
            else
            {
                int stageRewardMinIndex = (int)MailRewardType.StageReward_1;
                for (int i = 0; i < StaticData.Wrapper.rankRewardRange.Length; i++)
                {
                    int index = i;
                    RankingRewardInfoSlot obj = null;
                    if (index >= rankingRewardSlotList.Count)
                    {
                        obj = UnityEngine.Object.Instantiate(_viewRanking.rankingRewardslotPrefab);
                        obj.transform.SetParent(_viewRanking.rewardParentScrollRect.content, false);
                        rankingRewardSlotList.Add(obj);
                    }
                    else
                    {
                        obj = rankingRewardSlotList[index];
                    }
                    obj.Init(index, (MailRewardType)(index + stageRewardMinIndex));
                }
            }
            _viewRanking.rankingRewardWindow.SetActive(true);
        }
    }
}
