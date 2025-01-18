using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BlackTree.Core;
using BlackTree.Model;

namespace BlackTree.Bundles
{
    public class ViewArmorSlot : ViewBase
    {
        public Image ArmorIcon;
        public Image ArmorFrame;
        public Image ArmorInnerFrame;

        public TMP_Text Level;
        public TMP_Text ObtainCount;
        public Slider imageObtainFill;
        public Image[] gradeimages;

        public Image lockedImage;

        public GameObject Equiped;

        protected bool isUnlock = false;

        public BTButton SeeDetailbutton;

        public Player.EquipData ArmorCache;
        public ViewArmorSlot SetHideImage(bool unlocked)
        {
            lockedImage.gameObject.SetActive(unlocked);
            return this;
        }

        public ViewArmorSlot SetLevel(int level)
        {
            Level.text = $"Lv {level}";
            return this;
        }

        public ViewArmorSlot SetObtainCount(float currentObtain)
        {
            ObtainCount.text = $"{(int)currentObtain} / 5";
            imageObtainFill.value = Mathf.Clamp((float)(currentObtain / 5), 0, 1);
            return this;
        }

        public ViewArmorSlot SetEquip(bool equip)
        {
            Equiped.SetActive(equip);
            return this;
        }

        public ViewArmorSlot SetGrade(int _grade)
        {
            var grade = _grade - 1;
            for (int i = 0; i < gradeimages.Length; i++)
            {
                gradeimages[i].gameObject.SetActive(true);
                if (i >= grade)
                {
                    gradeimages[i].gameObject.SetActive(false);
                }
            }

            return this;
        }

        public ViewArmorSlot SetupstaticArmorData(Player.EquipData _ArmorCache)
        {
            ArmorCache = _ArmorCache;
            ArmorIcon.sprite = InGameResourcesBundle.Loaded.armorUIIcon[ArmorCache.tabledata.index];
            ArmorInnerFrame.sprite = InGameResourcesBundle.Loaded.weaponGradeInnerFrameSprite[ArmorCache.tabledata.grade - 1];

            return this;
        }
        public ViewArmorSlot SeeDetail(UnityEngine.Events.UnityAction callback)
        {
            SeeDetailbutton.onClick.AddListener(callback);
            return this;
        }

    }

}
