using BlackTree.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BlackTree.Model;
using System;
using BlackTree.Definition;

namespace BlackTree.Bundles
{
    public class ViewShopPackageItem : MonoBehaviour
    {
        [SerializeField] private Image bgImage;
        [SerializeField] private TMP_Text title;

        [SerializeField] private ScrollRect rewardslotParent;
        //[SerializeField] private Image boxImage;
        [SerializeField] private Image characterImage;
        [SerializeField] private TMP_Text _priceValue;
        [SerializeField] private BTButton button;
        [SerializeField] private TMP_Text buyableCount;
        DataPackage productTableData;

        [SerializeField] private GameObject discountObj;
        [SerializeField] private TMP_Text discountText;
        public void Init(DataPackage _data)
        {
            productTableData = _data;

            bgImage.sprite = GoodResourcesBundle.Loaded.packageBackgroundSprites[productTableData.backgroundImage];
            title.text = string.Format(StaticData.Wrapper.localizednamelist[(int)productTableData.title].StringToLocal);

            for(int i=0; i< productTableData.rewardTypes.Length; i++)
            {
                var slotObj = Instantiate(GoodResourcesBundle.Loaded.packageRewardslotPrefab);
                slotObj.goodsIconFrame.sprite = GoodResourcesBundle.Loaded.rewardSlotBGFrame[(int)productTableData.rewardTypes[i]];
                slotObj.goodsIcon.sprite = GoodResourcesBundle.Loaded.rewardSprites[(int)productTableData.rewardTypes[i]];

                LocalizeNameKeys nameKey = Player.ControllerGood.RewardTolocalize(productTableData.rewardTypes[i]);
                if(nameKey!=LocalizeNameKeys.None)
                {
                    string objName = StaticData.Wrapper.localizednamelist[(int)nameKey].StringToLocal;
                    slotObj.goodsDesc.text = objName;
                }
                else
                {
                    slotObj.goodsDesc.text = "";
                }
                  
                slotObj.goodValue.text = productTableData.rewardAmounts[i].ToNumberString();

                slotObj.transform.SetParent(rewardslotParent.content, false);
            }
            //boxImage.sprite = GoodResourcesBundle.Loaded.packageBoxSprites[productTableData.iconImageIndex];
            characterImage.sprite= GoodResourcesBundle.Loaded.packageCharacterImages[productTableData.iconImageIndex].images[0];
            characterImage.GetComponent<RectTransform>().sizeDelta = new Vector2(GoodResourcesBundle.Loaded.packageCharacterImages[productTableData.iconImageIndex].width, GoodResourcesBundle.Loaded.packageCharacterImages[productTableData.iconImageIndex].height);
            characterImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(GoodResourcesBundle.Loaded.packageCharacterImages[productTableData.iconImageIndex].posX, GoodResourcesBundle.Loaded.packageCharacterImages[productTableData.iconImageIndex].posY);

#if UNITY_ANDROID
            _priceValue.text = IAPManager.Instance.GetLocalPriceString(productTableData.productId);
#elif UNITY_IOS
            _priceValue.text = IAPManager.Instance.GetLocalPriceString(productTableData.productId_IOS);
#endif

            VipPurchaseCallback();

            Player.Products.PackagePurchaseCallback += VipPurchaseCallback;

            spriteIndex = 0;
            frameTime = 0;

            if(productTableData.increaseRate>0)
            {
                discountObj.SetActive(true);
                discountText.text =string.Format(StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Package_IncreaseRate].StringToLocal,productTableData.increaseRate);
            }
            else
            {
                discountObj.SetActive(false);
            }
        }

        int spriteIndex = 0;
        float frameTime = 0;
        private void Update()
        {
            if (this.gameObject.activeInHierarchy == false)
                return;

            characterImage.sprite = GoodResourcesBundle.Loaded.packageCharacterImages[productTableData.iconImageIndex].images[spriteIndex];
            frameTime += Time.deltaTime;
            if(frameTime>=0.08f)
            {
                frameTime = 0;
                spriteIndex++;
            }
            
            if(spriteIndex>= GoodResourcesBundle.Loaded.packageCharacterImages[productTableData.iconImageIndex].images.Length)
            {
                spriteIndex = 0;
            }
        }

        public void VipPurchaseCallback()
        {
            if (productTableData.key == ProductKey.package_vip_0)
            {
                this.gameObject.SetActive(false);
                if (Player.Cloud.inAppPurchase.purchaseVip == 0)
                {
                    this.gameObject.SetActive(true);
                }
            }

            if (productTableData.key == ProductKey.package_vip_1)
            {
                this.gameObject.SetActive(false);
                if (Player.Cloud.inAppPurchase.purchaseVip == 1)
                {
                    this.gameObject.SetActive(true);
                }
            }

            if (productTableData.key == ProductKey.package_vip_2)
            {
                this.gameObject.SetActive(false);
                if (Player.Cloud.inAppPurchase.purchaseVip == 2)
                {
                    this.gameObject.SetActive(true);
                }
            }
            var packageCache = Player.Products.GetPackage(productTableData.key);
            if(packageCache._userPurchase.currBoughtCount>=productTableData.buyableCount)
            {
                this.gameObject.SetActive(false);
            }
            else
            {
                buyableCount.text = string.Format("{0}/{1}", productTableData.buyableCount - packageCache._userPurchase.currBoughtCount, productTableData.buyableCount);
            }
        }

        public ViewShopPackageItem SetButton(Action action)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(action.Invoke);
            return this;
        }
    }

}
