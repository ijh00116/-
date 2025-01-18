using UnityEngine;
using BlackTree.Definition;
using System.Threading;
using Cysharp.Threading.Tasks;
using BlackTree.Bundles;
using BlackTree.Model;
using System.Collections.Generic;

namespace BlackTree.Core
{
    public class ControllerResearchWindow
    {
        enum TierType
        {
            tier1,tier2,
        }
        ViewCanvasResearchWindow _view;
        CancellationTokenSource _cts;

        ViewResearchDataSlot[] researchDataSlotList;
 
        ResearchUpgradeKey currentSelectedResearchType;
        System.DateTime currentSelectedExitTime;

        const int bitFalgForResearchDataEnum=64;

        List<ViewCurrentResearchSlot> currentResearchSlotList = new List<ViewCurrentResearchSlot>();


        ViewCanvasMainIcons _viewMainIcon;
        TierType currentTier;
        public ControllerResearchWindow(Transform parent,CancellationTokenSource cts)
        {
            _cts = cts;
            _view = ViewCanvas.Create<ViewCanvasResearchWindow>(parent);

            researchDataSlotList = new ViewResearchDataSlot[StaticData.Wrapper.researchTableDatas.Length];

            for(int i=0; i< researchDataSlotList.Length; i++)
            {
                int index = i;
                var tabledata = StaticData.Wrapper.researchTableDatas[index];

                var slotPrefab = UnityEngine.Object.Instantiate(_view.researchDataSlotPrefab);
                slotPrefab.transform.SetParent(_view.researchData_scrollRect.content,false);
                slotPrefab.Init(tabledata.researchTypeKey);

                slotPrefab.SetIcon().SetName().SetLevel().SetCost();
                //addListener
                slotPrefab.AddOpenDetailResearchWindowActionBtn(()=> {
                    OpenDetailResearchWindow(slotPrefab.researchType);
                });
             
                researchDataSlotList[index] = slotPrefab;
            }

            for(int i=0; i<Player.Research.PossibleResearchCount; i++)
            {
                if (i >= currentResearchSlotList.Count)
                {
                    var key = Player.Cloud.researchData.currentResearchKeylist[i];

                    var slotPrefab = UnityEngine.Object.Instantiate(_view.currentResearchDataSlotPrefab);
                    slotPrefab.transform.SetParent(_view.currentResearchData_scrollRect.content, false);
                    slotPrefab.Init(key);
                    currentResearchSlotList.Add(slotPrefab);
                }
            }
            if(Player.Research.PossibleResearchCount >= Player.Research.maxResearchCount)
            {
                _view.recommendPlusSlot.SetActive(false);
            }
            else
            {
                _view.recommendPlusSlot.SetActive(true);
                _view.recommendPlusSlot.transform.SetAsLastSibling();
            }

            Player.Research.StartUpdateSyncactions += StartUpdateListView;
            Player.Research.CompleteUpdateSyncactions += CompleteUpdateListView;

            for(int i=0; i< _view.closeBtn.Length; i++)
            {
                int index = i;
                _view.closeBtn[index].onClick.AddListener(CloseWindow);
            }

            for(int i=0; i< _view.detailCloseBtn.Length; i++)
            {
                int index = i;
                _view.detailCloseBtn[index].onClick.AddListener(CloseDetailResearchWindow);
            }

            _view.startCurrentSelectResearchBtn.onClick.AddListener(()=> { TryUpgrade(currentSelectedResearchType); });

            _view.obtainResearchPotion.Init();
            Player.ControllerGood.BindOnChange(GoodsKey.ResearchPotion, () =>
            {
                UpdateGoodView(GoodsKey.ResearchPotion);
            });
            UpdateGoodView(GoodsKey.ResearchPotion);

            _viewMainIcon = ViewCanvas.Get<ViewCanvasMainIcons>();

            TimeCheck().Forget();
            ChangeTier(TierType.tier1);

            _view.nextTierBtn.onClick.AddListener(NextTierPush);
            _view.prevTierBtn.onClick.AddListener(PrevTierPush);

            _view.goodsBtn.onClick.AddListener(() => {
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasInventory>().SetVisible(true);
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasInventory>().blackBG.PopupOpenColorFade();
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasInventory>().Wrapped.CommonPopupOpenAnimationDown();
            });
            string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_ResearchSlot].StringToLocal;
            _view.slotExpandNeeded.text = localizedvalue;

