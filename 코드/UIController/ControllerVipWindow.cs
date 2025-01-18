using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using BlackTree.Definition;
using Cysharp.Threading.Tasks;
using System;

namespace BlackTree.Core
{
    public class ControllerVipWindow
    {
        ViewCanvasVipWindow _view;
        CancellationTokenSource _cts;

        List<ViewGoodRewardSlot> rewardList = new List<ViewGoodRewardSlot>();
        public ControllerVipWindow(Transform parent, CancellationTokenSource cts)
        {
            _cts = cts;
            _view = ViewCanvas.Create<ViewCanvasVipWindow>(parent);
            _view.SetVisible(false);
            if (Player.Option.isAnotherDay())
            {
                Player.Cloud.inAppPurchase.isVipDailyRewardGet = false;
                InitializeVipReward();
            }

            Player.Products.PackagePurchaseCallback += InitializeVipReward;
            _view.getRewardBtn.onClick.AddListener(GetReward);
            _view.BindOnChangeVisible(OnPopupOpen);

            _view.closeBtn.onClick.AddListener(WindowOff);

            InitializeVipReward();

            Player.Option.AnotherDaySetting += AnotherDaySetting;
        }

        void AnotherDaySetting()
        {
            Player.Cloud.inAppPurchase.isVipDailyRewardGet = false;
            InitializeVipReward();
        }

        void OnPopupOpen(bool active)
        {
            if (Player.Cloud.inAppPurchase.isVipDailyRewardGet)
            {
                _view.alreadyRewarded.SetActive(true);
                for (int i = 0; i < rewardList.Count; i++)
                {
                    rewardList[i].gameObject.SetActive(false);
                }
            }
            else
            {
                _view.alreadyRewarded.SetActive(false);
            }

            InitializeVipReward();
        }

        void InitializeVipReward()
        {
            if (Player.Cloud.inAppPurchase.purchaseVip <= 0)
                return;

            for(int i=0; i< rewardList.Count; i++)
            {
                rewardList[i].gameObject.SetActive(false);
            }

            int index = Player.Cloud.inAppPurchase.purchaseVip - 1;
            for(int i=0; i<StaticData.Wrapper.vipDailyRewardADatas[index].rewardTypes.Length; i++)
            {
                if(i<rewardList.Count)
                {
                    var obj = rewardList[i];
                    obj.goodsIcon.sprite = GoodResourcesBundle.Loaded.goodSprites[(int)StaticData.Wrapper.vipDailyRewardADatas[index].rewardTypes[i]];
                    obj.goodValue.text = StaticData.Wrapper.vipDailyRewardADatas[index].rewardAmounts[i].ToNumberString();
                    obj.goodsDesc.text = "";
                    obj.gameObject.SetActive(true);
                }
                else
                {
                    var obj = UnityEngine.Object.Instantiate(_view.rewardSlotPrefab);
                    obj.goodsIcon.sprite = GoodResourcesBundle.Loaded.goodSprites[(int)StaticData.Wrapper.vipDailyRewardADatas[index].rewardTypes[i]];
                    obj.goodValue.text = StaticData.Wrapper.vipDailyRewardADatas[index].rewardAmounts[i].ToNumberString();
                    obj.goodsDesc.text = "";

                    obj.transform.SetParent(_view.slotParent, false);
                    rewardList.Add(obj);
                }
            }
            if (Player.Cloud.inAppPurchase.isVipDailyRewardGet)
            {
                for (int i = 0; i < rewardList.Count; i++)
                {
                    rewardList[i].gameObject.SetActive(false);
                }
            }

                VipFixedRewardSet();
        }

