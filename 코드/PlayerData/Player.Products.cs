using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Definition;
using System;
using BlackTree.Core;
using BlackTree.Bundles;
using Cysharp.Threading.Tasks;

namespace BlackTree.Model
{
 
    public static partial class Player
    {
        public static class Products
        {
            private static Dictionary<ProductKey, PackageCache> _packageCache = new Dictionary<ProductKey, PackageCache>();
            private static Dictionary<ProductKey, SkillPackageCache> _skillPackageCache = new Dictionary<ProductKey, SkillPackageCache>();
            private static Dictionary<ProductKey, InAppCache> _inappCache = new Dictionary<ProductKey, InAppCache>();

            public const int diaADPurchaseMaxCount= 5;
            public const int awakeStoneADPurchaseMaxCount = 2;

            public static Action PackagePurchaseCallback;
            public static Action<SkillKey> skillPopup;
            public static Action skillPackageCallback;

            public static Action adPurchaseCallback;
            public static Action inAppProductPurchaseCallback;

            public static Action FreeShopAwakeStoneSync;
            public static void Init()
            {
                //Debug.LogError("»óÁ¡ ÀÌ´Ö 1");
                for (int i = Cloud.userInAppProductPurchase.collection.Count; i < StaticData.Wrapper.inAppProducts.Length; ++i)
                {
                    Cloud.userInAppProductPurchase.collection.Add(new UserInAppPurchase());
                }
                //Debug.LogError("»óÁ¡ ÀÌ´Ö 2");
                for (int i = Cloud.inAppPurchase.collection.Count; i < StaticData.Wrapper.packages.Length; ++i)
                {
                    Cloud.inAppPurchase.collection.Add(new UserInAppPurchase());
                }
                //Debug.LogError("»óÁ¡ ÀÌ´Ö 3");
                for (int i = Cloud.inAppSkillPurchase.collection.Count; i < StaticData.Wrapper.skillPackages.Length; ++i)
                {
                    Cloud.inAppSkillPurchase.collection.Add(new UserInAppPurchase());
                }
                //Debug.LogError("»óÁ¡ ÀÌ´Ö 4");
                for (int i = 0; i < Cloud.inAppPurchase.collection.Count; ++i)
                {
                    var data = StaticData.Wrapper.packages[i];
                    _packageCache.Add(data.key, new PackageCache(Cloud.inAppPurchase.collection[i], data, i));
                }
                //Debug.LogError("»óÁ¡ ÀÌ´Ö 5");
                for (int i = 0; i < Cloud.inAppSkillPurchase.collection.Count; ++i)
                {
                    var data = StaticData.Wrapper.skillPackages[i];
                    _skillPackageCache.Add(data.key, new SkillPackageCache(Cloud.inAppSkillPurchase.collection[i], data, i));
                }
                //Debug.LogError("»óÁ¡ ÀÌ´Ö 6");
                for (int i = 0; i < Cloud.userInAppProductPurchase.collection.Count; ++i)
                {
                    var data = StaticData.Wrapper.inAppProducts[i];
                    _inappCache.Add(data.key, new InAppCache(Cloud.userInAppProductPurchase.collection[i], data, i));
                }

                Cloud.inAppPurchase.UpdateHash().SetDirty(true);
            }

            public static string GetProductId(ProductKey key)
            {
                if (_packageCache.ContainsKey(key))
                    return _packageCache[key].ProductId;
                else if (_inappCache.ContainsKey(key))
                    return _inappCache[key].ProductId;
                else if (_skillPackageCache.ContainsKey(key))
                    return _skillPackageCache[key].ProductId;

                return "";
            }

            public static InAppCache GetProduct(ProductKey key)
            {
                if (_inappCache.ContainsKey(key))
                    return _inappCache[key];
                else
                    return null;
            }

            public static PackageCache GetPackage(ProductKey key)
            {
                if (_packageCache.ContainsKey(key))
                    return _packageCache[key];
                else
                    return null;
            }

            public static SkillPackageCache GetSkillPackage(ProductKey key)
            {
                if (_skillPackageCache.ContainsKey(key))
                    return _skillPackageCache[key];
                else
                    return null;
            }

            public static PackageCache GetPackage(string id)
            {
                foreach (var cache in _packageCache.Values)
                {
                    if (cache.ProductId == id)
                        return cache;
                }
                return null;
            }

