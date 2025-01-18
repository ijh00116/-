using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Core;
using Cysharp.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine.Purchasing;
using System;
using BlackTree.Model;
using UnityEngine.Purchasing.Extension;
using UnityEngine.Purchasing.Security;

namespace BlackTree
{
    public class IAPManager : Monosingleton<IAPManager>, IStoreListener,IDetailedStoreListener
    {
        private IStoreController storeController;
        private IExtensionProvider extensionProvider;

        public List<string> product_ids;
        public Action OnBindInitialized;
        public Action OnBindPurchaseComplete;

        public void Initialize()
        {
            product_ids = new List<string>();
            for (int i=0; i<StaticData.Wrapper.inAppProducts.Length; i++)
            {
                var id = StaticData.Wrapper.inAppProducts[i].productId;
                product_ids.Add(id);
            }
            for (int i = 0; i < StaticData.Wrapper.packages.Length; i++)
            {
#if UNITY_ANDROID
                var id = StaticData.Wrapper.packages[i].productId;
#elif UNITY_IOS
                var id = StaticData.Wrapper.packages[i].productId_IOS;
#endif

                product_ids.Add(id);
            }
            for (int i = 0; i < StaticData.Wrapper.skillPackages.Length; i++)
            {
#if UNITY_ANDROID
                var id = StaticData.Wrapper.skillPackages[i].productId;
#elif UNITY_IOS
                var id = StaticData.Wrapper.skillPackages[i].productId_IOS;
#endif

                product_ids.Add(id);
            }

            for (int i = 0; i < StaticData.Wrapper.battlePassProductData.Length; i++)
            {
#if UNITY_ANDROID
                var id = StaticData.Wrapper.battlePassProductData[i].battlepassProductID;
#elif UNITY_IOS
                var id = StaticData.Wrapper.battlePassProductData[i].battlepassProductID_IOS;
#endif

                product_ids.Add(id);
            }

            InitializePurchasing();
        }

        public void InitializePurchasing()
        {
    
            var module = StandardPurchasingModule.Instance();

            ConfigurationBuilder builder = ConfigurationBuilder.Instance(module);
            foreach (var item in product_ids)
            {
                builder.AddProduct(item, ProductType.Consumable, new IDs
                {
                    { item, AppleAppStore.Name },
                    { item, GooglePlay.Name },
                 });
            }
            UnityPurchasing.Initialize(this, builder);
        }

        public bool IsInitialized()
        {
            return (storeController != null && extensionProvider != null);
        }