            string titlelocalizedvalue = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Research].StringToLocal;
            _view.titleTxt.text = titlelocalizedvalue;
        }

        bool IsTier1MaxLevel()
        {
            bool AllMax = true;
            for(int i=0; i<(int)ResearchUpgradeKey.SwordAttackIncrease_2; i++)
            {
                bool isMax = Player.Research.isMaxLevel((ResearchUpgradeKey)i);
                if(isMax==false)
                {
                    AllMax = false;
                    break;
                }
            }

            return AllMax;
        }

        void NextTierPush()
        {
            bool isAllMax = IsTier1MaxLevel();
            if(isAllMax)
            {
                ChangeTier(TierType.tier2);
            }
            else
            {
               // ChangeTier(TierType.tier2);
               
                string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_1TierUpgradeNeed].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
            }
        }

        void PrevTierPush()
        {
            ChangeTier(TierType.tier1);
        }

        void ChangeTier(TierType _tiertype)
        {
            currentTier = _tiertype;
            string localizedvalue = "";
            switch (currentTier)
            {
                case TierType.tier1:
                    localizedvalue = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_FirstTier].StringToLocal;
                    _view.tierText.text = localizedvalue;
                    break;
                case TierType.tier2:
                    localizedvalue = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_SecondTier].StringToLocal;
                    _view.tierText.text = localizedvalue;
                    break;
                default:
                    break;
            }
            UpdateslotList();
        }

        void UpdateslotList()
        {
            for(int i=0; i< researchDataSlotList.Length; i++)
            {
                researchDataSlotList[i].gameObject.SetActive(false);
            }

            if(currentTier==TierType.tier1)
            {
                for(int i=0; i<(int)ResearchUpgradeKey.SwordAttackIncrease_2; i++)
                {
                    researchDataSlotList[i].gameObject.SetActive(true);
                }
            }
            else
            {
                for (int i = (int)ResearchUpgradeKey.SwordAttackIncrease_2; i < (int)ResearchUpgradeKey.End; i++)
                {
                    researchDataSlotList[i].gameObject.SetActive(true);
                }
            }
        }
        private void UpdateGoodView(GoodsKey key)
        {
            if (_view.obtainResearchPotion.GoodsType == key)
            {
                var _value = Player.ControllerGood.GetValue(key);
                _view.obtainResearchPotion.StartRoutineUpdateView(_value);
            }
        }

        void CloseWindow()
        {
            _view.blackBG.PopupCloseColorFade();
            _view.Wrapped.CommonPopupCloseAnimationUp(() => {
                _view.SetVisible(false);
            });
        }
        void OpenDetailResearchWindow(ResearchUpgradeKey key)
        {
            currentSelectedResearchType = key;

            _view.detailTotalWindow.SetActive(true);

            _view.detailWindowBlackBg.PopupOpenColorFade();
            _view.detailWindowPopup.CommonPopupOpenAnimationUp();

            var tabledata= Player.Research.GetTableData(key);

            _view.currentSelectResearchIcon.sprite= InGameResourcesBundle.Loaded.researchIconSprites[tabledata.index];
            _view.currentSelectResearchTitle.text= string.Format(StaticData.Wrapper.localizednamelist[(int)tabledata.nameLmk].StringToLocal, Player.Research.GetValue(tabledata.researchTypeKey));
            _view.currentSelectResearchLevel.text= string.Format("{0}", Player.Research.GetLevel(key));
            if (Player.Research.isMaxLevel(key))
            {
                _view.currentSelectResearchDesc.text = string.Format("{0}",Player.Research.GetValue(key));
            }
            else
            {
                _view.currentSelectResearchDesc.text = string.Format("{0} > {1}", Player.Research.GetValue(key), Player.Research.GetNextValue(key));
            }
            int levelindex = Player.Research.GetLevel(key);

            double min;
            if (key >= ResearchUpgradeKey.SwordAttackIncrease_2)
            {
                min =Player.Research.GetTime(key);
            }
            else
            {
                min = StaticData.Wrapper.researchTableSequence[tabledata.index].ResearchTime_min;
            }

            int hour = (int)(min / 60);
            int minute = (int)(min % 60);
            _view.currentSelectResearchTime.text = string.Format("{0:D2}:{1:D2}:00", hour, minute);

            _view.currentSelectResearchCost.text= Player.Research.GetCost(tabledata.researchTypeKey).ToNumberString();

        }

        void CloseDetailResearchWindow()
        {
            _view.detailWindowBlackBg.PopupCloseColorFade();
            _view.detailWindowPopup.CommonPopupCloseAnimationDown(()=> {
                _view.detailTotalWindow.SetActive(false);
            });
        }

        async UniTaskVoid TimeCheck()
        {
            while (true)
            {
                Player.Research.currentTime = Extension.GetServerTime();

                bool canResearch = false;
                for (int i = 0; i < currentResearchSlotList.Count; i++)
                {
                    if (currentResearchSlotList[i].currentResearch == ResearchUpgradeKey.None)
                    {
                        canResearch = true;
                        break;
                    }
                }
                bool rpCanResearch = false;
                for(int i=0; i<(int)ResearchUpgradeKey.End; i++)
                {
                    if(Player.ControllerGood.GetValue(GoodsKey.ResearchPotion)>=Player.Research.GetCost((ResearchUpgradeKey)i))
                    {
                        rpCanResearch = true;
                        break;
                    }
                }

                _viewMainIcon.researchReddotObj.SetActive(canResearch&& rpCanResearch);

                await UniTask.Delay(1000);
            }
        }

        private bool TryUpgrade(ResearchUpgradeKey key)
        {
            var tableData= Player.Research.GetTableData(key);
            var ismaxLevel = Player.Research.isMaxLevel(key);
            var progressing = Player.Research.isProgressing(key);

            if (ismaxLevel || progressing)
                return false;

            bool canResearch = false;
            for (int i = 0; i < currentResearchSlotList.Count; i++)
            {
                if (currentResearchSlotList[i].currentResearch == ResearchUpgradeKey.None)
                {
                    canResearch = true;
                    break;
                }
            }
            if(canResearch==false)
            {
                string noMoreSlot = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_ResearchKeyNotEnough].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(noMoreSlot);
                return false;
            }

            var cost = Player.Research.GetCost(key);

            if (Player.ControllerGood.IsCanBuy(tableData.goodKey, cost)==false)
            {
                string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_ResearchKeyNotEnough].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                return false;
            }

            Player.ControllerGood.Consume(tableData.goodKey, cost);
            Player.Research.StartUpgrade(key);

            CloseDetailResearchWindow();
            return true;
        }

        private void StartUpdateListView(ResearchUpgradeKey key)
        {
            var tableData = Player.Research.GetTableData(key);
            int slotIndex = tableData.index;

            researchDataSlotList[slotIndex].
                SetLevel().SetCost().SetProgressState();

   
            for(int i=0; i< currentResearchSlotList.Count; i++)
            {
                if(currentResearchSlotList[i].currentResearch==ResearchUpgradeKey.None)
                {
                    Player.Cloud.researchData.currentResearchKeylist[i] = key;
                    LocalSaveLoader.SaveUserCloudData();
                    currentResearchSlotList[i].Init(key);
                    break;
                }
            }

            if (Player.Research.PossibleResearchCount >= Player.Research.maxResearchCount)
            {
                _view.recommendPlusSlot.SetActive(false);
            }
            else
            {
                _view.recommendPlusSlot.SetActive(true);
                _view.recommendPlusSlot.transform.SetAsLastSibling();
            }
        }

        private void CompleteUpdateListView(ResearchUpgradeKey key)
        {
            var tableData = Player.Research.GetTableData(key);
            int slotIndex = tableData.index;

            researchDataSlotList[slotIndex].
                SetLevel().SetCost().SetName().SetProgressState();

            if (key == ResearchUpgradeKey.ExpandResearchSlot)
            {
                for (int i = 0; i < Player.Research.PossibleResearchCount; i++)
                {
                    if (i >= currentResearchSlotList.Count)
                    {
                        var slotPrefab = UnityEngine.Object.Instantiate(_view.currentResearchDataSlotPrefab);
                        slotPrefab.transform.SetParent(_view.currentResearchData_scrollRect.content, false);
                        slotPrefab.Init(ResearchUpgradeKey.None);
                        currentResearchSlotList.Add(slotPrefab);
                    }
                }
            }

            for (int i = 0; i < currentResearchSlotList.Count; i++)
            {
                if (currentResearchSlotList[i].currentResearch == key)
                {
                    Player.Cloud.researchData.currentResearchKeylist[i] = ResearchUpgradeKey.None;
                    LocalSaveLoader.SaveUserCloudData();
                    currentResearchSlotList[i].Init(ResearchUpgradeKey.None);
                    break;
                }
            }

            if (Player.Research.PossibleResearchCount >= Player.Research.maxResearchCount)
            {
                _view.recommendPlusSlot.SetActive(false);
            }
            else
            {
                _view.recommendPlusSlot.SetActive(true);
                _view.recommendPlusSlot.transform.SetAsLastSibling();
            }
            string localizedvalue = string.Format(StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Research_Complete].StringToLocal);
            ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
        }

    }
}                              