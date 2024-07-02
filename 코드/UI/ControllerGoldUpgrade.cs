using UnityEngine;
using BlackTree.Definition;
using System.Threading;
using Cysharp.Threading.Tasks;
using BlackTree.Bundles;
using BlackTree.Model;

namespace BlackTree.Core
{
    public enum LvupType
    {
        lvup_1,
        lvup_10,
        lvup_Max,
    }
    public class ControllerGoldUpgrade
    {
        private readonly ViewCanvasGoldUpgrade _viewGoldUpgrade;
        private readonly GoldUpgradeSlotView[] _commonUpgradeListViews;
        private readonly GoldUpgradeSlotView[] _tierSecUpgradeListViews;

        private const int shieldUnLockLevel= 100;
        private const int criUnLockLevel = 100;
        private const int superUnLockLevel = 1000;
        private const int witchatkSpeedUnlockLevel = 50;
        private const int CharacterAwaketimeUnlockLevel = 200;

        LvupType currentSelectlvUpType;

        int currentShowingTier;
        const float slotHeight = 184;
        public ControllerGoldUpgrade(Transform parent)
        {
            _viewGoldUpgrade = ViewCanvas.Create<ViewCanvasGoldUpgrade>(parent);
            _viewGoldUpgrade.SetDesc(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ETC_InitCommonUpgrade].StringToLocal);

