using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using Cysharp.Threading.Tasks;
using BlackTree.Definition;

namespace BlackTree.Core
{
    public class ControllerBattlePass
    {
        public ViewcanvasBattlePass _viewBattlePass;
        CancellationTokenSource _cts;

        List<BattlePassSlot> normalslotList = new List<BattlePassSlot>();
        List<BattlePassSlot> premiumslotList = new List<BattlePassSlot>();

        /// <summary>
        /// 0=level,1=chapter
        /// </summary>
        int currentSelectedType = 0;
        int currentSelecedTier = 0;

        const int slotWidth = 30;
        const int slotHeight = 305;

        const int levelStartValue_0 = 0;
        const int levelStartValue_1 = 200;
        const int levelStartValue_2 = 700;

        const int chapterStartValue_0 = 0;
        const int chapterStartValue_1 = 500;
        const int chapterStartValue_2 = 1000;
        public ControllerBattlePass(Transform parent, CancellationTokenSource cts)
        {
            _viewBattlePass = ViewCanvas.Create<ViewcanvasBattlePass>(parent);
            _cts = cts;

            _viewBattlePass.levelBtn.onClick.AddListener(LevelBtnSelect);
            _viewBattlePass.chapterBtn.onClick.AddListener(ChapterBtnSelect);
            _viewBattlePass.nextBtn.onClick.AddListener(NextBtnSelect);
            _viewBattlePass.prevBtn.onClick.AddListener(PrevBtnSelect);

            for (int i=0; i<StaticData.Wrapper.levelBattlePass_0.Length; i++)
            {
                var obj = Object.Instantiate(_viewBattlePass.normalSlot);
                obj.transform.SetParent(_viewBattlePass.normalSlotParent, false);
                obj.Init(StaticData.Wrapper.levelBattlePass_0[i],0, true);

                normalslotList.Add(obj);
            }

            for (int i = 0; i < StaticData.Wrapper.levelBattlePass_0.Length; i++)
            {
                var obj = Object.Instantiate(_viewBattlePass.premiumSlot);
                obj.transform.SetParent(_viewBattlePass.prmiumSlotParent, false);
                obj.Init(StaticData.Wrapper.levelBattlePass_0[i], 0, false);

                premiumslotList.Add(obj);
            }

            for(int i=0; i< _viewBattlePass.closeBtns.Length; i++)
            {
                int index = i;
                _viewBattlePass.closeBtns[index].onClick.AddListener(()=> {
                    
                    Bundles.ViewCanvas.Get<Bundles.ViewcanvasBattlePass>().blackBG.PopupCloseColorFade();
                    Bundles.ViewCanvas.Get<Bundles.ViewcanvasBattlePass>().Wrapped.CommonPopupCloseAnimationUp(()=> {
                        Bundles.ViewCanvas.Get<Bundles.ViewcanvasBattlePass>().SetVisible(false);
                    });
                });
            }

            _viewBattlePass.BindOnChangeVisible((o)=> { 
                if(o)
                {
                    DataUpCallBack();
                    ScrollUpdate();
                    _viewBattlePass.scrollrect.SetContentScrollOffsetToTop();
                }
            
            });


            _viewBattlePass.purchaseBtn.onClick.AddListener(() => {
                if(StaticData.Wrapper.battlePassProductData[currentSelectedType * 3 + currentSelecedTier].isFree)
                {
                    PurchaseCurrentBattlePass();
                }
                else
                {
                    Player.Products.PurchaseBattlePass(currentSelectedType == 0 ? ContentLockType.UnitLevel : ContentLockType.ChapterLevel,
                    currentSelecedTier, PurchaseCurrentBattlePass);
                }
                
            });
            Player.Pass.chapterCallBack += DataUpCallBack;
            Player.Pass.levelCallBack += DataUpCallBack;
            Player.Pass.rewardedCallback += SlotRewardCallback;

            currentSelectedType = 0;
            currentSelecedTier = 0;

            DataUpCallBack();
            _viewBattlePass.scrollrect.content.gameObject.SetActive(false);
            ScrollUpdate();

            _viewBattlePass.titleTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_BattlePass].StringToLocal;
            _viewBattlePass.levelTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Level].StringToLocal;
            _viewBattlePass.levelTxt_on.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Level].StringToLocal;
            _viewBattlePass.chapterTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Chapter].StringToLocal;
            _viewBattlePass.chapterTxt_on.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Chapter].StringToLocal;

            string gradeLocal = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_XGrade].StringToLocal;
            _viewBattlePass.currentTierText.text = string.Format(gradeLocal, currentSelecedTier + 1);
        }

        void ScrollUpdate()
        {
            _viewBattlePass.normalSlotParent.gameObject.SetActive(false);
            _viewBattlePass.prmiumSlotParent.gameObject.SetActive(false);
    
            _viewBattlePass.scrollrect.content.gameObject.SetActive(true);
            _viewBattlePass.normalSlotParent.gameObject.SetActive(true);
            _viewBattlePass.prmiumSlotParent.gameObject.SetActive(true);
           
        }

        void LevelBtnSelect()
        {
            _viewBattlePass.topBtnSelectors.Show(0);
            currentSelectedType = 0;
            currentSelecedTier = 0;

            string gradeLocal = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_XGrade].StringToLocal;
            _viewBattlePass.currentTierText.text = string.Format(gradeLocal, currentSelecedTier + 1);

            DataUpCallBack();
            _viewBattlePass.scrollrect.SetContentScrollOffsetToTop();
        }

        void ChapterBtnSelect()
        {
            _viewBattlePass.topBtnSelectors.Show(1);
            currentSelectedType = 1;
            currentSelecedTier = 0;
            string gradeLocal = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_XGrade].StringToLocal;
            _viewBattlePass.currentTierText.text = string.Format(gradeLocal, currentSelecedTier + 1);

            DataUpCallBack();
            _viewBattlePass.scrollrect.SetContentScrollOffsetToTop();
        }

        void NextBtnSelect()
        {
            currentSelecedTier++;
            if(currentSelecedTier >= 2)
            {
                currentSelecedTier = 2;
            }

            string gradeLocal = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_XGrade].StringToLocal;
            _viewBattlePass.currentTierText.text = string.Format(gradeLocal, currentSelecedTier + 1);

            DataUpCallBack();
            _viewBattlePass.scrollrect.SetContentScrollOffsetToTop();
        }

        void PrevBtnSelect()
        {
            currentSelecedTier--;
            if (currentSelecedTier <= 0)
            {
                currentSelecedTier = 0;
            }
            string gradeLocal = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_XGrade].StringToLocal;
            _viewBattlePass.currentTierText.text = string.Format(gradeLocal, currentSelecedTier + 1);

            DataUpCallBack();
            _viewBattlePass.scrollrect.SetContentScrollOffsetToTop();
        }

        void LevelSyncUpdate()
        {
            BattlePassData[] passdatas=null;
            int startvalue = 0;
            if (currentSelecedTier==0)
            {
                passdatas = StaticData.Wrapper.levelBattlePass_0;
                startvalue = levelStartValue_0;
            }
            else if (currentSelecedTier == 1)
            {
                passdatas = StaticData.Wrapper.levelBattlePass_1;
                startvalue = levelStartValue_1;
            }
            else if (currentSelecedTier == 2)
            {
                passdatas = StaticData.Wrapper.levelBattlePass_2;
                startvalue = levelStartValue_2;
            }

            //for(int i=0; i< normalslotList.Count; i++)
            //{
            //    normalslotList[i].gameObject.SetActive(false);
            //}
            //for (int i = 0; i < premiumslotList.Count; i++)
            //{
            //    premiumslotList[i].gameObject.SetActive(false);
            //}

            for (int i=0; i< passdatas.Length; i++)
            {
                normalslotList[i].Init(passdatas[i],  currentSelecedTier, true);
            }

            for (int i = 0; i < passdatas.Length; i++)
            {
                premiumslotList[i].Init(passdatas[i],  currentSelecedTier, false);
            }

            _viewBattlePass.sliderObject.sizeDelta = new Vector2(slotWidth, slotHeight * passdatas.Length);

            float maxValue = passdatas[passdatas.Length - 1].unLockLevel;
            int currentChapter = Player.Cloud.userLevelData.currentLevel;
            _viewBattlePass.passSlider.value = ((float)(currentChapter - startvalue) / (float)(maxValue - startvalue));


#if UNITY_ANDROID
            string productid = StaticData.Wrapper.battlePassProductData[currentSelectedType * 3 + currentSelecedTier].battlepassProductID;
#elif UNITY_IOS
            string productid = StaticData.Wrapper.battlePassProductData[currentSelectedType * 3 + currentSelecedTier].battlepassProductID_IOS;
#endif

            if (StaticData.Wrapper.battlePassProductData[currentSelectedType * 3 + currentSelecedTier].isFree)
            {
                string freeLocal = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_FreeForBattlePass].StringToLocal;
                _viewBattlePass.purchasePrice.text = freeLocal;
            }
            else
            {
                _viewBattlePass.purchasePrice.text = IAPManager.Instance.GetLocalPriceString(productid);
            }
            ScrollUpdate();
        }

        void ChapterSyncUpdate()
        {
            BattlePassData[] passdatas = null;
            int startvalue = 0;
            if (currentSelecedTier == 0)
            {
                passdatas = StaticData.Wrapper.chapterBattlePass_0;
                startvalue = chapterStartValue_0;
            }
            else if (currentSelecedTier == 1)
            {
                passdatas = StaticData.Wrapper.chapterBattlePass_1;
                startvalue = chapterStartValue_1;
            }
            else if (currentSelecedTier == 2)
            {
                passdatas = StaticData.Wrapper.chapterBattlePass_2;
                startvalue = chapterStartValue_2;
            }

            //for (int i = 0; i < normalslotList.Count; i++)
            //{
            //    normalslotList[i].gameObject.SetActive(false);
            //}
            //for (int i = 0; i < premiumslotList.Count; i++)
            //{
            //    premiumslotList[i].gameObject.SetActive(false);
            //}

            for (int i = 0; i < passdatas.Length; i++)
            {
                normalslotList[i].Init(passdatas[i],  currentSelecedTier, true);
            }

            for (int i = 0; i < passdatas.Length; i++)
            {
                premiumslotList[i].Init(passdatas[i],  currentSelecedTier, false);
            }

            _viewBattlePass.sliderObject.sizeDelta = new Vector2(slotWidth, slotHeight * passdatas.Length);

            float maxValue = passdatas[passdatas.Length - 1].unLockLevel;
            int currentChapter= Player.Cloud.field.bestChapter * 5 + Player.Cloud.field.bestStage;
            _viewBattlePass.passSlider.value = (float)(currentChapter - startvalue) / (float)(maxValue - startvalue);

#if UNITY_ANDROID
            string productid = StaticData.Wrapper.battlePassProductData[currentSelectedType * 3 + currentSelecedTier].battlepassProductID;
#elif UNITY_IOS
            string productid = StaticData.Wrapper.battlePassProductData[currentSelectedType * 3 + currentSelecedTier].battlepassProductID_IOS;
#endif


            if (StaticData.Wrapper.battlePassProductData[currentSelectedType * 3 + currentSelecedTier].isFree)
            {
                string freeLocal = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_FreeForBattlePass].StringToLocal;
                _viewBattlePass.purchasePrice.text = freeLocal;
            }
            else
            {
                _viewBattlePass.purchasePrice.text = IAPManager.Instance.GetLocalPriceString(productid);
            }
            ScrollUpdate();
        }
        void PurchaseCurrentBattlePass()
        {
            if(currentSelectedType==0)
            {
                Player.Cloud.battlepassPurchaseHistory.levelPassPurchased[currentSelecedTier] = true;
            }
            else
            {
                Player.Cloud.battlepassPurchaseHistory.chapterPassPurchased[currentSelecedTier] = true;
            }
            Player.Cloud.battlepassPurchaseHistory.SetDirty(true).UpdateHash();


            string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_ThanksForPurchase].StringToLocal;
            ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);

            Player.SaveUserDataToFirebaseAndLocal().Forget();
            DataUpCallBack();
        }

        void SlotRewardCallback(int index,bool isNormal)
        {
            BattlePassData[] passdatas = null;
            if(currentSelectedType==0)
            {
                if (currentSelecedTier == 0)
                {
                    passdatas = StaticData.Wrapper.levelBattlePass_0;
                }
                else if (currentSelecedTier == 1)
                {
                    passdatas = StaticData.Wrapper.levelBattlePass_1;
                }
                else if (currentSelecedTier == 2)
                {
                    passdatas = StaticData.Wrapper.levelBattlePass_2;
                }
            }
            else
            {
                if (currentSelecedTier == 0)
                {
                    passdatas = StaticData.Wrapper.chapterBattlePass_0;
                }
                else if (currentSelecedTier == 1)
                {
                    passdatas = StaticData.Wrapper.chapterBattlePass_1;
                }
                else if (currentSelecedTier == 2)
                {
                    passdatas = StaticData.Wrapper.chapterBattlePass_2;
                }
            }
            if(isNormal)
            {
                normalslotList[index].Init(passdatas[index], currentSelecedTier, isNormal);
            }
            else
            {
                premiumslotList[index].Init(passdatas[index], currentSelecedTier, isNormal);
            }
            
        }

        void DataUpCallBack()
        {
            if(currentSelectedType==0)
            {
                LevelSyncUpdate();
            }
            else
            {
                ChapterSyncUpdate();
            }

            bool isPurchased = false;
            if (currentSelectedType == 0)
            {
                isPurchased=Player.Cloud.battlepassPurchaseHistory.levelPassPurchased[currentSelecedTier];
            }
            else
            {
                isPurchased=Player.Cloud.battlepassPurchaseHistory.chapterPassPurchased[currentSelecedTier];
            }
            _viewBattlePass.purchaseBtn.gameObject.SetActive(!isPurchased);
        }
    }

   
}
