using BlackTree.Core;
using BlackTree.Definition;
using BlackTree.Model;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BlackTree.Bundles
{
    public class AwakeUpgradeSlotView : ViewBase
    {
        [SerializeField] private BTButton _upgradeBtn;
        [SerializeField] private GameObject cantUpgradeObj;
        [SerializeField] private TextMeshProUGUI _textTitle;
        [SerializeField] private TextMeshProUGUI _textDescription;
        [SerializeField] private TextMeshProUGUI _textLevel;
        [SerializeField] private Image _levelSlider;
        [SerializeField] private TextMeshProUGUI _textPrice;
        [SerializeField] private Image _goodIcon;
        [SerializeField] private Image _icon;
        [SerializeField] private GameObject _lockPanelForMaxLevel;
        [SerializeField] public GameObject reddot;
        [SerializeField] private TextMeshProUGUI levelupTxt;

        private void Awake()
        {
            levelupTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_LevelUp].StringToLocal;
        }
        public AwakeUpgradeSlotView SetName(string text)
        {
            _textTitle.text = text;
            return this;
        }

        public AwakeUpgradeSlotView SetDescription(string text)
        {
            _textDescription.text = text;
            return this;
        }

        public AwakeUpgradeSlotView SetIcon(int index)
        {
            _icon.sprite = UpgradeResourcesBundle.Loaded.AwakeUpSprites[index];
            return this;
        }

    
        public AwakeUpgradeSlotView SetGoodValue(int goodkey,double cost)
        {

            _goodIcon.sprite = GoodResourcesBundle.Loaded.goodSprites[goodkey];
            _textPrice.text = cost.ToNumberString();
            return this;
        }

        public AwakeUpgradeSlotView SetLevel(int _Level,int maxLevel)
        {
            _textLevel.text = $"{_Level}/{maxLevel}";
            _levelSlider.fillAmount = (float)_Level / (float)maxLevel;
            return this;
        }
        public AwakeUpgradeSlotView SetLockForMaxLevel(bool flag)
        {
            _lockPanelForMaxLevel.SetActive(flag);
            return this;
        }

        public AwakeUpgradeSlotView SetOnClickUpgrade(UnityAction onClick)
        {
            _upgradeBtn.onClick.RemoveAllListeners();
            _upgradeBtn.onClick.AddListener(onClick);

            return this;
        }

        public AwakeUpgradeSlotView SetOnClickDownRepeatUpgrade(UnityAction onClick)
        {
            _upgradeBtn.onClickDown.RemoveAllListeners();
            _upgradeBtn.onClickDown.AddListener(onClick);

            return this;
        }
        public AwakeUpgradeSlotView SetOnClickUpUpgrade(UnityAction onClick)
        {
            _upgradeBtn.onClickUp.RemoveAllListeners();
            _upgradeBtn.onClickUp.AddListener(onClick);

            return this;
        }


        public AwakeUpgradeSlotView SetUpgradeBtn(bool flag)
        {
            //_upgradeBtn.enabled = flag;
            cantUpgradeObj.SetActive(!flag);

            return this;
        }

    }

}
