using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using Cysharp.Threading.Tasks;
using BlackTree.Definition;

namespace BlackTree.Core
{
    public class ControllerQuest
    {
        private ViewCanvasQuest _view;
        private CancellationTokenSource _cts;

        List<ViewQuestSlot> dailyslotList = new List<ViewQuestSlot>();
        List<ViewQuestSlot> subQuestslotList = new List<ViewQuestSlot>();
        public ControllerQuest(Transform parent, CancellationTokenSource cts)
        {
            _cts = cts;
            _view = ViewCanvas.Create<ViewCanvasQuest>(parent);

            _view.dailyQuestBtn.onClick.AddListener(()=> { ShowScrollview(0); });
            _view.repeatQuestBtn.onClick.AddListener(() => { ShowScrollview(1); });

            for(int i=0; i< _view.closeBtn.Length; i++)
            {
                int index = i;
                _view.closeBtn[index].onClick.AddListener(CloseQuestWindow);
            }


            int today =  Extension.GetServerTime().Day;
            if (today != Player.Cloud.playingRecord.savedDay)
            {
                Player.Cloud.playingRecord.InitDay();
                Player.Cloud.playingRecord.savedDay = today;

                for (int i = 0; i < StaticData.Wrapper.dailyQuest.Length; i++)
                {
                    Player.Quest.onChangeDailyRepeatQuest?.Invoke(StaticData.Wrapper.dailyQuest[i].questType);
                }
                ViewUpdateAfterRewarded(Definition.QuestType.ActivateAdBuff);

                Player.Cloud.playingRecord.UpdateHash().SetDirty(true);
                LocalSaveLoader.SaveUserCloudData();
            }

            for (int i=0; i<StaticData.Wrapper.dailyQuest.Length;i++)
            {
                var slot = UnityEngine.Object.Instantiate(_view.questSlotPrefab);
                slot.transform.SetParent(_view.dailyScrollRect.content, false);
                slot.Init(StaticData.Wrapper.dailyQuest[i], PlayingRecordType.Daily);
                dailyslotList.Add(slot);
            }

            for (int i = 0; i < StaticData.Wrapper.subRepeatQuest.Length; i++)
            {
                var slot = UnityEngine.Object.Instantiate(_view.questSlotPrefab);
                slot.transform.SetParent(_view.repeatScrollRect.content, false);
                slot.Init(StaticData.Wrapper.subRepeatQuest[i], PlayingRecordType.SubRepeat);
                subQuestslotList.Add(slot);
                if(StaticData.Wrapper.subRepeatQuest[i].questType==QuestType.KillEnemy||
                    StaticData.Wrapper.subRepeatQuest[i].questType == QuestType.AwakeTime||
                    StaticData.Wrapper.subRepeatQuest[i].questType == QuestType.PlayingTime_sec)
                {
                    slot.transform.SetAsFirstSibling();
                }
            }

            Main().Forget();
            QuestSyncUpdate().Forget();

            ShowScrollview(0);

            Player.Quest.onChangeAfterRecieveDailyRepeatQuest += ViewUpdateAfterRewarded;

            _view.dailyQuestCompleteQuestRewardBtn.onClick.AddListener(SendRewardDia);
            ViewUpdateAfterRewarded(Definition.QuestType.ActivateAdBuff);

            _view.TotalRecieveBtn.onClick.AddListener(SendTotalRecieve);
            _view.TotalRecieveBtn_repeat.onClick.AddListener(SendTotalRecieve_repeat);
            Player.Quest.onCompleteQuest += CountSetTotalRecieve;
            CountSetTotalRecieve();

            _view.BindOnChangeVisible(active => {
                if (active)
                {
                    if (Player.Guide.currentGuideQuest == QuestGuideType.QuestComplete)
                    {
                        Player.Guide.QuestGuideProgress(Player.Guide.currentGuideQuest);
                    }
                }
            });

            _view.titleTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Quest].StringToLocal;
            _view.dailyquest.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Daily].StringToLocal;
            _view.dailyquest_on.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Daily].StringToLocal;
            _view.repeatquest.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Repeat].StringToLocal;
            _view.repeatquest_on.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Repeat].StringToLocal;
            _view.dailyComplete.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_DailyMissionComplete].StringToLocal;
            _view.allReward.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_AllReward].StringToLocal;
        }

        void CloseQuestWindow()
        {
            _view.blackBG.PopupCloseColorFade();
            _view.Wrapped.CommonPopupCloseAnimationUp(() => {
                _view.SetVisible(false);
            });
        }

        async UniTaskVoid Main()
        {
            while (true)
            {
                await UniTask.Delay(60000);

                int today =  Extension.GetServerTime().Day;
                if (today != Player.Cloud.playingRecord.savedDay)
                {
                    Player.Cloud.playingRecord.InitDay();
                    Player.Cloud.playingRecord.savedDay = today;

                    for (int i = 0; i < StaticData.Wrapper.dailyQuest.Length; i++)
                    {
                        Player.Quest.onChangeDailyRepeatQuest?.Invoke(StaticData.Wrapper.dailyQuest[i].questType);
                    }
                    ViewUpdateAfterRewarded(Definition.QuestType.ActivateAdBuff);

                    Player.Cloud.playingRecord.UpdateHash().SetDirty(true);
                    LocalSaveLoader.SaveUserCloudData();
                }
            }
        }

        async UniTaskVoid QuestSyncUpdate()
        {
            while (true)
            {
                bool dailycomplete = CanRecieveCompleteQuest();
                bool subRepeatcomplete = CanRecieveCompleteQuest_repeat();
                _view.dailyRedDot.SetActive(dailycomplete);
                _view.repeatRedDot.SetActive(subRepeatcomplete);
                if (dailycomplete || subRepeatcomplete)
                {
                    Player.Option.questRedDotCallback?.Invoke(true);
                }
                else
                {
                    Player.Option.questRedDotCallback?.Invoke(false);
                }
                await UniTask.DelayFrame(30);
            }
        }
        void ShowScrollview(int index)
        {
            _view.buttonSelector.Show(index); 
            _view.panelSelector.Show(index);
        }

        void SendRewardDia()
        {
            if(Player.Cloud.playingRecord.dailyCompleteQuestRewardRecieve==false)
            {
                Player.ControllerGood.Earn(Definition.GoodsKey.Dia, 5000);
                Player.Cloud.playingRecord.dailyCompleteQuestRewardRecieve = true;
                _view.alReadyrewardCheck.SetActive(true);
                _view.cantdailyQuestCompleteQuestRewardObj.SetActive(true);
                _view.dailyQuestCompleteQuestRewardBtn.enabled = false;

                _view.dailyQuestCompleteRedDot.SetActive(false);

                Player.Cloud.playingRecord.UpdateHash().SetDirty(true);
                LocalSaveLoader.SaveUserCloudData();
            }
        }

        void ViewUpdateAfterRewarded(Definition.QuestType _type)
        {
            var sliderValue=(float)Player.Cloud.playingRecord.dailyQuestCompleteCount / (float)Player.Quest.dailyQuestMaxCount;
            _view.dailyQuestCompleteCountSlider.value = sliderValue;
            _view.dailyQuestCompleteCountText.text = string.Format("{0}/{1}", Player.Cloud.playingRecord.dailyQuestCompleteCount, Player.Quest.dailyQuestMaxCount);

            bool isRedDot = Player.Cloud.playingRecord.dailyQuestCompleteCount >= Player.Quest.dailyQuestMaxCount;

            if(Player.Cloud.playingRecord.dailyCompleteQuestRewardRecieve==false)
            {
                _view.dailyQuestCompleteRedDot.SetActive(isRedDot);
            }
                

            SetDailyCompleteQuest();
            CountSetTotalRecieve();

        }
        
        void SetDailyCompleteQuest()
        {
            if (Player.Cloud.playingRecord.dailyCompleteQuestRewardRecieve == false)
            {
                _view.alReadyrewardCheck.SetActive(false);
                if (isCompleteAllQuest())
                {
                    _view.cantdailyQuestCompleteQuestRewardObj.SetActive(false);
                    _view.dailyQuestCompleteQuestRewardBtn.enabled = true;
                }
                else
                {
                    _view.cantdailyQuestCompleteQuestRewardObj.SetActive(true);
                    _view.dailyQuestCompleteQuestRewardBtn.enabled = false;
                }
            }
            else
            {
                _view.alReadyrewardCheck.SetActive(true);
                _view.cantdailyQuestCompleteQuestRewardObj.SetActive(true);
                _view.dailyQuestCompleteQuestRewardBtn.enabled = false;
            }
        }
        bool isCompleteAllQuest()
        {
            return Player.Cloud.playingRecord.dailyQuestCompleteCount >= (float)Player.Quest.dailyQuestMaxCount;
        }

        void CountSetTotalRecieve()
        {
            int rewardValue = 0;
            for (int i = 0; i < dailyslotList.Count; i++)
            {
                if(dailyslotList[i].CanRecieve())
                {
                    rewardValue += 300;
                }
            }
            _view.TotalRecieveBtnText.text = $"{rewardValue}";
        }

        void SendTotalRecieve()
        {
            bool canRecieve = false;
            for (int i = 0; i < dailyslotList.Count; i++)
            {
                if(dailyslotList[i].CanRecieve(false))
                {
                    canRecieve = true;
                }
            }

            for (int i=0; i< dailyslotList.Count; i++)
            {
                dailyslotList[i].GiveReward(false);
            }
            _view.TotalRecieveBtnText.text = $"0";

            if(canRecieve)
            {
                string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.UI_GoodsRewarded].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
            }
            
        }

        void SendTotalRecieve_repeat()
        {
            bool canRecieve = false;
            for (int i = 0; i < subQuestslotList.Count; i++)
            {
                if (subQuestslotList[i].CanRecieve(false))
                {
                    canRecieve = true;
                }
            }

            for (int i = 0; i < subQuestslotList.Count; i++)
            {
                subQuestslotList[i].GiveRewardInSubrepeatAll();
            }

            if (canRecieve)
            {
                string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.UI_GoodsRewarded].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
            }
        }

        bool CanRecieveCompleteQuest()
        {
            bool isRedDot=false;
            for (int i = 0; i < dailyslotList.Count; i++)
            {
                if(dailyslotList[i].CanRecieve())
                {
                    isRedDot = true;
                    break;
                }
            }
            return isRedDot;
        }

        bool CanRecieveCompleteQuest_repeat()
        {
            bool isRedDot = false;
            for (int i = 0; i < subQuestslotList.Count; i++)
            {
                if (subQuestslotList[i].CanRecieve())
                {
                    isRedDot = true;
                    break;
                }
            }
            return isRedDot;
        }
    }
}
