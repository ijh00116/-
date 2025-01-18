using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using BlackTree.Model;

namespace BlackTree.Bundles
{
    public class ViewAdvanceSlot : ViewBase
    {
        [SerializeField] private Image _icon;
        [SerializeField] private BTButton selecBtn;
        [SerializeField] private GameObject isNotUpgradedObj;
        [SerializeField] private GameObject isSelected;

        public ViewAdvanceSlot SetIcon(int index)
        {
            _icon.sprite = UpgradeResourcesBundle.Loaded.StatUpSprites[index];
            return this;
        }

        public ViewAdvanceSlot SetOnClick(UnityAction onClick)
        {
            selecBtn.onClick.RemoveAllListeners();
            selecBtn.onClick.AddListener(onClick);

            return this;
        }

        public ViewAdvanceSlot SetSelected(bool selected)
        {
            isSelected.SetActive(selected);
            return this;
        }

        public ViewAdvanceSlot SetUpgraded(bool upgraded)
        {
            isNotUpgradedObj.SetActive(!upgraded);
            return this;
        }
    }

}
