using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using BlackTree.Definition;
using Cysharp.Threading.Tasks;

namespace BlackTree.Core
{
    public class ControllerMainTopUI
    {
        private ViewCanvasMainTop _viewCanvasMainTopui;
        private ViewCanvasMainIcons _viewCanvasMainIcons;
        private ViewCanvasMainWave _viewCanvasMainWave;

        private CancellationTokenSource _cts;

        ControllerSkillSlotEquiped[] equipedskillSlotList;

        private GoodsKey[] _visibleKeys;

        bool isMenuContentBarOpen;
        public ControllerMainTopUI(Transform parent, CancellationTokenSource cts)
        {
            _cts = cts;
            _viewCanvasMainTopui = ViewCanvas.Create<ViewCanvasMainTop>(parent);
            _viewCanvasMainTopui.SetVisible(true);

            _viewCanvasMainIcons = ViewCanvas.Create<ViewCanvasMainIcons>(parent);
            _viewCanvasMainIcons.SetVisible(true);

            _viewCanvasMainWave = ViewCanvas.Create<ViewCanvasMainWave>(parent);
            _viewCanvasMainWave.SetVisible(true);

            _viewCanvasMainTopui.sleepmodeBtn.onClick.AddListener(() => { Player.sleepMode?.Invoke(true); });

            if (Application.systemLanguage != SystemLanguage.Korean)
            {
                _viewCanvasMainTopui.InformBtn.gameObject.SetActive(false);
            }

            _viewCanvasMainTopui.questBtn.onClick.AddListener(()=> { 
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasQuest>().SetVisible(true);
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasQuest>().blackBG.PopupOpenColorFade();
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasQuest>().Wrapped.CommonPopupOpenAnimationDown();
                Player.Option.MenuContentBarOff?.Invoke(false);
            });

            _viewCanvasMainTopui.ProfileButton.onClick.AddListener(()=> { 
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasProfile>().SetVisible(true);
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasProfile>().blackBG.PopupOpenColorFade();
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasProfile>().Wrapped.CommonPopupOpenAnimationDown();
                Player.Option.MenuContentBarOff?.Invoke(false);
            });

            _viewCanvasMainTopui.optionButton.onClick.AddListener(() => { 
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasOption>().SetVisible(true);
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasOption>().blackBG.PopupOpenColorFade();
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasOption>().Wrapped.CommonPopupOpenAnimationDown();
                Player.Option.MenuContentBarOff?.Invoke(false);
            });

            _viewCanvasMainTopui.MailBtn.onClick.AddListener(() => {
                Player.Option.MailUpdate?.Invoke();
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasMail>().SetVisible(true);
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasMail>().blackBG.PopupOpenColorFade();
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasMail>().Wrapped.CommonPopupOpenAnimationDown();
                Player.Option.MenuContentBarOff?.Invoke(false);
            });

            _viewCanvasMainTopui.InformBtn.onClick.AddListener(() => {
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasInform>().SetVisible(true);
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasInform>().blackBG.PopupOpenColorFade();
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasInform>().Wrapped.CommonPopupOpenAnimationDown();
                Player.Option.MenuContentBarOff?.Invoke(false);
            });
        

            _viewCanvasMainTopui.attendBtn.onClick.AddListener(() => { 
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasAttendance>().SetVisible(true);
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasAttendance>().blackBG.PopupOpenColorFade();
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasAttendance>().Wrapped.CommonPopupOpenAnimationDown();
                Player.Option.MenuContentBarOff?.Invoke(false);
            });

            _viewCanvasMainTopui.menuButton.onClick.AddListener(()=> { ActiveMenuWindow(!isMenuContentBarOpen); });

            _viewCanvasMainIcons.adBuffButton.onClick.AddListener(() => { 
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasAdBuff>().SetVisible(true);
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasAdBuff>().blackBG.PopupOpenColorFade();
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasAdBuff>().Wrapped.CommonPopupOpenAnimationDown();
                Player.Option.MenuContentBarOff?.Invoke(false);

                Player.Guide.StartTutorial(Definition.TutorialType.AdBuff);
            });

            _viewCanvasMainIcons.goToClickerDungeon.onClick.AddListener(() => { 
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasEnterClickerDungeon>().SetVisible(true);
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasEnterClickerDungeon>().blackBG.PopupOpenColorFade();
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasEnterClickerDungeon>().Wrapped.CommonPopupOpenAnimationDown();
                Player.Option.MenuContentBarOff?.Invoke(false);
            });

            _viewCanvasMainIcons.goToRaidDungeon.onClick.AddListener(() => {
                LocalizeDescKeys localizedKey = LocalizeDescKeys.Etc_UnlockAfterChapter;
                int chapterValue = Model.Player.Option.ContentUIUnlockLevel(LockedUIType.RaidUnlock);


                if (Player.Cloud.field.bestChapter < chapterValue - 1)
                {
                    string localizedvalue = string.Format(StaticData.Wrapper.localizeddesclist[(int)localizedKey].StringToLocal, chapterValue);
                    ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                    return;
                }

                Bundles.ViewCanvas.Get<Bundles.ViewCanvasEnterRaidDungeon>().SetVisible(true);
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasEnterRaidDungeon>().blackBG.PopupOpenColorFade();
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasEnterRaidDungeon>().Wrapped.CommonPopupOpenAnimationDown();
                Player.Option.MenuContentBarOff?.Invoke(false);

                Player.Guide.StartTutorial(Definition.TutorialType.RaidStart);
            });

            _viewCanvasMainIcons.goResearchWindowBtn.onClick.AddListener(() => {
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasResearchWindow>().SetVisible(true);
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasResearchWindow>().blackBG.PopupOpenColorFade();
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasResearchWindow>().Wrapped.CommonPopupOpenAnimationDown();
                Player.Option.MenuContentBarOff?.Invoke(false);

                Player.Guide.StartTutorial(Definition.TutorialType.RPContentClear);
            });

            _viewCanvasMainIcons.vipWindowOpenBtn.onClick.AddListener(() => {
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasVipWindow>().SetVisible(true);
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasVipWindow>().blackBG.PopupOpenColorFade();
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasVipWindow>().Wrapped.CommonPopupOpenAnimationDown();
                Player.Option.MenuContentBarOff?.Invoke(false);
            });

            _viewCanvasMainIcons.freeShopOpenBtn.onClick.AddListener(() => {
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasFreeShop>().SetVisible(true);
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasFreeShop>().blackBG.PopupOpenColorFade();
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasFreeShop>().Wrapped.CommonPopupOpenAnimationDown();
                Player.Option.MenuContentBarOff?.Invoke(false);
            });

            _viewCanvasMainIcons.battlepassBtn.onClick.AddListener(() => {
                Bundles.ViewCanvas.Get<Bundles.ViewcanvasBattlePass>().SetVisible(true);
                Bundles.ViewCanvas.Get<Bundles.ViewcanvasBattlePass>().blackBG.PopupOpenColorFade();
                Bundles.ViewCanvas.Get<Bundles.ViewcanvasBattlePass>().Wrapped.CommonPopupOpenAnimationDown();
                Player.Option.MenuContentBarOff?.Invoke(false);
            });

            _viewCanvasMainWave.stageBtn.onClick.AddListener(() => { 
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasStage>().SetVisible(true);
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasStage>().blackBG.PopupOpenColorFade();
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasStage>().Wrapped.CommonPopupOpenAnimationDown();
                Player.Option.MenuContentBarOff?.Invoke(false);
            });

            _viewCanvasMainTopui.rankingBtn.onClick.AddListener(() => {
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasNormalRanking>().SetVisible(true);
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasNormalRanking>().blackBG.PopupOpenColorFade();
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasNormalRanking>().Wrapped.CommonPopupOpenAnimationDown();
                Player.Option.MenuContentBarOff?.Invoke(false);
            });

            _viewCanvasMainTopui.inventoryBtn.onClick.AddListener(() => {
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasInventory>().SetVisible(true);
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasInventory>().blackBG.PopupOpenColorFade();
                Bundles.ViewCanvas.Get<Bundles.ViewCanvasInventory>().Wrapped.CommonPopupOpenAnimationDown();
                Player.Option.MenuContentBarOff?.Invoke(false);
            });

            for(int i=0;i< _viewCanvasMainTopui.goodsBtn.Length; i++)
            {
                _viewCanvasMainTopui.goodsBtn[i].onClick.AddListener(() => {
                    Bundles.ViewCanvas.Get<Bundles.ViewCanvasInventory>().SetVisible(true);
                    Bundles.ViewCanvas.Get<Bundles.ViewCanvasInventory>().blackBG.PopupOpenColorFade();
                    Bundles.ViewCanvas.Get<Bundles.ViewCanvasInventory>().Wrapped.CommonPopupOpenAnimationDown();
                });
            }

            //money
            _viewCanvasMainTopui.Init();
            _visibleKeys = new GoodsKey[(int)GoodsKey.End];
            for (int i = 0; i < (int)GoodsKey.End; i++)
            {
                _visibleKeys[i] = (GoodsKey)i;

                int _index = i;
                Player.ControllerGood.BindOnChange((GoodsKey)_index, () =>
                {
                    UpdateGoodView((GoodsKey)_index);
                });
                UpdateGoodView((GoodsKey)_index);
            }
            //money

            Battle.Field.onChangeStage += ChangeLeftEnemyCallback;
            Battle.Field.CurrentEnemyStateCallback += ChangeLeftEnemyCallback;
            ChangeLeftEnemyCallback();

            Player.Level.callbackGetExp += UserInfoUpdate;

            Player.Unit.syncHpUI += UpdateUnitHpData;

            _viewCanvasMainWave.goToBossBtn.onClick.AddListener(() => { Player.Cloud.optiondata.autoChallengeBoss = true; });
            UpdateUnitHpData();
            UserInfoUpdate();
            Main().Forget();
            StageUpdate().Forget();

            Player.Option.menuRedDotCallback += RedDotUpdate;
            Player.Option.questRedDotCallback += QuestRedDotUpdate;

            isMenuContentBarOpen = false;
            Player.Option.MenuContentBarOff += ActiveMenuWindow;

            VipActivate();

            Player.Products.PackagePurchaseCallback += VipActivate;
        }

