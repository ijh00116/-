using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using BlackTree.Definition;
using Cysharp.Threading.Tasks;
using System;
using DG.Tweening;

namespace BlackTree.Core
{
    public class ControllerGoldDungeonIngame
    {
        private ViewCanvasDungeonIngame _viewDungeonIngame;
        private CancellationTokenSource _cts;

        List<ViewGoodRewardSlot> slotList = new List<ViewGoodRewardSlot>();

        public ControllerGoldDungeonIngame(Transform parent, CancellationTokenSource cts)
        {
            _cts = cts;
            _viewDungeonIngame = ViewCanvas.Create<ViewCanvasDungeonIngame>(parent);

            for(int i=0; i< _viewDungeonIngame.EndDungeonBtn.Length; i++)
            {
                int index = i;
                _viewDungeonIngame.EndDungeonBtn[index].onClick.AddListener(TouchConfirmInExitWindow);
            }

            Battle.Dungeon.DungeonReady += IntroToStart;
            Battle.Dungeon.DungeonEnd += PopupEndWindow;

            Message.AddListener<KillDungeonEnemy>(KillEnemyUpdate);
            Message.AddListener<DungeonLevelUp>(ExpDungeonLevelUpdate);
            Message.AddListener<DungeonWaveUp>(DungeonWaveUpdate);

            _viewDungeonIngame.inGameUI_Exp.SetActive(false);
            _viewDungeonIngame.inGameUI_Gold.SetActive(false);

            _viewDungeonIngame.openOutWindowBtn.onClick.AddListener(OpenOutDungeonWindow);

            for(int i=0; i<_viewDungeonIngame.cancelOutBtn.Length; i++)
            {
                _viewDungeonIngame.cancelOutBtn[i].onClick.AddListener(BacktoDungeon);
            }
            _viewDungeonIngame.confirmOutBtn.onClick.AddListener(TouchConfirmInExitWindow);

             _viewDungeonIngame.outDungeonTitle.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Out].StringToLocal; 
             _viewDungeonIngame.outDungeonDesc.text = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_AskDungeonOut].StringToLocal;
            _viewDungeonIngame.outDungeonCancel.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Cancel].StringToLocal;
            _viewDungeonIngame.outDungeonOut.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Out].StringToLocal;
            _viewDungeonIngame.exitWindow_success.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Victory].StringToLocal;
            _viewDungeonIngame.exitWindow_fail.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Fail].StringToLocal;
            _viewDungeonIngame.exitWindowTitletxt_success.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Victory].StringToLocal;
            _viewDungeonIngame.exitWindowTitletxt_fail.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Fail].StringToLocal;
            _viewDungeonIngame.exitWindowContinue_success.text = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_GotoMain].StringToLocal;
            _viewDungeonIngame.exitWindowContinue_fail.text = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_GotoMain].StringToLocal;
        }

        void IntroToStart(DungeonType _type)
        {
            Battle.Dungeon.currentDungeonType = _type;
            Battle.Dungeon.DungeonPlayingTime = 0;

            AllwindowOff();
            switch (_type)
            {
                case DungeonType.Research:
                    _viewDungeonIngame.inGameUI_Gold.SetActive(true);
                    Battle.Dungeon.DungeonMaxTime = Battle.Dungeon.DungeonMaxTimeInGold;

                    _viewDungeonIngame.currentKillEnemySlider.value = (float)Battle.Dungeon.currentDungeonData[(int)DungeonType.Research].currentKillEnemy / (float)Battle.Dungeon.GoldDungeonClearEnemyCount;
                    _viewDungeonIngame.currentKillEnemy_gold.text = string.Format("{0}/{1}", Battle.Dungeon.currentDungeonData[(int)DungeonType.Research].currentKillEnemy, Battle.Dungeon.GoldDungeonClearEnemyCount);
                    _viewDungeonIngame.dungeonTimeText_gold.text = string.Format("{0:F0}", Battle.Dungeon.DungeonMaxTime - Battle.Dungeon.DungeonPlayingTime);
                    _viewDungeonIngame.dungeonTime_gold.value = 1;
                    break;
                case DungeonType.Exp:
                    _viewDungeonIngame.inGameUI_Exp.SetActive(true);
                    Battle.Dungeon.DungeonMaxTime = Battle.Dungeon.DungeonMaxTimeInExp;

                    _viewDungeonIngame.dungeonTimeSlider_exp.value = 1;
                    _viewDungeonIngame.dungeonTimeText_exp.text = string.Format("{0:F0}", Battle.Dungeon.DungeonMaxTime - Battle.Dungeon.DungeonPlayingTime);
                    _viewDungeonIngame.killenemyCount.text= string.Format("{0}", Battle.Dungeon.currentDungeonData[(int)Battle.Dungeon.currentDungeonType].currentKillEnemy);
                    break;
                case DungeonType.Awake:
                    _viewDungeonIngame.inGameUI_awake.SetActive(true);
                    Battle.Dungeon.DungeonMaxTime = Battle.Dungeon.DungeonMaxTimeInAwake;
                    string temptxt= StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_giveDmg].StringToLocal;
                    _viewDungeonIngame.totalDmg.text = string.Format(temptxt,
                        Battle.Dungeon.currentDungeonData[(int)DungeonType.Awake].currentDamage.ToNumberString(), (Battle.Dungeon.currentDungeonData[(int)DungeonType.Awake].currentDamage / 10).ToNumberString());
                    _viewDungeonIngame.dungeonTimeSlider_awake.value = 1;
                    _viewDungeonIngame.dungeonTimeText_awake.text = string.Format("{0:F0}", Battle.Dungeon.DungeonMaxTime - Battle.Dungeon.DungeonPlayingTime);
                    break;
                case DungeonType.Rift:
                    _viewDungeonIngame.inGameUI_Rift.SetActive(true);
                    Battle.Dungeon.DungeonMaxTime = Battle.Dungeon.DungeonMaxTimeInRift;

                    float sliderValue = (float)((float)(Battle.Dungeon.currentDungeonData[(int)DungeonType.Rift].currentWave) / (float)Battle.Dungeon.riftDungeonMaxWave);
                    _viewDungeonIngame.dungeonWaveSlider_Rift.value = sliderValue;
                    _viewDungeonIngame.currentTimeSlider_Rift.value = 1;
                    _viewDungeonIngame.dungeonTimeText_Rift.text = string.Format("{0:F0}", Battle.Dungeon.DungeonMaxTime - Battle.Dungeon.DungeonPlayingTime);
                    break;
                case DungeonType.Rune:
                    _viewDungeonIngame.inGameUI_Rune.SetActive(true);
                    Battle.Dungeon.DungeonMaxTime = Battle.Dungeon.DungeonMaxTimeInRune;

                    _viewDungeonIngame.dungeonTimeSlider_rune.value = 1;
                    _viewDungeonIngame.dungeonTimeText_rune.text = string.Format("{0:F0}", Battle.Dungeon.DungeonMaxTime - Battle.Dungeon.DungeonPlayingTime);
                    _viewDungeonIngame.currentLevelCount_rune.text = string.Format("Level {0:F0}", Battle.Dungeon.RuneDungeonCurrentLevel);
                    break;
                case DungeonType.End:
                    break;
                default:
                    break;
            }
        
            IntroFlow().Forget();
        }

        void AllwindowOff()
        {
            _viewDungeonIngame.inGameUI_Gold.SetActive(false);
            _viewDungeonIngame.inGameUI_Exp.SetActive(false);
            _viewDungeonIngame.inGameUI_awake.SetActive(false);
            _viewDungeonIngame.inGameUI_Rift.SetActive(false);
            _viewDungeonIngame.inGameUI_Rune.SetActive(false);
        }

        async UniTaskVoid IntroFlow()
        {
            float currentTime=0;
            float maxtime = 0.6f;
            _viewDungeonIngame.outWindow.SetActive(false);
            _viewDungeonIngame.exitWindow.SetActive(false);
            while (currentTime<maxtime)
            {
                currentTime += Time.deltaTime;
                await UniTask.Yield(_cts.Token);
            }

            Battle.Dungeon.DungeonStart?.Invoke(Battle.Dungeon.currentDungeonType);
            Battle.Field.UnitRestart();
        
            TimeCheck().Forget();
        }

        async UniTaskVoid TimeCheck()
        {
            float leftTime = 0;
            while (Battle.Dungeon.dungeonEnd == false)
            {
                if (Battle.Dungeon.dungeonEnd)
                    break;
                if (Battle.Field.IsDungeonMainScene)
                {
                    Battle.Dungeon.DungeonPlayingTime += Time.deltaTime;
                    leftTime = Battle.Dungeon.DungeonMaxTime - Battle.Dungeon.DungeonPlayingTime;
                    switch (Battle.Dungeon.currentDungeonType)
                    {
                        case DungeonType.Research:
                            _viewDungeonIngame.currentKillEnemySlider.value = (float)Battle.Dungeon.currentDungeonData[(int)DungeonType.Research].currentKillEnemy / (float)Battle.Dungeon.GoldDungeonClearEnemyCount;
                            _viewDungeonIngame.currentKillEnemy_gold.text = string.Format("{0}/{1}", Battle.Dungeon.currentDungeonData[(int)DungeonType.Research].currentKillEnemy, Battle.Dungeon.GoldDungeonClearEnemyCount);
                            _viewDungeonIngame.dungeonTimeText_gold.text = string.Format("{0:F0}", Battle.Dungeon.DungeonMaxTime - Battle.Dungeon.DungeonPlayingTime);
                            _viewDungeonIngame.dungeonTime_gold.value = leftTime / Battle.Dungeon.DungeonMaxTime;
                            break;
                        case DungeonType.Exp:
                            _viewDungeonIngame.dungeonTimeSlider_exp.value = leftTime / Battle.Dungeon.DungeonMaxTime;
                            _viewDungeonIngame.dungeonTimeText_exp.text = string.Format("{0:F0}", Battle.Dungeon.DungeonMaxTime - Battle.Dungeon.DungeonPlayingTime);
                            break;
                        case DungeonType.Awake:
                            string temptxt = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_giveDmg].StringToLocal;
                            _viewDungeonIngame.totalDmg.text = string.Format(temptxt,
                                Battle.Dungeon.currentDungeonData[(int)DungeonType.Awake].currentDamage.ToNumberString(), (Battle.Dungeon.currentDungeonData[(int)DungeonType.Awake].currentDamage / 10).ToNumberString());
                            _viewDungeonIngame.dungeonTimeSlider_awake.value = leftTime / Battle.Dungeon.DungeonMaxTime;
                            _viewDungeonIngame.dungeonTimeText_awake.text = string.Format("{0:F0}", Battle.Dungeon.DungeonMaxTime - Battle.Dungeon.DungeonPlayingTime);
                            break;
                        case DungeonType.Rift:
                            //_viewDungeonIngame.dungeonWaveSlider_Rift.value =
                            //    (float)Battle.Dungeon.currentDungeonData[(int)DungeonType.Awake].currentWave / (float)Battle.Dungeon.riftDungeonMaxWave;
                            _viewDungeonIngame.currentTimeSlider_Rift.value = leftTime / Battle.Dungeon.DungeonMaxTime;
                            _viewDungeonIngame.dungeonTimeText_Rift.text = string.Format("{0:F0}", Battle.Dungeon.DungeonMaxTime - Battle.Dungeon.DungeonPlayingTime);
                            break;
                        case DungeonType.Rune:
                            _viewDungeonIngame.dungeonTimeSlider_rune.value = leftTime / Battle.Dungeon.DungeonMaxTime;
                            _viewDungeonIngame.dungeonTimeText_rune.text = string.Format("{0:F0}", Battle.Dungeon.DungeonMaxTime - Battle.Dungeon.DungeonPlayingTime);
                            _viewDungeonIngame.currentLevelCount_rune.text = string.Format("Level {0:F0}", Battle.Dungeon.RuneDungeonCurrentLevel);
                            break;
                        case DungeonType.End:
                            break;
                        default:
                            break;
                    }
                }

                await UniTask.Yield(_cts.Token);
            }
        }

        void PopupEndWindow(DungeonType dungeontype,bool isSuccess)
        {
            Battle.Field.MainEnemyStop();
            Battle.Field.UnitStop();

            _viewDungeonIngame.exitWindow.SetActive(true);
            _viewDungeonIngame.clearWindow.SetActive(isSuccess);
            _viewDungeonIngame.failWindow.SetActive(!isSuccess);

            closing = false;

            ViewGoodRewardSlot slotobj = null;
            for (int i = 0; i < slotList.Count; i++)
            {
                slotList[i].gameObject.SetActive(false);
            }


            int levelIndex=0;
            switch (Battle.Dungeon.currentDungeonType)
            {
                case DungeonType.Research:
                    if(isSuccess)
                    {
                        levelIndex = Battle.Dungeon.currentDungeonData[(int)DungeonType.Research].currentDungeonLevel;
                        var rewardTableData_RP = StaticData.Wrapper.rpDungeonRewardTableDatas[levelIndex];

                        for (int i = 0; i < rewardTableData_RP.rewardAmount.Length; i++)
                        {
                            if (slotList.Count <= i)
                            {
                                slotobj = UnityEngine.Object.Instantiate(_viewDungeonIngame.rewardSlotPrefab);
                                slotList.Add(slotobj);
                                slotobj.gameObject.SetActive(true);
                            }
                            else
                            {
                                slotobj = slotList[i];
                                slotobj.gameObject.SetActive(true);
                            }
                            slotobj.transform.SetParent(_viewDungeonIngame.rewardSlotParent_clear, false);
                            slotobj.goodsIcon.sprite = GoodResourcesBundle.Loaded.rewardSprites[(int)rewardTableData_RP.rewardType[i]];
                            slotobj.goodValue.text = rewardTableData_RP.rewardAmount[i].ToNumberString();
                            slotobj.goodsDesc.text = "";

                            RewardObtain((rewardTableData_RP.rewardType[i]), rewardTableData_RP.rewardAmount[i]);
                        }
                        string temptxt = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_ClearContents].StringToLocal;
                        _viewDungeonIngame.clearLevelDesc_clear.text = string.Format(temptxt, Battle.Dungeon.currentDungeonData[(int)DungeonType.Rift].currentDungeonLevel + 1);

                        if (Battle.Dungeon.currentDungeonData[(int)DungeonType.Research].currentDungeonLevel >= Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Research].bestLevel)
                        {
                            Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Research].bestLevel = Battle.Dungeon.currentDungeonData[(int)DungeonType.Research].currentDungeonLevel;
                        }
                        Player.ControllerGood.Consume(Definition.GoodsKey.RPDungeonTicket, 1);
                        Player.Quest.TryCountUp(QuestType.RPDungeonClear, 1);
                        Player.Quest.TryCountUp(QuestType.DungeonClear, 1);
                    }
                    else
                    {
                        string temptxt = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_FailContents].StringToLocal;
                        _viewDungeonIngame.LevelDesc_fail.text= string.Format(temptxt, Battle.Dungeon.currentDungeonData[(int)DungeonType.Research].currentDungeonLevel + 1);
                    }

                    break;
                case DungeonType.Exp:
                    if (isSuccess)
                    {
                        levelIndex = Battle.Dungeon.currentDungeonData[(int)DungeonType.Exp].currentDungeonLevel;
                        
                        if (slotList.Count <= 1)
                        {
                            slotobj = UnityEngine.Object.Instantiate(_viewDungeonIngame.rewardSlotPrefab);
                            slotList.Add(slotobj);
                            slotobj.gameObject.SetActive(true);
                        }
                        else
                        {
                            slotobj = slotList[0];
                            slotobj.gameObject.SetActive(true);
                        }
                        var expRewardValue = Battle.Field.GetRewardExpDungeonExpForEnemy(levelIndex) * Battle.Dungeon.DungeonRewardRate * Battle.Dungeon.currentDungeonData[(int)DungeonType.Exp].currentKillEnemy;

                        slotobj.transform.SetParent(_viewDungeonIngame.rewardSlotParent_clear, false);
                        slotobj.goodsIcon.sprite = GoodResourcesBundle.Loaded.rewardSprites[(int)RewardTypes.Exp];
                        slotobj.goodValue.text = expRewardValue.ToNumberString();
                        slotobj.goodsDesc.text = "EXP";

                        Player.Level.ExpUpAndLvUp(expRewardValue);

                        string temptxt = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_ClearContents].StringToLocal;
                        _viewDungeonIngame.clearLevelDesc_clear.text = string.Format(temptxt, Battle.Dungeon.currentDungeonData[(int)DungeonType.Exp].currentDungeonLevel + 1);

                        if (Battle.Dungeon.currentDungeonData[(int)DungeonType.Exp].currentDungeonLevel >= Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Exp].bestLevel)
                        {
                            Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Exp].bestLevel = Battle.Dungeon.currentDungeonData[(int)DungeonType.Exp].currentDungeonLevel;
                        }
                        if (Battle.Dungeon.currentDungeonData[(int)DungeonType.Exp].currentKillEnemy >= Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Exp].bestKillCount)
                        {
                            Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Exp].bestKillCount = Battle.Dungeon.currentDungeonData[(int)DungeonType.Exp].currentKillEnemy;
                        }

                        Player.ControllerGood.Consume(Definition.GoodsKey.ExpDungeonTicket, 1);
                        Player.Quest.TryCountUp(QuestType.ExpDungeonClear, 1);
                        Player.Quest.TryCountUp(QuestType.DungeonClear, 1);
                    }
                    else
                    {
                        string temptxt = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_FailContents].StringToLocal;
                        _viewDungeonIngame.LevelDesc_fail.text = string.Format(temptxt, Battle.Dungeon.currentDungeonData[(int)DungeonType.Exp].currentDungeonLevel + 1);
                    }
                    break;
                case DungeonType.Awake:
                    if (isSuccess)
                    {
                        if (Battle.Dungeon.currentDungeonData[(int)DungeonType.Awake].currentDamage >= Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Awake].bestDamage)
                        {
                            Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Awake].bestDamage = Battle.Dungeon.currentDungeonData[(int)DungeonType.Awake].currentDamage;
                        }

                        double rewardAmount = Battle.Dungeon.currentDungeonData[(int)DungeonType.Awake].currentDamage/10.0f;
                        
                        if (slotList.Count <= 0)
                        {
                            slotobj = UnityEngine.Object.Instantiate(_viewDungeonIngame.rewardSlotPrefab);
                            slotList.Add(slotobj);
                            slotobj.gameObject.SetActive(true);
                        }
                        else
                        {
                            slotobj = slotList[0];
                            slotobj.gameObject.SetActive(true);
                        }
                        slotobj.transform.SetParent(_viewDungeonIngame.rewardSlotParent_clear, false);
                        slotobj.goodsIcon.sprite = GoodResourcesBundle.Loaded.rewardSprites[(int)RewardTypes.AwakeStone];
                        slotobj.goodValue.text = rewardAmount.ToNumberString();
                        slotobj.goodsDesc.text = "";

                        Player.ControllerGood.Earn((GoodsKey)(RewardTypes.AwakeStone), rewardAmount);
                        string temptxt = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_Dmg].StringToLocal;
                        _viewDungeonIngame.clearLevelDesc_clear.text = string.Format(temptxt, Battle.Dungeon.currentDungeonData[(int)DungeonType.Awake].currentDamage.ToNumberString());
                        Player.ControllerGood.Consume(Definition.GoodsKey.AwakeDungeonTicket, 1);

                        Player.Quest.TryCountUp(QuestType.AwakeDungeonClear, 1);
                        Player.Quest.TryCountUp(QuestType.DungeonClear, 1);
                    }
                    else
                    {
                       //_viewDungeonIngame.LevelDesc_fail.text = string.Format("<color=#A6642E>{0}단계</color> 실패", Battle.Dungeon.currentDungeonData[(int)DungeonType.Awake].currentDungeonLevel);
                    }
                    break;
                case DungeonType.Rift:
                    if (isSuccess)
                    {
                        levelIndex = Battle.Dungeon.currentDungeonData[(int)DungeonType.Rift].currentDungeonLevel;
                        var rewardTableData_rift = StaticData.Wrapper.riftDungeonRewardTableDatas[levelIndex];
                        for (int i = 0; i < rewardTableData_rift.rewardAmount.Length; i++)
                        {
                            if (slotList.Count <= i)
                            {
                                slotobj = UnityEngine.Object.Instantiate(_viewDungeonIngame.rewardSlotPrefab);
                                slotList.Add(slotobj);
                                slotobj.gameObject.SetActive(true);
                            }
                            else
                            {
                                slotobj = slotList[i];
                                slotobj.gameObject.SetActive(true);
                            }

                            double perishRewardAmount = rewardTableData_rift.rewardAmount[i];
                            if(levelIndex<= Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Rift].bestLevel)
                            {
                                perishRewardAmount = rewardTableData_rift.rewardAmount[i]/2.0f;
                            }

                            slotobj.transform.SetParent(_viewDungeonIngame.rewardSlotParent_clear, false);
                            slotobj.goodsIcon.sprite = GoodResourcesBundle.Loaded.rewardSprites[(int)rewardTableData_rift.rewardType[i]];
                            slotobj.goodValue.text = perishRewardAmount.ToNumberString();
                            slotobj.goodsDesc.text = "";

                            RewardObtain((rewardTableData_rift.rewardType[i]), perishRewardAmount);
                        }

                        string temptxt = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_ClearContents].StringToLocal;
                        _viewDungeonIngame.clearLevelDesc_clear.text = string.Format(temptxt, Battle.Dungeon.currentDungeonData[(int)DungeonType.Rift].currentDungeonLevel + 1);

                        if (Battle.Dungeon.currentDungeonData[(int)DungeonType.Rift].currentDungeonLevel >= Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Rift].bestLevel)
                        {
                            Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Rift].bestLevel = Battle.Dungeon.currentDungeonData[(int)DungeonType.Rift].currentDungeonLevel;
                        }
                        Player.ControllerGood.Consume(Definition.GoodsKey.RiftDungeonTicket, 1);
                        Player.Quest.TryCountUp(QuestType.RiftDungeonClear, 1);
                        Player.Quest.TryCountUp(QuestType.DungeonClear, 1);
                    }
                    else
                    {
                        string temptxt = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_FailContents].StringToLocal;
                        _viewDungeonIngame.LevelDesc_fail.text = string.Format(temptxt, Battle.Dungeon.currentDungeonData[(int)DungeonType.Rift].currentDungeonLevel + 1);
                    }
                    break;

                case DungeonType.Rune:
                    if (isSuccess)
                    {
                        int index = -1;
                        for(int i=0; i<StaticData.Wrapper.runeDungeonRewardDatas.Length; i++)
                        {
                            if(Battle.Dungeon.RuneDungeonCurrentLevel>= StaticData.Wrapper.runeDungeonRewardDatas[i].Level)
                            {
                                index++;
                            }
                            else
                            {
                                break;
                            }
                        }

                        var runeRewardData = StaticData.Wrapper.runeDungeonRewardDatas[index];

                        Dictionary<int, int> runeIndexList = new Dictionary<int, int>();

                        for (int i = 0; i < runeRewardData.count; i++)
                        {
                            int randomIndex = GetRewardRuneIndex(runeRewardData);
                            int runeIndex = runeRewardData.runeRewardTable[randomIndex];

                            if(runeIndexList.ContainsKey(runeIndex))
                            {
                                runeIndexList[runeIndex]++;
                            }
                            else
                            {
                                runeIndexList.Add(runeIndex, 1);
                            }
                        }

                        Player.Cloud.Dungeondata.runeDungeonRewardHistory.Clear();

                        int slotindex = 0;
                        foreach(var _data in runeIndexList)
                        {
                            if (slotList.Count <= slotindex)
                            {
                                slotobj = UnityEngine.Object.Instantiate(_viewDungeonIngame.rewardSlotPrefab);
                                slotList.Add(slotobj);
                                slotobj.gameObject.SetActive(true);
                            }
                            else
                            {
                                slotobj = slotList[slotindex];
                                slotobj.gameObject.SetActive(true);
                            }

                            slotobj.transform.SetParent(_viewDungeonIngame.rewardSlotParent_clear, false);
                            slotobj.goodsIcon.sprite = PetResourcesBundle.Loaded.runeImage[_data.Key];
                            slotobj.goodValue.text = _data.Value.ToString();

                            Player.Rune.Obtain(_data.Key, _data.Value);

                            slotindex++;

                            Player.Cloud.Dungeondata.runeDungeonRewardHistory.Add(_data.Key);
                        }

                        if (Battle.Dungeon.RuneDungeonCurrentLevel>Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Rune].bestLevel)
                        {
                            Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Rune].bestLevel = Battle.Dungeon.RuneDungeonCurrentLevel;
                        }
                        string temptxt = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_ClearContents].StringToLocal;
                        _viewDungeonIngame.clearLevelDesc_clear.text = string.Format(temptxt, Battle.Dungeon.RuneDungeonCurrentLevel);

                        Player.ControllerGood.Consume(Definition.GoodsKey.RuneDungeonTicket, 1);
                        Player.Quest.TryCountUp(QuestType.RuneDungeonClear, 1);
                    }
                    else
                    {
                        string temptxt = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_runefail].StringToLocal;
                        _viewDungeonIngame.LevelDesc_fail.text = string.Format(temptxt);
                    }
                    break;
                case DungeonType.End:
                    break;
                default:
                    break;
            }

            Player.Cloud.Dungeondata.UpdateHash().SetDirty(true);
            LocalSaveLoader.SaveUserCloudData();

            for (int i = 0; i < (int)SkillKey.End; i++)
            {
                Player.Skill.SkillActivate?.Invoke((SkillKey)i, false);
            }

            for (int i = 0; i < (int)PetSkillKey.End; i++)
            {
                Player.Pet.SkillActivate?.Invoke((PetSkillKey)i, false);
            }
            ExitAndGotoMain().Forget();
        }

        int GetRewardRuneIndex(RuneDungeonRewardTable tableData)
        {
            float randomValue = UnityEngine.Random.Range(0.0f, 100.0f);
            float calculValue = 0;
            int index = 0;
            for(int i=0; i< tableData.rateData.Length; i++)
            {
                calculValue += tableData.rateData[i];
                if(randomValue<=calculValue)
                {
                    break;
                }
                else
                {
                    index++;
                }
            }

            return index;
        }

        private void RewardObtain(RewardTypes rewardType,double amount)
        {
            switch (rewardType)
            {
                case RewardTypes.Coin:
                    Player.ControllerGood.Earn(GoodsKey.Coin, amount);
                    break;
                case RewardTypes.Dia:
                    Player.ControllerGood.Earn(GoodsKey.Dia, amount);
                    break;
                case RewardTypes.StatusPoint:
                    Player.ControllerGood.Earn(GoodsKey.StatusPoint, amount);
                    break;
                case RewardTypes.AwakeStone:
                    Player.ControllerGood.Earn(GoodsKey.AwakeStone, amount);
                    break;
                case RewardTypes.ResearchPotion:
                    Player.ControllerGood.Earn(GoodsKey.ResearchPotion, amount);
                    break;
                case RewardTypes.skillAwakeStone:
                    Player.ControllerGood.Earn(GoodsKey.SkillAwakeStone, amount);
                    break;
                case RewardTypes.Exp:
                    Player.Level.ExpUpAndLvUp(amount);
                    break;
                default:
                    break;
            }
        }


        private void BacktoDungeon()
        {
            bool isPause = false;
            if (Battle.Field.currentSceneState == eSceneState.RPDungeonPause)
            {
                isPause = true;
                Battle.Field.ChangeSceneState(eSceneState.RPDungeon);
            }
            if (Battle.Field.currentSceneState == eSceneState.EXPDungeonPause)
            {
                isPause = true;
                Battle.Field.ChangeSceneState(eSceneState.EXPDungeon);
            }
            if (Battle.Field.currentSceneState == eSceneState.AwakeDungeonPause)
            {
                isPause = true;
                Battle.Field.ChangeSceneState(eSceneState.AwakeDungeon);
            }
            if (Battle.Field.currentSceneState == eSceneState.RiftDungeonPause)
            {
                isPause = true;
                Battle.Field.ChangeSceneState(eSceneState.RiftDungeon);
            }
            if (Battle.Field.currentSceneState == eSceneState.RuneDungeon)
            {
                isPause = true;
                Battle.Field.ChangeSceneState(eSceneState.RuneDungeonPause);
            }
            if (isPause)
            {
                closing = false;
                _viewDungeonIngame.outWindow.SetActive(false);
            }
        }
        private void OpenOutDungeonWindow()
        {
            bool isPause = false;
            if (Battle.Field.currentSceneState == eSceneState.RPDungeon)
            {
                isPause = true;
                Battle.Field.ChangeSceneState(eSceneState.RPDungeonPause);
            }
            if (Battle.Field.currentSceneState == eSceneState.EXPDungeon)
            {
                isPause = true;
                Battle.Field.ChangeSceneState(eSceneState.EXPDungeonPause);
            }
            if (Battle.Field.currentSceneState == eSceneState.AwakeDungeon)
            {
                isPause = true;
                Battle.Field.ChangeSceneState(eSceneState.AwakeDungeonPause);
            }
            if (Battle.Field.currentSceneState == eSceneState.RiftDungeon)
            {
                isPause = true;
                Battle.Field.ChangeSceneState(eSceneState.RiftDungeonPause);
            }
            if (Battle.Field.currentSceneState == eSceneState.RuneDungeon)
            {
                isPause = true;
                Battle.Field.ChangeSceneState(eSceneState.RuneDungeonPause);
            }
            if (isPause)
            {
                closing = false;
                _viewDungeonIngame.outWindow.SetActive(true);
            }
        }
        
        bool closing = false;
        async UniTaskVoid ExitAndGotoMain()
        {
            float currentTime = 0;
            float maxtime = 2.0f;

            while (currentTime < maxtime)
            {
                currentTime += Time.deltaTime;
                await UniTask.Yield(_cts.Token);
            }

            TouchConfirmInExitWindow();
        }

        void TouchConfirmInExitWindow()
        {
            if (closing)
                return;
            closing = true;
            switch (Battle.Dungeon.currentDungeonType)
            {
                case DungeonType.Research:
                    Battle.Field.ChangeSceneState(eSceneState.RPDungeonEnd);
                    break;
                case DungeonType.Exp:
                    Battle.Field.ChangeSceneState(eSceneState.EXPDungeonEnd);
                    break;
                case DungeonType.Awake:
                    Battle.Field.ChangeSceneState(eSceneState.AwakeDungeonEnd);
                    break;
                case DungeonType.Rift:
                    Battle.Field.ChangeSceneState(eSceneState.RiftDungeonEnd);
                    break;
                case DungeonType.Rune:
                    Battle.Field.ChangeSceneState(eSceneState.RuneDungeonEnd);
                    break;
                case DungeonType.End:
                    break;
                default:
                    break;
            }
        }

     

        void KillEnemyUpdate(KillDungeonEnemy msg)
        {
            Battle.Dungeon.currentDungeonData[(int)Battle.Dungeon.currentDungeonType].currentKillEnemy++;
            if(Battle.Dungeon.currentDungeonType==DungeonType.Exp)
            {
                _viewDungeonIngame.killenemyCount.text = string.Format("{0}", Battle.Dungeon.currentDungeonData[(int)Battle.Dungeon.currentDungeonType].currentKillEnemy);
            }
        }
        void ExpDungeonLevelUpdate(DungeonLevelUp msg)
        {
            Battle.Dungeon.currentDungeonData[(int)Battle.Dungeon.currentDungeonType].currentDungeonLevel = msg.currentLevel;
        }

        void DungeonWaveUpdate(DungeonWaveUp msg)
        {
            if(msg.dungeonType==DungeonType.Rift)
            {
                float sliderValue= (float)((float)(Battle.Dungeon.currentDungeonData[(int)DungeonType.Rift].currentWave) / (float)Battle.Dungeon.riftDungeonMaxWave);
                _viewDungeonIngame.dungeonWaveSlider_Rift.DOValue(sliderValue,0.3f);
            }
        }
    }

}
