using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using BlackTree.Definition;
using Cysharp.Threading.Tasks;
using System;
using static BlackTree.Model.Player;

namespace BlackTree.Core
{
    public class ControllerPackagePopup
    {
        private readonly ViewCanvasPackagePopup _view;
        private readonly CancellationTokenSource _cts;

        List<ViewGoodRewardSlot> rewardSlotList = new List<ViewGoodRewardSlot>();
        public ControllerPackagePopup(Transform parent, CancellationTokenSource cts)
        {
            _cts = cts;
            _view = ViewCanvas.Create<ViewCanvasPackagePopup>(parent);
            _view.SetVisible(false);

            Player.Products.skillPopup += SkillPackagePopup;

            for(int i=0; i< _view.closeBtn.Length; i++)
            {
                _view.closeBtn[i].onClick.AddListener(() => { _view.SetVisible(false); });
            }
            Main().Forget();
        }

        int spriteIndex = 0;
        async UniTask Main()
        {
            while (true)
            {
                if(_view.gameObject.activeInHierarchy)
                {
                    spriteIndex++;
                    if (spriteIndex >= _view.animSprites.Length)
                    {
                        spriteIndex = 0;
                    }
                    _view.characterAnimImage.sprite = _view.animSprites[spriteIndex];
                }
                await UniTask.Delay(100);
            }
        }
        void SkillPackagePopup(SkillKey _key)
        {
            bool canPopup=false;
            SkillDataPackage skillPackage=null;
            for(int i=0; i< StaticData.Wrapper.skillPackages.Length; i++)
            {
                if(StaticData.Wrapper.skillPackages[i].packageSkillkey==_key)
                {
                    canPopup = true;
                    skillPackage = StaticData.Wrapper.skillPackages[i];
                }
            }
            if (canPopup==false)
                return;
            var product=Player.Products.GetSkillPackage(skillPackage.key);

            if (product._userPurchase.isSkillWindowPopuped)
                return;

            product._userPurchase.isSkillWindowPopuped = true;

            _view.SetVisible(true);

            DateTime expiredDay = System.DateTime.Now.AddDays(skillPackage.limitDay);
            product._userPurchase.expiredDay = expiredDay.ToIsoString();

            _view.titleDesc.text =StaticData.Wrapper.localizednamelist[(int)skillPackage.title].StringToLocal;

            for(int i=0; i< skillPackage.rewardTypes.Length; i++)
            {
                ViewGoodRewardSlot obj = null;
                if (i< rewardSlotList.Count)
                {
                    obj = rewardSlotList[i];
                }
                else
                {
                    obj = UnityEngine.Object.Instantiate(_view.rewardSlotPrefab);
                    obj.transform.SetParent(_view.rewardSlotParent, false);
                    rewardSlotList.Add(obj);
                }
                if ((int)skillPackage.rewardTypes[i] >= (int)RewardTypes.package_swordfewhit
                    && (int)skillPackage.rewardTypes[i] <= (int)RewardTypes.package_multielectric)
                {
                    obj.goodValue.text = skillPackage.rewardAmounts[i].ToNumberString();
                    obj.goodsIcon.sprite = InGameResourcesBundle.Loaded.skillIcon[(int)skillPackage.packageSkillkey];
                }
                else if ((int)skillPackage.rewardTypes[i] == (int)RewardTypes.package_skyLight)
                {
                    obj.goodValue.text = skillPackage.rewardAmounts[i].ToNumberString();
                    obj.goodsIcon.sprite = InGameResourcesBundle.Loaded.skillIcon[(int)skillPackage.packageSkillkey];
                }
                else
                {
                    obj.goodValue.text = skillPackage.rewardAmounts[i].ToNumberString();
                    obj.goodsIcon.sprite = GoodResourcesBundle.Loaded.rewardSprites[(int)skillPackage.rewardTypes[i]];
                }
            }

#if UNITY_ANDROID
            _view.btnText.text = IAPManager.Instance.GetLocalPriceString(skillPackage.productId);

#elif UNITY_IOS
            _view.btnText.text = IAPManager.Instance.GetLocalPriceString(skillPackage.productId_IOS);
#endif

            string localized = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_BuyPossible].StringToLocal;
            _view.expiredDay.text = string.Format($"~{expiredDay.Month}/{expiredDay.Day} {localized}");
            _view.purchaseBtn.onClick.RemoveAllListeners();
            _view.purchaseBtn.onClick.AddListener(()=> {
                Purchase(skillPackage);
            });

            Player.Products.skillPackageCallback?.Invoke();

            Player.Cloud.inAppSkillPurchase.UpdateHash().SetDirty(true);
            LocalSaveLoader.SaveUserCloudData();
        }

        void Purchase(SkillDataPackage skilldataPackage)
        {
            Player.Products.PurchaseInApp(skilldataPackage.key, ()=> {
                PurchaseCallback(skilldataPackage).Forget();
            });
        }

