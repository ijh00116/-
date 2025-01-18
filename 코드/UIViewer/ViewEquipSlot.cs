using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BlackTree.Core;
using BlackTree.Model;

namespace BlackTree.Bundles
{
    public class ViewEquipSlot : ViewBase
    {
        public Image ItemIcon;
        public Image ItemFrame;
        public GameObject focusObject;

        public TMP_Text maxLevel;
        public TMP_Text Level;
        public TMP_Text ObtainCount;
        public Slider imageObtainFill;
        public Image[] gradeimages;

        public BTButton SeeDetailbutton;

        public Image lockedImage;
        public GameObject Equiped;
        protected bool isUnlock = false;

        public GameObject redDot;

        public Player.EquipData equipCache;
        public ViewEquipSlot SetHideImage(bool unlocked)
        {
            ItemIcon.color = unlocked ? Color.white : Color.black;
            lockedImage.gameObject.SetActive(unlocked==false);
            return this;
        }

        public ViewEquipSlot SetLevel(int level)
        {
            Level.text = $"Lv {level}";
            return this;
        }
        public ViewEquipSlot SetMaxLevel(int level)
        {
            maxLevel.text = $"Max.{level}";
            return this;
        }

        public ViewEquipSlot SetObtainCount(Player.EquipData equipdata)
        {
            if(equipdata.IsMaxLevel())
            {
                ObtainCount.text = $"{(int)equipdata.userEquipdata.Obtaincount} /{Player.Equip.CountForCombine}";
                imageObtainFill.value = Mathf.Clamp((float)(equipdata.userEquipdata.Obtaincount / (float)(Player.Equip.CountForCombine)), 0.0f, 1.0f);
                redDot.SetActive(equipdata.userEquipdata.Obtaincount >= Player.Equip.CountForCombine);
            }
            else
            {
                ObtainCount.text = $"{(int)equipdata.userEquipdata.Obtaincount} / {StaticData.Wrapper.itemAmountTableData[equipdata.userEquipdata.Obtainlv].amountForLvUp}";
                imageObtainFill.value = Mathf.Clamp((float)(equipdata.userEquipdata.Obtaincount / (float)(StaticData.Wrapper.itemAmountTableData[equipdata.userEquipdata.Obtainlv].amountForLvUp)), 0.0f, 1.0f);
                redDot.SetActive(equipdata.userEquipdata.Obtaincount >= StaticData.Wrapper.itemAmountTableData[equipdata.userEquipdata.Obtainlv].amountForLvUp);
            }
            
            return this;
        }

        public ViewEquipSlot SetEquip(bool equip)
        {
            Equiped.SetActive(equip);
            return this;
        }

        public ViewEquipSlot SetEquipMydata()
        {
            Equiped.SetActive(equipCache.IsEquiped);
            return this;
        }

        public ViewEquipSlot SetGrade(int _grade)
        {
            for (int i = 0; i < gradeimages.Length; i++)
            {
                gradeimages[i].gameObject.SetActive(true);
                if (i >= _grade)
                {
                    gradeimages[i].gameObject.SetActive(false);
                }
            }

            return this;
        }

        public ViewEquipSlot SetupstaticData(Player.EquipData _equipCache)
        {
            equipCache = _equipCache;
            if (_equipCache.equipType != Definition.EquipType.Weapon)
            {
                ItemIcon.GetComponent<RectTransform>().rotation = Quaternion.identity;
            }
            switch (_equipCache.equipType)
            {
                case Definition.EquipType.Weapon:
                    ItemIcon.sprite = InGameResourcesBundle.Loaded.weaponIcon[equipCache.tabledata.index];
                    break;
                case Definition.EquipType.Armor:
                    ItemIcon.sprite = InGameResourcesBundle.Loaded.armorUIIcon[equipCache.tabledata.index];
                    break;
                case Definition.EquipType.Bow:
                    ItemIcon.sprite = InGameResourcesBundle.Loaded.bowIcon[equipCache.tabledata.index];
                    break;
                case Definition.EquipType.END:
                    break;
                default:
                    break;
            }


            ItemFrame.sprite = InGameResourcesBundle.Loaded.weaponGradeInnerFrameSprite[equipCache.tabledata.grade - 1];

            return this;
        }

        public ViewEquipSlot AddListenerSeeDetail(UnityEngine.Events.UnityAction callback)
        {
            SeeDetailbutton.onClick.AddListener(callback);
            Player.EquipItem.EquipedSlotTouched += SlotTouched;
            return this;
        }

        void SlotTouched(Definition.EquipType _type,int index)
        {
            bool focused = equipCache.equipType == _type && equipCache.tabledata.index == index;
            focusObject.SetActive(focused);
        }


    }

}