            public static void PurchaseBattlePass(ContentLockType locktype,int tier,Action callback)
            {
                int index = 0;
                switch (locktype)
                {
                    case ContentLockType.UnitLevel:
                        index = tier;
                        break;
                    case ContentLockType.ChapterLevel:
                        index = 3 + tier;
                        break;
                    default:
                        break;
                }

                var battlepassProduct=StaticData.Wrapper.battlePassProductData[index];

#if UNITY_ANDROID
                IAPManager.Instance.BuyProductID(battlepassProduct.battlepassProductID, callback);
#elif UNITY_IOS
                IAPManager.Instance.BuyProductID(battlepassProduct.battlepassProductID_IOS, callback);
#endif


            }
            public static void PurchaseInApp(ProductKey key, Action callback = null)
            {
                //if (Model.Player.Cloud.optiondata.isGuest==true)
                //{
                //    PurchaseInAppGuest(key,callback).Forget();
                  
                //    return;
                //}
                var inappProduct= Player.Products.GetProduct(key);
                if (inappProduct!=null)
                {
                    callback += () =>
                    {
                        inappProduct.BoughtCount++;
                        Player.Products.inAppProductPurchaseCallback?.Invoke();
                    };
                }
                var packageProduct = Player.Products.GetPackage(key);
                if (packageProduct != null)
                {
                    callback += () =>
                    {
                        if (packageProduct.Loop)
                        {
                            packageProduct.LastTimePackage = Extension.NextDayYMD;
                        }
                        packageProduct.BoughtCount++;
                        packageProduct._userPurchase.isPurchased = true;

                        Player.Products.PackagePurchaseCallback?.Invoke();
                    };
                }

                var skillPackageProduct = Player.Products.GetSkillPackage(key);
                if (skillPackageProduct != null)
                {
                    callback += () =>
                    {
                        if (skillPackageProduct.Loop)
                        {
                            skillPackageProduct.LastTimePackage = Extension.NextDayYMD;
                        }
                        skillPackageProduct.BoughtCount++;
                        skillPackageProduct._userPurchase.isPurchased = true;

                        Player.Products.PackagePurchaseCallback?.Invoke();
                    };
                }

                if (_packageCache.ContainsKey(key))
                    IAPManager.Instance.BuyProductID(_packageCache[key].ProductId, callback);
                else if (_inappCache.ContainsKey(key))
                    IAPManager.Instance.BuyProductID(_inappCache[key].ProductId, callback);
                else if (_skillPackageCache.ContainsKey(key))
                    IAPManager.Instance.BuyProductID(_skillPackageCache[key].ProductId, callback);
                else
                    IAPManager.Instance.BuyProductID(key.ToString(), callback);
            }

            async static UniTask PurchaseInAppGuest(ProductKey key, Action callback = null)
            {
                bool isConfirm = false;
                string desc = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Social_please].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowBox("¾Ë¸²", desc, ()=> { isConfirm = true; }, true);

                await UniTask.WaitUntil(() => isConfirm == true);

                var inappProduct = Player.Products.GetProduct(key);
                if (inappProduct != null)
                {
                    callback += () =>
                    {
                        inappProduct.BoughtCount++;
                        Player.Products.inAppProductPurchaseCallback?.Invoke();
                    };
                }
                var packageProduct = Player.Products.GetPackage(key);
                if (packageProduct != null)
                {
                    callback += () =>
                    {
                        if (packageProduct.Loop)
                        {
                            packageProduct.LastTimePackage = Extension.NextDayYMD;
                        }
                        packageProduct.BoughtCount++;
                        packageProduct._userPurchase.isPurchased = true;

                        Player.Products.PackagePurchaseCallback?.Invoke();
                    };
                }

                var skillPackageProduct = Player.Products.GetSkillPackage(key);
                if (skillPackageProduct != null)
                {
                    callback += () =>
                    {
                        if (skillPackageProduct.Loop)
                        {
                            skillPackageProduct.LastTimePackage = Extension.NextDayYMD;
                        }
                        skillPackageProduct.BoughtCount++;
                        skillPackageProduct._userPurchase.isPurchased = true;

                        Player.Products.PackagePurchaseCallback?.Invoke();
                    };
                }

                if (_packageCache.ContainsKey(key))
                    IAPManager.Instance.BuyProductID(_packageCache[key].ProductId, callback);
                else if (_inappCache.ContainsKey(key))
                    IAPManager.Instance.BuyProductID(_inappCache[key].ProductId, callback);
                else if (_skillPackageCache.ContainsKey(key))
                    IAPManager.Instance.BuyProductID(_skillPackageCache[key].ProductId, callback);
                else
                    IAPManager.Instance.BuyProductID(key.ToString(), callback);

            }
        }


        public class InAppCache
        {
            public int _cloudIndex = 0;
            public ProductKey Key => _dataInAppProduct.key;
            public string ProductId => _dataInAppProduct.productId;

            public int CurrBoughtCount => _userPurchase.currBoughtCount;