        public void VipFixedRewardSet()
        {
            int vipLevel = Player.Cloud.inAppPurchase.purchaseVip;
            if (vipLevel==1)
            {
                _view.vipFixedgoldRewardSlot.goodsIconFrame.sprite = GoodResourcesBundle.Loaded.rewardSlotBGFrame[1];
                _view.vipFixedgoldRewardSlot.goodsIcon.sprite = GoodResourcesBundle.Loaded.rewardSprites[(int)RewardTypes.Coin];

                _view.vipFixedgoldRewardSlot.goodsDesc.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.VIP_DescFiexdRewardGold].StringToLocal;
                string goldvaluedesc = string.Format("{0:F1}", StaticData.Wrapper.vIPFixedRewardsDatas[0].goldFixedRewardTimes);
                _view.vipFixedgoldRewardSlot.goodValue.text = $"x{goldvaluedesc}";

                _view.vipFixedExpRewardSlot.goodsIconFrame.sprite = GoodResourcesBundle.Loaded.rewardSlotBGFrame[1];
                _view.vipFixedExpRewardSlot.goodsIcon.sprite = GoodResourcesBundle.Loaded.rewardSprites[(int)RewardTypes.Exp];
                _view.vipFixedExpRewardSlot.goodsDesc.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.VIP_DescFiexdRewardExp].StringToLocal;
                string expvaluedesc = string.Format("{0:F1}", StaticData.Wrapper.vIPFixedRewardsDatas[0].expFixedRewardTimes);
                _view.vipFixedExpRewardSlot.goodValue.text = $"x{expvaluedesc}";

                _view.vipFixedgoldRewardSlot.gameObject.SetActive(true);
                _view.vipFixedExpRewardSlot.gameObject.SetActive(true);
                _view.vipFixedDungeonRewardSlot.gameObject.SetActive(false);
            }
            if (vipLevel == 2)
            {
                _view.vipFixedgoldRewardSlot.goodsIconFrame.sprite = GoodResourcesBundle.Loaded.rewardSlotBGFrame[1];
                _view.vipFixedgoldRewardSlot.goodsIcon.sprite = GoodResourcesBundle.Loaded.rewardSprites[(int)RewardTypes.Coin];

                _view.vipFixedgoldRewardSlot.goodsDesc.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.VIP_DescFiexdRewardGold].StringToLocal;
                string goldvaluedesc = string.Format("{0:F1}", StaticData.Wrapper.vIPFixedRewardsDatas[1].goldFixedRewardTimes);
                _view.vipFixedgoldRewardSlot.goodValue.text = $"x{goldvaluedesc}";

