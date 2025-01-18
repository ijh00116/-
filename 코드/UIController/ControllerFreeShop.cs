using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using BlackTree.Definition;
using System;
using static BlackTree.Model.Player;

namespace BlackTree.Core
{
    public class ControllerFreeShop
    {
        ViewCanvasFreeShop _view;
        private readonly CancellationTokenSource _cts;

        List<ViewAdShopItem> shopItemList = new List<ViewAdShopItem>();
        public ControllerFreeShop(Transform parent, CancellationTokenSource cts)
        {
            _view = ViewCanvas.Create<ViewCanvasFreeShop>(parent);

            for(int i=Player.Cloud.adShopProductData.boughtProductList.Count; i< StaticData.Wrapper.adShopProductDatas.Length; i++)
            {
                AdShopProductHistory adshopHistory = new AdShopProductHistory() { boughtCount = 0 };
                Player.Cloud.adShopProductData.boughtProductList.Add(adshopHistory);
            }

            if (Player.Option.isAnotherDay())
            {
                for(int i=0; i< Player.Cloud.adShopProductData.boughtProductList.Count; i++)
                {
                    Player.Cloud.adShopProductData.boughtProductList[i].boughtCount = 0;
                }
            }

            for (int i = 0; i < StaticData.Wrapper.adShopProductDatas.Length; i++)
            {
                var productData = StaticData.Wrapper.adShopProductDatas[i];
                var productObj = UnityEngine.Object.Instantiate(_view.productPrefab);
                productObj.transform.SetParent(_view.productParent.content, false);

                int index = i;
                productObj.Init(productData, index);

                shopItemList.Add(productObj);
            }

            for(int i=0; i< _view.closeBtns.Length; i++)
            {
                int index = i;
                _view.closeBtns[index].onClick.AddListener(CloseWindow);
            }

            Player.Products.FreeShopAwakeStoneSync += UpdateSyncAwakeStoneValue;
            Player.Option.AnotherDaySetting += AnotherDaySetting;

            _view.titleTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_FreeShop].StringToLocal;
        }

        void AnotherDaySetting()
        {
            for (int i = 0; i < Player.Cloud.adShopProductData.boughtProductList.Count; i++)
            {
                Player.Cloud.adShopProductData.boughtProductList[i].boughtCount = 0;
            }
            for(int i=0; i< shopItemList.Count; i++)
            {
                shopItemList[i].SyncAfterPurchase();
            }
        }

        void UpdateSyncAwakeStoneValue()
        {
            for(int i=0; i< shopItemList.Count; i++)
            {
                shopItemList[i].AmountUpdate();
            }
        }

        void CloseWindow()
        {
            _view.blackBG.PopupCloseColorFade();
            _view.Wrapped.CommonPopupCloseAnimationUp(() => {
                _view.SetVisible(false);
            });
        }
    }

}
