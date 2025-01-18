using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using BlackTree.Definition;
using System;
using Cysharp.Threading.Tasks;
using static BlackTree.Model.Player;

namespace BlackTree.Core
{
    public class ControllerInAppShop
    {
        public static int Index => _index;
        private static readonly int _index = 5;

        private readonly ViewCanvasInAppShop _viewCanvasShop;
        private readonly CancellationTokenSource _cts;

        List<ViewShopSkillPackageItem> skillPackageslotList = new List<ViewShopSkillPackageItem>();
        public ControllerInAppShop(Transform parent, CancellationTokenSource cts)
        {
            _cts = cts;
            _viewCanvasShop = ViewCanvas.Create<ViewCanvasInAppShop>(parent);
            _viewCanvasShop.SetVisible(false);

            _viewCanvasShop.openProductwindowBtn.onClick.AddListener(TouchProductBtn);
            _viewCanvasShop.openPackagewindowBtn.onClick.AddListener(TouchPackageBtn);

            for(int i=0; i<StaticData.Wrapper.inAppProducts.Length; i++)
            {
                int index = i;
                ProductKey _key= StaticData.Wrapper.inAppProducts[index].key;
                var productObj = UnityEngine.Object.Instantiate(_viewCanvasShop.productItemPrefab);
                if(StaticData.Wrapper.inAppProducts[index].rewardTypes==RewardTypes.Dia)
                {
                    productObj.transform.SetParent(_viewCanvasShop.diaProductParent, false);
                }
                else
                {
                    productObj.transform.SetParent(_viewCanvasShop.awakeProductParent, false);
                }
                
                productObj.Init(StaticData.Wrapper.inAppProducts[index]);

                productObj.SetButton(() => {
                    if (_key == ProductKey.dia_ad)
                    {
                        if(Player.Cloud.inAppPurchase.adDiapurchaseCount< Player.Products.diaADPurchaseMaxCount)
                        {
                            Purchase(_key, () => { PurchaseCallback(_key).Forget(); });
                        }
                    }
                    else if (_key == ProductKey.skillAwake_ad)
                    {
                        if (Player.Cloud.inAppPurchase.adSkillAwakepurchaseCount < Player.Products.awakeStoneADPurchaseMaxCount)
                        {
                            Purchase(_key, () => { PurchaseCallback(_key).Forget(); });
                        }
                    }
                    else
                    {
                        Purchase(_key, () => { PurchaseCallback(_key).Forget(); });
                    }
                });
            }

            for (int i = 0; i < StaticData.Wrapper.packages.Length; i++)
            {
                int index = i;
                ProductKey _key = StaticData.Wrapper.packages[index].key;

                if(_key==ProductKey.package_vip_0 || _key == ProductKey.package_vip_1|| _key == ProductKey.package_vip_2)
                {
                    var productObj = UnityEngine.Object.Instantiate(_viewCanvasShop.packageVIPItemPrefab);
                    productObj.transform.SetParent(_viewCanvasShop.packageParent.content, false);
                    productObj.Init(StaticData.Wrapper.packages[index]);

                    productObj.SetButton(() => {
                        Purchase(_key,
                            () => {
                                PurchaseCallback(_key).Forget();
                            });
                    });
                }
                else
                {
                    var productObj = UnityEngine.Object.Instantiate(_viewCanvasShop.packageItemPrefab);
                    productObj.transform.SetParent(_viewCanvasShop.packageParent.content, false);
                    productObj.Init(StaticData.Wrapper.packages[index]);

                    productObj.SetButton(() => {
                        Purchase(_key,
                            () => {
                                PurchaseCallback(_key).Forget();
                            });
                    });
                }
           
            }

            for (int i = 0; i < StaticData.Wrapper.skillPackages.Length; i++)
            {
                int index = i;
                ProductKey _key = StaticData.Wrapper.skillPackages[index].key;

                var productObj = UnityEngine.Object.Instantiate(_viewCanvasShop.packageskillItemPrefab);
                productObj.transform.SetParent(_viewCanvasShop.packageParent.content, false);
                productObj.Init(StaticData.Wrapper.skillPackages[index]);

                productObj.SetButton(() => {
                    Purchase(_key,
                        () => {
                            PurchaseCallback(_key).Forget();
                        });
                });

                productObj.gameObject.SetActive(false);

                var product = Player.Products.GetSkillPackage(_key);
                if(string.IsNullOrEmpty(product._userPurchase.expiredDay)==false && product._userPurchase.isPurchased==false)
                {
                    productObj.gameObject.SetActive(true);
                    productObj.SkillPackageUpdateCallback();
                }
                skillPackageslotList.Add(productObj);
            }

            if (Player.Option.isAnotherDay())
            {
                Player.Cloud.inAppPurchase.adDiapurchaseCount = 0;
                Player.Cloud.inAppPurchase.adSkillAwakepurchaseCount= 0;
            }

            MainNav.onChange += UpdateViewVisible;

            for (int i = 0; i < _viewCanvasShop.closeBtn.Length; i++)
            {
                int index = i;
                _viewCanvasShop.closeBtn[index].onClick.AddListener(() => MainNav.CloseMainUIWindow());
            }

            _viewCanvasShop.goodsTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_GoodsForShop].StringToLocal; 
            _viewCanvasShop.goodsTxt_on.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_GoodsForShop].StringToLocal;
            _viewCanvasShop.packageTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_SpecialForShop].StringToLocal;
            _viewCanvasShop.packageTxt_on.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_SpecialForShop].StringToLocal;

            Player.Option.AnotherDaySetting += AnotherDaySetting;
        }

        void AnotherDaySetting()
        {
            Player.Cloud.inAppPurchase.adDiapurchaseCount = 0;
            Player.Cloud.inAppPurchase.adSkillAwakepurchaseCount = 0;

            Player.Products.adPurchaseCallback?.Invoke();
        }

        void TouchProductBtn()
        {
            _viewCanvasShop.ActiveOffScrollRect();
            _viewCanvasShop.buttonSelector.Show(0);
            _viewCanvasShop.productWindow.SetActive(true);
        }

        void TouchPackageBtn()
        {
            _viewCanvasShop.ActiveOffScrollRect();
            _viewCanvasShop.buttonSelector.Show(1);
            _viewCanvasShop.packageWindow.SetActive(true);
        }

        private void Purchase(ProductKey key, Action callback = null)
        {
            switch (key)
            {
                case ProductKey.dia_ad:
                    if (Player.Cloud.inAppPurchase.adDiapurchaseCount < Player.Products.diaADPurchaseMaxCount)
                    {
                        var inAppCache = Player.Products.GetProduct(key);
                        if (Player.Cloud.inAppPurchase.purchaseAds)
                        {
                            Player.ControllerGood.Earn(GoodsKey.Dia, inAppCache._dataInAppProduct.rewardAmounts);
                            Player.Cloud.inAppPurchase.adDiapurchaseCount++;
                            Player.Products.adPurchaseCallback?.Invoke();
                            Player.Cloud.inAppPurchase.UpdateHash().SetDirty(true);
                            callback?.Invoke();
                        }
                        else
                        {
                            AdmobManager.Instance.ShowRewardedAd(() => {
                                Player.ControllerGood.Earn(GoodsKey.Dia, inAppCache._dataInAppProduct.rewardAmounts);
                                Player.Cloud.inAppPurchase.adDiapurchaseCount++;
                                Player.Products.adPurchaseCallback?.Invoke();
                                Player.Cloud.inAppPurchase.UpdateHash().SetDirty(true);
                                callback?.Invoke();
                            });
                        }
                    }
                    break;
                case ProductKey.dia_0:
                    Player.Products.PurchaseInApp(key, callback);
                    break;
                case ProductKey.dia_1:
                    Player.Products.PurchaseInApp(key, callback);
                    break;
                case ProductKey.dia_2:
                    Player.Products.PurchaseInApp(key, callback);
                    break;
                case ProductKey.dia_0_first:
                    Player.Products.PurchaseInApp(key, callback);
                    break;
                case ProductKey.dia_1_first:
                    Player.Products.PurchaseInApp(key, callback);
                    break;
                case ProductKey.dia_2_first:
                    Player.Products.PurchaseInApp(key, callback);
                    break;
                case ProductKey.skillAwake_ad:
                    if (Player.Cloud.inAppPurchase.adSkillAwakepurchaseCount < Player.Products.awakeStoneADPurchaseMaxCount)
                    {
                        var inAppCache = Player.Products.GetProduct(key);
                        if (Player.Cloud.inAppPurchase.purchaseAds)
                        {
                            Player.ControllerGood.Earn(GoodsKey.SkillAwakeStone, inAppCache._dataInAppProduct.rewardAmounts);
                            Player.Cloud.inAppPurchase.adSkillAwakepurchaseCount++;
                            Player.Cloud.inAppPurchase.UpdateHash().SetDirty(true);
                            Player.Products.adPurchaseCallback?.Invoke();
                            callback?.Invoke();
                        }
                        else
                        {
                            AdmobManager.Instance.ShowRewardedAd(() => {
                                Player.ControllerGood.Earn(GoodsKey.SkillAwakeStone, inAppCache._dataInAppProduct.rewardAmounts);
                                Player.Cloud.inAppPurchase.adSkillAwakepurchaseCount++;
                                Player.Cloud.inAppPurchase.UpdateHash().SetDirty(true);
                                Player.Products.adPurchaseCallback?.Invoke();
                                callback?.Invoke();
                            });
                        }
                    }
                    break;
                case ProductKey.package_starter_0:
                    Player.Products.PurchaseInApp(key, callback);
                    break;
                case ProductKey.package_adremove_0:
                    Player.Products.PurchaseInApp(key, callback);
                    break;
                case ProductKey.package_vip_0:
                    Player.Products.PurchaseInApp(key, callback);
                    break;
                case ProductKey.package_vip_1:
                    Player.Products.PurchaseInApp(key, callback);
                    break;
                case ProductKey.package_vip_2:
                    Player.Products.PurchaseInApp(key, callback);
                    break;
                case ProductKey.package_swordfewhit:
                    Player.Products.PurchaseInApp(key, callback);
                    break;
                case ProductKey.package_magicfewhit:
                    Player.Products.PurchaseInApp(key, callback);
                    break;
                case ProductKey.package_setturret:
                    Player.Products.PurchaseInApp(key, callback);
                    break;
                case ProductKey.package_companionspawn:
                    Player.Products.PurchaseInApp(key, callback);
                    break;
                case ProductKey.package_guidedmissile:
                    Player.Products.PurchaseInApp(key, callback);
                    break;
                case ProductKey.package_godmode:
                    Player.Products.PurchaseInApp(key, callback);
                    break;
                case ProductKey.package_summon:
                    Player.Products.PurchaseInApp(key, callback);
                    break;
                case ProductKey.package_nova:
                    Player.Products.PurchaseInApp(key, callback);
                    break;
                case ProductKey.package_meteor:
                    Player.Products.PurchaseInApp(key, callback);
                    break;
                case ProductKey.package_multielectric:
                    Player.Products.PurchaseInApp(key, callback);
                    break;
                case ProductKey.package_SkillAwakeStone_0:
                    Player.Products.PurchaseInApp(key, callback);
                    break;
                case ProductKey.package_SkillAwakeStone_1:
                    Player.Products.PurchaseInApp(key, callback);
                    break;
                case ProductKey.package_RiftDungeonKey_0:
                    Player.Products.PurchaseInApp(key, callback);
                    break;
                case ProductKey.package_skyLight:
                    Player.Products.PurchaseInApp(key, callback);
                    break;
                case ProductKey.package_SkillAwakeStone_2:
                    Player.Products.PurchaseInApp(key, callback);
                    break;
                case ProductKey.package_SkillAwakeStone_3:
                    Player.Products.PurchaseInApp(key, callback);
                    break;
                default:
                    break;
            }

        }

        private void UpdateViewVisible()
        {
            if (MainNav.SelectTabIndex == _index)
            {
                if (_viewCanvasShop.IsVisible)
                {
                    _viewCanvasShop.blackBG.PopupCloseColorFade();
                    _viewCanvasShop.Wrapped.CommonPopupCloseAnimationDown(() => {
                        _viewCanvasShop.SetVisible(false);
                    });
                }
                else
                {
                    _viewCanvasShop.SetVisible(true);

                    _viewCanvasShop.blackBG.PopupOpenColorFade();
                    _viewCanvasShop.Wrapped.CommonPopupOpenAnimationUp();
                }
            }
            else
            {
                _viewCanvasShop.blackBG.PopupCloseColorFade();
                _viewCanvasShop.Wrapped.CommonPopupCloseAnimationDown(() => {
                    _viewCanvasShop.SetVisible(false);
                });
            }
        }

        enum PurchaseType
        {
            InappProduct,
            Package,
            SkillPackage,
        }

        private async UniTask PurchaseCallback(ProductKey _key)
        {
            await UniTask.Delay(300);

            InAppCache inAppCache = null;
            PackageCache packageCache = null;
            SkillPackageCache skillPackageCache = null;

            PurchaseType productType;

            if ((int)_key<1000)
            {
                inAppCache = Player.Products.GetProduct(_key);
                productType = PurchaseType.InappProduct;
            }
            else if((int)_key<2000)
            {
                packageCache = Player.Products.GetPackage(_key);
                productType = PurchaseType.Package;
            }
            else
            {
                skillPackageCache = Player.Products.GetSkillPackage(_key);
                productType = PurchaseType.SkillPackage;
            }


            switch (_key)
            {
                case ProductKey.dia_ad:
                    break;
                case ProductKey.dia_0:
                    Player.ControllerGood.Earn(GoodsKey.Dia, inAppCache._dataInAppProduct.rewardAmounts);
                    break;
                case ProductKey.dia_1:
                    Player.ControllerGood.Earn(GoodsKey.Dia, inAppCache._dataInAppProduct.rewardAmounts);
                    break;
                case ProductKey.dia_2:
                    Player.ControllerGood.Earn(GoodsKey.Dia, inAppCache._dataInAppProduct.rewardAmounts);
                    break;
                case ProductKey.dia_0_first:
                    Player.ControllerGood.Earn(GoodsKey.Dia, inAppCache._dataInAppProduct.rewardAmounts);
                    break;
                case ProductKey.dia_1_first:
                    Player.ControllerGood.Earn(GoodsKey.Dia, inAppCache._dataInAppProduct.rewardAmounts);
                    break;
                case ProductKey.dia_2_first:
                    Player.ControllerGood.Earn(GoodsKey.Dia, inAppCache._dataInAppProduct.rewardAmounts);
                    break;
                case ProductKey.skillAwake_ad:
                  
                    break;
                case ProductKey.package_starter_0:
                    for(int i=0; i< packageCache.RewardTypes.Length; i++)
                    {
                        if ((int)packageCache.RewardTypes[i] >= (int)RewardTypes.Weapon_4_4
                      && (int)packageCache.RewardTypes[i] <= (int)RewardTypes.Armor_4_4)
                        {
                            if(packageCache.RewardTypes[i]==RewardTypes.Weapon_4_4)
                                Player.EquipItem.Obtain(EquipType.Weapon, Player.EquipItem.starterPackageItemIndex, 5);
                            if (packageCache.RewardTypes[i] == RewardTypes.Staff_4_4)
                                Player.EquipItem.Obtain(EquipType.Bow, Player.EquipItem.starterPackageItemIndex, 5);
                            if (packageCache.RewardTypes[i] == RewardTypes.Armor_4_4)
                                Player.EquipItem.Obtain(EquipType.Armor, Player.EquipItem.starterPackageItemIndex,5);
                        }
                        else
                        {
                            GoodsKey good = Player.ControllerGood.RewardToGoods(packageCache.RewardTypes[i]);
                            Player.ControllerGood.Earn(good, packageCache._dataPackage.rewardAmounts[i]);
                        }
                    }
                    break;
                case ProductKey.package_adremove_0:
                    for (int i = 0; i < packageCache.RewardTypes.Length; i++)
                    {
                        GoodsKey goodkey = Player.ControllerGood.RewardToGoods(packageCache.RewardTypes[i]);
                        if (goodkey != GoodsKey.None)
                        {
                            Player.ControllerGood.Earn(goodkey, packageCache._dataPackage.rewardAmounts[i]);
                        }
                    }
                    Player.Cloud.inAppPurchase.purchaseAds = true;
                    break;
                case ProductKey.package_vip_0:
                    for (int i = 0; i < packageCache.RewardTypes.Length; i++)
                    {
                        GoodsKey goodkey = Player.ControllerGood.RewardToGoods(packageCache.RewardTypes[i]);
                        if (goodkey != GoodsKey.None)
                        {
                            Player.ControllerGood.Earn(goodkey, packageCache._dataPackage.rewardAmounts[i]);
                        }
                    }
                    Player.Cloud.inAppPurchase.purchaseVip=1;
                    Player.Cloud.inAppPurchase.isVipDailyRewardGet = false;
                    break;
                case ProductKey.package_vip_1:
                    for (int i = 0; i < packageCache.RewardTypes.Length; i++)
                    {
                        GoodsKey goodkey = Player.ControllerGood.RewardToGoods(packageCache.RewardTypes[i]);
                        if (goodkey != GoodsKey.None)
                        {
                            Player.ControllerGood.Earn(goodkey, packageCache._dataPackage.rewardAmounts[i]);
                        }
                    }
                    Player.Cloud.inAppPurchase.purchaseVip=2;
                    Player.Cloud.inAppPurchase.isVipDailyRewardGet = false;
                    break;
                case ProductKey.package_vip_2:
                    for (int i = 0; i < packageCache.RewardTypes.Length; i++)
                    {
                        GoodsKey goodkey = Player.ControllerGood.RewardToGoods(packageCache.RewardTypes[i]);
                        if (goodkey != GoodsKey.None)
                        {
                            Player.ControllerGood.Earn(goodkey, packageCache._dataPackage.rewardAmounts[i]);
                        }
                    }
                    Player.Cloud.inAppPurchase.purchaseVip=3;
                    Player.Cloud.inAppPurchase.isVipDailyRewardGet = false;
                    break;
                case ProductKey.package_swordfewhit:
                case ProductKey.package_magicfewhit:
                case ProductKey.package_setturret:
                case ProductKey.package_companionspawn:
                case ProductKey.package_guidedmissile:
                case ProductKey.package_godmode:
                case ProductKey.package_summon:
                case ProductKey.package_nova:
                case ProductKey.package_meteor:
                case ProductKey.package_multielectric:
                case ProductKey.package_skyLight:
                    for (int i = 0; i < skillPackageCache.RewardTypes.Length; i++)
                    {
                        if ((int)skillPackageCache.RewardTypes[i] >= (int)RewardTypes.package_swordfewhit
                        && (int)skillPackageCache.RewardTypes[i] <= (int)RewardTypes.package_multielectric)
                        {
                            SkillKey skill = Player.Skill.RewardToSkill(skillPackageCache.RewardTypes[i]);
                            Player.Skill.Obtain(skill, (int)skillPackageCache._dataPackage.rewardAmounts[i]);
                        }
                        else if((int)skillPackageCache.RewardTypes[i] == (int)RewardTypes.package_skyLight)
                        {
                            SkillKey skill = Player.Skill.RewardToSkill(skillPackageCache.RewardTypes[i]);
                            Player.Skill.Obtain(skill, (int)skillPackageCache._dataPackage.rewardAmounts[i]);
                        }
                        else
                        {
                            GoodsKey good = Player.ControllerGood.RewardToGoods(skillPackageCache.RewardTypes[i]);
                            Player.ControllerGood.Earn(good, skillPackageCache._dataPackage.rewardAmounts[i]);
                        }
                    }
                    break;
                case ProductKey.package_SkillAwakeStone_0:
                    for (int i = 0; i < packageCache.RewardTypes.Length; i++)
                    {
                        GoodsKey goodkey = Player.ControllerGood.RewardToGoods(packageCache.RewardTypes[i]);
                        if (goodkey != GoodsKey.None)
                        {
                            Player.ControllerGood.Earn(goodkey, packageCache._dataPackage.rewardAmounts[i]);
                        }
                    }
                    break;
                case ProductKey.package_SkillAwakeStone_1:
                    for (int i = 0; i < packageCache.RewardTypes.Length; i++)
                    {
                        GoodsKey goodkey = Player.ControllerGood.RewardToGoods(packageCache.RewardTypes[i]);
                        if (goodkey != GoodsKey.None)
                        {
                            Player.ControllerGood.Earn(goodkey, packageCache._dataPackage.rewardAmounts[i]);
                        }
                    }
                    break;
                case ProductKey.package_RiftDungeonKey_0:
                    for (int i = 0; i < packageCache.RewardTypes.Length; i++)
                    {
                        GoodsKey goodkey = Player.ControllerGood.RewardToGoods(packageCache.RewardTypes[i]);
                        if (goodkey != GoodsKey.None)
                        {
                            Player.ControllerGood.Earn(goodkey, packageCache._dataPackage.rewardAmounts[i]);
                        }
                    }
                    Player.Cloud.inAppPurchase.purchaseAds = true;
                    break;
                case ProductKey.package_SkillAwakeStone_2:
                    for (int i = 0; i < packageCache.RewardTypes.Length; i++)
                    {
                        GoodsKey goodkey = Player.ControllerGood.RewardToGoods(packageCache.RewardTypes[i]);
                        if (goodkey != GoodsKey.None)
                        {
                            Player.ControllerGood.Earn(goodkey, packageCache._dataPackage.rewardAmounts[i]);
                        }
                    }
                    break;
                case ProductKey.package_SkillAwakeStone_3:
                    for (int i = 0; i < packageCache.RewardTypes.Length; i++)
                    {
                        GoodsKey goodkey = Player.ControllerGood.RewardToGoods(packageCache.RewardTypes[i]);
                        if (goodkey != GoodsKey.None)
                        {
                            Player.ControllerGood.Earn(goodkey, packageCache._dataPackage.rewardAmounts[i]);
                        }
                    }
                    break;
                default:
                    break;
            }
            Player.Products.PackagePurchaseCallback?.Invoke();
            Player.SaveUserDataToFirebaseAndLocal().Forget();

            RewardToastPopup(_key);
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

                    string localizedvalue = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_GoodsReward].StringToLocal;
                    toastCanvas.titleDesc.text = localizedvalue;
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
            //Debug.LogError("토스트 메세지 리워드 오픈팝업 함수 종료");
        }
    }
}