        void RedDotUpdate(MenuType _type,bool isActive)
        {
            _viewCanvasMainTopui.redDotList[(int)_type].SetActive(isActive);
            bool isRedDot = false;
            for (int i=0; i< _viewCanvasMainTopui.redDotList.Length;i++)
            {
                if(Application.systemLanguage!=SystemLanguage.Korean)
                {
                    if ((MenuType)i == MenuType.Inform)
                        continue;
                }
                if(_viewCanvasMainTopui.redDotList[i].activeSelf)
                {
                    isRedDot = true;
                    break;
                }
            }
            _viewCanvasMainTopui.menuRedDot.SetActive(isRedDot);
        }

        void QuestRedDotUpdate(bool isActive)
        {
            _viewCanvasMainTopui.questRedDot.SetActive(isActive);
        }
        void UserInfoUpdate()
        {
            _viewCanvasMainTopui.userLv.text=string.Format("Lv.{0}", Player.Cloud.userLevelData.currentLevel.ToString()) ;
            _viewCanvasMainTopui.userCombatValue.text = string.Format("ÀüÅõ·Â: {0}", Player.Unit.SwordAtk.ToNumberString()) ;
            _viewCanvasMainTopui.expfillAmount.fillAmount = (float)(Player.Level.currentExp / Player.Level.currentmaxExp);
        }

