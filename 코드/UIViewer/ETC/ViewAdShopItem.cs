using BlackTree.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BlackTree.Model;
using BlackTree.Definition;
using Cysharp.Threading.Tasks;

namespace BlackTree.Bundles
{
    public class ViewAdShopItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text title;
        [SerializeField] private Image bgGoodsImage;

        [SerializeField] private GameObject countTextObj;
        [SerializeField] private TMP_Text _buyableCount_dia;

        [SerializeField] private TMP_Text _goodAmount;
        [SerializeField] private TMP_Text _buyableCount_ad;
        [SerializeField] private BTButton button;

        [SerializeField] private GameObject adImage;
        [SerializeField] private GameObject diaImage;
  

        AdShopProduct productTableData;
        int productIndex = 0;

        double awakeStoneAmount = 0;
        public void Init(AdShopProduct _data,int _productIndex)
        {
            productTableData = _data;
            productIndex = _productIndex;

            title.text = StaticData.Wrapper.localizednamelist[(int)_data.productName].StringToLocal;
            bgGoodsImage.sprite = GoodResourcesBundle.Loaded.goodSprites[(int)_data.goodsType];

            if(productTableData.goodsType==GoodsKey.AwakeStone)
            {
                awakeStoneAmount = Player.Unit.SwordAtk + Player.Unit.BowAtk;
                _goodAmount.text = awakeStoneAmount.ToNumberString();
            }
            else
            {
                _goodAmount.text = productTableData.rewardAmount.ToString();
            }
            

            countTextObj.gameObject.SetActive(false);
            //_buyableCount_dia.gameObject.SetActive(false);
            adImage.SetActive(false);
            diaImage.SetActive(false);

            int boughtCount = Player.Cloud.adShopProductData.boughtProductList[productIndex].boughtCount;
            int maxBuyablecount = _data.dailyBuyableCount;

            if (productTableData.isAd)
            {
                adImage.SetActive(true);
                _buyableCount_ad.text = string.Format("{0}/{1}", maxBuyablecount - boughtCount, maxBuyablecount);
            }
            else
            {
                countTextObj.gameObject.SetActive(true);
                //_buyableCount_dia.gameObject.SetActive(true);
                diaImage.SetActive(true);

                _buyableCount_dia.text = string.Format("{0}/{1}", maxBuyablecount - boughtCount, maxBuyablecount);
                _buyableCount_ad.text = string.Format(productTableData.diaCost.ToString());
            }

 

            button.onClick.AddListener(Purchase);
        }

        public void AmountUpdate()
        {
            if (productTableData.goodsType == GoodsKey.AwakeStone)
            {
                awakeStoneAmount = Player.Unit.SwordAtk + Player.Unit.BowAtk;
                _goodAmount.text = awakeStoneAmount.ToNumberString();
            }
            else
            {
                _goodAmount.text = productTableData.rewardAmount.ToString();
            }
        }
        public void Purchase()
        {
            if (Player.Cloud.adShopProductData.boughtProductList[productIndex].boughtCount < productTableData.dailyBuyableCount)
            {
                if(productTableData.isAd)
                {
                    if (Player.Cloud.inAppPurchase.purchaseAds)
                    {
                        if (productTableData.goodsType == GoodsKey.AwakeStone)
                        {
                            Player.ControllerGood.Earn(productTableData.goodsType, awakeStoneAmount);
                        }
                        else
                        {
                            Player.ControllerGood.Earn(productTableData.goodsType, productTableData.rewardAmount);
                        }
                        Player.Cloud.adShopProductData.boughtProductList[productIndex].boughtCount++;

                        string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.UI_GoodsRewarded].StringToLocal;
                        ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                        SyncAfterPurchase();
                    }
                    else
                    {
                        AdmobManager.Instance.ShowRewardedAd(() => {
                            if (productTableData.goodsType == GoodsKey.AwakeStone)
                            {
                                Player.ControllerGood.Earn(productTableData.goodsType, awakeStoneAmount);
                            }
                            else
                            {
                                Player.ControllerGood.Earn(productTableData.goodsType, productTableData.rewardAmount);
                            }
                            Player.Cloud.adShopProductData.boughtProductList[productIndex].boughtCount++;

                            string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.UI_GoodsRewarded].StringToLocal;
                            ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                            SyncAfterPurchase();
                        });
                    }
                }
                else
                {
                    if(Player.ControllerGood.IsCanBuy(GoodsKey.Dia,productTableData.diaCost)==false)
                    {
                        string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_DiaNotEnough].StringToLocal;
                        ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                    }
                    else
                    {
                        Player.ControllerGood.Consume(GoodsKey.Dia, productTableData.diaCost);

                        if(productTableData.goodsType==GoodsKey.AwakeStone)
                        {
                            Player.ControllerGood.Earn(productTableData.goodsType, awakeStoneAmount);
                        }
                        else
                        {
                            Player.ControllerGood.Earn(productTableData.goodsType, productTableData.rewardAmount);
                        }
                        
                        Player.Cloud.adShopProductData.boughtProductList[productIndex].boughtCount++;

                        string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.UI_GoodsRewarded].StringToLocal;
                        ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                        SyncAfterPurchase();
                    }
                }

                Player.Cloud.adShopProductData.UpdateHash().SetDirty(true);
            }
        }

        public void SyncAfterPurchase()
        {
            int boughtCount = Player.Cloud.adShopProductData.boughtProductList[productIndex].boughtCount;
            int maxBuyablecount = productTableData.dailyBuyableCount;

            if (productTableData.isAd)
            {
                _buyableCount_ad.text = string.Format("{0}/{1}", maxBuyablecount - boughtCount, maxBuyablecount);
            }
            else
            {
                _buyableCount_dia.text = string.Format("{0}/{1}", maxBuyablecount - boughtCount, maxBuyablecount);
            }
        }
    }

}
