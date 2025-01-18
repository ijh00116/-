using UnityEngine;
using BlackTree.Definition;
using Cysharp.Threading.Tasks;
using BlackTree.Bundles;
using BlackTree.Model;
using System.Collections.Generic;


namespace BlackTree.Core
{
    public class ControllerStatusUpgrade
    {
        enum tierType
        {
            tier1,tier2,
        }

        private readonly ViewCanvasStatusUpgrade _viewCanvasstatusUpgrade;
        private readonly ViewUpgradeSlot[] _statusUpgradeListViews;

        StatusUpgradeKey currentStatUpgradeKey;

        List<StatusSlotListLevelView> StatusLevelslotList = new List<StatusSlotListLevelView>();

        LvupType currentSelectlvUpType;
        tierType currentTierType;
        public ControllerStatusUpgrade(Transform parent)
        {
            _viewCanvasstatusUpgrade = ViewCanvas.Create<ViewCanvasStatusUpgrade>(parent);

            _viewCanvasstatusUpgrade.Init();
           

            for (int i = 0; i <= Player.StatusUpgrade.MaxGradeLevel; ++i)
            {
                int index = i;
                var levelSlot = UnityEngine.Object.Instantiate(_viewCanvasstatusUpgrade.slotListLevelViewPrefab);
                levelSlot.transform.SetParent(_viewCanvasstatusUpgrade.scrollRects.content,false);
                levelSlot.Init(index);

                StatusLevelslotList.Add(levelSlot);
            }

            _statusUpgradeListViews = new ViewUpgradeSlot[StaticData.Wrapper.statusUpgradedatas.Length];
            for (int i = 0; i < _statusUpgradeListViews.Length; ++i)
            {
                var key = StaticData.Wrapper.statusUpgradedatas[i].key;
                var data = StaticData.Wrapper.statusUpgradedatas[i];

                var parentObject = StatusLevelslotList[StaticData.Wrapper.statusUpgradedatas[i].upGradeGrade].parent;

                _statusUpgradeListViews[i] = ViewBase.Create<ViewUpgradeSlot>(parentObject);
                _statusUpgradeListViews[i].SetOnClickUpgrade(() => { 
                    currentStatUpgradeKey = key;
                    _viewCanvasstatusUpgrade.DetailInfoUpdate(key, currentSelectlvUpType);
                });
                UpdateSlotView(key);
            }


            for (int i=0; i< StatusLevelslotList.Count; i++)
            {
                if (i == 0)
                {
                    StatusLevelslotList[i].lockedObject.gameObject.SetActive(false);
                }
                else
                {
                    StatusLevelslotList[i].lockedObject.gameObject.SetActive(true);
                }
            }

            for (int i = 0; i < StatusLevelslotList.Count; i++)
            {
                int index = i;
                StatusLevelslotList[i].lockedObject.onClick.AddListener(() => {
                    string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_UnlockAtBeforeLevelComplete].StringToLocal;
                    ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                });
            }
            _viewCanvasstatusUpgrade.statUpgradeBtn.onClick.AddListener(()=> { TryStatUpgrade(currentStatUpgradeKey); });
            _viewCanvasstatusUpgrade.statUpgradeBtn.onClickDown.AddListener(() => { TryStatTemporaryUpgrade(currentStatUpgradeKey); });
            _viewCanvasstatusUpgrade.statUpgradeBtn.onClickUp.AddListener(() => { Player.StatusUpgrade.UpdateSyncData(); });

            Player.StatusUpgrade.UpdateSyncactions += o => { UpdateStatListView(o, currentSelectlvUpType); };

            _viewCanvasstatusUpgrade.lvup_1.onClick.AddListener(() => { LvupTypeSelect(LvupType.lvup_1); });
            _viewCanvasstatusUpgrade.lvup_10.onClick.AddListener(() => { LvupTypeSelect(LvupType.lvup_10); });
            _viewCanvasstatusUpgrade.lvup_max.onClick.AddListener(() => { LvupTypeSelect(LvupType.lvup_Max); });

            _viewCanvasstatusUpgrade.nextTier.onClick.AddListener(NextTierClick);
            _viewCanvasstatusUpgrade.prevTier.onClick.AddListener(PrevTierClick);