        void SendReward(Product product)
        {
            string productIdStr = product.definition.id;

         
#if UNITY_ANDROID
            if (productIdStr.Equals("battlepass_level_0"))
            {
                BlackTree.Model.Player.Cloud.battlepassPurchaseHistory.levelPassPurchased[1] = true;
                Player.Cloud.battlepassPurchaseHistory.SetDirty(true).UpdateHash();
            }
            if (productIdStr.Contains("battlepass_level_1"))
            {
                BlackTree.Model.Player.Cloud.battlepassPurchaseHistory.levelPassPurchased[2] = true;
                Player.Cloud.battlepassPurchaseHistory.SetDirty(true).UpdateHash();
            }
            if (productIdStr.Equals("battlepass_chapter_0"))
            {
                BlackTree.Model.Player.Cloud.battlepassPurchaseHistory.chapterPassPurchased[0] = true;
                Player.Cloud.battlepassPurchaseHistory.SetDirty(true).UpdateHash();
            }
            if (productIdStr.Contains("battlepass_chapter_1"))
            {
                BlackTree.Model.Player.Cloud.battlepassPurchaseHistory.chapterPassPurchased[1] = true;
                Player.Cloud.battlepassPurchaseHistory.SetDirty(true).UpdateHash();
            }
            if (productIdStr.Contains("battlepass_chapter_2"))
            {
                BlackTree.Model.Player.Cloud.battlepassPurchaseHistory.chapterPassPurchased[2] = true;
                Player.Cloud.battlepassPurchaseHistory.SetDirty(true).UpdateHash();
            }
            if (productIdStr.Equals("package_adremove_0"))
            {
                Player.Cloud.inAppPurchase.purchaseAds = true;
                Player.Cloud.inAppPurchase.SetDirty(true).UpdateHash();
            }
            if (productIdStr.Equals("package_vip_0"))
            {
                Player.Cloud.inAppPurchase.purchaseVip = 1;
                Player.Cloud.inAppPurchase.SetDirty(true).UpdateHash();
            }
            if (productIdStr.Equals("package_vip_1"))
            {
                Player.Cloud.inAppPurchase.purchaseVip = 2;
                Player.Cloud.inAppPurchase.SetDirty(true).UpdateHash();
            }
            if (productIdStr.Equals("package_vip_2"))
            {
                Player.Cloud.inAppPurchase.purchaseVip = 3;
                Player.Cloud.inAppPurchase.SetDirty(true).UpdateHash();
            }
#else
            if (productIdStr.Equals("battlepass_level_0_IOS"))
            {
                BlackTree.Model.Player.Cloud.battlepassPurchaseHistory.levelPassPurchased[1] = true;
                Player.Cloud.battlepassPurchaseHistory.SetDirty(true).UpdateHash();
            }
            if (productIdStr.Contains("battlepass_level_1_IOS"))
            {
                BlackTree.Model.Player.Cloud.battlepassPurchaseHistory.levelPassPurchased[2] = true;
                Player.Cloud.battlepassPurchaseHistory.SetDirty(true).UpdateHash();
            }
            if (productIdStr.Equals("battlepass_chapter_0_IOS"))
            {
                BlackTree.Model.Player.Cloud.battlepassPurchaseHistory.chapterPassPurchased[0] = true;
                Player.Cloud.battlepassPurchaseHistory.SetDirty(true).UpdateHash();
            }
            if (productIdStr.Contains("battlepass_chapter_1_IOS"))
            {
                BlackTree.Model.Player.Cloud.battlepassPurchaseHistory.chapterPassPurchased[1] = true;
                Player.Cloud.battlepassPurchaseHistory.SetDirty(true).UpdateHash();
            }
            if (productIdStr.Contains("battlepass_chapter_2_IOS"))
            {
                BlackTree.Model.Player.Cloud.battlepassPurchaseHistory.chapterPassPurchased[2] = true;
                Player.Cloud.battlepassPurchaseHistory.SetDirty(true).UpdateHash();
            }
            if (productIdStr.Equals("package_adremove_0_IOS"))
            {
                Player.Cloud.inAppPurchase.purchaseAds = true;
                Player.Cloud.inAppPurchase.SetDirty(true).UpdateHash();
            }
            if (productIdStr.Equals("package_vip_0_IOS"))
            {
                Player.Cloud.inAppPurchase.purchaseVip = 1;
                Player.Cloud.inAppPurchase.SetDirty(true).UpdateHash();
            }
            if (productIdStr.Equals("package_vip_1_IOS"))
            {
                Player.Cloud.inAppPurchase.purchaseVip = 2;
                Player.Cloud.inAppPurchase.SetDirty(true).UpdateHash();
            }
            if (productIdStr.Equals("package_vip_2_IOS"))
            {
                Player.Cloud.inAppPurchase.purchaseVip = 3;
                Player.Cloud.inAppPurchase.SetDirty(true).UpdateHash();
            }
#endif


#if SuperAccount
#else
            storeController.ConfirmPendingPurchase(product);
#endif
            if(OnBindPurchaseComplete!=null)
            {
                OnBindPurchaseComplete?.Invoke();
            }
            OnBindPurchaseComplete = null;

#if UNITY_EDITOR


#elif UNITY_ANDROID
            var validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
            var result = validator.Validate(product.receipt);

            

            foreach (var purchaseReceipt in result)
            {
                BackEnd.Param param = new BackEnd.Param();

                param.Add("transactionID", purchaseReceipt.transactionID);
                param.Add("purchaseid", purchaseReceipt.productID);
                param.Add("userID", Player.Cloud.optiondata.useruuid.ToString());
                param.Add("Date", purchaseReceipt.purchaseDate.ToString());
                Debug.Log(purchaseReceipt.productID);
                Debug.Log(purchaseReceipt.purchaseDate);
                Debug.Log(purchaseReceipt.transactionID);

                Player.BackendData.LogEvent("inapppurchase", param);
            }

            
#else
           var validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
            var result = validator.Validate(product.receipt);

          

            foreach (var purchaseReceipt in result)
            {
                BackEnd.Param param = new BackEnd.Param();

                param.Add("transactionID", purchaseReceipt.transactionID);
                param.Add("purchaseid", purchaseReceipt.productID);
                param.Add("userID", Player.Cloud.optiondata.useruuid.ToString());
                param.Add("Date", purchaseReceipt.purchaseDate.ToString());
                Debug.Log(purchaseReceipt.productID);
                Debug.Log(purchaseReceipt.purchaseDate);
                Debug.Log(purchaseReceipt.transactionID);

                Player.BackendData.LogEvent("inapppurchase", param);     
            }

         
#endif

            FirebaseManager.Instance.LogEvent("inapppurchase",
                      new Firebase.Analytics.Parameter("purchaseid", productIdStr));

            //string title = "제품이 구매되었습니다. 감사합니다.";
            //BlackTree.Bundles.ViewCanvas.Get<Bundles.ViewCanvasToastMessage>().ShowandFade(title);

            Debug.Log("purchased Process complete");
            Player.SaveUserDataToFirebaseAndLocal().Forget();
        }

