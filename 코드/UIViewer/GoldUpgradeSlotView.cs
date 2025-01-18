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
    public class GoldUpgradeSlotView : ViewBase
    {
        public int Level
        {
            get => _level;
            set
            {
                _level = value;
            }
        }

        [SerializeField] private BTButton _upgradeBtn;
        [SerializeField] private TextMeshProUGUI _textTitle;
        [SerializeField] private TextMeshProUGUI _textDescription;
        [SerializeField] private TextMeshProUGUI _textLevel;
        [SerializeField] private TextMeshProUGUI _textPrice;
        [SerializeField] private Image _goodIcon;
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text levelupTxt;
        [SerializeField] private GameObject _lockPanelForMaxLevel;
        [SerializeField] private TMP_Text maxLevelTxt;
        [SerializeField] private BTButton lockedImage;
        [SerializeField] private GameObject redDot;
        public GameObject arrowObj;
        private int _level;

        private void Start()
        {
            maxLevelTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_maxLevel].StringToLocal;
            levelupTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_LevelUp].StringToLocal;
            //Level = 0;
            Level =1;
        }
      
        public GoldUpgradeSlotView SetName(string text)
        {
            _textTitle.text = text;
            return this;
        }

        public void SetArrowObjectAnim()
        {
            arrowObj.SetActive(true);
        }

        public GoldUpgradeSlotView SetDescription(string text)
        {
            _textDescription.text = text;
            return this;
        }

        public GoldUpgradeSlotView SetIcon(int index,int tier)
        {
            if(tier==0)
            {
                _icon.sprite = UpgradeResourcesBundle.Loaded.GoldUpgradeSprites[index];
            }
            else
            {
                _icon.sprite = UpgradeResourcesBundle.Loaded.Tier2GoldUpgradeSprites[index];
            }
            
            return this;
        }

        public GoldUpgradeSlotView SetLevel(int level)
        {
            Level = level;
            _textLevel.text = string.Format(" <size=15>LV</size>.<color=yellow>{0}</color>", Level);
            return this;
        }

        public GoldUpgradeSlotView SetGoodValue(GoodsValue goodValue)
        {
            int key = (int)goodValue.key;

            _goodIcon.sprite = GoodResourcesBundle.Loaded.goodSprites[key];
            _textPrice.text = goodValue.value.ToNumberString();
            return this;
        }

        public GoldUpgradeSlotView SetLockForMaxLevel(bool flag)
        {
            _lockPanelForMaxLevel.SetActive(flag);
            return this;
        }

        public GoldUpgradeSlotView SetLockImage(bool flag)
        {
            lockedImage.gameObject.SetActive(flag);
            return this;
        }

        public GoldUpgradeSlotView SetOnClickUpgrade(UnityAction onClick)
        {
            _upgradeBtn.onClick.RemoveAllListeners();
            _upgradeBtn.onClick.AddListener(onClick);
            _upgradeBtn.onClick.AddListener(() => arrowObj.SetActive(false));
            return this;
        }
        public GoldUpgradeSlotView SetOnClickDownRepeatUpgrade(UnityAction onClick)
        {
            _upgradeBtn.onClickDown.RemoveAllListeners();
            _upgradeBtn.onClickDown.AddListener(onClick);
            _upgradeBtn.onClick.AddListener(() => arrowObj.SetActive(false));

            return this;
        }
        public GoldUpgradeSlotView SetOnClickUpUpgrade(UnityAction onClick)
        {
            _upgradeBtn.onClickUp.RemoveAllListeners();
            _upgradeBtn.onClickUp.AddListener(onClick);
            _upgradeBtn.onClick.AddListener(() => arrowObj.SetActive(false));

            return this;
        }

        public GoldUpgradeSlotView SetOnClickLockBtn(UnityAction onClick)
        {
            lockedImage.onClick.RemoveAllListeners();
            lockedImage.onClick.AddListener(onClick);

            return this;
        }

        public GoldUpgradeSlotView SetRedDot(bool isActive)
        {
            redDot.SetActive(isActive);
            return this;
        }
   
    }
}