            LvupTypeSelect(LvupType.lvup_1);
            UpdateLockedGrade();

            currentTierType = tierType.tier1;
            TierChange(currentTierType);

            Main().Forget();
        }

        void NextTierClick()
        {
            if(Player.StatusUpgrade.GetIsMaxLevel(StatusUpgradeKey.AttackIncrease_5))
            {
                TierChange(tierType.tier2);
            }
            else
            {
                string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_1TierUpgradeNeed].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
            }
        }
        void PrevTierClick()
        {
            TierChange(tierType.tier1);
        }

        void TierChange(tierType tier)
        {
            currentTierType = tier;
            for (int i=0; i<StatusLevelslotList.Count; i++)
            {
                StatusLevelslotList[i].gameObject.SetActive(false);
            }
            switch (currentTierType)
            {
                case tierType.tier1:
                    for (int i = 0; i <= Player.StatusUpgrade.tier1GradeLevel; i++)
                    {
                        StatusLevelslotList[i].gameObject.SetActive(true);
                    }
                    _viewCanvasstatusUpgrade.tierText.text= StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_FirstTier].StringToLocal;
                    break;
                case tierType.tier2:
                    for (int i = Player.StatusUpgrade.tier1GradeLevel+1; i <= Player.StatusUpgrade.MaxGradeLevel; i++)
                    {
                        StatusLevelslotList[i].gameObject.SetActive(true);
                    }
                    _viewCanvasstatusUpgrade.tierText.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_SecondTier].StringToLocal;
                    break;
                default:
                    break;
            }
        }
        async UniTaskVoid Main()
        {
            while (true)
            {
                Player.StatusUpgrade.canUpgrade = false;
                for (int i = 0; i < _statusUpgradeListViews.Length; ++i)
                {
                    var key = (StatusUpgradeKey)i;
                    var data = StaticData.Wrapper.statusUpgradedatas[i];

                    var cost = Player.StatusUpgrade.GetCost(key);
                    bool reddot = Player.ControllerGood.IsCanBuy(data.goodKey, cost);
                    if (reddot)
                    {
                        Player.StatusUpgrade.canUpgrade = true;
                    }
                }
                bool isRedDot = Model.Player.AwakeUpgrade.canUpgrade || Model.Player.GoldUpgrade.canUpgrade || Model.Player.StatusUpgrade.canUpgrade;
                ViewCanvas.Get<ViewCanvasMainNav>().viewMainNavButtons[(int)MainNavigationType.Upgrade].ActivateNotification(isRedDot);

                await UniTask.DelayFrame(60);
            }
        }

        void LvupTypeSelect(LvupType lvupType)
        {
            if (currentSelectlvUpType == lvupType)
                return;
            currentSelectlvUpType = lvupType;

            switch (lvupType)
            {
                case LvupType.lvup_1:
                    _viewCanvasstatusUpgrade.lvTypeSelector.Show(0);
                    break;
                case LvupType.lvup_10:
                    _viewCanvasstatusUpgrade.lvTypeSelector.Show(1);
                    break;
                case LvupType.lvup_Max:
                    _viewCanvasstatusUpgrade.lvTypeSelector.Show(2);
                    break;
                default:
                    break;
            }

            if(_viewCanvasstatusUpgrade.detailWindow.activeInHierarchy)
            {
                UpdateStatListView(currentStatUpgradeKey, lvupType);
            }
            

        }

        #region ½ºÅÈ ¾÷±Û
        private bool TryStatUpgrade(StatusUpgradeKey key)
        {
            var index = (int)key;
            var isMaxLevel = Player.StatusUpgrade.GetIsMaxLevel(key);
            var data = StaticData.Wrapper.statusUpgradedatas[index];

            if (isMaxLevel)
            {
                string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_LevelupComplete].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                return false;
            }

            int lvupCount = 1;
            Player.StatusUpgrade.StatupgradeValueForLvType costLv = null;
            switch (currentSelectlvUpType)
            {
                case LvupType.lvup_1:
                    costLv = Player.StatusUpgrade.GetCostLv(key, 1);
                    lvupCount = costLv.lv - Player.StatusUpgrade.GetLevel(key);
                    break;
                case LvupType.lvup_10:
                    costLv = Player.StatusUpgrade.GetCostLv(key, 10);
                    lvupCount = costLv.lv - Player.StatusUpgrade.GetLevel(key);
                    break;
                case LvupType.lvup_Max:
                    costLv = Player.StatusUpgrade.GetMaxCostLv(key);
                    lvupCount = costLv.lv - Player.StatusUpgrade.GetLevel(key);
                    break;
                default:
                    break;
            }

            if (!Player.ControllerGood.IsCanBuy(data.goodKey, costLv.cost) || lvupCount <= 0)
            {
                string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_StatusPointNotEnough].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                return false;
            }

            Player.ControllerGood.Consume(data.goodKey, costLv.cost);
            Player.StatusUpgrade.Upgrade(key, lvupCount);

            UpdateLockedGrade();

            return true;
        }

        private bool TryStatTemporaryUpgrade(StatusUpgradeKey key)
        {
            var index = (int)key;
            var isMaxLevel = Player.StatusUpgrade.GetIsMaxLevel(key);
            var data = StaticData.Wrapper.statusUpgradedatas[index];

            if (isMaxLevel)
            {
                string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_LevelupComplete].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                return false;
            }

            int lvupCount = 1;
            Player.StatusUpgrade.StatupgradeValueForLvType costLv = null;
            switch (currentSelectlvUpType)
            {
                case LvupType.lvup_1:
                    costLv = Player.StatusUpgrade.GetCostLv(key, 1);
                    lvupCount = costLv.lv - Player.StatusUpgrade.GetLevel(key);
                    break;
                case LvupType.lvup_10:
                    costLv = Player.StatusUpgrade.GetCostLv(key, 10);
                    lvupCount = costLv.lv - Player.StatusUpgrade.GetLevel(key);
                    break;
                case LvupType.lvup_Max:
                    costLv = Player.StatusUpgrade.GetMaxCostLv(key);
                    lvupCount = costLv.lv - Player.StatusUpgrade.GetLevel(key);
                    break;
                default:
                    break;
            }

            if (!Player.ControllerGood.IsCanBuy(data.goodKey, costLv.cost) || lvupCount <= 0)
            {
                string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_StatusPointNotEnough].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                return false;
            }

            Player.ControllerGood.Consume(data.goodKey, costLv.cost);

            Player.StatusUpgrade.TemporaryUpgrade(key,lvupCount);

            UpdateLockedGrade();

            return true;
        }

        private void UpdateStatListView(StatusUpgradeKey key, LvupType lvupType)
        {
            _viewCanvasstatusUpgrade.DetailInfoUpdate(key,lvupType);
            UpdateSlotView(key);
        }

        private void UpdateSlotView(StatusUpgradeKey key)
        {
            int index = (int)key;

            var level = Player.StatusUpgrade.GetLevel(key);
            var maxlevel = Player.StatusUpgrade.GetMaxLevel(key);
            _statusUpgradeListViews[index]
                .SetIcon((int)key)
                .SetLevel(level, maxlevel);
        }

        private void UpdateLockedGrade()
        {
            for(int i=0; i<Player.StatusUpgrade.MaxGradeLevel; i++)
            {
                int level = i;
                bool isUnlockNext = IsMaxLevel(i);
                if(isUnlockNext)
                {
                    StatusLevelslotList[level + 1].lockedObject.gameObject.SetActive(false);
                }
            }
        }

        private bool IsMaxLevel(int gradeLevel)
        {
            bool isMax = true;
            for (int i = 0; i < StaticData.Wrapper.statusUpgradedatas.Length; i++)
            {
                if (StaticData.Wrapper.statusUpgradedatas[i].upGradeGrade == gradeLevel)
                {
                    var maxLevel = Player.StatusUpgrade.GetIsMaxLevel((StatusUpgradeKey)i);
                    if(maxLevel==false)
                    {
                        isMax = false;
                        break;
                    }
                }
            }
            return isMax;
        }
        #endregion
    }

}
