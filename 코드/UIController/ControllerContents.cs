using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using BlackTree.Definition;
using System;


namespace BlackTree.Core
{
    public class ControllerContents
    {
        private ViewCanvasContents _viewContents;
        private CancellationTokenSource _cts;

        public const int _index = 6;

        eSceneState currentSelectScene;

        List<ViewGoodRewardSlot> slotList=new List<ViewGoodRewardSlot>();
        List<ViewGoodRewardSlot> slotList_forPet = new List<ViewGoodRewardSlot>();

        List<ViewGoodRewardSlot> slotListInPerishWindow = new List<ViewGoodRewardSlot>();

        public ControllerContents(Transform parent,CancellationTokenSource cts)
        {
            _viewContents = ViewCanvas.Create<ViewCanvasContents>(parent);
            _cts = cts;

            _viewContents.EnterRPDungeon.onClick.AddListener(() => {

                int unlockLv = StaticData.Wrapper.ingameLockData[(int)LockedUIType.UnlockRPDungeon].unLockLevel;
                if (Player.Quest.mainQuestCurrentId < unlockLv)
                {
                    LocalizeDescKeys localizedKey = LocalizeDescKeys.Etc_UnlockAfterMission;
                    int levelValue = Model.Player.Option.ContentUIUnlockLevel(LockedUIType.UnlockRPDungeon);
                    string localizedvalue = string.Format(StaticData.Wrapper.localizeddesclist[(int)localizedKey].StringToLocal, levelValue);
                    ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                }
                else
                {
                    currentSelectScene = eSceneState.WaitForRPDungeon;
                    Battle.Dungeon.currentDungeonData[(int)DungeonType.Research].currentDungeonLevel =
                        Math.Clamp(Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Research].bestLevel + 1, 0, StaticData.Wrapper.rpDungeonRewardTableDatas.Length - 1);

                    Player.Guide.StartTutorial(TutorialType.RPDungeonClear);
                    Player.Guide.QuestGuideProgress(QuestGuideType.RPDungeonClear);
                    PopupDetail();

                    _viewContents.detailBlackBG.gameObject.SetActive(true);
                    _viewContents.detailBlackBG.PopupOpenColorFade();
                    _viewContents.totalDetailWindow.CommonPopupOpenAnimationUp();
                }
            });
            _viewContents.EnterEXPDungeon.onClick.AddListener(() => {
                int unlockLv = StaticData.Wrapper.ingameLockData[(int)LockedUIType.UnlockExpDungeon].unLockLevel;
                if (Player.Quest.mainQuestCurrentId < unlockLv)
                {
                    LocalizeDescKeys localizedKey = LocalizeDescKeys.Etc_UnlockAfterMission;
                    int levelValue = Model.Player.Option.ContentUIUnlockLevel(LockedUIType.UnlockExpDungeon);
                    string localizedvalue = string.Format(StaticData.Wrapper.localizeddesclist[(int)localizedKey].StringToLocal, levelValue);
                    ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                }
                else
                {
                    currentSelectScene = eSceneState.WaitForEXPDungeon;

                    Battle.Dungeon.currentDungeonData[(int)DungeonType.Exp].currentDungeonLevel =
                    Math.Clamp(Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Exp].bestLevel + 1, 0, StaticData.Wrapper.chapterRewardTableDatas.Length - 1);

                    Player.Guide.StartTutorial(TutorialType.ExpDungeonClear);
                    Player.Guide.QuestGuideProgress(QuestGuideType.ExpDungeonClear);

                    PopupDetail();

                    _viewContents.detailBlackBG.gameObject.SetActive(true);
                    _viewContents.detailBlackBG.PopupOpenColorFade();
                    _viewContents.totalDetailWindow.CommonPopupOpenAnimationUp();
                }
     
            });
            _viewContents.EnterAwakeDungeon.onClick.AddListener(() => {
                int unlockLv = StaticData.Wrapper.ingameLockData[(int)LockedUIType.UnlockAwakeDungeon].unLockLevel;
                if (Player.Quest.mainQuestCurrentId < unlockLv)
                {
                    LocalizeDescKeys localizedKey = LocalizeDescKeys.Etc_UnlockAfterMission;
                    int levelValue = Model.Player.Option.ContentUIUnlockLevel(LockedUIType.UnlockAwakeDungeon);
                    string localizedvalue = string.Format(StaticData.Wrapper.localizeddesclist[(int)localizedKey].StringToLocal, levelValue);
                    ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                }
                else
                {
                    currentSelectScene = eSceneState.WaitForAwakeDungeon;

                    Player.Guide.StartTutorial(TutorialType.AwakeDungeonClear);

                    PopupDetail();

                    _viewContents.detailBlackBG.gameObject.SetActive(true);
                    _viewContents.detailBlackBG.PopupOpenColorFade();
                    _viewContents.totalDetailWindow.CommonPopupOpenAnimationUp();
                }
              
            });
            _viewContents.EnterRiftDungeon.onClick.AddListener(() => {
                int unlockLv = StaticData.Wrapper.ingameLockData[(int)LockedUIType.UnlockRiftDungeon].unLockLevel;
                if (Player.Cloud.userLevelData.currentLevel < unlockLv)
                {
                    LocalizeDescKeys localizedKey = LocalizeDescKeys.Etc_UnlockAfterLevel;
                    int levelValue = Model.Player.Option.ContentUIUnlockLevel(LockedUIType.UnlockRiftDungeon);
                    string localizedvalue = string.Format(StaticData.Wrapper.localizeddesclist[(int)localizedKey].StringToLocal, levelValue);
                    ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                }
                else
                {
                    currentSelectScene = eSceneState.WaitForRiftDungeon;

                    Battle.Dungeon.currentDungeonData[(int)DungeonType.Rift].currentDungeonLevel =
                    Math.Clamp(Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Rift].bestLevel + 1, 0, StaticData.Wrapper.riftDungeonRewardTableDatas.Length - 1);

                    PopupDetail();

                    _viewContents.detailBlackBG.gameObject.SetActive(true);
                    _viewContents.detailBlackBG.PopupOpenColorFade();
                    _viewContents.totalDetailWindow.CommonPopupOpenAnimationUp();
                }
              
            });


            _viewContents.nextLevel_RP.onClick.AddListener(()=> {
               
                Battle.Dungeon.currentDungeonData[(int)DungeonType.Research].currentDungeonLevel++;
                if (Battle.Dungeon.currentDungeonData[(int)DungeonType.Research].currentDungeonLevel >= Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Research].bestLevel + 6)
                {
#if UNITY_EDITOR
                    //Battle.Dungeon.currentDungeonData[(int)DungeonType.Research].currentDungeonLevel = Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Research].bestLevel + 2;
#else
       //   Battle.Dungeon.currentDungeonData[(int)DungeonType.Research].currentDungeonLevel = Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Research].bestLevel  +2;
#endif

                }
                if (Battle.Dungeon.currentDungeonData[(int)DungeonType.Research].currentDungeonLevel>= StaticData.Wrapper.rpDungeonRewardTableDatas.Length)
                {
                    Battle.Dungeon.currentDungeonData[(int)DungeonType.Research].currentDungeonLevel = StaticData.Wrapper.rpDungeonRewardTableDatas.Length - 1;
                }
                PopupDetail();
            });
            _viewContents.prevLevel_RP.onClick.AddListener(() => {
                Battle.Dungeon.currentDungeonData[(int)DungeonType.Research].currentDungeonLevel--;
                if (Battle.Dungeon.currentDungeonData[(int)DungeonType.Research].currentDungeonLevel <0)
                {
                    Battle.Dungeon.currentDungeonData[(int)DungeonType.Research].currentDungeonLevel = 0;
                }
                PopupDetail();
            });

            _viewContents.nextLevel_exp.onClick.AddListener(() => {
                Battle.Dungeon.currentDungeonData[(int)DungeonType.Exp].currentDungeonLevel++;
                if (Battle.Dungeon.currentDungeonData[(int)DungeonType.Exp].currentDungeonLevel >= Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Exp].bestLevel +6)
                {
                   // Battle.Dungeon.currentDungeonData[(int)DungeonType.Exp].currentDungeonLevel = Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Exp].bestLevel +2;
                }
                if (Battle.Dungeon.currentDungeonData[(int)DungeonType.Exp].currentDungeonLevel >= StaticData.Wrapper.chapterRewardTableDatas.Length)
                {
                    Battle.Dungeon.currentDungeonData[(int)DungeonType.Exp].currentDungeonLevel = StaticData.Wrapper.chapterRewardTableDatas.Length - 1;
                }
              
                PopupDetail();
            });
            _viewContents.prevLevel_exp.onClick.AddListener(() => {
                Battle.Dungeon.currentDungeonData[(int)DungeonType.Exp].currentDungeonLevel--;
                if (Battle.Dungeon.currentDungeonData[(int)DungeonType.Exp].currentDungeonLevel < 0)
                {
                    Battle.Dungeon.currentDungeonData[(int)DungeonType.Exp].currentDungeonLevel = 0;
                }
                PopupDetail();
            });


            _viewContents.nextLevel_rift.onClick.AddListener(() => {
                Battle.Dungeon.currentDungeonData[(int)DungeonType.Rift].currentDungeonLevel++;
                if (Battle.Dungeon.currentDungeonData[(int)DungeonType.Rift].currentDungeonLevel >= Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Rift].bestLevel + 2)
                {
                    //Battle.Dungeon.currentDungeonData[(int)DungeonType.Rift].currentDungeonLevel = Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Rift].bestLevel + 2;
                }
                if (Battle.Dungeon.currentDungeonData[(int)DungeonType.Rift].currentDungeonLevel >= StaticData.Wrapper.riftDungeonRewardTableDatas.Length)
                {
                    Battle.Dungeon.currentDungeonData[(int)DungeonType.Rift].currentDungeonLevel = StaticData.Wrapper.riftDungeonRewardTableDatas.Length - 1;
                }
                PopupDetail();
            });
            _viewContents.prevLevel_rift.onClick.AddListener(() => {
                Battle.Dungeon.currentDungeonData[(int)DungeonType.Rift].currentDungeonLevel--;
                if (Battle.Dungeon.currentDungeonData[(int)DungeonType.Rift].currentDungeonLevel < 0)
                {
                    Battle.Dungeon.currentDungeonData[(int)DungeonType.Rift].currentDungeonLevel = 0;
                }
                PopupDetail();
            });


            _viewContents.enterRPDungeon.onClick.AddListener(() => {
                ChangeSceneToDungeon();
            });
            _viewContents.perishRPDungeon.onClick.AddListener(() => {
                RPDungeonPerish();
            });

            _viewContents.enterexpDungeon.onClick.AddListener(ChangeSceneToDungeon);
            _viewContents.perishexpDungeon.onClick.AddListener(ExpDungeonPerish);

            _viewContents.EnterDungeon_awake.onClick.AddListener(() => {
                ChangeSceneToDungeon();
            });
            _viewContents.PerishDungeon_awake.onClick.AddListener(() => {
                AwakeDungeonPerish();
            });

            _viewContents.enterRiftDungeon.onClick.AddListener(ChangeSceneToDungeon);
            _viewContents.perishRiftDungeon.onClick.AddListener(RiftDungeonPerish);

            _viewContents.enterRuneDungeon.onClick.AddListener(ChangeSceneToDungeon);

            _viewContents.detailCloseBtn.onClick.AddListener(() => {
                _viewContents.detailBlackBG.PopupCloseColorFade();
                _viewContents.totalDetailWindow.CommonPopupCloseAnimationDown(()=> {
                    _viewContents.expDetailWindow.SetActive(false);
                    _viewContents.RPDetailWindow.SetActive(false);
                    _viewContents.awakedetailWindow.SetActive(false);
                    _viewContents.riftDetailWindow.SetActive(false);
                    _viewContents.detailBlackBG.gameObject.SetActive(false);
                    _viewContents.runeDetailWindow.SetActive(false);
                });
            });

          

            for (int i=0; i< _viewContents.perishWindowCloseBtn.Length; i++)
            {
                _viewContents.perishWindowCloseBtn[i].onClick.AddListener(()=> {
                    _viewContents.perishWindow.SetActive(false);
                });
            }

            MainNav.onChange += UpdateViewVisible;

            for (int i = 0; i < _viewContents.closeBtn.Length; i++)
            {
                int index = i;
                _viewContents.closeBtn[index].onClick.AddListener(() => MainNav.CloseMainUIWindow());
            }

            _viewContents.RPdungeonTicketObtain.Init();
            _viewContents.expdungeonTicketObtain.Init();
            _viewContents.awakeObtain.Init();
            _viewContents.riftObtain.Init();
            _viewContents.runeObtain.Init();

            Player.ControllerGood.BindOnChange(GoodsKey.ExpDungeonTicket, () =>
            {
                UpdateView(GoodsKey.ExpDungeonTicket);
            });
            Player.ControllerGood.BindOnChange(GoodsKey.RPDungeonTicket, () =>
            {
                UpdateView(GoodsKey.RPDungeonTicket);
            });
            Player.ControllerGood.BindOnChange(GoodsKey.AwakeDungeonTicket, () =>
            {
                UpdateView(GoodsKey.AwakeDungeonTicket);
            });
            Player.ControllerGood.BindOnChange(GoodsKey.RiftDungeonTicket, () =>
            {
                UpdateView(GoodsKey.RiftDungeonTicket);
            });
            Player.ControllerGood.BindOnChange(GoodsKey.RuneDungeonTicket, () =>
            {
                UpdateView(GoodsKey.RuneDungeonTicket);
            });
            UpdateView(GoodsKey.RPDungeonTicket);
            UpdateView(GoodsKey.ExpDungeonTicket);
            UpdateView(GoodsKey.AwakeDungeonTicket);
            UpdateView(GoodsKey.RiftDungeonTicket);
            UpdateView(GoodsKey.RuneDungeonTicket);

            _viewContents.detailBlackBG.gameObject.SetActive(false);

            Player.Option.ContentUnlockUpdate += LockUpdate;
            LockUpdate();

            for (int i = 0; i < _viewContents.goodsBtn.Length; i++)
            {
                _viewContents.goodsBtn[i].onClick.AddListener(() => {
                    Bundles.ViewCanvas.Get<Bundles.ViewCanvasInventory>().SetVisible(true);
                    Bundles.ViewCanvas.Get<Bundles.ViewCanvasInventory>().blackBG.PopupOpenColorFade();
                    Bundles.ViewCanvas.Get<Bundles.ViewCanvasInventory>().Wrapped.CommonPopupOpenAnimationDown();
                });
            }


            //rune
            _viewContents.EnterRuneDungeon.onClick.AddListener(() => {
                int unlockLv = StaticData.Wrapper.ingameLockData[(int)LockedUIType.RuneSummonUnlock].unLockLevel;
                if (Player.Cloud.userLevelData.currentLevel < unlockLv)
                {
                    LocalizeDescKeys localizedKey = LocalizeDescKeys.Etc_UnlockAfterLevel;
                    int levelValue = Model.Player.Option.ContentUIUnlockLevel(LockedUIType.RuneSummonUnlock);
                    string localizedvalue = string.Format(StaticData.Wrapper.localizeddesclist[(int)localizedKey].StringToLocal, levelValue);
                    ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                }
                else
                {
                    currentSelectScene = eSceneState.WaitForRuneDungeon;

                    PopupDetail();

                    _viewContents.detailBlackBG.gameObject.SetActive(true);
                    _viewContents.detailBlackBG.PopupOpenColorFade();
                    _viewContents.totalDetailWindow.CommonPopupOpenAnimationUp();
                }
            });

            for(int i=0; i<StaticData.Wrapper.runeDungeonRewardDatas.Length; i++)
            {
                var dungeonreward = StaticData.Wrapper.runeDungeonRewardDatas[i];
                var rewardObj = UnityEngine.Object.Instantiate(_viewContents.chapterRewardSlotPrefab);
                rewardObj.transform.SetParent(_viewContents.rewardslotParentInTableWindow, false);
                
                string temptext= StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_level_random].StringToLocal;
                rewardObj.title.text = string.Format(temptext, dungeonreward.Level, dungeonreward.count);

                for (int j=0; j< dungeonreward.runeRewardTable.Length; j++)
                {
                    var slotobj = UnityEngine.Object.Instantiate(_viewContents.rewardslotPrefab);
                    slotobj.transform.SetParent(rewardObj.rewardSlotParent, false);
                    
                    int grade = StaticData.Wrapper.runedatas[dungeonreward.runeRewardTable[j]].grade;
                    slotobj.goodsFrame.sprite = InGameResourcesBundle.Loaded.weaponGradeInnerFrameSprite[grade-1];
                    slotobj.goodsIcon.sprite = PetResourcesBundle.Loaded.runeImage[dungeonreward.runeRewardTable[j]];
                    slotobj.goodValue.text = "";
                }
            }

            _viewContents.openRewardTableWindowBtn.onClick.AddListener(()=> {
                _viewContents.rewardTableWindow.SetActive(true);
            });

            for(int i=0; i< _viewContents.closeBtnInRewardTable.Length; i++)
            {
                int index = i;
                _viewContents.closeBtnInRewardTable[index].onClick.AddListener(()=> {
                    _viewContents.rewardTableWindow.SetActive(false);

                });
            }


            _viewContents.expTitle.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_ExpDungeon].StringToLocal;
            _viewContents.expDesc.text = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_ExpDunDesc].StringToLocal;

            _viewContents.awakeTitle.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_AwakeDungeon].StringToLocal;
            _viewContents.awakeDesc.text = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_AwakeDunDesc].StringToLocal;

            _viewContents.riftTitle.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_RiftDungeon].StringToLocal;
            _viewContents.riftDesc.text = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_RiftDunDesc].StringToLocal;

            _viewContents.rpTitle.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_RPDungeon].StringToLocal;
            _viewContents.rpDesc.text = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_RpDunDesc].StringToLocal;

            _viewContents.runeTitle.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_RuneDungeon].StringToLocal;
            _viewContents.runeDesc.text = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_RuneDunDesc].StringToLocal;


            _viewContents.rpDungeonDetailTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_RPDungeon].StringToLocal;
            _viewContents.expDungeonDetailTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_ExpDungeon].StringToLocal;
            _viewContents.awakeDungeonDetailTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_AwakeDungeon].StringToLocal;
            _viewContents.riftDungeonDetailTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_RiftDungeon].StringToLocal;
            _viewContents.runeDungeonDetailTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_RuneDungeon].StringToLocal;

             _viewContents.rpClearTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Clear].StringToLocal;
             _viewContents.rpEnterTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Enter].StringToLocal;
             _viewContents.expClearTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Clear].StringToLocal;
             _viewContents.expEnterTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Enter].StringToLocal;
             _viewContents.awakeClearTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Clear].StringToLocal;
             _viewContents.awakeEnterTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Enter].StringToLocal;
             _viewContents.riftClearTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Clear].StringToLocal;
             _viewContents.riftEnterTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Enter].StringToLocal;
             _viewContents.runeEnterTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Enter].StringToLocal;

             _viewContents.expDungeonDescText.text = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_expDungeonDesc].StringToLocal;
             _viewContents.riftDungeonDescText.text = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_riftDungeonDesc].StringToLocal;
             _viewContents.rewardTableBtnTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_RewardTable].StringToLocal;
             _viewContents.runeRewardWindowTitle.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_RewardTable].StringToLocal;
        }

        void LockUpdate()
        {
            int unlockRPLv= StaticData.Wrapper.ingameLockData[(int)LockedUIType.UnlockRPDungeon].unLockLevel;
            int unlockEXPLv = StaticData.Wrapper.ingameLockData[(int)LockedUIType.UnlockExpDungeon].unLockLevel;
            int unlockAWAKELv = StaticData.Wrapper.ingameLockData[(int)LockedUIType.UnlockAwakeDungeon].unLockLevel;
            int unlockRIFTLv = StaticData.Wrapper.ingameLockData[(int)LockedUIType.UnlockRiftDungeon].unLockLevel;
            int unlockRuneLv = StaticData.Wrapper.ingameLockData[(int)LockedUIType.RuneSummonUnlock].unLockLevel;
            if (Player.Quest.mainQuestCurrentId >= unlockRPLv)
            {
                _viewContents.RPLockedObj.SetActive(false);
            }
            else
            {
                _viewContents.RPLockedObj.SetActive(true);
            }

            if (Player.Quest.mainQuestCurrentId >= unlockEXPLv)
            {
                _viewContents.ExpLockedObj.SetActive(false);
            }
            else
            {
                _viewContents.ExpLockedObj.SetActive(true);
            }

            if (Player.Quest.mainQuestCurrentId >= unlockAWAKELv)
            {
                _viewContents.AwakeLockedObj.SetActive(false);
            }
            else
            {
                _viewContents.AwakeLockedObj.SetActive(true);
            }

            if (Player.Cloud.userLevelData.currentLevel >= unlockRIFTLv)
            {
                _viewContents.RiftLockedObj.SetActive(false);
            }
            else
            {
                _viewContents.RiftLockedObj.SetActive(true);
            }

            if (Player.Cloud.userLevelData.currentLevel >= unlockRuneLv)
            {
                _viewContents.runeLockedObj.SetActive(false);
            }
            else
            {
                _viewContents.runeLockedObj.SetActive(true);
            }
        }

        private void UpdateView(GoodsKey key)
        {
            double goodValue=0;
            if (key ==GoodsKey.ExpDungeonTicket)
            {
                goodValue = Player.ControllerGood.GetValue(key);
                _viewContents.expdungeonTicketObtain.SyncUpdate(goodValue);
            }
            if (key == GoodsKey.RPDungeonTicket)
            {
                goodValue = Player.ControllerGood.GetValue(key);
                _viewContents.RPdungeonTicketObtain.SyncUpdate(goodValue);
            }
            if (key == GoodsKey.AwakeDungeonTicket)
            {
                goodValue = Player.ControllerGood.GetValue(key);
                _viewContents.awakeObtain.SyncUpdate(goodValue);
            }
            if (key == GoodsKey.RiftDungeonTicket)
            {
                goodValue = Player.ControllerGood.GetValue(key);
                _viewContents.riftObtain.SyncUpdate(goodValue);
            }

            if (key == GoodsKey.RuneDungeonTicket)
            {
                goodValue = Player.ControllerGood.GetValue(key);
                _viewContents.runeObtain.SyncUpdate(goodValue);
            }
        }

        public void PopupDetail()
        {
            ViewGoodRewardSlot slotobj = null;
            int levelIndex = 0;
            for (int i = 0; i < slotList.Count; i++)
            {
                slotList[i].gameObject.SetActive(false);
            }
            string tempGradeTxt = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_XGrade].StringToLocal;
            switch (currentSelectScene)
            {
                case eSceneState.WaitForRPDungeon:
                    _viewContents.RPDetailWindow.SetActive(true);
                    if (Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Research].bestLevel >= 0)
                    {
                        string temptext_best = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_bestAchive].StringToLocal;
                        _viewContents.bestLevel_RP.text = string.Format(temptext_best,
                        Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Research].bestLevel + 1);
                    }
                    else
                    {
                        _viewContents.bestLevel_RP.text = "";
                    }
                    _viewContents.currentLevel_RP.text = string.Format(tempGradeTxt, Battle.Dungeon.currentDungeonData[(int)DungeonType.Research].currentDungeonLevel+1);


                    levelIndex = Battle.Dungeon.currentDungeonData[(int)DungeonType.Research].currentDungeonLevel;
                    var rewardTableData_Research = StaticData.Wrapper.rpDungeonRewardTableDatas[levelIndex];

                    bool cantEnterResearch = Battle.Dungeon.currentDungeonData[(int)DungeonType.Research].currentDungeonLevel >= Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Research].bestLevel + 6;
                    _viewContents.enterRPDungeon.enabled = !cantEnterResearch;
                    _viewContents.cantEnterRPDungeon.SetActive(cantEnterResearch);

                    bool canPerishResearch = Battle.Dungeon.currentDungeonData[(int)DungeonType.Research].currentDungeonLevel <= Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Research].bestLevel;
                    _viewContents.perishRPDungeon.enabled = canPerishResearch;
                    _viewContents.cantPerishRPDungeon.SetActive(!canPerishResearch);

                    for(int i=0; i< rewardTableData_Research.rewardAmount.Length; i++)
                    {
                        if (slotList.Count <= i)
                        {
                            slotobj = UnityEngine.Object.Instantiate(_viewContents.rewardSlotPrefab);
                            slotList.Add(slotobj);
                            slotobj.gameObject.SetActive(true);
                        }
                        else
                        {
                            slotobj = slotList[i];
                            slotobj.gameObject.SetActive(true);
                        }

                        double perishRewardAmount = rewardTableData_Research.rewardAmount[i];

                        if (canPerishResearch)
                        {
                            perishRewardAmount = rewardTableData_Research.rewardAmount[i];
                        }

                        slotobj.transform.SetParent(_viewContents.rewardslotParent_RP, false);
                        slotobj.goodsIcon.sprite = GoodResourcesBundle.Loaded.rewardSprites[(int)rewardTableData_Research.rewardType[i]];
                        slotobj.goodValue.text = perishRewardAmount.ToNumberString();
                        slotobj.goodsDesc.text = "";
                    }
                    break;
                case eSceneState.WaitForEXPDungeon:
                    _viewContents.expDetailWindow.SetActive(true);

                    if (Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Exp].bestLevel>= 0)
                    {
                        string temptext_exp = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_bestAchive_exp].StringToLocal;
                        _viewContents.bestObtainExp.text = string.Format(temptext_exp,
                       Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Exp].bestLevel + 1,
                       ((Battle.Field.GetRewardExpDungeonExpForEnemy(Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Exp].bestLevel) * Battle.Dungeon.DungeonRewardRate)* Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Exp].bestKillCount).ToNumberString()
                      );
                    }
                    else
                    {
                        _viewContents.bestObtainExp.text = "";
                    }

                    
                    _viewContents.currentLevel_exp.text = string.Format(tempGradeTxt, Battle.Dungeon.currentDungeonData[(int)DungeonType.Exp].currentDungeonLevel+1);

                    if (slotList.Count < 1)
                    {
                        slotobj = UnityEngine.Object.Instantiate(_viewContents.rewardSlotPrefab);
                        slotList.Add(slotobj);
                        slotobj.gameObject.SetActive(true);
                    }
                    else
                    {
                        slotobj = slotList[0];
                        slotobj.gameObject.SetActive(true);
                    }
                    slotobj.transform.SetParent(_viewContents.rewardslotParent_exp, false);
                    slotobj.goodsIcon.sprite = GoodResourcesBundle.Loaded.rewardSprites[(int)RewardTypes.Exp];

                    string temptext = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_MonsterCount].StringToLocal;
                    slotobj.goodValue.text = string.Format(temptext, (Battle.Field.GetRewardExpDungeonExpForEnemy(Battle.Dungeon.currentDungeonData[(int)DungeonType.Exp].currentDungeonLevel) * Battle.Dungeon.DungeonRewardRate).ToNumberString());
                    slotobj.goodsDesc.text = "EXP";

                    bool cantEnterExp = Battle.Dungeon.currentDungeonData[(int)DungeonType.Exp].currentDungeonLevel >= Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Exp].bestLevel + 6;
                    _viewContents.enterexpDungeon.enabled = !cantEnterExp;
                    _viewContents.cantEnterExpDungeon.SetActive(cantEnterExp);

                    bool canPerishExp = Battle.Dungeon.currentDungeonData[(int)DungeonType.Exp].currentDungeonLevel <= Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Exp].bestLevel;
                    _viewContents.perishexpDungeon.enabled = canPerishExp;
                    _viewContents.cantPerishExpDungeon.SetActive(!canPerishExp);
                    break;
                case eSceneState.WaitForAwakeDungeon:
                    _viewContents.awakedetailWindow.SetActive(true);
                    string temptext_awake = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_bestAchive_awake].StringToLocal;
                    _viewContents.bestLeveldesc_awake.text = string.Format(temptext_awake, 
                        Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Awake].bestDamage.ToNumberString(),
                        (Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Awake].bestDamage/10.0f).ToNumberString());

                    if (slotList.Count <= 1)
                    {
                        slotobj = UnityEngine.Object.Instantiate(_viewContents.rewardSlotPrefab);
                        slotList.Add(slotobj);
                        slotobj.gameObject.SetActive(true);
                    }
                    else
                    {
                        slotobj = slotList[0];
                        slotobj.gameObject.SetActive(true);
                    }
                    slotobj.transform.SetParent(_viewContents.rewardslotParent_awake, false);
                    slotobj.goodsIcon.sprite = GoodResourcesBundle.Loaded.rewardSprites[(int)RewardTypes.AwakeStone];
                    slotobj.goodValue.text = "";
                    slotobj.goodsDesc.text = "";
                    
                    bool cantEnterAwake = Battle.Dungeon.currentDungeonData[(int)DungeonType.Awake].currentDungeonLevel == Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Awake].bestLevel + 2;
                    _viewContents.EnterDungeon_awake.enabled = !cantEnterAwake;
                    _viewContents.cantEnterAwakeDungeon.SetActive(cantEnterAwake);

                    bool canPerishAwake = Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Awake].bestDamage >0;
                    _viewContents.PerishDungeon_awake.enabled = canPerishAwake;
                    _viewContents.cantPerishAwakeDungeon.SetActive(!canPerishAwake);

                    break;
                case eSceneState.WaitForRiftDungeon:
                    _viewContents.riftDetailWindow.SetActive(true);
                    if(Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Rift].bestLevel>=0)
                    {
                        string temptext_bestgrade= StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_bestAchive].StringToLocal;
                        _viewContents.bestLevel_rift.text = string.Format(temptext_bestgrade,
                       Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Rift].bestLevel + 1);
                    }
                    else
                    {
                        _viewContents.bestLevel_rift.text = "";
                    }
                    _viewContents.currentLevel_rift.text = string.Format(tempGradeTxt, Battle.Dungeon.currentDungeonData[(int)DungeonType.Rift].currentDungeonLevel+1);

                    levelIndex = Battle.Dungeon.currentDungeonData[(int)DungeonType.Rift].currentDungeonLevel;
                    var rewardTableData_rift = StaticData.Wrapper.riftDungeonRewardTableDatas[levelIndex];

                    bool cantEnterRift = Battle.Dungeon.currentDungeonData[(int)DungeonType.Rift].currentDungeonLevel >= Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Rift].bestLevel + 2;
                    _viewContents.enterRiftDungeon.enabled = !cantEnterRift;
                    _viewContents.cantEnterRiftDungeon.SetActive(cantEnterRift);

                    bool canPerishrift = Battle.Dungeon.currentDungeonData[(int)DungeonType.Rift].currentDungeonLevel <= Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Rift].bestLevel;
                    _viewContents.perishRiftDungeon.enabled = canPerishrift;
                    _viewContents.cantPerishRiftDungeon.SetActive(!canPerishrift);

                    for (int i = 0; i < rewardTableData_rift.rewardAmount.Length; i++)
                    {
                        if (slotList.Count <= i)
                        {
                            slotobj = UnityEngine.Object.Instantiate(_viewContents.rewardSlotPrefab);
                            slotList.Add(slotobj);
                            slotobj.gameObject.SetActive(true);
                        }
                        else
                        {
                            slotobj = slotList[i];
                            slotobj.gameObject.SetActive(true);
                        }

                        double perishRewardAmount = rewardTableData_rift.rewardAmount[i];

                        if (canPerishrift)
                        {
                            perishRewardAmount = rewardTableData_rift.rewardAmount[i] / 2.0f;
                        }

                        slotobj.transform.SetParent(_viewContents.rewardslotParent_rift, false);
                        slotobj.goodsIcon.sprite = GoodResourcesBundle.Loaded.rewardSprites[(int)rewardTableData_rift.rewardType[i]];
                        slotobj.goodValue.text = perishRewardAmount.ToNumberString();
                        slotobj.goodsDesc.text = "";
                    }
                    break;
                case eSceneState.WaitForRuneDungeon:
                    _viewContents.runeDetailWindow.SetActive(true);
                    if (Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Rune].bestLevel >= 0)
                    {
                        string temptext_bestlevel = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_bestAchive].StringToLocal;
                        _viewContents.bestLevel_rune.text = string.Format(temptext_bestlevel,
                       Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Rune].bestLevel + 1);
                    }
                    else
                    {
                        string notclear = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_NotClear].StringToLocal;
                        _viewContents.bestLevel_rune.text = notclear;
                    }

                    if (Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Rune].bestLevel >= 0)
                    {
                        string recent = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_recentReward].StringToLocal;
                        _viewContents.recentRewardText.text = recent;
                    }
                    else
                    {
                        _viewContents.recentRewardText.text = "";
                    }

                    if (Player.Cloud.Dungeondata.runeDungeonRewardHistory.Count>0)
                    {
                        for(int i=0; i< Player.Cloud.Dungeondata.runeDungeonRewardHistory.Count; i++)
                        {
                            if (slotList.Count <= i)
                            {
                                slotobj = UnityEngine.Object.Instantiate(_viewContents.rewardSlotPrefab);
                                slotList.Add(slotobj);
                                slotobj.gameObject.SetActive(true);
                            }
                            else
                            {
                                slotobj = slotList[i];
                                slotobj.gameObject.SetActive(true);
                            }
                            slotobj.transform.SetParent(_viewContents.runeslotParent_rune, false);

                            int index = Player.Cloud.Dungeondata.runeDungeonRewardHistory[i];
                            var runedata = StaticData.Wrapper.runedatas[index];
                            slotobj.goodsFrame.sprite = InGameResourcesBundle.Loaded.weaponGradeInnerFrameSprite[runedata.grade-1];
                            slotobj.goodsIcon.sprite = PetResourcesBundle.Loaded.runeImage[runedata.index];
                            slotobj.goodValue.text = "";
                            slotobj.goodsDesc.text = "";
                        }
                    }
                    break;
                default:
                    break;
            }
      
        }

        public void ChangeSceneToDungeon()
        {
            if (Battle.Field.IsCantChangeScene)
            {
                //message
                string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_CanPlayForSomeReason].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                return;
            }
            bool canEnter = false;
            switch (currentSelectScene)
            {
                case eSceneState.WaitForRPDungeon:
                    if(Player.ControllerGood.GetValue(GoodsKey.RPDungeonTicket) >= 1)
                    {
                        canEnter = true;
                    }
                    break;
                case eSceneState.WaitForEXPDungeon:
                    if (Player.ControllerGood.GetValue(GoodsKey.ExpDungeonTicket) >= 1)
                    {
                        canEnter = true;
                    }
                    break;
                case eSceneState.WaitForAwakeDungeon:
                    if (Player.ControllerGood.GetValue(GoodsKey.AwakeDungeonTicket) >= 1)
                    {
                        canEnter = true;
                    }
                    break;
                case eSceneState.WaitForRiftDungeon:
                    if (Player.ControllerGood.GetValue(GoodsKey.RiftDungeonTicket) >= 1)
                    {
                        canEnter = true;
                    }
                    break;
                case eSceneState.WaitForRuneDungeon:
                    if (Player.ControllerGood.GetValue(GoodsKey.RuneDungeonTicket) >= 1)
                    {
                        canEnter = true;
                    }
                    break;
                default:
                    break;
            }
            if(canEnter)
            {
                Battle.Field.ChangeSceneState(currentSelectScene);
                MainNav.CloseMainUIWindow();
                Player.Skill.SkillCoolTimeInit();
                for(int i=0; i<(int)SkillKey.End; i++)
                {
                    Player.Skill.SkillActivate?.Invoke((SkillKey)i,false);
                }

                for (int i = 0; i < (int)PetSkillKey.End; i++)
                {
                    Player.Pet.SkillActivate?.Invoke((PetSkillKey)i, false);
                }
        

                _viewContents.detailBlackBG.PopupCloseColorFade();
                _viewContents.totalDetailWindow.CommonPopupCloseAnimationDown(() => {
                    _viewContents.expDetailWindow.SetActive(false);
                    _viewContents.RPDetailWindow.SetActive(false);
                    _viewContents.awakedetailWindow.SetActive(false);
                    _viewContents.riftDetailWindow.SetActive(false);
                    _viewContents.detailBlackBG.gameObject.SetActive(false);
                });

                Player.Unit.ResetUnitWhenGoDungeon();

                
            }
            else
            {
                string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_DungeonKeyNotEnough].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                return;
            }
        }
        private void UpdateViewVisible()
        {
            if (MainNav.SelectTabIndex == _index)
            {
                if (_viewContents.IsVisible)
                {
                    _viewContents.blackBG.PopupCloseColorFade();
                    _viewContents.Wrapped.CommonPopupCloseAnimationDown(() => {
                        _viewContents.SetVisible(false);
                    });
                }
                else
                {
                    _viewContents.SetVisible(true);

                    _viewContents.RPDetailWindow.SetActive(false);
                    _viewContents.awakedetailWindow.SetActive(false);
                    _viewContents.riftDetailWindow.SetActive(false);
                    _viewContents.expDetailWindow.SetActive(false);

                    _viewContents.blackBG.PopupOpenColorFade();
                    _viewContents.Wrapped.CommonPopupOpenAnimationUp(() => {
                        if(Player.Guide.currentTutorial==TutorialType.AwakeDungeonClear|| Player.Guide.currentTutorial==TutorialType.RPDungeonClear ||
                        Player.Guide.currentTutorial==TutorialType.ExpDungeonClear)
                        {
                            Player.Guide.StartTutorial(Player.Guide.currentTutorial);
                        }

                        if (Player.Guide.currentGuideQuest == QuestGuideType.RPDungeonClear || Player.Guide.currentGuideQuest == QuestGuideType.ExpDungeonClear)
                        {
                            Player.Guide.QuestGuideProgress(Player.Guide.currentGuideQuest);
                        }
                    });
                }
            }
            else
            {
                _viewContents.blackBG.PopupCloseColorFade();
                _viewContents.Wrapped.CommonPopupCloseAnimationDown(() => {
                    _viewContents.SetVisible(false);
                });
            }

        }

        private void RPDungeonPerish()
        {
            if (Player.ControllerGood.GetValue(GoodsKey.RPDungeonTicket) < 1)
            {
                string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_DungeonKeyNotEnough].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                return;
            }
            _viewContents.perishWindow.SetActive(true);
            var bestlevel = Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Research].bestLevel;
            var rewardTableData_rp = StaticData.Wrapper.rpDungeonRewardTableDatas[bestlevel];

            ViewGoodRewardSlot slotobj = null;
            for (int i = 0; i < slotListInPerishWindow.Count; i++)
            {
                slotListInPerishWindow[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < rewardTableData_rp.rewardAmount.Length; i++)
            {
                if (slotListInPerishWindow.Count <= i)
                {
                    slotobj = UnityEngine.Object.Instantiate(_viewContents.rewardSlotPrefab);
                    slotListInPerishWindow.Add(slotobj);
                    slotobj.gameObject.SetActive(true);
                }
                else
                {
                    slotobj = slotListInPerishWindow[i];
                    slotobj.gameObject.SetActive(true);
                }

                double perishRewardAmount = rewardTableData_rp.rewardAmount[i];

                slotobj.transform.SetParent(_viewContents.rewardSlotParentinPerish, false);
                slotobj.goodsIcon.sprite = GoodResourcesBundle.Loaded.rewardSprites[(int)rewardTableData_rp.rewardType[i]];
                slotobj.goodValue.text = perishRewardAmount.ToNumberString();
                slotobj.goodsDesc.text = "";

                RewardObtain((rewardTableData_rp.rewardType[i]), rewardTableData_rp.rewardAmount[i]);
            }
            string tempTxt = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_rpClear].StringToLocal;
            _viewContents.perishDesc.text = string.Format(tempTxt, bestlevel + 1);

            Player.ControllerGood.Consume(Definition.GoodsKey.RPDungeonTicket, 1);
            Player.Quest.TryCountUp(QuestType.RPDungeonClear, 1);
            Player.Quest.TryCountUp(QuestType.DungeonClear, 1);
        }
        private void ExpDungeonPerish()
        {
            if (Player.ControllerGood.GetValue(GoodsKey.ExpDungeonTicket) < 1)
            {
                string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_DungeonKeyNotEnough].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                return;
            }
            _viewContents.perishWindow.SetActive(true);
            var bestlevel = Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Exp].bestLevel;
            ViewGoodRewardSlot slotobj = null;
            for (int i = 0; i < slotListInPerishWindow.Count; i++)
            {
                slotListInPerishWindow[i].gameObject.SetActive(false);
            }
            if (slotListInPerishWindow.Count <= 1)
            {
                slotobj = UnityEngine.Object.Instantiate(_viewContents.rewardSlotPrefab);
                slotListInPerishWindow.Add(slotobj);
                slotobj.gameObject.SetActive(true);
            }
            else
            {
                slotobj = slotListInPerishWindow[0];
                slotobj.gameObject.SetActive(true);
            }
            var expRewardValue = Battle.Field.GetRewardExpDungeonExpForEnemy(bestlevel) * Battle.Dungeon.DungeonRewardRate * Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Exp].bestKillCount;

            slotobj.transform.SetParent(_viewContents.rewardSlotParentinPerish, false);
            slotobj.goodsIcon.sprite = GoodResourcesBundle.Loaded.rewardSprites[(int)RewardTypes.Exp];
            slotobj.goodValue.text = expRewardValue.ToNumberString();
            slotobj.goodsDesc.text = "EXP";

            Player.Level.ExpUpAndLvUp(expRewardValue);

            string tempTxt = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_expClear].StringToLocal;
            _viewContents.perishDesc.text = string.Format(tempTxt, bestlevel + 1);

            Player.ControllerGood.Consume(Definition.GoodsKey.ExpDungeonTicket, 1);
            Player.Quest.TryCountUp(QuestType.ExpDungeonClear, 1);
            Player.Quest.TryCountUp(QuestType.DungeonClear, 1);
        }
        private void AwakeDungeonPerish()
        {
            if (Player.ControllerGood.GetValue(GoodsKey.AwakeDungeonTicket) < 1)
            {
                string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_DungeonKeyNotEnough].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                return;
            }
            _viewContents.perishWindow.SetActive(true);
            double rewardAmount = Player.Cloud.Dungeondata.dungeoninfo[(int)DungeonType.Awake].bestDamage / 10.0f;
            ViewGoodRewardSlot slotobj = null;
            for (int i = 0; i < slotListInPerishWindow.Count; i++)
            {
                slotListInPerishWindow[i].gameObject.SetActive(false);
            }
            if (slotListInPerishWindow.Count <= 0)
            {
                slotobj = UnityEngine.Object.Instantiate(_viewContents.rewardSlotPrefab);
                slotListInPerishWindow.Add(slotobj);
                slotobj.gameObject.SetActive(true);
            }
            else
            {
                slotobj = slotListInPerishWindow[0];
                slotobj.gameObject.SetActive(true);
            }
            slotobj.transform.SetParent(_viewContents.rewardSlotParentinPerish, false);
            slotobj.goodsIcon.sprite = GoodResourcesBundle.Loaded.rewardSprites[(int)RewardTypes.AwakeStone];
            slotobj.goodValue.text = rewardAmount.ToNumberString();
            slotobj.goodsDesc.text = "";

            string tempTxt = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_awakeClear].StringToLocal;
            _viewContents.perishDesc.text = string.Format(tempTxt, rewardAmount.ToNumberString());

            Player.ControllerGood.Earn((GoodsKey)(RewardTypes.AwakeStone), rewardAmount);
            Player.ControllerGood.Consume(Definition.GoodsKey.AwakeDungeonTicket, 1);

            Player.Quest.TryCountUp(QuestType.AwakeDungeonClear, 1);
            Player.Quest.TryCountUp(QuestType.DungeonClear, 1);
        }
        private void RiftDungeonPerish()
        {
            if (Player.ControllerGood.GetValue(GoodsKey.RiftDungeonTicket) < 1)
            {
                string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_DungeonKeyNotEnough].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                return;
            }
            _viewContents.perishWindow.SetActive(true);
           
            var currentlevel = Battle.Dungeon.currentDungeonData[(int)DungeonType.Rift].currentDungeonLevel;
            var rewardTableData_rift = StaticData.Wrapper.riftDungeonRewardTableDatas[currentlevel];

            ViewGoodRewardSlot slotobj = null;
            for (int i = 0; i < slotListInPerishWindow.Count; i++)
            {
                slotListInPerishWindow[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < rewardTableData_rift.rewardAmount.Length; i++)
            {
                if (slotListInPerishWindow.Count <= i)
                {
                    slotobj = UnityEngine.Object.Instantiate(_viewContents.rewardSlotPrefab);
                    slotListInPerishWindow.Add(slotobj);
                    slotobj.gameObject.SetActive(true);
                }
                else
                {
                    slotobj = slotListInPerishWindow[i];
                    slotobj.gameObject.SetActive(true);
                }

                double perishRewardAmount = rewardTableData_rift.rewardAmount[i]/2.0f;

                slotobj.transform.SetParent(_viewContents.rewardSlotParentinPerish, false);
                slotobj.goodsIcon.sprite = GoodResourcesBundle.Loaded.rewardSprites[(int)rewardTableData_rift.rewardType[i]];
                slotobj.goodValue.text = perishRewardAmount.ToNumberString();
                slotobj.goodsDesc.text = "";

                RewardObtain((rewardTableData_rift.rewardType[i]), perishRewardAmount);
            }
            string tempTxt = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_riftClear].StringToLocal;
            _viewContents.perishDesc.text = string.Format(tempTxt, currentlevel + 1);
            
            Player.ControllerGood.Consume(Definition.GoodsKey.RiftDungeonTicket, 1);

            Player.Quest.TryCountUp(QuestType.RiftDungeonClear, 1);
            Player.Quest.TryCountUp(QuestType.DungeonClear, 1);
        }

        private void RewardObtain(RewardTypes rewardType, double amount)
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
                case RewardTypes.Exp:
                    Player.Level.ExpUpAndLvUp(amount);
                    break;
                case RewardTypes.skillAwakeStone:
                    Player.ControllerGood.Earn(GoodsKey.SkillAwakeStone, amount);
                    break;
                default:
                    break;
            }
        }
    }
}