            _commonUpgradeListViews = new GoldUpgradeSlotView[StaticData.Wrapper.goldUpgradedatas.Length];
            for (int i = 0; i < _commonUpgradeListViews.Length; ++i)
            {
                var key = (GoldUpgradeKey)i;
                var data = StaticData.Wrapper.goldUpgradedatas[i];

                _commonUpgradeListViews[i] = ViewBase.Create<GoldUpgradeSlotView>(_viewGoldUpgrade.scrollRect.content);
                _commonUpgradeListViews[i].SetOnClickUpgrade(() => TryUpgrade(key));
                _commonUpgradeListViews[i].SetOnClickDownRepeatUpgrade(() => TemporaryUpgrade(key));
                _commonUpgradeListViews[i].SetOnClickUpUpgrade(() => Player.GoldUpgrade.UpdateSyncData());

                _commonUpgradeListViews[i].SetLockImage(false);
                int index = i;

                if (IslockedFirst(key))
                {
                    _commonUpgradeListViews[i].SetLockImage(true);
                }
                if(key==GoldUpgradeKey.IncreaseShield|| key == GoldUpgradeKey.IncreaseShieldRecover)
                {
                    _commonUpgradeListViews[i].SetOnClickLockBtn(()=> {
                        string localizedvalue =string.Format(StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_HpLevelComplete].StringToLocal, shieldUnLockLevel);
                        ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                    });
                }
                if (key == GoldUpgradeKey.IncreaseCriticalDmg || key == GoldUpgradeKey.IncreaseCriticalRate)
                {
                    _commonUpgradeListViews[i].SetOnClickLockBtn(() => {
                        string localizedvalue = string.Format(StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_AttackLevelComplete].StringToLocal, criUnLockLevel);
                        ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                    });
                }
                if (key == GoldUpgradeKey.IncreaseSuperRate || key == GoldUpgradeKey.IncreaseSuperDmg)
                {
                    _commonUpgradeListViews[i].SetOnClickLockBtn(() => {
                        string localizedvalue = string.Format(StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_CriRateLevelComplete].StringToLocal, superUnLockLevel);
                        ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                    });
                }
                if(key==GoldUpgradeKey.IncreaseMaxAtkSpeedForWitch)
                {
                    _commonUpgradeListViews[i].SetOnClickLockBtn(() => {
                        string localizedvalue = string.Format(StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_AttackLevelComplete].StringToLocal, witchatkSpeedUnlockLevel);
                        ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                    });
                }
                if (key == GoldUpgradeKey.IncreaseCharacterAwakeTime)
                {
                    _commonUpgradeListViews[i].SetOnClickLockBtn(() => {
                        string localizedvalue = string.Format(StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_WitchAttackSpeedLevelComplete].StringToLocal, CharacterAwaketimeUnlockLevel);
                        ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                    });
                }
            }

            _tierSecUpgradeListViews = new GoldUpgradeSlotView[StaticData.Wrapper.tierSecgoldUpgradedatas.Length];
            for (int i = 0; i < _tierSecUpgradeListViews.Length; ++i)
            {
                var key = (Tier2GoldUpgradeKey)i;
                var data = StaticData.Wrapper.tierSecgoldUpgradedatas[i];

                _tierSecUpgradeListViews[i] = ViewBase.Create<GoldUpgradeSlotView>(_viewGoldUpgrade.scrollRect.content);
                _tierSecUpgradeListViews[i].SetOnClickUpgrade(() => TryUpgrade(key));
                _tierSecUpgradeListViews[i].SetOnClickDownRepeatUpgrade(() => TemporaryUpgrade(key));
                _tierSecUpgradeListViews[i].SetOnClickUpUpgrade(() => Player.SecondGoldUpgrade.UpdateSyncData());

                _tierSecUpgradeListViews[i].SetLockImage(false);
            }

            currentSelectlvUpType = LvupType.lvup_1;
            LvupTypeSelect(LvupType.lvup_1);

            _viewGoldUpgrade.lvup_1.onClick.AddListener(()=> { LvupTypeSelect(LvupType.lvup_1); });
            _viewGoldUpgrade.lvup_10.onClick.AddListener(() => { LvupTypeSelect(LvupType.lvup_10); });
            _viewGoldUpgrade.lvup_max.onClick.AddListener(() => { LvupTypeSelect(LvupType.lvup_Max); });

            Player.GoldUpgrade.UpdateSyncactions += (goldUpKey) => { UpdateListView(goldUpKey, currentSelectlvUpType); };
            Player.GoldUpgrade.OnResetUpgrade += UpdateAllListView;

            Player.SecondGoldUpgrade.UpdateSyncactions += (goldUpKey) => { UpdateListView(goldUpKey, currentSelectlvUpType); };


            _viewGoldUpgrade.nextTier.onClick.AddListener(NextTier);
            _viewGoldUpgrade.prevTier.onClick.AddListener(PrevTier);


            currentShowingTier =1;
            for(int i=0; i< (int)GoldUpgradeKey.End; i++)
            {
                if(Player.GoldUpgrade.GetIsMaxLevel((GoldUpgradeKey)i)==false)
                {
                    currentShowingTier = 0;
                    break;
                }
            }

            if(currentShowingTier==0)
            {
                _viewGoldUpgrade.tierText.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_FirstTier].StringToLocal;
            }
            else
            {
                _viewGoldUpgrade.tierText.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_SecondTier].StringToLocal;
            }
            UpdateAllListView();

            Player.Guide.questGuideAction +=o=> { GuideArrowObject(); };

            _viewGoldUpgrade.BindOnChangeVisible((active)=> { 
                if(active)
                {
                    GuideArrowObject();
                }
            
            });

            Main().Forget();
            MainUIActive().Forget();
        }


        void GuideArrowObject()
        {
            if (Player.Quest.CurrentMainQuestType == Definition.QuestType.GoldUpgrade_Attack)
            {
                _viewGoldUpgrade.scrollRect.SetContentScrollOffsetToTop();
                _commonUpgradeListViews[(int)GoldUpgradeKey.AttackIncrease].SetArrowObjectAnim();
            }
            if (Player.Quest.CurrentMainQuestType == Definition.QuestType.GoldUpgrade_MaxHp)
            {
                _viewGoldUpgrade.scrollRect.SetContentScrollOffsetToTop();
                _commonUpgradeListViews[(int)GoldUpgradeKey.IncreaseHp].SetArrowObjectAnim();
            }
            if (Player.Quest.CurrentMainQuestType == Definition.QuestType.GoldUpgrade_Hprecover)
            {
                _viewGoldUpgrade.scrollRect.SetContentScrollOffsetToTop();
                _commonUpgradeListViews[(int)GoldUpgradeKey.IncreaseHpRecover].SetArrowObjectAnim();
            }
            if (Player.Quest.CurrentMainQuestType == Definition.QuestType.GoldUpgrade_CriRate)
            {
                _viewGoldUpgrade.scrollRect.SetContentScrollOffsetToBottom(slotHeight * 7);
                _commonUpgradeListViews[(int)GoldUpgradeKey.IncreaseCriticalRate].SetArrowObjectAnim();
            }
            if (Player.Quest.CurrentMainQuestType == Definition.QuestType.GoldUpgrade_MaxShield)
            {
                _viewGoldUpgrade.scrollRect.SetContentScrollOffsetToTop();
                _commonUpgradeListViews[(int)GoldUpgradeKey.IncreaseShield].SetArrowObjectAnim();
            }
            if (Player.Quest.CurrentMainQuestType == Definition.QuestType.GoldUpgrade_ShieldRecover)
            {
                _viewGoldUpgrade.scrollRect.SetContentScrollOffsetToTop();
                _commonUpgradeListViews[(int)GoldUpgradeKey.IncreaseShieldRecover].SetArrowObjectAnim();
            }
            if (Player.Quest.CurrentMainQuestType == Definition.QuestType.WitchSpeedIncrease)
            {
                _viewGoldUpgrade.scrollRect.SetContentScrollOffsetToTop();
                _commonUpgradeListViews[(int)GoldUpgradeKey.IncreaseMaxAtkSpeedForWitch].SetArrowObjectAnim();
            }
            if (Player.Quest.CurrentMainQuestType == Definition.QuestType.WitchSpeedMaxTimeIncrease)
            {
                _viewGoldUpgrade.scrollRect.SetContentScrollOffsetToBottom(slotHeight * 7);
                _commonUpgradeListViews[(int)GoldUpgradeKey.IncreaseCharacterAwakeTime].SetArrowObjectAnim();
            }
           
        }
        void NextTier()
        {
            bool isMax = true;
            for (int i = 0; i < (int)GoldUpgradeKey.End; i++)
            {
                if (Player.GoldUpgrade.GetIsMaxLevel((GoldUpgradeKey)i) == false)
                {
                    isMax = false;
                    break;
                }
            }
            if (isMax==false)
            {
                {
                    string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_1TierUpgradeNeed].StringToLocal;
                    ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                    return;
                }
            }
           
            currentShowingTier++;
            if (currentShowingTier >= 1)
            {
                currentShowingTier = 1;
            }
         
            _viewGoldUpgrade.tierText.text = string.Format("{0} Tire", currentShowingTier + 1);
            UpdateAllListView();
        }
        void PrevTier()
        {
            currentShowingTier--;
            if (currentShowingTier <= 0)
            {
                currentShowingTier = 0;
            }

            _viewGoldUpgrade.tierText.text = string.Format("{0} Tire", currentShowingTier + 1);
            UpdateAllListView();
        }

        void LvupTypeSelect(LvupType lvupType)
        {
            if (currentSelectlvUpType == lvupType)
                return;
            currentSelectlvUpType = lvupType;

            switch (lvupType)
            {
                case LvupType.lvup_1:
                    _viewGoldUpgrade.lvTypeSelector.Show(0);
                    break;
                case LvupType.lvup_10:
                    _viewGoldUpgrade.lvTypeSelector.Show(1);
                    break;
                case LvupType.lvup_Max:
                    _viewGoldUpgrade.lvTypeSelector.Show(2);
                    break;
                default:
                    break;
            }

            UpdateAllListView();
        }

        bool IslockedFirst(GoldUpgradeKey upgradekey)
        {
            bool locked = false;
            if(upgradekey==GoldUpgradeKey.IncreaseMaxAtkSpeedForWitch||
                (int)upgradekey>= (int)GoldUpgradeKey.IncreaseShield)
            {
                locked = true;
            }
            return locked;
        }
        async UniTaskVoid Main()
        {
            while (true)
            {
                Player.GoldUpgrade.canUpgrade = false;
                for (int i = 0; i < _commonUpgradeListViews.Length; ++i)
                {
                    var key = (GoldUpgradeKey)i;
                    var data = StaticData.Wrapper.goldUpgradedatas[i];

                    var cost = Player.GoldUpgrade.GetCost(key);
                    bool reddot = Player.ControllerGood.IsCanBuy(data.goodKey, cost);

                    if (Player.GoldUpgrade.GetIsMaxLevel(key))
                    {
                        reddot = false;
                    }

                    if (reddot)
                    {
                        Player.GoldUpgrade.canUpgrade =true;
                    }
                }

                if (Player.GoldUpgrade.canUpgrade == false)
                {
                    for (int i = 0; i < _tierSecUpgradeListViews.Length; ++i)
                    {
                        var key = (Tier2GoldUpgradeKey)i;
                        var data = StaticData.Wrapper.tierSecgoldUpgradedatas[i];

                        var cost = Player.SecondGoldUpgrade.GetCost(key);
                        bool reddot = Player.ControllerGood.IsCanBuy(data.goodKey, cost);

                        if (Player.SecondGoldUpgrade.GetIsMaxLevel(key))
                        {
                            reddot = false;
                        }

                        if (reddot)
                        {
                            Player.GoldUpgrade.canUpgrade = true;
                        }
                    }
                }

                bool isRedDot = Model.Player.AwakeUpgrade.canUpgrade || Model.Player.GoldUpgrade.canUpgrade || Model.Player.StatusUpgrade.canUpgrade;

                ViewCanvas.Get<ViewCanvasMainNav>().viewMainNavButtons[(int)MainNavigationType.Upgrade].ActivateNotification(isRedDot);

                await UniTask.DelayFrame(10);
            }
        }

        async UniTaskVoid MainUIActive()
        {
            while (true)
            {
                if (_viewGoldUpgrade.IsVisible)
                {
                    if(currentShowingTier==0)
                    {
                        for (int i = 0; i < _commonUpgradeListViews.Length; ++i)
                        {
                            var key = (GoldUpgradeKey)i;
                            var data = StaticData.Wrapper.goldUpgradedatas[i];

                            Player.GoldUpgrade.GoldupgradeValueForLvType costLv = null;
                            string desc = StringFormat(key, StaticData.Wrapper.localizeddesclist[(int)data.descLmk].StringToLocal);
                            switch (currentSelectlvUpType)
                            {
                                case LvupType.lvup_1:
                                    costLv = Player.GoldUpgrade.GetCostLv(key, 1);
                                    break;
                                case LvupType.lvup_10:
                                    costLv = Player.GoldUpgrade.GetCostLv(key, 10);
                                    desc = desc + string.Format(StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.UI_GoldUpgradeDesc].StringToLocal, costLv.lv, Player.GoldUpgrade.GetValue(key, costLv.lv).ToNumberString());
                                    break;
                                case LvupType.lvup_Max:
                                    costLv = Player.GoldUpgrade.GetMaxCostLv(key);
                                    desc = desc + string.Format(StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.UI_GoldUpgradeDesc].StringToLocal, costLv.lv, Player.GoldUpgrade.GetValue(key, costLv.lv).ToNumberString());
                                    break;
                                default:
                                    break;
                            }

                            GoodsValue _goodValue = new GoodsValue(data.goodKey, costLv.cost);
                            bool reddot = Player.ControllerGood.IsCanBuy(data.goodKey, costLv.cost);

                            if (Player.GoldUpgrade.GetIsMaxLevel(key))
                            {
                                reddot = false;
                            }
                            _commonUpgradeListViews[i]
                                .SetGoodValue(_goodValue)
                                .SetDescription(desc)
                                .SetRedDot(reddot);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < _tierSecUpgradeListViews.Length; ++i)
                        {
                            var key = (Tier2GoldUpgradeKey)i;
                            var data = StaticData.Wrapper.tierSecgoldUpgradedatas[i];

                            Player.SecondGoldUpgrade.GoldupgradeValueForLvType costLv = null;
                            string desc = StringFormat(key, StaticData.Wrapper.localizeddesclist[(int)data.descLmk].StringToLocal);
                            switch (currentSelectlvUpType)
                            {
                                case LvupType.lvup_1:
                                    costLv = Player.SecondGoldUpgrade.GetCostLv(key, 1);
                                    break;
                                case LvupType.lvup_10:
                                    costLv = Player.SecondGoldUpgrade.GetCostLv(key, 10);
                                    desc = desc + string.Format(StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.UI_GoldUpgradeDesc].StringToLocal, costLv.lv, Player.SecondGoldUpgrade.GetValue(key, costLv.lv).ToNumberString());
                                    break;
                                case LvupType.lvup_Max:
                                    costLv = Player.SecondGoldUpgrade.GetMaxCostLv(key);
                                    desc = desc + string.Format(StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.UI_GoldUpgradeDesc].StringToLocal, costLv.lv, Player.SecondGoldUpgrade.GetValue(key, costLv.lv).ToNumberString());
                                    break;
                                default:
                                    break;
                            }

                            GoodsValue _goodValue = new GoodsValue(data.goodKey, costLv.cost);
                            bool reddot = Player.ControllerGood.IsCanBuy(data.goodKey, costLv.cost);

                            if (Player.SecondGoldUpgrade.GetIsMaxLevel(key))
                            {
                                reddot = false;
                            }
                            _tierSecUpgradeListViews[i]
                                .SetGoodValue(_goodValue)
                                .SetDescription(desc)
                                .SetRedDot(reddot);
                        }
                    }
                
                }

                await UniTask.DelayFrame(20);
            }
        }

        private bool TryUpgrade(GoldUpgradeKey key)
        {
            var index = (int)key;
            var isMaxLevel = Player.GoldUpgrade.GetIsMaxLevel(key);
            var data = StaticData.Wrapper.goldUpgradedatas[index];

            if (isMaxLevel) return false;

            int lvupCount = 1;
            Player.GoldUpgrade.GoldupgradeValueForLvType costLv = null;
            switch (currentSelectlvUpType)
            {
                case LvupType.lvup_1:
                    costLv = Player.GoldUpgrade.GetCostLv(key, 1);
                    lvupCount = costLv.lv - Player.GoldUpgrade.GetLevel(key);
                    break;
                case LvupType.lvup_10:
                    costLv = Player.GoldUpgrade.GetCostLv(key, 10);
                    lvupCount = costLv.lv - Player.GoldUpgrade.GetLevel(key);
                    break;
                case LvupType.lvup_Max:
                    costLv = Player.GoldUpgrade.GetMaxCostLv(key);
                    lvupCount = costLv.lv - Player.GoldUpgrade.GetLevel(key);
                    break;
                default:
                    break;
            }

            if (!Player.ControllerGood.IsCanBuy(data.goodKey, costLv.cost) || lvupCount<=0)
            {
                string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_GoldNotEnough].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                return false;
            }
                

            Player.ControllerGood.Consume(data.goodKey, costLv.cost);
            Player.GoldUpgrade.Upgrade(key, lvupCount);

            Player.Quest.TryCountUp(QuestType.GoldUpgrade, lvupCount);
            if(key==GoldUpgradeKey.AttackIncrease)
            {
                Player.Quest.TryCountUp(QuestType.GoldUpgrade_Attack, lvupCount);
            }
            if (key == GoldUpgradeKey.IncreaseHpRecover)
            {
                Player.Quest.TryCountUp(QuestType.GoldUpgrade_Hprecover, lvupCount);
            }
            if (key == GoldUpgradeKey.IncreaseHp)
            {
                Player.Quest.TryCountUp(QuestType.GoldUpgrade_MaxHp, lvupCount);
            }

            if (key == GoldUpgradeKey.IncreaseCriticalRate)
            {
                Player.Quest.TryCountUp(QuestType.GoldUpgrade_CriRate, lvupCount);
            }
            if (key == GoldUpgradeKey.IncreaseShield)
            {
                Player.Quest.TryCountUp(QuestType.GoldUpgrade_MaxShield, lvupCount);
            }
            if (key == GoldUpgradeKey.IncreaseShieldRecover)
            {
                Player.Quest.TryCountUp(QuestType.GoldUpgrade_ShieldRecover, lvupCount);
            }
            if (key == GoldUpgradeKey.IncreaseMaxAtkSpeedForWitch)
            {
                Player.Quest.TryCountUp(QuestType.WitchSpeedIncrease, lvupCount);
            }
            if (key == GoldUpgradeKey.IncreaseCharacterAwakeTime)
            {
                Player.Quest.TryCountUp(QuestType.WitchSpeedMaxTimeIncrease, lvupCount);
            }
            return true;
        }

        private bool TemporaryUpgrade(GoldUpgradeKey key)
        {
            var index = (int)key;
            var isMaxLevel = Player.GoldUpgrade.GetIsMaxLevel(key);
            var data = StaticData.Wrapper.goldUpgradedatas[index];

            if (isMaxLevel) 
                return false;
            if (currentSelectlvUpType == LvupType.lvup_Max)
                return false;

            int lvupCount = 1;
            Player.GoldUpgrade.GoldupgradeValueForLvType costLv = null;
            switch (currentSelectlvUpType)
            {
                case LvupType.lvup_1:
                    costLv = Player.GoldUpgrade.GetCostLv(key, 1);
                    lvupCount = costLv.lv - Player.GoldUpgrade.GetLevel(key);
                    break;
                case LvupType.lvup_10:
                    costLv = Player.GoldUpgrade.GetCostLv(key, 10);
                    lvupCount = costLv.lv - Player.GoldUpgrade.GetLevel(key);
                    break;
                case LvupType.lvup_Max:
                    costLv = Player.GoldUpgrade.GetMaxCostLv(key);
                    lvupCount = costLv.lv - Player.GoldUpgrade.GetLevel(key);
                    break;
                default:
                    break;
            }

          

            if (!Player.ControllerGood.IsCanBuy(data.goodKey, costLv.cost) || lvupCount <= 0)
            {
                string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_GoldNotEnough].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                return false;
            }

            Player.ControllerGood.Consume(data.goodKey, costLv.cost);
            Player.GoldUpgrade.TemporaryUpgrade(key, lvupCount);

            Player.Quest.TryCountUp(QuestType.GoldUpgrade, lvupCount);
            

            if (key == GoldUpgradeKey.AttackIncrease)
            {
                Player.Quest.TryCountUp(QuestType.GoldUpgrade_Attack, lvupCount);
            }
            if (key == GoldUpgradeKey.IncreaseHpRecover)
            {
                Player.Quest.TryCountUp(QuestType.GoldUpgrade_Hprecover, lvupCount);
            }
            if (key == GoldUpgradeKey.IncreaseHp)
            {
                Player.Quest.TryCountUp(QuestType.GoldUpgrade_MaxHp, lvupCount);
            }

            if (key == GoldUpgradeKey.IncreaseCriticalRate)
            {
                Player.Quest.TryCountUp(QuestType.GoldUpgrade_CriRate, lvupCount);
            }
            if (key == GoldUpgradeKey.IncreaseShield)
            {
                Player.Quest.TryCountUp(QuestType.GoldUpgrade_MaxShield, lvupCount);
            }
            if (key == GoldUpgradeKey.IncreaseShieldRecover)
            {
                Player.Quest.TryCountUp(QuestType.GoldUpgrade_ShieldRecover, lvupCount);
            }
            if (key == GoldUpgradeKey.IncreaseMaxAtkSpeedForWitch)
            {
                Player.Quest.TryCountUp(QuestType.WitchSpeedIncrease, lvupCount);
            }
            if (key == GoldUpgradeKey.IncreaseCharacterAwakeTime)
            {
                Player.Quest.TryCountUp(QuestType.WitchSpeedMaxTimeIncrease, lvupCount);
            }
            return true;
        }

        private void UpdateListView(GoldUpgradeKey key, LvupType lvupType)
        {
            GoodsValue _goodValue;
            int index = (int)key;
            var data = StaticData.Wrapper.goldUpgradedatas[index];
            var level = Player.GoldUpgrade.GetLevel(key);

            var name = StaticData.Wrapper.localizednamelist[(int)data.nameLmk].StringToLocal
                +string.Format("(Max.{0})", Player.GoldUpgrade.GetMaxLevelText(key));

            var maxLevelTxt = string.Format("Max.\n{0}", Player.GoldUpgrade.GetMaxLevel(key));

            int lvupCount = 1;
            Player.GoldUpgrade.GoldupgradeValueForLvType costLv = null;
            string desc = StringFormat(key, StaticData.Wrapper.localizeddesclist[(int)data.descLmk].StringToLocal);
            switch (lvupType)
            {
                case LvupType.lvup_1:
                    costLv = Player.GoldUpgrade.GetCostLv(key,1);
                    break;
                case LvupType.lvup_10:
                    costLv = Player.GoldUpgrade.GetCostLv(key, 10);
                    desc=desc+string.Format(StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.UI_GoldUpgradeDesc].StringToLocal, costLv.lv, Player.GoldUpgrade.GetValue(key,costLv.lv).ToNumberString());
                    break;
                case LvupType.lvup_Max:
                    costLv = Player.GoldUpgrade.GetMaxCostLv(key);
                    desc = desc + string.Format(StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.UI_GoldUpgradeDesc].StringToLocal, costLv.lv, Player.GoldUpgrade.GetValue(key, costLv.lv).ToNumberString());
                    break;
                default:
                    break;
            }

            _goodValue = new GoodsValue(data.goodKey, costLv.cost);

            bool reddot = Player.ControllerGood.IsCanBuy(data.goodKey, costLv.cost);

            if (Player.GoldUpgrade.GetIsMaxLevel(key))
            {
                reddot = false;
            }

            _commonUpgradeListViews[index]
                .SetName(name)
                .SetDescription(desc)
                .SetIcon((int)key,0)
                .SetGoodValue(_goodValue)
                .SetLevel(level)
                .SetLockForMaxLevel(Player.GoldUpgrade.GetIsMaxLevel(key))
                .SetRedDot(reddot);



            if(key==GoldUpgradeKey.AttackIncrease)
            {
                if(level>= criUnLockLevel)
                {
                    _commonUpgradeListViews[(int)GoldUpgradeKey.IncreaseCriticalRate].SetLockImage(false);
                    _commonUpgradeListViews[(int)GoldUpgradeKey.IncreaseCriticalDmg].SetLockImage(false);
                }
                if (level >= witchatkSpeedUnlockLevel)
                {
                    _commonUpgradeListViews[(int)GoldUpgradeKey.IncreaseMaxAtkSpeedForWitch].SetLockImage(false);
                }
            }
            if (key == GoldUpgradeKey.IncreaseHp)
            {
                if (level >= shieldUnLockLevel)
                {
                    _commonUpgradeListViews[(int)GoldUpgradeKey.IncreaseShield].SetLockImage(false);
                    _commonUpgradeListViews[(int)GoldUpgradeKey.IncreaseShieldRecover].SetLockImage(false);
                }
            }
            if (key == GoldUpgradeKey.IncreaseCriticalRate)
            {
                if (level >= superUnLockLevel)
                {
                    _commonUpgradeListViews[(int)GoldUpgradeKey.IncreaseSuperRate].SetLockImage(false);
                    _commonUpgradeListViews[(int)GoldUpgradeKey.IncreaseSuperDmg].SetLockImage(false);
                }
            }
            if(key==GoldUpgradeKey.IncreaseMaxAtkSpeedForWitch)
            {
                if (level >= CharacterAwaketimeUnlockLevel)
                {
                    _commonUpgradeListViews[(int)GoldUpgradeKey.IncreaseCharacterAwakeTime].SetLockImage(false);
                }
            }
        }

        public string StringFormat(GoldUpgradeKey key, string description)
        {
            int pointCount = ((int)key >= (int)GoldUpgradeKey.IncreaseHp && (int)key <= (int)GoldUpgradeKey.IncreaseShieldRecover) ? 2 : 1;
            string value = null;
            if(key==GoldUpgradeKey.IncreaseCharacterAwakeTime)
            {
                value = Player.GoldUpgrade.GetValue(key).ToNumberString(1);
            }
            else
            {
                value = Player.GoldUpgrade.GetValue(key).ToNumberString();
            }
            var temp = string.Format(description, value);
            return temp;
        }

        private void UpdateAllListView()
        {
            if(currentShowingTier==0)
            {
                for (int i = 0; i < _commonUpgradeListViews.Length; i++)
                {
                    _commonUpgradeListViews[i].gameObject.SetActive(true);
                    var key = (GoldUpgradeKey)i;
                    UpdateListView(key, currentSelectlvUpType);
                }
                for (int i = 0; i < _tierSecUpgradeListViews.Length; i++)
                {
                    _tierSecUpgradeListViews[i].gameObject.SetActive(false);
                }
            }
            else
            {
                for (int i = 0; i < _tierSecUpgradeListViews.Length; i++)
                {
                    _tierSecUpgradeListViews[i].gameObject.SetActive(true);
                    var key = (Tier2GoldUpgradeKey)i;
                    UpdateListView(key, currentSelectlvUpType);
                }
                for (int i = 0; i < _commonUpgradeListViews.Length; i++)
                {
                    _commonUpgradeListViews[i].gameObject.SetActive(false);
                }
            }
            
        }










        private bool TryUpgrade(Tier2GoldUpgradeKey key)
        {
            var index = (int)key;
            var isMaxLevel = Player.SecondGoldUpgrade.GetIsMaxLevel(key);
            var data = StaticData.Wrapper.tierSecgoldUpgradedatas[index];

            if (isMaxLevel) return false;

            int lvupCount = 1;
            Player.SecondGoldUpgrade.GoldupgradeValueForLvType costLv = null;
            switch (currentSelectlvUpType)
            {
                case LvupType.lvup_1:
                    costLv = Player.SecondGoldUpgrade.GetCostLv(key, 1);
                    lvupCount = costLv.lv - Player.SecondGoldUpgrade.GetLevel(key);
                    break;
                case LvupType.lvup_10:
                    costLv = Player.SecondGoldUpgrade.GetCostLv(key, 10);
                    lvupCount = costLv.lv - Player.SecondGoldUpgrade.GetLevel(key);
                    break;
                case LvupType.lvup_Max:
                    costLv = Player.SecondGoldUpgrade.GetMaxCostLv(key);
                    lvupCount = costLv.lv - Player.SecondGoldUpgrade.GetLevel(key);
                    break;
                default:
                    break;
            }

            if (!Player.ControllerGood.IsCanBuy(data.goodKey, costLv.cost) || lvupCount <= 0)
            {
                string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_GoldNotEnough].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                return false;
            }


            Player.ControllerGood.Consume(data.goodKey, costLv.cost);
            Player.SecondGoldUpgrade.Upgrade(key, lvupCount);

            Player.Quest.TryCountUp(QuestType.GoldUpgrade, lvupCount);
      
            return true;
        }

        private bool TemporaryUpgrade(Tier2GoldUpgradeKey key)
        {
            var index = (int)key;
            var isMaxLevel = Player.SecondGoldUpgrade.GetIsMaxLevel(key);
            var data = StaticData.Wrapper.tierSecgoldUpgradedatas[index];

            if (isMaxLevel)
                return false;
            if (currentSelectlvUpType == LvupType.lvup_Max)
                return false;

            int lvupCount = 1;
            Player.SecondGoldUpgrade.GoldupgradeValueForLvType costLv = null;
            switch (currentSelectlvUpType)
            {
                case LvupType.lvup_1:
                    costLv = Player.SecondGoldUpgrade.GetCostLv(key, 1);
                    lvupCount = costLv.lv - Player.SecondGoldUpgrade.GetLevel(key);
                    break;
                case LvupType.lvup_10:
                    costLv = Player.SecondGoldUpgrade.GetCostLv(key, 10);
                    lvupCount = costLv.lv - Player.SecondGoldUpgrade.GetLevel(key);
                    break;
                case LvupType.lvup_Max:
                    costLv = Player.SecondGoldUpgrade.GetMaxCostLv(key);
                    lvupCount = costLv.lv - Player.SecondGoldUpgrade.GetLevel(key);
                    break;
                default:
                    break;
            }



            if (!Player.ControllerGood.IsCanBuy(data.goodKey, costLv.cost) || lvupCount <= 0)
            {
                string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_GoldNotEnough].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                return false;
            }

            Player.ControllerGood.Consume(data.goodKey, costLv.cost);
            Player.SecondGoldUpgrade.TemporaryUpgrade(key, lvupCount);

            Player.Quest.TryCountUp(QuestType.GoldUpgrade, lvupCount);
            return true;
        }

        private void UpdateListView(Tier2GoldUpgradeKey key, LvupType lvupType)
        {
            GoodsValue _goodValue;
            int index = (int)key;
            var data = StaticData.Wrapper.tierSecgoldUpgradedatas[index];
            var level = Player.SecondGoldUpgrade.GetLevel(key);

            var name = StaticData.Wrapper.localizednamelist[(int)data.nameLmk].StringToLocal
                + string.Format("(Max.{0})", Player.SecondGoldUpgrade.GetMaxLevelText(key));

            var maxLevelTxt = string.Format("Max.\n{0}", Player.SecondGoldUpgrade.GetMaxLevel(key));

            int lvupCount = 1;
            Player.SecondGoldUpgrade.GoldupgradeValueForLvType costLv = null;
            string desc = StringFormat(key, StaticData.Wrapper.localizeddesclist[(int)data.descLmk].StringToLocal);
            switch (lvupType)
            {
                case LvupType.lvup_1:
                    costLv = Player.SecondGoldUpgrade.GetCostLv(key, 1);
                    break;
                case LvupType.lvup_10:
                    costLv = Player.SecondGoldUpgrade.GetCostLv(key, 10);
                    desc = desc + string.Format(StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.UI_GoldUpgradeDesc].StringToLocal, costLv.lv, Player.SecondGoldUpgrade.GetValue(key, costLv.lv).ToNumberString());
                    break;
                case LvupType.lvup_Max:
                    costLv = Player.SecondGoldUpgrade.GetMaxCostLv(key);
                    desc = desc + string.Format(StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.UI_GoldUpgradeDesc].StringToLocal, costLv.lv, Player.SecondGoldUpgrade.GetValue(key, costLv.lv).ToNumberString());
                    break;
                default:
                    break;
            }

            _goodValue = new GoodsValue(data.goodKey, costLv.cost);

            bool reddot = Player.ControllerGood.IsCanBuy(data.goodKey, costLv.cost);

            if (Player.SecondGoldUpgrade.GetIsMaxLevel(key))
            {
                reddot = false;
            }

            _tierSecUpgradeListViews[index]
                .SetName(name)
                .SetDescription(desc)
                .SetIcon((int)key,1)
                .SetGoodValue(_goodValue)
                .SetLevel(level)
                .SetLockForMaxLevel(Player.SecondGoldUpgrade.GetIsMaxLevel(key))
                .SetRedDot(reddot);
        }

        public string StringFormat(Tier2GoldUpgradeKey key, string description)
        {
            string value = null;
            value = Player.SecondGoldUpgrade.GetValue(key).ToNumberString();
            var temp = string.Format(description, value);
            return temp;
        }

    }

}
