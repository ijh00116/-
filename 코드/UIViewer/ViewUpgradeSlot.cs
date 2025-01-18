using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using BlackTree.Model;

namespace BlackTree.Bundles
{
    public class ViewUpgradeSlot : ViewBase
    {
        public int Level
        {
            get => _level;
            set
            {
                _level = value;
            }
        }
        [SerializeField] private Slider levelSlider;
        [SerializeField] private TextMeshProUGUI _textLevel;
        [SerializeField] private Image _icon;
        [SerializeField] private BTButton selecBtn;
        private int _level;
        

        public ViewUpgradeSlot SetIcon(int index)
        {
            _icon.sprite = UpgradeResourcesBundle.Loaded.StatUpSprites[index];
            return this;
        }

        public ViewUpgradeSlot SetLevel(int level,int maxLv)
        {
            Level = level;
            _textLevel.text = string.Format("{0}/{1}", level, maxLv);
            levelSlider.value = (float)level / (float)maxLv;
            return this;
        }

        public ViewUpgradeSlot SetOnClickUpgrade(UnityAction onClick)
        {
            selecBtn.onClick.RemoveAllListeners();
            selecBtn.onClick.AddListener(onClick);

            return this;
        }
    }
}