        private async UniTask PurchaseCallback(SkillDataPackage skilldataPackage)
        {
            _view.SetVisible(false);
            await UniTask.Delay(300);

            string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_purchasedThanks].StringToLocal;
            ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);

            for (int i = 0; i < skilldataPackage.rewardTypes.Length; i++)
            {
                if ((int)skilldataPackage.rewardTypes[i] >= (int)RewardTypes.package_swordfewhit
                 && (int)skilldataPackage.rewardTypes[i] <= (int)RewardTypes.package_multielectric)
                {
                    SkillKey skill = Player.Skill.RewardToSkill(skilldataPackage.rewardTypes[i]);
                    Player.Skill.Obtain(skill, (int)skilldataPackage.rewardAmounts[i]);
                }
                else if ((int)skilldataPackage.rewardTypes[i] == (int)RewardTypes.package_skyLight)
                {
                    SkillKey skill = Player.Skill.RewardToSkill(skilldataPackage.rewardTypes[i]);
                    Player.Skill.Obtain(skill, (int)skilldataPackage.rewardAmounts[i]);
                }
                else
                {
                    GoodsKey good = Player.ControllerGood.RewardToGoods(skilldataPackage.rewardTypes[i]);
                    Player.ControllerGood.Earn(good, (int)skilldataPackage.rewardAmounts[i]);
                }
            }
            RewardToastPopup(skilldataPackage.key);
        }

        enum PurchaseType
        {
            InappProduct,
            Package,
            SkillPackage,
        }

        public void RewardToastPopup(ProductKey _key)
        {
            InAppCache inAppCache = null;
            PackageCache packageCache = null;
            SkillPackageCache skillPackageCache = null;

            PurchaseType productType;

            if ((int)_key < 1000)
            {
                inAppCache = Player.Products.GetProduct(_key);
                productType = PurchaseType.InappProduct;
            }
            else if ((int)_key < 2000)
            {
                packageCache = Player.Products.GetPackage(_key);
                productType = PurchaseType.Package;
            }
            else
            {
                skillPackageCache = Player.Products.GetSkillPackage(_key);
                productType = PurchaseType.SkillPackage;
            }
            //Debug.LogError("토스트 메세지 리워드 오픈팝업 ");
            var toastCanvas = ViewCanvas.Get<ViewCanvasToastMessage>();

            switch (productType)
            {
                case PurchaseType.InappProduct:
                    for (int i = 0; i < toastCanvas.rewardSlotList.Count; i++)
                    {
                        toastCanvas.rewardSlotList[i].gameObject.SetActive(false);
                    }

                    ViewGoodRewardSlot slotObj_0 = null;
                    if (0 < toastCanvas.rewardSlotList.Count)
                    {
                        slotObj_0 = toastCanvas.rewardSlotList[0];
                    }
                    else
                    {
                        slotObj_0 = UnityEngine.Object.Instantiate(toastCanvas.rewardSlotPrefab);
                        slotObj_0.transform.SetParent(toastCanvas.rewardParent, false);
                        toastCanvas.rewardSlotList.Add(slotObj_0);
                    }
                    slotObj_0.goodValue.text = inAppCache._dataInAppProduct.rewardAmounts.ToNumberString();
                    slotObj_0.goodsIcon.sprite = GoodResourcesBundle.Loaded.rewardSprites[(int)inAppCache._dataInAppProduct.rewardTypes];
                    slotObj_0.gameObject.SetActive(true);

                    toastCanvas.titleDesc.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_GoodsReward].StringToLocal;
                    toastCanvas.RewardPopupShowandFade();
                    break;
                case PurchaseType.Package:
                    for (int i = 0; i < toastCanvas.rewardSlotList.Count; i++)
                    {
                        toastCanvas.rewardSlotList[i].gameObject.SetActive(false);
                    }
                    for (int i = 0; i < packageCache._dataPackage.rewardTypes.Length; i++)
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
                        slotObj.goodValue.text = packageCache._dataPackage.rewardAmounts[i].ToNumberString();
                        slotObj.goodsIcon.sprite = GoodResourcesBundle.Loaded.rewardSprites[(int)packageCache._dataPackage.rewardTypes[i]];
                        slotObj.gameObject.SetActive(true);
                    }
                    toastCanvas.titleDesc.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_GoodsReward].StringToLocal;
                    toastCanvas.RewardPopupShowandFade();
                    break;
                case PurchaseType.SkillPackage:
                    for (int i = 0; i < toastCanvas.rewardSlotList.Count; i++)
                    {
                        toastCanvas.rewardSlotList[i].gameObject.SetActive(false);
                    }
                    for (int i = 0; i < skillPackageCache._dataPackage.rewardTypes.Length; i++)
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
                        slotObj.goodValue.text = skillPackageCache._dataPackage.rewardAmounts[i].ToNumberString();
                        slotObj.goodsIcon.sprite = GoodResourcesBundle.Loaded.rewardSprites[(int)skillPackageCache._dataPackage.rewardTypes[i]];
                        slotObj.gameObject.SetActive(true);
                    }
                    toastCanvas.titleDesc.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_GoodsReward].StringToLocal;
                    toastCanvas.RewardPopupShowandFade();
                    break;
                default:
                    break;
            }
        }
    }
}

