using UnityEngine;
using BlackTree.Definition;
using System.Threading;
using Cysharp.Threading.Tasks;
using BlackTree.Bundles;
using BlackTree.Model;
using System.Collections.Generic;

namespace BlackTree.Core
{
    public class ControllerAwakeUpgrade
    {
        private readonly ViewCanvasAwakeUpgrade _viewCanvasAwakeUpgrade;
        private readonly AwakeUpgradeSlotView[] _awakeUpgradeListViews;

        //advancement
        List<ViewAdvanceSlot> advanceSlotList = new List<ViewAdvanceSlot>();

        int currentSelectedAdvanceIndex=0;

        LvupType currentSelectlvUpType;
        public ControllerAwakeUpgrade(Transform parent)
        {
            _viewCanvasAwakeUpgrade = ViewCanvas.Create<ViewCanvasAwakeUpgrade>(parent);
            _viewCanvasAwakeUpgrade.SetDesc(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ETC_InitCommonUpgrade].StringToLocal);

            _awakeUpgradeListViews = new AwakeUpgradeSlotView[StaticData.Wrapper.awakeUpgradedatas.Length];

            for (int i = 0; i < _awakeUpgradeListViews.Length; ++i)
            {
                var key = (AwakeUpgradeKey)i;
                var data = StaticData.Wrapper.awakeUpgradedatas[i];

                _awakeUpgradeListViews[i] = ViewBase.Create<AwakeUpgradeSlotView>(_viewCanvasAwakeUpgrade.scrollRect.content);
                _awakeUpgradeListViews[i].SetOnClickUpgrade(() => { 
                    TryUpgrade(key);
                    CheckAdvancePossible();
                } );
                _awakeUpgradeListViews[i].SetOnClickDownRepeatUpgrade(() => TemporaryUpgrade(key));
                _awakeUpgradeListViews[i].SetOnClickUpUpgrade(() => {
                    Player.AwakeUpgrade.UpdateSyncData();
                    CheckAdvancePossible();
                } );
            }

            for (int i = 0; i < _awakeUpgradeListViews.Length; ++i)
            {
                var key = (AwakeUpgradeKey)i;

                UpdateListView(key);
            }

            _viewCanvasAwakeUpgrade.awakestoneObtain.Init();

            Player.ControllerGood.BindOnChange(GoodsKey.AwakeStone, () =>
            {
                UpdateView(GoodsKey.AwakeStone);
                for (int i = 0; i < _awakeUpgradeListViews.Length; ++i)
                {
                    var key = (AwakeUpgradeKey)i;
                    UpdateBtninListView(key);
                }
            });
            UpdateView(GoodsKey.AwakeStone);
            Player.AwakeUpgrade.UpdateSyncactions += (o) =>
            {
                for (int i = 0; i < _awakeUpgradeListViews.Length; ++i)
                {
                    var key = (AwakeUpgradeKey)i;

                    UpdateListView(key);
                }
            };

            //advance
            _viewCanvasAwakeUpgrade.advanceBtn.onClick.AddListener(OpenAdvancementWindow);
            for(int i=0; i< _viewCanvasAwakeUpgrade.advanceWindowCloseBtns.Length; i++)
            {
                _viewCanvasAwakeUpgrade.advanceWindowCloseBtns[i].onClick.AddListener(CloseAdvancementWindow);
            }
            CheckAllMaxLevel();
   
            for(int i=0; i<StaticData.Wrapper.advancementDatas.Length; i++)
            {
                var parentObject = _viewCanvasAwakeUpgrade.slotParentList[StaticData.Wrapper.advancementDatas[i].grade];

                advanceSlotList.Add(ViewBase.Create<ViewAdvanceSlot>(parentObject));
                int index = i;
                advanceSlotList[i].SetOnClick(() => {
                    currentSelectedAdvanceIndex = index;
                    _viewCanvasAwakeUpgrade.DetailInfoUpdate(StaticData.Wrapper.advancementDatas[index].grade, index);
                    for(int j=0; j< advanceSlotList.Count; j++)
                    {
                        advanceSlotList[j].SetSelected(false);
                    }
                    advanceSlotList[index].SetSelected(true);
                    CheckAdvanceBtnType();
                });
                advanceSlotList[i].SetIcon(index);
                UpdateSlotView(index);
            }

