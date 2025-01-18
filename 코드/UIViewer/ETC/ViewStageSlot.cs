using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BlackTree.Core;
using TMPro;
using BlackTree.Model;
using System;
using BlackTree.Definition;

namespace BlackTree.Bundles
{
    public class StageInfo_scrollview
    {
        public int chapterIndex;
        public int stageIndex;
    }
    public class ViewStageSlot : UIRecycleViewCell<StageInfo_scrollview>
    {
        public Image stageImage;

        public BTButton enterCurrentStage;
        public Image goodIcon;
        public TMP_Text goodValue;
        public BTButton rewardCurrentStage;
        public TMP_Text startStage;
        public GameObject lockedObject;

        public TMP_Text goldIncrease;
        public TMP_Text expIncrease;
        public TMP_Text monsterHp;
        public TMP_Text monsterAttack;
        public TMP_Text stageMoveBtnText;

        int chapterindex;
        int stageindex;

        StageInfo_scrollview stagedata;

        private void Start()
        {
            Battle.Field.onChangeStage += UpdateStageText;
            stageMoveBtnText.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Move].StringToLocal;


        }
        public override void UpdateContent(StageInfo_scrollview itemData)
        {
            stagedata = itemData;
            chapterindex = itemData.chapterIndex;
            stageindex = itemData.stageIndex;

            enterCurrentStage.onClick.RemoveAllListeners();
            enterCurrentStage.onClick.AddListener(MoveToStage);

            rewardCurrentStage.onClick.RemoveAllListeners();
            rewardCurrentStage.onClick.AddListener(GetReward);

            startStage.text = string.Format("{0}-{1}", chapterindex+1, stageindex+1);

            int stageSpriteIndex = chapterindex  /100;

            stageImage.sprite = StageResourcesBundle.Loaded.stageSlotBGSprite[stageSpriteIndex];

            var stageinfo = StaticData.Wrapper.chapterRewardTableDatas[chapterindex];

            int expdescIndex = chapterindex * 5 + stageindex;

            double currentgoldRate = Battle.Field.GetRewardGoldForindex(chapterindex);
            double currentexpRate = Battle.Field.GetRewardExpForindex(expdescIndex);

            string goldtxt = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ETC_InitCommonUpgrade].StringToLocal;
            string exptxt = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Exp].StringToLocal;
            string monshptxt = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_MonsHp].StringToLocal;
            string monatktxt = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_MonsAtk].StringToLocal;
            goldIncrease.text = string.Format(goldtxt+": {0}", currentgoldRate.ToNumberString());
            expIncrease.text = string.Format(exptxt+": {0}", currentexpRate.ToNumberString());
            monsterHp.text = string.Format(monshptxt+": {0}", Battle.Field.CalculateHp(chapterindex, stageindex).ToNumberString());
            monsterAttack.text = string.Format(monatktxt+": {0}", Battle.Field.CalculateAtk(chapterindex).ToNumberString());

            var chapterRewardData = StaticData.Wrapper.chapterRewardTableDatas[stagedata.chapterIndex];

            goodIcon.sprite = GoodResourcesBundle.Loaded.goodSprites[(int)chapterRewardData.rewardGood];
            goodValue.text = string.Format("{0}", chapterRewardData.rewardValue);

            lockedObject.SetActive(true);

            if (chapterindex < Player.Cloud.field.bestChapter)
            {
                lockedObject.SetActive(false);
            }
            else if (chapterindex == Player.Cloud.field.bestChapter)
            {
                bool isUnlock = stageindex <= Player.Cloud.field.bestStage;
                lockedObject.SetActive(!isUnlock);
            }
#if UNITY_EDITOR
            lockedObject.SetActive(false);
#endif

            int rewardIndex = stagedata.chapterIndex*5 + stagedata.stageIndex;

            if (rewardIndex <=Player.Cloud.chapterRewardedData.LastRewardIndex)
            {
                rewardCurrentStage.gameObject.SetActive(false);
            }
            else
            {
                rewardCurrentStage.gameObject.SetActive(true);
            }
        }

      
        void MoveToStage()
        {
            int slotvalue = chapterindex * 10 + stageindex;
            int playerValue = Player.Cloud.field.bestChapter * 10 + Player.Cloud.field.bestStage;

#if UNITY_EDITOR
#else
            if (playerValue < slotvalue)
                return;
#endif


            var stageinfo = StaticData.Wrapper.chapterRewardTableDatas[stagedata.chapterIndex];

            Battle.Field.CurrentFieldChapter = stagedata.chapterIndex;
            Battle.Field.CurrentFieldStage = stagedata.stageIndex;

            Battle.Field.currentKillEnemy = 0;
            Battle.Field.ChangeSceneState(eSceneState.WaitForMainIdle);

            Player.Option.stageUIOff?.Invoke();

            
        }

        void AllReward()
        {
            GetReward();
        }
        void GetReward()
        {
            string returnToShield = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_canRewardGoods].StringToLocal;
            ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(returnToShield);
            return;

            bool canRecieve = false;
            if (chapterindex < Player.Cloud.field.bestChapter)
            {
                canRecieve = true;
            }
            else if (chapterindex == Player.Cloud.field.bestChapter)
            {
                canRecieve = stageindex <= Player.Cloud.field.bestStage;
            }

            if (canRecieve == false)
                return;
            int rewardIndex = stagedata.chapterIndex * 5 + stagedata.stageIndex;

            var chapterRewardData = StaticData.Wrapper.chapterRewardTableDatas[stagedata.chapterIndex];

            Player.ControllerGood.Earn(chapterRewardData.rewardGood, chapterRewardData.rewardValue);

            LocalSaveLoader.SaveUserCloudData();
            Player.Cloud.chapterRewardedData.UpdateHash().SetDirty(true);
            rewardCurrentStage.gameObject.SetActive(false);
        }
     
        void UpdateStageText()
        {
            lockedObject.SetActive(true);

            if (chapterindex < Player.Cloud.field.bestChapter)
            {
                lockedObject.SetActive(false);
            }
            else if (chapterindex == Player.Cloud.field.bestChapter)
            {
                bool isUnlock = stageindex <= Player.Cloud.field.bestStage;
                lockedObject.SetActive(!isUnlock);
            }

            int rewardIndex = stagedata.chapterIndex * 5 + stagedata.stageIndex;

            if (rewardIndex<= Player.Cloud.chapterRewardedData.LastRewardIndex)
            {
                rewardCurrentStage.gameObject.SetActive(false);
            }
            else
            {
                rewardCurrentStage.gameObject.SetActive(true);
            }
        }
    }
}