        void ActiveMenuWindow(bool active)
        {
            isMenuContentBarOpen = active;
           // bool active = _viewCanvasMainTopui.menuWindow.gameObject.activeInHierarchy;
            if(active)
            {
                _viewCanvasMainTopui.menuWindow.gameObject.SetActive(true);
                _viewCanvasMainTopui.menuWindow.CommonPopupMoveAnimationCustom(_viewCanvasMainTopui.menuClosePos, _viewCanvasMainTopui.menuOpenPos);
            }
            else
            {
                _viewCanvasMainTopui.menuWindow.CommonPopupMoveAnimationCustom(_viewCanvasMainTopui.menuOpenPos, _viewCanvasMainTopui.menuClosePos, () => {
                    _viewCanvasMainTopui.menuWindow.gameObject.SetActive(false);
                });
            }
            
        }
        private void UpdateGoodView(GoodsKey key)
        {
            ObtainGood goodview = null;
            foreach (var _view in _viewCanvasMainTopui.ObtainGoods)
            {
                if (_view.GoodsType == key)
                {
                    goodview = _view;
                    break;
                }
            }
            if (goodview != null)
            {
                var value = Player.ControllerGood.GetValue(key);
                goodview.StartRoutineUpdateView(value);
            }

        }

        void UpdateUnitHpData()
        {
            _viewCanvasMainTopui.hpText.text = string.Format("{0}/{1}", Player.Unit.Hp.ToNumberString(),Player.Unit.MaxHp.ToNumberString());
            _viewCanvasMainTopui.hpSlider.value = (float)(Model.Player.Unit.Hp) / (float)(Model.Player.Unit.MaxHp);
            _viewCanvasMainTopui.shieldText.text = string.Format("{0}/{1}", Player.Unit.Shield.ToNumberString(), Player.Unit.MaxShield.ToNumberString());
            _viewCanvasMainTopui.shieldSlider.value = (float)(Model.Player.Unit.Shield) / (float)(Model.Player.Unit.MaxShield);

        }