                _view.vipFixedExpRewardSlot.goodsIconFrame.sprite = GoodResourcesBundle.Loaded.rewardSlotBGFrame[1];
                _view.vipFixedExpRewardSlot.goodsIcon.sprite = GoodResourcesBundle.Loaded.rewardSprites[(int)RewardTypes.Exp];
                _view.vipFixedExpRewardSlot.goodsDesc.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.VIP_DescFiexdRewardExp].StringToLocal;
                string expvaluedesc = string.Format("{0:F1}", StaticData.Wrapper.vIPFixedRewardsDatas[1].expFixedRewardTimes);
                _view.vipFixedExpRewardSlot.goodValue.text = $"x{expvaluedesc}";

                _view.vipFixedgoldRewardSlot.gameObject.SetActive(true);
                _view.vipFixedExpRewardSlot.gameObject.SetActive(true);
                _view.vipFixedDungeonRewardSlot.gameObject.SetActive(false);
            }
            if (vipLevel == 3)
            {
                _view.vipFixedgoldRewardSlot.goodsIconFrame.sprite = GoodResourcesBundle.Loaded.rewardSlotBGFrame[1];
                _view.vipFixedgoldRewardSlot.goodsIcon.sprite = GoodResourcesBundle.Loaded.rewardSprites[(int)RewardTypes.Coin];
                _view.vipFixedgoldRewardSlot.goodsDesc.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.VIP_DescFiexdRewardGold].StringToLocal;
                string goldvaluedesc = string.Format("{0:F1}", StaticData.Wrapper.vIPFixedRewardsDatas[2].goldFixedRewardTimes);
                _view.vipFixedgoldRewardSlot.goodValue.text = $"x{goldvaluedesc}";

                _view.vipFixedExpRewardSlot.goodsIconFrame.sprite = GoodResourcesBundle.Loaded.rewardSlotBGFrame[2];
                _view.vipFixedExpRewardSlot.goodsIcon.sprite = GoodResourcesBundle.Loaded.rewardSprites[(int)RewardTypes.Exp];
                _view.vipFixedExpRewardSlot.goodsDesc.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.VIP_DescFiexdRewardExp].StringToLocal;
                string expvaluedesc = string.Format("{0:F1}", StaticData.Wrapper.vIPFixedRewardsDatas[2].expFixedRewardTimes);
                _view.vipFixedExpRewardSlot.goodValue.text = $"x{expvaluedesc}";

                _view.vipFixedDungeonRewardSlot.goodsIconFrame.sprite = GoodResourcesBundle.Loaded.rewardSlotBGFrame[1];
                _view.vipFixedDungeonRewardSlot.goodsIcon.sprite = GoodResourcesBundle.Loaded.rewardSprites[(int)RewardTypes.AwakeStone];
                _view.vipFixedDungeonRewardSlot.goodsDesc.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.VIP_DescFiexdRewardAwakeStone].StringToLocal;
                string awakevaluedesc = string.Format("{0:F1}", StaticData.Wrapper.vIPFixedRewardsDatas[2].DungeonFixedRewardTimes);
                _view.vipFixedDungeonRewardSlot.goodValue.text = $"x{awakevaluedesc}";

                _view.vipFixedgoldRewardSlot.gameObject.SetActive(true);
                _view.vipFixedExpRewardSlot.gameObject.SetActive(true);
                _view.vipFixedDungeonRewardSlot.gameObject.SetActive(true);

               
            }
        }

        void GetReward()
        {
            if (Player.Cloud.inAppPurchase.isVipDailyRewardGet)
                return;
            int index = Player.Cloud.inAppPurchase.purchaseVip - 1;
            for (int i = 0; i < StaticData.Wrapper.vipDailyRewardADatas[index].rewardTypes.Length; i++)
            {
                GoodsKey key = StaticData.Wrapper.vipDailyRewardADatas[index].rewardTypes[i];
                Player.ControllerGood.Earn(key, StaticData.Wrapper.vipDailyRewardADatas[index].rewardAmounts[i]);
            }
            Player.Cloud.inAppPurchase.isVipDailyRewardGet = true;

            Player.Cloud.inAppPurchase.UpdateHash().SetDirty(true);
            Player.SaveUserDataToFirebaseAndLocal().Forget();

            _view.alreadyRewarded.SetActive(true);
            for (int i = 0; i < rewardList.Count; i++)
            {
                rewardList[i].gameObject.SetActive(false);
            }

            var toastCanvas = ViewCanvas.Get<ViewCanvasToastMessage>();

            for (int i = 0; i < toastCanvas.rewardSlotList.Count; i++)
            {
                toastCanvas.rewardSlotList[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < StaticData.Wrapper.vipDailyRewardADatas[index].rewardTypes.Length; i++)
            {
                ViewGoodRewardSlot slotObj = null;
                if (i < toastCanvas.rewardSlotList.Count)
                {
                    slotObj = toastCanvas.rewardSlotList[i];
                }
                else
                {
                    slotObj = UnityEngine.Object.Instantiate(toastCanvas.rewardSlotPrefab);
                    slotObj.transform.SetParent(toastCanvas.rewardParent, false);
                    toastCanvas.rewardSlotList.Add(slotObj);
                }
                GoodsKey key = StaticData.Wrapper.vipDailyRewardADatas[index].rewardTypes[i];
                slotObj.goodValue.text = StaticData.Wrapper.vipDailyRewardADatas[index].rewardAmounts[i].ToNumberString();
                slotObj.goodsIcon.sprite = GoodResourcesBundle.Loaded.goodSprites[(int)key];

                slotObj.gameObject.SetActive(true);
            }

            string localizedvalue = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_GoodsReward].StringToLocal;
            toastCanvas.titleDesc.text = localizedvalue;
            toastCanvas.RewardPopupShowandFadeAsync().Forget();
            //WindowOff();
        }

        void WindowOff()
        {
            _view.blackBG.PopupCloseColorFade();
            _view.Wrapped.CommonPopupCloseAnimationUp(() => {
                _view.SetVisible(false);
            });
        }

    }
}

