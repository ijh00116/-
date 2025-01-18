using BlackTree.Core;
using BlackTree.Definition;
using BlackTree.Model;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTree.Bundles
{
    public class ViewCanvasStatusUpgrade : ViewCanvas
    {
        public ScrollRect scrollRects;
        public GameObject SlotList;

        public ObtainGood statusObtain;

        public BTSelector lvTypeSelector;
        public BTButton lvup_1;
        public BTButton lvup_10;
        public BTButton lvup_max;

        [Header("detail info window")]
        public GameObject detailWindow;
        public Image iconimage;
        public Slider levelSlider;
        public TMP_Text levelText;
        public TMP_Text upgradeTitle;
        public TMP_Text upgradeDesc;
        public TMP_Text statusPointForLvup;
        public BTButton statUpgradeBtn;

        public TMP_Text tierText;
        public BTButton nextTier;
        public BTButton prevTier;

        public StatusSlotListLevelView slotListLevelViewPrefab;

        [SerializeField] private TMP_Text maxLevelTxt;
        [SerializeField] private TMP_Text levelupTxt;
        public void Init()
        {
            statusObtain.Init();

            Player.ControllerGood.BindOnChange(GoodsKey.StatusPoint, () =>
            {
                UpdateView(GoodsKey.StatusPoint);
            });
            UpdateView(GoodsKey.StatusPoint);

            maxLevelTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_maxLevel].StringToLocal;
            levelupTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_LevelUp].StringToLocal;
        }

        private void UpdateView(GoodsKey key)
        {
            if(key!=GoodsKey.StatusPoint)
            {
                return;
            }

            var value = Player.ControllerGood.GetValue(key);
            statusObtain.StartRoutineUpdateView(value);
        }

        public void Show()
        {
            scrollRects.gameObject.SetActive(true);
            SlotList.SetActive(true);

            detailWindow.SetActive(false);
        }

        public void DetailInfoUpdate(StatusUpgradeKey key, LvupType lvupType)
        {
            detailWindow.SetActive(true);

            GoodsValue _goodValue;
            int index = (int)key;
            var data = StaticData.Wrapper.statusUpgradedatas[index];
            var level = Player.StatusUpgrade.GetLevel(key);
            var maxLevel= Player.StatusUpgrade.GetMaxLevel(key);

            var name = StaticData.Wrapper.localizednamelist[(int)data.nameLmk].StringToLocal;

            int lvupCount = 1;
            Player.StatusUpgrade.StatupgradeValueForLvType costLv = null;
            switch (lvupType)
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

            statusPointForLvup.text = costLv.cost.ToString();

            iconimage.sprite = UpgradeResourcesBundle.Loaded.StatUpSprites[index];
            upgradeTitle.text = name;
            upgradeDesc.text = StatStringFormat(key, StaticData.Wrapper.localizeddesclist[(int)data.descLmk].StringToLocal);
            levelText.text = string.Format("{0}/{1}", level, maxLevel);
            levelSlider.value = (float)level / (float)maxLevel;
        }

        public string StatStringFormat(StatusUpgradeKey key, string description)
        {
            var value = Player.StatusUpgrade.GetValue(key);
            var temp = string.Format(description, value);
            return temp;
        }
    }

}