        void ChangeLeftEnemyCallback()
        {
            _viewCanvasMainWave.SetLeftEnemy();
           
        }
        enum UIStep
        {
            First, Second,
        }
        UIStep uiStep;
        float currentTime;
        Vector3 originalScale = new Vector3(0.9f, 0.9f, 0.9f);
        Vector3 zoomScale = new Vector3(1.1f, 1.1f, 1.1f);

        async UniTaskVoid Main()
        {
            while (true)
            {
                switch (uiStep)
                {
                    case UIStep.First:
                        currentTime += Time.deltaTime * 0.7f;
                        _viewCanvasMainWave.BossObj.transform.localScale = Vector3.Lerp(originalScale, zoomScale, currentTime);
                        _viewCanvasMainWave.monsterImage.transform.localScale = Vector3.Lerp(originalScale, zoomScale, currentTime);
                        _viewCanvasMainWave.goToBossBtn.transform.localScale = Vector3.Lerp(originalScale, zoomScale, currentTime);
                        if (currentTime >= 1)
                        {
                            currentTime = 0;
                            uiStep = UIStep.Second;
                        }
                        break;
                    case UIStep.Second:
                        currentTime += Time.deltaTime * 0.7f;
                        _viewCanvasMainWave.BossObj.transform.localScale = Vector3.Lerp(zoomScale, originalScale, currentTime);
                        _viewCanvasMainWave.monsterImage.transform.localScale = Vector3.Lerp(zoomScale, originalScale, currentTime);
                        _viewCanvasMainWave.goToBossBtn.transform.localScale = Vector3.Lerp(zoomScale, originalScale, currentTime);
                        if (currentTime >= 1)
                        {
                            currentTime = 0;
                            uiStep = UIStep.First;
                        }
                        break;
                    default:
                        break;
                }
                if (Player.Cloud.optiondata.autoChallengeBoss)
                {
                    _viewCanvasMainWave.leftEnemyText.gameObject.SetActive(Battle.Field.leftEnemy > 0);
                    _viewCanvasMainWave.leftEnemyText.text = string.Format("{0}", Battle.Field.leftEnemy);
                    _viewCanvasMainWave.monsterImage.gameObject.SetActive(true);
                    _viewCanvasMainWave.goToBossObj.gameObject.SetActive(false);
                }
                else
                {
                    if (Battle.Field.leftEnemy <= 0)
                    {
                        _viewCanvasMainWave.leftEnemyText.gameObject.SetActive(false);
                        _viewCanvasMainWave.monsterImage.gameObject.SetActive(false);
                        _viewCanvasMainWave.goToBossObj.gameObject.SetActive(true);
                    }
                    else
                    {
                        _viewCanvasMainWave.leftEnemyText.gameObject.SetActive(true);
                        _viewCanvasMainWave.monsterImage.gameObject.SetActive(true);
                        _viewCanvasMainWave.goToBossObj.gameObject.SetActive(false);
                    }
                }

                await UniTask.Yield(_cts.Token);
            }
        }

        async UniTaskVoid StageUpdate()
        {
            while (true)
            {
                int bestIndex = Player.Cloud.field.bestChapter * 5 + Player.Cloud.field.bestStage;
                bool rewardPossible = bestIndex > Player.Cloud.chapterRewardedData.LastRewardIndex;
                if (rewardPossible)
                {
                    _viewCanvasMainWave.stageRedDot.SetActive(true);
                }
                else
                {
                    _viewCanvasMainWave.stageRedDot.SetActive(false);
                }
                await UniTask.Yield(_cts.Token);
            }
        }

      
        void VipActivate()
        {
            if (Player.Cloud.inAppPurchase.purchaseVip > 0)
            {
                if (Player.Cloud.inAppPurchase.purchaseVip == 1)
                {
                    _viewCanvasMainIcons.vipIcon.sprite = GoodResourcesBundle.Loaded.rewardSprites[(int)RewardTypes.Vip_1];
                }
                if (Player.Cloud.inAppPurchase.purchaseVip == 2)
                {
                    _viewCanvasMainIcons.vipIcon.sprite = GoodResourcesBundle.Loaded.rewardSprites[(int)RewardTypes.Vip_2];
                }
                if (Player.Cloud.inAppPurchase.purchaseVip == 3)
                {
                    _viewCanvasMainIcons.vipIcon.sprite = GoodResourcesBundle.Loaded.rewardSprites[(int)RewardTypes.Vip_3];
                }

                if (_viewCanvasMainIcons.vipWindowOpenBtn.gameObject.activeInHierarchy == false)
                {
                    _viewCanvasMainIcons.vipWindowOpenBtn.gameObject.SetActive(true);
                }
            }
            else
            {
                _viewCanvasMainIcons.vipWindowOpenBtn.gameObject.SetActive(false);
            }
        }

    
    }

}
