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
    public class ControllerEnterRaidDungeon
    {
        ViewCanvasEnterRaidDungeon _viewCanvasEnterRaid;
        CancellationTokenSource _cts;

        bool isBackendFixServerNow = false;

        List<RankingRewardInfoSlot> rankingRewardSlotList = new List<RankingRewardInfoSlot>();
        List<ViewRankingSlot> rankslotList = new List<ViewRankingSlot>();

        int currentSelectedRewardIndex = 0;
        public ControllerEnterRaidDungeon(Transform parent,CancellationTokenSource cts)
        {
            _cts = cts;
            _viewCanvasEnterRaid = ViewCanvas.Create<ViewCanvasEnterRaidDungeon>(parent);
            _viewCanvasEnterRaid.SetVisible(false);

            _viewCanvasEnterRaid.BindOnChangeVisible(OnClickDungeonUIVisible);

            _viewCanvasEnterRaid.enterBtn.onClick.AddListener(EnterClickerDungeon);

            for (int i = 0; i < _viewCanvasEnterRaid.closeBtn.Length; i++)
            {
                int index = i;
                _viewCanvasEnterRaid.closeBtn[index].onClick.AddListener(UIOff);
            }

            //ranking
            _viewCanvasEnterRaid.rankingBtn.onClick.AddListener(RankingWindowOn);
            for (int i = 0; i < _viewCanvasEnterRaid.rankingWindowOffBtn.Length; i++)
            {
                int index = i;
                _viewCanvasEnterRaid.rankingWindowOffBtn[index].onClick.AddListener(RankingWindowOff);
            }
            isBackendFixServerNow = false;

            Player.BackendData.SetRaidRankList();
            Player.BackendData.SetMyRaidRank();

            for(int i=0; i<Player.BackendData.raidRankList.Count; i++)
            {
                int index = i;
                ViewRankingSlot slotObj = null;
                if (index>= rankslotList.Count)
                {
                    slotObj = UnityEngine.Object.Instantiate(_viewCanvasEnterRaid.rankSlotPrefab);
                    slotObj.transform.SetParent(_viewCanvasEnterRaid.rankScrollview.content,false);
                    rankslotList.Add(slotObj);
                }
                else
                {
                    slotObj = rankslotList[index];
                }
                slotObj.gameObject.SetActive(true);

                slotObj.UpdateContent(Player.BackendData.raidRankList[index]);
            }
            _viewCanvasEnterRaid.myRankSlot.UpdateContent(Player.BackendData.myRaidRankInfo);

            if(Player.BackendData.myRaidRankInfo==null)
            {
                Player.Cloud.userRaidData.bestDamage = 0;
            }

            _viewCanvasEnterRaid.openRewardTableBtn.onClick.AddListener(()=> { 
                OpenRankingRewardTable(currentSelectedRewardIndex);
            });

            currentSelectedRewardIndex = (int)(Player.Cloud.field.bestChapter/Player.BackendData.indexConstForraidRanking);

            for (int i = 0; i < _viewCanvasEnterRaid.closeRewardTableBtns.Length; i++)
            {
                _viewCanvasEnterRaid.closeRewardTableBtns[i].onClick.AddListener(() => {
                    _viewCanvasEnterRaid.rankingRewardWindow.SetActive(false);
                });
            }

            _viewCanvasEnterRaid.tablePrevBtn.onClick.AddListener(()=> {
                currentSelectedRewardIndex--;
                if (currentSelectedRewardIndex <= 0)
                    currentSelectedRewardIndex = 0;
                OpenRankingRewardTable(currentSelectedRewardIndex);
            });
            _viewCanvasEnterRaid.tableNextBtn.onClick.AddListener(() => {
                currentSelectedRewardIndex++;
                if (currentSelectedRewardIndex >= StaticData.Wrapper.backEndRaidRankingNameDatas.Length)
                    currentSelectedRewardIndex = StaticData.Wrapper.backEndRaidRankingNameDatas.Length-1;
                OpenRankingRewardTable(currentSelectedRewardIndex);
            });

            int day = System.DateTime.Now.Day;
            int hour = System.DateTime.Now.Hour;
            int dayIndex = day;
            if (hour >= 0 && hour <= 4)
            {
                isBackendFixServerNow = true;
                dayIndex = System.DateTime.Now.AddDays(-1).Day;
            }
            else
            {
                dayIndex = System.DateTime.Now.Day;
            }


            int raidIndex = dayIndex % Battle.Raid.stageChangeIndex;

            Battle.Raid.raidBossIndex = raidIndex;
            raidBossDescSet();

            _viewCanvasEnterRaid.raidDescOpenBtn.onClick.AddListener(()=> {
                _viewCanvasEnterRaid.raidDescWindow.SetActive(true);
            });
            for (int i=0; i< _viewCanvasEnterRaid.raidDescWindowCloseBtn.Length; i++)
            {
                _viewCanvasEnterRaid.raidDescWindowCloseBtn[i].onClick.AddListener(() => {
                    _viewCanvasEnterRaid.raidDescWindow.SetActive(false);
                });
            }
            raidDescSet();

            if (Player.Option.isAnotherDay())
            {
                Player.Cloud.userRaidData.todayParticipateCount = 0;
            }

            Player.Option.AnotherDaySetting += AnotherDaySetting;

            Main().Forget();

            _viewCanvasEnterRaid.titleTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Raid].StringToLocal;
            _viewCanvasEnterRaid.descTxt.text = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_RaidDesc].StringToLocal;
            _viewCanvasEnterRaid.recommendTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_RecommendSkill].StringToLocal;
            _viewCanvasEnterRaid.enterTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Enter].StringToLocal;
            _viewCanvasEnterRaid.rankTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Ranking].StringToLocal;

             _viewCanvasEnterRaid.ranktitleTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Raid].StringToLocal;
            _viewCanvasEnterRaid.rankdescTxt.text = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_raidRankdesc].StringToLocal;
            _viewCanvasEnterRaid.rankRewardTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_RewardTable].StringToLocal;
            _viewCanvasEnterRaid.rewardTitleTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_RewardTable].StringToLocal;
        }

        void AnotherDaySetting()
        {
            if (Player.Option.isAnotherDay())
            {
                Player.Cloud.userRaidData.todayParticipateCount = 0;
            }
        }
        void raidBossDescSet()
        {
            if(Battle.Raid.raidBossIndex==0)
            {
                _viewCanvasEnterRaid.raidBossDesc.text = string.Format(StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.raidBoss_0_Desc].StringToLocal);
            }
            if (Battle.Raid.raidBossIndex == 1)
            {
                _viewCanvasEnterRaid.raidBossDesc.text = string.Format(StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.raidBoss_1_Desc].StringToLocal);
            }
        }

        void raidDescSet()
        {
            string desc = "";
            for(int i= (int)LocalizeDescKeys.raidDesc_0; i<= (int)LocalizeDescKeys.raidDesc_5; i++)
            {
                desc += StaticData.Wrapper.localizeddesclist[i].StringToLocal + "\n";
            }
            _viewCanvasEnterRaid.raidDesc.text = desc;
        }

        async UniTaskVoid Main()
        {
            while (true)
            {
                Battle.Raid.currentRankInitTime += Time.deltaTime;
                
                await UniTask.Yield(_cts.Token);
            }
        }

        void OnClickDungeonUIVisible(bool active)
        {
            //dayindex for raid dungeon
            int day = System.DateTime.Now.Day;
            int hour = System.DateTime.Now.Hour;
            int dayIndex = day;
            if (hour >= 0 && hour <= 4)
            {
                isBackendFixServerNow = true;
                dayIndex = System.DateTime.Now.AddDays(-1).Day;
            }
            else
            {
                dayIndex = System.DateTime.Now.Day;
            }
         

            int raidIndex = dayIndex % Battle.Raid.stageChangeIndex;

            Battle.Raid.raidBossIndex = raidIndex;

            raidBossDescSet();
            for (int i=0; i< _viewCanvasEnterRaid.suggestedSkillObj.Length; i++)
            {
                _viewCanvasEnterRaid.suggestedSkillObj[i].SetActive(false);
            }

            for(int i=0; i<StaticData.Wrapper.raidTableData[raidIndex].recommandSkill.Length; i++)
            {
                SkillKey skill = StaticData.Wrapper.raidTableData[raidIndex].recommandSkill[i];
                _viewCanvasEnterRaid.suggestedSkillObj[i].SetActive(true);
                _viewCanvasEnterRaid.suggestedSkillImage[i].sprite = InGameResourcesBundle.Loaded.skillIcon[(int)skill];
            }
            _viewCanvasEnterRaid.bossImage.sprite = StageResourcesBundle.Loaded.raidDungeonBossImage[raidIndex];

            _viewCanvasEnterRaid.RankingTotalWindow.SetActive(false);
            _viewCanvasEnterRaid.currentEnterCount.text = string.Format("{0}/{1}", Battle.Raid.maxRaidParticipateMaxCount-Player.Cloud.userRaidData.todayParticipateCount, Battle.Raid.maxRaidParticipateMaxCount);
        }

        void EnterClickerDungeon()
        {
            if (isBackendFixServerNow)
            {
           
                string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_ServerFix].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                Debug.Log(localizedvalue);
                return;
            }
            if (Battle.Field.IsCantChangeScene)
            {
                string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_CanPlayForSomeReason].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                return;
            }
            if (Player.Cloud.userRaidData.todayParticipateCount>= Battle.Raid.maxRaidParticipateMaxCount)
            {
               
                string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_NomoreUseRaid].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                Debug.Log(localizedvalue);
                return;
            }

            LocalizeDescKeys localizedKey = LocalizeDescKeys.Etc_UnlockAfterChapter;
            int chapterValue = Model.Player.Option.ContentUIUnlockLevel(LockedUIType.RaidUnlock);


            if(Player.Cloud.field.bestChapter< chapterValue-1)
            {
                string localizedvalue = string.Format(StaticData.Wrapper.localizeddesclist[(int)localizedKey].StringToLocal, chapterValue);
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                return;
            }

            Battle.Field.ChangeSceneState(eSceneState.WaitForRaidDungeon);
            MainNav.CloseMainUIWindow();
            Player.Skill.SkillCoolTimeInit();
            for (int i = 0; i < (int)SkillKey.End; i++)
            {
                Player.Skill.SkillActivate?.Invoke((SkillKey)i, false);
            }
            for (int i = 0; i < (int)PetSkillKey.End; i++)
            {
                Player.Pet.SkillActivate?.Invoke((PetSkillKey)i, false);
            }

            Player.Unit.ResetUnitWhenGoDungeon();

            Player.Quest.TryCountUp(QuestType.RaidContent, 1);
        }

        void UIOff()
        {
            _viewCanvasEnterRaid.blackBG.PopupCloseColorFade();
            _viewCanvasEnterRaid.Wrapped.CommonPopupCloseAnimationUp(() => {
                _viewCanvasEnterRaid.SetVisible(false);
            });
        }

        void RankindslotSetting()
        {
            for(int i=0; i< rankslotList.Count; i++)
            {
                rankslotList[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < Player.BackendData.raidRankList.Count; i++)
            {
                int index = i;
                ViewRankingSlot slotObj = null;
                if (index >= rankslotList.Count)
                {
                    slotObj = UnityEngine.Object.Instantiate(_viewCanvasEnterRaid.rankSlotPrefab);
                    slotObj.transform.SetParent(_viewCanvasEnterRaid.rankScrollview.content,false);
                    rankslotList.Add(slotObj);
                }
                else
                {
                    slotObj = rankslotList[index];
                }
                slotObj.gameObject.SetActive(true);
                slotObj.UpdateContent(Player.BackendData.raidRankList[index]);
            }
            _viewCanvasEnterRaid.myRankSlot.UpdateContent(Player.BackendData.myRaidRankInfo);
        }

        void RankingWindowOn()
        {
            if (Battle.Raid.currentRankInitTime>=Battle.Raid.raidInitRankTime)
            {
                Battle.Raid.currentRankInitTime = 0;
                Player.BackendData.SetRaidRankList();
                Player.BackendData.SetMyRaidRank();
            }

            if (Player.BackendData.myRaidRankInfo == null)
            {
                string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_RaidDescUnComplete].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                return;
            }
            _viewCanvasEnterRaid.RankingTotalWindow.SetActive(true);
            _viewCanvasEnterRaid.rankingWindowBG.PopupOpenColorFade();

            string _rank = "D";

            string ranktitlename = Player.Cloud.userRaidData.currentRegisteredRankTitle;

            switch (ranktitlename)
            {
                case "RaidRanking_0":
                    _rank = "D";
                    break;
                case "RaidRanking_1":
                    _rank = "C";
                    break;
                case "RaidRanking_2":
                    _rank = "B";
                    break;
                case "RaidRanking_3":
                    _rank = "A";
                    break;
                case "RaidRanking_4":
                    _rank = "S";
                    break;
                case "tempraid_0":
                    _rank = "D";
                    break;
                case "tempraid_1":
                    _rank = "C";
                    break;
                default:
                    break;
            }

            string rankvalue = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_rankForRaid].StringToLocal;
            _viewCanvasEnterRaid.myRankText.text = string.Format(rankvalue, _rank);
            _viewCanvasEnterRaid.rankingPopupwindow.CommonPopupOpenAnimationDown(() => {
                RankindslotSetting();
            });
        }

        void RankingWindowOff()
        {
            _viewCanvasEnterRaid.rankingWindowBG.PopupCloseColorFade();
            _viewCanvasEnterRaid.rankingPopupwindow.CommonPopupCloseAnimationUp(() => {
                _viewCanvasEnterRaid.RankingTotalWindow.SetActive(false);
            });
        }

        void OpenRankingRewardTable(int currentChapterIndex)
        {
            if(Player.BackendData.myRaidRankInfo==null)
            {
                
                string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_RaidDescUnComplete].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                return;
            }
            int rewardMinIndex = 0;
            switch (currentChapterIndex)
            {
                case 0:
                    rewardMinIndex = (int)Definition.MailRewardType.RankReward_0_1;
                    break;
                case 1:
                    rewardMinIndex = (int)Definition.MailRewardType.RankReward_1_1;
                    break;
                case 2:
                    rewardMinIndex = (int)Definition.MailRewardType.RankReward_2_1;
                    break;
                case 3:
                    rewardMinIndex = (int)Definition.MailRewardType.RankReward_3_1;
                    break;
                case 4:
                    rewardMinIndex = (int)Definition.MailRewardType.RankReward_4_1;
                    break;
                default:
                    break;
            }

            for (int i = 0; i < StaticData.Wrapper.rankRewardRange.Length; i++)
            {
                int index = i;
                RankingRewardInfoSlot obj = null;
                if (index >= rankingRewardSlotList.Count)
                {
                    obj = UnityEngine.Object.Instantiate(_viewCanvasEnterRaid.rankingRewardslotPrefab);
                    obj.transform.SetParent(_viewCanvasEnterRaid.rewardParentScrollRect.content, false);
                    rankingRewardSlotList.Add(obj);
                }
                else
                {
                    obj = rankingRewardSlotList[index];
                }
                obj.Init(index, (MailRewardType)(index + rewardMinIndex));
            }
            _viewCanvasEnterRaid.rankingRewardWindow.SetActive(true);

            string _rank="D";
            switch (currentSelectedRewardIndex)
            {
                case 0:
                    _rank = "D";
                    break;
                case 1:
                    _rank = "C";
                    break;
                case 2:
                    _rank = "B";
                    break;
                case 3:
                    _rank = "A";
                    break;
                case 4:
                    _rank = "S";
                    break;
                default:
                    break;
            }

            string rankvalue = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_rankForRaid].StringToLocal;
            _viewCanvasEnterRaid.currentRewardTable.text = string.Format(rankvalue, _rank);
        }

    }
}