            public UserInAppPurchase _userPurchase;
            public DataInAppProduct _dataInAppProduct;
            public InAppCache(UserInAppPurchase userInAppPurchase,DataInAppProduct dataPackage,int index)
            {
                _cloudIndex = index;
                _userPurchase = userInAppPurchase;
                _dataInAppProduct = dataPackage;
            }

            public int BoughtCount
            {
                get => Cloud.userInAppProductPurchase.collection[_cloudIndex].currBoughtCount;
                set
                {
                    Cloud.userInAppProductPurchase.collection[_cloudIndex].currBoughtCount = value;
                    Cloud.userInAppProductPurchase.UpdateHash().SetDirty(true);
                }
            }
        }
        public class PackageCache
        {
            public int _cloudIndex = 0;
            public DateTime LastTimePackage
            {
                get
                {
                    var lastTime = Cloud.inAppPurchase.collection[_cloudIndex].lastTimePackage;
                    return Extension.GetDateTimeByIsoString(lastTime);
                }
                set
                {
                    Cloud.inAppPurchase.collection[_cloudIndex].lastTimePackage = value.ToIsoString();
                    Cloud.inAppPurchase.UpdateHash().SetDirty(true);
                }
            }
            
            public int CurrBoughtCount => _userPurchase.currBoughtCount;
            public int BuyableCount => _dataPackage.buyableCount;
            public bool Loop => _dataPackage.loop;
            public RewardTypes[] RewardTypes => _dataPackage.rewardTypes;
            
            public int IncreaseRate => _dataPackage.increaseRate;
            public bool CanBuy => BuyableCount <= -1 ? true : CurrBoughtCount < BuyableCount;
            public bool IsVisible => CanBuy || Loop;

#if UNITY_ANDROID
            public string ProductId => _dataPackage.productId;
#elif UNITY_IOS
            public string ProductId => _dataPackage.productId_IOS;
#endif

            public void ResetBuyableCount()
            {
                _userPurchase.currBoughtCount = 0;
            }
            public int BoughtCount
            {
                get => Cloud.inAppPurchase.collection[_cloudIndex].currBoughtCount;
                set
                {
                    Cloud.inAppPurchase.collection[_cloudIndex].currBoughtCount = value;
                    Cloud.inAppPurchase.UpdateHash().SetDirty(true);
                }
            }

            public UserInAppPurchase _userPurchase;
            public DataPackage _dataPackage;

            public PackageCache(UserInAppPurchase userInAppPurchase, DataPackage dataPackage, int index)
            {
                _cloudIndex = index;
                _userPurchase = userInAppPurchase;
                _dataPackage = dataPackage;
            }

            public void SetInitLastTime()
            {
                Cloud.inAppPurchase.UpdateHash().SetDirty(true);
            }
        }
        public class SkillPackageCache
        {
            public int _cloudIndex = 0;
            public DateTime LastTimePackage
            {
                get
                {
                    var lastTime = Cloud.inAppSkillPurchase.collection[_cloudIndex].lastTimePackage;
                    return Extension.GetDateTimeByIsoString(lastTime);
                }
                set
                {
                    Cloud.inAppSkillPurchase.collection[_cloudIndex].lastTimePackage = value.ToIsoString();
                    Cloud.inAppSkillPurchase.UpdateHash().SetDirty(true);
                }
            }

            public int CurrBoughtCount => _userPurchase.currBoughtCount;
            public int BuyableCount => _dataPackage.buyableCount;
            public bool Loop => _dataPackage.loop;
            public RewardTypes[] RewardTypes => _dataPackage.rewardTypes;

            public int IncreaseRate => _dataPackage.increaseRate;
            public bool CanBuy => BuyableCount <= -1 ? true : CurrBoughtCount < BuyableCount;
            public bool IsVisible => CanBuy || Loop;

#if UNITY_ANDROID
            public string ProductId => _dataPackage.productId;
#elif UNITY_IOS
            public string ProductId => _dataPackage.productId_IOS;
#endif

            public void ResetBuyableCount()
            {
                _userPurchase.currBoughtCount = 0;
            }
            public int BoughtCount
            {
                get => Cloud.inAppSkillPurchase.collection[_cloudIndex].currBoughtCount;
                set
                {
                    Cloud.inAppSkillPurchase.collection[_cloudIndex].currBoughtCount = value;
                    Cloud.inAppSkillPurchase.UpdateHash().SetDirty(true);
                }
            }

            public UserInAppPurchase _userPurchase;
            public SkillDataPackage _dataPackage;

            public SkillPackageCache(UserInAppPurchase userInAppPurchase, SkillDataPackage dataPackage, int index)
            {
                _cloudIndex = index;
                _userPurchase = userInAppPurchase;
                _dataPackage = dataPackage;
            }

            public void SetInitLastTime()
            {
                Cloud.inAppPurchase.UpdateHash().SetDirty(true);
            }
        }
    }
}
