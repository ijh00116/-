using BlackTree.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BlackTree.Model;
using System;
using BlackTree.Definition;

namespace BlackTree.Bundles
{
    public class ViewShopItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text title;
        [SerializeField] private Image bgImage;
        [SerializeField] private Image bgGoodsImage;

        [SerializeField] private Image rewardIconImage;
        [SerializeField] private TMP_Text _goodValue;
        [SerializeField] private TMP_Text _priceValue;
        [SerializeField]private BTButton button;
        [SerializeField] private Image adImageInBtn;

        [SerializeField] private GameObject possiblePurchaseCountObj;
        [SerializeField] private TMP_Text possiblePurchaseCount;

        [SerializeField] private GameObject discountObj;
        [SerializeField] private TMP_Text discountText;

        DataInAppProduct productTableData;
        public void Init(DataInAppProduct _data)
        {
            productTableData = _data;

            title.text = StaticData.Wrapper.localizednamelist[(int)productTableData.title].StringToLocal;

            bgImage.sprite = GoodResourcesBundle.Loaded.productBackgroundSprites[productTableData.backgroundImage];
            bgGoodsImage.sprite = GoodResourcesBundle.Loaded.productGoodsSprites[productTableData.goodsImage];

            rewardIconImage.sprite = GoodResourcesBundle.Loaded.rewardSprites[(int)productTableData.rewardTypes];
            _goodValue.text = string.Format(StaticData.Wrapper.localizeddesclist[(int)productTableData.desc].StringToLocal, productTableData.rewardAmounts);

            adImageInBtn.gameObject.SetActive(productTableData.isAd);
            if (productTableData.isAd)
            {
                string localized = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_ShowAd].StringToLocal;
                _priceValue.text = localized;
                if (productTableData.key==ProductKey.dia_ad)
                {
                    _priceValue.text = string.Format("{0}/{1}", (Player.Products.diaADPurchaseMaxCount-Player.Cloud.inAppPurchase.adDiapurchaseCount),Player.Products.diaADPurchaseMaxCount);
                }
                if (productTableData.key == ProductKey.skillAwake_ad)
                {
                    _priceValue.text = string.Format("{0}/{1}", (Player.Products.awakeStoneADPurchaseMaxCount - Player.Cloud.inAppPurchase.adSkillAwakepurchaseCount), Player.Products.awakeStoneADPurchaseMaxCount);
                }
            }
            else
            {
                _priceValue.text = IAPManager.Instance.GetLocalPriceString(productTableData.productId);
            }

            if(productTableData.discount>0)
            {
                discountObj.SetActive(true);
                discountText.text = string.Format("-{0}%", productTableData.discount);
            }
            else
            {
                discountObj.SetActive(false);
            }

            Player.Products.adPurchaseCallback += AdUICallback;
            Player.Products.inAppProductPurchaseCallback += ProductPurchaseCallback;

            ProductPurchaseCallback();
        }

        void AdUICallback()
        {
            if (productTableData.isAd)
            {
                string localized = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_ShowAd].StringToLocal;
                _priceValue.text = localized;
                if (productTableData.key == ProductKey.dia_ad)
                {
                    _priceValue.text = string.Format("{0}/{1}", (Player.Products.diaADPurchaseMaxCount - Player.Cloud.inAppPurchase.adDiapurchaseCount), Player.Products.diaADPurchaseMaxCount);
                }
                if (productTableData.key == ProductKey.skillAwake_ad)
                {
                    _priceValue.text = string.Format("{0}/{1}", (Player.Products.awakeStoneADPurchaseMaxCount - Player.Cloud.inAppPurchase.adSkillAwakepurchaseCount), Player.Products.awakeStoneADPurchaseMaxCount);
                }
            }
        }

        void ProductPurchaseCallback()
        {
            var inappCache = Player.Products.GetProduct(productTableData.key);
            if(productTableData.buyableCount>0)
            {
                possiblePurchaseCountObj.SetActive(true);
                if (inappCache.BoughtCount< productTableData.buyableCount)
                {
                    this.gameObject.SetActive(true);
                    possiblePurchaseCount.text = string.Format("{0}/{1}", (productTableData.buyableCount - inappCache.BoughtCount), productTableData.buyableCount);
                }
                else
                {
                    this.gameObject.SetActive(false);
                }
            }
            else
            {
                possiblePurchaseCountObj.SetActive(false);
            }
        }


        public ViewShopItem SetButton(Action action)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(action.Invoke);
            return this;
        }
    }
}