            _viewCanvasAwakeUpgrade.AdvancementProgressBtn.onClick.AddListener(AdvanceUpgrade);
            _viewCanvasAwakeUpgrade.changeAdvanceProgressBtn.onClick.AddListener(ChangeAdvanceUpgrade);
            _viewCanvasAwakeUpgrade.changeAdvanceProgressBtn.gameObject.SetActive(false);

            Player.AwakeUpgrade.onAfterAdvanceUpgrade += UpdateAllslotView;
            Player.AwakeUpgrade.onAfterAdvanceChanged += UpdateAdvanceChanged;

            int bestIndex = 0;
            for(int i=0; i<Player.Cloud.userAdvancedata.AdvanceInfo.Count; i++)
            {
                if(Player.Cloud.userAdvancedata.AdvanceInfo[i].isAdvanced)
                {
                    bestIndex = i;
                }
            }
            currentSelectedAdvanceIndex = bestIndex;
            _viewCanvasAwakeUpgrade.DetailInfoUpdate(StaticData.Wrapper.advancementDatas[currentSelectedAdvanceIndex].grade, currentSelectedAdvanceIndex);
            for (int j = 0; j < advanceSlotList.Count; j++)
            {
                advanceSlotList[j].SetSelected(false);
            }
            advanceSlotList[currentSelectedAdvanceIndex].SetSelected(true);

            _viewCanvasAwakeUpgrade.changeBtninDetail.onClick.AddListener(()=> {
                if (Player.Good.Get(GoodsKey.Dia)> advanceChangeDiaCost)
                {
                    Player.ControllerGood.Consume(GoodsKey.Dia, advanceChangeDiaCost);
                    Player.AwakeUpgrade.ChangeAdvance(currentSelectedAdvanceIndex);
                }
            });

            for(int i=0; i< _viewCanvasAwakeUpgrade.closeDetailBtn.Length; i++)
            {
                _viewCanvasAwakeUpgrade.closeDetailBtn[i].onClick.AddListener(() => {
                    _viewCanvasAwakeUpgrade.advanceChangeWindow.SetActive(false);
                });
            }

            _viewCanvasAwakeUpgrade.lvup_1.onClick.AddListener(() => { LvupTypeSelect(LvupType.lvup_1); });
            _viewCanvasAwakeUpgrade.lvup_10.onClick.AddListener(() => { LvupTypeSelect(LvupType.lvup_10); });
            _viewCanvasAwakeUpgrade.lvup_max.onClick.AddListener(() => { LvupTypeSelect(LvupType.lvup_Max); });

            LvupTypeSelect(LvupType.lvup_1);
            _viewCanvasAwakeUpgrade.arrowObject.gameObject.SetActive(false);
            CheckAdvanceBtnType();

            Main().Forget();

