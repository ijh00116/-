using BlackTree.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BlackTree.Model;
using System.Collections;
using BlackTree.Definition;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace BlackTree.Bundles
{
    public class ViewCanvasMainQuest : ViewCanvas
    {
        public Image goodIcon;
        public TMP_Text goodRewardCount;
        public TMP_Text desc;
        public TMP_Text playingCounttext;

        public BTButton rewardBtn;

        public RectTransform mainQuestUI;

        [HideInInspector] public Definition.DataQuest questData;

        [HideInInspector] public PlayingRecordType recordType;

        [SerializeField] Image questMainImage;
        const string ShinePropertyName = "_ShineLocation";
        float shineValue = 0;

        public void Init(Definition.DataQuest q, PlayingRecordType r)
        {
            questData = q;
            recordType = r;

            var currentvalue = Player.Quest.CurrentValue(recordType, questData.questType);
            var maxvalue = Player.Quest.GoalValue(recordType, questData.questType);

            goodIcon.sprite = GoodResourcesBundle.Loaded.goodSprites[(int)q.rewardGoodkey];
            goodRewardCount.text = string.Format( "{0}",q.rewardGoodValue.ToNumberString());

            if(questData.questType==QuestType.UserStage)
            {
                int currentChapter = currentvalue / 5;
                int currentStage = currentvalue % 5;
                int goalChapter = maxvalue / 5;
                int goalStage = maxvalue % 5;
                playingCounttext.text = $"{currentChapter+1}-{currentStage + 1}/{goalChapter + 1}-{goalStage + 1}";

                int index = questData.id + 1;
                if(index >=Player.Quest.mainQuestLoopStartIndex)
                {
                    desc.text = string.Format($"{StaticData.Wrapper.localizeddesclist[(int)questData.questDesc].StringToLocal}", $"{goalChapter + 1}-{goalStage + 1}");
                }
                else
                {
                    desc.text = string.Format($"{questData.id + 1}.{StaticData.Wrapper.localizeddesclist[(int)questData.questDesc].StringToLocal}", $"{goalChapter + 1}-{goalStage + 1}");
                }
                
            }
            else
            {
                playingCounttext.text = $"{currentvalue}/{maxvalue}";

                int index = questData.id + 1;
                if (index >= Player.Quest.mainQuestLoopStartIndex)
                {
                    desc.text = string.Format($"{StaticData.Wrapper.localizeddesclist[(int)questData.questDesc].StringToLocal}", questData.goal);
                }
                else
                {
                    desc.text = string.Format($"{questData.id + 1}.{StaticData.Wrapper.localizeddesclist[(int)questData.questDesc].StringToLocal}", questData.goal);
                }
                
            }

            Player.Quest.onChangeMainQuest += SyncPlayingRecord;

            rewardBtn.onClick.AddListener(GiveReward);

            CheckQuestComplete();
        }
        public void SyncPlayingRecord()
        {
            questData =StaticData.Wrapper.mainRepeatQuest[Player.Cloud.playingRecord.mainQuest.id];
            
            var currentvalue = Player.Quest.CurrentValue(recordType, questData.questType);
            var maxvalue = Player.Quest.GoalValue(recordType, questData.questType);
            goodIcon.sprite = GoodResourcesBundle.Loaded.goodSprites[(int)questData.rewardGoodkey];

            goodRewardCount.text = string.Format("{0}", questData.rewardGoodValue.ToNumberString());

            if (questData.questType == QuestType.UserStage)
            {
                int currentChapter = currentvalue / 5;
                int currentStage = currentvalue % 5;
                int goalChapter = maxvalue / 5;
                int goalStage = maxvalue % 5;
                playingCounttext.text = $"{currentChapter + 1}-{currentStage + 1}/{goalChapter + 1}-{goalStage + 1}";

                int index = questData.id + 1;
                if (index >= Player.Quest.mainQuestLoopStartIndex)
                {
                    desc.text = string.Format($"{StaticData.Wrapper.localizeddesclist[(int)questData.questDesc].StringToLocal}", $"{goalChapter + 1}-{goalStage + 1}");
                }
                else
                {
                    desc.text = string.Format($"{questData.id + 1}.{StaticData.Wrapper.localizeddesclist[(int)questData.questDesc].StringToLocal}", $"{goalChapter + 1}-{goalStage + 1}");
                }
                
            }
            else
            {
                playingCounttext.text = $"{currentvalue}/{maxvalue}";

                int index = questData.id + 1;
                if (index >= Player.Quest.mainQuestLoopStartIndex)
                {
                    desc.text = string.Format($"{StaticData.Wrapper.localizeddesclist[(int)questData.questDesc].StringToLocal}", questData.goal);
                }
                else
                {
                    desc.text = string.Format($"{questData.id + 1}.{StaticData.Wrapper.localizeddesclist[(int)questData.questDesc].StringToLocal}", questData.goal);
                }
            }

            CheckQuestComplete();
        }

        Coroutine colorCoroutine = null;
        void CheckQuestComplete()
        {
            bool canrecieve = Player.Quest.CanRecieve(recordType, questData.questType);

            if(canrecieve)
            {
                if(colorCoroutine==null)
                    colorCoroutine=StartCoroutine(QuestCompleteCallback());
            }
            else
            {
                questMainImage.material.SetFloat(ShinePropertyName, 0);
                
            }
        }

        enum ColorState
        {
            FadeIn,FadeOut,
        }
        IEnumerator QuestCompleteCallback()
        {
            questMainImage.material.SetFloat(ShinePropertyName, 0);

            shineValue = 0;
            while (true)
            {
                shineValue += Time.deltaTime;
                shineValue = Mathf.Lerp(0, 1, shineValue);
                questMainImage.material.SetFloat(ShinePropertyName, shineValue);
                if (shineValue >= 1)
                {
                    shineValue = 0;
                }
                
                yield return null;
            }
        }
        private void GiveReward()
        {
            if(Player.Quest.CanRecieve(recordType, questData.questType))
            {
                bool isEventExist = false;
                QuestClearEventData eventData = null;
                for(int i=0; i<StaticData.Wrapper.questClearEvent.Length; i++)
                {
                    if(StaticData.Wrapper.questClearEvent[i].mainQuestId== Player.Cloud.playingRecord.mainQuest.id)
                    {
                        isEventExist = true;
                        eventData = StaticData.Wrapper.questClearEvent[i];
                        break;
                    }
                }
                if(isEventExist)
                {
                    ShowEventPopup(eventData);
                }
                
                StopCoroutine(colorCoroutine);
                colorCoroutine = null;
                questMainImage.material.SetFloat(ShinePropertyName, 0);
                Player.Quest.GiveReward(recordType, questData.questType);
                AudioManager.Instance.Play(AudioSourceKey.GoodsRewarded);
                Player.Option.ContentUnlockUpdate?.Invoke();
                
                LogToFirebase(Player.Cloud.playingRecord.PlayingMainQuestCount);

                if(isEventExist==false)
                {
                    string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.UI_GoodsRewarded].StringToLocal;
                    ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                }

                //skillawake but 가이드미사일 없음
                if(Player.Cloud.playingRecord.mainQuest.id == 18)
                {
                    Player.Skill.Obtain(SkillKey.GuidedMissile, 1);
                }
            }
            else
            {
                if(Player.Quest.CurrentMainQuestType==Definition.QuestType.RPDungeonClear)
                {
                    if (Player.Cloud.tutorialData.tutorialCleared[(int)TutorialType.RPDungeonClear]==false)
                    {
                        Player.Cloud.tutorialData.tutorialCleared[(int)TutorialType.RPDungeonClear] = true;
                    }
                    Player.Guide.QuestSetQueue(Definition.QuestGuideType.RPDungeonClear);
                    Player.Guide.QuestGuideProgress(Definition.QuestGuideType.RPDungeonClear, true);
                }
                if (Player.Quest.CurrentMainQuestType == Definition.QuestType.ExpDungeonClear)
                {
                    if(Player.Cloud.tutorialData.tutorialCleared[(int)TutorialType.ExpDungeonClear]==false)
                    {
                        Player.Cloud.tutorialData.tutorialCleared[(int)TutorialType.ExpDungeonClear] = true;
                    }
                    Player.Guide.QuestSetQueue(Definition.QuestGuideType.ExpDungeonClear);
                    Player.Guide.QuestGuideProgress(Definition.QuestGuideType.ExpDungeonClear, true);
                }
                if (Player.Quest.CurrentMainQuestType == Definition.QuestType.GoldUpgrade_Attack||
                    Player.Quest.CurrentMainQuestType == Definition.QuestType.GoldUpgrade_MaxHp||
                    Player.Quest.CurrentMainQuestType == Definition.QuestType.GoldUpgrade_Hprecover||
                    Player.Quest.CurrentMainQuestType == Definition.QuestType.GoldUpgrade_CriRate ||
                    Player.Quest.CurrentMainQuestType == Definition.QuestType.GoldUpgrade_MaxShield ||
                    Player.Quest.CurrentMainQuestType == Definition.QuestType.GoldUpgrade_ShieldRecover||
                    Player.Quest.CurrentMainQuestType == Definition.QuestType.WitchSpeedIncrease||
                    Player.Quest.CurrentMainQuestType == Definition.QuestType.WitchSpeedMaxTimeIncrease)
                {
                    if(Player.Cloud.tutorialData.tutorialCleared[(int)TutorialType.GoldUpgrade]==false)
                    {
                        Player.Cloud.tutorialData.tutorialCleared[(int)TutorialType.GoldUpgrade] = true;
                    }
                    Player.Guide.QuestSetQueue(Definition.QuestGuideType.GoldUpgrade);
                    Player.Guide.QuestGuideProgress(Definition.QuestGuideType.GoldUpgrade, true);
                }
                if (Player.Quest.CurrentMainQuestType == Definition.QuestType.StatLevel)
                {
                    if(Player.Cloud.tutorialData.tutorialCleared[(int)TutorialType.StatusUpgrade]==false)
                    {
                        Player.Cloud.tutorialData.tutorialCleared[(int)TutorialType.StatusUpgrade] = true;
                    }
                    Player.Guide.QuestSetQueue(Definition.QuestGuideType.StatUpgrade);
                    Player.Guide.QuestGuideProgress(Definition.QuestGuideType.StatUpgrade, true);
                }
                if (Player.Quest.CurrentMainQuestType == Definition.QuestType.SummonEquip)
                {
                    if(Player.Cloud.tutorialData.tutorialCleared[(int)TutorialType.SummonEquip]==false)
                    {
                        Player.Cloud.tutorialData.tutorialCleared[(int)TutorialType.SummonEquip] = true;
                    }
                    Player.Guide.QuestSetQueue(Definition.QuestGuideType.SummonEquip);
                    Player.Guide.QuestGuideProgress(Definition.QuestGuideType.SummonEquip, true);
                }
                if (Player.Quest.CurrentMainQuestType == Definition.QuestType.SummonSkill)
                {
                    if(Player.Cloud.tutorialData.tutorialCleared[(int)TutorialType.SummonSkill]==false)
                    {
                        Player.Cloud.tutorialData.tutorialCleared[(int)TutorialType.SummonSkill] = true;
                    }
                    Player.Guide.QuestSetQueue(Definition.QuestGuideType.SummonSkill);
                    Player.Guide.QuestGuideProgress(Definition.QuestGuideType.SummonSkill, true);
                }
                if (Player.Quest.CurrentMainQuestType == Definition.QuestType.SummonPet )
                {
                    if(Player.Cloud.tutorialData.tutorialCleared[(int)TutorialType.SummonPet]==false)
                    {
                        Player.Cloud.tutorialData.tutorialCleared[(int)TutorialType.SummonPet] = true;
                    }
                    Player.Guide.QuestSetQueue(Definition.QuestGuideType.SummonPet);
                    Player.Guide.QuestGuideProgress(Definition.QuestGuideType.SummonPet, true);
                }

                if (Player.Quest.CurrentMainQuestType == Definition.QuestType.EquipItemUse)
                {
                    Player.Guide.QuestSetQueue(Definition.QuestGuideType.ItemEquip);
                    Player.Guide.QuestGuideProgress(Definition.QuestGuideType.ItemEquip, true);
                }

                if (Player.Quest.CurrentMainQuestType == Definition.QuestType.EquipSkill)
                {
                    Player.Guide.QuestSetQueue(Definition.QuestGuideType.SkillEquip);
                    Player.Guide.QuestGuideProgress(Definition.QuestGuideType.SkillEquip, true);
                }

                if (Player.Quest.CurrentMainQuestType == Definition.QuestType.QuestDailyComplete)
                {
                    Player.Guide.QuestSetQueue(Definition.QuestGuideType.QuestComplete);
                    Player.Guide.QuestGuideProgress(Definition.QuestGuideType.QuestComplete, true);
                }

                if (Player.Quest.CurrentMainQuestType == Definition.QuestType.OpenProfileDetail)
                {
                    Player.Guide.QuestSetQueue(Definition.QuestGuideType.OpenProfileDetail);
                    Player.Guide.QuestGuideProgress(Definition.QuestGuideType.OpenProfileDetail, true);
                }

                if (Player.Quest.CurrentMainQuestType == Definition.QuestType.GetStageReward)
                {
                    Player.Guide.QuestSetQueue(Definition.QuestGuideType.StageReward);
                    Player.Guide.QuestGuideProgress(Definition.QuestGuideType.StageReward, true);
                }

                if (Player.Quest.CurrentMainQuestType == Definition.QuestType.EquipPet)
                {
                    Player.Guide.QuestSetQueue(Definition.QuestGuideType.PetEquip);
                    Player.Guide.QuestGuideProgress(Definition.QuestGuideType.PetEquip, true);
                }
                if (Player.Quest.CurrentMainQuestType == Definition.QuestType.KillEnemy|| Player.Quest.CurrentMainQuestType == Definition.QuestType.ActivateAutoSkill)
                {
                    if (ViewCanvas.Get<ViewCanvasEquipInventory>().IsVisible == true)
                        MainNav.SetTabIndex(ControllerEquipInventory._index);
                    if (ViewCanvas.Get<ViewCanvasSkillInventory>().IsVisible == true)
                        MainNav.SetTabIndex(ControllerSkillInventory._index);
                    if (ViewCanvas.Get<ViewCanvasUnitUpgrade>().IsVisible == true)
                        MainNav.SetTabIndex(ControllerUnitUpgrade._index);
                    if (ViewCanvas.Get<ViewCanvasPetInventory>().IsVisible == true)
                        MainNav.SetTabIndex(ControllerPetInventory._index);
                    if (ViewCanvas.Get<ViewCanvasContents>().IsVisible == true)
                        MainNav.SetTabIndex(ControllerContents._index);
                }
            }
        }

        void ShowEventPopup(QuestClearEventData eventData)
        {
            for(int i=0; i< eventData.rewardType.Length; i++)
            {
                if (eventData.rewardType[i] == RewardTypes.package_multielectric)
                {
                    Player.Skill.Obtain(SkillKey.MultipleElectric, 1);
                }
                else if (eventData.rewardType[i] == RewardTypes.package_nova)
                {
                    Player.Skill.Obtain(SkillKey.NoveForSeconds, 1);
                }
                else if (eventData.rewardType[i] == RewardTypes.package_meteor)
                {
                    Player.Skill.Obtain(SkillKey.SpawnMeteor, 1);
                }
                else
                {
                    var goodType = RewardToGoods(eventData.rewardType[i]);
                    Player.ControllerGood.Earn(goodType, eventData.amount[i]);
                }
            }

            PopupRewardPopup(eventData);
        }

        private void PopupRewardPopup(QuestClearEventData eventData)
        {
            var toastCanvas = ViewCanvas.Get<ViewCanvasToastMessage>();

            Dictionary<Definition.RewardTypes, double> goodsAmount = new Dictionary<Definition.RewardTypes, double>();

            for (int i = 0; i < eventData.rewardType.Length; i++)
            {
                if (goodsAmount.ContainsKey(eventData.rewardType[i]))
                {
                    goodsAmount[eventData.rewardType[i]] += eventData.amount[i];
                }
                else
                {
                    goodsAmount.Add(eventData.rewardType[i], eventData.amount[i]);
                }
            }

            for (int i = 0; i < toastCanvas.rewardSlotList.Count; i++)
            {
                toastCanvas.rewardSlotList[i].gameObject.SetActive(false);
            }
            int index = 0;
            foreach (var rewardData in goodsAmount)
            {
                ViewGoodRewardSlot slotObj = null;
                if (index < toastCanvas.rewardSlotList.Count)
                {
                    slotObj = toastCanvas.rewardSlotList[index];
                }
                else
                {
                    slotObj = UnityEngine.Object.Instantiate(toastCanvas.rewardSlotPrefab);
                    slotObj.transform.SetParent(toastCanvas.rewardParent, false);
                    toastCanvas.rewardSlotList.Add(slotObj);
                }
                slotObj.goodValue.text = rewardData.Value.ToNumberString();
                slotObj.goodsIcon.sprite = GoodResourcesBundle.Loaded.rewardSprites[(int)rewardData.Key];

                slotObj.gameObject.SetActive(true);

                index++;
            }

            string earnTitle;
            string localizedskillname = "";
            string skillLocal= StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Skill].StringToLocal;
            if (eventData.rewardType[0]==RewardTypes.package_nova)
            {
                localizedskillname = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Skill_NoveForSeconds].StringToLocal;
                earnTitle = $"{localizedskillname} {skillLocal}";
            }
            else if (eventData.rewardType[0] == RewardTypes.package_meteor)
            {
                localizedskillname = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Skill_SpawnMeteor].StringToLocal;
                earnTitle = $"{localizedskillname} {skillLocal}";
            }
            else if (eventData.rewardType[0] == RewardTypes.package_multielectric)
            {
                localizedskillname = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Skill_MultipleElectric].StringToLocal;
                earnTitle = $"{localizedskillname} {skillLocal}";
            }
            else
            {
                earnTitle = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Goods_SkillAwakeStone].StringToLocal;
            }
            string localized= StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_tutoCompleteReward].StringToLocal;
            toastCanvas.titleDesc.text = string.Format(localized, eventData.mainQuestId+1, earnTitle);
            toastCanvas.RewardPopupShowandFade();

            AudioManager.Instance.Play(AudioSourceKey.GoodsRewarded);
        }

        public static GoodsKey RewardToGoods(RewardTypes _type)
        {
            GoodsKey goodkey = GoodsKey.None;

            switch (_type)
            {
                case RewardTypes.None:
                    break;
                case RewardTypes.Coin:
                    goodkey = GoodsKey.Coin;
                    break;
                case RewardTypes.Dia:
                    goodkey = GoodsKey.Dia;
                    break;
                case RewardTypes.StatusPoint:
                    goodkey = GoodsKey.StatusPoint;
                    break;
                case RewardTypes.AwakeStone:
                    goodkey = GoodsKey.AwakeStone;
                    break;
                case RewardTypes.ResearchPotion:
                    goodkey = GoodsKey.ResearchPotion;
                    break;
                case RewardTypes.skillAwakeStone:
                    goodkey = GoodsKey.SkillAwakeStone;
                    break;
                case RewardTypes.Exp:
                    break;
                case RewardTypes.Pet:
                    break;
                case RewardTypes.ADRemove:
                    break;
                case RewardTypes.Vip_1:
                    break;
                case RewardTypes.Vip_2:
                    break;
                case RewardTypes.Vip_3:
                    break;
                case RewardTypes.riftDungeonKey:
                    goodkey = GoodsKey.RiftDungeonTicket;
                    break;
                default:
                    break;
            }

            return goodkey;
        }

        private void LogToFirebase(int qIndex)
        {
            FirebaseManager.Instance.LogEvent("quest_clear",
                        new Firebase.Analytics.Parameter("quest_clearCount", qIndex.ToString()));
        }

    }
}

