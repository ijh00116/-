using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using BlackTree.Definition;

namespace BlackTree.Core
{
    public class ControllerEnterClickerDungeon
    {
        ViewCanvasEnterClickerDungeon _viewCanvasEnterClicker;
        CancellationTokenSource _cts;

        List<ViewGoodRewardSlot> slotList = new List<ViewGoodRewardSlot>();
        List<ViewGoodRewardSlot> slotList_forPet = new List<ViewGoodRewardSlot>();

        List<ClickDungeonRateTableSlot> rateTableSlotList = new List<ClickDungeonRateTableSlot>();
        public ControllerEnterClickerDungeon(Transform parent, CancellationTokenSource cts)
        {
            _cts = cts;
            _viewCanvasEnterClicker = ViewCanvas.Create<ViewCanvasEnterClickerDungeon>(parent);
            _viewCanvasEnterClicker.SetVisible(false);

            _viewCanvasEnterClicker.BindOnChangeVisible(OnClickDungeonUIVisible);

            _viewCanvasEnterClicker.enterBtn.onClick.AddListener(EnterClickerDungeon);

            for(int i=0; i< _viewCanvasEnterClicker.closeBtn.Length; i++)
            {
                int index = i;
                _viewCanvasEnterClicker.closeBtn[index].onClick.AddListener(UIOff);
            }

            _viewCanvasEnterClicker.openRateTableWindow.onClick.AddListener(OpenRateTable);

            for(int i=0; i<_viewCanvasEnterClicker.closeRateTableWindow.Length; i++)
            {
                _viewCanvasEnterClicker.closeRateTableWindow[i].onClick.AddListener(()=> {
                    _viewCanvasEnterClicker.rateTableWindow.SetActive(false);
                });
            }
        }

        void OnClickDungeonUIVisible(bool active)
        {
            if(active)
            {
                double maxGoodsCount=Battle.Clicker.dayMaxEnemyKillCount;
                double currentGoodsCount = Player.Cloud.clickerDungeonData.todayEarnRewardCount;
                _viewCanvasEnterClicker.currentRSPotionCount.text = string.Format("{0}/{1}",currentGoodsCount.ToNumberString(),maxGoodsCount.ToNumberString());

                for(int i=0; i<Player.Cloud.clickerDungeonData.todayEarnRewardtypeList.Count; i++)
                {
                    ViewGoodRewardSlot slotobj = null;
                    int slotIndex = i;

                    var rewardType = Player.Cloud.clickerDungeonData.todayEarnRewardtypeList[i];
                    double rewardAmount = Player.Cloud.clickerDungeonData.todayEarnRewardAmountList[i];

                    if (slotList.Count <= slotIndex)
                    {
                        slotobj = UnityEngine.Object.Instantiate(_viewCanvasEnterClicker.rewardslot);
                        slotList.Add(slotobj);
                        slotobj.gameObject.SetActive(true);
                    }
                    else
                    {
                        slotobj = slotList[slotIndex];
                        slotobj.gameObject.SetActive(true);
                    }
                    slotobj.transform.SetParent(_viewCanvasEnterClicker.rewardslotParent, false);
                    slotobj.goodsIcon.sprite = GoodResourcesBundle.Loaded.rewardSprites[(int)rewardType];
                    slotobj.goodValue.text = rewardAmount.ToNumberString();
                    slotobj.goodsDesc.text = "";
                }
            }
        }

        void EnterClickerDungeon()
        {
            if (Player.Cloud.clickerDungeonData.currentKillCount >= Battle.Clicker.dayMaxEnemyKillCount)
            {
                string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_EndCountOfClickDungeon].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                return;
            }
            Battle.Field.ChangeSceneState(eSceneState.WaitForClickerDungeon);
            //UIOff();
        }

        void UIOff()
        {
            _viewCanvasEnterClicker.blackBG.PopupCloseColorFade();
            _viewCanvasEnterClicker.Wrapped.CommonPopupCloseAnimationUp(() => {
                _viewCanvasEnterClicker.SetVisible(false);
            });
        }

        #region rateTable
        void OpenRateTable()
        {
            _viewCanvasEnterClicker.rateTableWindow.SetActive(true);

            var clickerRewardInfo = StaticData.Wrapper.ClickerDungeonRewardInfo[Player.Cloud.field.bestChapter];

            _viewCanvasEnterClicker.currentdifficulty.text = $"현재 난이도: 챕터 {Player.Cloud.field.bestChapter+1}";

            for(int i=0; i< rateTableSlotList.Count;i++)
            {
                rateTableSlotList[i].gameObject.SetActive(false);
            }
            for (int i=0;i< clickerRewardInfo.rates.Length; i++)
            {
                ClickDungeonRateTableSlot slot=null;
                int index = i;
                if (index >= rateTableSlotList.Count)
                {
                    slot = UnityEngine.Object.Instantiate(_viewCanvasEnterClicker.rateTableSlot);
                    slot.transform.SetParent(_viewCanvasEnterClicker.slotParent, false);
                    rateTableSlotList.Add(slot);
                }
                else
                {
                    slot = rateTableSlotList[index];
                }
                slot.gameObject.SetActive(true);
                
                var rewardType = clickerRewardInfo.rewardType[index];
                double rewardAmount = clickerRewardInfo.rewardAmount[index];

                slot.goodImage.sprite = GoodResourcesBundle.Loaded.rewardSprites[(int)rewardType];
                slot.amountText.text = rewardAmount.ToNumberString();
                slot.rateText.text = string.Format("{0}%", (int)(clickerRewardInfo.rates[index] * 100));
            }
        }
        #endregion
    }

}