            _viewCanvasAwakeUpgrade.advanceBtnTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Advance].StringToLocal;
            _viewCanvasAwakeUpgrade.advanceWindowTitleTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Advance].StringToLocal;
            _viewCanvasAwakeUpgrade.advanceWindowDescTxt.text = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_NeedToAwake].StringToLocal;
            _viewCanvasAwakeUpgrade.advanceWindowBtnTxt.text = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_Advance].StringToLocal;
        }

        void LvupTypeSelect(LvupType lvupType)
        {
            if (currentSelectlvUpType == lvupType)
                return;
            currentSelectlvUpType = lvupType;

            switch (lvupType)
            {
                case LvupType.lvup_1:
                    _viewCanvasAwakeUpgrade.lvTypeSelector.Show(0);
                    break;
                case LvupType.lvup_10:
                    _viewCanvasAwakeUpgrade.lvTypeSelector.Show(1);
                    break;
                case LvupType.lvup_Max:
                    _viewCanvasAwakeUpgrade.lvTypeSelector.Show(2);
                    break;
                default:
                    break;
            }

            for (int i = 0; i < _awakeUpgradeListViews.Length; ++i)
            {
                var key = (AwakeUpgradeKey)i;

                UpdateListView(key);
            }
        }

        async UniTaskVoid Main()
        {
            while (true)
            {
                Player.AwakeUpgrade.canUpgrade = false;
                for (int i = 0; i < _awakeUpgradeListViews.Length; ++i)
                {
                    var key = (AwakeUpgradeKey)i;
                    var data = StaticData.Wrapper.awakeUpgradedatas[i];

                    var cost = Player.AwakeUpgrade.GetCost(key);
                    bool reddot = Player.ControllerGood.IsCanBuy(data.goodKey, cost);
                    _awakeUpgradeListViews[i].reddot.SetActive(reddot);
                    if (reddot)
                    {
                        Player.AwakeUpgrade.canUpgrade = true;
                    }
                }
                bool isRedDot = Model.Player.AwakeUpgrade.canUpgrade || Model.Player.GoldUpgrade.canUpgrade || Model.Player.StatusUpgrade.canUpgrade;
                ViewCanvas.Get<ViewCanvasMainNav>().viewMainNavButtons[(int)MainNavigationType.Upgrade].ActivateNotification(isRedDot);

                await UniTask.DelayFrame(60);
            }
        }

        private void UpdateView(GoodsKey key)
        {
            if (key != GoodsKey.AwakeStone)
            {
                return;
            }

            var value = Player.ControllerGood.GetValue(key);
            _viewCanvasAwakeUpgrade.awakestoneObtain.StartRoutineUpdateView(value);
        }

        private bool TryUpgrade(AwakeUpgradeKey key)
        {
            var index = (int)key;
            var isMaxLevel = Player.AwakeUpgrade.GetIsMaxLevel(key);
            var data = StaticData.Wrapper.awakeUpgradedatas[index];

            if (isMaxLevel) return false;


            int lvupCount = 1;
            Player.AwakeUpgrade.AwakeupgradeValueForLvType costLv = null;
            switch (currentSelectlvUpType)
            {
                case LvupType.lvup_1:
                    costLv = Player.AwakeUpgrade.GetCostLv(key, 1);
                    lvupCount = costLv.lv - Player.AwakeUpgrade.GetLevel(key);
                    break;
                case LvupType.lvup_10:
                    costLv = Player.AwakeUpgrade.GetCostLv(key, 10);
                    lvupCount = costLv.lv - Player.AwakeUpgrade.GetLevel(key);
                    break;
                case LvupType.lvup_Max:
                    costLv = Player.AwakeUpgrade.GetMaxCostLv(key);
                    lvupCount = costLv.lv - Player.AwakeUpgrade.GetLevel(key);
                    break;
                default:
                    break;
            }

            if (!Player.ControllerGood.IsCanBuy(data.goodKey, costLv.cost) || lvupCount <= 0)
            {
                string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_AwakeStoneNotEnough].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                return false;
            }

            Player.ControllerGood.Consume(data.goodKey, costLv.cost);
            Player.AwakeUpgrade.Upgrade(key, lvupCount);

            CheckAllMaxLevel();

            return true;
        }

        private void CheckAdvancePossible()
        {
            if(CheckAllMaxLevel())
            {
                _viewCanvasAwakeUpgrade.SetArrowObject();
            }
        }

        private bool TemporaryUpgrade(AwakeUpgradeKey key)
        {
            var index = (int)key;
            var isMaxLevel = Player.AwakeUpgrade.GetIsMaxLevel(key);
            var data = StaticData.Wrapper.awakeUpgradedatas[index];

            if (isMaxLevel) return false;

            int lvupCount = 1;
            Player.AwakeUpgrade.AwakeupgradeValueForLvType costLv = null;
            switch (currentSelectlvUpType)
            {
                case LvupType.lvup_1:
                    costLv = Player.AwakeUpgrade.GetCostLv(key, 1);
                    lvupCount = costLv.lv - Player.AwakeUpgrade.GetLevel(key);
                    break;
                case LvupType.lvup_10:
                    costLv = Player.AwakeUpgrade.GetCostLv(key, 10);
                    lvupCount = costLv.lv - Player.AwakeUpgrade.GetLevel(key);
                    break;
                case LvupType.lvup_Max:
                    costLv = Player.AwakeUpgrade.GetMaxCostLv(key);
                    lvupCount = costLv.lv - Player.AwakeUpgrade.GetLevel(key);
                    break;
                default:
                    break;
            }



            if (!Player.ControllerGood.IsCanBuy(data.goodKey, costLv.cost) || lvupCount <= 0)
            {
                string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_AwakeStoneNotEnough].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                return false;
            }

            Player.ControllerGood.Consume(data.goodKey, costLv.cost);
            Player.AwakeUpgrade.TemporaryUpgrade(key, lvupCount);

            CheckAdvancePossible();
            return true;
        }


        private void UpdateListView(AwakeUpgradeKey key)
        {
            int index = (int)key;

            var data = StaticData.Wrapper.awakeUpgradedatas[index];
            var level = Player.AwakeUpgrade.GetLevel(key);
            var maxlevel = Player.AwakeUpgrade.GetMaxLevel(key);
            var value = Player.AwakeUpgrade.GetValue(key);

            var name = StaticData.Wrapper.localizednamelist[(int)data.nameLmk].StringToLocal;
            var desc = StaticData.Wrapper.localizeddesclist[(int)data.descLmk].StringToLocal;

            Player.AwakeUpgrade.AwakeupgradeValueForLvType costLv = null;
            switch (currentSelectlvUpType)
            {
                case LvupType.lvup_1:
                    costLv = Player.AwakeUpgrade.GetCostLv(key, 1);
                    break;
                case LvupType.lvup_10:
                    costLv = Player.AwakeUpgrade.GetCostLv(key, 10);
                    desc= desc+ string.Format(StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.UI_GoldUpgradeDesc].StringToLocal, costLv.lv, Player.AwakeUpgrade.GetValue(key, costLv.lv).ToNumberString());
                    break;
                case LvupType.lvup_Max:
                    costLv = Player.AwakeUpgrade.GetMaxCostLv(key);
                    desc = desc + string.Format(StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.UI_GoldUpgradeDesc].StringToLocal, costLv.lv, Player.AwakeUpgrade.GetValue(key, costLv.lv).ToNumberString());
                    break;
                default:
                    break;
            }


            bool canUpgrade = Player.AwakeUpgrade.GetIsMaxLevel(key) == false && Player.Good.Get(GoodsKey.AwakeStone) >= costLv.cost;

            _awakeUpgradeListViews[index]
                .SetName(name)
                .SetDescription(string.Format(desc, value))
                .SetIcon((int)key)
                .SetGoodValue((int)GoodsKey.AwakeStone, costLv.cost)
                .SetLevel(level, maxlevel)
                .SetUpgradeBtn(canUpgrade)
                .SetLockForMaxLevel(Player.AwakeUpgrade.GetIsMaxLevel(key));
        }

        private void UpdateBtninListView(AwakeUpgradeKey key)
        {
            int index = (int)key;
            double cost = Player.AwakeUpgrade.GetCost(key);
            bool canUpgrade = Player.AwakeUpgrade.GetIsMaxLevel(key) == false && Player.Good.Get(GoodsKey.AwakeStone) >= cost;

            _awakeUpgradeListViews[index]
                .SetUpgradeBtn(canUpgrade);
        }

        // advancement
        bool canAdvance = true;
        private bool CheckAllMaxLevel()
        {
            bool ismaxlv = true;
            for(int i=0; i< StaticData.Wrapper.awakeUpgradedatas.Length; i++)
            {
                AwakeUpgradeKey key = StaticData.Wrapper.awakeUpgradedatas[i].key;
                ismaxlv = Player.AwakeUpgrade.GetIsMaxLevel(key);
                if (ismaxlv == false)
                    break;
            }
            canAdvance = ismaxlv;

            return ismaxlv;
        }

        private void CheckAdvanceBtnType()
        {
            bool isUpgrade = StaticData.Wrapper.advancementDatas[currentSelectedAdvanceIndex].grade <= Player.Cloud.userAdvancedata.Grade;

            bool AlreadyUpgrade = false;
            for(int i=0; i< StaticData.Wrapper.advancementDatas.Length; i++)
            {
                if(StaticData.Wrapper.advancementDatas[currentSelectedAdvanceIndex].grade== StaticData.Wrapper.advancementDatas[i].grade)
                {
                    if(Player.Cloud.userAdvancedata.AdvanceInfo[i].isAdvanced)
                    {
                        AlreadyUpgrade = true;
                        break;
                    }
                }
            }


            if (isUpgrade)
            {
                if(AlreadyUpgrade)
                {
                    if(Player.Cloud.userAdvancedata.AdvanceInfo[currentSelectedAdvanceIndex].isAdvanced)
                    {
                        _viewCanvasAwakeUpgrade.AdvancementProgressBtn.gameObject.SetActive(false);
                        _viewCanvasAwakeUpgrade.changeAdvanceProgressBtn.gameObject.SetActive(false);
                    }
                    else
                    {
                        _viewCanvasAwakeUpgrade.AdvancementProgressBtn.gameObject.SetActive(false);
                        _viewCanvasAwakeUpgrade.changeAdvanceProgressBtn.gameObject.SetActive(true);
                    }
                }
                else
                {
                    _viewCanvasAwakeUpgrade.AdvancementProgressBtn.gameObject.SetActive(true);
                    _viewCanvasAwakeUpgrade.changeAdvanceProgressBtn.gameObject.SetActive(false);
                }
            }
            else
            {
                _viewCanvasAwakeUpgrade.AdvancementProgressBtn.gameObject.SetActive(false);
                _viewCanvasAwakeUpgrade.changeAdvanceProgressBtn.gameObject.SetActive(false);
            }
        }
      

        private void OpenAdvancementWindow()
        {
            //if(canAdvance==false)
            //{
            //    Debug.Log("각성레벨이 부족하여 승급이 안됩니다.");
            //    return;
            //}

            _viewCanvasAwakeUpgrade.advancementObject.gameObject.SetActive(true);
            _viewCanvasAwakeUpgrade.advancementBG.PopupOpenColorFade();
            _viewCanvasAwakeUpgrade.advancementWindow.CommonPopupOpenAnimationUp();
        }

        private void CloseAdvancementWindow()
        {
            _viewCanvasAwakeUpgrade.advancementBG.PopupCloseColorFade();
            _viewCanvasAwakeUpgrade.advancementWindow.CommonPopupCloseAnimationDown(() => {
                _viewCanvasAwakeUpgrade.advancementObject.gameObject.SetActive(false);
            });
        }

        public void UpdateSlotView(int index)
        {
            advanceSlotList[index].SetUpgraded(Player.Cloud.userAdvancedata.AdvanceInfo[index].isAdvanced);
        }

        public void UpdateAllslotView(int index)
        {
            for(int i=0; i< advanceSlotList.Count; i++)
            {
                advanceSlotList[i].SetUpgraded(Player.Cloud.userAdvancedata.AdvanceInfo[i].isAdvanced);
            }

            for (int i = 0; i < _awakeUpgradeListViews.Length; ++i)
            {
                var key = (AwakeUpgradeKey)i;

                UpdateListView(key);
            }
        }

        public void UpdateAdvanceChanged(int index)
        {
            string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_AdvanceChanged].StringToLocal;
            ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);

            _viewCanvasAwakeUpgrade.advanceChangeWindow.SetActive(false);
        }

        public void AdvanceUpgrade()
        {
            bool canGradeUpgrade = CheckAllMaxLevel();
            
            if(canGradeUpgrade)
            {
             
                Player.AwakeUpgrade.Advance(currentSelectedAdvanceIndex);

                Player.Unit.syncHpUI?.Invoke();

                _viewCanvasAwakeUpgrade.OffArrowObject();
            }
            else
            {
                string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_AwakeLvNotEnough].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);

                //Debug.Log("각성레벨을 최대로 달성하세요");
            }
            
        }
        public void ChangeAdvanceUpgrade()
        {
            OpenChangeAdvance();
          
        }

        int advanceChangeDiaCost = 0;
        void OpenChangeAdvance()
        {
            _viewCanvasAwakeUpgrade.advanceChangeWindow.SetActive(true);
            int currentGrade = StaticData.Wrapper.advancementDatas[currentSelectedAdvanceIndex].grade;
            advanceChangeDiaCost = 1000;
            if(currentGrade==1)
            {
                advanceChangeDiaCost = 1000;
            }
            else if (currentGrade == 3)
            {
                advanceChangeDiaCost = 2000;
            }
            _viewCanvasAwakeUpgrade.changeImage.sprite = UpgradeResourcesBundle.Loaded.StatUpSprites[currentSelectedAdvanceIndex];
            _viewCanvasAwakeUpgrade.desc.text =string.Format(StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Advance_ChangeDesc].StringToLocal, advanceChangeDiaCost);
            _viewCanvasAwakeUpgrade.changeCost.text = advanceChangeDiaCost.ToString();


        }
    }
}

