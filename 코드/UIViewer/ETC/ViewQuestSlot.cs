using BlackTree.Core;
using BlackTree.Definition;
using BlackTree.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTree.Bundles
{
    public class ViewQuestSlot : MonoBehaviour
    {
        public Slider playingCount;
        public TMP_Text playingCounttext;
        public TMP_Text desc;
        public TMP_Text rewardText;
        public TMP_Text rewardText_ad;
        public TMP_Text possibleRewardCount;

        public BTButton   rewardBtn;
        public GameObject cantrewardBtn;
        public GameObject rewardCheck;
        public BTButton   adRewardBtn;
        public GameObject adRewardCheck;
        public GameObject cantrewardBtn_ad;
        public GameObject redDot;
        public GameObject redDot_ad;

        public Image icon;
        public Image icon_ad;


        [HideInInspector] public Definition.DataQuest questData;

        [HideInInspector]public PlayingRecordType recordType;
        public void Init(Definition.DataQuest q, PlayingRecordType r)
        {
            questData = q;
            recordType = r;

            //Debug.Log(string.Format("{0},{1}", recordType, questData.questType));
            var currentvalue = Player.Quest.CurrentValue(recordType, questData.questType);
            var maxvalue= Player.Quest.GoalValue(recordType, questData.questType);
            playingCount.value = (float)Player.Quest.CurrentValue(recordType, questData.questType) / (float)Player.Quest.GoalValue(recordType, questData.questType);

            if (questData.questType == QuestType.PlayingTime_sec)
            {
                playingCounttext.text = $"{currentvalue/60}/{maxvalue/60}";
            }
            else
            {
                playingCounttext.text = $"{currentvalue}/{maxvalue}";
            }

       
            rewardText.text = questData.rewardGoodValue.ToNumberString();
            rewardText_ad.text = (questData.rewardGoodValue*2).ToNumberString();

            icon.sprite = GoodResourcesBundle.Loaded.goodSprites[(int)q.rewardGoodkey];
            icon_ad.sprite = GoodResourcesBundle.Loaded.goodSprites[(int)q.rewardGoodkey];

            if(questData.questType==QuestType.PlayingTime_sec)
            {
                desc.text = string.Format(StaticData.Wrapper.localizeddesclist[(int)questData.questDesc].StringToLocal, (questData.goal/60));
            }
            else
            {
                desc.text = string.Format(StaticData.Wrapper.localizeddesclist[(int)questData.questDesc].StringToLocal, questData.goal);
            }
            

            Player.Quest.onChangeDailyRepeatQuest+=SyncPlayingRecord;

            bool isDaily = recordType == PlayingRecordType.Daily;
            adRewardBtn.gameObject.SetActive(isDaily);

            adRewardBtn.onClick.AddListener(()=> { GiveReward(true); });
            rewardBtn.onClick.AddListener(() => { GiveReward(false); });

            bool canrecieve = Player.Quest.CanRecieve(recordType, questData.questType);
            bool adcanrecieve = Player.Quest.CanRecieve(recordType, questData.questType, true);

            rewardBtn.enabled = canrecieve;
            rewardCheck.SetActive(Player.Quest.isAlreadyRewarded(questData.questType, false));
            cantrewardBtn.SetActive(!canrecieve);

            adRewardBtn.enabled = adcanrecieve;
            cantrewardBtn_ad.gameObject.SetActive(!adcanrecieve);
            adRewardCheck.SetActive(Player.Quest.isAlreadyRewarded(questData.questType, true));

            cantrewardBtn.SetActive(!canrecieve);

            redDot.SetActive(canrecieve);
            redDot_ad.SetActive(adcanrecieve);
            possibleRewardCount.gameObject.SetActive(false);
            bool isRepeat = recordType == PlayingRecordType.SubRepeat;
            if(isRepeat)
            {
                rewardCheck.SetActive(false);
                string localized = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_CompleteQuest].StringToLocal;
                possibleRewardCount.text = string.Format(localized, (currentvalue / maxvalue));
                possibleRewardCount.gameObject.SetActive(true);
            }
            
            if (canrecieve)
            {
                //Player.Option.menuRedDotCallback?.Invoke(MenuType.Quest,true);
            }
        }
        public void SyncPlayingRecord(Definition.QuestType questtype)
        {
            if(questData.questType==questtype)
            {
                var currentvalue = Player.Quest.CurrentValue(recordType, questData.questType);
                var maxvalue = Player.Quest.GoalValue(recordType, questData.questType);
                playingCount.value = (float)Player.Quest.CurrentValue(recordType, questData.questType) / (float)Player.Quest.GoalValue(recordType, questData.questType);
                if (questData.questType == QuestType.PlayingTime_sec)
                {
                    playingCounttext.text = $"{currentvalue / 60}/{maxvalue / 60}";
                }
                else
                {
                    playingCounttext.text = $"{currentvalue}/{maxvalue}";
                }

                bool canrecieve = Player.Quest.CanRecieve(recordType, questtype);
                bool adcanrecieve = Player.Quest.CanRecieve(recordType, questtype, true);

                rewardBtn.enabled = canrecieve;
                rewardCheck.SetActive(Player.Quest.isAlreadyRewarded(questData.questType, false));
                cantrewardBtn.SetActive(!canrecieve);

                adRewardBtn.enabled = adcanrecieve;
                cantrewardBtn_ad.gameObject.SetActive(!adcanrecieve);
                adRewardCheck.SetActive(Player.Quest.isAlreadyRewarded(questData.questType, true));

                cantrewardBtn.SetActive(!canrecieve);

                redDot.SetActive(canrecieve);
                redDot_ad.SetActive(adcanrecieve);

                if (CanRecieve())
                {
                    Player.Quest.onCompleteQuest?.Invoke();
                }

                if (canrecieve)
                {
                    //Player.Option.menuRedDotCallback?.Invoke(MenuType.Quest, true);
                }

                if(recordType==PlayingRecordType.SubRepeat)
                {
                    string localized = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_CompleteQuest].StringToLocal;
                    possibleRewardCount.text = string.Format(localized, (currentvalue / maxvalue));
                }

            }
            bool isRepeat = recordType == PlayingRecordType.SubRepeat;
            if (isRepeat)
            {
                rewardCheck.SetActive(false);
                
            }
        }
        public bool CanRecieve(bool isAD=false)
        {
            return Player.Quest.CanRecieve(recordType, questData.questType, isAD);
        }

        public void GiveReward(bool isAdreward)
        {
            if(CanRecieve(isAdreward))
            {
                if(isAdreward)
                {
                    if (Player.Cloud.inAppPurchase.purchaseAds)
                    {
                        Player.Quest.GiveReward(recordType, questData.questType, isAdreward);
                        if (recordType == PlayingRecordType.Daily)
                        {
                            Player.Cloud.playingRecord.dailyQuestCompleteCount++;
                        }
                        Player.Quest.onChangeAfterRecieveDailyRepeatQuest?.Invoke(questData.questType);
                    }
                    else
                    {
                        AdmobManager.Instance.ShowRewardedAd(() => {
                            Player.Quest.GiveReward(recordType, questData.questType, isAdreward);
                            if (recordType == PlayingRecordType.Daily)
                            {
                                Player.Cloud.playingRecord.dailyQuestCompleteCount++;
                            }
                            Player.Quest.onChangeAfterRecieveDailyRepeatQuest?.Invoke(questData.questType);
                        });
                    }
                }
                else
                {
                    Player.Quest.GiveReward(recordType, questData.questType, isAdreward);
                    if (recordType == PlayingRecordType.Daily)
                    {
                        Player.Cloud.playingRecord.dailyQuestCompleteCount++;
                    }
                    Player.Quest.onChangeAfterRecieveDailyRepeatQuest?.Invoke(questData.questType);
                }

                string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.UI_GoodsRewarded].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
            }
        }

        public void GiveRewardInSubrepeatAll()
        {
            if (CanRecieve())
            {
                Player.Quest.GiveRewardToRepeat(recordType, questData.questType, false);
                Player.Quest.onChangeAfterRecieveDailyRepeatQuest?.Invoke(questData.questType);

                string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.UI_GoodsRewarded].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
            }
        }
    }

}