        public void BuyProductID(string productId, Action callback = null)
        {
            OnBindPurchaseComplete = callback;
            if (IsInitialized())
            {
                Product p = storeController.products.WithID(productId);
                if (p != null && p.availableToPurchase)
                {
                    storeController.InitiatePurchase(p);
                }
                else
                {
                    Debug.Log("Purchase failed #" + productId);
                    if (p == null)
                    {
                        Debug.Log("Purchase failed productid is null#" + productId);
                    }
                    else
                    {
                        Debug.Log("Purchase failed for not available to purcahse #" + productId);
                    }
                }
            }
            else
            {
                Debug.Log("Purchase failed for not initialize #" + productId);
            }
        }
        public string GetLocalPriceString(string productId)
        {
            if (storeController == null || storeController.products == null || storeController.products.WithID(productId) == null)
            {
                return string.Empty;
            }
            Product p = storeController.products.WithID(productId);
            return p.metadata.localizedPriceString;//.localizedPriceString;
        }

        public Product GetProduct(string productId)
        {
            Product p = storeController.products.WithID(productId);
            return p;
        }



        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Debug.Log("IAP OnInitialized");
            storeController = controller;
            extensionProvider = extensions;
            OnBindInitialized?.Invoke();
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.Log("IAP init failed");
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            Debug.Log("IAP init failed");
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
#if UNITY_EDITOR
            OnPurchaseComplete(args.purchasedProduct);

#else
            bool validPurchase = true;

            var validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
            try
            {
                var result = validator.Validate(args.purchasedProduct.receipt);

                foreach (var purchaseReceipt in result)
                {
                    Debug.Log(purchaseReceipt.productID);
                    Debug.Log(purchaseReceipt.purchaseDate);
                    Debug.Log(purchaseReceipt.transactionID);
                }
            }
            catch (IAPSecurityException)
            {
                Debug.Log("Invalid receipt");
                validPurchase = false;
            }

            if(validPurchase)
            {
                OnPurchaseComplete(args.purchasedProduct);
            }
            else
            {
                string title = StaticData.Wrapper.localizeddesclist[(int)Definition.LocalizeDescKeys.Etc_wrongRequest].StringToLocal;
                BlackTree.Bundles.ViewCanvas.Get<Bundles.ViewCanvasToastMessage>().ShowandFade(title);
                return PurchaseProcessingResult.Complete;
            }
#endif

            return PurchaseProcessingResult.Pending;
        }

        public void OnPurchaseComplete(Product product)
        {
            //구매 완료 후 콜백
            Debug.Log("product 구매 완료" + $"{product.definition.id}");
            SendReward(product);
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            Debug.Log("IAP init failed");
        }

        public void RestorePurchase()
        {
            if (!IsInitialized()) 
                return;

            if(Application.platform==RuntimePlatform.IPhonePlayer || Application.platform==RuntimePlatform.OSXPlayer)
            {
                Debug.Log("Try Restore purchase");
                var appleExt = extensionProvider.GetExtension<IAppleExtensions>();
                appleExt.RestoreTransactions((result,msg)=> {
                    Debug.Log(message: $"구매복구 시도 결과- {result}");
                    Debug.Log($"message:{msg}");

                    string restored = StaticData.Wrapper.localizeddesclist[(int)Definition.LocalizeDescKeys.Etc_RestorePurchase].StringToLocal;
                    BlackTree.Bundles.ViewCanvas.Get<Bundles.ViewCanvasToastMessage>().ShowandFade(restored);
                });
            }
        }

        public bool HadPurchased(string productId)
        {
            if (!IsInitialized())
                return false;
            var product = storeController.products.WithID(productId);

            if(product!=null)
            {
                return product.hasReceipt;
            }
            return false;
        }
    }

}
